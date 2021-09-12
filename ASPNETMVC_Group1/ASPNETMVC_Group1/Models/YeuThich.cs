namespace ASPNETMVC_Group1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("YeuThich")]
    public partial class YeuThich
    {
        [Key]
        public int MaYeuThich { get; set; }

        [Required]
        [StringLength(128)]
        public string MaKhachHang { get; set; }

        public int MaSanPham { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual SanPham SanPham { get; set; }
    }
}
