using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace libHT
{
    public class classNhatKySuDung
    {
        private SsoftvnContext _db;  
        
        public classNhatKySuDung(SsoftvnContext db)
        {
            _db = db;
        }

        public List<LichSuThaoTac> GetListLichSuThaoTac(string ID_NhanVien, Guid ID_ChiNhanh, string NoiDung_Search, string ChucNang_Search, DateTime timeStart, DateTime timeEnd,
            string ThaoTac, string XemDS_PhongBan, string XemDS_HeThong, Guid ID_NguoiDung)
        {
            try
            {
                List<SqlParameter> sqlPRM = new List<SqlParameter>();
                sqlPRM.Add(new SqlParameter("ID_NhanVien", ID_NhanVien.Replace(",null", "")));
                sqlPRM.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sqlPRM.Add(new SqlParameter("NoiDung", NoiDung_Search));
                sqlPRM.Add(new SqlParameter("ChucNang", ChucNang_Search));
                sqlPRM.Add(new SqlParameter("timeStart", timeStart));
                sqlPRM.Add(new SqlParameter("timeEnd", timeEnd));
                sqlPRM.Add(new SqlParameter("ThaoTac", ThaoTac.Replace(",null", "").Replace("null,", "")));
                sqlPRM.Add(new SqlParameter("NhatKy_XemDS_PhongBan", XemDS_PhongBan));
                sqlPRM.Add(new SqlParameter("NhatKy_XemDS_HeThong", XemDS_HeThong));
                sqlPRM.Add(new SqlParameter("ID_NguoiDung", ID_NguoiDung));
                return _db.Database.SqlQuery<LichSuThaoTac>("exec getList_NhatKySuDung @ID_NhanVien, @ID_ChiNhanh, @NoiDung, @ChucNang, @timeStart, @timeEnd, @ThaoTac, @NhatKy_XemDS_PhongBan, @NhatKy_XemDS_HeThong, @ID_NguoiDung", sqlPRM.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libHT - classNhatKySuDung - GetListLichSuThaoTac: " + ex.Message);
                return new List<LichSuThaoTac>();
            }
        }
    }
}
