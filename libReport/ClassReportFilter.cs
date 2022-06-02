using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace libReport
{
    public class ClassReportFilter
    {
        private SsoftvnContext _db;

        public ClassReportFilter(SsoftvnContext db)
        {
            _db = db;
        }

        public List<Report_NhomHangHoa_byName> getList_ID_NhomHangHoa_ByName(List<Report_NhomHangHoa_byName> lst, string TenNhomHang)
        {
            Report_NhomHangHoa DM = new Report_NhomHangHoa();
            var tbl = from nh in _db.DM_NhomHangHoa
                      where nh.TrangThai != true
                      select new Report_NhomHangHoa_byName
                      {
                          ID = nh.ID,
                          TenNhomHang = nh.TenNhomHangHoa,
                          ID_Parent = nh.ID_Parent,
                      };
            var tbl1 = tbl.AsEnumerable().Select(t => new
            {
                ID = t.ID,
                ID_Parent = t.ID_Parent,
                TenNhomHangHoa = t.TenNhomHang,
                //NgayTao = t.NgayTao,
                TenNhomHangHoa_CV = CommonStatic.ConvertToUnSign(t.TenNhomHang).ToLower(),
                TenNhomHangHoa_GC = CommonStatic.GetCharsStart(t.TenNhomHang).ToLower(),
            });
            if (TenNhomHang != null & TenNhomHang != "" & TenNhomHang != "null")
            {
                TenNhomHang = CommonStatic.ConvertToUnSign(TenNhomHang).ToLower();
                tbl1 = tbl1.Where(x => x.TenNhomHangHoa_CV.Contains(@TenNhomHang) || x.TenNhomHangHoa_GC.Contains(@TenNhomHang));
                foreach (var item in tbl1)
                {
                    lst = getList_NhomHangHoa(lst, item.ID, item.TenNhomHangHoa, null);
                }
            }
            else
            {
                lst = tbl.ToList();
            }
            return lst;
        }

        public List<Report_NhomHangHoa_byName> getList_NhomHangHoa(List<Report_NhomHangHoa_byName> lst, Guid ID_NhomHang, string TenNhomHang, Guid? ID_Parent)
        {
            Report_NhomHangHoa_byName DM = new Report_NhomHangHoa_byName();
            if (lst.Count() > 0)
            {
                for (int i = 0; i < lst.Count(); i++)
                {
                    if (lst[i].ID == ID_NhomHang)
                        break;
                    if (i == lst.Count() - 1)
                    {
                        DM.ID = ID_NhomHang;
                        DM.TenNhomHang = TenNhomHang;
                        DM.ID_Parent = ID_Parent;
                        lst.Add(DM);
                    }
                }
            }
            else
            {
                DM.ID = ID_NhomHang;
                DM.TenNhomHang = TenNhomHang;
                DM.ID_Parent = ID_Parent;
                lst.Add(DM);
            }
            var tb1 = from nh1 in _db.DM_NhomHangHoa
                      where nh1.ID_Parent == ID_NhomHang
                      select new Report_NhomHangHoa_byName
                      {
                          ID = nh1.ID,
                          ID_Parent = nh1.ID_Parent,
                          TenNhomHang = nh1.TenNhomHangHoa
                      };
            foreach (var item in tb1)
            {
                lst = getList_NhomHangHoa(lst, item.ID, item.TenNhomHang, item.ID_Parent);
            }
            return lst;
        }

        public List<Report_NhomDoiTuongPRC> getList_NhomDoiTuong(int LoaiDoiTuong)
        {
            List<Report_NhomDoiTuongPRC> lst = new List<Report_NhomDoiTuongPRC>();
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("LoaiDoiTuong", LoaiDoiTuong));
            return _db.Database.SqlQuery<Report_NhomDoiTuongPRC>("exec getList_NhomDoiTuong @LoaiDoiTuong", sql.ToArray()).ToList();
        }

        public List<ListYear> getListYear()
        {
            List<ListYear> lst = new List<ListYear>();
            var tbl = from hd in _db.BH_HoaDon
                      group hd by new
                      {
                          hd.NgayLapHoaDon.Year
                      } into g
                      select new ListYear
                      {
                          Year = g.Key.Year
                      };

            var tblQuy = from hd in _db.Quy_HoaDon
                         group hd by new
                         {
                             hd.NgayLapHoaDon.Year
                         } into g
                         select new ListYear
                         {
                             Year = g.Key.Year
                         };
            var lstUnion = tblQuy.Union(tbl);
            try
            {
                lst = lstUnion.Distinct().OrderByDescending(x => x.Year).ToList();
            }
            catch
            { }
            return lst;
        }
    }
}
