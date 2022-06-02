using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("ChietKhauDoanhThu")]
    public partial class ChietKhauDoanhThu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChietKhauDoanhThu()
        {
            ChietKhauDoanhThu_ChiTiet = new HashSet<ChietKhauDoanhThu_ChiTiet>();
            ChietKhauDoanhThu_NhanVien = new HashSet<ChietKhauDoanhThu_NhanVien>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "int")]
        public int TinhChietKhauTheo { get; set; } // (1.ThucThu, 2.DoanhThu) --> nv lap hoadon, 3.theo dichvu --> nv thuchien/tuvan, 4.theo hoadon --> nv ban (F9)

        [Column(TypeName = "datetime")]
        public DateTime ApDungTuNgay { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ApDungDenNgay { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }// 0.Đã xóa

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; } = DateTime.Now;

        [Column(TypeName = "int")]
        public int? LoaiNhanVienApDung { get; set; } = 3; //1, NV bán hàg, 2. Nhân viên thực hiện/ tư vấn dịch vụ, 3. Nhân viên lập hóa đơn

        public virtual DM_DonVi DM_DonVi { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChietKhauDoanhThu_ChiTiet> ChietKhauDoanhThu_ChiTiet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChietKhauDoanhThu_NhanVien> ChietKhauDoanhThu_NhanVien { get; set; }

        [NotMapped]
        public List<ChietKhauDoanhThu_NhanVien> NhanViens { get; set; }
        [NotMapped]
        public List<ChietKhauDoanhThu_ChiTiet> DoanhThuChiTiet{ get; set; }
    }
}
