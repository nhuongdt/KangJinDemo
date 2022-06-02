using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_GiaBan_ApDung")]
    public partial class DM_GiaBan_ApDung
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_GiaBan { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonVi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhomKhachHang { get; set; }

        public virtual DM_GiaBan DM_GiaBan { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }

        public virtual DM_NhomDoiTuong DM_NhomDoiTuong { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }
    }
}
