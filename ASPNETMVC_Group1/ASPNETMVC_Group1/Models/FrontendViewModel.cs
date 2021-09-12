using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ASPNETMVC_Group1.Models
{
    [NotMapped]
    public class DanhMucSPViewModel : SanPham
    {
        public Anh anh { get; set; }
        public string GBJS { get; set; }
        public string GTTJS { get; set; }
    }
    [NotMapped]
    public class DanhMucTTViewModel : TinTuc
    {
        public Anh anh { get; set; }
    }
    public class ListDanhMucViewModel
    {
        public List<DanhMuc> danhMucs { get; set; }
        public List<SanPham> sanPhams { get; set; }
    }
    public class ListThuongHieuViewModel
    {
        public List<NhanHieu> nhanHieus { get; set; }
        public List<SanPham> sanPhams { get; set; }
    }

    public class DetailSanPhamViewModel
    {
        public SanPham sanPham { get; set; }
        public List<Anh> anhs { get; set; }
        public List<SanPham> cungXem { get; set; }
        public List<SanPham> muaCung { get; set; }
        public List<SanPham> cungLoai { get; set; }
    }

    public class DetailsTinTucViewModel
    {
        public TinTuc tinTuc { get; set; }
        public List<TinTuc> ttKhacs { get; set; }
        public List<Anh> anhs { get; set; }
    }

    public class HomeViewModel
    {
        public List<SanPham> khuyenMais { get; set; }
        public List<SanPham> moiVes { get; set; }
        public List<SanPham> banChays { get; set; }
        public List<SanPham> trangDiems { get; set; }
        public List<SanPham> chamSocDas { get; set; }
        public List<TinTuc> tinNoiBats { get; set; }
        public List<Anh> anhs { get; set; }
    }

    public class SearchView
    {
        public int MaSanPham { get; set; }

        public string TenSanPham { get; set; }

        public int? GiaThiTruong { get; set; }

        public int? GiaBan { get; set; }

        public int? GiamGia { get; set; }

        public int MaTinTuc { get; set; }

        public string TieuDe { get; set; }

        public string LoaiTinTuc { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Anh anh { get; set; }
    }

    public class CartView
    {
        public int MaSanPham { get; set; }

        public string TenSanPham { get; set; }

        public int? GiaThiTruong { get; set; }

        public int? GiaBan { get; set; }

        public int? SoLuong { get; set; }

        public int? SoLuongCon { get; set; }

        public int? GiamGia { get; set; }

        public string Anh { get; set; }
    }

    public class ThanhToanView
    {
        [Required]
        public string HoTen { get; set; }

        [Required]
        public string SDT { get; set; }

        [Required]
        public string ThanhPho { get; set; }

        [Required]
        public string QuanHuyen { get; set; }

        [Required]
        public string DiaChi { get; set; }

        public string GhiChu { get; set; }

        public List<CartView> cart { get; set; }
    }
}