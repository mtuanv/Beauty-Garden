namespace ASPNETMVC_Group1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tag")]
    public partial class Tag
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tag()
        {
            SanPhamTags = new HashSet<SanPhamTag>();
        }

        [Key]
        public int MaTag { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "Hãy nhập tên tag.")]
        [Display(Name = "Tên tag")]
        public string TenTag { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanPhamTag> SanPhamTags { get; set; }
    }
}
