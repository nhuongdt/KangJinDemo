using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_GiaBan_ChiTiet")]
    public partial class DM_GiaBan_ChiTiet
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_GiaBan { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_KhoHang { get; set; }

        [Column(TypeName = "float")]
        public double GiaBan { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NgoaiTe { get; set; }

        [Column(TypeName = "int")]
        public int? PhuongThucTinhGiaBan { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool? TheoPT { get; set; } = false;

        [Column(TypeName = "datetime")]
        public DateTime? NgayNhap { get; set; }

        [Required]
        public Guid ID_DonViQuiDoi { get; set; }

        public virtual DM_GiaBan DM_GiaBan { get; set; }

        public virtual DM_Kho DM_Kho { get; set; }

        public virtual DM_TienTe DM_TienTe { get; set; }

        public virtual DonViQuiDoi DonViQuiDoi { get; set; }
    }
}
