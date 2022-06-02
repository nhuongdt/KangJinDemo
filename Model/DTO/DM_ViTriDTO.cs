using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DM_ViTriDTO
    {
        public Guid ID { get; set; }
        public string MaViTri { get; set; }
        public string TenViTri { get; set; }
        public string GhiChu { get; set; }
        public string TenKhuVuc { get; set; }
        public Guid ID_KhuVuc { get; set; }
    }
}
