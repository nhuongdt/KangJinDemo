using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_CauHinh_TichDiemApDung")]
    public partial class HT_CauHinh_TichDiemApDung
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_TichDiem { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhomDoiTuong { get; set; }

        public virtual HT_CauHinh_TichDiemChiTiet HT_CauHinh_TichDiemChiTiet { get; set; }
        public virtual DM_NhomDoiTuong DM_NhomDoiTuong { get; set; }
    }
}
