namespace ASPNETMVC_Group1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DonHang")]
    public partial class DonHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DonHang()
        {
            ChiTietDonHangs = new HashSet<ChiTietDonHang>();
        }

        [Key]
        public int MaDonHang { get; set; }

        [StringLength(128)]
        public string MaKhachHang { get; set; }

        public DateTime? NgayDat { get; set; }

        [StringLength(255)]
        public string Ten { get; set; }

        [StringLength(255)]
        public string ThanhPho { get; set; }

        [StringLength(255)]
        public string QuanHuyen { get; set; }

        [Column(TypeName = "ntext")]
        public string DiaChi { get; set; }

        public string SDT { get; set; }

        public int? TrangThai { get; set; }

        [Column(TypeName = "ntext")]
        public string GhiChu { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
    }
}
