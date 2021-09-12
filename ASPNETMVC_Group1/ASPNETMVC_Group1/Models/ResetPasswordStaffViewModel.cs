using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASPNETMVC_Group1.Models
{
    public class ResetPasswordStaffViewModel
    {
        [Required]
        public string Id { get; set; }

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
}