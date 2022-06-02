using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_NguoiDung")]
    public partial class HT_NguoiDung
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HT_NguoiDung()
        {
            HT_NguoiDung_Nhom = new HashSet<HT_NguoiDung_Nhom>();
            HT_QuyenMacDinh = new HashSet<HT_QuyenMacDinh>();
            HT_ThongBao_CaiDat = new HashSet<HT_ThongBao_CaiDat>();
            HeThong_SMS = new HashSet<HeThong_SMS>();
            HeThong_SMS_TaiKhoanChuyen = new HashSet<HeThong_SMS_TaiKhoan>();
            HeThong_SMS_TaiKhoanNhan = new HashSet<HeThong_SMS_TaiKhoan>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVien { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string TaiKhoan { get; set; }

        [Required]
        [StringLength(150)]
        [Column(TypeName = "nvarchar")]
        public string MatKhau { get; set; }

        [Column(TypeName = "bit")]
        public bool LaNhanVien { get; set; }

        [Column(TypeName = "bit")]
        public bool LaAdmin { get; set; }

        [Column(TypeName = "bit")]
        public bool DangHoatDong { get; set; }

        [Column(TypeName = "bit")]
        public bool IsSystem { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        //public string ResetPasswordToken { get; set; }

        //public DateTime? TokenExpire { get; set; }

        [Column(TypeName = "bit")]
        public bool? XemGiaVon { get; set; } = false;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonVi { get; set; }

        [Column(TypeName = "float")]
        public double? SoDuTaiKhoan { get; set; } = 0;

        public virtual DM_DonVi DM_DonVi { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_NguoiDung_Nhom> HT_NguoiDung_Nhom { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_QuyenMacDinh> HT_QuyenMacDinh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_ThongBao_CaiDat> HT_ThongBao_CaiDat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeThong_SMS> HeThong_SMS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeThong_SMS_TaiKhoan> HeThong_SMS_TaiKhoanChuyen { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeThong_SMS_TaiKhoan> HeThong_SMS_TaiKhoanNhan { get; set; }
    }
}
