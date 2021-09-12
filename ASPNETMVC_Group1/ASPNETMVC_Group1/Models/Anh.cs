namespace ASPNETMVC_Group1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Anh")]
    public partial class Anh
    {
        [Key]
        public int MaAnh { get; set; }

        [Column(TypeName = "ntext")]
        public string LinkAnh { get; set; }

        public int? MaSanPham { get; set; }

        public int? MaTinTuc { get; set; }

        public virtual TinTuc TinTuc { get; set; }
    }
}
