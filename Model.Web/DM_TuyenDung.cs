using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DM_TuyenDung")]
    public partial class DM_TuyenDung
    {
        public DM_TuyenDung()
        {
            DS_HoSoUngTuyens = new HashSet<DS_HoSoUngTuyen>();
        }
        [Key]
        public int ID { get; set; }
        public int ID_NhomBaiViet { get; set; }
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public string QuyenLoi { get; set; }
        public string DiaChi { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
        public string MucLuong { get; set; }
        public DateTime TuNgay { get; set; }
        public string Link { get; set; }
        public DateTime DenNgay { get; set; }
        public DateTime NgayDangBai { get; set; }
        public int? TrangThai { get; set; }
        public int? SoLuong { get; set; }
        public string MaTinhThanh { get; set; }

        public virtual DM_NhomBaiViet DM_NhomBaiViet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DS_HoSoUngTuyen> DS_HoSoUngTuyens { get; set; }
    }
}
