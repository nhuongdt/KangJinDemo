using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DM_Menu")]
    public partial class DM_Menu
    {
        [Key]
        public Guid ID { get; set; }
        public string DuongDan { get; set; }
        public string Link { get; set; }
        public int? ThuTuHienThi { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public bool? TrangThai { get; set; }
        public int ID_Loaimenu { get; set; }
        public string KeyWord { get; set; }
    }
}
