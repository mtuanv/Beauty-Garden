namespace ASPNETMVC_Group1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChiTietDonHang")]
    public partial class ChiTietDonHang
    {
        [Key]
        public int MaCTDH { get; set; }

        public int? MaSanPham { get; set; }

        public int? MaDonHang { get; set; }

        public int? SoLuong { get; set; }

        public int? DonGia { get; set; }

        public int? GiamGia { get; set; }

        public virtual DonHang DonHang { get; set; }

        public virtual SanPham SanPham { get; set; }
    }
}
