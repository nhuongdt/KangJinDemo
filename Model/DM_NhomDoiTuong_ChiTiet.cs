using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_NhomDoiTuong_ChiTiet")]
    public partial class DM_NhomDoiTuong_ChiTiet
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhomDoiTuong { get; set; }

        [Column(TypeName = "int")]
        public int? LoaiDieuKien { get; set; } = 0; //1. Tổng mua (Trừ trả hàng), 2. Tổng mua, 3. Thời gian mua hàng, 4. Số lần mua, 5. Công nợ hiện tại, 6. Tháng sinh, 7. Tuổi, 8. Giới tính, 9. Khu vực, 10. Vùng miền

        [Column(TypeName = "int")]
        public int? LoaiSoSanh { get; set; } = 0; //1. >, 2. >=, 3. =, 4. <=, 5. <, 6. Khác

        [Column(TypeName = "float")]
        public double? GiaTriSo { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool? GiaTriBool { get; set; } = false;

        [Column(TypeName = "datetime")]
        public DateTime? GiaTriThoiGian { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? GiaTriKhuVuc { get; set; } //DM_TinhThanh

        [Column(TypeName = "uniqueidentifier")]
        public Guid? GiaTriVungMien { get; set; }

        [Column(TypeName = "int")]
        public int? STT { get; set; } = 0;

        public virtual DM_NhomDoiTuong DM_NhomDoiTuong { get; set; }
        public virtual DM_TinhThanh DM_TinhThanh { get; set; }
        public virtual DM_VungMien DM_VungMien { get; set; }
    }
}
