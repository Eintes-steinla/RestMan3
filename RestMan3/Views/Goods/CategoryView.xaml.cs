using RestMan3.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RestMan3.Views.Goods
{
    public partial class CategoryView : UserControl
    {
        private CategoryViewModel _viewModel;

        public CategoryView()
        {
            InitializeComponent();
            _viewModel = new CategoryViewModel();
            this.DataContext = _viewModel;

            // Bind RecordsPerPageComboBox
            RecordsPerPageComboBox.SelectionChanged += RecordsPerPageComboBox_SelectionChanged;
        }

        private void RecordsPerPageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel != null && RecordsPerPageComboBox.SelectedItem is ComboBoxItem item)
            {
                _viewModel.PageSize = int.Parse(item.Content.ToString());
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //if (sender is TextBox textBox)
            //{
            //    if (textBox.Text == "Theo mã, tên hàng" || textBox.Text == "Tìm kiếm nhóm hàng")
            //    {
            //        textBox.Text = "";
            //        textBox.Foreground = System.Windows.Media.Brushes.Black;
            //    }
            //}
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (sender is TextBox textBox)
            //{
            //    if (string.IsNullOrWhiteSpace(textBox.Text))
            //    {
            //        if (textBox.Name == "SearchTextBox")
            //        {
            //            textBox.Text = "Theo mã, tên hàng";
            //        }
            //        else if (textBox.Name == "SearchCategoryTextBox")
            //        {
            //            textBox.Text = "Tìm kiếm nhóm hàng";
            //        }

            //        textBox.Foreground = new System.Windows.Media.SolidColorBrush(
            //            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#666666")
            //        );
            //    }
            //}
        }
    }
}