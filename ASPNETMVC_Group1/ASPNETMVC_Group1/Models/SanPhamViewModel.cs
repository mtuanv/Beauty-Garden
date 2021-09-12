using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Models
{
    public class CreateSanPhamViewModel
    {
        [StringLength(255)]
        [Required(ErrorMessage = "Hãy nhập tên sản phẩm.")]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; }

        [Column(TypeName = "ntext")]
        [Display(Name = "Công dụng")]
        [AllowHtml]
        public string CongDung { get; set; }

        [Column(TypeName = "ntext")]
        [Required(ErrorMessage = "Hãy nhập mô tả.")]
        [Display(Name = "Mô tả")]
        [AllowHtml]
        public string MoTa { get; set; }

        [Required(ErrorMessage = "Hãy nhập giá thị trường.")]
        [Display(Name = "Giá thị trường")]
        public int GiaThiTruong { get; set; }

        [Required(ErrorMessage = "Hãy nhập giá bán.")]
        [Display(Name = "Giá bán")]
        [LessThan("GiaThiTruong", ErrorMessage = "Giá bán cần thấp hơn giá thị trường.")]
        public int GiaBan { get; set; }

        [Display(Name = "Giảm giá")]
        public int? GiamGia { get; set; }

        [Required(ErrorMessage = "Hãy chọn nhãn hiệu.")]
        [Display(Name = "Nhãn hiệu")]
        public int MaNhanHieu { get; set; }

        [Required(ErrorMessage = "Hãy chọn danh mục.")]
        [Display(Name = "Danh mục")]
        public int MaDanhMuc { get; set; }

        [Required(ErrorMessage = "Hãy chọn ảnh sản phẩm.")]
        [Display(Name = "Ảnh minh hoạ")]
        public HttpPostedFileBase[] Images { get; set; }

        [Display(Name = "Tag")]
        public int?[] Tags { get; set; }
    }

    public class EditSanPhamViewModel
    {
        public int MaSanPham { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "Hãy nhập tên sản phẩm.")]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; }

        [Column(TypeName = "ntext")]
        [Display(Name = "Công dụng")]
        [AllowHtml]
        public string CongDung { get; set; }

        [Column(TypeName = "ntext")]
        [Required(ErrorMessage = "Hãy nhập mô tả.")]
        [Display(Name = "Mô tả")]
        [AllowHtml]
        public string MoTa { get; set; }

        [Required(ErrorMessage = "Hãy nhập giá thị trường.")]
        [Display(Name = "Giá thị trường")]
        public int GiaThiTruong { get; set; }

        [Required(ErrorMessage = "Hãy nhập giá bán.")]
        [Display(Name = "Giá bán")]
        [LessThan("GiaThiTruong", ErrorMessage = "Giá bán cần thấp hơn giá thị trường.")]
        public int GiaBan { get; set; }

        [Display(Name = "Giảm giá")]
        public int? GiamGia { get; set; }

        [Required(ErrorMessage = "Hãy chọn nhãn hiệu.")]
        [Display(Name = "Nhãn hiệu")]
        public int MaNhanHieu { get; set; }

        [Required(ErrorMessage = "Hãy chọn danh mục.")]
        [Display(Name = "Danh mục")]
        public int MaDanhMuc { get; set; }

        [Required(ErrorMessage = "Hãy chọn ảnh sản phẩm.")]
        [Display(Name = "Ảnh minh hoạ")]
        public HttpPostedFileBase[] Images { get; set; }

        public List<Anh> Anhs { get; set; }

        [Display(Name = "Tag")]
        public int?[] Tags { get; set; }
    }

    public class NhapHangViewModel
    {
        public int MaSanPham { get; set; }

        [Required]
        public int SoLuong { get; set; }
        [Required]
        public int GiaNhap { get; set; }
        public int page { get; set; }

        public string search { get; set; }
    }

    public class Top3DashboardViewModel
    {
        public int MaSanPham { get; set; }

        public string TenSanPham { get; set; }

        public string Anh { get; set; }

        public int SoLuongBan { get; set; }
    }
}