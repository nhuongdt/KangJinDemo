using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_DoiTuong_Nhom")]
    public partial class DM_DoiTuong_Nhom
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DoiTuong { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhomDoiTuong { get; set; }

        public virtual DM_DoiTuong DM_DoiTuong { get; set; }
        public virtual DM_NhomDoiTuong DM_NhomDoiTuong { get; set; }
    }
}
