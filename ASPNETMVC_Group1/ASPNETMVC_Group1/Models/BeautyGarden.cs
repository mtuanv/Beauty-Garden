using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ASPNETMVC_Group1.Models
{
    public partial class BeautyGarden : DbContext
    {
        public BeautyGarden()
            : base("name=BeautyGarden")
        {
        }

        public virtual DbSet<Anh> Anhs { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public virtual DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }
        public virtual DbSet<ChiTietNhapHang> ChiTietNhapHangs { get; set; }
        public virtual DbSet<DanhGia> DanhGias { get; set; }
        public virtual DbSet<DanhMuc> DanhMucs { get; set; }
        public virtual DbSet<DonHang> DonHangs { get; set; }
        public virtual DbSet<GioHang> GioHangs { get; set; }
        public virtual DbSet<NhanHieu> NhanHieux { get; set; }
        public virtual DbSet<NhapHang> NhapHangs { get; set; }
        public virtual DbSet<SanPham> SanPhams { get; set; }
        public virtual DbSet<SanPhamTag> SanPhamTags { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<TinTuc> TinTucs { get; set; }
        public virtual DbSet<YeuThich> YeuThiches { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.DanhGias)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.MaKhachHang);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.DonHangs)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.MaKhachHang);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.GioHangs)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.MaKhachHang);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.YeuThiches)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.MaKhachHang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DanhMuc>()
                .HasMany(e => e.DanhMuc1)
                .WithOptional(e => e.DanhMuc2)
                .HasForeignKey(e => e.MaDanhMucCha);

            modelBuilder.Entity<DonHang>()
                .HasMany(e => e.ChiTietDonHangs)
                .WithOptional(e => e.DonHang)
                .HasForeignKey(e => e.MaDonHang);

            modelBuilder.Entity<SanPham>()
                .HasMany(e => e.YeuThiches)
                .WithRequired(e => e.SanPham)
                .WillCascadeOnDelete(false);
        }
    }
}
