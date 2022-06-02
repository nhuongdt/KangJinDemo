using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_CongBoSung")]
    public class NS_CongBoSung
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_ChamCongChiTiet { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_MayChamCong { get; set; } //null: Chấm thủ công

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_CaLamViec { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonVi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_BangLuongChiTiet { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayCham { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioVao { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioRa { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioVaoOT { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioRaOT { get; set; }
        
        [Column(TypeName = "int")]
        public int TrangThai { get; set; } // 1.taomoi 2. thuoc tamluu bangluong ,3.daduyetbangluong = dachot bangluong, 4. dathanhtoan bangluong
        // TrangThai (NS_CongBoSung) = TrangThai (NS_BangLuong) + 1

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar")]
        public string KyHieuCong { get; set; }

        [Column(TypeName = "float")]
        public double Cong { get; set; } = 0;

        [Column(TypeName = "int")]
        public int SoPhutDiMuon { get; set; } = 0;

        [Column(TypeName = "float")]
        public double SoGioOT { get; set; } = 0;

        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "float")]
        public double CongQuyDoi { get; set; } = 0; // VD: đi làm ngày thứ 7 chỉ dc 1/2 công, nhưng hệ số lương 

        [Column(TypeName = "int")]
        public int LoaiNgay { get; set; } = 1; //1 - Công ngày thường , 2 - Công Ngày nghỉ, ngày lễ + khác

        [Column(TypeName = "float")]
        public double GioOTQuyDoi { get; set; } = 0;

        [Column(TypeName = "int")]
        public int Thu { get; set; }

        public virtual NS_MayChamCong NS_MayChamCong { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }

        public virtual NS_CaLamViec NS_CaLamViec { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }

        public virtual NS_BangLuong_ChiTiet NS_BangLuong_ChiTiet { get; set; }
    }
}
