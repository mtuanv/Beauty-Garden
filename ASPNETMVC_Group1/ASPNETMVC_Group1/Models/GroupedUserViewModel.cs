using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPNETMVC_Group1.Models
{
    public class GroupedUserViewModel
    {
        public List<AspNetUser> Staffs { get; set; }
        public List<AspNetUser> Admins { get; set; }
    }
}