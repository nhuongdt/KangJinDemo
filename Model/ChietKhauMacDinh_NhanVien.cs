using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("ChietKhauMacDinh_NhanVien")]
    public partial class ChietKhauMacDinh_NhanVien
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "float")]
        public double ChietKhau { get; set; } //thuc hien

        [Column(TypeName = "bit")]
        public bool LaPhanTram { get; set; }

        [Column(TypeName = "float")]
        public double ChietKhau_YeuCau { get; set; } // yeu cau

        [Column(TypeName = "bit")]
        public bool LaPhanTram_YeuCau { get; set; }

        [Column(TypeName = "float")]
        public double ChietKhau_TuVan { get; set; } // tu van

        [Column(TypeName = "bit")]
        public bool LaPhanTram_TuVan { get; set; }

        [Column(TypeName = "float")]
        public double? ChietKhau_BanGoi { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool? LaPhanTram_BanGoi { get; set; } = false;

        [Column(TypeName = "int")]
        public int? TheoChietKhau_ThucHien { get; set; } = 0; // default TheoCKThucHien = hoahong thuchien + %/vnd

        [Column(TypeName = "datetime")]
        public DateTime? NgayNhap { get; set; }

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonViQuiDoi { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }

        public virtual DonViQuiDoi DonViQuiDoi { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }
    }
}
