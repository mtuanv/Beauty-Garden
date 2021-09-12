namespace ASPNETMVC_Group1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DanhGia")]
    public partial class DanhGia
    {
        [Key]
        public int MaDanhGia { get; set; }

        [StringLength(128)]
        public string MaKhachHang { get; set; }

        public int? MaSanPham { get; set; }

        public int? MaDonHang { get; set; }

        [Column("DanhGia")]
        public int? DanhGia1 { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual SanPham SanPham { get; set; }

        public virtual DonHang DonHang { get; set; }
    }
}
