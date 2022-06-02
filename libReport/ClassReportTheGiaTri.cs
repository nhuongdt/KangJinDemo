using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace libReport
{
    public class ClassReportTheGiaTri : ClassReportPublic
    {
        private SsoftvnContext _db;

        public ClassReportTheGiaTri(SsoftvnContext db)
        {
            _db = db;
        }

        public List<SP_ReportValueCard_Balance> GetReportBalance_ValueCard(ParamReportValueCard lstParam)
        {
            string idChiNhanh = lstParam.ID_ChiNhanhs;
            string txtSearch = lstParam.TextSearch;
            if (txtSearch != null)
            {
                txtSearch = txtSearch.Trim();
            }
            string dateFrom = lstParam.DateFrom;
            string dateTo = lstParam.DateTo;
            string status = "%" + lstParam.Status + "%";
            var isChiNhanhs = string.Join(",", lstParam.LstIDChiNhanh);

            List<SqlParameter> paramSql = new List<SqlParameter>();
            paramSql.Add(new SqlParameter("ID_ChiNhanhs", isChiNhanhs));
            paramSql.Add(new SqlParameter("TextSearch", txtSearch));
            paramSql.Add(new SqlParameter("DateFrom", dateFrom));
            paramSql.Add(new SqlParameter("DateTo", dateTo));
            paramSql.Add(new SqlParameter("Status", status));
            paramSql.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
            paramSql.Add(new SqlParameter("PageSize", lstParam.PageSize));

            List<SP_ReportValueCard_Balance> data = _db.Database.SqlQuery<SP_ReportValueCard_Balance>("EXEC ReportValueCard_Balance @TextSearch, @ID_ChiNhanhs, @DateFrom, @DateTo, @Status," +
                "@CurrentPage, @PageSize", paramSql.ToArray()).ToList();
            return data;
        }

        public List<SP_ReportValueCard_HisUsed> GetReportDiary_ValueCard(ParamReportValueCard lstParam)
        {
            string txtSearch = lstParam.TextSearch;
            if (txtSearch != null)
            {
                txtSearch = txtSearch.Trim();
            }
            string dateFrom = lstParam.DateFrom;
            string dateTo = lstParam.DateTo;
            string status = "%" + lstParam.Status + "%";
            var isChiNhanhs = string.Join(",", lstParam.LstIDChiNhanh);

            List<SqlParameter> paramSql = new List<SqlParameter>();
            paramSql.Add(new SqlParameter("ID_ChiNhanhs", isChiNhanhs));
            paramSql.Add(new SqlParameter("TextSearch", txtSearch));
            paramSql.Add(new SqlParameter("DateFrom", dateFrom));
            paramSql.Add(new SqlParameter("DateTo", dateTo));
            paramSql.Add(new SqlParameter("Status", status));
            paramSql.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
            paramSql.Add(new SqlParameter("PageSize", lstParam.PageSize));
            var data = _db.Database.SqlQuery<SP_ReportValueCard_HisUsed>("EXEC ReportValueCard_DiaryUsed @ID_ChiNhanhs, @TextSearch, @DateFrom, @DateTo, @Status," +
                "@CurrentPage, @PageSize", paramSql.ToArray()).ToList();
            return data;
        }

        public List<SP_ReportValueCard_HisUsed> SP_ValueCard_GetListHisUsed(ParamNKyGDV param)
        {
            try
            {
                if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count > 0)
                {
                    var isChiNhanhs = string.Join(",", param.IDChiNhanhs);
                    var idCus = string.Empty;
                    if (param.IDCustomers != null && param.IDCustomers.Count > 0)
                    {
                        idCus = string.Join(",", param.IDCustomers);
                    }

                    List<SqlParameter> paramSql = new List<SqlParameter>();
                    paramSql.Add(new SqlParameter("ID_ChiNhanhs", isChiNhanhs));
                    paramSql.Add(new SqlParameter("ID_KhachHang", idCus));
                    paramSql.Add(new SqlParameter("DateFrom", param.DateFrom));
                    paramSql.Add(new SqlParameter("DateTo", param.DateTo));
                    paramSql.Add(new SqlParameter("Currentpage", param.CurrentPage));
                    paramSql.Add(new SqlParameter("PageSize", param.PageSize));
                    return _db.Database.SqlQuery<SP_ReportValueCard_HisUsed>("EXEC ValueCard_GetListHisUsed @ID_ChiNhanhs, @ID_KhachHang, @DateFrom, @DateTo," +
                        "@Currentpage, @PageSize", paramSql.ToArray()).ToList();
                }
                return new List<SP_ReportValueCard_HisUsed>();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTheGiaTri - ValueCard_GetListHisUsed: " + ex);
                return new List<SP_ReportValueCard_HisUsed>();
            }
        }
        /// <summary>
        /// get lich su nap tien thegiatri
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<TheGiaTriDTO> TheGiaTri_GetLichSuNapTien(ParamNKyGDV param)
        {
            try
            {

                var isChiNhanhs = string.Empty;
                if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count > 0)
                {
                    isChiNhanhs = string.Join(",", param.IDChiNhanhs);
                }
                var idCus = string.Empty;
                if (param.IDCustomers != null && param.IDCustomers.Count > 0)
                {
                    idCus = string.Join(",", param.IDCustomers);
                }

                List<SqlParameter> paramSql = new List<SqlParameter>();
                paramSql.Add(new SqlParameter("IDChiNhanhs", isChiNhanhs ?? (object)DBNull.Value));
                paramSql.Add(new SqlParameter("ID_Cutomer", idCus ?? (object)DBNull.Value));
                paramSql.Add(new SqlParameter("TextSearch", param.TextSearch ?? (object)DBNull.Value));
                paramSql.Add(new SqlParameter("DateFrom", param.DateFrom ?? (object)DBNull.Value));
                paramSql.Add(new SqlParameter("DateTo", param.DateTo ?? (object)DBNull.Value));
                paramSql.Add(new SqlParameter("Currentpage", param.CurrentPage ?? (object)DBNull.Value));
                paramSql.Add(new SqlParameter("PageSize", param.PageSize ?? (object)DBNull.Value));
                return _db.Database.SqlQuery<TheGiaTriDTO>("EXEC TheGiaTri_GetLichSuNapTien @IDChiNhanhs, @ID_Cutomer, @TextSearch, @DateFrom, @DateTo," +
                    "@Currentpage, @PageSize", paramSql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTheGiaTri - ValueCard_GetListHisUsed: " + ex);
                return new List<TheGiaTriDTO>();
            }
        }

        /// <summary>
        /// get nhat ky su dung dich vu cua TheGiaTri
        /// </summary>
        /// <param name="lstParam"></param>
        /// <returns></returns>
        public List<SP_ValueCard_ServiceUsed> SP_ValueCard_ServiceUsed(ParamReportValueCard lstParam)
        {
            try
            {
                var isChiNhanhs = string.Join(",", lstParam.LstIDChiNhanh);
                string txtSearch = lstParam.TextSearch;
                if (txtSearch != null)
                {
                    txtSearch = txtSearch.Trim();
                }
                string dateFrom = lstParam.DateFrom;
                string dateTo = lstParam.DateTo;
                string status = "%" + lstParam.Status + "%";

                List<SqlParameter> paramSql = new List<SqlParameter>();
                paramSql.Add(new SqlParameter("ID_ChiNhanhs", isChiNhanhs));
                paramSql.Add(new SqlParameter("TextSearch", txtSearch));
                paramSql.Add(new SqlParameter("DateFrom", dateFrom));
                paramSql.Add(new SqlParameter("DateTo", dateTo));
                paramSql.Add(new SqlParameter("Status", status));
                paramSql.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
                paramSql.Add(new SqlParameter("PageSize", lstParam.PageSize));

                var data = _db.Database.SqlQuery<SP_ValueCard_ServiceUsed>("EXEC ValueCard_ServiceUsed @ID_ChiNhanhs, @TextSearch, @DateFrom, @DateTo, @Status," +
                    "@CurrentPage, @PageSize", paramSql.ToArray()).ToList();
                if (data != null && data.Count() > 0)
                {
                    // filter by Ma, TenDoiTuong
                    txtSearch = txtSearch.ToLower();
                    char[] whitespace = new char[] { ' ', '\t' };
                    string[] textFilter = txtSearch.Split(whitespace);
                    string[] utf8 = textFilter.Where(o => o.Any(c => VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                    string[] utf = textFilter.Where(o => !o.Any(c => VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                    string txtSearchUnSign = CommonStatic.ConvertToUnSign(txtSearch).ToLower().Trim();

                    // ma,ten doi tuong
                    data = data.Where(o =>
                        o.MaDoiTuong.ToLower().Contains(@txtSearchUnSign)
                        || o.TenDoiTuong.Contains(@txtSearchUnSign.Trim())
                        || (o.TenDoiTuong.ToLower().Contains(txtSearch))
                        || utf.All(d => CommonStatic.ConvertToUnSign(o.TenDoiTuong).ToLower().Contains(d))
                        || o.MaHangHoa.ToLower().Contains(@txtSearchUnSign)
                        || utf.All(d => CommonStatic.ConvertToUnSign(o.TenHangHoa).ToLower().Contains(d))
                        ).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTheGiaTri - ValueCard_ServiceUsed: " + ex.Message);
                return new List<SP_ValueCard_ServiceUsed>();
            }
        }
    }
}
