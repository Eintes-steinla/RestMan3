## **✅ LUỒNG ĐÚNG - GIẢI THÍCH CHI TIẾT**
```
┌─────────────────────────────────────────────────────────┐
│ USER (UI) - Người dùng                                  │
│ • Nhập dữ liệu vào TextBox, ComboBox                    │
│ • Nhấn Button, chọn item trong DataGrid                 │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────┐
│ VIEW (XAML) - Giao diện                                 │
│ • Hiển thị UI (Button, TextBox, DataGrid...)            │
│ • Data Binding: {Binding Property}                      │
│ • Command Binding: Command="{Binding SaveCommand}"      │
│ • Code-behind (.xaml.cs) CHỈ CÓ InitializeComponent()   │
│                                                         │
│ ❌ KHÔNG CHỨA:                                         │ 
│   - Business logic                                      │
│   - Database access                                     │
│   - Validation                                          │
└────────────────┬────────────────────────────────────────┘
                 │ (Binding)
                 ▼
┌─────────────────────────────────────────────────────────┐
│ COMMAND - Cầu nối                                       │
│ • ICommand interface                                    │
│ • RelayCommand implementation                           │
│ • Execute(parameter) - Thực thi action                  │
│ • CanExecute(parameter) - Kiểm tra điều kiện            │
│                                                         │
│ Command NẰM TRONG ViewModel dưới dạng Property:         │
│ public ICommand SaveCommand { get; set; }               │
└────────────────┬────────────────────────────────────────┘
                 │ (Gọi method)
                 ▼
┌─────────────────────────────────────────────────────────┐
│ VIEWMODEL - Trung gian UI ↔ Business Logic              │ 
│                                                         │
│ ✅ CHỨA:                                               │
│ 1. Properties binding với View                          │
│    • public MenuItem CurrentMenuItem { get; set; }      │
│    • public ObservableCollection<MenuItem> Items        │
│                                                         │
│ 2. Commands                                             │
│    • public ICommand SaveCommand { get; set; }          │
│    • public ICommand DeleteCommand { get; set; }        │
│                                                         │
│ 3. Presentation Logic                                   │
│    • Format dữ liệu: DisplayPrice = $"{Price:N0} VNĐ"   │
│    • Tính toán UI: TotalItems = Items.Count             │
│                                                         │
│ 4. UI State Management                                  │
│    • public bool IsLoading { get; set; }                │
│    • public bool IsEditMode { get; set; }               │
│                                                         │
│ 5. Orchestration (điều phối)                            │
│    • Gọi BLL services                                   │
│    • Xử lý kết quả từ BLL                               │
│    • Update UI state                                    │
│    • Show messages                                      │
│                                                         │
│ ❌ KHÔNG CHỨA:                                         │
│    • Business validation (việc của BLL)                 │
│    • Database queries (việc của DAL)                    │
│    • Business rules (việc của BLL)                      │
└────────────────┬────────────────────────────────────────┘
                 │ (Gọi service)
                 ▼
┌─────────────────────────────────────────────────────────┐
│ BLL (Business Logic Layer) - Xử lý nghiệp vụ            │
│                                                         │
│ ✅ CHỨA:                                               │
│ 1. Business Validation                                  │
│    • Kiểm tra giá >= 10,000đ                            │
│    • Kiểm tra tên món không trùng                       │
│    • Kiểm tra danh mục tồn tại                          │
│                                                         │
│ 2. Business Rules                                       │
│    • Quy tắc giảm giá tối đa 50%                        │
│    • Quy tắc chỉ edit món đang active                   │
│    • Quy tắc tính thuế, phí                             │
│                                                         │
│ 3. Complex Operations                                   │
│    • Xử lý transaction phức tạp                         │
│    • Điều phối nhiều repository                         │
│    • Aggregation, calculation                           │
│                                                         │
│ 4. Data Transformation                                  │
│    • DTO ↔ Entity conversion                            │
│    • Pagination logic                                   │
│    • Filtering logic                                    │
│                                                         │
│ ❌ KHÔNG CHỨA:                                         │
│    • SQL queries (việc của DAL)                         │
│    • UI logic (việc của ViewModel)                      │
│    • Dapper code (việc của DAL)                         │
└────────────────┬────────────────────────────────────────┘
                 │ (Gọi repository)
                 ▼
┌─────────────────────────────────────────────────────────┐
│ DAL (Data Access Layer) - Truy cập dữ liệu              │
│                                                         │
│ ✅ CHỨA:                                               │
│ 1. Repository Pattern                                   │
│    • MenuItemRepository.cs                              │
│    • MenuCategoryRepository.cs                          │
│                                                         │
│ 2. CRUD Operations                                      │
│    • Insert(entity) → int id                            │
│    • Update(entity) → void                              │
│    • Delete(id) → void                                  │ 
│    • GetById(id) → entity                               │
│    • GetAll() → List<entity>                            │
│                                                         │
│ 3. Query Methods                                        │
│    • GetByName(name) → entity                           │
│    • Search(filter) → List<entity>                      │
│    • GetPaged(pageIndex, pageSize) → PagedResult        │
│                                                         │
│ 4. SQL Execution (qua Dapper)                           │
│    • Tạo câu SQL (SELECT, INSERT, UPDATE, DELETE)       │
│    • Tạo parameters để tránh SQL Injection              │
│    • Map Entity ↔ Database Table                        │
│                                                         │
│ ❌ KHÔNG CHỨA:                                         │
│    • Business validation (việc của BLL)                 │
│    • UI logic (việc của ViewModel)                      │
│    • Business rules (việc của BLL)                      │
└────────────────┬────────────────────────────────────────┘
                 │ (Execute SQL)
                 ▼
┌─────────────────────────────────────────────────────────┐
│ DAPPER ORM - Object-Relational Mapper                   │
│ • DapperHelper.cs                                       │
│ • ExecuteScalar<T>(sql, params)                         │
│ • Query<T>(sql, params)                                 │
│ • Execute(sql, params)                                  │
│ • Map giữa C# objects ↔ SQL rows                        │
└────────────────┬────────────────────────────────────────┘
                 │ (ADO.NET)
                 ▼
┌─────────────────────────────────────────────────────────┐
│ SQL SERVER DATABASE                                     │
│ • MenuItems table                                       │
│ • MenuCategories table                                  │
│ • Execute SQL commands                                  │
│ • Return results                                        │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼
        ┌────────────────┐
        │ TRẢ KẾT QUẢ    │
        │ NGƯỢC LẠI      │
        └────────────────┘
                 │
Database  →  DAL    →  BLL   →  ViewModel  →   View    →    USER
(data)     (entity)   (DTO)    (update UI)   (display)   (see result)