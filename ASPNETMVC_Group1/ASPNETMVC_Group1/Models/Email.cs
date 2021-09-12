using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASPNETMVC_Group1.Models
{
    public class Email
    {
        [Required(ErrorMessage = "Hãy nhập họ tên.")]
        [Display(Name = "Họ tên")]
        public string Ten { get; set; }

        [Required(ErrorMessage = "Hãy nhập số điện thoại.")]
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; }

        [Required(ErrorMessage = "Hãy nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [Display(Name = "Email")]
        public string Mail { get; set; }

        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; }
    }
    public class EmailEvent
    {
        [Required(ErrorMessage = "Hãy nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [Display(Name = "Email")]
        public string Mail { get; set; }
    }
}