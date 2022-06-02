using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DS_FileDinhKem")]
    public partial class DS_FileDinhKem
    {
        [Key]
        public int ID { get; set; }

        public string TenFile { get; set; }

        public string LinkFile { get; set; }


        public int Size { get; set; }

        public int ID_HoSoUngTuyen { get; set; }

        public virtual DS_HoSoUngTuyen DS_HoSoUngTuyen { get; set; }
    }
}
