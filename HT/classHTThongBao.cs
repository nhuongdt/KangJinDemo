using libHT;
using Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HT
{
    public class classHTThongBao
    {
        private SsoftvnContext _db;
        public classHTThongBao(SsoftvnContext dbcontext)
        {
            _db = dbcontext;
        }
        public List<GetListThongBao> HTGetListThongBao(Guid IdDonVi, string IdNguoiDung, bool NhacSinhNhat, bool NhacTonKho, bool NhacDieuChuyen, bool NhacLoHang, bool NhacBaoDuong, int CurrentPage, int PageSize)
        {
            try
            { 
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("IdDonVi", IdDonVi));
                sql.Add(new SqlParameter("IdNguoiDung", IdNguoiDung));
                sql.Add(new SqlParameter("NhacSinhNhat", NhacSinhNhat));
                sql.Add(new SqlParameter("NhacTonKho", NhacTonKho));
                sql.Add(new SqlParameter("NhacDieuChuyen", NhacDieuChuyen));
                sql.Add(new SqlParameter("NhacLoHang", NhacLoHang));
                sql.Add(new SqlParameter("NhacBaoDuong", NhacBaoDuong));
                sql.Add(new SqlParameter("CurrentPage", CurrentPage));
                sql.Add(new SqlParameter("PageSize", PageSize));
                List<GetListThongBao> xx = _db.Database.SqlQuery<GetListThongBao>("GetListThongBao @IdDonVi, @IdNguoiDung, @NhacSinhNhat, @NhacTonKho, @NhacDieuChuyen, @NhacLoHang, @NhacBaoDuong, @CurrentPage, @PageSize", sql.ToArray()).ToList();
                return xx;
            }
            catch
            {
                return new List<GetListThongBao>();
            }
        }
    }
}
