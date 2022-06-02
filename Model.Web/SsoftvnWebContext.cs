using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Model.Web
{
    public partial class SsoftvnWebContext : DbContext
    {
        public SsoftvnWebContext() : base ("name=SsoftvnWebContext")
        { }

        public SsoftvnWebContext(string connectionStrings) : base("name=" + connectionStrings)
        {

        }

        public virtual DbSet<DM_Anh_Slider> DM_Anh_Slider { get; set; }
        public virtual DbSet<DM_BaiViet> DM_BaiViet { get; set; }
        public virtual DbSet<DM_BaiViet_Tag> DM_BaiViet_Tag { get; set; }
        public virtual DbSet<DM_DonHang> DM_DonHang { get; set; }
        public virtual DbSet<DM_DonHangChiTiet> DM_DonHangChiTiet { get; set; }
        public virtual DbSet<DM_KhachHang> DM_KhachHang { get; set; }
        public virtual DbSet<DM_LienHe> DM_LienHe { get; set; }
        public virtual DbSet<DM_Menu> DM_Menu { get; set; }
        public virtual DbSet<DM_NhomBaiViet> DM_NhomBaiViet { get; set; }
        public virtual DbSet<DM_NhomSanPham> DM_NhomSanPham { get; set; }
        public virtual DbSet<DM_SanPham> DM_SanPham { get; set; }
        public virtual DbSet<DM_Tags> DM_Tags { get; set; }
        public virtual DbSet<DM_TinhThanh> DM_TinhThanh { get; set; }
        public virtual DbSet<HT_NguoiDung> HT_NguoiDung { get; set; }
        public virtual DbSet<HT_NhomNguoiDung> HT_NhomNguoiDung { get; set; }
        public virtual DbSet<HT_NhomNguoiDung_Quyen> HT_NhomNguoiDung_Quyen { get; set; }
        public virtual DbSet<HT_Quyen> HT_Quyen { get; set; }
        public virtual DbSet<HT_ThongTinCuaHang> HT_ThongTinCuaHang { get; set; }
        public virtual DbSet<DM_TuyenDung> DM_TuyenDung { get; set; }
        public virtual DbSet<DS_HoSoUngTuyen> DS_HoSoUngTuyen { get; set; }
        public virtual DbSet<DS_FileDinhKem> DS_FileDinhKem { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SsoftvnWebContext, Migrations.Configuration>());
            modelBuilder.Entity<DM_DonHang>()
                .HasMany(e => e.DM_DonHangChiTiet)
                .WithRequired(e => e.DM_DonHang)
                .HasForeignKey(e => e.ID_DonHang);

            modelBuilder.Entity<DM_NhomBaiViet>()
                .HasMany(e => e.DM_BaiViet)
                .WithRequired(e => e.DM_NhomBaiViet)
                .HasForeignKey(e => e.ID_NhomBaiViet);

            modelBuilder.Entity<DM_NhomBaiViet>()
                .HasMany(e => e.DM_NhomBaiVietCha)
                .WithOptional(e => e.DM_NhomBaiViet1)
                .HasForeignKey(e => e.ID_NhomCha);

            modelBuilder.Entity<DM_NhomBaiViet>()
                .HasMany(e => e.DM_TuyenDung)
                .WithRequired(e => e.DM_NhomBaiViet)
                .HasForeignKey(e => e.ID_NhomBaiViet);

            modelBuilder.Entity<DM_NhomSanPham>()
                .HasMany(e => e.DM_NhomSanPhamCha)
                .WithOptional(e => e.DM_NhomSanPham1)
                .HasForeignKey(e => e.ID_NhomCha);

            modelBuilder.Entity<DM_NhomSanPham>()
                .HasMany(e => e.DM_SanPham)
                .WithRequired(e => e.DM_NhomSanPham)
                .HasForeignKey(e => e.ID_NhomSanPham);

            modelBuilder.Entity<DM_SanPham>()
                .HasMany(e => e.DM_DonHangChiTiet)
                .WithRequired(e => e.DM_SanPham)
                .HasForeignKey(e => e.ID_SanPham);

            modelBuilder.Entity<DM_Tags>()
                .HasMany(e => e.DM_BaiViet_Tag)
                .WithRequired(e => e.DM_Tags)
                .HasForeignKey(e => e.ID_Tag);

            modelBuilder.Entity<HT_NhomNguoiDung>()
                .HasMany(e => e.HT_NguoiDung)
                .WithRequired(e => e.HT_NhomNguoiDung)
                .HasForeignKey(e => e.ID_NhomNguoiDung);

            modelBuilder.Entity<HT_NhomNguoiDung>()
                .HasMany(e => e.HT_NhomNguoiDung_Quyen)
                .WithRequired(e => e.HT_NhomNguoiDung)
                .HasForeignKey(e => e.ID_NhomNguoiDung);

            modelBuilder.Entity<HT_Quyen>()
                .HasMany(e => e.HT_NhomNguoiDung_Quyen)
                .WithRequired(e => e.HT_Quyen)
                .HasForeignKey(e => e.MaQuyen);

            modelBuilder.Entity<DS_HoSoUngTuyen>()
               .HasMany(e => e.DS_FileDinhKems)
               .WithRequired(e => e.DS_HoSoUngTuyen)
               .HasForeignKey(e => e.ID_HoSoUngTuyen);
            modelBuilder.Entity<DM_TuyenDung>()
              .HasMany(e => e.DS_HoSoUngTuyens)
              .WithRequired(e => e.DM_TuyenDung)
              .HasForeignKey(e => e.ID_TuyenDung);
        }
    }
}
