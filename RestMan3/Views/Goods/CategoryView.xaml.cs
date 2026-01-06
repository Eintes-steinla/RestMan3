using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RestMan3.Views.Goods
{
    public partial class CategoryView : UserControl
    {
        public CategoryView()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            var products = new ObservableCollection<Product>
            {
                new Product
                {
                    Code = "SP000018",
                    Name = "Mint Tea",
                    MenuType = "Khác",
                    Price = 15000,
                    Cost = 7000,
                    Stock = 1005
                },
                new Product
                {
                    Code = "SP000019",
                    Name = "Lipton with milk",
                    MenuType = "Khác",
                    Price = 15000,
                    Cost = 7000,
                    Stock = 1013
                },
                new Product
                {
                    Code = "SP000020",
                    Name = "Lemon Juice",
                    MenuType = "Khác",
                    Price = 15000,
                    Cost = 7000,
                    Stock = 1013
                },
                new Product
                {
                    Code = "SP000021",
                    Name = "Bia Heiniken",
                    MenuType = "Khác",
                    Price = 30000,
                    Cost = 20500,
                    Stock = 1005
                },
                new Product
                {
                    Code = "SP000022",
                    Name = "Bia Hà Nội",
                    MenuType = "Khác",
                    Price = 30000,
                    Cost = 20500,
                    Stock = 1001
                },
                new Product
                {
                    Code = "SP000023",
                    Name = "Thuốc lá Vinataba",
                    MenuType = "Khác",
                    Price = 30000,
                    Cost = 20500,
                    Stock = 1011
                }
            };

            ProductsDataGrid.ItemsSource = products;
        }

        // Thay thế cả hai hàm GotFocus cũ bằng hàm này
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Ép kiểu sender thành TextBox
            TextBox currentTextBox = (TextBox)sender;

            // Kiểm tra nội dung placeholder dựa trên tên của TextBox (hoặc Text)
            if (currentTextBox.Name == "SearchTextBox" && currentTextBox.Text == "Theo mã, tên hàng")
            {
                currentTextBox.Text = "";
                currentTextBox.Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1A1A1A"));
            }
            else if (currentTextBox.Name == "SearchCategoryTextBox" && currentTextBox.Text == "Tìm kiếm nhóm hàng")
            {
                currentTextBox.Text = "";
                currentTextBox.Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1A1A1A"));
            }
        }

        // Thay thế cả hai hàm LostFocus cũ bằng hàm này
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox currentTextBox = (TextBox)sender;

            if (string.IsNullOrWhiteSpace(currentTextBox.Text))
            {
                // Đặt lại placeholder và màu dựa trên tên TextBox
                if (currentTextBox.Name == "SearchTextBox")
                {
                    currentTextBox.Text = "Theo mã, tên hàng";
                }
                else if (currentTextBox.Name == "SearchCategoryTextBox")
                {
                    currentTextBox.Text = "Tìm kiếm nhóm hàng";
                }

                currentTextBox.Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#666666"));
            }
        }

    }

    // Product Model
    public class Product
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string MenuType { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public int Stock { get; set; }
    }
}