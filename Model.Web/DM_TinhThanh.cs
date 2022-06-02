using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DM_TinhThanh")]
    public partial class DM_TinhThanh
    {
        public DM_TinhThanh()
        {
            
        }

        [Key]
        public string MaTinhThanh { get; set; }
        public string TenTinhThanh { get; set; }
        public string GhiChu { get; set; }
        
        public int? Priority { get; set; }

    }
}
