using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("OptinForm_NgayNghiLe")]
    public partial class OptinForm_NgayNghiLe
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_OptinForm { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayNghiLe { get; set; }

        public virtual OptinForm OptinForm { get; set; }
    }
}
