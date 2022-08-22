using System;
using Model;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace libGara
{
    public class ClassPhieuTiepNhan
    {
        private SsoftvnContext _db;
        public ClassPhieuTiepNhan(SsoftvnContext dbcontext)
        {
            _db = dbcontext;
        }

        public Gara_PhieuTiepNhan GetGara_PhieuTiepNhanById(Guid id)
        {
            return _db.Gara_PhieuTiepNhan.Where(p => p.ID == id).FirstOrDefault();
        }
        public bool CheckExist_MaPhieuTN(string maPhieuTN, Guid? id = null)
        {
            if (string.IsNullOrEmpty(maPhieuTN))
            {
                return false;
            }
            else
            {
                maPhieuTN = maPhieuTN.Trim();
                if (id != null && id != Guid.Empty)
                {
                    return _db.Gara_PhieuTiepNhan.Where(p => p.ID != id && p.MaPhieuTiepNhan == maPhieuTN).Count() > 0;
                }
                else
                {
                    return _db.Gara_PhieuTiepNhan.Where(p => p.MaPhieuTiepNhan == maPhieuTN).Count() > 0;
                }
            }
        }
        public string SP_GetPhieuTiepNhan_byTemp(int? loaiHoaDon, Guid idDonVi, DateTime ngayLapHoaDon)
        {
            string mahoadon = string.Empty;
            try
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("LoaiHoaDon", loaiHoaDon));
                lstParam.Add(new SqlParameter("ID_DonVi", idDonVi));
                lstParam.Add(new SqlParameter("NgayLapHoaDon", ngayLapHoaDon));
                mahoadon = _db.Database.SqlQuery<string>("EXEC GetMaMaPhieuTiepNhan_byTemp @LoaiHoaDon, @ID_DonVi, @NgayLapHoaDon", lstParam.ToArray()).FirstOrDefault().ToString();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ClassBH_HoaDon.SP_GetMaHoaDon_byTemp: " + ex.InnerException + ex.Message + ex.HResult);
                return string.Empty;
            }
            return mahoadon;
        }

        public List<DTOInt> PTN_CheckChangeCus(Gara_PhieuTiepNhan paramNew)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_PhieuTiepNhan", paramNew.ID));
            lstParam.Add(new SqlParameter("ID_KhachHangNew", paramNew.ID_KhachHang));
            lstParam.Add(new SqlParameter("ID_BaoHiemNew", paramNew.ID_BaoHiem ?? (object)DBNull.Value));
            return _db.Database.SqlQuery<DTOInt>("EXEC PTN_CheckChangeCus @ID_PhieuTiepNhan, @ID_KhachHangNew, @ID_BaoHiemNew", lstParam.ToArray()).ToList();
        }

        public void ChangePTN_updateCus(Gara_PhieuTiepNhan ptnOld, List<int> arrType)
        {
            var sType = string.Empty;
            if (arrType != null && arrType.Count() > 0)
            {
                sType = string.Join(",", arrType);
            }
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_PhieuTiepNhan", ptnOld.ID));
            lstParam.Add(new SqlParameter("ID_KhachHangOld", ptnOld.ID_KhachHang));
            lstParam.Add(new SqlParameter("ID_BaoHiemOld", ptnOld.ID_BaoHiem ?? (object)DBNull.Value));
            lstParam.Add(new SqlParameter("Types", sType));
            _db.Database.ExecuteSqlCommand("EXEC ChangePTN_updateCus @ID_PhieuTiepNhan, @ID_KhachHangOld, @ID_BaoHiemOld, @Types", lstParam.ToArray());
        }
        /// <summary>
        /// Tìm phiếu tiếp nhận by MaPhieu, BienSoXe hoặc IDKhachHang
        /// </summary>
        /// <param name="idChiNhanh"></param>
        /// <param name="text"></param>
        /// <param name="cusID"></param>
        /// <returns></returns>
        public List<GetListPhieuTiepNhan_v2> JqAuto_PhieuTiepNhan(ParamSearch param)
        {
            var idChiNhanh = string.Join(",", param.LstIDChiNhanh);
            var txt = "%" + param.TextSearch + "%";
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idChiNhanh));
            sql.Add(new SqlParameter("TextSearch", txt));
            sql.Add(new SqlParameter("CustomerID", param.ID_HangXe));
            List<GetListPhieuTiepNhan_v2> xx = _db.Database.SqlQuery<GetListPhieuTiepNhan_v2>("exec JqAuto_PhieuTiepNhan @IDChiNhanhs, @TextSearch, @CustomerID", sql.ToArray()).ToList();
            return xx;
        }
        public List<PhieuTiepNhan> GetListPhieuTiepNhan(ParamSearch param)
        {
            var idChiNhanh = string.Join(",", param.LstIDChiNhanh);
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idChiNhanh));
            sql.Add(new SqlParameter("ID_HangXe", param.ID_HangXe));
            sql.Add(new SqlParameter("ID_LoaiXe", param.ID_LoaiXe));
            sql.Add(new SqlParameter("NamSanXuat", param.NamSanXuat));
            sql.Add(new SqlParameter("TextSearch", param.TextSearch));
            sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
            sql.Add(new SqlParameter("PageSize", param.PageSize));
            List<PhieuTiepNhan> xx = _db.Database.SqlQuery<PhieuTiepNhan>(" GetListPhieuTiepNhan @IDChiNhanhs, @ID_HangXe, @ID_LoaiXe, @NamSanXuat, @TextSearch, @CurrentPage, @PageSize", sql.ToArray()).ToList();
            return xx;
        }

        public List<GetListPhieuTiepNhan_v2> GetListPhieuTiepNhan_v2(ParamGetListPhieuTiepNhan_v2 param)
        {
            string idChiNhanh = "";
            if (param.IdChiNhanhs.Count > 0)
            {
                idChiNhanh = string.Join(",", param.IdChiNhanhs);
            }

            string strTrangThai = "";
            if (param.TrangThais.Count > 0)
            {
                strTrangThai = string.Join(",", param.TrangThais);
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IdChiNhanhs", idChiNhanh));
            sql.Add(new SqlParameter("NgayTiepNhan_From", param.NgayTiepNhan_From == null ? (object)DBNull.Value : param.NgayTiepNhan_From.Value));
            sql.Add(new SqlParameter("NgayTiepNhan_To", param.NgayTiepNhan_To == null ? (object)DBNull.Value : param.NgayTiepNhan_To.Value));
            sql.Add(new SqlParameter("NgayXuatXuongDuKien_From", param.NgayXuatXuongDuKien_From == null ? (object)DBNull.Value : param.NgayXuatXuongDuKien_From.Value));
            sql.Add(new SqlParameter("NgayXuatXuongDuKien_To", param.NgayXuatXuongDuKien_To == null ? (object)DBNull.Value : param.NgayXuatXuongDuKien_To.Value));
            sql.Add(new SqlParameter("NgayXuatXuong_From", param.NgayXuatXuong_From == null ? (object)DBNull.Value : param.NgayXuatXuong_From.Value));
            sql.Add(new SqlParameter("NgayXuatXuong_To", param.NgayXuatXuong_To == null ? (object)DBNull.Value : param.NgayXuatXuong_To.Value));
            sql.Add(new SqlParameter("TrangThais", strTrangThai));
            sql.Add(new SqlParameter("TextSearch", param.TextSearch));
            sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
            sql.Add(new SqlParameter("PageSize", param.PageSize));
            sql.Add(new SqlParameter("BaoHiem", param.BaoHiem));

            string sqlquery = "GetListPhieuTiepNhan_v2 @IdChiNhanhs, @NgayTiepNhan_From, @NgayTiepNhan_To, " +
                "@NgayXuatXuongDuKien_From, @NgayXuatXuongDuKien_To, @NgayXuatXuong_From, @NgayXuatXuong_To, " +
                "@TrangThais, @TextSearch, @CurrentPage, @PageSize, @BaoHiem";
            List<GetListPhieuTiepNhan_v2> xx = _db.Database.SqlQuery<GetListPhieuTiepNhan_v2>(sqlquery, sql.ToArray()).ToList();
            return xx;
        }

        public List<Gara_BaoGia> Gara_GetListBaoGia(ParamSearch param)
        {
            var idChiNhanh = string.Join(",", param.LstIDChiNhanh);
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idChiNhanh));
            sql.Add(new SqlParameter("FromDate", param.FromDate));
            sql.Add(new SqlParameter("ToDate", param.ToDate));
            sql.Add(new SqlParameter("ID_PhieuSuaChua", param.ID_HangXe));// muon tam truong
            sql.Add(new SqlParameter("TrangThais", param.TrangThai));
            sql.Add(new SqlParameter("TextSearch", param.TextSearch));
            sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
            sql.Add(new SqlParameter("PageSize", param.PageSize));
            List<Gara_BaoGia> xx = _db.Database.SqlQuery<Gara_BaoGia>("exec Gara_GetListBaoGia @IDChiNhanhs, @FromDate, @ToDate, @ID_PhieuSuaChua," +
                " @TrangThais, @TextSearch, @CurrentPage, @PageSize", sql.ToArray()).ToList();
            return xx;
        }
        public List<Gara_BaoGia> Gara_GetListHoaDonSuaChua(ParamSearch param)
        {
            var idChiNhanh = string.Join(",", param.LstIDChiNhanh);
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idChiNhanh));
            sql.Add(new SqlParameter("FromDate", param.FromDate));
            sql.Add(new SqlParameter("ToDate", param.ToDate));
            sql.Add(new SqlParameter("ID_PhieuSuaChua", param.ID_HangXe));// muon tam truong
            sql.Add(new SqlParameter("TrangThai", param.TrangThai));
            sql.Add(new SqlParameter("TextSearch", param.TextSearch));
            sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
            sql.Add(new SqlParameter("PageSize", param.PageSize));
            sql.Add(new SqlParameter("IDXe", param.IDXe == "" ? (object)DBNull.Value : new Guid(param.IDXe)));
            List<Gara_BaoGia> xx = _db.Database.SqlQuery<Gara_BaoGia>("exec GetListHoaDonSuaChua @IDChiNhanhs, @FromDate, @ToDate," +
                "@ID_PhieuSuaChua, @IDXe, @TrangThai, @TextSearch, @CurrentPage, @PageSize", sql.ToArray()).ToList();
            return xx;
        }

        public List<GetListPhieuNhapXuatKhoByIDPhieuTiepNhan> Gara_GetListPhieuNhapXuatKho(ParamGetListPhieuNhapXuatKhoByIDPhieuTiepNhan param)
        {
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDPhieuTiepNhan", param.IDPhieuTiepNhan));// muon tam truong
            sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
            sql.Add(new SqlParameter("PageSize", param.PageSize));
            List<GetListPhieuNhapXuatKhoByIDPhieuTiepNhan> xx = _db.Database.SqlQuery<GetListPhieuNhapXuatKhoByIDPhieuTiepNhan>(" GetListPhieuNhapXuatKhoByIDPhieuTiepNhan @IDPhieuTiepNhan, @CurrentPage, @PageSize", sql.ToArray()).ToList();
            return xx;
        }

        //public List<Gara_BaoGia> Gara_GetListHoaDonSuaChuaByIDXe(ParamSearch param)
        //{
        //    var idChiNhanh = string.Join(",", param.LstIDChiNhanh);
        //    List<SqlParameter> sql = new List<SqlParameter>();
        //    sql.Add(new SqlParameter("IDChiNhanhs", idChiNhanh));
        //    sql.Add(new SqlParameter("FromDate", param.FromDate));
        //    sql.Add(new SqlParameter("ToDate", param.ToDate));
        //    sql.Add(new SqlParameter("ID_PhieuSuaChua", param.ID_HangXe));// muon tam truong
        //    sql.Add(new SqlParameter("TrangThai", param.TrangThai));
        //    sql.Add(new SqlParameter("TextSearch", param.TextSearch));
        //    sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
        //    sql.Add(new SqlParameter("PageSize", param.PageSize));
        //    sql.Add(new SqlParameter("IDXe", (object)DBNull.Value));
        //    List<Gara_BaoGia> xx = _db.Database.SqlQuery<Gara_BaoGia>(" GetListHoaDonSuaChua @IDChiNhanhs, @FromDate, @ToDate," +
        //        "@ID_PhieuSuaChua, @IDXe, @TrangThai, @TextSearch, @CurrentPage, @PageSize", sql.ToArray()).ToList();
        //    return xx;
        //}
        public List<Gara_BaoGia> GetListBaoGia_AfterXuLy(ParamSearch param)
        {
            var idChiNhanh = string.Join(",", param.LstIDChiNhanh);
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idChiNhanh));
            sql.Add(new SqlParameter("FromDate", param.FromDate));
            sql.Add(new SqlParameter("ToDate", param.ToDate));
            sql.Add(new SqlParameter("TrangThais", param.TrangThai));
            sql.Add(new SqlParameter("TextSearch", param.TextSearch));
            sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
            sql.Add(new SqlParameter("PageSize", param.PageSize));
            List<Gara_BaoGia> xx = _db.Database.SqlQuery<Gara_BaoGia>(" exec GetListBaoGia_AfterXuLy @IDChiNhanhs, @FromDate, @ToDate," +
                " @TrangThais, @TextSearch, @CurrentPage, @PageSize", sql.ToArray()).ToList();
            return xx;
        }

        public List<GetListPhieuTiepNhan_v2> PhieuTiepNhan_GetThongTinChiTiet(Guid id)
        {
            SqlParameter sql = new SqlParameter("ID_PhieuTiepNhan", id);
            List<GetListPhieuTiepNhan_v2> xx = _db.Database.SqlQuery<GetListPhieuTiepNhan_v2>("exec PhieuTiepNhan_GetThongTinChiTiet @ID_PhieuTiepNhan", sql).ToList();
            return xx;
        }

        public List<PhieuTiepNhan_TinhTrang> PhieuTiepNhan_GetHangMucSuaChua(Guid id)
        {
            return _db.Gara_HangMucSuaChua.Where(x => x.ID_PhieuTiepNhan == id)
                .Select(x => new PhieuTiepNhan_TinhTrang
                {
                    ID = x.ID,
                    ID_PhieuTiepNhan = x.ID_PhieuTiepNhan,
                    TenHangMuc = x.TenHangMuc,
                    TinhTrang = x.TinhTrang,
                    PhuongAnSuaChua = x.PhuongAnSuaChua,
                    TrangThai = x.TrangThai,
                    Anh = x.Anh
                }).ToList();
        }
        public List<PhieuTiepNhan_VatDungDinhKem> PhieuTiepNhan_GetVatDungKemTheo(Guid id)
        {
            return _db.Gara_GiayToKemTheo.Where(x => x.ID_PhieuTiepNhan == id)
                 .Select(x => new PhieuTiepNhan_VatDungDinhKem
                 {
                     ID = x.ID,
                     ID_PhieuTiepNhan = x.ID_PhieuTiepNhan,
                     TieuDe = x.TieuDe,
                     SoLuong = x.SoLuong,
                     FileDinhKem = x.FileDinhKem,
                     TrangThai = x.TrangThai,
                 }).ToList();
        }

        public void AddPhieuTiepNhan(Gara_PhieuTiepNhan obj)
        {
            _db.Gara_PhieuTiepNhan.Add(obj);
            _db.SaveChanges();
        }

        public void UpdatePhieuTiepNhan(Gara_PhieuTiepNhan obj)
        {
            Gara_PhieuTiepNhan xe = _db.Gara_PhieuTiepNhan.Find(obj.ID);
            xe.ID_DonVi = obj.ID_DonVi;
            xe.MaPhieuTiepNhan = obj.MaPhieuTiepNhan;
            xe.NgayVaoXuong = obj.NgayVaoXuong;
            xe.ID_NhanVien = obj.ID_NhanVien;
            xe.ID_CoVanDichVu = obj.ID_CoVanDichVu;
            xe.ID_Xe = obj.ID_Xe;
            xe.ID_KhachHang = obj.ID_KhachHang;
            xe.TenLienHe = obj.TenLienHe;
            xe.SoDienThoaiLienHe = obj.SoDienThoaiLienHe;
            xe.ID_BaoHiem = obj.ID_BaoHiem;
            xe.NguoiLienHeBH = obj.NguoiLienHeBH;
            xe.SoDienThoaiLienHeBH = obj.SoDienThoaiLienHeBH;
            xe.SoKmVao = obj.SoKmVao;
            xe.SoKmRa = obj.SoKmRa;
            xe.NgayXuatXuong = obj.NgayXuatXuong;
            xe.NgayXuatXuongDuKien = obj.NgayXuatXuongDuKien;
            xe.GhiChu = obj.GhiChu;
            xe.NgaySua = DateTime.Now;
            xe.NguoiSua = obj.NguoiSua;
            xe.TrangThai = obj.TrangThai;
            _db.SaveChanges();
        }
        public void PhieuTN_XuatXuong(Gara_PhieuTiepNhan obj)
        {
            Gara_PhieuTiepNhan xe = _db.Gara_PhieuTiepNhan.Find(obj.ID);
            xe.SoKmRa = obj.SoKmRa;
            xe.NgayXuatXuong = obj.NgayXuatXuong;
            xe.XuatXuong_GhiChu = obj.XuatXuong_GhiChu;
            xe.TrangThai = 3;
            _db.SaveChanges();
        }

        public void PhieuTiepNhan_UpdateTrangThai(Guid id, int status)
        {
            var phieuTN = _db.Gara_PhieuTiepNhan.Find(id);
            phieuTN.TrangThai = status;
            switch (status)
            {
                case 0:
                    // huy phieuTN --> huybaogia
                    (_db.BH_HoaDon.Where(x => x.ID_PhieuTiepNhan == id && x.LoaiHoaDon == 3)).ToList()
                        .ForEach(x =>
                        {
                            x.ChoThanhToan = null;
                            x.YeuCau = "4";
                        });
                    break;
                case 1:// xuatxuong ---> dangsua
                    phieuTN.NgayXuatXuong = null;
                    break;
            }
            _db.SaveChanges();
        }
        public void UpdateLichBaoDuong_whenUpdateSoKM_ofPhieuTN(Guid idPhieuTN, float chenhLechKM)
        {
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("@ID_PhieuTiepNhan", idPhieuTN));
            sql.Add(new SqlParameter("@ChenhLech_SoKM", chenhLechKM));
            _db.Database.ExecuteSqlCommand("exec UpdateLichBaoDuong_whenUpdateSoKM_ofPhieuTN @ID_PhieuTiepNhan, @ChenhLech_SoKM", sql.ToArray());
        }
        public void AddHangMucSuaChua(List<Gara_HangMucSuaChua> lst)
        {
            _db.Gara_HangMucSuaChua.AddRange(lst);
            _db.SaveChanges();
        }

        public void RemoveHangMucSuaChua(Guid idPhieuTN)
        {
            var lst = _db.Gara_HangMucSuaChua.Where(x => x.ID_PhieuTiepNhan == idPhieuTN);
            _db.Gara_HangMucSuaChua.RemoveRange(lst);
            _db.SaveChanges();
        }

        public void AddVatDungKemTheo(List<Gara_GiayToKemTheo> lst)
        {
            _db.Gara_GiayToKemTheo.AddRange(lst);
            _db.SaveChanges();
        }

        public void RemoveVatDungKemTheo(Guid idPhieuTN)
        {
            var lst = _db.Gara_GiayToKemTheo.Where(x => x.ID_PhieuTiepNhan == idPhieuTN);
            _db.Gara_GiayToKemTheo.RemoveRange(lst);
            _db.SaveChanges();
        }
        public Param_XuatKhoToanBo XuatKhoToanBo_FromHoaDonSC(Guid idHoaDon)
        {
            SqlParameter sql = new SqlParameter("ID_HoaDon", idHoaDon);
            return _db.Database.SqlQuery<Param_XuatKhoToanBo>("XuatKhoToanBo_FromHoaDonSC @ID_HoaDon", sql).FirstOrDefault();
        }

        public List<XuatKho_JqautoHDSC> JqAuto_HoaDonSC(ParamSearch param)
        {
            var idChiNhanh = string.Join(",", param.LstIDChiNhanh);
            var txt = "%" + param.TextSearch + "%";
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idChiNhanh));
            sql.Add(new SqlParameter("ID_PhieuTiepNhan", param.ID_HangXe));// muontam truong
            sql.Add(new SqlParameter("TextSearch", txt));
            List<XuatKho_JqautoHDSC> xx = _db.Database.SqlQuery<XuatKho_JqautoHDSC>(" exec JqAuto_HoaDonSC @IDChiNhanhs, @ID_PhieuTiepNhan, @TextSearch ", sql.ToArray()).ToList();
            return xx;
        }

        public bool CheckBaoGia_DaTaoHoaDonSuaChua_VaXuatKho(Guid idDatHang)
        {
            // get hoadon dc xuly tu baogia
            var lsthd = (from hd in _db.BH_HoaDon
                         where hd.ID_HoaDon == idDatHang && hd.LoaiHoaDon == 25 && hd.ChoThanhToan != null
                         select hd.ID).ToList();

            if (lsthd != null && lsthd.Count() > 0)
            {
                // get phieuxuatkho tu hoadon
                var xk = (from hd in _db.BH_HoaDon
                          where hd.LoaiHoaDon == 8
                          && hd.ChoThanhToan != null
                         && _db.BH_HoaDon.Any(x => lsthd.Contains(x.ID_HoaDon ?? Guid.Empty) == true)
                          select new { hd.ID }).Count();
                return xk > 0;
            }
            return false;
        }

        public List<PhuTung_LichBaoDuong> GetLichBaoDuong(ParamSeachLichBaoDuong param)
        {
            var idChiNhanhs = string.Empty;
            if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count > 0)
            {
                idChiNhanhs = string.Join(",", param.IDChiNhanhs);
            }
            var idNhanViens = string.Empty;
            if (param.IDNhanVienPhuTrachs != null && param.IDNhanVienPhuTrachs.Count > 0)
            {
                idNhanViens = string.Join(",", param.IDNhanVienPhuTrachs);
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idChiNhanhs ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("TextSeach", param.TextSeach ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("NgayBaoDuongFrom", param.NgayBaoDuongFrom ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("NgayBaoDuongTo", param.NgayBaoDuongTo ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("NgayNhacFrom", param.NgayNhacFrom ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("NgayNhacTo", param.NgayNhacTo ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("IDNhanVienPhuTrachs", idNhanViens ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("IDNhomHangs", param.IDNhomHangs ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("ID_Xe", param.ID_Xe ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("ID_PhieuTiepNhan", param.ID_PhieuTiepNhan ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("LanNhacs", param.LanNhacs ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("TrangThais", param.TrangThais ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("CurrentPage", param.CurrentPage ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("PageSize", param.PageSize ?? (object)DBNull.Value));
            return _db.Database.SqlQuery<PhuTung_LichBaoDuong>("exec dbo.[GetLichNhacBaoDuong] @IDChiNhanhs,@TextSeach, @NgayBaoDuongFrom," +
                "@NgayBaoDuongTo, @NgayNhacFrom, @NgayNhacTo, @IDNhanVienPhuTrachs, @IDNhomHangs," +
                "@ID_Xe, @ID_PhieuTiepNhan, @LanNhacs, @TrangThais, @CurrentPage, @PageSize", sql.ToArray()).ToList();
        }
        public List<NhatKyBaoDuong> GetNhatKyBaoDuong_byCar(Guid idCar)
        {
            SqlParameter param = new SqlParameter("ID_Xe", idCar);
            return _db.Database.SqlQuery<NhatKyBaoDuong>("exec GetNhatKyBaoDuong_byCar @ID_Xe", param).ToList();
        }
        public void Insert_LichNhacBaoDuong(Guid idHoaDon)
        {
            SqlParameter param = new SqlParameter("ID_HoaDon", idHoaDon);
            _db.Database.ExecuteSqlCommand("exec Insert_LichNhacBaoDuong @ID_HoaDon", param);
        }

        public void UpdateLichBD_whenChangeNgayLapHD(Guid idHoaDon, DateTime ngaylapOld, DateTime ngaylapNew)
        {
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("ID_HoaDon", idHoaDon));
            sql.Add(new SqlParameter("NgayLapHDOld", ngaylapOld));
            sql.Add(new SqlParameter("NgayLapNew", ngaylapNew));
            _db.Database.ExecuteSqlCommand("exec UpdateLichBD_whenChangeNgayLapHD @ID_HoaDon, @NgayLapHDOld, @NgayLapNew", sql.ToArray());
        }
        public void UpdateHD_UpdateLichBaoDuong(ParamUpdateLichBaoDuong param)
        {
            if (param.IDHangHoas != null && param.IDHangHoas.Count > 0)
            {
                var idProducts = string.Join(",", param.IDHangHoas);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("ID_HoaDon", param.ID_HoaDon));
                sql.Add(new SqlParameter("IDHangHoas", idProducts));
                sql.Add(new SqlParameter("NgayLapHDOld", param.NgayLapHoaDonOld));
                _db.Database.ExecuteSqlCommand("exec UpdateHD_UpdateLichBaoDuong @ID_HoaDon, @IDHangHoas, @NgayLapHDOld", sql.ToArray());
            }
        }
        public void InsertLichNhacBaoDuong_whenQuaHan_orEnoughLanNhac(Guid idLichNhac, int typeUpdate = 0)
        {
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("ID_LichBaoDuong", idLichNhac));
            sql.Add(new SqlParameter("TypeUpdate", typeUpdate));
            _db.Database.ExecuteSqlCommand("exec InsertLichNhacBaoDuong_whenQuaHan_orEnoughLanNhac @ID_LichBaoDuong, @TypeUpdate", sql.ToArray());
        }
        public void SuDungBaoDuong_UpdateStatus(List<Guid> arrIDLich, int status)
        {
            if (arrIDLich != null && arrIDLich.Count > 0)
            {
                var idLichs = string.Join(",", arrIDLich);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("IDLichNhacs", idLichs));
                sql.Add(new SqlParameter("Status", status));
                _db.Database.ExecuteSqlCommand("exec SuDungBaoDuong_UpdateStatus @IDLichNhacs, @Status", sql.ToArray());
            }
        }
    }
    public class XuatKho_JqautoHDSC
    {
        public Guid ID { get; set; }
        public Guid ID_PhieuTiepNhan { get; set; }
        public string MaHoaDon { get; set; }
        public string BienSo { get; set; }
        public string MaPhieuTiepNhan { get; set; }
    }

    public class PhieuTiepNhan
    {
        public Guid ID { get; set; }
        public Guid ID_Xe { get; set; }
        public string MaPhieuTiepNhan { get; set; }
        public DateTime? NgayVaoXuong { get; set; }
        public DateTime? NgayXuatXuongDuKien { get; set; }
        public string NhanVienTiepNhan { get; set; }
        public string CoVanDichVu { get; set; }
        public string TenLienHe { get; set; }
        public string SoDienThoaiLienHe { get; set; }
        public string GhiChu { get; set; }
        public string BienSo { get; set; }// biển số xe
        public string SoKhung { get; set; }
        public string SoMay { get; set; }
        public int? SoKmVao { get; set; }
        public int? SoKmRa { get; set; }
        public string TenLoaiXe { get; set; }
        public string TenMauXe { get; set; }
        public string TenHangXe { get; set; }
        public Guid ID_KhachHang { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public Guid? ID_CoVanDichVu { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoaiKhachHang { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string MauSon { get; set; }
        public string NamSanXuat { get; set; }
        public string DungTich { get; set; }
        public string HopSo { get; set; }
        public string ChuXe_SDT { get; set; }
        public string ChuXe_DiaChi { get; set; }
        public string ChuXe_Email { get; set; }
        public string CoVan_SDT { get; set; }
        public int TrangThai { get; set; }
        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
    }
    public class Gara_BaoGia
    {
        public Guid ID { get; set; }
        public Guid? ID_PhieuTiepNhan { get; set; }
        public Guid? ID_DoiTuong { get; set; }
        public Guid? ID_BaoHiem { get; set; }
        public Guid? ID_BangGia { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public Guid? ID_DonVi { get; set; }
        public Guid? ID_HoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public string MaBaoGia { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoaiKhachHang { get; set; }
        public string MaSoThue { get; set; }
        public string TaiKhoanNganHang { get; set; }// so TK of khachhang
        public string TenBaoHiem { get; set; }
        public string DienThoaiBaoHiem { get; set; }
        public string BH_Email { get; set; }
        public string BH_DiaChi { get; set; }
        public string LienHeBaoHiem { get; set; } // == BH_TenLienHe at mauin
        public string SoDienThoaiLienHeBaoHiem { get; set; }
        public string TrangThaiText { get; set; }
        public string TrangThai { get; set; }
        public string DienGiai { get; set; }
        public double TongTienHang { get; set; }
        public double PhaiThanhToan { get; set; }
        public double? TongThanhToan { get; set; }
        public double? KhachDaTra { get; set; }
        public double? BaoHiemDaTra { get; set; }
        public double? PhaiThanhToanBaoHiem { get; set; }
        public double? PTThueHoaDon { get; set; }
        public double TongGiamGia { get; set; }
        public double? TongTienBHDuyet { get; set; }
        public double? PTThueBaoHiem { get; set; }
        public double? TongTienThueBaoHiem { get; set; }
        public int? SoVuBaoHiem { get; set; }
        public double? KhauTruTheoVu { get; set; }
        public double? PTGiamTruBoiThuong { get; set; }
        public double? GiamTruBoiThuong { get; set; }
        public double? BHThanhToanTruocThue { get; set; }
        public double? TongChietKhau { get; set; }// check when xuly dathang
        public double? KhuyeMai_GiamGia { get; set; }
        public double? TongChiPhi { get; set; }
        public double? TongTienThue { get; set; }
        public double? TongThueKhachHang { get; set; }
        public int? CongThucBaoHiem { get; set; }
        public string MaPhieuTiepNhan { get; set; }
        public string ChiPhi_GhiChu { get; set; }
        public string BienSo { get; set; }
        public string YeuCau { get; set; }// used to check trangthai hddathang (1:Phieu tam, 2: Dang giao hang, 3: HoanThanh, 4: Huy)
        public double? DiemGiaoDich { get; set; }
        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }

        public double? Khach_TienMat { get; set; }
        public double? Khach_TienPOS { get; set; }
        public double? Khach_TienCK { get; set; }
        public double? Khach_TheGiaTri { get; set; }
        public double? Khach_TienCoc { get; set; }
        public double? Khach_TienDiem { get; set; }

        public double? BH_TienMat { get; set; }
        public double? BH_TienPOS { get; set; }
        public double? BH_TienCK { get; set; }
        public double? BH_TheGiaTri { get; set; }
        public double? BH_TienCoc { get; set; }
        public double? BH_TienDiem { get; set; }
    }
    public class PhieuTiepNhan_TinhTrang
    {
        public Guid ID { get; set; }
        public Guid? ID_PhieuTiepNhan { get; set; }
        public string TenHangMuc { get; set; }
        public string TinhTrang { get; set; }
        public string PhuongAnSuaChua { get; set; }
        public string Anh { get; set; }
        public int TrangThai { get; set; }
    }
    public class PhieuTiepNhan_VatDungDinhKem
    {
        public Guid ID { get; set; }
        public Guid? ID_PhieuTiepNhan { get; set; }
        public string TieuDe { get; set; }
        public double? SoLuong { get; set; }
        public string FileDinhKem { get; set; }
        public int TrangThai { get; set; }
    }
    public class Param_XuatKhoToanBo
    {
        public Guid ID_HoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
    }
    public class PhuTung_LichBaoDuong
    {
        public Guid? ID { get; set; }
        public Guid? ID_HangHoa { get; set; }
        public Guid? ID_Xe { get; set; }
        public Guid? ID_DoiTuong { get; set; }
        public string BienSo { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public string TenNhomHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public int? LanBaoDuong { get; set; }
        public int? SoKmBaoDuong { get; set; }
        public DateTime? NgayBaoDuongDuKien { get; set; }
        public DateTime? NgayNhacBatDau { get; set; }
        public DateTime? NgayNhacKetThuc { get; set; }
        public int? LanNhac { get; set; }
        public string GhiChu { get; set; }
        public int? TrangThai { get; set; }
        public string sTrangThai { get; set; }
        public string sThoiGianBaoHanh { get; set; }
        public DateTime? HanBaoHanh { get; set; }
        public int? TotalRow { get; set; }
    }
    public class NhatKyBaoDuong
    {
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public double? SoLuong { get; set; }
        public int? LanBaoDuong { get; set; }
        public int? SoKmBaoDuong { get; set; }
        public int? TrangThai { get; set; }
        public string sTrangThai { get; set; }
        public string NVThucHiens { get; set; }
        public string GhiChu { get; set; }
    }
}
