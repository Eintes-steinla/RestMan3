USE master

-- RestMan3 - ~40 tables
-- Tác giả: Deku
-- Chú thích: Tệp bao gồm chú thích đầu cuối rõ ràng
-- Thiết kế cho Microsoft SQL Server

-- Tạo cơ sở dữ liệu nếu chưa tồn tại
IF DB_ID('RestMan3') IS NULL
BEGIN
    CREATE DATABASE RestMan3;
END
GO
USE RestMan3;
GO

/* ======================================================================================
   SECTION: Business / Multi-tenant
   - BusinessOwner: lưu thông tin chủ doanh nghiệp (dùng cho mô hình SaaS hoặc nhiều cửa hàng)
     OwnerID: PK

   - Store: đại diện cho 1 'gian hàng' / thương hiệu trong hệ thống
     StoreID: PK
     OwnerID: FK tới BusinessOwner

   - Branch: chi nhánh cụ thể (có thể là địa điểm vật lý)
     BranchID: PK
     StoreID: FK tới Store

   Lý do tách: hỗ trợ nhiều cửa hàng / nhiều chi nhánh, quản lý phân quyền, báo cáo theo chi nhánh.
   ====================================================================================== */
CREATE TABLE BusinessOwner(
    OwnerID INT IDENTITY(1,1) PRIMARY KEY,
    OwnerName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15),
    Email NVARCHAR(50),
    Country NVARCHAR(100),
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME()
);

CREATE TABLE [Store](
    StoreID INT IDENTITY(1,1) PRIMARY KEY,
    OwnerID INT NOT NULL,
    StoreName NVARCHAR(100) NOT NULL,
    Industry NVARCHAR(100), -- Restaurant, Cafe, Bar, etc
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Store_Owner FOREIGN KEY(OwnerID) REFERENCES BusinessOwner(OwnerID)
);

CREATE TABLE Branch(
    BranchID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NOT NULL,
    BranchName NVARCHAR(100) NOT NULL,
    [Address] NVARCHAR(300),
    Phone NVARCHAR(15),
    Country NVARCHAR(100),
    Timezone NVARCHAR(50),
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Branch_Store FOREIGN KEY(StoreID) REFERENCES [Store](StoreID)
);

/* ======================================================================================
   SECTION: Users, Roles, Permissions
   - Role: định nghĩa vai trò (Admin, Manager, Staff, Customer,..)
   - Permission: danh sách quyền chi tiết (nếu cần granular permission)
   - RolePermission: liên kết nhiều-nhiều giữa Role và Permission
   - UserAccount: tài khoản dùng để đăng nhập. Lưu username/password hash, role
       - StoreID nullable để hỗ trợ tài khoản cấp độ owner (không thuộc store cụ thể)
   - Employee: lưu thông tin nhân viên (chi tiết hơn UserAccount)
   - Customer: lưu thông tin khách hàng (membership, điểm thưởng) riêng với employee
   - AccountProfileLink: liên kết 1-1 giữa UserAccount và Employee hoặc Customer
     (để giữ nguyên tắc separation of concerns giữa auth và profile)

   Lý do tách: avoids nullable columns and mixing unrelated fields, dễ maintenance.
   ====================================================================================== */
CREATE TABLE [Role](
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500)
);

CREATE TABLE Permission(
    PermissionID INT IDENTITY(1,1) PRIMARY KEY,
    PermissionKey NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(500)
);

CREATE TABLE RolePermission(
    RoleID INT NOT NULL,
    PermissionID INT NOT NULL,
    CONSTRAINT PK_RolePermission PRIMARY KEY(RoleID, PermissionID),
    CONSTRAINT FK_RP_Role FOREIGN KEY(RoleID) REFERENCES Role(RoleID),
    CONSTRAINT FK_RP_Perm FOREIGN KEY(PermissionID) REFERENCES Permission(PermissionID)
);

CREATE TABLE UserAccount(
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NULL, -- which store this account belongs to (nullable for owner-level)
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    Email NVARCHAR(50),
    Phone NVARCHAR(15),
    RoleID INT NOT NULL,
    IsActive BIT DEFAULT 1,
	IsDelete BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_User_Role FOREIGN KEY(RoleID) REFERENCES Role(RoleID),
    CONSTRAINT FK_User_Store FOREIGN KEY(StoreID) REFERENCES [Store](StoreID)
);

-- Employee: chi tiết nhân sự. Lưu ý: EmployeeID khác UserID để tách auth/profile
CREATE TABLE Employee(
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NOT NULL,
    BranchID INT NULL,
    FullName NVARCHAR(100) NOT NULL,
    DOB DATE NULL,
    Phone NVARCHAR(15),
    Email NVARCHAR(50),
    HireDate DATE NULL,
    Salary DECIMAL(18,2) NULL,
    RoleDescription NVARCHAR(200) NULL, -- job title
    IsActive BIT DEFAULT 1,
	IsDelete BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Emp_Store FOREIGN KEY(StoreID) REFERENCES [Store](StoreID),
    CONSTRAINT FK_Emp_Branch FOREIGN KEY(BranchID) REFERENCES Branch(BranchID)
);

-- Customer: thông tin khách hàng, membership, điểm tích lũy
CREATE TABLE Customer(
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15),
    Email NVARCHAR(50),
    MembershipLevel NVARCHAR(100) DEFAULT 'Normal',
    Points INT DEFAULT 0,
	IsActive BIT DEFAULT 1,
	IsDelete BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Customer_Store FOREIGN KEY(StoreID) REFERENCES [Store](StoreID)
);

-- Liên kết 1-1 để map UserAccount <-> Employee/Customer
CREATE TABLE AccountProfileLink(
    UserID INT NOT NULL PRIMARY KEY,
    EmployeeID INT NULL,
    CustomerID INT NULL,
    
    -- Foreign Keys
    CONSTRAINT FK_APL_User FOREIGN KEY(UserID) REFERENCES UserAccount(UserID),
    CONSTRAINT FK_APL_Emp FOREIGN KEY(EmployeeID) REFERENCES Employee(EmployeeID),
    CONSTRAINT FK_APL_Cust FOREIGN KEY(CustomerID) REFERENCES Customer(CustomerID)
);

/* ======================================================================================
   SECTION: Tables (bàn), Shifts, Reservation
   - RestaurantTable: lưu trạng thái và sức chứa của bàn
   - WorkShift: định nghĩa ca làm
   - EmployeeShift: phân công nhân viên theo ca
   - Reservation: đặt bàn từ khách hàng

   Ghi chú: TableID liên quan đến BranchID để biết bàn thuộc chi nhánh nào.
   ====================================================================================== */
CREATE TABLE RestaurantTable(
    TableID INT IDENTITY(1,1) PRIMARY KEY,
    BranchID INT NOT NULL,
    TableName NVARCHAR(100) NOT NULL,
    Capacity INT DEFAULT 1,
    [Status] NVARCHAR(50) DEFAULT 'Available',
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Table_Branch FOREIGN KEY(BranchID) REFERENCES Branch(BranchID)
);

CREATE TABLE WorkShift(
    ShiftID INT IDENTITY(1,1) PRIMARY KEY,
    ShiftName NVARCHAR(100),
    StartTime TIME,
    EndTime TIME
);

CREATE TABLE EmployeeShift(
    EmployeeShiftID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT NOT NULL,
    ShiftID INT NOT NULL,
    WorkDate DATE NOT NULL,
    CONSTRAINT FK_ES_Emp FOREIGN KEY(EmployeeID) REFERENCES Employee(EmployeeID),
    CONSTRAINT FK_ES_Shift FOREIGN KEY(ShiftID) REFERENCES WorkShift(ShiftID)
);

CREATE TABLE Reservation(
    ReservationID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NULL,
    BranchID INT NULL,
    CustomerID INT NULL,
    TableID INT NULL,
    ReservationTime DATETIME2 NOT NULL,
    Seats INT DEFAULT 1,
    [Status] NVARCHAR(50) DEFAULT 'Pending',
    Note NVARCHAR(1000),
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Res_Store FOREIGN KEY(StoreID) REFERENCES [Store](StoreID),
    CONSTRAINT FK_Res_Branch FOREIGN KEY(BranchID) REFERENCES Branch(BranchID),
    CONSTRAINT FK_Res_Cust FOREIGN KEY(CustomerID) REFERENCES Customer(CustomerID),
    CONSTRAINT FK_Res_Table FOREIGN KEY(TableID) REFERENCES RestaurantTable(TableID)
);

/* ======================================================================================
   SECTION: Menu / Items / Category
   - MenuCategory: phân loại món (kebab, main, drinks...)
   - MenuItem: danh sách tất cả món/đồ uống nhà hàng có thể chế biến
       - Price: giá bán, CostPrice: giá vốn nếu muốn tính lãi
   - MenuAvailable: optional, nếu bạn muốn bật/tắt món theo ngày/chi nhánh

   Lưu ý: Bạn có thể không sử dụng MenuAvailable nếu muốn hiển thị tất cả
   ====================================================================================== */
CREATE TABLE MenuCategory(
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NULL,
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500)
);

CREATE TABLE MenuItem(
    ItemID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NULL,
    ItemName NVARCHAR(100) NOT NULL,
    CategoryID INT NULL,
    Unit NVARCHAR(50),
    Price DECIMAL(18,2) NOT NULL DEFAULT 0,
    CostPrice DECIMAL(18,2) NULL,
    IsActive BIT DEFAULT 1,
	IsDelete BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Item_Category FOREIGN KEY(CategoryID) REFERENCES MenuCategory(CategoryID)
);

CREATE TABLE MenuAvailable(
    MenuAvailableID INT IDENTITY(1,1) PRIMARY KEY,
    ItemID INT NOT NULL,
    BranchID INT NULL,
    AvailableDate DATE NULL, -- NULL => always available
    IsAvailable BIT DEFAULT 1,
	-- IsActive BIT DEFAULT 1,
	IsDelete BIT DEFAULT 0,
    CONSTRAINT FK_MA_Item FOREIGN KEY(ItemID) REFERENCES MenuItem(ItemID),
    CONSTRAINT FK_MA_Branch FOREIGN KEY(BranchID) REFERENCES Branch(BranchID)
);

/* ======================================================================================
   SECTION: Orders / OrderDetail / Payment
   - Order: ghi nhận thông tin đơn (cả tại chỗ hoặc take-away/delivery)
       - có BranchID, TableID, EmployeeID để biết ai tiếp nhận
   - OrderDetail: chi tiết các món trong 1 order (quantity, unitprice, discount)
       - Total: computed column để tránh lỗi tính toán trên client
   - PaymentMethod: danh sách các phương thức thanh toán (dùng để chuẩn hoá dữ liệu)

   Ghi chú: Order.Status nên được quản lý cẩn thận (Pending -> Serving -> Paid -> Closed)
   ====================================================================================== */
CREATE TABLE [Order](
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NULL,
    BranchID INT NULL,
    TableID INT NULL,
    CustomerID INT NULL,
    EmployeeID INT NULL, -- who took the order
    OrderTime DATETIME2 DEFAULT SYSUTCDATETIME(),
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Serving, Paid, Cancelled
    TotalAmount DECIMAL(18,2) DEFAULT 0,
    PaymentMethod NVARCHAR(100),
    Note NVARCHAR(1000),
	IsActive BIT DEFAULT 1,
	IsDelete BIT DEFAULT 0,
    CONSTRAINT FK_Order_Branch FOREIGN KEY(BranchID) REFERENCES Branch(BranchID),
    CONSTRAINT FK_Order_Table FOREIGN KEY(TableID) REFERENCES RestaurantTable(TableID),
    CONSTRAINT FK_Order_Customer FOREIGN KEY(CustomerID) REFERENCES Customer(CustomerID),
    CONSTRAINT FK_Order_Employee FOREIGN KEY(EmployeeID) REFERENCES Employee(EmployeeID)
);

CREATE TABLE OrderDetail(
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    ItemID INT NOT NULL,
    Quantity INT DEFAULT 1,
    UnitPrice DECIMAL(18,2) NOT NULL,
    Discount DECIMAL(18,2) DEFAULT 0,
    Total AS (Quantity * UnitPrice - Discount) PERSISTED,
	IsActive BIT DEFAULT 1,
	IsDelete BIT DEFAULT 0,
    CONSTRAINT FK_OD_Order FOREIGN KEY(OrderID) REFERENCES [Order](OrderID),
    CONSTRAINT FK_OD_Item FOREIGN KEY(ItemID) REFERENCES MenuItem(ItemID)
);

CREATE TABLE PaymentMethod(
    PaymentMethodID INT IDENTITY(1,1) PRIMARY KEY,
    MethodName NVARCHAR(100) NOT NULL -- Cash, BankTransfer, Card, eWallet
);

/* ======================================================================================
   SECTION: Inventory / Warehouse
   - Supplier: nha cung cap
   - InventoryItem: danh sách nguyên vật liệu / hàng tồn kho (không phải MenuItem)
       - ItemName, CurrentStock, MinStock để quản lý tồn
   - InventoryReceipt: phiếu nhập kho (một phiếu có thể chứa nhiều mặt hàng)
   - InventoryReceiptDetail: chi tiết từng dòng hàng trong phiếu nhập (Quantity, UnitCost)
   - InventoryIssue: phiếu xuất kho (ví dụ xuất để chế biến hoặc dùng nội bộ)
   - InventoryIssueDetail: chi tiết phiếu xuất

   Chi tiết: Bảng Detail cho phép 1-to-many: 1 Receipt -> n ReceiptDetail
   ====================================================================================== */
CREATE TABLE Supplier(
    SupplierID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15),
    Email NVARCHAR(50),
    Address NVARCHAR(500)
);

CREATE TABLE InventoryItem(
    InventoryItemID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NULL,
    ItemName NVARCHAR(100) NOT NULL,
    Unit NVARCHAR(50),
    Category NVARCHAR(200),
    CurrentStock DECIMAL(18,3) DEFAULT 0,
    MinStock DECIMAL(18,3) DEFAULT 0,
    CostPrice DECIMAL(18,2) DEFAULT 0,
    IsActive BIT DEFAULT 1
);

CREATE TABLE InventoryReceipt(
    ReceiptID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NULL,
    BranchID INT NULL,
    SupplierID INT NULL,
    EmployeeID INT NULL,
    ReceiptDate DATETIME2 DEFAULT SYSUTCDATETIME(),
    TotalCost DECIMAL(18,2) DEFAULT 0,
    Note NVARCHAR(1000),
    CONSTRAINT FK_IR_Supplier FOREIGN KEY(SupplierID) REFERENCES Supplier(SupplierID),
    CONSTRAINT FK_IR_Emp FOREIGN KEY(EmployeeID) REFERENCES Employee(EmployeeID)
);

CREATE TABLE InventoryReceiptDetail(
    IRDetailID INT IDENTITY(1,1) PRIMARY KEY,
    ReceiptID INT NOT NULL,
    InventoryItemID INT NOT NULL,
    Quantity DECIMAL(18,3) NOT NULL,
    UnitCost DECIMAL(18,2) NOT NULL,
    LineTotal AS (Quantity * UnitCost) PERSISTED,
    CONSTRAINT FK_IRD_Receipt FOREIGN KEY(ReceiptID) REFERENCES InventoryReceipt(ReceiptID),
    CONSTRAINT FK_IRD_Item FOREIGN KEY(InventoryItemID) REFERENCES InventoryItem(InventoryItemID)
);

CREATE TABLE InventoryIssue(
    IssueID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NULL,
    BranchID INT NULL,
    EmployeeID INT NULL,
    IssueDate DATETIME2 DEFAULT SYSUTCDATETIME(),
    Note NVARCHAR(1000),
    CONSTRAINT FK_II_Emp FOREIGN KEY(EmployeeID) REFERENCES Employee(EmployeeID)
);

CREATE TABLE InventoryIssueDetail(
    IIDetailID INT IDENTITY(1,1) PRIMARY KEY,
    IssueID INT NOT NULL,
    InventoryItemID INT NOT NULL,
    Quantity DECIMAL(18,3) NOT NULL,
    CONSTRAINT FK_IID_Issue FOREIGN KEY(IssueID) REFERENCES InventoryIssue(IssueID),
    CONSTRAINT FK_IID_Item FOREIGN KEY(InventoryItemID) REFERENCES InventoryItem(InventoryItemID)
);

/* ======================================================================================
   SECTION: Finance / Cash / Loans
   - FinancialCategory: phân loại các khoản thu/chi (tiền mặt, chi phí marketing, tiền lương, thuế...)
   - CashTransaction: ghi nhận dòng tiền (thu hoặc chi) liên quan tới store/branch
   - Loan & LoanPayment: quản lý khoản vay và các lần trả nợ

   Ghi chú: CashTransaction có thể liên kết tới OrderID/ReceiptID nếu muốn tracking chính xác.
   ====================================================================================== */
CREATE TABLE FinancialCategory(
    FinancialCategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    Type NVARCHAR(50) DEFAULT 'Expense' -- Income / Expense
);

CREATE TABLE CashTransaction(
    TransactionID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NULL,
    BranchID INT NULL,
    FinancialCategoryID INT NULL,
    EmployeeID INT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    TransactionType NVARCHAR(50) NOT NULL, -- Income / Expense
    PaymentMethod NVARCHAR(100),
    TransactionDate DATETIME2 DEFAULT SYSUTCDATETIME(),
    Note NVARCHAR(1000),
    CONSTRAINT FK_CT_FinCat FOREIGN KEY(FinancialCategoryID) REFERENCES FinancialCategory(FinancialCategoryID)
);

CREATE TABLE [Loan](
    LoanID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NULL,
    Lender NVARCHAR(300) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    InterestRate DECIMAL(5,2) NULL,
    StartDate DATE,
    EndDate DATE,
    Status NVARCHAR(50) DEFAULT 'Active'
);

CREATE TABLE LoanPayment(
    LoanPaymentID INT IDENTITY(1,1) PRIMARY KEY,
    LoanID INT NOT NULL,
    PaymentDate DATE NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Method NVARCHAR(100),
    CONSTRAINT FK_LP_Loan FOREIGN KEY(LoanID) REFERENCES [Loan](LoanID)
);

/* ======================================================================================
   SECTION: Settings / Notifications / Integrations
   - Setting: key/value global settings
   - LanguageSetting: hỗ trợ đa ngôn ngữ (ví dụ vi/en)
   - Notification: lưu log thông báo cho người dùng/chi nhánh
   - AppIntegration: cấu hình liên kết nền tảng giao hàng, chatbot, v.v.
   ====================================================================================== */
CREATE TABLE [Setting](
    SettingKey NVARCHAR(200) PRIMARY KEY,
    SettingValue NVARCHAR(MAX),
    [Description] NVARCHAR(1000)
);

CREATE TABLE LanguageSetting(
    LanguageCode NVARCHAR(10) PRIMARY KEY, -- e.g. vi, en
    DisplayName NVARCHAR(200),
    IsDefault BIT DEFAULT 0
);

CREATE TABLE Notification(
    NotificationID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NULL,
    BranchID INT NULL,
    Title NVARCHAR(300),
    Body NVARCHAR(2000),
    Data NVARCHAR(MAX),
    IsRead BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME()
);

CREATE TABLE AppIntegration(
    IntegrationID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NULL,
    [Name] NVARCHAR(100), -- e.g. ShipNow, Grab, GHTK
    Config NVARCHAR(MAX),
    IsActive BIT DEFAULT 0
);

/* ======================================================================================
   SECTION: Audit / Logs / Reviews / Delivery
   - AuditLog: ghi nhận các hành động quan trọng (audit trail)
   - Review: đánh giá của khách hàng cho order
   - DeliveryPartner: đối tác giao hàng
   - DeliveryOrder: liên kết order với partner, tracking
   ====================================================================================== */
CREATE TABLE AuditLog(
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    StoreID INT NULL,
    UserID INT NULL,
    [Action] NVARCHAR(200),
    Detail NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME()
);

CREATE TABLE Review(
    ReviewID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NULL,
    CustomerID INT NULL,
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(2000),
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Review_Order FOREIGN KEY(OrderID) REFERENCES [Order](OrderID),
    CONSTRAINT FK_Review_Cust FOREIGN KEY(CustomerID) REFERENCES Customer(CustomerID)
);

CREATE TABLE DeliveryPartner(
    PartnerID INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(200),
    APIConfig NVARCHAR(MAX),
    IsActive BIT DEFAULT 0
);

CREATE TABLE DeliveryOrder(
    DeliveryOrderID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NULL,
    PartnerID INT NULL,
    TrackingCode NVARCHAR(200),
    [Status] NVARCHAR(100),
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_DO_Order FOREIGN KEY(OrderID) REFERENCES [Order](OrderID),
    CONSTRAINT FK_DO_Partner FOREIGN KEY(PartnerID) REFERENCES DeliveryPartner(PartnerID)
);

-- Indexes for common lookups
CREATE INDEX IDX_MenuItem_Store ON MenuItem(StoreID);
CREATE INDEX IDX_InventoryItem_Store ON InventoryItem(StoreID);
CREATE INDEX IDX_Order_Branch_Date ON [Order](BranchID, OrderTime);

GO

/*
   PHẦN: Mở rộng
   - Chuẩn hóa tối đa cơ sở dữ liệu (thêm nhiều sdt, email, ...)
   - Thêm các index
*/