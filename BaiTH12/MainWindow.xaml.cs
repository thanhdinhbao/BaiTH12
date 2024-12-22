using BaiTH12.Models;
using System.Windows;

namespace BaiTH12
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        QlbanHangContext _context = new QlbanHangContext();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadData()
        {

            var sanPhamList = _context.SanPhams
                .Select(sp => new
                {
                    sp.MaSp,
                    sp.TenSp,
                    LoaiSP = sp.MaLoaiNavigation.TenLoai,
                    sp.DonGia,
                    sp.SoLuong,
                    sp.MaLoai,
                    ThanhTien = sp.DonGia * sp.SoLuong
                })
                .OrderBy(sp => sp.DonGia)
                .ToList();

            dgSanPham.ItemsSource = sanPhamList;

            var loaiSPs = _context.LoaiSanPhams
                             .Select(lsp => new
                             {
                                 MaLoai = lsp.MaLoai,
                                 TenLoai = lsp.TenLoai
                             })
                             .ToList();

            cmbLoaiSP.ItemsSource = loaiSPs;
            cmbLoaiSP.DisplayMemberPath = "TenLoai"; 
            cmbLoaiSP.SelectedValuePath = "MaLoai";
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtMaSanPham.Text) ||
                string.IsNullOrWhiteSpace(txtTenSanPham.Text) ||
                string.IsNullOrWhiteSpace(txtDonGia.Text) ||
                string.IsNullOrWhiteSpace(txtSoLuong.Text) ||
                cmbLoaiSP.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(txtDonGia.Text, out _) || !int.TryParse(txtSoLuong.Text, out _))
            {
                MessageBox.Show("Đơn giá và số lượng phải là số hợp lệ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }


        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;


            var maLoai = cmbLoaiSP.SelectedValue?.ToString();
            var sanPham = new SanPham
            {
                MaSp = txtMaSanPham.Text,
                TenSp = txtTenSanPham.Text,
                DonGia = int.Parse(txtDonGia.Text),
                SoLuong = int.Parse(txtSoLuong.Text),
                MaLoai = maLoai
            };

            // Thêm vào cơ sở dữ liệu
            _context.SanPhams.Add(sanPham);
            _context.SaveChanges();

            // Hiển thị thông báo
            MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            // Làm mới DataGrid
            LoadData();
        }


        private void btnSua_Click(object sender, RoutedEventArgs e)
        {

            string maSanPham = txtMaSanPham.Text;

            var sanPham = _context.SanPhams.FirstOrDefault(sp => sp.MaSp == maSanPham);
            if (sanPham == null)
            {
                MessageBox.Show("Không tìm thấy sản phẩm với mã này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenSanPham.Text) ||
                !int.TryParse(txtDonGia.Text, out int donGia) || donGia <= 0 ||
                !int.TryParse(txtSoLuong.Text, out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin và đảm bảo Đơn giá, Số lượng là số nguyên > 0!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            sanPham.TenSp = txtTenSanPham.Text;
            sanPham.DonGia = donGia;
            sanPham.SoLuong = soLuong;
            cmbLoaiSP.SelectedValue = sanPham.MaLoai;

            _context.SaveChanges();

            MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadData();
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {

            // Lấy mã sản phẩm từ text box
            string maSanPham = txtMaSanPham.Text;

            // Kiểm tra xem mã sản phẩm có tồn tại trong cơ sở dữ liệu không
            var sanPham = _context.SanPhams.FirstOrDefault(sp => sp.MaSp == maSanPham);
            if (sanPham == null)
            {
                MessageBox.Show("Không tìm thấy sản phẩm với mã này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Hiển thị thông báo xác nhận xóa
            var result = MessageBox.Show($"Bạn có chắc muốn xóa sản phẩm với mã: {maSanPham}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // Xóa sản phẩm
                _context.SanPhams.Remove(sanPham);
                _context.SaveChanges();

                MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Tải lại dữ liệu
                LoadData();
            }


        }

        private void btnTim_Click(object sender, RoutedEventArgs e)
        {
            string maSanPham = txtMaSanPham.Text.Trim();

            if (string.IsNullOrEmpty(maSanPham))
            {
                MessageBox.Show("Vui lòng nhập mã sản phẩm.");
                return;
            }

            var product = _context.SanPhams
                .Where(sp => sp.MaSp == maSanPham)
                .Select(sp => new
                {
                    sp.MaSp,
                    sp.TenSp,
                    sp.DonGia,
                    sp.SoLuong,
                    sp.MaLoai,
                    TenLoai = sp.MaLoaiNavigation.TenLoai,
                    ThanhTien = sp.SoLuong * sp.DonGia
                })
                .FirstOrDefault();

            // Nếu tìm thấy sản phẩm, mở cửa sổ mới và hiển thị thông tin sản phẩm
            if (product != null)
            {
                // Tạo một danh sách chứa sản phẩm (ItemsSource yêu cầu danh sách)
                var productList = new List<object> { product };

                // Mở cửa sổ mới
                Window2 productWindow = new Window2();
                productWindow.LoadData(productList);
                productWindow.Show();
            }
            else
            {
                MessageBox.Show("Không tìm thấy sản phẩm với mã này.");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void dgSanPham_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgSanPham.SelectedItem == null)
            {
                return;
            }

            // Lấy sản phẩm được chọn
            var selectedProduct = dgSanPham.SelectedItem as dynamic;

            // Hiển thị thông tin lên các điều khiển nhập liệu
            txtMaSanPham.Text = selectedProduct.MaSp;
            txtTenSanPham.Text = selectedProduct.TenSp;
            txtDonGia.Text = selectedProduct.DonGia.ToString();
            txtSoLuong.Text = selectedProduct.SoLuong.ToString();
            cmbLoaiSP.Text = selectedProduct.MaLoai;
        }
    }
}