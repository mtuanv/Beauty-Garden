using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace ASPNETMVC_Group1.Models
{
    public class IndexViewModel
    {
        //public bool HasPassword { get; set; }
        //public IList<UserLoginInfo> Logins { get; set; }
        //public string PhoneNumber { get; set; }
        //public bool TwoFactor { get; set; }
        //public bool BrowserRemembered { get; set; }
        public string Avatar { get; set; }
        public string Ten { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Hãy nhập mật khẩu hiện tại.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật Khẩu Hiện Tại")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Hãy nhập mật khẩu mới.")]
        [StringLength(100, ErrorMessage = "{0} có ít nhất {2} kí tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không trùng khớp.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }

    public class DetailDonHang
    {
        public int MaSanPham { get; set; }

        public string TenSanPham { get; set; }

        public int? DonGia { get; set; }

        public int? GiamGia { get; set; }

        public int? SoLuong { get; set; }

        public int? DanhGia { get; set; }

        public string Anh { get; set; }
    }

    public class DonHangViewModel
    {
        public int MaDonHang { get; set; }

        public DateTime? NgayDat { get; set; }

        public int? TrangThai { get; set; }

        public List<DetailDonHang> detailDonHangs { get; set; }

        public string Ten { get; set; }

        public string DiaChi { get; set; }

        public string ThanhPho { get; set; }

        public string QuanHuyen { get; set; }

        public string SDT { get; set; }
    }
}