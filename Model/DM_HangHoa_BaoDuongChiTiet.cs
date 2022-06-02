using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_HangHoa_BaoDuongChiTiet")]
    public partial class DM_HangHoa_BaoDuongChiTiet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_HangHoa_BaoDuongChiTiet()
        {

        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_HangHoa { get; set; }

        [Column(TypeName = "int")]
        public int LanBaoDuong { get; set; } // Lần 1, 2, 3

        [Column(TypeName = "float")]
        public double GiaTri { get; set; } // Số km hoặc thời gian tính bảo dưỡng

        [Column(TypeName = "int")]
        public int LoaiGiaTri { get; set; } //1. Ngày, 2. Tháng, 3. Năm, 4. Km

        [Column(TypeName = "int")]
        public int? BaoDuongLapDinhKy { get; set; } = 0;

        public virtual DM_HangHoa DM_HangHoa { get; set; }
    }
}
