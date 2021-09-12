namespace ASPNETMVC_Group1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SanPhamTag")]
    public partial class SanPhamTag
    {
        [Key]
        public int MaSPT { get; set; }

        public int? MaSanPham { get; set; }

        public int? MaTag { get; set; }

        public virtual SanPham SanPham { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
