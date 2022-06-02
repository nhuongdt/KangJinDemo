using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DM_BaiViet_Tag")]
    public partial class DM_BaiViet_Tag
    {
        [Key]
        public Guid ID { get; set; }
        public string ID_Tag { get; set; }
        public int ID_BaiViet { get; set; }
        public int Loai { get; set; }

        public virtual DM_Tags DM_Tags { get; set; }
    }
}
