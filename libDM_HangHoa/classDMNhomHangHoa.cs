using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
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

        public List<NhomHangHoa_TongSuDung> GetTongGiaTriSuDung_ofKhachHang(ParamNKyGDV param)
        {
            string idDonVis = string.Empty;
            if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count > 0)
            {
                idDonVis = string.Join(",", idDonVis);
            }
            if (param.IDCustomers != null && param.IDCustomers.Count > 0)
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("IDDonVis", idDonVis ?? (object)DBNull.Value));
                paramlist.Add(new SqlParameter("ID_KhachHang", param.IDCustomers[0]));
                paramlist.Add(new SqlParameter("ToDate", param.DateTo));
                return db.Database.SqlQuery<NhomHangHoa_TongSuDung>("EXEC dbo.GetTongGiaTriSuDung_ofKhachHang @IDDonVis, @ID_KhachHang, @ToDate", paramlist.ToArray()).ToList();
            }
            return new List<NhomHangHoa_TongSuDung>();
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
