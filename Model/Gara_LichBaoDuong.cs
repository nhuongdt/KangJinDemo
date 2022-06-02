using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Gara_LichBaoDuong")]
    public partial class Gara_LichBaoDuong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Gara_LichBaoDuong()
        {
            BH_HoaDon_ChiTiet = new HashSet<BH_HoaDon_ChiTiet>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_HangHoa { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_HoaDon { get; set; }

        [Column(TypeName = "int")]
        public int LanBaoDuong { get; set; } = 0;

        [Column(TypeName = "int")]
        public int SoKmBaoDuong { get; set; } = 0;

        [Column(TypeName = "datetime")]
        public DateTime NgayBaoDuongDuKien { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "int")]
        public int TrangThai { get; set; } //1. Chưa xử lý, 2. Đã xử lý, 3. Đã nhắc, 
                                           // 4. Chuyển từ bảo dưỡng ---> không bảo dưỡng (Xảy ra khi cập nhật hóa đơn: phụ tùng bị xóa)
                                           // 5. quá hạn bảo dưỡng: vd: làm hóa đơn, chọn bảo dưỡng lần 2 --> cập nhật lần 1: quá hạn

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_Xe { get; set; }

        [Column(TypeName = "nvarchar")]
        public string GhiChu { get; set; } = "";

        [Column(TypeName = "int")]
        public int? LanNhac { get; set; } = 0; //Số lần đã nhắc khách hàng đến bảo  dưỡng

        public virtual DM_HangHoa DM_HangHoa { get; set; }
        public virtual BH_HoaDon BH_HoaDon { get; set; }
        public virtual Gara_DanhMucXe Gara_DanhMucXe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon_ChiTiet> BH_HoaDon_ChiTiet { get; set; }
    }
}
