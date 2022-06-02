using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("CSKH_DatLich")]
    public partial class CSKH_DatLich
    {
        public CSKH_DatLich()
        {
            CSKH_DatLich_HangHoa = new HashSet<CSKH_DatLich_HangHoa>();
            CSKH_DatLich_NhanVien = new HashSet<CSKH_DatLich_NhanVien>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid IDDonVi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDDoiTuong { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDXe { get; set; }

        [Column(TypeName = "nvarchar")]
        public string TenDoiTuong { get; set; } = "";

        [Column(TypeName = "nvarchar")]
        public string SoDienThoai { get; set; } = "";

        [Column(TypeName = "nvarchar")]
        public string DiaChi { get; set; } = "";

        [Column(TypeName = "nvarchar")]
        public string BienSo { get; set; } = "";

        [Column(TypeName = "nvarchar")]
        public string LoaiXe { get; set; } = "";

        [Column(TypeName = "datetime")]
        public DateTime? NgaySinh { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime ThoiGian { get; set; } //Thời gian đặt lịch hoặc khách hàng checkin

        [Column(TypeName = "int")]
        public int TrangThai { get; set; } //0. Hủy, 1. Chưa xử lý, 2. Đã liên hệ, 3. Hoàn thành

        [Column(TypeName = "int")]
        public int LoaiDatLich { get; set; } // 1. Đặt lịch, 2. Checkin

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public virtual DM_DonVi DM_DonVi { get; set; }
        public virtual DM_DoiTuong DM_DoiTuong { get; set; }

        public virtual Gara_DanhMucXe Gara_DanhMucXe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CSKH_DatLich_HangHoa> CSKH_DatLich_HangHoa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CSKH_DatLich_NhanVien> CSKH_DatLich_NhanVien { get; set; }

    }
}
