using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPNETMVC_Group1.Models
{
    public class CreateTinTucViewModel
    {
        [StringLength(255)]
        [Required(ErrorMessage = "Hãy nhập tiêu đề.")]
        [Display(Name = "Tiêu đề")]
        public string TieuDe { get; set; }

        [Column(TypeName = "ntext")]
        [Required(ErrorMessage = "Hãy nhập nội dung.")]
        [Display(Name = "Nội dung")]
        [AllowHtml]
        public string NoiDung { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "Hãy chọn loại tin tức.")]
        [Display(Name = "Loại tin tức")]
        public string LoaiTinTuc { get; set; }

        [Required(ErrorMessage = "Hãy chọn loại tin nổi bật.")]
        [Display(Name = "Tin nổi bật")]
        public bool? NoiBat { get; set; }

        [Required(ErrorMessage = "Hãy chọn ảnh.")]
        [Display(Name = "Ảnh minh hoạ")]
        public HttpPostedFileBase Image { get; set; }
    }
    public class EditTinTucViewModel
    {
        public int MaTinTuc { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "Hãy nhập tiêu đề.")]
        [Display(Name = "Tiêu đề")]
        public string TieuDe { get; set; }

        [Column(TypeName = "ntext")]
        [Required(ErrorMessage = "Hãy nhập nội dung.")]
        [Display(Name = "Nội dung")]
        [AllowHtml]
        public string NoiDung { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "Hãy chọn loại tin tức.")]
        [Display(Name = "Loại tin tức")]
        public string LoaiTinTuc { get; set; }

        [Required(ErrorMessage = "Hãy chọn loại tin nổi bật.")]
        [Display(Name = "Tin nổi bật")]
        public bool? NoiBat { get; set; }

        [Display(Name = "Ảnh minh hoạ")]
        public HttpPostedFileBase Image { get; set; }

        public Anh Anh { get; set; }
    }
  
}