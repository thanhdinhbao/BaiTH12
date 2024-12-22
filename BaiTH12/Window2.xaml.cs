using System.Windows;

namespace BaiTH12
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
        }

        public void LoadData(IEnumerable<object> data)
        {
            if (data == null || !data.Any())
            {
                MessageBox.Show("Không có dữ liệu để hiển thị.");
                return;
            }

            // Gán dữ liệu cho DataGrid
            dgThongTinBanHang.ItemsSource = data;
        }
    }
}
