using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_MayChamCong")]
    public partial class NS_MayChamCong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_MayChamCong()
        {
            NS_DuLieuCongTho = new HashSet<NS_DuLieuCongTho>();
            NS_MaChamCong = new HashSet<NS_MaChamCong>();
            NS_CongBoSung = new HashSet<NS_CongBoSung>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string MaMCC { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenMCC { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenHienThi { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string IP { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_ChiNhanh { get; set; }

        [Column(TypeName = "int")]
        public int LoaiKetNoi { get; set; } = 1; //Kiểu kết nối

        [Column(TypeName = "int")]
        public int CongCOM { get; set; } = 0;

        [Column(TypeName = "int")]
        public int TocDoCOM { get; set; } = 0;

        [Column(TypeName = "nvarchar(max)")]
        public string TenMien { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int LoaiHinh { get; set; } = 1; //kiểu máy

        [Column(TypeName = "nvarchar(max)")]
        public string SoDangKy { get; set; } = "1"; //ID máy

        [Column(TypeName = "nvarchar(max)")]
        public string MatMa { get; set; } = "0";

        [Column(TypeName = "int")]
        public int VaoRa { get; set; } = 1;

        [Column(TypeName = "int")]
        public int Port { get; set; } = 5500;

        [Column(TypeName = "nvarchar(max)")]
        public string SoSeries { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; } = 1; //1 đang hoạt động, 0 - Xóa

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_DuLieuCongTho> NS_DuLieuCongTho { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_MaChamCong> NS_MaChamCong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_CongBoSung> NS_CongBoSung { get; set; }
    }
}
