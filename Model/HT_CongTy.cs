using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_CongTy")]
    public partial class HT_CongTy
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string TenCongTy { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string DiaChi { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string SoFax { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string MaSoThue { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string Mail { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string Website { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenGiamDoc { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenKeToanTruong { get; set; } = string.Empty;

        [Column(TypeName = "image")]
        public byte[] Logo { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TaiKhoanNganHang { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NganHang { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string DiaChiNganHang { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenVT { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string DiaChiVT { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool? DangKyNhanSu { get; set; } = false;

        [Column(TypeName = "float")]
        public double? NgayCongChuan { get; set; } = 0;

        [NotMapped]
        public DateTime HanSuDung { get; set; }

        //public string MaKichHoat { get; set; }

        [Column(TypeName = "bit")]
        public bool DangHoatDong { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string ZaloAccessToken { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string ZaloRefreshToken { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string EmailAccount { get; set; }
        
        [Column(TypeName = "nvarchar(max)")]
        public string EmailPassword { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string ZaloCodeVerifier { get; set; }

        public virtual DM_NganHang DM_NganHang { get; set; }
    }
}
