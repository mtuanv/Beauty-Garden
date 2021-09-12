namespace ASPNETMVC_Group1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChiTietNhapHang")]
    public partial class ChiTietNhapHang
    {
        [Key]
        public int MaCTNH { get; set; }

        public int? MaSanPham { get; set; }

        public int? MaNhapHang { get; set; }

        public int? SoLuong { get; set; }

        public int? GiaNhap { get; set; }

        public virtual NhapHang NhapHang { get; set; }

        public virtual SanPham SanPham { get; set; }
    }
}
