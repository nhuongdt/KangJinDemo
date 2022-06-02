using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_NhatKySuDung_Backup")]
    public partial class HT_NhatKySuDung_Backup
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string ChucNang { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime ThoiGian { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NoiDung { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string NoiDungChiTiet { get; set; } = string.Empty; //save as html

        [Column(TypeName = "int")]
        public int LoaiNhatKy { get; set; } = 1; // 1.Insert, 2.Update, 3.Delete, 4.Huy, 5.Import, 6.Export, 7.Login, 8.HD delete & update

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_HoaDon { get; set; }

        [Column(TypeName = "int")]
        public int? LoaiHoaDon { get; set; } = 0;

        [Column(TypeName = "datetime")]
        public DateTime? ThoiGianUpdateGV { get; set; }
    }
}
