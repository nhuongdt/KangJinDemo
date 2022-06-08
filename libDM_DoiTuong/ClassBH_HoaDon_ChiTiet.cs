using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace libDM_DoiTuong
{
    public class ClassBH_HoaDon_ChiTiet
    {
        private SsoftvnContext db;
        public ClassBH_HoaDon_ChiTiet(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public BH_HoaDon_ChiTiet Select_HoaDon_ChiTiet(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.BH_HoaDon_ChiTiet.Find(id);
            }
        }

        public List<BH_HoaDon_ChiTiet> Gets(Expression<Func<BH_HoaDon_ChiTiet, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.BH_HoaDon_ChiTiet.ToList();
                else
                    return db.BH_HoaDon_ChiTiet.Where(query).ToList();
            }
        }

        public List<SP_ThanhPhanDinhLuong> SP_GetThanhPhanDinhLuong_CTHD(Guid? idCTHD, int? loaiHoaDon = null)
        {
            try
            {
                List<SqlParameter> lst = new List<SqlParameter>();
                lst.Add(new SqlParameter("ID_CTHD", idCTHD));
                lst.Add(new SqlParameter("LoaiHoaDon", loaiHoaDon ?? (object)DBNull.Value));
                var data = db.Database.SqlQuery<SP_ThanhPhanDinhLuong>("EXEC GetTPDinhLuong_ofCTHD @ID_CTHD, @LoaiHoaDon", lst.ToArray()).ToList();
                return data;
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("SP_GetThanhPhanDinhLuong_CTHD " + e.InnerException + e.Message);
                return null;
            }
        }
        /// <summary>
        /// used at modal NhatKyGoiBaoDuong
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public List<SP_ThanhPhanDinhLuong> GetTPDinhLuong_ofHoaDon(List<Guid> lst)
        {
            var idHoaDons = string.Join(",", lst);
            SqlParameter param = new SqlParameter("ID_HoaDons", idHoaDons);
            var data = db.Database.SqlQuery<SP_ThanhPhanDinhLuong>("EXEC GetTPDinhLuong_ofHoaDon @ID_HoaDons", param).ToList();
            return data;
        }
        public List<SP_ThanhPhanDinhLuong> CTHD_GetDichVubyDinhLuong(Guid idHoaDon, Guid? idQuiDoi, Guid? idLoHang = null)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_HoaDon", idHoaDon));
            lstParam.Add(new SqlParameter("ID_DonViQuiDoi", idQuiDoi));
            lstParam.Add(new SqlParameter("ID_LoHang", idLoHang ?? (object)DBNull.Value));
            var data = db.Database.SqlQuery<SP_ThanhPhanDinhLuong>("EXEC CTHD_GetDichVubyDinhLuong @ID_HoaDon, @ID_DonViQuiDoi, @ID_LoHang", lstParam.ToArray()).ToList();
            return data;
        }
        /// <summary>
        /// used to saveHoaDonTraHang
        /// </summary>
        /// <param name="idHoaDon"></param>
        /// <param name="idQuiDoi"></param>
        /// <param name="idLoHang"></param>
        /// <returns></returns>
        public void HDTraHang_InsertTPDinhLuong(Guid idHoaDon)
        {
            SqlParameter param = new SqlParameter("ID_HoaDon", idHoaDon);
            db.Database.ExecuteSqlCommand("EXEC HDTraHang_InsertTPDinhLuong @ID_HoaDon", param);
        }
        public List<SP_ThanhPhanDinhLuong> HDSC_GetChiTietXuatKho(Guid idHoaDon, Guid idChiTietHD, int? loaiHang = 0)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_HoaDon", idHoaDon));
            lstParam.Add(new SqlParameter("IDChiTietHD", idChiTietHD));
            lstParam.Add(new SqlParameter("LoaiHang", loaiHang));
            var data = db.Database.SqlQuery<SP_ThanhPhanDinhLuong>("EXEC HDSC_GetChiTietXuatKho @ID_HoaDon, @IDChiTietHD, @LoaiHang", lstParam.ToArray()).ToList();
            return data;
        }

        public List<BH_HoaDon_ChiTietDTO> GetCTHD_DVQD_byIDHoaDon(Guid idHoaDon)
        {
            List<BH_HoaDon_ChiTietDTO> lstCTHD = new List<BH_HoaDon_ChiTietDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var data = (from cthd in db.BH_HoaDon_ChiTiet
                            join dvqd in db.DonViQuiDois on cthd.ID_DonViQuiDoi equals dvqd.ID
                            join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                            where cthd.ID_HoaDon == idHoaDon
                            select new
                            {
                                ID_DonViQuiDoi = dvqd.ID,
                                ID_LoHang = cthd.ID_LoHang,
                                SoLuong = cthd.SoLuong,
                                DonGia = cthd.DonGia,
                                MaHangHoa = dvqd.MaHangHoa,
                                TenHangHoa = hh.TenHangHoa,
                                TenDonViTinh = dvqd.TenDonViTinh,
                                LaDonViChuan = dvqd.LaDonViChuan,
                                TyLeChuyenDoi = dvqd.TyLeChuyenDoi,
                                QuyCach = hh.QuyCach,
                            }).ToList();

                foreach (var item in data)
                {
                    BH_HoaDon_ChiTietDTO itemCT = new BH_HoaDon_ChiTietDTO();
                    itemCT.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                    itemCT.ID_LoHang = item.ID_LoHang;
                    itemCT.SoLuong = item.SoLuong;
                    itemCT.DonGia = item.DonGia;
                    itemCT.MaHangHoa = item.MaHangHoa;
                    itemCT.TenHangHoa = item.TenHangHoa;
                    itemCT.TenDonViTinh = item.TenDonViTinh;
                    itemCT.LaDonViChuan = item.LaDonViChuan;
                    itemCT.TyLeChuyenDoi = item.TyLeChuyenDoi;
                    itemCT.QuyCach = item.QuyCach;

                    lstCTHD.Add(itemCT);
                }
                return lstCTHD;
            }
        }

        public List<BH_HoaDon_ChiTiet> GetsPhieuHuy(Expression<Func<XH_HoaDon_ChiTietDTO, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.BH_HoaDon_ChiTiet.ToList();
            }
        }

        public BH_HoaDon_ChiTiet Get(Expression<Func<BH_HoaDon_ChiTiet, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.BH_HoaDon_ChiTiet.Where(query).FirstOrDefault();
            }
        }

        public bool BH_HoaDon_ChiTietExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {

                return db.BH_HoaDon_ChiTiet.Count(e => e.ID == id) > 0;
            }
        }

        #endregion

        #region insert
        public string Add_ChiTietHoaDon(List<BH_HoaDon_ChiTiet> objAdd)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.BH_HoaDon_ChiTiet.AddRange(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("AddChiTietHoaDon " + ex);
                    strErr = string.Concat("Add_ChiTietHoaDon", ex.InnerException, ex.Message);
                }
            }
            return strErr;
        }

        public string Add_ChiTietHoaDon(BH_HoaDon_ChiTiet objAdd)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.BH_HoaDon_ChiTiet.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = string.Concat("Add_ChiTietHoaDon ", ex);
                    CookieStore.WriteLog(strErr);
                }
            }
            return strErr;
        }

        public void UpdateIDCTNew_forCTOld(List<string> lstPairID)
        {
            try
            {
                if (lstPairID != null && lstPairID.Count > 0)
                {
                    var sPairID = string.Join(";", lstPairID);
                    SqlParameter param = new SqlParameter("Pair_IDNewIDOld", sPairID);
                    db.Database.ExecuteSqlCommand("exec UpdateIDCTNew_forCTOld @Pair_IDNewIDOld", param);
                }
            }
            catch (Exception e)
            {
                CookieStore.WriteLog(string.Concat("UpdateIDCTNew_forCTOld ", e.InnerException, e.Message));
            }
        }
        #endregion

        #region update
        public string Update_ChiTietHoaDon(BH_HoaDon_ChiTiet obj)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    #region BH_HoaDon_ChiTiet
                    BH_HoaDon_ChiTiet objUpd = db.BH_HoaDon_ChiTiet.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TienChietKhau = obj.TienChietKhau; // tổng số lượng nhận
                    objUpd.Bep_SoLuongYeuCau = obj.Bep_SoLuongYeuCau;
                    objUpd.Bep_SoLuongHoanThanh = obj.Bep_SoLuongHoanThanh;
                    objUpd.Bep_SoLuongChoCungUng = obj.Bep_SoLuongChoCungUng;
                    objUpd.GiaVon_NhanChuyenHang = obj.GiaVon_NhanChuyenHang;
                    objUpd.GhiChu = obj.GhiChu;
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        public string Update_ChiTietKiemKho(BH_HoaDon_ChiTiet obj)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    #region BH_HoaDon_ChiTiet
                    BH_HoaDon_ChiTiet objUpd = db.BH_HoaDon_ChiTiet.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TienChietKhau = obj.TienChietKhau;
                    objUpd.ThanhTien = obj.ThanhTien;
                    objUpd.SoLuong = obj.SoLuong;
                    objUpd.ThanhToan = obj.ThanhToan;
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        public string Update_ChiTietHoaDonTheKho(BH_HoaDon_ChiTiet obj, int loaiHoaDon)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    #region BH_HoaDon_ChiTiet
                    BH_HoaDon_ChiTiet objUpd = db.BH_HoaDon_ChiTiet.Find(obj.ID);
                    if (loaiHoaDon == 8)
                    {
                        objUpd.GiaVon = obj.GiaVon;
                        objUpd.ThanhTien = obj.GiaVon.Value * obj.SoLuong;
                    }
                    if (loaiHoaDon == 18)
                    {
                        objUpd.DonGia = obj.DonGia;
                        objUpd.PTChietKhau = obj.GiaVon.Value - obj.DonGia > 0 ? obj.GiaVon.Value - obj.DonGia : 0;
                        objUpd.TienChietKhau = obj.GiaVon.Value - obj.DonGia > 0 ? 0 : obj.GiaVon.Value - obj.DonGia;
                    }
                    else
                    {
                        objUpd.GiaVon = obj.GiaVon;
                    }
                    //objUpd.Bep_SoLuongYeuCau = obj.Bep_SoLuongYeuCau;
                    //objUpd.Bep_SoLuongHoanThanh = obj.Bep_SoLuongHoanThanh;
                    //objUpd.Bep_SoLuongChoCungUng = obj.Bep_SoLuongChoCungUng;
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        public string Update_ChiTietHoaDonXH(BH_HoaDon_ChiTiet obj)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    #region BH_HoaDon_ChiTiet
                    BH_HoaDon_ChiTiet objUpd = db.BH_HoaDon_ChiTiet.Find(obj.ID);
                    objUpd.GiaVon = obj.GiaVon;
                    //objUpd.Bep_SoLuongYeuCau = obj.Bep_SoLuongYeuCau;
                    //objUpd.Bep_SoLuongHoanThanh = obj.Bep_SoLuongHoanThanh;
                    //objUpd.Bep_SoLuongChoCungUng = obj.Bep_SoLuongChoCungUng;
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        /// <summary>
        /// update again TonLuyKe, TonKho, GiaVon when update cthd
        /// </summary>
        /// <param name="idHoaDon"></param>
        /// <param name="idChiNhanh"></param>
        /// <param name="ngaylapHDOld"></param>
        /// <returns></returns>
        public string UpdateTonKhoGiaVon_whenUpdateCTHD(Guid idHoaDon, Guid idChiNhanh, DateTime ngaylapHDOld)
        {
            string sErr = string.Empty;
            try
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("IDHoaDonInput", idHoaDon));
                lstParam.Add(new SqlParameter("IDChiNhanhInput", idChiNhanh));
                lstParam.Add(new SqlParameter("NgayLapHDOld", ngaylapHDOld));
                var tblCheck = db.Database.SqlQuery<SP_CheckUpdateGiaVon>("EXEC UpdateTonLuyKeCTHD_whenUpdate @IDHoaDonInput, @IDChiNhanhInput, @NgayLapHDOld", lstParam.ToArray()).ToList().FirstOrDefault();

                Task<string> strGiaVon = UpdateGiaVonCTHD_WhenEdit2(idHoaDon, idChiNhanh, tblCheck.NgayLapHDMin, db);
                if (tblCheck.UpdateKiemKe > 0)
                {
                    UpdatePhieuKiemKe_WhenEdit(idHoaDon, idChiNhanh, tblCheck.NgayLapHDMin);
                }
            }
            catch (Exception ex)
            {
                sErr = string.Concat("UpdateTonKhoGiaVon_whenUpdateCTHD ", ex.Message, ex.InnerException);
                CookieStore.WriteLog(sErr);
            }
            return sErr;
        }

        public async static Task<string> UpdateGiaVonCTHD_WhenEdit2(Guid idHoaDon, Guid idChiNhanh, DateTime ngaylapHDOld, SsoftvnContext db)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("IDHoaDonInput", idHoaDon));
            lstParam.Add(new SqlParameter("IDChiNhanh", idChiNhanh));
            lstParam.Add(new SqlParameter("NgayLapHDMin", ngaylapHDOld));
            db.Database.ExecuteSqlCommand("EXEC UpdateGiaVon_WhenEditCTHD @IDHoaDonInput, @IDChiNhanh, @NgayLapHDMin", lstParam.ToArray());
            return await Task.FromResult("");
        }

        public void UpdateGiaVonCTHD_WhenEdit(Guid idHoaDon, Guid idChiNhanh, DateTime ngaylapHDOld)
        {
            try
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("IDHoaDonInput", idHoaDon));
                lstParam.Add(new SqlParameter("IDChiNhanh", idChiNhanh));
                lstParam.Add(new SqlParameter("NgayLapHDMin", ngaylapHDOld));
                db.Database.ExecuteSqlCommand("EXEC UpdateGiaVon_WhenEditCTHD @IDHoaDonInput, @IDChiNhanh, @NgayLapHDMin", lstParam.ToArray());
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog(string.Concat("UpdateGiaVonCTHD_WhenEdit ", ex.Message, ex.InnerException));
            }
        }

        public void UpdatePhieuKiemKe_WhenEdit(Guid idHoaDon, Guid idChiNhanh, DateTime ngaylapHDOld)
        {
            try
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("IDHoaDonInput", idHoaDon));
                lstParam.Add(new SqlParameter("IDChiNhanhInput", idChiNhanh));
                lstParam.Add(new SqlParameter("NgayLapHDMin", ngaylapHDOld));
                db.Database.ExecuteSqlCommand("EXEC UpdateChiTietKiemKe_WhenEditCTHD @IDHoaDonInput, @IDChiNhanhInput, @NgayLapHDMin", lstParam.ToArray());
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog(string.Concat("UpdatePhieuKiemKe_WhenEdit ", ex.Message, ex.InnerException));
            }
        }

        public void UpdateTheGiaTri(Guid? idDoiTuong, DateTime ngaylapHD)
        {
            try
            {
                //if (idDoiTuong != null && idDoiTuong != Guid.Empty)
                //{
                //    List<SqlParameter> paramlist = new List<SqlParameter>
                //    {
                //        new SqlParameter("NgayLapHoaDonInput", ngaylapHD.ToString("yyyy-MM-dd HH:mm:ss")),
                //        new SqlParameter("IDDoiTuong", idDoiTuong)
                //    };
                //    db.Database.ExecuteSqlCommand(" UpdateLaiSoDuTheNap @NgayLapHoaDonInput, @IDDoiTuong", paramlist.ToArray());
                //}
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("UpdateTheGiaTri: " + ex);
            }
        }

        /// <summary>
        /// check gói dịch vụ đã được sử dụng --> không cho cập nhật chi tiết hóa đơn
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true: used</returns>
        public bool ServicePackage_CheckUsed(Guid id)
        {
            try
            {
                List<Guid> ids = db.BH_HoaDon_ChiTiet.Where(x => x.ID_HoaDon == id).Select(x => x.ID).ToList();
                var count = (from hd in db.BH_HoaDon
                             join ct in db.BH_HoaDon_ChiTiet on hd.ID equals ct.ID_HoaDon
                             where hd.ChoThanhToan == false && ids.Contains(ct.ID_ChiTietGoiDV ?? Guid.Empty)
                             select ct.ID).Count();
                return count > 0;
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("SP_GetChiTietHD_MultipleHoaDon " + e.InnerException + e.Message);
                return true;
            }
        }

        public List<GoiDichVu_KhachHang> GetDSGoiDichVu_ofKhachHang(ParamNKyGDV param)
        {
            var idChiNhanhs = string.Empty;
            if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count > 0)
            {
                idChiNhanhs = string.Join(",", param.IDChiNhanhs);
            }
            var idCus = string.Empty;
            if (param.IDCustomers != null && param.IDCustomers.Count > 0)
            {
                idCus = string.Join(",", param.IDCustomers);
            }
            var idCar = string.Empty;
            if (param.IDCars != null && param.IDCars.Count > 0)
            {
                idCar = string.Join(",", param.IDCars);
            }
            List<SqlParameter> paramlist = new List<SqlParameter>();
            paramlist.Add(new SqlParameter("IDChiNhanhs", idChiNhanhs ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("IDCustomers", idCus ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("IDCars", idCar ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("TextSearch", param.TextSearch ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("DateFrom", param.DateFrom ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("DateTo", param.DateTo ?? (object)DBNull.Value));
            return db.Database.SqlQuery<GoiDichVu_KhachHang>("exec GetDSGoiDichVu_ofKhachHang @IDChiNhanhs, @IDCustomers," +
                " @IDCars, @TextSearch,@DateFrom, @DateTo", paramlist.ToArray()).ToList();
        }

        public List<ChiPhiDichVuDTO> CTHD_GetChiPhiDichVu(List<string> arrID, List<string> arrVendor)
        {
            var idHoaDons = string.Empty;
            var idVendors = string.Empty;
            if (arrID != null && arrID.Count > 0)
            {
                idHoaDons = string.Join(",", arrID);
            }
            if (arrVendor != null && arrVendor.Count > 0)
            {
                idVendors = string.Join(",", arrVendor);
            }
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("IDHoaDons", idHoaDons ?? (object)DBNull.Value));
            param.Add(new SqlParameter("IDVendors", idVendors ?? (object)DBNull.Value));
            return db.Database.SqlQuery<ChiPhiDichVuDTO>("exec CTHD_GetChiPhiDichVu @IDHoaDons, @IDVendors", param.ToArray()).ToList();
        }

        #endregion

        #region delete
        static string CheckDelete_HoaDon(SsoftvnContext db, BH_HoaDon_ChiTiet obj)
        {
            string strCheck = string.Empty;

            List<CongDoan_DichVu> lstCongDoans = db.CongDoan_DichVu.Where(p => p.ID_CongDoan == obj.ID).ToList();
            if (lstCongDoans != null && lstCongDoans.Count > 0)
            {
                strCheck = "Hàng hóa/Dịch vụ đã được sử dụng để lập danh mục công đoạn cho hàng hóa/dịch vụ khác.";
                return strCheck;
            }
            return strCheck;
        }

        public string Delete_HoaDon_ChiTiet(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                BH_HoaDon_ChiTiet objDel = db.BH_HoaDon_ChiTiet.Find(id);
                if (objDel != null)
                {
                    string strCheck = CheckDelete_HoaDon(db, objDel);
                    if (strCheck == string.Empty)
                    {
                        try
                        {
                            db.BH_HoaDon_ChiTiet.Remove(objDel);
                            db.SaveChanges();
                        }
                        catch (Exception exxx)
                        {
                            strErr = exxx.Message;
                            return strErr;
                        }
                    }
                    else
                    {
                        strErr = strCheck;
                        return strErr;
                    }
                }
                else
                {
                    strErr = "Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
                    return strErr;
                }
            }
            return strErr;
        }

        /// <summary>
        /// remove nvth of cthd + hoadon (don't remove ckThucThu)
        /// and remove cthd
        /// </summary>
        /// <param name="idHD"></param>
        /// <returns></returns>
        public string Delete_HoaDon_ChiTiet_ByIDHoaDon(Guid idHD, List<BH_NhanVienThucHien> nvthNew = null)
        {
            string sErr = string.Empty;
            try
            {
                // remove nvth of cthd + hoadon (don't remove ckThucThu)
                var data = from th in db.BH_NhanVienThucHien
                           join ct in db.BH_HoaDon_ChiTiet on th.ID_ChiTietHoaDon equals ct.ID into CKNVien
                           from ck in CKNVien.DefaultIfEmpty()
                           where (ck != null && ck.ID_HoaDon == idHD)
                           || (th.ID_HoaDon == idHD && th.TinhChietKhauTheo != 1)
                           select th;
                if (data != null)
                    db.BH_NhanVienThucHien.RemoveRange(data);
                IQueryable<BH_HoaDon_ChiTiet> lstCTHD = db.BH_HoaDon_ChiTiet.Where(p => p.ID_HoaDon == idHD);

                db.BH_HoaDon_ChiTiet.RemoveRange(lstCTHD); //remove cthd
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Delete_HoaDon_ChiTiet_ByIDHoaDon " + ex.Message + ex.InnerException);
                sErr = ex.ToString();
            }
            return sErr;
        }
        #endregion
    }

    public class BH_HoaDon_ChiTietDTO
    {
        public Guid? ID { get; set; }
        public Guid? ID_HoaDon { get; set; }
        public Guid? ID_LoHang { get; set; }
        public Guid? ID_DonVi { get; set; }
        public double DonGia { get; set; }
        public double? GiaVon { get; set; }
        public double SoLuong { get; set; }
        public double SoThuTu { get; set; }
        public double TonKho { get; set; }
        public double ThanhTien { get; set; }
        public double ThanhToan { get; set; }
        public double TyLeChuyenDoi { get; set; }
        public double GiamGia { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid ID_HangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string YeuCau { get; set; }
        public string ChatLieu { get; set; }
        public DateTime? ThoiGian { get; set; }
        public double? Bep_SoLuongYeuCau { get; set; }
        public double? Bep_SoLuongHoanThanh { get; set; }
        public double? Bep_SoLuongChoCungUng { get; set; }
        public string TenPhongBan { get; set; }
        public string MaHoaDon { get; set; }
        public string SrcImage { get; set; }
        public string TenDonViTinh { get; set; }
        public string MaLoHang { get; set; }
        public string TenDonVi { get; set; }
        public double PTChietKhau { get; set; }
        public double TienChietKhau { get; set; }
        public string GhiChu { get; set; }
        public Guid? ID_ViTri { get; set; }
        public double GiaBan { get; set; } // Gia ban thuc te
        public double GiaNhap { get; set; }
        public double GiaBanMaVach { get; set; } // Gia ban thuc te
        public bool? LaHangHoa { get; set; } // HangHoa/DichVu
        public bool? QuanLyTheoLoHang { get; set; }
        public Guid? ID_KhuyenMai { get; set; }
        public double SoLuongConLai { get; set; }
        public List<DonViTinh> DonViTinh { get; set; }
        public List<HangHoa_ThuocTinh> HangHoa_ThuocTinh { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenHangHoaFull { get; set; } // = TenHangHoa + ThuocTinh_GiaTri + (TenDonViTinh) + SoLo --> get from Store procedure
        public List<BH_NhanVienThucHienDTO> BH_NhanVienThucHien { get; set; }

        public Guid? ID_ChiTietDinhLuong { get; set; }
        public Guid? ID_ChiTietGoiDV { get; set; }
        public Guid? ID_TangKem { get; set; } // check TraHang from HoaDon co KM hanghoa tang kem
        public bool? TangKem { get; set; }
        public Guid? ID_NhomHangHoa { get; set; } // check HangHoa thuoc Nhom KhuyenMai
        public string TenNhomHangHoa { get; set; } // bind at mauin: theonhomhang
        public bool? LaDonViChuan { get; set; } // check nhap QuyCach
        public double? QuyCach { get; set; }
        public double? PTThue { get; set; }
        public double? TienThue { get; set; }
        public double? PhiDichVu { get; set; }
        public double? TongPhiDichVu { get; set; }
        public bool? LaPTPhiDichVu { get; set; }
        public double GiaBanHH { get; set; } // Gia ban hh

        public double? ThoiGianBaoHanh { get; set; }
        public double? LoaiThoiGianBH { get; set; }
        public double? ThoiGianThucHien { get; set; } // print at list DS HD
        public DateTime? ThoiGianHoanThanh { get; set; } // check hd_TamLuu
        public int? SoPhutThucHien { get; set; } // caculator timeremain in HD_TamLuu
        public string TimeStart { get; set; }
        public string TenViTri { get; set; }
        public int? QuaThoiGian { get; set; }
        public List<SP_ThanhPhanDinhLuong> ThanhPhan_DinhLuong { get; set; }
        public string GhiChuHH { get; set; }// bind mota hh when print
        public double? DiemKhuyenMai { get; set; } // check kmai congdiem
        public int? DichVuTheoGio { get; set; }
        public int? DuocTichDiem { get; set; }
        public double? SoLuongMua { get; set; }
        public double? SoLuongDVDaSuDung { get; set; }
        public double? SoLuongDVConLai { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public double? SoLuongXuat { get; set; } // used at gara - show soluong phutung daxuat
        public double? DonGiaBaoHiem { get; set; } = 0;
        public string TenHangHoaThayThe { get; set; } = string.Empty;
        public Guid? ID_LichBaoDuong { get; set; }
        public int? LoaiHangHoa { get; set; }
        public Guid? ID_ParentCombo { get; set; }
        public List<BH_HoaDon_ChiTietDTO> ThanhPhanComBo { get; set; }
        public int? HoaHongTruocChietKhau { get; set; }// TinhHoaHongTruocCK (0,1)
    }

    public class ChiPhiDichVuDTO
    {
        public Guid? ID { get; set; }
        public Guid? ID_NhaCungCap { get; set; }
        public Guid? ID_HoaDon_ChiTiet { get; set; }
        public Guid? ID_HoaDon { get; set; }
        public Guid? ID_DonViQuiDoi { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaThayThe { get; set; }
        public int? LoaiHangHoa { get; set; }
        public string MaNhaCungCap { get; set; }
        public string TenNhaCungCap { get; set; }
        public string BienSo { get; set; }
        public double? SoLuong { get; set; }
        public double? SoLuongHoaDon { get; set; }
        public double? DonGia { get; set; }
        public double? ThanhTien { get; set; }
        public double? GiaBan { get; set; }// giaban of hanghoa (at DonViQuiDoi)
        public double? TongChiPhi { get; set; }// = sum all chiphi
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string GhiChu { get; set; }// ghichu of chiphi
    }

    public class MaxCodeMaHoaDon
    {
        public double MaxCode { get; set; }
    }

    public class BH_KiemKhoChiTiet_Excel
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string MaLoHang { get; set; }
        public double TienChietKhau { get; set; }
        public double ThanhTien { get; set; }
        public double SoLuong { get; set; }
        public double ThanhToan { get; set; }
    }
    public class XH_HoaDon_ChiTietDTO
    {
        public Guid? ID { get; set; }
        public Guid? ID_HoaDon { get; set; }
        public double DonGia { get; set; }
        public double? GiaVon { get; set; }
        public double SoLuong { get; set; }
        public double ThanhTien { get; set; }
        public double ThanhToan { get; set; }
        public double GiamGia { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public DateTime? ThoiGian { get; set; }
        //public string TenPhongBan { get; set; }
        public double? Bep_SoLuongYeuCau { get; set; }
        public double? Bep_SoLuongHoanThanh { get; set; }
        public double? Bep_SoLuongChoCungUng { get; set; }
        public string TenPhongBan { get; set; }
        public string MaHoaDon { get; set; }
        public string TenDonViTinh { get; set; }
        public double PTTienChietKhau { get; set; }
        public double TienChietKhau { get; set; }
        public string GhiChu { get; set; }
        public Guid? ID_ViTri { get; set; }
    }
    public class XH_HoaDon_ChiTietPRC
    {
        public Guid ID { get; set; }
        public Guid ID_HoaDon { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid? ID_LoHang { get; set; }
        public Guid? ID_HangHoa { get; set; }
        public bool? QuanLyTheoLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string MaHoaDon { get; set; }
        public string TenNhanVien { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenDonViTinh { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public String TenLoHang { get; set; }
        public double TonKho { get; set; }
        public double TrangThaiMoPhieu { get; set; }
        public double SoLuong { get; set; }
        public double SoLuongXuatHuy { get; set; }
        public double DonGia { get; set; }
        public double? GiaVon { get; set; }
        public double GiaTriHuy { get; set; }
        public double GiamGia { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public string GhiChu { get; set; }
        public string ChatLieu { get; set; } // = 4.xuat kho sudung Goi bao duong
        public int SoThuTu { get; set; }
        public List<SP_ThanhPhanDinhLuong> ThanhPhan_DinhLuong { get; set; }
        public List<DonViTinh> DonViTinh { get; set; }
    }

    public class GoiDichVu_KhachHang
    {
        public Guid ID_GoiDV { get; set; }
        public string MaHoaDon { get; set; } // = MagoiDV
        public string NgayLapHoaDon { get; set; }
        public string NgayApDungGoiDV { get; set; }
        public string HanSuDungGoiDV { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid? ID_ChiTietGoiDV { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public double GiaBan { get; set; } // = DonGia - ChietKhau
        public double SoLuongMua { get; set; }
        public double? SoLuongTang { get; set; }
        public double SoLuongDung { get; set; }
        public double SoLuongConLai { get; set; }
        public string TenDonViTinh { get; set; }
        public Guid ID_HangHoa { get; set; }
        public Guid? ID_NhomHangHoa { get; set; }
        public string TenNhomHangHoa { get; set; }
        public bool TangKem { get; set; }
        public Guid? ID_TangKem { get; set; }
        public double? PhiDichVu { get; set; }
        public bool? LaPTPhiDichVu { get; set; }
        public double? QuyCach { get; set; }
        public int? SoPhutThucHien { get; set; }// get from DM_HangHoa
        public string GhiChuHH { get; set; }// print mota hh
        public Guid? ID_DoiTuong { get; set; }// used to find KH by magoi
        public string MaDoiTuong { get; set; }
        public Guid? ID_LoHang { get; set; }
        public string MaLoHang { get; set; }
        public bool? QuanLyTheoLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string GhiChu { get; set; }// ghichu of cthd
        public string BienSo { get; set; }
        public Guid? ID_Xe { get; set; }
        public Guid? ID_ParentCombo { get; set; }
        public int? SoThuTu { get; set; }// combo: STT=0, else 1
        public int? DichVuTheoGio { get; set; }
        public int? DuocTichDiem { get; set; }
        public int? LoaiHangHoa { get; set; }
        public bool? LaHangHoa { get; set; }
        public double? GiaVon { get; set; }
        public double? TienChietKhau { get; set; } 
        public double? PTChietKhau { get; set; } 
    }

    public class SP_NhatKySuDung_GoiDV
    {
        public Guid? ID_Xe { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenHangHoa_KhongDau { get; set; }
        public double? SoLuong { get; set; }
        public string NhanVienThucHien { get; set; } // list TenNhanVien thuc hien dich vu
        public double? TongChietKhau { get; set; }
        public double? TongSoLuong { get; set; }
        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
    }

    public class SP_InforServicePackage
    {
        public Guid ID { get; set; }
        public string MaHoaDon { get; set; }
        public string MaPhieuThu { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public double? TongTienHang { get; set; } // TongMua - TongTra
        public double? TongMua { get; set; }
        public double? TongTra { get; set; }
        public double? PhaiThanhToan { get; set; }
        public double? DaThanhToan { get; set; } // thu - chi
        public double? ConNo { get; set; }
        public string GhiChu { get; set; }
        public Guid? ID_Xe { get; set; }
        public string BienSo { get; set; }
        public int? TotalRow { get; set; }
        public double? SumTongMua { get; set; }
        public double? SumTongTra { get; set; }
        public double? SumPhaiThanhToan { get; set; }
        public double? SumDaThanhToan { get; set; }
        public double? SumConNo { get; set; }
    }

    public class SP_SoQuy_ServicePackage
    {
        public string MaHoaDon { get; set; } // ma goi DV
        public string NgayLapHoaDon { get; set; }
        public double TienMat { get; set; } // TongMua - TongTra
        public double TienGui { get; set; }
        public double TongThu { get; set; } // tong thu
        public string NoiDungThu { get; set; }
        public string LoaiThuChi { get; set; } // VD: Thu tiền bán hàng/bán gói DV
        public Guid? ID_Xe { get; set; }
    }

    public class SP_ThanhPhanDinhLuong
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public Guid? ID_DonViQuiDoi { get; set; }
        public double SoLuong { get; set; }
        public double? GiaVon { get; set; }
        public Guid? ID_ChiTietGoiDV { get; set; }
        public double? SoLuongDinhLuong_BanDau { get; set; }
        public double? GiaTriDinhLuong_BanDau { get; set; }
        public double? QuyCach { get; set; }
        public string DonViTinhQuyCach { get; set; }
        public string GhiChu { get; set; }
        public double? GiaBan { get; set; }// used gara: get giaban of dichvu
        public double? SoLuongConLai { get; set; }// used gara: if copy hdxuatkho: only get soluong conlai of dichvu (else if capnhat: get all soluong)
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string MaLoHang { get; set; }
        public Guid? ID_ChiTietDinhLuong { get; set; }
        public Guid? ID_LoHang { get; set; }
        public bool? LaHangHoa { get; set; }
        public Guid? IDChiTietDichVu { get; set; } // used to xuất kho định lượng (thêm hàng ngoài --> get idchitiet của dihcj vụ at hdsc)
        public int? LaDinhLuongBoSung { get; set; }
        public string ChatLieu { get; set; } // chatlieu = 4: xuatkho sudung goi baoduong
        public double? GiaNhap { get; set; } // used to nhahang from hoadon
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public double? GiaBanHH { get; set; } /// used to nhaphang from hoadon
    }

    public class SP_InvoiceNewest
    {
        public Guid? ID_DonViQuiDoi { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
    }

    public class SP_CheckUpdateGiaVon
    {
        public double UpdateGiaVon { get; set; }
        public double UpdateKiemKe { get; set; }
        public DateTime NgayLapHDMin { get; set; }
    }
}
