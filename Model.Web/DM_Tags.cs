using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DM_Tags")]
    public partial class DM_Tags
    {
        public DM_Tags()
        {
            DM_BaiViet_Tag = new HashSet<DM_BaiViet_Tag>();
        }

        [Key]
        public string ID { get; set; }
        public string TenTheTag { get; set; }
        public string KeyWordTag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_BaiViet_Tag> DM_BaiViet_Tag { get; set; }
    }
}
