using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NhomHang_KhoangApDung")]
    public partial class NhomHang_KhoangApDung
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NhomHang_KhoangApDung()
        {

        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        [Column(TypeName = "int")]
        public int STT { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid Id_NhomHang { get; set; }

        [Column(TypeName = "float")]
        public double GiaTriSuDungTu { get; set; }

        [Column(TypeName = "float")]
        public double GiaTriSuDungDen { get; set; } //0. Không giới hạn

        [Column(TypeName = "float")]
        public double GiaTriHoTro { get; set; }

        [Column(TypeName = "int")]
        public int KieuHoTro { get; set; } //1. Phần trăm, 0. VNĐ

        public virtual DM_NhomHangHoa DM_NhomHangHoa { get; set; }
    }
}
