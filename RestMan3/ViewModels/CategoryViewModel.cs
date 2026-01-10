using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using BLL.Services;
using DTO.DTOs;
using DTO.Entities;
using GalaSoft.MvvmLight.Command;

namespace RestMan3.ViewModels
{
    public class CategoryViewModel : INotifyPropertyChanged
    {
        private readonly MenuItemService _menuItemService;

        // Properties for binding
        private ObservableCollection<MenuItemDTO> _products;
        public ObservableCollection<MenuItemDTO> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        private ObservableCollection<MenuCategory> _categories;
        public ObservableCollection<MenuCategory> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                SearchCommand?.Execute(null);
            }
        }

        private int? _selectedCategoryId;
        public int? SelectedCategoryId
        {
            get => _selectedCategoryId;
            set
            {
                _selectedCategoryId = value;
                OnPropertyChanged(nameof(SelectedCategoryId));
                LoadData();
            }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));

                // Báo cho các Command kiểm tra lại điều kiện bấn nút (CanExecute)
                (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();

                LoadData();
            }
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value;
                OnPropertyChanged(nameof(PageSize));
                LoadData();
            }
        }

        private int _totalCount;
        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                OnPropertyChanged(nameof(TotalCount));
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(PaginationText));
            }
        }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public string PaginationText => $"Hiển thị {((CurrentPage - 1) * PageSize) + 1} - {Math.Min(CurrentPage * PageSize, TotalCount)} trên tổng số {TotalCount} hàng hóa";

        // Commands
        public ICommand LoadDataCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }
        public ICommand FirstPageCommand { get; set; }
        public ICommand LastPageCommand { get; set; }

        // Constructor
        public CategoryViewModel()
        {
            _menuItemService = new MenuItemService();
            Products = new ObservableCollection<MenuItemDTO>();
            Categories = new ObservableCollection<MenuCategory>();

            InitializeCommands();
            LoadCategories();
            LoadData();
        }

        private void InitializeCommands()
        {
            LoadDataCommand = new RelayCommand(LoadData);
            SearchCommand = new RelayCommand(Search);
            NextPageCommand = new RelayCommand(NextPage, () => CurrentPage < TotalPages);
            PreviousPageCommand = new RelayCommand(PreviousPage, () => CurrentPage > 1);
            FirstPageCommand = new RelayCommand(FirstPage);
            LastPageCommand = new RelayCommand(LastPage);
        }

        /// <summary>
        /// Load danh sách danh mục
        /// </summary>
        private void LoadCategories()
        {
            try
            {
                var categories = _menuItemService.GetAllCategories();
                Categories.Clear();

                // Thêm "Tất cả"
                Categories.Add(new MenuCategory { CategoryID = 0, CategoryName = "Tất cả" });

                foreach (var cat in categories)
                {
                    Categories.Add(cat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load danh mục: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load dữ liệu MenuItem
        /// </summary>
        public void LoadData()
        {
            try
            {
                var filter = new MenuItemFilterDTO
                {
                    SearchText = SearchText,
                    CategoryID = SelectedCategoryId > 0 ? SelectedCategoryId : null,
                    IsActive = true, // Chỉ lấy hàng đang kinh doanh
                    PageNumber = CurrentPage,
                    PageSize = PageSize
                };

                var result = _menuItemService.GetMenuItems(filter);

                Products.Clear();
                foreach (var item in result.Items)
                {
                    Products.Add(item);
                }

                TotalCount = result.TotalCount;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Search()
        {
            CurrentPage = 1;
            LoadData();
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
            }
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
            }
        }

        private void FirstPage()
        {
            CurrentPage = 1;
        }

        private void LastPage()
        {
            CurrentPage = TotalPages;
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
