using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_Luong_PhuCap")]
    public partial class NS_Luong_PhuCap
    {
        public NS_Luong_PhuCap()
        {
            NS_ThietLapLuongChiTiet = new HashSet<NS_ThietLapLuongChiTiet>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LoaiLuong { get; set; }

        [Column(TypeName = "int")]
        public int? LoaiLuong { get; set; } 
                                            // 1. lương cố định, 2. lương theo ngày công (luong/ngaycongchuan), 3. lương theo ca, 4. lương theo giờ, 
                                            // 51. phucap theongay, 52. phucap codinh vnd, 53. phucap codinh %luong
                                            // 61. giamtru theo ngay, 62. giamtru codinh vnd, 63. giamtru %luong

        [Column(TypeName = "datetime")]
        public DateTime NgayApDung { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayKetThuc { get; set; }

        [Column(TypeName = "float")]
        public double SoTien { get; set; }

        [Column(TypeName = "float")]
        public double HeSo { get; set; } = 1;

        [Column(TypeName = "nvarchar(max)")]
        public string Bac { get; set; } = string.Empty; // sử dụng để check các kiểu tính lương (1. đc tính OT, 2. không tính OT )
                                                        // VD: Cài đặt tính lương theo ca, và không dc nhân %Hệ số lương khi OT (Loai = 3, Bac = 2)

        [Column(TypeName = "nvarchar(max)")]
        public string NoiDung { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonVi { get; set; }

        public virtual NS_LoaiLuong NS_LoaiLuong { get; set; }
        public virtual NS_NhanVien NS_NhanVien { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_ThietLapLuongChiTiet> NS_ThietLapLuongChiTiet { get; set; }
    }
}
