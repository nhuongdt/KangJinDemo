using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace libDM_HangHoa
{
    public class classDMNhomHangHoa
    {
        private SsoftvnContext db;
        public classDMNhomHangHoa(SsoftvnContext _db)
        {
            db = _db;
        }
        public List<DMNhomHangHoa> GetDMNhomHangHoas()
        {
            var nhh = db.DM_NhomHangHoa.Where(p => p.TrangThai != true).OrderByDescending(p => p.NgayTao).Select(p => new DMNhomHangHoa
            {
                ID = p.ID,
                Text = p.TenNhomHangHoa,
                ParentId = p.ID_Parent
            }).ToList();
            return nhh;
        }
    }

    public class DMNhomHangHoa
    {
        public Guid ID { get; set; }
        public string Text { get; set; }
        public Guid? ParentId { get; set; }
        //public List<DMNhomHangHoa> Children { get; set; }
    }
}
