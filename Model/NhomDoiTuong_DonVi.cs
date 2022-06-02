using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NhomDoiTuong_DonVi")]
    public partial class NhomDoiTuong_DonVi
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhomDoiTuong { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }

        public virtual DM_NhomDoiTuong DM_NhomDoiTuong { get; set; }
    }

    public partial class NhomDoiTuong_DonViDTO
    {
        public Guid ID_NhomDoiTuong { get; set; }
        public Guid ID { get; set; } // iddovi
        public string TenDonVi { get; set; }
    }
}
