using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RestMan3.Views.Goods
{
    public partial class SideDishTab : UserControl
    {
        public ObservableCollection<ComponentItem> Components { get; set; }
        private List<ComponentItem> _allAvailableComponents;

        public SideDishTab()
        {
            InitializeComponent();
            Components = new ObservableCollection<ComponentItem>();
            ComponentsItemsControl.ItemsSource = Components;

            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            _allAvailableComponents = new List<ComponentItem>
            {
                new ComponentItem { MaHangHoa = "SP000011", TenHangHoa = "Sup kem rau 4 mua", GiaVon = 100500 },
                new ComponentItem { MaHangHoa = "SP000015", TenHangHoa = "Sup kem kieu Paris", GiaVon = 120000 },
                new ComponentItem { MaHangHoa = "SP000012", TenHangHoa = "Sup kem ga nu hoang", GiaVon = 115000 },
                new ComponentItem { MaHangHoa = "SP000013", TenHangHoa = "Sup hanh tay kieu Phap", GiaVon = 105000 },
                new ComponentItem { MaHangHoa = "SP000014", TenHangHoa = "Sup kem bi do voi sua dua", GiaVon = 110000 },
                new ComponentItem { MaHangHoa = "SP000016", TenHangHoa = "Sup ca chua tuoi", GiaVon = 95000 },
                new ComponentItem { MaHangHoa = "SP000017", TenHangHoa = "Sup nam rom", GiaVon = 105000 },
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                SearchResultsBorder.Visibility = Visibility.Collapsed;
                return;
            }

            var results = _allAvailableComponents
                .Where(c => c.TenHangHoa.ToLower().Contains(searchText) ||
                           c.MaHangHoa.ToLower().Contains(searchText))
                .ToList();

            var addedCodes = Components.Select(c => c.MaHangHoa).ToList();
            results = results.Where(r => !addedCodes.Contains(r.MaHangHoa)).ToList();

            SearchResultsListBox.ItemsSource = results;
            SearchResultsBorder.Visibility = results.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SearchResult_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SearchResultsListBox.SelectedItem is ComponentItem selectedComponent)
            {
                AddComponent(selectedComponent);
                SearchTextBox.Clear();
                SearchResultsBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void AddComponent(ComponentItem component)
        {
            var newComponent = new ComponentItem
            {
                MaHangHoa = component.MaHangHoa,
                TenHangHoa = component.TenHangHoa,
                GiaVon = component.GiaVon,
                SoLuong = 1,
                STT = Components.Count + 1
            };

            // Đăng ký sự kiện để cập nhật tổng tiền khi số lượng thay đổi
            newComponent.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ComponentItem.SoLuong))
                {
                    UpdateSummary();
                }
            };

            Components.Add(newComponent);
            UpdateSummary();
        }

        private void DeleteComponent_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ComponentItem component)
            {
                Components.Remove(component);

                for (int i = 0; i < Components.Count; i++)
                {
                    Components[i].STT = i + 1;
                }

                UpdateSummary();
            }
        }

        private void UpdateSummary()
        {
            decimal totalCost = Components.Sum(c => c.GiaVon * c.SoLuong);
            TotalCostTextBlock.Text = totalCost.ToString("N0");
            TotalPriceTextBlock.Text = totalCost.ToString("N0");
        }

        public List<ComponentItem> GetComponents()
        {
            return Components.ToList();
        }
    }

    public class SideDishTabItem : INotifyPropertyChanged
    {
        private int _stt;
        private int _soLuong = 1;
        private string _maHangHoa;
        private string _tenHangHoa;
        private decimal _giaVon;

        public int STT
        {
            get => _stt;
            set { if (_stt != value) { _stt = value; OnPropertyChanged(nameof(STT)); } }
        }

        public string MaHangHoa
        {
            get => _maHangHoa;
            set { if (_maHangHoa != value) { _maHangHoa = value; OnPropertyChanged(nameof(MaHangHoa)); } }
        }

        public string TenHangHoa
        {
            get => _tenHangHoa;
            set { if (_tenHangHoa != value) { _tenHangHoa = value; OnPropertyChanged(nameof(TenHangHoa)); } }
        }

        public decimal GiaVon
        {
            get => _giaVon;
            set { if (_giaVon != value) { _giaVon = value; OnPropertyChanged(nameof(GiaVon)); OnPropertyChanged(nameof(ThanhTien)); } }
        }

        public int SoLuong
        {
            get => _soLuong;
            set { if (_soLuong != value) { _soLuong = value; OnPropertyChanged(nameof(SoLuong)); OnPropertyChanged(nameof(ThanhTien)); } }
        }

        public decimal ThanhTien => GiaVon * SoLuong;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
