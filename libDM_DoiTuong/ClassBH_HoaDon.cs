using libDonViQuiDoi;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

namespace libDM_DoiTuong
{
    public class ClassBH_HoaDon
    {
        private SsoftvnContext db;
        public ClassBH_HoaDon(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public DonViQuiDoi getList_DonViQuyDoi(Guid ID)
        {
            DonViQuiDoi DQ = db.DonViQuiDois.Where(x => x.ID == ID).FirstOrDefault();
            return DQ;
        }

        public class DM_GiaBanSelect1
        {
            public Guid ID { get; set; }
            public string TenGiaBan { get; set; }
        }

        public string UpdateSoDuTheNap(string str, DateTime ngaytao, Guid? iddoituong)
        {
            SsoftvnContext db1 = SystemDBContext.GetDBContext(str);
            List<SqlParameter> paramlist = new List<SqlParameter>();
            paramlist.Add(new SqlParameter("NgayLapHoaDonInput", ngaytao));
            paramlist.Add(new SqlParameter("IDDoiTuong", iddoituong));
            db1.Database.ExecuteSqlCommand("exec UpdateLaiSoDuTheNap @NgayLapHoaDonInput, @IDDoiTuong", paramlist.ToArray());
            return "";
        }

        public string CheckTheDaSuDung(Guid id)
        {
            try
            {
                BH_HoaDon objUpd = db.BH_HoaDon.Find(id);
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("ID_DoiTuong", objUpd.ID_DoiTuong));
                lstParam.Add(new SqlParameter("ID_TheGiaTri", id));
                var delete = db.Database.SqlQuery<Model.SP_ReturnBool>("EXEC CheckThucThu_TongSuDung @ID_DoiTuong, @ID_TheGiaTri", lstParam.ToArray()).FirstOrDefault();
                // neu thucthu > tongsudung (duoc phep xoa)
                if (delete.Exist == false)
                {
                    return "Thẻ giá trị đã đem sử dụng không thể xóa";
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                return string.Concat("CheckTheDaSuDung ", e.InnerException, e.Message);
            }
        }

        public List<BH_HoaDonTheNapDTO> GetInforTheGiaTri_byID(Guid id)
        {
            SqlParameter param = new SqlParameter("ID", id);
            return db.Database.SqlQuery<BH_HoaDonTheNapDTO>("EXEC GetInforTheGiaTri_byID @ID", param).ToList();
        }
        public List<BH_HoaDonTheNapDTO> GetInfor_PhieuHoanTraCoc(Guid id)
        {
            SqlParameter param = new SqlParameter("ID", id);
            return db.Database.SqlQuery<BH_HoaDonTheNapDTO>("EXEC GetInforTheGiaTri_byID @ID", param).ToList();
        }

        public List<BH_HoaDonTheNapDTO> LoadDanhMucTheGiaTri(ModelHoaDonTheNap model)
        {
            var isDonVis = string.Join(",", model.arrChiNhanh);
            var txtSearch = model.maHoaDon;
            if (txtSearch == null)
            {
                txtSearch = string.Empty;
            }

            var sTrangThai = "%%";
            switch (model.trangThai)
            {
                case 1: //HoanThanh
                    sTrangThai = "%2%";
                    break;
                case 2: // Huy
                    sTrangThai = "%0%";
                    break;
            }
            var sLoaiHoaDon = "22";
            if (model.ArrLoaiHoaDon != null && model.ArrLoaiHoaDon.Count > 0)
            {
                sLoaiHoaDon = string.Join(",", model.ArrLoaiHoaDon);
            }

            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("IDDonVis", isDonVis));
            lstParam.Add(new SqlParameter("LoaiHoaDons", sLoaiHoaDon));
            lstParam.Add(new SqlParameter("TextSearch", txtSearch));
            lstParam.Add(new SqlParameter("FromDate", model.dayStart));
            lstParam.Add(new SqlParameter("ToDate", model.dayEnd));
            lstParam.Add(new SqlParameter("TrangThais", sTrangThai));
            lstParam.Add(new SqlParameter("MucNapFrom", model.mucnaptu));
            lstParam.Add(new SqlParameter("MucNapTo", model.mucnapden.HasValue ? model.mucnapden : (object)DBNull.Value));
            lstParam.Add(new SqlParameter("KhuyenMaiFrom", model.khuyenmaitu));
            lstParam.Add(new SqlParameter("KhuyenMaiTo", model.khuyenmaiden.HasValue ? model.khuyenmaiden : (object)DBNull.Value));
            lstParam.Add(new SqlParameter("KhuyenMaiLaPTram", model.loaikhuyenmai));
            lstParam.Add(new SqlParameter("ChietKhauFrom", model.chietkhautu));
            lstParam.Add(new SqlParameter("ChietKhauTo", model.chietkhauden.HasValue ? model.chietkhauden : (object)DBNull.Value));
            lstParam.Add(new SqlParameter("ChietKhauLaPTram", model.loaichietkhau));
            lstParam.Add(new SqlParameter("CurrentPage", model.currentPage));
            lstParam.Add(new SqlParameter("PageSize", model.pageSize));

            return db.Database.SqlQuery<BH_HoaDonTheNapDTO>("EXEC GetListTheGiaTri @IDDonVis, @LoaiHoaDons, @TextSearch, @FromDate, @ToDate," +
                 "@TrangThais, @MucNapFrom, @MucNapTo, @KhuyenMaiFrom, @KhuyenMaiTo, @KhuyenMaiLaPTram, @ChietKhauFrom, @ChietKhauTo, " +
                 "@ChietKhauLaPTram, @CurrentPage, @PageSize", lstParam.ToArray()).ToList();
        }

        public List<TGT_LichSuNapTraDTO> GetHisChargeValueCard(ModelLichSuNapThe model)
        {
            var isDonVis = string.Join(",", model.arrChiNhanh);
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_DoiTuong", model.iddt));
            lstParam.Add(new SqlParameter("IDChiNhanhs", isDonVis));
            lstParam.Add(new SqlParameter("FromDate", model.dayStart));
            lstParam.Add(new SqlParameter("ToDate", model.dayEnd));
            lstParam.Add(new SqlParameter("CurrentPage", model.currentPage));
            lstParam.Add(new SqlParameter("PageSize", model.pageSize));
            return db.Database.SqlQuery<TGT_LichSuNapTraDTO>("EXEC GetHisChargeValueCard @ID_DoiTuong, @IDChiNhanhs, @FromDate, @ToDate," +
                 " @CurrentPage, @PageSize", lstParam.ToArray()).ToList();
        }

        public List<DM_GiaBanSelect1> GetDM_GiaBanByIDDonVi(Guid iddonvi)
        {
            try
            {
                if (db == null)
                {
                    return null;
                }
                else
                {
                    var tbl = from gb in db.DM_GiaBan
                              join gbad in db.DM_GiaBan_ApDung on gb.ID equals gbad.ID_GiaBan
                              where gbad.ID_DonVi == null || gbad.ID_DonVi == iddonvi
                              select new DM_GiaBanSelect1
                              {
                                  ID = gb.ID,
                                  TenGiaBan = gb.TenGiaBan
                              };
                    return tbl.Distinct().ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public BH_HoaDon Select_HoaDon(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.BH_HoaDon.Find(id);
            }
        }

        public void UpdateGiaVonDM_GiaVonLT(Guid id, Guid iddonvi, DateTime? ngaynew, int loai, string JobSubDomain)
        {
            // loai = 1 : Thêm mới, loai = 2 : editHD , loai= 3 : Xóa HD
            BH_HoaDon item = db.BH_HoaDon.Find(id);

            DateTime? NgayHDEdit = ngaynew;
            if (loai == 1)
            {
                NgayHDEdit = ngaynew;
            }
            if (loai == 2)
            {
                if (item.NgayLapHoaDon < ngaynew)
                {
                    NgayHDEdit = item.NgayLapHoaDon;
                }
                else
                {
                    NgayHDEdit = ngaynew;
                }
            }
            if (loai == 3)
            {
                NgayHDEdit = ngaynew;
            }

            List<SqlParameter> paramlist = new List<SqlParameter>();
            paramlist.Add(new SqlParameter("IDHoaDonInput", id));
            paramlist.Add(new SqlParameter("IDChiNhanhInput", iddonvi));
            paramlist.Add(new SqlParameter("ThoiGian", NgayHDEdit));
            try
            {
                Thread str2 = new Thread(() => db.Database.ExecuteSqlCommand("exec UpdateGiaVonVer2 @IDHoaDonInput,@IDChiNhanhInput, @ThoiGian", paramlist.ToArray()));
                str2.Start();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ClassBH_HoaDon - UpdateGiaVonDM_GiaVonLT: " + ex.Message + ex.InnerException);
            }
        }

        public DateTime ReturnTimeUpDateGiaVon(DateTime ngaylaphdold, DateTime ngaynew)
        {
            DateTime NgayHDEdit = ngaynew;
            if (ngaynew < ngaylaphdold)
            {
                NgayHDEdit = ngaynew;
            }
            else
            {
                NgayHDEdit = ngaylaphdold;
            }
            return NgayHDEdit;
        }

        //tinh gia von
        public class BH_HoaDonUpdateGV
        {
            public Guid ID_ChiNhanhHD { get; set; }
            public Guid ID_DonViQuiDoiHH { get; set; }
            public Guid? ID_LoHangHH { get; set; }
            public DateTime DateTimeHD { get; set; }
            public bool? ChoThanhToanHD { get; set; }
            public string YeuCauHD { get; set; }
        }

        public void UpdateGiaVonDMM(BH_HoaDonUpdateGV item1, string subdomain)
        {
            List<SqlParameter> paramlist = new List<SqlParameter>();
            paramlist.Add(new SqlParameter("ID_ChiNhanhHD", item1.ID_ChiNhanhHD));
            paramlist.Add(new SqlParameter("ID_DonViQuiDoiHH", item1.ID_DonViQuiDoiHH));
            paramlist.Add(new SqlParameter("ID_LoHangHH", item1.ID_LoHangHH == null ? (object)DBNull.Value : item1.ID_LoHangHH));
            paramlist.Add(new SqlParameter("DateTimeHD", item1.DateTimeHD));
            if (item1.ChoThanhToanHD == null)
                paramlist.Add(new SqlParameter("ChoThanhToanHD", item1.ChoThanhToanHD == false ? 0 : 1));
            else
                paramlist.Add(new SqlParameter("ChoThanhToanHD", item1.ChoThanhToanHD == false ? 0 : 1));
            paramlist.Add(new SqlParameter("YeuCauHD", item1.YeuCauHD == null ? "" : item1.YeuCauHD));
            int retryCount = 1000;
            bool success = false;
            while (retryCount > 0 && !success)
            {
                try
                {
                    db.Database.ExecuteSqlCommand("exec UpDateGiaVonDMHangHoaKhiTaoHD @ID_ChiNhanhHD, @ID_DonViQuiDoiHH, @ID_LoHangHH, @DateTimeHD, @ChoThanhToanHD, @YeuCauHD", paramlist.ToArray());
                    success = true;
                }
                catch (SqlException exception)
                {
                    if (exception.Number != 1205)
                    {
                        CookieStore.WriteLog("UpdateGiaVonDMM1205 - Error: " + exception.Message + exception.InnerException);
                    }
                    retryCount--;
                    if (retryCount == 0) throw exception;
                }
            }
        }

        public DonViQuiDoi getList_DonViQuyDoibyID(Guid? ID)
        {
            DonViQuiDoi DQ = db.DonViQuiDois.Where(x => x.ID == ID).FirstOrDefault();
            return DQ;
        }
        public DM_DonVi getList_DonVibyID(Guid? ID)
        {
            DM_DonVi DQ = db.DM_DonVi.Where(x => x.ID == ID).FirstOrDefault();
            return DQ;
        }

        public NS_NhanVien getList_NhanVienbyID(Guid? ID)
        {
            NS_NhanVien DQ = db.NS_NhanVien.Where(x => x.ID == ID).FirstOrDefault();
            return DQ;
        }
        public DM_NhomDoiTuong getList_NhomKhachHangbyID(Guid? ID)
        {
            DM_NhomDoiTuong DQ = db.DM_NhomDoiTuong.Where(x => x.ID == ID).FirstOrDefault();
            return DQ;
        }
        public DM_NhomHangHoa getList_NhomHangHoabyID(Guid? ID)
        {
            DM_NhomHangHoa DQ = db.DM_NhomHangHoa.Where(x => x.ID == ID).FirstOrDefault();
            return DQ;
        }
        /// <summary>
        /// Cập nhật trạng thái cho HD đặt hàng khi hủy hóa đơn
        /// </summary>
        /// <param name="idHoaDon,nguoiSua "></param>
        /// <returns>string: error</returns>
        public string UpdateStatus_HDDatHang(Guid? idHoaDon, string nguoiSua, int? loaiHoaDon = 1)
        {
            if (db == null)
            {
                return "DB null";
            }
            else
            {
                // find all HD was creat from HD DatHang and chua bi Huy
                var lstHDfromDH = db.BH_HoaDon.Where(x => x.ID_HoaDon == idHoaDon && x.LoaiHoaDon == loaiHoaDon && x.ChoThanhToan != null);

                BH_HoaDon itemDH = db.BH_HoaDon.Find(idHoaDon);

                if (lstHDfromDH != null && lstHDfromDH.Count() > 0)
                {
                    // update YeuCau = Dang giao hang
                    itemDH.YeuCau = "2"; // dang giao hang
                }
                else
                {
                    // neu find count = 0 --> update trang thai HD DatHang --> Phieu tam
                    itemDH.YeuCau = "1"; // Phieu tam
                }

                try
                {
                    itemDH.NguoiSua = nguoiSua;
                    itemDH.NgaySua = DateTime.Now;
                    db.Entry(itemDH).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return "Error";
                }
            }
            return "";
        }

        // huy hoadonsc--> huy phieu xuatkho(8) + update tonkho
        // huy baogia --> tim hdban(1) + update tokho
        public void HuyHoaDonLienQuan(Guid idHoaDon)
        {
            var lst = db.BH_HoaDon.Where(x => x.ID_HoaDon == idHoaDon);
            if (lst != null && lst.Count() > 0)
            {
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                lst.ToList().ForEach(x => x.ChoThanhToan = null);

                // huy quyhd of hoadonlienquan
                (from quy in db.Quy_HoaDon
                 join quyct in
                    // lay ds quyct
                    (db.Quy_HoaDon_ChiTiet.Where(x => lst.Select(y => y.ID).ToList().Contains(x.ID_HoaDonLienQuan ?? Guid.Empty))
                    ) on quy.ID equals quyct.ID_HoaDon
                 select quy).ToList().ForEach(x => x.TrangThai = false);

                // chi update tonkho: neu loai 8,1
                var lstUpdate = lst.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 8);
                foreach (var item in lstUpdate)
                {
                    classhoadonchitiet.UpdateTonKhoGiaVon_whenUpdateCTHD(item.ID, item.ID_DonVi, item.NgayLapHoaDon);
                }
                db.SaveChanges();
            }
        }

        public IQueryable<BH_HoaDon> Gets(Expression<Func<BH_HoaDon, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.BH_HoaDon;
                else
                    return db.BH_HoaDon.Where(query);
            }
        }

        public BH_HoaDon Get(Expression<Func<BH_HoaDon, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var data = db.BH_HoaDon.Where(query);
                if (data != null)
                {
                    return data.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }

        public int CountHoaDonByIDQuiDoi(Guid? iddv)
        {
            try
            {
                var _ClassDVQD = new classDonViQuiDoi(db);
                Guid idhanghoa = _ClassDVQD.Get(p => p.ID == iddv).ID_HangHoa;
                var tbl = from dvqd in db.DonViQuiDois
                          join hdct in db.BH_HoaDon_ChiTiet on dvqd.ID equals hdct.ID_DonViQuiDoi
                          join hd in db.BH_HoaDon on hdct.ID_HoaDon equals hd.ID
                          where dvqd.ID_HangHoa == idhanghoa && hd.ChoThanhToan == false
                          select new
                          {
                              ID = hd.ID
                          };
                if (tbl != null)
                {
                    return tbl.Count();
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("CountHoaDonByIDQuiDoi: " + ex.Message + ex.InnerException);
                return 0;
            }
        }

        public List<BH_HoaDonDTO> CountHoaDonByIDQuiDoiSauOld(Guid? iddv, DateTime? ngayOld)
        {
            try
            {
                var _ClassDVQD = new classDonViQuiDoi(db);
                Guid idhanghoa = _ClassDVQD.Get(p => p.ID == iddv).ID_HangHoa;
                var tbl = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon != 3)
                          join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                          join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
                          where dvqd.ID_HangHoa == idhanghoa && hd.NgayLapHoaDon <= ngayOld && hd.ChoThanhToan == false
                          orderby hd.NgayLapHoaDon, hd.LoaiHoaDon, hd.MaHoaDon
                          select new BH_HoaDonDTO
                          {
                              TienChietKhau = hdct.TienChietKhau,
                              LoaiHoaDon = hd.LoaiHoaDon,
                              GiaVon = hdct.GiaVon,
                              SoLuong = hdct.SoLuong,
                              YeuCau = hd.YeuCau,
                              ID_DonVi = hd.ID_DonVi,
                              ID_CheckIn = hd.ID_CheckIn,
                              ID_DonViQuiDoi = hdct.ID_DonViQuiDoi,
                              NgayLapHoaDon = hd.NgayLapHoaDon,
                              TyLeChuyenDoi = dvqd.TyLeChuyenDoi
                          };
                if (tbl != null)
                {
                    return tbl.ToList();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("CountHoaDonByIDQuiDoiSauOld " + ex.Message + ex.InnerException);
                return null;
            }
        }

        public List<BH_HoaDonDTO> CountHoaDonByIDQuiDoiCanUpdate(Guid? iddv, DateTime? ngayOld)
        {
            try
            {
                var _ClassDVQD = new classDonViQuiDoi(db);
                Guid idhanghoa = _ClassDVQD.Get(p => p.ID == iddv).ID_HangHoa;
                var tbl = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon != 3)
                          join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                          join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
                          where dvqd.ID_HangHoa == idhanghoa && hd.NgayLapHoaDon >= ngayOld && hd.ChoThanhToan == false
                          orderby hd.NgayLapHoaDon, hd.LoaiHoaDon, hd.MaHoaDon
                          select new BH_HoaDonDTO
                          {
                              ID = hdct.ID,
                              ID_DonViQuiDoi = hdct.ID_DonViQuiDoi,
                              MaHoaDon = hd.MaHoaDon,
                              ID_HoaDon = hd.ID,
                              NgayLapHoaDon = hd.NgayLapHoaDon,
                              TienChietKhau = hdct.TienChietKhau,
                              TongGiamGia = hd.TongGiamGia,
                              TongTienHang = hd.TongTienHang,
                              LoaiHoaDon = hd.LoaiHoaDon,
                              GiaVon = hdct.GiaVon,
                              SoLuong = hdct.SoLuong,
                              SoThuTu = hdct.SoThuTu
                          };
                if (tbl != null)
                {
                    return tbl.OrderByDescending(p => p.SoThuTu).ToList();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("CountHoaDonByIDQuiDoiCanUpdate " + ex.Message + ex.InnerException);
                return null;
            }
        }

        public bool BH_HoaDonExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.BH_HoaDon.Count(e => e.ID == id) > 0;
            }
        }

        public HD_QHD_QHDCT JoinHoaDon_SoQuy_byIDHoaDon(Guid? id)
        {
            try
            {
                var tbl = (from hd in db.BH_HoaDon
                           where hd.ID == id
                           select new HD_QHD_QHDCT
                           {
                               MaHoaDonGoc = hd.MaHoaDon,
                               TongTienHDTra = hd.PhaiThanhToan,
                               LoaiHoaDonGoc = hd.LoaiHoaDon,
                           });
                if (tbl.Count() > 0)
                {
                    return tbl.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("JoinHoaDon_SoQuy_byIDHoaDon: " + ex.Message + ex.InnerException);
                return null;
            }
        }

        public double GetTongTienHDDoiTra_fromHDTra(Guid? id)
        {
            double sum = 0;
            try
            {
                var data = from hd in db.BH_HoaDon
                           where hd.ID_HoaDon == id
                           select new { TongTienHang = hd.TongTienHang - hd.TongGiamGia };
                if (data.Count() > 0)
                {
                    sum = data.FirstOrDefault().TongTienHang;
                }
                return sum;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("GetTongTienHDDoiTra_fromHDTra: " + ex.Message + ex.InnerException);
                return 0;
            }
        }

        public List<BH_HoaDonDTO> GetHDTraHang_ofHoaDon(Guid id)
        {
            List<BH_HoaDonDTO> lstReturn = new List<BH_HoaDonDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from hd in db.BH_HoaDon
                          join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID
                          where hd.ID_HoaDon == id
                          select new
                          {
                              ID = hd.ID,
                              MaHoaDon = hd.MaHoaDon,
                              NgayLapHoaDon = hd.NgayLapHoaDon,
                              TenNhanVien = nv.TenNhanVien,
                              // get TrangThai in lst LichSuTraHang; LichSuHoaDon of HDDatHang,
                              TrangThai = hd.ChoThanhToan,
                              TongTienHang = hd.TongTienHang,
                              LoaiHoaDon = hd.LoaiHoaDon,
                          };

                foreach (var item in tbl)
                {
                    var trangthai = string.Empty;
                    switch (item.TrangThai)
                    {
                        case true:
                            trangthai = "Chờ thanh toán";
                            break;
                        case false:
                            if (item.LoaiHoaDon == 6)
                            {
                                trangthai = "Đã trả";
                            }
                            else
                            {
                                trangthai = "Hoàn thành";
                            }
                            break;
                        default:
                            trangthai = "Đã hủy";
                            break;
                    }
                    BH_HoaDonDTO dto = new BH_HoaDonDTO
                    {
                        ID = item.ID,
                        MaHoaDon = item.MaHoaDon,
                        LoaiHoaDon = item.LoaiHoaDon,
                        NgayLapHoaDon = item.NgayLapHoaDon,
                        TenNhanVien = item.TenNhanVien,

                        TrangThai = trangthai,
                        TongTienHang = item.TongTienHang,
                    };
                    lstReturn.Add(dto);
                }
                return lstReturn;
            }
        }

        public List<KhachHang_TabHoaDon> GetHoaDon_FromIDDoiTuong(Guid idDoiTuong, string idChiNhanh = null)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var data = (from hd in db.BH_HoaDon
                            join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID
                            where idChiNhanh.Contains(hd.ID_DonVi.ToString()) && hd.ID_DoiTuong == idDoiTuong
                            select new KhachHang_TabHoaDon
                            {
                                ID = hd.ID,
                                LoaiHoaDon = hd.LoaiHoaDon,
                                MaHoaDon = hd.MaHoaDon,
                                NgayLapHoaDon = hd.NgayLapHoaDon,
                                PhaiThanhToan = hd.LoaiHoaDon == 6 || hd.LoaiHoaDon == 32 ? -hd.PhaiThanhToan : hd.PhaiThanhToan,// trahang + hoantra soduTGT
                                TenNhanVien = nv.TenNhanVien,
                                ChoThanhToan = hd.ChoThanhToan,
                                NguoiTao = hd.NguoiTao,
                            }).ToList();
                return data;
            }
        }

        // not use, change = func SP_GetHoaDonandSoQuy_FromIDDoiTuong
        public List<BH_HoaDonDTO> GetHoaDonandSoQuy_FromIDDoiTuong(Guid idDoiTuong, Guid? idChiNhanh = null)
        {
            List<BH_HoaDonDTO> lstReturn = new List<BH_HoaDonDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                // get HD of DoiTuong (not get Hd DatHang)
                var data1 = from hd in db.BH_HoaDon
                            join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID
                            where hd.ID_DoiTuong == idDoiTuong && hd.LoaiHoaDon != 3 && hd.ChoThanhToan != null && hd.ID_DonVi == idChiNhanh
                            select new
                            {
                                ID = hd.ID,
                                MaHoaDon = hd.MaHoaDon,
                                NgayLapHoaDon = hd.NgayLapHoaDon,
                                GiaTri = hd.LoaiHoaDon == 6 || hd.LoaiHoaDon == 7 ? -hd.PhaiThanhToan : hd.PhaiThanhToan,
                                LoaiHoaDon = hd.LoaiHoaDon,
                            };

                // get SoQuy of DoiTuong
                var data2 = from dt in db.DM_DoiTuong
                            join qct in db.Quy_HoaDon_ChiTiet on dt.ID equals qct.ID_DoiTuong
                            join qhd in db.Quy_HoaDon on qct.ID_HoaDon equals qhd.ID
                            where qct.ID_DoiTuong == idDoiTuong && qhd.TrangThai != false && qhd.ID_DonVi == idChiNhanh
                            select new
                            {
                                ID = qhd.ID,
                                MaHoaDon = qhd.MaHoaDon,
                                NgayLapHoaDon = qhd.NgayLapHoaDon,
                                // KH: thu (-), chi (+), ; NCC: thu (+), chi (-)
                                GiaTri = dt.LoaiDoiTuong == 1 ? qhd.LoaiHoaDon == 11 ? -qhd.TongTienThu : qhd.TongTienThu :
                                qhd.LoaiHoaDon == 11 ? qhd.TongTienThu : -qhd.TongTienThu,
                                LoaiHoaDon = qhd.LoaiHoaDon ?? 0,
                            };

                var data3 = data1.Union(data2);

                var tb_gop = data3.AsEnumerable().Select(t => new
                {
                    ID = t.ID,
                    MaHoaDon = t.MaHoaDon,
                    NgayLapHoaDon = (t.LoaiHoaDon == 11 || t.LoaiHoaDon == 12) ? t.NgayLapHoaDon.AddSeconds(2) : t.NgayLapHoaDon,
                    LoaiHoaDon = t.LoaiHoaDon,
                    GiaTri = Math.Round(t.GiaTri, 3, MidpointRounding.ToEven)
                });

                tb_gop = tb_gop.OrderBy(x => x.NgayLapHoaDon);

                double nodau = 0;
                foreach (var item in tb_gop)
                {
                    BH_HoaDonDTO dto = new BH_HoaDonDTO();
                    dto.ID = item.ID;
                    dto.MaHoaDon = item.MaHoaDon;
                    dto.NgayLapHoaDon = item.NgayLapHoaDon;
                    dto.LoaiHoaDon = item.LoaiHoaDon;
                    dto.PhaiThanhToan = Math.Round(item.GiaTri, 3, MidpointRounding.ToEven);
                    dto.DuNoKH = Math.Round(item.GiaTri + nodau, 3, MidpointRounding.ToEven);
                    switch (item.LoaiHoaDon)
                    {
                        case 1: // HD
                            dto.strLoaiHoaDon = "Bán hàng";
                            break;
                        case 4: // Nhap hang
                            dto.strLoaiHoaDon = "Nhập hàng";
                            break;
                        case 11: // PhieuThu
                            dto.strLoaiHoaDon = "Thanh toán";
                            break;
                        case 6: // Tra Hang KH
                        case 7: // Tra Hang NCC
                            dto.strLoaiHoaDon = "Trả hàng";
                            dto.NgayLapHoaDon = item.NgayLapHoaDon.AddSeconds(1);// HD tra phai sau HD mua --> show in list His ThanhToan right
                            break;
                        case 12: // PhieuChi
                            dto.strLoaiHoaDon = "Thanh toán";
                            break;
                        case 19: // PhieuChi
                            dto.strLoaiHoaDon = "Gói dịch vụ";
                            break;
                    }
                    lstReturn.Add(dto);
                    nodau = item.GiaTri + nodau;
                }
                return lstReturn;
            }
        }

        public List<SoQuyDTO> SP_GetHoaDonandSoQuy_FromIDDoiTuong(Guid idDoiTuong, string idChiNhanh = null)
        {
            List<SoQuyDTO> lstReturn = new List<SoQuyDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("ID_DoiTuong", idDoiTuong));
                lstParam.Add(new SqlParameter("ID_DonVi", idChiNhanh));
                var data3 = db.Database.SqlQuery<SP_HoaDonAndSoQuy>("SP_GetHoaDonAndSoQuy_FromIDDoiTuong @ID_DoiTuong, @ID_DonVi", lstParam.ToArray()).ToList();

                var tb_gop = data3.AsEnumerable().Select(t => new
                {
                    ID = t.ID,
                    MaHoaDon = t.MaHoaDon,
                    NgayLapHoaDon = (t.LoaiHoaDon == 11 || t.LoaiHoaDon == 12) ? t.NgayLapHoaDon.AddSeconds(2) : t.NgayLapHoaDon,
                    LoaiHoaDon = t.LoaiHoaDon,
                    GiaTri = Math.Round(t.GiaTri, 3, MidpointRounding.ToEven),
                    LoaiThanhToan = t.LoaiThanhToan,
                });

                tb_gop = tb_gop.OrderBy(x => x.NgayLapHoaDon).ThenBy(x => x.MaHoaDon).ToList();

                double nodau = 0;
                foreach (var item in tb_gop)
                {
                    SoQuyDTO dto = new SoQuyDTO();
                    dto.ID = item.ID;
                    dto.MaHoaDon = item.MaHoaDon;
                    dto.NgayLapHoaDon = item.NgayLapHoaDon;
                    dto.LoaiHoaDon = item.LoaiHoaDon;
                    dto.LoaiThanhToan = item.LoaiThanhToan;
                    dto.PhaiThanhToan = Math.Round(item.GiaTri, 3, MidpointRounding.ToEven);
                    dto.DuNoKH = Math.Round(item.GiaTri + nodau, 3, MidpointRounding.ToEven);
                    switch (item.LoaiHoaDon)
                    {
                        case 1: // HD
                            dto.strLoaiHoaDon = "Bán hàng";
                            break;
                        case 2: 
                            dto.strLoaiHoaDon = "Hóa đơn bảo hành";
                            break;
                        case 3: // HD
                            dto.strLoaiHoaDon = "Đặt hàng";
                            break;
                        case 4: // Nhap hang
                            dto.strLoaiHoaDon = "Nhập hàng";
                            break;
                        case 11: // PhieuThu
                            dto.strLoaiHoaDon = "Phiếu thu";
                            break;
                        case 6: // Tra Hang KH
                        case 7: // Tra Hang NCC
                            dto.strLoaiHoaDon = "Trả hàng";
                            dto.NgayLapHoaDon = item.NgayLapHoaDon.AddSeconds(1);// HD tra phai sau HD mua --> show in list His ThanhToan right
                            break;
                        case 12: // PhieuChi
                            dto.strLoaiHoaDon = "Phiếu chi";
                            break;
                        case 19:
                            dto.strLoaiHoaDon = "Gói dịch vụ";
                            break;
                        case 22: // TheGiaTri
                            dto.strLoaiHoaDon = "Nạp thẻ giá trị";
                            break;
                        case 23: // DieuChinh
                            dto.strLoaiHoaDon = "Điều chỉnh thẻ giá trị";
                            break;
                        case 25: // HoaDonSuaChua - Gara
                            dto.strLoaiHoaDon = "Hóa đơn sửa chữa";
                            break;
                        case 27: // HD datcoc
                            dto.strLoaiHoaDon = "Nạp tiền đặt cọc";
                            break;
                        case 125: // CP dichvu giacong (hoadon 1 + hdsc)
                            dto.strLoaiHoaDon = "Chi phí hóa đơn";
                            break;
                        case 32: // HD datcoc
                            dto.strLoaiHoaDon = "Trả lại số dư cọc";
                            break;
                    }
                    lstReturn.Add(dto);
                    nodau = item.GiaTri + nodau;
                }
                return lstReturn;
            }
        }

        public List<BH_HoaDonDTO> GetLichSu_TichDiem(Guid id)
        {
            List<BH_HoaDonDTO> lstReturn = new List<BH_HoaDonDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    // get HD chua bi huy va co diem GD >0
                    var lstHD = from hd in db.BH_HoaDon
                                where hd.ChoThanhToan != null && hd.ID_DoiTuong == id && hd.DiemGiaoDich > 0
                                select new
                                {
                                    ID = hd.ID,
                                    NgayLapHoaDon = hd.NgayLapHoaDon,
                                    MaHoaDon = hd.MaHoaDon,
                                    TrangThai = hd.ChoThanhToan,
                                    GiaTri = hd.LoaiHoaDon == 6 ? -hd.PhaiThanhToan : hd.PhaiThanhToan,
                                    DiemGiaoDich = hd.LoaiHoaDon == 6 ? -hd.DiemGiaoDich : hd.DiemGiaoDich,
                                    LoaiHoaDon = hd.LoaiHoaDon,
                                    PhieuDieuChinhCongNo = 0
                                };

                    // get quyhd chua huy (chi get Quy_CT cua HD ChoThanhToan == false)
                    var quyCT = from ct in db.Quy_HoaDon_ChiTiet
                                join quy in db.Quy_HoaDon on ct.ID_HoaDon equals quy.ID
                                join hd in db.BH_HoaDon on ct.ID_HoaDonLienQuan equals hd.ID into HD_Quy
                                from hdquy in HD_Quy.DefaultIfEmpty()
                                where ct.ID_DoiTuong == id && ct.DiemThanhToan > 0 && (quy.TrangThai == true || quy.TrangThai == null)
                                && (hdquy == null || hdquy != null && hdquy.ChoThanhToan == false)
                                select new
                                {
                                    ID = quy.ID,
                                    NgayLapHoaDon = quy.NgayLapHoaDon,
                                    MaHoaDon = quy.MaHoaDon,
                                    TrangThai = quy.TrangThai,
                                    GiaTri = quy.LoaiHoaDon == 12 ? -ct.TienThu : ct.TienThu,
                                    // DieuChinhDiem: Diem, ThanhToanDiem (-Diem)
                                    DiemGiaoDich = quy.MaHoaDon.Contains("CB") ? quy.LoaiHoaDon == 12 ? -ct.DiemThanhToan : ct.DiemThanhToan : -ct.DiemThanhToan,
                                    LoaiHoaDon = quy.LoaiHoaDon ?? 0,
                                    PhieuDieuChinhCongNo = ct.HinhThucThanhToan ?? 0//  muontamtruong = PhieuDieuChinhCongNo
                                };

                    var dataUnion = lstHD.Union(quyCT);
                    var tb_gop = dataUnion.AsEnumerable().Select(t => new
                    {
                        ID = t.ID,
                        MaHoaDon = t.MaHoaDon,
                        NgayLapHoaDon = (t.LoaiHoaDon == 11) ? t.NgayLapHoaDon.AddSeconds(2) : t.NgayLapHoaDon,
                        TrangThai = t.TrangThai,
                        GiaTri = Math.Round(t.GiaTri, 3),
                        DiemGiaoDich = t.DiemGiaoDich,
                        LoaiHoaDon = t.LoaiHoaDon,
                        PhieuDieuChinhCongNo = t.PhieuDieuChinhCongNo
                    });

                    tb_gop = tb_gop.OrderBy(x => x.NgayLapHoaDon);

                    double diemHienTai = 0;
                    foreach (var item in tb_gop)
                    {
                        BH_HoaDonDTO dto = new BH_HoaDonDTO();
                        dto.ID = item.ID;
                        dto.MaHoaDon = item.MaHoaDon;
                        dto.LoaiHoaDon = item.LoaiHoaDon;
                        dto.NgayLapHoaDon = item.NgayLapHoaDon;
                        dto.PhaiThanhToan = Math.Round(item.GiaTri, 3);
                        dto.DiemGiaoDich = item.DiemGiaoDich ?? 0;
                        dto.DiemSauGD = Math.Round(dto.DiemGiaoDich + diemHienTai, 3, MidpointRounding.ToEven);

                        switch (item.LoaiHoaDon)
                        {
                            case 1: // HD
                                dto.strLoaiHoaDon = "Bán hàng";
                                break;
                            case 6: // TH
                                dto.strLoaiHoaDon = "Trả hàng";
                                break;
                            case 11: // PhieuChi
                                if (item.PhieuDieuChinhCongNo == 0)
                                {
                                    dto.strLoaiHoaDon = "Điều chỉnh điểm";
                                }
                                else
                                {
                                    dto.strLoaiHoaDon = "Sử dụng điểm";
                                }
                                break;
                            case 19: // PhieuChi
                                dto.strLoaiHoaDon = "Gói dịch vụ";
                                break;
                            case 22: // PhieuChi
                                dto.strLoaiHoaDon = "Thẻ giá trị";
                                break;
                            case 25: // PhieuChi
                                dto.strLoaiHoaDon = "Hóa đơn sửa chữa";
                                break;
                        }
                        lstReturn.Add(dto);
                        diemHienTai = dto.DiemGiaoDich + diemHienTai;
                    }
                }
                catch (Exception e)
                {
                    CookieStore.WriteLog("GetLichSu_TichDiem: " + e.InnerException + e.Message);
                }
                return lstReturn;
            }
        }

        public IQueryable<BH_HoaDonDTO> GetAllHoaDon()
        {
            classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
            libNS_NhanVien.ClassNS_NhanVien classNhanVien = new libNS_NhanVien.ClassNS_NhanVien(db);

            var tbl = (from hd in db.BH_HoaDon select hd).Select(dto => new BH_HoaDonDTO
            {
                ID = dto.ID,
                MaHoaDon = dto.MaHoaDon,
                NgayLapHoaDon = dto.NgayLapHoaDon,
                TenDoiTuong = (dto.ID_DoiTuong == null) || (dto.ID_DoiTuong == Guid.Empty) ? "" :
         classdoituong.Select_DoiTuong(dto.ID_DoiTuong ?? Guid.Empty).TenDoiTuong,
                NguoiTaoHD = classNhanVien.Select_NhanVien(dto.ID_NhanVien).TenNhanVien,
            });

            if (tbl != null)
                return tbl;
            else
                return null;
        }

        public List<BH_HoaDon_ChiTiet> GroupjoinHD_CTHD(Guid id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = from hd in db.BH_HoaDon
                       join ct in db.BH_HoaDon_ChiTiet on hd.ID_HoaDon equals ct.ID_HoaDon
                       where hd.ID_HoaDon == id
                       group ct by ct.ID_DonViQuiDoi;

            List<BH_HoaDon_ChiTiet> lstReturn = new List<BH_HoaDon_ChiTiet>();

            foreach (var item in data)
            {
                BH_HoaDon_ChiTiet itemDTO = new BH_HoaDon_ChiTiet();
                itemDTO.ID_DonViQuiDoi = item.Key;

                double soluong = 0;
                foreach (var itemIn in item)
                {
                    soluong += itemIn.SoLuong;
                }
                itemDTO.SoLuong = soluong;
                lstReturn.Add(itemDTO);
            }
            return lstReturn;
        }

        public List<BH_HoaDonDTO> getMaSoQuy(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = (from hd in db.BH_HoaDon
                           join qct in db.Quy_HoaDon_ChiTiet on hd.ID equals qct.ID_HoaDonLienQuan
                           join qhd in db.Quy_HoaDon on qct.ID_HoaDon equals qhd.ID
                           select new BH_HoaDonDTO
                           {
                               MaPhieuChi = qhd.MaHoaDon,
                           }).ToList();
                return tbl;
            }
        }

        public List<BH_HoaDonDTO> GetListHoaDons_QuyHD_where(int loaiHoaDon, string maHoaDon,
            int trangThai, string dayStart, string dayEnd, Guid id_donvi, string arrChiNhanh, string columsort, string sort)
        {
            List<BH_HoaDonDTO> lst = new List<BH_HoaDonDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    var tbl = from hd in db.BH_HoaDon
                              join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                              from hd_dt in HD_DT.DefaultIfEmpty()
                              where hd.LoaiHoaDon == loaiHoaDon // && hd.ChoThanhToan == false

                              join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                              from hd_dv in HD_DV.DefaultIfEmpty()
                              where hd.LoaiHoaDon == loaiHoaDon // && hd.ChoThanhToan == false

                              join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                              from hd_nv in HD_NV.DefaultIfEmpty()
                              join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                              from hd_vt in HD_VT.DefaultIfEmpty()
                              join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                              from hd_bg in HD_BG.DefaultIfEmpty()
                              orderby hd.NgayLapHoaDon descending

                              select new BH_HoaDonDTO
                              {
                                  ID = hd.ID,
                                  ID_HoaDon = hd.ID_HoaDon,
                                  ID_CheckIn = hd.ID_CheckIn,
                                  MaHoaDon = hd.MaHoaDon,
                                  NguoiTao = hd.NguoiTao,
                                  TenDonVi = hd_dv.TenDonVi,
                                  DienThoaiChiNhanh = hd_dv.SoDienThoai,
                                  DiaChiChiNhanh = hd_dv.DiaChi,
                                  NgayLapHoaDon = hd.NgayLapHoaDon,
                                  NgaySua = hd.NgaySua,
                                  TongGiamGia = hd.TongGiamGia,
                                  TongTienHang = hd.TongTienHang,
                                  TongChiPhi = hd.TongChiPhi,
                                  PhaiThanhToan = hd.PhaiThanhToan,
                                  ID_NhanVien = hd.ID_NhanVien,
                                  TenNhanVien = hd_nv.TenNhanVien,
                                  TenDoiTuong = hd_dt.TenDoiTuong,
                                  DienGiai = hd.DienGiai,
                                  Email = hd_dt.Email,
                                  DienThoai = hd_dt.DienThoai,
                                  ID_ViTri = hd.ID_ViTri,
                                  TenPhongBan = hd_vt.TenViTri,
                                  NguoiTaoHD = hd.KhuyenMai_GhiChu,
                                  ID_BangGia = hd.ID_BangGia,
                                  TenBangGia = hd_bg.TenGiaBan,
                                  ChoThanhToan = hd.ChoThanhToan,
                                  LoaiHoaDon = hd.LoaiHoaDon,
                                  // trạng thái lưu vào trường yêu cầu
                                  YeuCau = hd.YeuCau,
                                  ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty,
                                  //MaPhieuChi = quy.MaHoaDon,
                                  //ID_PhieuChi = quy.ID,
                                  ID_DonVi = hd_dv.ID
                              };

                    if (maHoaDon != string.Empty && maHoaDon != null)
                    {
                        tbl = tbl.Where(hd => hd.MaHoaDon.Contains(maHoaDon) || hd.TenDoiTuong.Contains(maHoaDon));
                    }


                    List<Guid> lstIDCN = new List<Guid>();
                    if (arrChiNhanh != null)
                    {
                        var arrIDCN = arrChiNhanh.Split(',');
                        for (int i = 0; i < arrIDCN.Length; i++)
                        {
                            lstIDCN.Add(new Guid(arrIDCN[i]));
                        }
                    }
                    if (lstIDCN.Count > 0)
                    {
                        tbl = tbl.Where(p => p.ID_DonVi != null).Where(hd => lstIDCN.Contains(hd.ID_DonVi) || lstIDCN.Contains(hd.ID_CheckIn.Value));
                    }
                    else
                    {
                        if (id_donvi != Guid.Empty && id_donvi != null)
                        {
                            tbl = tbl.Where(hd => (hd.ID_DonVi == id_donvi) || (hd.ID_CheckIn == id_donvi));
                        }
                    }

                    if (dayStart != null && dayStart != string.Empty)
                    {
                        DateTime dtStart = DateTime.Parse(dayStart);
                        if (dayEnd != null && dayEnd != string.Empty)
                        {
                            DateTime dtEnd = DateTime.Parse(dayEnd);
                            if (dayStart == dayEnd)
                            {
                                tbl = tbl.Where(hd => hd.NgayLapHoaDon.Year == dtStart.Year
                                && hd.NgayLapHoaDon.Month == dtStart.Month
                                && hd.NgayLapHoaDon.Day == dtEnd.Day);
                            }
                            else
                            {
                                tbl = tbl.Where(hd => hd.NgayLapHoaDon >= dtStart && hd.NgayLapHoaDon < dtEnd);
                            }
                        }
                        else
                        {
                            tbl = tbl.Where(hd => hd.NgayLapHoaDon >= dtStart);
                        }
                    }
                    else
                    {
                        if (dayEnd != null && dayEnd != string.Empty)
                        {
                            DateTime dtEnd = DateTime.Parse(dayEnd);
                            tbl = tbl.Where(hd => hd.NgayLapHoaDon < dtEnd);
                        }
                    }
                    // trang thai HoaDon
                    switch (trangThai)
                    {
                        case 1: // Đang chuyển
                            tbl = tbl.Where(hd => hd.YeuCau == "1");
                            break;
                        case 2: // phiếu tạm
                            tbl = tbl.Where(hd => hd.YeuCau == "2");
                            break;
                        case 3: // Đã hủy
                            tbl = tbl.Where(hd => hd.YeuCau == "3");
                            break;
                        case 4: //Đã nhận
                            tbl = tbl.Where(hd => hd.YeuCau == "4");
                            break;
                        case 5: // DC +Pt
                            tbl = tbl.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "2");
                            break;
                        case 6: // DC +DH
                            tbl = tbl.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "3");
                            break;
                        case 7: // DC +DN
                            tbl = tbl.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "4");
                            break;
                        case 8: // PT +DH
                            tbl = tbl.Where(hd => hd.YeuCau == "2" || hd.YeuCau == "3");
                            break;
                        case 9: // PT +DH
                            tbl = tbl.Where(hd => hd.YeuCau == "2" || hd.YeuCau == "4");
                            break;
                        case 10:// PT +DH
                            tbl = tbl.Where(hd => hd.YeuCau == "3" || hd.YeuCau == "4");
                            break;
                        case 11: // PT +DH
                            tbl = tbl.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "2" || hd.YeuCau == "3");
                            break;
                        case 12: // PT +DH
                            tbl = tbl.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "2" || hd.YeuCau == "4");
                            break;
                        case 13: // PT +DH
                            tbl = tbl.Where(hd => hd.YeuCau == "2" || hd.YeuCau == "3" || hd.YeuCau == "4");
                            break;
                        case 14: // PT +DH
                            tbl = tbl.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "3" || hd.YeuCau == "4");
                            break;
                        case 15: // PT +DH
                            tbl = tbl.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "2" || hd.YeuCau == "3" || hd.YeuCau == "4");
                            break;
                        case 16: // PT +DH
                            tbl = null;
                            break;
                        default:
                            tbl = tbl.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "2" || hd.YeuCau == "3" || hd.YeuCau == "4");
                            break;
                    }
                    if (tbl != null)
                    {
                        foreach (var item in tbl)
                        {
                            if (item.ID_CheckIn == id_donvi)
                            {
                                BH_HoaDonDTO bH_HoaDonDTO = new BH_HoaDonDTO();
                                bH_HoaDonDTO.ID = item.ID;
                                bH_HoaDonDTO.ID_HoaDon = item.ID_HoaDon;
                                bH_HoaDonDTO.ID_CheckIn = item.ID_CheckIn;
                                bH_HoaDonDTO.MaHoaDon = item.MaHoaDon;
                                bH_HoaDonDTO.TenDonVi = item.TenDonVi;
                                bH_HoaDonDTO.NguoiTao = item.NguoiTao;
                                bH_HoaDonDTO.NguoiTaoHD = item.NguoiTaoHD;
                                bH_HoaDonDTO.NgayLapHoaDon = item.NgayLapHoaDon;
                                bH_HoaDonDTO.NgaySua = item.NgaySua;
                                bH_HoaDonDTO.TongGiamGia = item.TongGiamGia;
                                bH_HoaDonDTO.TongTienHang = item.TongTienHang;
                                bH_HoaDonDTO.TongChiPhi = item.TongChiPhi;
                                bH_HoaDonDTO.PhaiThanhToan = item.PhaiThanhToan;
                                bH_HoaDonDTO.ID_NhanVien = item.ID_NhanVien;
                                bH_HoaDonDTO.TenNhanVien = item.TenNhanVien;
                                bH_HoaDonDTO.TenDoiTuong = item.TenDoiTuong;
                                bH_HoaDonDTO.DienGiai = item.DienGiai;
                                bH_HoaDonDTO.Email = item.Email;
                                bH_HoaDonDTO.DienThoai = item.DienThoai;
                                bH_HoaDonDTO.ID_ViTri = item.ID_ViTri;
                                bH_HoaDonDTO.TenPhongBan = item.TenPhongBan;
                                bH_HoaDonDTO.TenNhanVien = item.TenNhanVien;
                                bH_HoaDonDTO.ID_BangGia = item.ID_BangGia;
                                bH_HoaDonDTO.TenBangGia = item.TenBangGia;
                                bH_HoaDonDTO.ChoThanhToan = item.ChoThanhToan;
                                bH_HoaDonDTO.LoaiHoaDon = item.LoaiHoaDon;
                                // trạng thái lưu vào trường yêu cầu
                                bH_HoaDonDTO.YeuCau = item.YeuCau;
                                bH_HoaDonDTO.ID_DoiTuong = item.ID_DoiTuong;
                                bH_HoaDonDTO.MaPhieuChi = item.MaHoaDon;
                                bH_HoaDonDTO.ID_PhieuChi = item.ID_PhieuChi;
                                bH_HoaDonDTO.ID_DonVi = item.ID_DonVi;
                                lst.Add(bH_HoaDonDTO);
                            }
                            else
                            {
                                //if (item.YeuCau != "2")
                                //{
                                BH_HoaDonDTO bH_HoaDonDTO = new BH_HoaDonDTO();
                                bH_HoaDonDTO.ID = item.ID;
                                bH_HoaDonDTO.ID_HoaDon = item.ID_HoaDon;
                                bH_HoaDonDTO.ID_CheckIn = item.ID_CheckIn;
                                bH_HoaDonDTO.MaHoaDon = item.MaHoaDon;
                                bH_HoaDonDTO.TenDonVi = item.TenDonVi;
                                bH_HoaDonDTO.NguoiTao = item.NguoiTao;
                                bH_HoaDonDTO.NguoiTaoHD = item.NguoiTaoHD;
                                bH_HoaDonDTO.NgayLapHoaDon = item.NgayLapHoaDon;
                                bH_HoaDonDTO.NgaySua = item.NgaySua;
                                bH_HoaDonDTO.TongGiamGia = item.TongGiamGia;
                                bH_HoaDonDTO.TongTienHang = item.TongTienHang;
                                bH_HoaDonDTO.TongChiPhi = item.TongChiPhi;
                                bH_HoaDonDTO.PhaiThanhToan = item.PhaiThanhToan;
                                bH_HoaDonDTO.ID_NhanVien = item.ID_NhanVien;
                                bH_HoaDonDTO.TenNhanVien = item.TenNhanVien;
                                bH_HoaDonDTO.TenDoiTuong = item.TenDoiTuong;
                                bH_HoaDonDTO.DienGiai = item.DienGiai;
                                bH_HoaDonDTO.Email = item.Email;
                                bH_HoaDonDTO.DienThoai = item.DienThoai;
                                bH_HoaDonDTO.ID_ViTri = item.ID_ViTri;
                                bH_HoaDonDTO.TenPhongBan = item.TenPhongBan;
                                bH_HoaDonDTO.TenNhanVien = item.TenNhanVien;
                                bH_HoaDonDTO.ID_BangGia = item.ID_BangGia;
                                bH_HoaDonDTO.TenBangGia = item.TenBangGia;
                                bH_HoaDonDTO.ChoThanhToan = item.ChoThanhToan;
                                bH_HoaDonDTO.LoaiHoaDon = item.LoaiHoaDon;
                                // trạng thái lưu vào trường yêu cầu
                                bH_HoaDonDTO.YeuCau = item.YeuCau;
                                bH_HoaDonDTO.ID_DoiTuong = item.ID_DoiTuong;
                                bH_HoaDonDTO.MaPhieuChi = item.MaHoaDon;
                                bH_HoaDonDTO.ID_PhieuChi = item.ID_PhieuChi;
                                bH_HoaDonDTO.ID_DonVi = item.ID_DonVi;
                                lst.Add(bH_HoaDonDTO);
                                //}
                            }
                        }
                        if (sort != "null")
                        {
                            if (sort == "0")
                            {
                                if (columsort == "MaHoaDon")
                                {
                                    lst = lst.OrderBy(p => p.MaHoaDon).ToList();
                                }
                                if (columsort == "TuChiNhanh")
                                {
                                    lst = lst.OrderBy(p => p.TenDonVi).ToList();
                                }
                                if (columsort == "NguoiTao")
                                {
                                    lst = lst.OrderBy(p => p.NguoiTao).ToList();
                                }
                                if (columsort == "ToiChiNhanh")
                                {
                                    lst = lst.OrderBy(p => p.TenDonViChuyen).ToList();
                                }
                                if (columsort == "NguoiNhan")
                                {
                                    lst = lst.OrderBy(p => p.NguoiTaoHD).ToList();
                                }
                                if (columsort == "NgayChuyen")
                                {
                                    lst = lst.OrderBy(p => p.NgayLapHoaDon).ToList();
                                }
                                if (columsort == "NgayNhan")
                                {
                                    lst = lst.OrderBy(p => p.NgaySua).ToList();
                                }
                                if (columsort == "GiaChuyen")
                                {
                                    lst = lst.OrderBy(p => p.TongTienHang).ToList();
                                }
                                if (columsort == "GiaNhan")
                                {
                                    lst = lst.OrderBy(p => p.TongChiPhi).ToList();
                                }
                                if (columsort == "GhiChu")
                                {
                                    lst = lst.OrderBy(p => p.DienGiai).ToList();
                                }
                            }
                            else
                            {
                                if (columsort == "MaHoaDon")
                                {
                                    lst = lst.OrderByDescending(p => p.MaHoaDon).ToList();
                                }
                                if (columsort == "TuChiNhanh")
                                {
                                    lst = lst.OrderByDescending(p => p.TenDonVi).ToList();
                                }
                                if (columsort == "NguoiTao")
                                {
                                    lst = lst.OrderByDescending(p => p.NguoiTao).ToList();
                                }
                                if (columsort == "ToiChiNhanh")
                                {
                                    lst = lst.OrderByDescending(p => p.TenDonViChuyen).ToList();
                                }
                                if (columsort == "NguoiNhan")
                                {
                                    lst = lst.OrderByDescending(p => p.NguoiTaoHD).ToList();
                                }
                                if (columsort == "NgayChuyen")
                                {
                                    lst = lst.OrderByDescending(p => p.NgayLapHoaDon).ToList();
                                }
                                if (columsort == "NgayNhan")
                                {
                                    lst = lst.OrderByDescending(p => p.NgaySua).ToList();
                                }
                                if (columsort == "GiaChuyen")
                                {
                                    lst = lst.OrderByDescending(p => p.TongTienHang).ToList();
                                }
                                if (columsort == "GiaNhan")
                                {
                                    lst = lst.OrderByDescending(p => p.TongChiPhi).ToList();
                                }
                                if (columsort == "GhiChu")
                                {
                                    lst = lst.OrderByDescending(p => p.DienGiai).ToList();
                                }
                            }
                        }
                        return lst;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("GetListHoaDons_QuyHD_where: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }

        public List<BH_HoaDonDTO> SP_GetHoaDonChoThanhToan(int loaiHoaDon, string idDonVi)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("LoaiHoaDon", loaiHoaDon));
            lstParam.Add(new SqlParameter("ID_DonVi", idDonVi));
            try
            {
                return db.Database.SqlQuery<BH_HoaDonDTO>("EXEC SP_GetHoaDonChoThanhToan @LoaiHoaDon, @ID_DonVi", lstParam.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_GetHoaDonChoThanhToan: " + ex.Message + ex.InnerException);
                return null;
            }
        }
        public List<BH_HoaDonDTO> SP_GetHoaDonChoThanhToanNhaBep(int loaiHoaDon, string idDonVi)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("LoaiHoaDon", loaiHoaDon));
            lstParam.Add(new SqlParameter("ID_DonVi", idDonVi));
            try
            {
                return db.Database.SqlQuery<BH_HoaDonDTO>("EXEC SP_GetHoaDonThanhToanNhaBep @LoaiHoaDon, @ID_DonVi", lstParam.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_GetHoaDonChoThanhToan: " + ex.Message + ex.InnerException);
                return null;
            }
        }

        public List<BH_HoaDonDTO> GetListHoaDons_QuyHD(int loaiHoaDon)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    var tbl = from hd in db.BH_HoaDon
                              join qct in db.Quy_HoaDon_ChiTiet on hd.ID equals qct.ID_HoaDonLienQuan into Quy_HD
                              from quy_hd in Quy_HD.DefaultIfEmpty()
                              join qhd in db.Quy_HoaDon on quy_hd.ID_HoaDon equals qhd.ID into QUY
                              from quy in QUY.DefaultIfEmpty()
                              join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                              from hd_dt in HD_DT.DefaultIfEmpty()
                              where hd.LoaiHoaDon == loaiHoaDon // && hd.ChoThanhToan == false

                              join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                              from hd_dv in HD_DV.DefaultIfEmpty()

                              join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                              from hd_nv in HD_NV.DefaultIfEmpty()
                              join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                              from hd_vt in HD_VT.DefaultIfEmpty()
                              where hd_vt.TinhTrang != true // get phongban chua xoa
                              join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                              from hd_bg in HD_BG.DefaultIfEmpty()
                              orderby hd.NgayLapHoaDon descending

                              select new BH_HoaDonDTO
                              {
                                  ID = hd.ID,
                                  ID_HoaDon = hd.ID_HoaDon,
                                  ID_CheckIn = hd.ID_CheckIn,
                                  MaHoaDon = hd.MaHoaDon,
                                  TenDonVi = hd_dv.TenDonVi,
                                  NgayLapHoaDon = hd.NgayLapHoaDon,
                                  TongGiamGia = hd.TongGiamGia,
                                  TongTienHang = hd.TongTienHang,
                                  TongChiPhi = hd.TongChiPhi,
                                  PhaiThanhToan = hd.PhaiThanhToan,
                                  ID_NhanVien = hd.ID_NhanVien,
                                  TenNhanVien = hd_nv.TenNhanVien,
                                  TenDoiTuong = hd_dt.TenDoiTuong,
                                  DienGiai = hd.DienGiai,
                                  Email = hd_dt.Email,
                                  DienThoai = hd_dt.DienThoai,
                                  ID_ViTri = hd.ID_ViTri,
                                  TenPhongBan = hd_vt.TenViTri,
                                  NguoiTaoHD = hd_nv.NguoiTao,
                                  ID_BangGia = hd.ID_BangGia,
                                  TenBangGia = hd_bg.TenGiaBan,
                                  ChoThanhToan = hd.ChoThanhToan,
                                  LoaiHoaDon = hd.LoaiHoaDon,
                                  // trạng thái lưu vào trường yêu cầu
                                  YeuCau = hd.YeuCau,
                                  ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty,
                                  MaPhieuChi = quy.MaHoaDon,
                                  ID_PhieuChi = quy.ID,
                                  KhachDaTra = (double?)quy.TongTienThu ?? 0,
                              };
                    return tbl.ToList();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("GetListHoaDons_QuyHD: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }

        public List<BH_HoaDonDTO> GetListHoaDonsKK_Where(int loaiHoaDon, string maHoaDon,
            int trangThai, string dayStart, string dayEnd, string iddonvi, string arrChiNhanh, string columsort, string sort)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    var tbl = from hd in db.BH_HoaDon
                              join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                              from hd_dt in HD_DT.DefaultIfEmpty()
                              where hd.LoaiHoaDon == loaiHoaDon || hd.LoaiHoaDon == 2 // && hd.ChoThanhToan == false

                              join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                              from hd_dv in HD_DV.DefaultIfEmpty()
                              where hd.LoaiHoaDon == loaiHoaDon || hd.LoaiHoaDon == 2 // && hd.ChoThanhToan == false

                              join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                              from hd_nv in HD_NV.DefaultIfEmpty()
                              join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                              from hd_vt in HD_VT.DefaultIfEmpty()
                              join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                              from hd_bg in HD_BG.DefaultIfEmpty()
                              orderby hd.NgayLapHoaDon descending

                              select new BH_HoaDonDTO
                              {
                                  ID = hd.ID,
                                  MaHoaDon = hd.MaHoaDon,
                                  TenDonVi = hd_dv.TenDonVi,
                                  NgayLapHoaDon = hd.NgayLapHoaDon,
                                  TongGiamGia = hd.TongGiamGia,
                                  TongChiPhi = hd.TongChiPhi,
                                  TongTienHang = hd.TongTienHang,
                                  TongChietKhau = hd.TongChietKhau,
                                  TongTienThue = hd.TongTienThue,
                                  PhaiThanhToan = hd.PhaiThanhToan,
                                  ID_NhanVien = hd.ID_NhanVien,
                                  TenNhanVien = hd_nv.TenNhanVien,
                                  NguoiTao = hd.NguoiTao,
                                  TenDoiTuong = hd_dt.TenDoiTuong,
                                  DienGiai = hd.DienGiai,
                                  Email = hd_dt.Email,
                                  DienThoai = hd_dt.DienThoai,
                                  ID_ViTri = hd.ID_ViTri,
                                  TenPhongBan = hd_vt.TenViTri,
                                  NguoiTaoHD = hd_nv.TenNhanVien,
                                  ID_BangGia = hd.ID_BangGia,
                                  TenBangGia = hd_bg.TenGiaBan,
                                  ChoThanhToan = hd.ChoThanhToan,
                                  ID_DonVi = hd_dv.ID,
                                  // trạng thái lưu vào trường yêu cầu
                                  YeuCau = hd.YeuCau == null ? "" : (hd.YeuCau == "1" ? "Phiếu tạm" : (hd.YeuCau == "2" ? "Đang hủy" : (hd.YeuCau == "3" ? "Đã nhận" : (hd.YeuCau == "4" ? "Đã hủy" : "")))),
                                  ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty
                              };

                    // get list ID_ViTri
                    if (maHoaDon != string.Empty && maHoaDon != null)
                    {
                        tbl = tbl.Where(hd => hd.MaHoaDon.Contains(maHoaDon) || hd.TenDoiTuong.Contains(maHoaDon) || hd.DienThoai.Contains(maHoaDon));
                    }

                    List<Guid> lstIDCN = new List<Guid>();
                    if (arrChiNhanh != null)
                    {
                        var arrIDCN = arrChiNhanh.Split(',');
                        for (int i = 0; i < arrIDCN.Length; i++)
                        {
                            lstIDCN.Add(new Guid(arrIDCN[i]));
                        }
                    }
                    if (lstIDCN.Count > 0)
                    {
                        tbl = tbl.Where(p => p.ID_DonVi != null).Where(hd => lstIDCN.Contains(hd.ID_DonVi));
                    }
                    else
                    {
                        if (iddonvi != String.Empty && iddonvi != null)
                        {
                            tbl = tbl.Where(hd => hd.ID_DonVi.ToString().Contains(iddonvi));
                        }
                    }
                    // NgayLapHoaDon

                    if (dayStart != null && dayStart != string.Empty)
                    {
                        DateTime dtStart = DateTime.Parse(dayStart);
                        if (dayEnd != null && dayEnd != string.Empty)
                        {
                            DateTime dtEnd = DateTime.Parse(dayEnd);
                            if (dayStart == dayEnd)
                            {
                                tbl = tbl.Where(hd => hd.NgayLapHoaDon.Year == dtStart.Year
                                && hd.NgayLapHoaDon.Month == dtStart.Month
                                && hd.NgayLapHoaDon.Day == dtEnd.Day);
                            }
                            else
                            {
                                tbl = tbl.Where(hd => hd.NgayLapHoaDon >= dtStart && hd.NgayLapHoaDon < dtEnd);
                            }
                        }
                        else
                        {
                            tbl = tbl.Where(hd => hd.NgayLapHoaDon >= dtStart);
                        }
                    }
                    else
                    {
                        if (dayEnd != null && dayEnd != string.Empty)
                        {
                            DateTime dtEnd = DateTime.Parse(dayEnd);
                            tbl = tbl.Where(hd => hd.NgayLapHoaDon < dtEnd);
                        }
                    }
                    // trang thai HoaDon
                    switch (trangThai)
                    {
                        case 1: // Đã hủy
                            tbl = tbl.Where(hd => hd.ChoThanhToan == null);
                            break;
                        case 2: // phiếu tạm
                            tbl = tbl.Where(hd => hd.ChoThanhToan == true);
                            break;
                        case 3: // Hoàn thành
                            tbl = tbl.Where(hd => hd.ChoThanhToan == false);
                            break;
                        case 4: // phiếu tạm + Đã hủy
                            tbl = tbl.Where(hd => hd.ChoThanhToan != false);
                            break;
                        case 5: // Hoàn thành + Đã hủy
                            tbl = tbl.Where(hd => hd.ChoThanhToan != true);
                            break;
                        case 6: // Hoàn thành + Đã hủy
                            break;
                        case 7: //
                            tbl = null;
                            break;
                        case 0: // Hoàn thành + Đã hủy
                            tbl = tbl.Where(hd => hd.ChoThanhToan != null);
                            break;
                        default: // tam luu
                            tbl = tbl.Where(hd => hd.ChoThanhToan != null);
                            break;
                    }
                    if (sort != null)
                    {
                        if (sort == "0")
                        {
                            if (columsort == "MaHoaDon")
                            {
                                tbl = tbl.OrderBy(p => p.MaHoaDon);
                            }
                            if (columsort == "ThoiGian")
                            {
                                tbl = tbl.OrderBy(p => p.NgayLapHoaDon);
                            }
                            if (columsort == "DonVi")
                            {
                                tbl = tbl.OrderBy(p => p.TenDonVi);
                            }
                            if (columsort == "GhiChu")
                            {
                                tbl = tbl.OrderBy(p => p.DienGiai);
                            }
                        }
                        else
                        {
                            if (columsort == "MaHoaDon")
                            {
                                tbl = tbl.OrderByDescending(p => p.MaHoaDon);
                            }
                            if (columsort == "ThoiGian")
                            {
                                tbl = tbl.OrderByDescending(p => p.NgayLapHoaDon);
                            }
                            if (columsort == "DonVi")
                            {
                                tbl = tbl.OrderByDescending(p => p.TenDonVi);
                            }
                            if (columsort == "GhiChu")
                            {
                                tbl = tbl.OrderByDescending(p => p.DienGiai);
                            }
                        }
                    }

                    return tbl.ToList();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("classBH_HoaDon - GetListHoaDonsKK_Where: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }

        /// <summary>
        /// get infor hoa don BH_HoaDonDTO
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BH_HoaDonDTO SP_GetInforHoaDon_byID(Guid id)
        {
            try
            {
                SqlParameter param = new SqlParameter("ID_HoaDon", id);
                var data = db.Database.SqlQuery<BH_HoaDonDTO>("EXEC GetInforHoaDon_ByID @ID_HoaDon", param).ToList();
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                if (data != null && data.Count() > 0)
                {
                    var itemHD = data.FirstOrDefault();
                    itemHD.BH_HoaDon_ChiTiet = SP_GetChiTietHD_byIDHoaDon(id);
                    foreach (var ct in itemHD.BH_HoaDon_ChiTiet)
                    {
                        if (ct.ID_ChiTietDinhLuong != null && ct.ID_ChiTietDinhLuong == ct.ID)
                        {
                            ct.ThanhPhan_DinhLuong = classhoadonchitiet.SP_GetThanhPhanDinhLuong_CTHD(ct.ID);
                        }
                        if (ct.ID_ParentCombo != null && ct.ID_ParentCombo == ct.ID)
                        {
                            ct.ThanhPhanComBo = GetListComBo_ofCTHD(ct.ID_HoaDon ?? Guid.Empty, ct.ID.ToString());
                        }
                    }
                    return itemHD;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("GetInforHoaDon_ByID " + e.InnerException + e.Message);
                return null;
            }
        }

        /// <summary>
        /// get infor hoa don BH_HoaDonDTO by MaHoaDon (used to when click MaHoaDon and show infor)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BH_HoaDonDTO SP_GetInforHoaDon_byMaHoaDon(string maHoaDon)
        {
            try
            {
                SqlParameter param = new SqlParameter("MaHoaDon", maHoaDon);
                var data = db.Database.SqlQuery<BH_HoaDonDTO>("EXEC SP_GetInforHoaDon_ByMaHoaDon @MaHoaDon", param).ToList();
                if (data != null && data.Count() > 0)
                {
                    var itemHD = data.FirstOrDefault();
                    itemHD.BH_HoaDon_ChiTiet = SP_GetChiTietHD_byIDHoaDon(itemHD.ID ?? Guid.Empty);
                    return itemHD;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("SP_GetInforHoaDon_byMaHoaDon " + e.InnerException + e.Message);
                return null;
            }
        }

        /// <summary>
        /// get TongThu from HDXuLy from HDDatHang
        /// </summary>
        /// <param name="idHoaDon"></param>
        /// <returns></returns>
        public double SP_GetTongThu_fromHDXuLy(Guid idHoaDon)
        {
            try
            {
                SqlParameter param = new SqlParameter("ID_HoaDon", idHoaDon);
                var data = db.Database.SqlQuery<double>("EXEC SP_GetTongThu_fromHDXuLy @ID_HoaDon", param);

                if (data != null && data.Count() > 0)
                {
                    return data.FirstOrDefault();
                }
                else
                {
                    return 0.0;
                }
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("SP_GetTongThu_fromHDXuLy " + e.InnerException + e.Message);
                return 0.0;
            }
        }

        public List<BH_HoaDon_ChiTietDTO> SP_GetChiTietHD_byIDHoaDon(Guid id)
        {
            SqlParameter param = new SqlParameter("ID_HoaDon", id);
            var data = db.Database.SqlQuery<BH_HoaDon_ChiTietDTO>("EXEC GetChiTietHoaDon_ByIDHoaDon @ID_HoaDon", param).ToList();
            foreach (var item in data)
            {
                item.DonViTinh = db.DonViQuiDois.Where(ct => ct.ID_HangHoa == item.ID_HangHoa && ct.Xoa != true)
                            .Select(x => new DonViTinh
                            {
                                ID_HangHoa = item.ID_HangHoa,
                                TenDonViTinh = x.TenDonViTinh,
                                ID_DonViQuiDoi = x.ID,
                                QuanLyTheoLoHang = item.QuanLyTheoLoHang,
                                Xoa = false,
                                TyLeChuyenDoi = x.TyLeChuyenDoi
                            }).ToList();
            }
            return data;
        }

        public List<BH_HoaDon_ChiTietDTO> SP_GetChiTietHD_byIDHoaDon_ChietKhauNV(Guid id)
        {
            SqlParameter param = new SqlParameter("ID_HoaDon", id);
            try
            {
                var data = db.Database.SqlQuery<BH_HoaDon_ChiTietDTO>("EXEC GetChiTietHoaDon_ByIDHoaDon @ID_HoaDon", param).ToList();
                if (data != null && data.Count() > 0)
                {
                    ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);

                    foreach (var item in data)
                    {
                        var nv_ck = (from nv in db.NS_NhanVien
                                     join bh_nv in db.BH_NhanVienThucHien
                                     on nv.ID equals bh_nv.ID_NhanVien
                                     where bh_nv.ID_ChiTietHoaDon == item.ID
                                     select new BH_NhanVienThucHienDTO
                                     {
                                         ID_NhanVien = bh_nv.ID_NhanVien,
                                         TenNhanVien = nv.TenNhanVien,
                                         ID_ChiTietHoaDon = bh_nv.ID_ChiTietHoaDon,
                                         ThucHien_TuVan = bh_nv.ThucHien_TuVan,
                                         TienChietKhau = bh_nv.TienChietKhau,
                                         PT_ChietKhau = bh_nv.PT_ChietKhau,
                                         TheoYeuCau = bh_nv.TheoYeuCau,
                                         HeSo = bh_nv.HeSo,
                                         TinhChietKhauTheo = bh_nv.TinhChietKhauTheo,
                                         TinhHoaHongTruocCK = bh_nv.TinhHoaHongTruocCK != null ? bh_nv.TinhHoaHongTruocCK : 0,
                                     }).ToList();
                        if (item.ID_ChiTietDinhLuong != null && item.ID_ChiTietDinhLuong == item.ID)
                        {
                            item.ThanhPhan_DinhLuong = classhoadonchitiet.SP_GetThanhPhanDinhLuong_CTHD(item.ID);
                        }
                        item.BH_NhanVienThucHien = nv_ck;
                        item.DonViTinh = db.DonViQuiDois.Where(ct => ct.ID_HangHoa == item.ID_HangHoa && ct.Xoa != true)
                            .Select(x => new DonViTinh
                            {
                                ID_HangHoa = item.ID_HangHoa,
                                TenDonViTinh = x.TenDonViTinh,
                                ID_DonViQuiDoi = x.ID,
                                QuanLyTheoLoHang = item.QuanLyTheoLoHang,
                                Xoa = false,
                                TyLeChuyenDoi = x.TyLeChuyenDoi
                            }).ToList();
                    }
                }
                return data;
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("SP_GetChiTietHD_byIDHoaDon_ChietKhauNV: " + e.InnerException + e.Message);
                return null;
            }
        }
        public List<BH_HoaDon_ChiTietDTO> GetListComBo_ofCTHD(Guid idHoaDon, string idCTHD = null)
        {
            string id = "%%";
            if (!string.IsNullOrEmpty(idCTHD))
            {
                id = idCTHD;
            }
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("ID_HoaDon", idHoaDon));
            param.Add(new SqlParameter("IDChiTiet", id));
            var data = db.Database.SqlQuery<BH_HoaDon_ChiTietDTO>("EXEC GetListComBo_ofCTHD @ID_HoaDon, @IDChiTiet", param.ToArray()).ToList();
            if (data != null && data.Count() > 0)
            {
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                foreach (var item in data)
                {
                    var nv_ck = (from nv in db.NS_NhanVien
                                 join bh_nv in db.BH_NhanVienThucHien
                                 on nv.ID equals bh_nv.ID_NhanVien
                                 where bh_nv.ID_ChiTietHoaDon == item.ID
                                 select new BH_NhanVienThucHienDTO
                                 {
                                     ID_NhanVien = bh_nv.ID_NhanVien,
                                     TenNhanVien = nv.TenNhanVien,
                                     ID_ChiTietHoaDon = bh_nv.ID_ChiTietHoaDon,
                                     ThucHien_TuVan = bh_nv.ThucHien_TuVan,
                                     TienChietKhau = bh_nv.TienChietKhau,
                                     PT_ChietKhau = bh_nv.PT_ChietKhau,
                                     TheoYeuCau = bh_nv.TheoYeuCau,
                                     HeSo = bh_nv.HeSo,
                                     TinhChietKhauTheo = bh_nv.TinhChietKhauTheo,
                                     TinhHoaHongTruocCK = bh_nv.TinhHoaHongTruocCK != null ? bh_nv.TinhHoaHongTruocCK : 0,
                                 }).ToList();
                    if (item.ID_ChiTietDinhLuong != null && item.ID_ChiTietDinhLuong == item.ID)
                    {
                        item.ThanhPhan_DinhLuong = classhoadonchitiet.SP_GetThanhPhanDinhLuong_CTHD(item.ID);
                    }
                    item.BH_NhanVienThucHien = nv_ck;
                }
            }
            return data;
        }

        public List<BH_NhanVienThucHienDTO> GetNVThucHienDichVu(Guid idChiTiet)
        {
            List<BH_NhanVienThucHienDTO> lstNV = (from nv in db.NS_NhanVien
                                                  join bh_nv in db.BH_NhanVienThucHien
                                                  on nv.ID equals bh_nv.ID_NhanVien
                                                  where bh_nv.ID_ChiTietHoaDon == idChiTiet
                                                  select new BH_NhanVienThucHienDTO
                                                  {
                                                      ID_NhanVien = bh_nv.ID_NhanVien,
                                                      TenNhanVien = nv.TenNhanVien,
                                                      ID_ChiTietHoaDon = bh_nv.ID_ChiTietHoaDon,
                                                      ThucHien_TuVan = bh_nv.ThucHien_TuVan,
                                                      TienChietKhau = bh_nv.TienChietKhau,
                                                      PT_ChietKhau = bh_nv.PT_ChietKhau,
                                                      TheoYeuCau = bh_nv.TheoYeuCau,
                                                      HeSo = bh_nv.HeSo,
                                                      TinhChietKhauTheo = bh_nv.TinhChietKhauTheo,
                                                      TinhHoaHongTruocCK = bh_nv.TinhHoaHongTruocCK != null ? bh_nv.TinhHoaHongTruocCK : 0,
                                                  }).ToList();
            return lstNV;
        }

        public List<BH_HoaDonDTO> GetListHDbyIDs(string sIDs)
        {
            SqlParameter param = new SqlParameter("IDHoaDons", sIDs);
            return db.Database.SqlQuery<BH_HoaDonDTO>("EXEC GetListHDbyIDs @IDHoaDons", param).ToList();
        }
        public List<BH_HoaDon_ChiTietDTO> SP_GetChiTietHD_MultipleHoaDon(string arrID_HoaDon)
        {
            SqlParameter param = new SqlParameter("lstID_HoaDon", arrID_HoaDon);
            try
            {
                var data = db.Database.SqlQuery<BH_HoaDon_ChiTietDTO>("EXEC GetChiTietHD_MultipleHoaDon @lstID_HoaDon", param).ToList();
                if (data != null && data.Count() > 0)
                {
                    data = data.OrderBy(x => x.SoThuTu).ToList();
                    foreach (var item in data)
                    {
                        var nv_ck = (from nv in db.NS_NhanVien
                                     join bh_nv in db.BH_NhanVienThucHien
                                     on nv.ID equals bh_nv.ID_NhanVien
                                     where bh_nv.ID_ChiTietHoaDon == item.ID
                                     select new BH_NhanVienThucHienDTO
                                     {
                                         ID_NhanVien = bh_nv.ID_NhanVien,
                                         TenNhanVien = nv.TenNhanVien,
                                         ID_ChiTietHoaDon = bh_nv.ID_ChiTietHoaDon,
                                         ThucHien_TuVan = bh_nv.ThucHien_TuVan,
                                         TienChietKhau = bh_nv.TienChietKhau,
                                         PT_ChietKhau = bh_nv.PT_ChietKhau,
                                         HeSo = bh_nv.HeSo,
                                         ID_HoaDon = bh_nv.ID_HoaDon,
                                         TheoYeuCau = bh_nv.TheoYeuCau,
                                         TinhChietKhauTheo = bh_nv.TinhChietKhauTheo,
                                         TinhHoaHongTruocCK = bh_nv.TinhHoaHongTruocCK != null ? bh_nv.TinhHoaHongTruocCK : 0,
                                     }).ToList();
                        item.BH_NhanVienThucHien = nv_ck;
                    }
                }
                return data;
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("SP_GetChiTietHD_MultipleHoaDon " + e.InnerException + e.Message);
                return null;
            }
        }

        #region Service package
        public List<SP_NhatKySuDung_GoiDV> SP_NhatKySuDung_GoiDV(List<Guid> lstIDKhachHang, Guid? ID_DonVi, int currentPage, int pageSize)
        {
            try
            {
                if (lstIDKhachHang != null && lstIDKhachHang.Count > 0)
                {
                    string ids = string.Join(",", lstIDKhachHang);
                    List<SqlParameter> paramlist = new List<SqlParameter>();
                    paramlist.Add(new SqlParameter("idDoiTuongs", ids));
                    paramlist.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                    paramlist.Add(new SqlParameter("CurrentPage", currentPage));
                    paramlist.Add(new SqlParameter("PageSize", pageSize));
                    var data = db.Database.SqlQuery<SP_NhatKySuDung_GoiDV>("EXEC SP_NhatKySuDung_GoiDV @idDoiTuongs, @ID_DonVi, @CurrentPage, @PageSize", paramlist.ToArray()).ToList();
                    return data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_NhatKySuDung_GoiDV: " + ex.InnerException + ex.Message);
                return null;
            }
        }

        public List<SP_NhatKySuDung_GoiDV> GetNhatKySuDung_GDV(ParamNKyGDV param)
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
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idChiNhanhs ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("IDCustomers", idCus ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("TextSearch", param.TextSearch ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("DateFrom", param.DateFrom ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("DateTo", param.DateTo ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("LoaiHoaDons", param.LoaiHoaDons ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("CurrentPage", param.CurrentPage ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("PageSize", param.PageSize ?? (object)DBNull.Value));
            return db.Database.SqlQuery<SP_NhatKySuDung_GoiDV>("exec GetNhatKySuDung_GDV @IDChiNhanhs, @IDCustomers, @TextSearch," +
                "@DateFrom, @DateTo, @LoaiHoaDons, @CurrentPage, @PageSize", sql.ToArray()).ToList();
        }

        public List<SP_InforServicePackage> GetNhatKyGiaoDich_ofCus(ParamNKyGDV param)
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
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idChiNhanhs ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("IDCustomers", idCus ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("IDCars", idCar ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("LoaiHoaDons", param.LoaiHoaDons ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("CurrentPage", param.CurrentPage ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("PageSize", param.PageSize ?? (object)DBNull.Value));
            return db.Database.SqlQuery<SP_InforServicePackage>("exec GetNhatKyGiaoDich_ofCus @IDChiNhanhs, @IDCustomers, @IDCars,@LoaiHoaDons, @CurrentPage, @PageSize", sql.ToArray()).ToList();
        }

        public List<SP_InforServicePackage> GetListImgInvoice_byCus(ParamNKyGDV param)
        {
            var idCus = string.Empty;
            if (param.IDCustomers != null && param.IDCustomers.Count > 0)
            {
                idCus = string.Join(",", param.IDCustomers);
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("TextSearch", param.TextSearch ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("ID_Customer", idCus ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("CurrentPage", param.CurrentPage ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("PageSize", param.PageSize ?? (object)DBNull.Value));
            return db.Database.SqlQuery<SP_InforServicePackage>("exec GetListImgInvoice_byCus @TextSearch, @ID_Customer,  @CurrentPage, @PageSize", sql.ToArray()).ToList();
        }

        /// <summary>
        /// Get list HD used service
        /// </summary>
        /// <param name="ID_DoiTuong"></param>
        /// <param name="ID_DonVi"></param>
        /// <returns></returns>
        public List<SP_InforServicePackage> SP_GetListHoaDon_UseService(Guid ID_DoiTuong, Guid ID_DonVi)
        {
            try
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_DoiTuong", ID_DoiTuong));
                paramlist.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                var data = db.Database.SqlQuery<SP_InforServicePackage>("EXEC GetListHoaDon_UseService @ID_DoiTuong, @ID_DonVi", paramlist.ToArray()).ToList();
                return data;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("GetListHoaDon_UseService: " + ex.InnerException + ex.Message);
                return null;
            }
        }
        #endregion

        public List<BH_HoaDon_ChiTietDTO> GetChiTietHD_byIDHoaDonLT(Guid id, Guid iddonvi)
        {
            var _ClassDVQD = new classDonViQuiDoi(db);
            SqlParameter param = new SqlParameter("ID_HoaDon", id);
            List<BH_HoaDon_ChiTietDTO> data = db.Database.SqlQuery<BH_HoaDon_ChiTietDTO>("EXEC GetChiTietHoaDon_ByIDHoaDon @ID_HoaDon", param).ToList();
            data = data.Select(p => new BH_HoaDon_ChiTietDTO()
            {
                ID = p.ID,
                ID_HoaDon = p.ID_HoaDon,
                DonGia = p.DonGia,
                GiaVon = p.GiaVon ?? 0,
                SoLuong = p.SoLuong,
                SoLuongConLai = p.SoLuongConLai,// used to check nhapmua from PO
                SoThuTu = p.SoThuTu,
                ThanhTien = p.ThanhTien,
                TienChietKhau = p.GiamGia,
                ThanhToan = p.ThanhToan,
                TienThue = p.TienThue,
                ID_DonViQuiDoi = p.ID_DonViQuiDoi,
                ID_HangHoa = p.ID_HangHoa,
                ID_ChiTietGoiDV = p.ID_ChiTietGoiDV,// used to check nhapmua from PO
                ID_ChiTietDinhLuong = p.ID_ChiTietDinhLuong,
                TenDonViTinh = p.TenDonViTinh,
                MaHangHoa = p.MaHangHoa,
                GiamGia = p.GiamGia,
                PTChietKhau = p.PTChietKhau,
                PTThue = p.PTThue,
                GiaBan = p.DonGia - p.TienChietKhau,
                GiaBanHH = p.GiaBanHH,
                ThoiGian = p.ThoiGian,
                GhiChu = p.GhiChu,
                ID_KhuyenMai = p.ID_KhuyenMai, // use show/hide icon KMai in lstCTHD
                ThuocTinh_GiaTri = p.ThuocTinh_GiaTri,
                TenHangHoaFull = p.TenHangHoaFull,
                TenHangHoaThayThe = p.TenHangHoaThayThe,
                LaHangHoa = p.LaHangHoa,
                QuanLyTheoLoHang = p.QuanLyTheoLoHang,
                TenHangHoa = p.TenHangHoa,
                TyLeChuyenDoi = p.TyLeChuyenDoi,
                YeuCau = p.YeuCau,
                ID_LoHang = p.ID_LoHang,
                MaLoHang = p.MaLoHang,
                TonKho = p.TonKho,
                TonLuyKe = p.TonLuyKe,
                ID_DonVi = p.ID_DonVi,
                GiaNhap = p.GiaNhap,
                GiaBanMaVach = p.GiaBanMaVach,
                LaDonViChuan = p.LaDonViChuan,
                ID_NhomHangHoa = p.ID_NhomHangHoa,
                QuyCach = p.QuyCach,
                PhiDichVu = p.PhiDichVu,
                LaPTPhiDichVu = p.LaPTPhiDichVu,
                NgayHetHan = p.NgayHetHan,
                NgaySanXuat = p.NgaySanXuat,
                DuocTichDiem = p.DuocTichDiem,
                DichVuTheoGio = p.DichVuTheoGio,
                ID_LichBaoDuong = p.ID_LichBaoDuong,
                LoaiHangHoa = p.LoaiHangHoa,
                HoaHongTruocChietKhau = p.HoaHongTruocChietKhau,
                ChietKhauMD_NV = p.ChietKhauMD_NV,
                ChietKhauMD_NVTheoPT = p.ChietKhauMD_NVTheoPT,
                DonViTinh = _ClassDVQD.Gets(ct => ct.ID_HangHoa == p.ID_HangHoa && ct.Xoa != true).Select(x => new DonViTinh
                {
                    ID_HangHoa = p.ID_HangHoa,
                    TenDonViTinh = x.TenDonViTinh,
                    ID_DonViQuiDoi = x.ID,
                    QuanLyTheoLoHang = p.QuanLyTheoLoHang,
                    Xoa = false,
                    TyLeChuyenDoi = x.TyLeChuyenDoi
                }).ToList(),
            }).ToList();
            return data;
        }

        public double? TinhTonTheoLoHang(Guid idhanghoa, Guid id_lohang, Guid id_donvi)
        {
            List<SqlParameter> paramlist = new List<SqlParameter>();
            DateTime timeEnd = DateTime.Now;
            Guid ID_ChiNhanh = id_donvi;
            Guid ID_HangHoa = idhanghoa;
            Guid ID_LoHang = id_lohang;

            paramlist.Add(new SqlParameter("timeEnd", timeEnd));
            paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
            paramlist.Add(new SqlParameter("ID_LoHang", ID_LoHang));
            paramlist.Add(new SqlParameter("ID_HangHoa", ID_HangHoa));
            List<DM_HangHoaDTO> listTon = db.Database.SqlQuery<DM_HangHoaDTO>("exec TinhTonTheoLoHangHoa @timeEnd, @ID_ChiNhanh,@ID_LoHang, @ID_HangHoa", paramlist.ToArray()).ToList();
            return listTon.Count > 0 ? listTon.FirstOrDefault().TonKho : 0;
        }

        public double? TinhTonTheoLoHangKK(Guid idhanghoa, Guid id_lohang, Guid id_donvi, DateTime timeKK)
        {
            List<SqlParameter> paramlist = new List<SqlParameter>();
            DateTime timeEnd = timeKK.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
            Guid ID_ChiNhanh = id_donvi;
            Guid ID_HangHoa = idhanghoa;
            Guid ID_LoHang = id_lohang;

            paramlist.Add(new SqlParameter("timeEnd", timeEnd));
            paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
            paramlist.Add(new SqlParameter("ID_LoHang", ID_LoHang));
            paramlist.Add(new SqlParameter("ID_HangHoa", ID_HangHoa));
            List<DM_HangHoaDTO> listTon = db.Database.SqlQuery<DM_HangHoaDTO>("exec TinhTonTheoLoHangHoa @timeEnd, @ID_ChiNhanh,@ID_LoHang, @ID_HangHoa", paramlist.ToArray()).ToList();
            return listTon.Count > 0 ? listTon.FirstOrDefault().TonKho : 0;
        }

        public List<BH_HoaDon_ChiTietDTO> GetChiTietHD_byIDHoaDonLoadChiTiet(Guid idHoaDon, int currentpage, int pageSize)
        {
            List<BH_HoaDon_ChiTietDTO> lstReturns = new List<BH_HoaDon_ChiTietDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var data = from hd in db.BH_HoaDon
                           join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                           from hd_dv in HD_DV.DefaultIfEmpty()
                           join cthd in db.BH_HoaDon_ChiTiet on hd.ID equals cthd.ID_HoaDon
                           join dvqd in db.DonViQuiDois on cthd.ID_DonViQuiDoi equals dvqd.ID
                           join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                           join dmlo in db.DM_LoHang on cthd.ID_LoHang equals dmlo.ID into DMLO
                           from lohang in DMLO.DefaultIfEmpty()
                           where cthd.ID_HoaDon == idHoaDon
                           orderby hd.NgayLapHoaDon descending
                           select new BH_HoaDon_ChiTietDTO
                           {
                               TenDonVi = hd_dv.TenDonVi,
                               ID_DonVi = hd_dv.ID,
                               ID = cthd.ID,
                               ID_HoaDon = hd.ID,
                               YeuCau = hd.YeuCau,
                               DonGia = cthd.DonGia,
                               GiaVon = cthd.GiaVon,
                               SoLuong = cthd.SoLuong,
                               ThanhTien = cthd.ThanhTien,
                               ID_DonViQuiDoi = cthd.ID_DonViQuiDoi,
                               ID_HangHoa = hh.ID,
                               TenDonViTinh = dvqd.TenDonViTinh,
                               MaHangHoa = dvqd.MaHangHoa,
                               TenHangHoa = hh.TenHangHoa,
                               GiamGia = cthd.TienChietKhau,
                               PTChietKhau = cthd.PTChietKhau,
                               TienChietKhau = cthd.TienChietKhau,
                               ThoiGian = cthd.ThoiGian,
                               GhiChu = cthd.GhiChu,
                               ThanhToan = cthd.ThanhToan,
                               TyLeChuyenDoi = dvqd.TyLeChuyenDoi,
                               LaHangHoa = hh.LaHangHoa,
                               ID_KhuyenMai = cthd.ID_KhuyenMai,
                               SoThuTu = cthd.SoThuTu,// load at MauIn
                               MaLoHang = (lohang == null ? "" : lohang.MaLoHang),
                               ID_LoHang = (lohang == null ? (Guid?)null : lohang.ID),
                           };

                var classhoadon = new ClassBH_HoaDon(db);
                foreach (var item in data.OrderByDescending(p => p.SoThuTu).Skip(currentpage * pageSize).Take(pageSize))
                {
                    BH_HoaDon_ChiTietDTO dto = new BH_HoaDon_ChiTietDTO();
                    dto.ID = item.ID;
                    dto.TenDonVi = item.TenDonVi;
                    dto.ID_HoaDon = item.ID_HoaDon;
                    dto.YeuCau = item.YeuCau;
                    dto.DonGia = Math.Round(item.DonGia, MidpointRounding.ToEven);
                    dto.GiaVon = item.GiaVon;
                    dto.SoLuong = item.SoLuong;
                    dto.SoThuTu = item.SoThuTu;
                    dto.ThanhTien = item.ThanhTien;
                    dto.TienChietKhau = item.GiamGia;
                    dto.ThanhToan = item.ThanhToan;
                    dto.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                    dto.ID_HangHoa = item.ID_HangHoa;
                    dto.TenDonViTinh = item.TenDonViTinh;
                    dto.MaHangHoa = item.MaHangHoa;
                    dto.GiamGia = item.GiamGia;
                    dto.PTChietKhau = item.PTChietKhau;
                    dto.GiaBan = item.DonGia - item.TienChietKhau;
                    dto.ThoiGian = item.ThoiGian;
                    dto.GhiChu = item.GhiChu;
                    dto.TenHangHoa = item.TenHangHoa;
                    dto.LaHangHoa = item.LaHangHoa;
                    dto.MaLoHang = item.MaLoHang;
                    dto.ID_LoHang = item.ID_LoHang;
                    dto.SoThuTu = item.SoThuTu;
                    dto.ID_KhuyenMai = item.ID_KhuyenMai; // use show/hide icon KMai in lstCTHD
                    dto.TonKho = Math.Round(classhoadon.TinhSLTonHH(item.ID_HangHoa, item.ID_DonVi.Value).Value / item.TyLeChuyenDoi, 3, MidpointRounding.ToEven);
                    dto.HangHoa_ThuocTinh = SelectHangHoa_ThuocTinh(item.ID_HangHoa).Select(x => new HangHoa_ThuocTinh
                    {
                        GiaTri = x.GiaTri,
                        ThuTuNhap = x.ThuTuNhap
                    }).OrderBy(p => p.ThuTuNhap).ToList();
                    lstReturns.Add(dto);
                }
                if (lstReturns.Count > 0)
                {
                    return lstReturns.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<BH_HoaDon_ChiTietDTO> GetChiTietHD_byIDHoaDonPageCount(Guid idHoaDon)
        {
            List<BH_HoaDon_ChiTietDTO> lstReturns = new List<BH_HoaDon_ChiTietDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var data = from hd in db.BH_HoaDon
                           join cthd in db.BH_HoaDon_ChiTiet on hd.ID equals cthd.ID_HoaDon
                           where cthd.ID_HoaDon == idHoaDon
                           select new BH_HoaDon_ChiTietDTO
                           {
                               ID = cthd.ID
                           };

                if (data == null)
                {
                    return lstReturns;
                }
                else
                {
                    return data.ToList();
                }
            }
        }

        //public static string GenerateBacode(string _data, string _filename)
        //{
        //    var filenameImage = Guid.NewGuid().ToString() + ".jpg";
        //    System.Drawing.Image barcodeImage = null;
        //    Barcode bc = new Barcode();
        //    barcodeImage = bc.Encode(TYPE.CODE39, _data);
        //    //barcodeImage.Save(Path.Combine(HttpContext.Current.Server.MapPath("~/imageHH/" + _data + ".jpg")), System.Drawing.Imaging.ImageFormat.Jpeg);
        //    //var ms = new MemoryStream();
        //    //barcodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        //    byte[] bytes = (byte[])(new System.Drawing.ImageConverter()).ConvertTo(barcodeImage, typeof(byte[]));
        //    var base64 = Convert.ToBase64String(bytes);
        //    var imgSrc = String.Format("data:image/gif;base64,{0}", base64);
        //    return imgSrc;
        //}
        public List<HangHoa_ThuocTinh> SelectHangHoa_ThuocTinh(Guid id)
        {
            List<HangHoa_ThuocTinh> lst = new List<HangHoa_ThuocTinh>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from tthh in db.HangHoa_ThuocTinh
                          where tthh.ID_HangHoa == id
                          select new
                          {
                              tthh.ID,
                              tthh.ID_HangHoa,
                              tthh.ID_ThuocTinh,
                              tthh.GiaTri,
                              tthh.ThuTuNhap
                          };
                foreach (var item in tbl)
                {
                    HangHoa_ThuocTinh objHH = new HangHoa_ThuocTinh();
                    objHH.ID = item.ID;
                    objHH.ID_HangHoa = item.ID_HangHoa;
                    objHH.ID_ThuocTinh = item.ID_ThuocTinh;
                    objHH.GiaTri = item.GiaTri;
                    objHH.ThuTuNhap = item.ThuTuNhap;
                    lst.Add(objHH);
                }
                return lst;
            }
        }

        public List<BH_HoaDon_ChiTietDTO> GetChiTietHD_byIDHoaDonChuyenHang(Guid idHoaDon, int currentpage, int pageSize, Guid iddonvi)
        {
            List<BH_HoaDon_ChiTietDTO> lstReturns = new List<BH_HoaDon_ChiTietDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var data = from hd in db.BH_HoaDon
                           join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                           from hd_dv in HD_DV.DefaultIfEmpty()
                           join cthd in db.BH_HoaDon_ChiTiet on hd.ID equals cthd.ID_HoaDon
                           join dvqd in db.DonViQuiDois on cthd.ID_DonViQuiDoi equals dvqd.ID
                           join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                           join dmlo in db.DM_LoHang on cthd.ID_LoHang equals dmlo.ID into DMLO
                           from lohang in DMLO.DefaultIfEmpty()
                           where cthd.ID_HoaDon == idHoaDon
                           orderby hd.NgayLapHoaDon descending
                           select new BH_HoaDon_ChiTietDTO
                           {
                               TenDonVi = hd_dv.TenDonVi,
                               ID_DonVi = hd_dv.ID,
                               ID = cthd.ID,
                               ID_HoaDon = hd.ID,
                               DonGia = cthd.DonGia,
                               GiaVon = cthd.GiaVon,
                               SoLuong = cthd.SoLuong,
                               ThanhTien = cthd.ThanhTien,
                               ID_DonViQuiDoi = cthd.ID_DonViQuiDoi,
                               ID_HangHoa = hh.ID,
                               TenDonViTinh = dvqd.TenDonViTinh,
                               MaHangHoa = dvqd.MaHangHoa,
                               TenHangHoa = hh.TenHangHoa,
                               GiamGia = cthd.TienChietKhau,
                               YeuCau = hd.YeuCau,
                               PTChietKhau = cthd.PTChietKhau,
                               TienChietKhau = cthd.TienChietKhau,
                               ThoiGian = cthd.ThoiGian,
                               GhiChu = cthd.GhiChu,
                               ThanhToan = cthd.ThanhToan,
                               TyLeChuyenDoi = dvqd.TyLeChuyenDoi,
                               LaHangHoa = hh.LaHangHoa,
                               ID_KhuyenMai = cthd.ID_KhuyenMai,
                               SoThuTu = cthd.SoThuTu,// load at MauIn
                               MaLoHang = lohang.MaLoHang,
                               ID_LoHang = lohang.ID,
                               QuanLyTheoLoHang = hh.QuanLyTheoLoHang
                           };

                foreach (var item in data.OrderByDescending(p => p.SoThuTu).Skip(currentpage * pageSize).Take(pageSize))
                {
                    BH_HoaDon_ChiTietDTO dto = new BH_HoaDon_ChiTietDTO();
                    dto.ID = item.ID;
                    dto.TenDonVi = item.TenDonVi;
                    dto.ID_HoaDon = item.ID_HoaDon;
                    dto.DonGia = Math.Round(item.DonGia, MidpointRounding.ToEven);
                    dto.GiaVon = Math.Round(item.GiaVon.Value, MidpointRounding.ToEven);
                    dto.SoLuong = item.SoLuong;
                    dto.SoThuTu = item.SoThuTu;
                    dto.ThanhTien = item.ThanhTien;
                    dto.TienChietKhau = item.GiamGia;
                    dto.ThanhToan = Math.Round(item.ThanhToan, MidpointRounding.ToEven);
                    dto.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                    dto.TenDonViTinh = item.TenDonViTinh;
                    dto.MaHangHoa = item.MaHangHoa;
                    dto.YeuCau = item.YeuCau;
                    dto.GiamGia = item.GiamGia;
                    dto.PTChietKhau = item.PTChietKhau;
                    dto.GiaBan = item.DonGia - item.TienChietKhau;
                    dto.ThoiGian = item.ThoiGian;
                    dto.GhiChu = item.GhiChu;
                    dto.TenHangHoa = item.TenHangHoa;
                    dto.LaHangHoa = item.LaHangHoa;
                    dto.MaLoHang = item.MaLoHang;
                    dto.ID_LoHang = item.ID_LoHang;
                    dto.QuanLyTheoLoHang = item.QuanLyTheoLoHang;
                    dto.ID_KhuyenMai = item.ID_KhuyenMai; // use show/hide icon KMai in lstCTHD
                    dto.TonKho = Math.Round(TinhSLTonHH(item.ID_HangHoa, iddonvi).Value / item.TyLeChuyenDoi, 3, MidpointRounding.ToEven);
                    dto.HangHoa_ThuocTinh = SelectHangHoa_ThuocTinh(item.ID_HangHoa).Select(x => new HangHoa_ThuocTinh
                    {
                        GiaTri = x.GiaTri,
                        ThuTuNhap = x.ThuTuNhap
                    }).OrderBy(p => p.ThuTuNhap).ToList();
                    lstReturns.Add(dto);
                }
                if (lstReturns.Count > 0)
                {
                    return lstReturns.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<BH_HoaDon_ChiTietDTO> GetChiTietHD_byIDHoaDonChuyenHangThaoTac(Guid idHoaDon, Guid iddonvi)
        {
            List<BH_HoaDon_ChiTietDTO> lstReturns = new List<BH_HoaDon_ChiTietDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var data = from hd in db.BH_HoaDon
                           join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                           from hd_dv in HD_DV.DefaultIfEmpty()
                           join cthd in db.BH_HoaDon_ChiTiet on hd.ID equals cthd.ID_HoaDon
                           join dvqd in db.DonViQuiDois on cthd.ID_DonViQuiDoi equals dvqd.ID
                           join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                           join dmlo in db.DM_LoHang on cthd.ID_LoHang equals dmlo.ID into DMLO
                           from lohang in DMLO.DefaultIfEmpty()
                           where cthd.ID_HoaDon == idHoaDon
                           orderby hd.NgayLapHoaDon descending
                           select new BH_HoaDon_ChiTietDTO
                           {
                               TenDonVi = hd_dv.TenDonVi,
                               ID_DonVi = hd_dv.ID,
                               ID = cthd.ID,
                               ID_HoaDon = hd.ID,
                               DonGia = cthd.DonGia,
                               GiaVon = cthd.GiaVon,
                               SoLuong = cthd.SoLuong,
                               ThanhTien = cthd.ThanhTien,
                               ID_DonViQuiDoi = cthd.ID_DonViQuiDoi,
                               ID_HangHoa = hh.ID,
                               TenDonViTinh = dvqd.TenDonViTinh,
                               MaHangHoa = dvqd.MaHangHoa,
                               TenHangHoa = hh.TenHangHoa,
                               GiamGia = cthd.TienChietKhau,
                               YeuCau = hd.YeuCau,
                               PTChietKhau = cthd.PTChietKhau,
                               TienChietKhau = cthd.TienChietKhau,
                               ThoiGian = cthd.ThoiGian,
                               GhiChu = cthd.GhiChu,
                               ThanhToan = cthd.ThanhToan,
                               TyLeChuyenDoi = dvqd.TyLeChuyenDoi,
                               LaHangHoa = hh.LaHangHoa,
                               ID_KhuyenMai = cthd.ID_KhuyenMai,
                               SoThuTu = cthd.SoThuTu,// load at MauIn
                               MaLoHang = lohang.MaLoHang,
                               ID_LoHang = lohang.ID,
                               QuanLyTheoLoHang = hh.QuanLyTheoLoHang
                           };

                foreach (var item in data)
                {
                    BH_HoaDon_ChiTietDTO dto = new BH_HoaDon_ChiTietDTO();
                    dto.ID = item.ID;
                    dto.TenDonVi = item.TenDonVi;
                    dto.ID_HoaDon = item.ID_HoaDon;
                    dto.DonGia = Math.Round(item.DonGia, MidpointRounding.ToEven);
                    dto.GiaVon = Math.Round(item.GiaVon.Value, MidpointRounding.ToEven);
                    dto.SoLuong = item.SoLuong;
                    dto.SoThuTu = item.SoThuTu;
                    dto.ThanhTien = item.ThanhTien;
                    dto.TienChietKhau = item.GiamGia;
                    dto.ThanhToan = item.ThanhToan;
                    dto.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                    dto.TenDonViTinh = item.TenDonViTinh;
                    dto.MaHangHoa = item.MaHangHoa;
                    dto.YeuCau = item.YeuCau;
                    dto.GiamGia = item.GiamGia;
                    dto.PTChietKhau = item.PTChietKhau;
                    dto.GiaBan = item.DonGia - item.TienChietKhau;
                    dto.ThoiGian = item.ThoiGian;
                    dto.GhiChu = item.GhiChu;
                    dto.TenHangHoa = item.TenHangHoa;
                    dto.LaHangHoa = item.LaHangHoa;
                    dto.MaLoHang = item.MaLoHang;
                    dto.ID_LoHang = item.ID_LoHang;
                    dto.QuanLyTheoLoHang = item.QuanLyTheoLoHang;
                    dto.ID_KhuyenMai = item.ID_KhuyenMai; // use show/hide icon KMai in lstCTHD
                    dto.TonKho = Math.Round(TinhSLTonHH(item.ID_HangHoa, iddonvi).Value / item.TyLeChuyenDoi, 3, MidpointRounding.ToEven);
                    dto.HangHoa_ThuocTinh = SelectHangHoa_ThuocTinh(item.ID_HangHoa).Select(x => new HangHoa_ThuocTinh
                    {
                        GiaTri = x.GiaTri,
                        ThuTuNhap = x.ThuTuNhap
                    }).OrderBy(p => p.ThuTuNhap).ToList();
                    lstReturns.Add(dto);
                }
                if (lstReturns.Count > 0)
                {
                    return lstReturns.OrderByDescending(p => p.SoThuTu).ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<BH_HoaDon_ChiTietDTO> GetPageCount_byIDHoaDonChuyenHang(Guid idHoaDon, Guid iddonvi)
        {
            List<BH_HoaDon_ChiTietDTO> lstReturns = new List<BH_HoaDon_ChiTietDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var data = from hd in db.BH_HoaDon
                           join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                           from hd_dv in HD_DV.DefaultIfEmpty()
                           join cthd in db.BH_HoaDon_ChiTiet on hd.ID equals cthd.ID_HoaDon
                           join dvqd in db.DonViQuiDois on cthd.ID_DonViQuiDoi equals dvqd.ID
                           join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                           where cthd.ID_HoaDon == idHoaDon
                           orderby hd.NgayLapHoaDon descending
                           select new BH_HoaDon_ChiTietDTO
                           {
                               ID = cthd.ID,
                           };

                if (data == null)
                {
                    return lstReturns;
                }
                else
                {
                    return data.ToList();
                }
            }
        }

        public double? TinhSLTonHH(Guid id_HangHoa, Guid iddonvi)
        {
            List<SqlParameter> paramlist = new List<SqlParameter>();
            DateTime timeEnd = DateTime.Now;
            Guid ID_ChiNhanh = iddonvi;
            Guid ID_HangHoa = id_HangHoa;

            paramlist.Add(new SqlParameter("timeEnd", timeEnd));
            paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
            paramlist.Add(new SqlParameter("ID_HangHoa", ID_HangHoa));
            List<DM_Ton> listTon = db.Database.SqlQuery<DM_Ton>("exec TinhSLTon @timeEnd, @ID_ChiNhanh,@ID_HangHoa", paramlist.ToArray()).ToList();
            return listTon.Count > 0 ? listTon.FirstOrDefault().TonKho : 0;
        }

        public double? TinhSLTonHHKK(Guid id_HangHoa, Guid iddonvi, DateTime timeKK)
        {
            List<SqlParameter> paramlist = new List<SqlParameter>();
            DateTime timeEnd = timeKK.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
            Guid ID_ChiNhanh = iddonvi;
            Guid ID_HangHoa = id_HangHoa;

            paramlist.Add(new SqlParameter("timeEnd", timeEnd));
            paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
            paramlist.Add(new SqlParameter("ID_HangHoa", ID_HangHoa));
            List<DM_Ton> listTon = db.Database.SqlQuery<DM_Ton>("exec TinhSLTon @timeEnd, @ID_ChiNhanh,@ID_HangHoa", paramlist.ToArray()).ToList();
            return listTon.Count > 0 ? listTon.FirstOrDefault().TonKho : 0;
        }

        public List<BH_HoaDonDTO> GetListHoaDons_Where(int loaiHoaDon, string maHoaDon,
           string id_ViTris, string id_BangGias, int trangThai, DateTime dayStart, DateTime dayEnd, string id_donvi, string arrChiNhanh)
        {
            List<BH_HoaDonDTO> lsrReturns = new List<BH_HoaDonDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    var tbl = from hd in db.BH_HoaDon
                              join qct in db.Quy_HoaDon_ChiTiet on hd.ID equals qct.ID_HoaDonLienQuan into QCT_HD
                              from qct_hd in QCT_HD.DefaultIfEmpty()

                              join qhd in db.Quy_HoaDon on qct_hd.ID_HoaDon equals qhd.ID into QUY
                              from quy in QUY.DefaultIfEmpty()

                              join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                              from hd_dt in HD_DT.DefaultIfEmpty()

                              join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                              from hd_dv in HD_DV.DefaultIfEmpty()

                              join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                              from hd_nv in HD_NV.DefaultIfEmpty()

                              join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                              from hd_vt in HD_VT.DefaultIfEmpty()

                              join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                              from hd_bg in HD_BG.DefaultIfEmpty()

                              where hd.LoaiHoaDon == loaiHoaDon

                              orderby hd.NgayLapHoaDon descending, hd.MaHoaDon descending

                              select new
                              {
                                  ID = hd.ID,
                                  ID_HoaDon = hd.ID_HoaDon,
                                  MaHoaDon = hd.MaHoaDon,
                                  TenDonVi = hd_dv.TenDonVi,
                                  ID_DonVi = hd_dv.ID,
                                  DiaChiChiNhanh = hd_dv.DiaChi,
                                  NgayLapHoaDon = hd.NgayLapHoaDon,
                                  TongGiamGia = hd.TongGiamGia,
                                  TongTienHang = hd.TongTienHang,
                                  PhaiThanhToan = hd.PhaiThanhToan,
                                  TongChiPhi = hd.TongChiPhi,
                                  ID_NhanVien = hd.ID_NhanVien,
                                  TenNhanVien = hd_nv.TenNhanVien,
                                  TenDoiTuong = hd_dt != null ? hd_dt.TenDoiTuong : "Khách lẻ",
                                  DienGiai = hd.DienGiai,
                                  Email = hd_dt.Email,
                                  PhuongXa = hd_dt.DM_QuanHuyen != null ? hd_dt.DM_QuanHuyen.TenQuanHuyen : "",
                                  KhuVuc = hd_dt.DM_TinhThanh != null ? hd_dt.DM_TinhThanh.TenTinhThanh : "",
                                  DiaChiKhachHang = hd_dt.DiaChi,
                                  DienThoai = hd_dt.DienThoai,
                                  ID_ViTri = hd.ID_ViTri,
                                  TenPhongBan = hd_vt != null ? hd_vt.TinhTrang == true ? "" : hd_vt.TenViTri : "", // if PhongBan was deleted --> TenPhongBan = ""
                                  NguoiTaoHD = hd.NguoiTao,
                                  ID_BangGia = hd.ID_BangGia,
                                  TenBangGia = hd_bg != null ? hd_bg.TenGiaBan : "Bảng giá chung",
                                  ChoThanhToan = hd.ChoThanhToan,
                                  LoaiHoaDon = hd.LoaiHoaDon,
                                  // trạng thái lưu vào trường yêu cầu
                                  //YeuCau = hd.YeuCau == null ? "" : (hd.YeuCau == "1" ? "Phiếu tạm" : (hd.YeuCau == "2" ? "Đang hủy" : (hd.YeuCau == "3" ? "Đã nhận" : (hd.YeuCau == "4" ? "Đã hủy" : "")))),
                                  YeuCau = hd.YeuCau ?? "",
                                  ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty,
                                  MaPhieuChi = quy.MaHoaDon ?? "",
                                  ID_PhieuChi = qct_hd.ID == null ? Guid.Empty : qct_hd.ID,
                                  //KhachDaTra = (double?)qct_hd.TienMat ?? 0 + (double?)qct_hd.TienGui ?? 0,
                                  KhachDaTra = (double?)quy.TongTienThu ?? 0,
                                  TienMat = (double?)qct_hd.TienMat ?? 0,
                                  TienGui = (double?)qct_hd.TienGui ?? 0,
                                  TongChietKhau = (double?)hd.TongChietKhau ?? 0, // PTGiam
                              };

                    var data = tbl.AsEnumerable().Select(hd => new BH_HoaDonDTO
                    {
                        ID = hd.ID,
                        ID_HoaDon = hd.ID_HoaDon,
                        MaHoaDon = hd.MaHoaDon,
                        TenDoiTuongUnSign = CommonStatic.ConvertToUnSign(hd.TenDoiTuong).ToLower(),
                        TenDoiTuongStartChars = CommonStatic.GetCharsStart(hd.TenDoiTuong).ToLower(),
                        ID_DonVi = hd.ID_DonVi,
                        DiaChiChiNhanh = hd.DiaChiChiNhanh,
                        NgayLapHoaDon = hd.NgayLapHoaDon,
                        TongGiamGia = hd.TongGiamGia,
                        TongTienHang = hd.TongTienHang,
                        PhaiThanhToan = hd.PhaiThanhToan,
                        TongChiPhi = hd.TongChiPhi,
                        ID_NhanVien = hd.ID_NhanVien,
                        TenNhanVien = hd.TenNhanVien,
                        TenDoiTuong = hd.TenDoiTuong,
                        DienGiai = hd.DienGiai,
                        Email = hd.Email,
                        DienThoai = hd.DienThoai,
                        DiaChiKhachHang = hd.DiaChiKhachHang,
                        ID_ViTri = hd.ID_ViTri,
                        TenPhongBan = hd.TenPhongBan,
                        NguoiTaoHD = hd.NguoiTaoHD,
                        ID_BangGia = hd.ID_BangGia,
                        TenBangGia = hd.TenBangGia,
                        ChoThanhToan = hd.ChoThanhToan,
                        LoaiHoaDon = hd.LoaiHoaDon,
                        // trạng thái lưu vào trường yêu cầu
                        //YeuCau = hd.YeuCau == null ? "" : (hd.YeuCau == "1" ? "Phiếu tạm" : (hd.YeuCau == "2" ? "Đang hủy" : (hd.YeuCau == "3" ? "Đã nhận" : (hd.YeuCau == "4" ? "Đã hủy" : "")))),
                        YeuCau = hd.YeuCau,
                        ID_DoiTuong = hd.ID_DoiTuong,
                        MaPhieuChi = hd.MaPhieuChi,
                        ID_PhieuChi = hd.ID_PhieuChi,
                        KhachDaTra = hd.KhachDaTra,
                        TenDonVi = hd.TenDonVi,
                        TienMat = hd.TienMat,
                        TienATM = hd.TienGui,
                        TongChietKhau = hd.TongChietKhau,
                        //MaHoaDonGoc = ClassBH_HoaDon.JoinHoaDon_SoQuy_byIDHoaDon(hd.ID_HoaDon) != null ? ClassBH_HoaDon.JoinHoaDon_SoQuy_byIDHoaDon(hd.ID_HoaDon).MaHoaDonGoc : "",
                        //TongTienHDTra = ClassBH_HoaDon.JoinHoaDon_SoQuy_byIDHoaDon(hd.ID_HoaDon) != null ? ClassBH_HoaDon.JoinHoaDon_SoQuy_byIDHoaDon(hd.ID_HoaDon).TongTienHDTra : 0,
                    });

                    // get list ID_ViTri
                    List<string> lstIDViTri = new List<string>();
                    if (id_ViTris != "null")
                    {
                        var arrIDViTri = id_ViTris.Split(',');
                        for (int i = 0; i < arrIDViTri.Length; i++)
                        {
                            lstIDViTri.Add(arrIDViTri[i].ToString());
                        }
                    }

                    List<string> lstIDBangGia = new List<string>();
                    if (id_BangGias != "null")
                    {
                        // get list ID_BangGia
                        var arrIDBanGia = id_BangGias.Split(',');
                        for (int i = 0; i < arrIDBanGia.Length; i++)
                        {
                            lstIDBangGia.Add(arrIDBanGia[i].ToString());
                        }
                    }

                    List<Guid> lstIDCN = new List<Guid>();
                    if (arrChiNhanh != null)
                    {
                        var arrIDCN = arrChiNhanh.Split(',');
                        for (int i = 0; i < arrIDCN.Length; i++)
                        {
                            lstIDCN.Add(new Guid(arrIDCN[i]));
                        }
                    }
                    if (lstIDCN.Count > 0)
                    {
                        data = data.Where(p => p.ID_DonVi != null).Where(hd => lstIDCN.Contains(hd.ID_DonVi));
                    }
                    else
                    {
                        if (id_donvi != string.Empty && id_donvi != null)
                        {
                            data = data.Where(hd => hd.ID_DonVi.ToString().Contains(id_donvi));
                        }
                    }

                    if (lstIDViTri.Count > 0)
                    {
                        data = data.Where(hd => lstIDViTri.Contains(hd.ID_ViTri.ToString()));
                    }

                    if (lstIDBangGia.Count > 0)
                    {
                        data = data.Where(hd => lstIDBangGia.Contains(hd.ID_BangGia.ToString()));
                    }

                    if (dayStart == dayEnd)
                    {
                        data = data.Where(hd => hd.NgayLapHoaDon.Year == dayStart.Year
                        && hd.NgayLapHoaDon.Month == dayStart.Month
                        && hd.NgayLapHoaDon.Day == dayStart.Day);
                    }
                    else
                    {
                        data = data.Where(hd => hd.NgayLapHoaDon >= dayStart && hd.NgayLapHoaDon < dayEnd);
                    }

                    // trang thai HoaDon
                    switch (trangThai)
                    {
                        // DatHang: trang thai luu vao truong yeu cau
                        case 0: // Hoàn thành + Huy + Tam luu + GiaoHang (All)
                            break;
                        case 1: //  Huy + HoanThanh + TamLuu
                            if (loaiHoaDon == 3)
                            {
                                data = data.Where(hd => hd.YeuCau != "2");
                            }
                            break;
                        case 2: // Huy + HoanThanh + GiaoHang
                            if (loaiHoaDon != 3)
                            {
                                data = data.Where(hd => hd.ChoThanhToan != true);
                            }
                            else
                            {
                                data = data.Where(hd => hd.YeuCau != "1");
                            }
                            break;
                        case 3: // Huy + HoanThanh
                            if (loaiHoaDon != 3)
                            {
                                data = data.Where(hd => hd.ChoThanhToan != false);
                            }
                            else
                            {
                                data = data.Where(hd => hd.YeuCau != "1" || hd.YeuCau != "2");
                            }
                            break;
                        case 4: // Huy + TamLuu + GiaoHang
                            if (loaiHoaDon != 3)
                            {
                                data = data.Where(hd => hd.ChoThanhToan != false);
                            }
                            else
                            {
                                data = data.Where(hd => hd.YeuCau != "3");
                            }
                            break;
                        case 5: // Huy + TamLuu
                            if (loaiHoaDon != 3)
                            {
                                data = data.Where(hd => hd.ChoThanhToan != false);
                            }
                            else
                            {
                                data = data.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "4");
                            }
                            break;
                        case 6: // Huy + GiaoHang
                            if (loaiHoaDon != 3)
                            {
                                data = data.Where(hd => hd.ChoThanhToan == null);
                            }
                            else
                            {
                                data = data.Where(hd => hd.YeuCau == "2" || hd.YeuCau == "4");
                            }
                            break;
                        case 7: // Huy
                            if (loaiHoaDon != 3)
                            {
                                data = data.Where(hd => hd.ChoThanhToan == null);
                            }
                            else
                            {
                                data = data.Where(hd => hd.YeuCau == "4");
                            }
                            break;
                        case 8: // HoanThanh +TamLuu + GiaoHang
                            if (loaiHoaDon != 3)
                            {
                                data = data.Where(hd => hd.ChoThanhToan != null);
                            }
                            else
                            {
                                data = data.Where(hd => hd.YeuCau != "4");
                            }
                            break;
                        case 9: // HoanThanh +TamLuu
                            if (loaiHoaDon != 3)
                            {
                                data = data.Where(hd => hd.ChoThanhToan != null);
                            }
                            else
                            {
                                data = data.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "3");
                            }
                            break;
                        case 10: // HoanThanh + GiaoHang
                            if (loaiHoaDon != 3)
                            {
                                data = data.Where(hd => hd.ChoThanhToan == false);
                            }
                            else
                            {
                                data = data.Where(hd => hd.YeuCau == "2" || hd.YeuCau == "3");
                            }
                            break;
                        case 11: // HoanThanh
                            if (loaiHoaDon != 3)
                            {
                                data = data.Where(hd => hd.ChoThanhToan == false);
                            }
                            else
                            {
                                data = data.Where(hd => hd.YeuCau == "3");
                            }
                            break;
                        case 12: // TamLuu + GiaoHang
                            if (loaiHoaDon != 3)
                            {
                                data = data.Where(hd => hd.ChoThanhToan == true);
                            }
                            else
                            {
                                data = data.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "2");
                            }
                            break;
                        case 13: // TamLuu
                            if (loaiHoaDon != 3)
                            {
                                data = data.Where(hd => hd.ChoThanhToan == true);
                            }
                            else
                            {
                                data = data.Where(hd => hd.YeuCau == "1");
                            }
                            break;
                        case 14: // GiaoHang
                            if (loaiHoaDon == 3)
                            {
                                data = data.Where(hd => hd.YeuCau == "2");
                            }
                            break;
                        default:
                            data = null;
                            break;
                    }

                    var maHDunsingn = CommonStatic.ConvertToUnSign(maHoaDon).ToLower();
                    if (maHoaDon != string.Empty && maHoaDon != null)
                    {
                        data = data.Where(hd => hd.MaHoaDon.ToLower().Contains(@maHDunsingn)
                        || hd.TenDoiTuongUnSign.Contains(@maHDunsingn) || hd.TenDoiTuongStartChars.Contains(@maHDunsingn) ||
                        (hd.DienThoai != null && hd.DienThoai.Contains(@maHDunsingn)));
                    }

                    // maHDGoc
                    //if (maHDGoc != string.Empty && maHDGoc != null)
                    //{
                    //    data = data.Where(x => x.MaHoaDonGoc.Contains(maHDGoc));
                    //}
                    return data.ToList();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("GetListHoaDons_Where: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }

        public List<BH_NhatKySDTheDTO> GetListNhatKySDThe(ModelNhatKySDThe model)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_DoiTuong", model.iddt));
            lstParam.Add(new SqlParameter("FromDate", model.dayStart));
            lstParam.Add(new SqlParameter("ToDate", model.dayEnd));
            lstParam.Add(new SqlParameter("CurrentPage", model.currentPage));
            lstParam.Add(new SqlParameter("PageSize", model.pageSize));
            return db.Database.SqlQuery<BH_NhatKySDTheDTO>("EXEC GetListSuDungThe @ID_DoiTuong, @FromDate, @ToDate, @CurrentPage, @PageSize", lstParam.ToArray()).ToList();
        }

        public List<BH_HoaDonDTO> GetList_HoaDonNhapHang(ModelHoaDon model)
        {
            var isChiNhanhs = string.Join(",", model.arrChiNhanh);
            var columnSort = "NgayLapHoaDon";
            var sortBy = "DESC";
            var trangthais = "0,1,2";
            var mahoadon = string.Empty;
            var loaiHoaDons = "7";
            if (model.columsort != null && model.columsort != string.Empty)
            {
                columnSort = model.columsort;
            }
            if (model.sort != null && model.sort != string.Empty)
            {
                sortBy = model.sort;
            }
            if (model.maHoaDon != null && model.maHoaDon != string.Empty)
            {
                mahoadon = model.maHoaDon;
            }
            if (model.ArrTrangThai != null && model.ArrTrangThai.Count > 0)
            {
                trangthais = string.Join(",", model.ArrTrangThai);
            }
            if (model.ArrLoaiHoaDon != null && model.ArrLoaiHoaDon.Count > 0)
            {
                loaiHoaDons = string.Join(",", model.ArrLoaiHoaDon);
            }
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("TextSearch", mahoadon));
            lstParam.Add(new SqlParameter("LoaiHoaDon", loaiHoaDons));
            lstParam.Add(new SqlParameter("IDChiNhanhs", isChiNhanhs));
            lstParam.Add(new SqlParameter("FromDate", model.dayStart));
            lstParam.Add(new SqlParameter("ToDate", model.dayEnd));
            lstParam.Add(new SqlParameter("TrangThais", trangthais));
            lstParam.Add(new SqlParameter("CurrentPage", model.currentPage));
            lstParam.Add(new SqlParameter("PageSize", model.pageSize));
            lstParam.Add(new SqlParameter("ColumnSort", columnSort));
            lstParam.Add(new SqlParameter("SortBy", sortBy));
            return db.Database.SqlQuery<BH_HoaDonDTO>("EXEC GetList_HoaDonNhapHang @TextSearch, @LoaiHoaDon, @IDChiNhanhs, @FromDate, @ToDate," +
                "@TrangThais, @CurrentPage, @PageSize, @ColumnSort, @SortBy", lstParam.ToArray()).ToList();
        }

        public static readonly string[] VietnameseSigns = new string[]

         { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            "í","ì","ỉ","ĩ","ị",
            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            "ý","ỳ","ỷ","ỹ","ỵ",};
        public List<BH_HoaDonDTO> SP_GetListHoaDons_Where(int loaiHoaDon, string maHoaDon, string maHDGoc,
         string id_ViTris, string id_BangGias, int trangThai, DateTime dayStart, DateTime dayEnd, string id_donvi, string arrChiNhanh, string columsort,
         string sort, string ptThanhToan = null, string maHangHoa = null)
        {

            List<SqlParameter> lstParam = new List<SqlParameter>();
            string maHDSearch = string.Empty;
            if (maHoaDon == string.Empty || maHoaDon == null)
            {
                maHDSearch = "%%";
            }
            else
            {
                maHDSearch = "%" + CommonStatic.ConvertToUnSign(maHoaDon).ToLower() + "%";
            }

            string maHDGocSearch = string.Empty;
            if (maHDGoc == string.Empty || maHDGoc == null)
            {
                maHDGocSearch = "%%";
            }
            else
            {
                maHDGocSearch = "%" + CommonStatic.ConvertToUnSign(maHDGoc).ToLower() + "%";
            }

            string sIDChiNhanhs = string.Empty;
            if (arrChiNhanh != null && arrChiNhanh != string.Empty)
            {
                sIDChiNhanhs = arrChiNhanh;
            }

            lstParam.Add(new SqlParameter("timeStart", dayStart));
            lstParam.Add(new SqlParameter("timeEnd", dayEnd));
            lstParam.Add(new SqlParameter("ID_ChiNhanh", sIDChiNhanhs));

            List<BH_HoaDonDTO> data = null;
            try
            {
                switch (loaiHoaDon)
                {
                    case 1:
                        lstParam.Add(new SqlParameter("MaHD", "%%"));
                        data = db.Database.SqlQuery<BH_HoaDonDTO>("EXEC getlist_HoaDonBanHang_FindMaHang @timeStart, @timeEnd, @ID_ChiNhanh, @MaHD", lstParam.ToArray()).ToList();
                        break;
                    case 3:
                        lstParam.Add(new SqlParameter("MaHD", "%%"));
                        data = db.Database.SqlQuery<BH_HoaDonDTO>("EXEC getlist_HoaDonDatHang_FindMaHang @timeStart, @timeEnd, @ID_ChiNhanh, @MaHD", lstParam.ToArray()).ToList();
                        break;
                    case 6:
                        lstParam.Add(new SqlParameter("MaHD", maHDGocSearch));
                        lstParam.Add(new SqlParameter("MaPT", maHDSearch));
                        lstParam.Add(new SqlParameter("TrangThai", "%%"));
                        // gộp chung các điều kiện tìm kiếm: MaHDGoc, MaHDTra, MaHangHoa, TenHangHoa vào chung 1 trường @MaPT
                        data = db.Database.SqlQuery<BH_HoaDonDTO>("EXEC getlist_HoaDonTraHang_FindMaHang @timeStart, @timeEnd, @ID_ChiNhanh, @MaPT, @MaHD, @TrangThai", lstParam.ToArray()).ToList();
                        break;
                    case 19: // goiDV
                        lstParam.Add(new SqlParameter("MaHD", "%%"));
                        data = db.Database.SqlQuery<BH_HoaDonDTO>("EXEC GetList_GoiDichVu_Where @timeStart, @timeEnd, @ID_ChiNhanh, @MaHD", lstParam.ToArray()).ToList();
                        break;
                    default:
                        break;
                }
                if (loaiHoaDon != 6)
                {
                    if (!string.IsNullOrWhiteSpace(maHoaDon))
                    {
                        char[] whitespace = new char[] { ' ', '\t' };
                        string[] textFilter = maHoaDon.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Split(whitespace);
                        string[] utf8 = textFilter.Where(o => o.Any(c => VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                        string[] utf = textFilter.Where(o => !o.Any(c => VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                        var maHDunsingn = CommonStatic.ConvertToUnSign(maHoaDon).ToLower().Trim();

                        data = data.Where(o =>
                        o.MaHoaDon.ToLower().Contains(@maHDunsingn)
                        || (o.DienThoai != null && o.DienThoai.Contains(@maHDunsingn)) || (o.TenDoiTuong.Contains(maHoaDon.Trim()))
                        || (utf8.All(c => o.TenDoiTuong.ToLower().Contains(c))
                        && utf.All(d => CommonStatic.ConvertToUnSign(o.TenDoiTuong).ToLower().Contains(d) || o.MaHoaDon.ToLower().Contains(d)))
                        || o.HoaDon_HangHoa.ToLower().Contains(@maHDunsingn)// find Ma/TenHang                
                        ).ToList();

                        //data = data.Where(o => o.MaHoaDon.ToLower().Contains(@maHDunsingn)).ToList();
                    }
                }
                // get list ID_ViTri
                List<string> lstIDViTri = new List<string>();
                if (id_ViTris != "null")
                {
                    var arrIDViTri = id_ViTris.Split(',');
                    for (int i = 0; i < arrIDViTri.Length; i++)
                    {
                        lstIDViTri.Add(arrIDViTri[i].ToString());
                    }
                }

                List<string> lstIDBangGia = new List<string>();
                if (id_BangGias != "null")
                {
                    // get list ID_BangGia
                    var arrIDBanGia = id_BangGias.Split(',');
                    for (int i = 0; i < arrIDBanGia.Length; i++)
                    {
                        lstIDBangGia.Add(arrIDBanGia[i].ToString());
                    }
                }

                if (lstIDViTri.Count > 0)
                {
                    data = data.Where(hd => lstIDViTri.Contains(hd.ID_ViTri.ToString())).ToList();
                }

                if (lstIDBangGia.Count > 0)
                {
                    // if search BgiaChung
                    if (lstIDBangGia.Count == 1 && lstIDBangGia.IndexOf("00000000-0000-0000-0000-000000000000") > -1)
                    {
                        data = data.Where(hd => hd.ID_BangGia == Guid.Empty).ToList();
                    }
                    else
                    {
                        if (lstIDBangGia.IndexOf("00000000-0000-0000-0000-000000000000") > -1)
                        {
                            data = data.Where(hd => lstIDBangGia.Contains(hd.ID_BangGia.ToString()) || hd.ID_BangGia == Guid.Empty).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => lstIDBangGia.Contains(hd.ID_BangGia.ToString())).ToList();
                        }
                    }
                }

                switch (trangThai)
                {
                    // DatHang: trang thai luu vao truong yeu cau
                    case 0: // Hoàn thành + Huy + Tam luu + GiaoHang (All)
                        break;
                    case 1: //  Huy + HoanThanh + TamLuu
                        if (loaiHoaDon == 3)
                        {
                            data = data.Where(hd => hd.YeuCau != "2").ToList();
                        }
                        break;
                    case 2: // Huy + HoanThanh + GiaoHang
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan != true).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau != "1").ToList();
                        }
                        break;
                    case 3: // Huy + HoanThanh
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan != false).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau != "1" && hd.YeuCau != "2").ToList();
                        }
                        break;
                    case 4: // Huy + TamLuu + GiaoHang
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan != false).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau != "3").ToList();
                        }
                        break;
                    case 5: // Huy + TamLuu
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan != false).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "4").ToList();
                        }
                        break;
                    case 6: // Huy + GiaoHang
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan == null).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "2" || hd.YeuCau == "4").ToList();
                        }
                        break;
                    case 7: // Huy
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan == null).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "4").ToList();
                        }
                        break;
                    case 8: // HoanThanh +TamLuu + GiaoHang
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan != null).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau != "4").ToList();
                        }
                        break;
                    case 9: // HoanThanh +TamLuu
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan != null).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "3").ToList();
                        }
                        break;
                    case 10: // HoanThanh + GiaoHang
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan == false).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "2" || hd.YeuCau == "3").ToList();
                        }
                        break;
                    case 11: // HoanThanh
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan == false).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "3").ToList();
                        }
                        break;
                    case 12: // TamLuu + GiaoHang
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan == true).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "2").ToList();
                        }
                        break;
                    case 13: // TamLuu
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan == true).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "1").ToList();
                        }
                        break;
                    case 14: // GiaoHang
                        if (loaiHoaDon == 3)
                        {
                            data = data.Where(hd => hd.YeuCau == "2").ToList();
                        }
                        break;
                    default: // tam luu
                        data = null;
                        break;
                }

                // phuong thuc thanh toan: The, TinMat
                if (ptThanhToan != null)
                {
                    switch (ptThanhToan)
                    {
                        case "1":
                            data = data.Where(hd => hd.TienMat >= 0).ToList();
                            break;
                        case "2":
                            data = data.Where(hd => hd.ChuyenKhoan > 0).ToList();
                            break;
                        default:
                            break;
                    }
                }

                // ma hang hoa (todo)
                //if (maHangHoa != null)
                //{
                //    var cthd_dvqd = from cthd in db.BH_HoaDon_ChiTiet
                //                    join qd in db.DonViQuiDois on cthd.ID_DonViQuiDoi equals qd.ID
                //                    join hh in db.DM_HangHoa on qd.ID_HangHoa equals hh.ID
                //                    where CommonStatic.ConvertToUnSign(hh.TenHangHoa).Contains(CommonStatic.ConvertToUnSign(maHangHoa))
                //                     || CommonStatic.ConvertToUnSign(qd.MaHangHoa).Contains(CommonStatic.ConvertToUnSign(maHangHoa))
                //                    select new
                //                    {
                //                        ID_DonViQuiDoi = qd.ID
                //                    };
                //}

                if (sort != "null")
                {
                    if (sort == "0")
                    {
                        if (columsort == "MaHoaDon")
                        {
                            data = data.OrderBy(p => p.MaHoaDon).ToList();
                        }
                        if (columsort == "MaHoaDonGoc")
                        {
                            data = data.OrderBy(p => p.MaHoaDonGoc).ToList();
                        }
                        if (columsort == "ThoiGian")
                        {
                            data = data.OrderBy(p => p.NgayLapHoaDon).ToList();
                        }
                        if (columsort == "KhachHang")
                        {
                            data = data.OrderBy(p => p.TenDoiTuong).ToList();
                        }
                        if (columsort == "Email")
                        {
                            data = data.OrderBy(p => p.Email).ToList();
                        }
                        if (columsort == "SoDienThoai")
                        {
                            data = data.OrderBy(p => p.DienThoai).ToList();
                        }
                        if (columsort == "DiaChi")
                        {
                            data = data.OrderBy(p => p.DiaChiKhachHang).ToList();
                        }
                        if (columsort == "KhuVuc")
                        {
                            data = data.OrderBy(p => p.KhuVuc).ToList();
                        }
                        if (columsort == "PhuongXa")
                        {
                            data = data.OrderBy(p => p.PhuongXa).ToList();
                        }
                        if (columsort == "NguoiBan")
                        {
                            data = data.OrderBy(p => p.TenNhanVien).ToList();
                        }
                        if (columsort == "NguoiTao")
                        {
                            data = data.OrderBy(p => p.NguoiTaoHD).ToList();
                        }
                        if (columsort == "GhiChu")
                        {
                            data = data.OrderBy(p => p.DienGiai).ToList();
                        }
                        if (columsort == "TongTienHang")
                        {
                            data = data.OrderBy(p => p.TongTienHang).ToList();
                        }
                        if (columsort == "GiamGia")
                        {
                            data = data.OrderBy(p => p.TongGiamGia).ToList();
                        }
                        if (columsort == "KhachCanTra")
                        {
                            data = data.OrderBy(p => p.PhaiThanhToan).ToList();
                        }
                        if (columsort == "KhachDaTra")
                        {
                            data = data.OrderBy(p => p.KhachDaTra).ToList();
                        }
                    }
                    else
                    {
                        if (columsort == "MaHoaDon")
                        {
                            data = data.OrderByDescending(p => p.MaHoaDon).ToList();
                        }
                        if (columsort == "MaHoaDonGoc")
                        {
                            data = data.OrderByDescending(p => p.MaHoaDonGoc).ToList();
                        }
                        if (columsort == "ThoiGian")
                        {
                            data = data.OrderByDescending(p => p.NgayLapHoaDon).ToList();
                        }
                        if (columsort == "KhachHang")
                        {
                            data = data.OrderByDescending(p => p.TenDoiTuong).ToList();
                        }
                        if (columsort == "Email")
                        {
                            data = data.OrderByDescending(p => p.Email).ToList();
                        }
                        if (columsort == "SoDienThoai")
                        {
                            data = data.OrderByDescending(p => p.DienThoai).ToList();
                        }
                        if (columsort == "DiaChi")
                        {
                            data = data.OrderByDescending(p => p.DiaChiKhachHang).ToList();
                        }
                        if (columsort == "KhuVuc")
                        {
                            data = data.OrderByDescending(p => p.KhuVuc).ToList();
                        }
                        if (columsort == "PhuongXa")
                        {
                            data = data.OrderByDescending(p => p.PhuongXa).ToList();
                        }
                        if (columsort == "NguoiBan")
                        {
                            data = data.OrderByDescending(p => p.TenNhanVien).ToList();
                        }
                        if (columsort == "NguoiTao")
                        {
                            data = data.OrderByDescending(p => p.NguoiTaoHD).ToList();
                        }
                        if (columsort == "GhiChu")
                        {
                            data = data.OrderByDescending(p => p.DienGiai).ToList();
                        }
                        if (columsort == "TongTienHang")
                        {
                            data = data.OrderByDescending(p => p.TongTienHang).ToList();
                        }
                        if (columsort == "GiamGia")
                        {
                            data = data.OrderByDescending(p => p.TongGiamGia).ToList();
                        }
                        if (columsort == "KhachCanTra")
                        {
                            data = data.OrderByDescending(p => p.PhaiThanhToan).ToList();
                        }
                        if (columsort == "KhachDaTra")
                        {
                            data = data.OrderByDescending(p => p.KhachDaTra).ToList();
                        }
                    }
                }
                return data.ToList();
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("SP_GetListHoaDons_Where " + e.InnerException + e.Message);
                return null;
            }
        }
        public List<BH_HoaDonDTO> GetListInvoice_Paging(Params_GetListHoaDon param)
        {
            string sIDChiNhanhs = string.Empty;
            string idNhanVienLogin = string.Empty;
            string idViTris = string.Empty;
            string idBangGias = string.Empty;
            string trangthais = string.Empty;
            string phuongthucTT = string.Empty;
            string trangThaiHDSC = "0,1";//0.hdban, 1.hdsc
            List<BH_HoaDonDTO> data = new List<BH_HoaDonDTO>();
            if (param.ID_ChiNhanhs != null)
            {
                sIDChiNhanhs = string.Join(",", param.ID_ChiNhanhs);
            }
            if (param.ID_NhanViens != null && param.ID_NhanViens.Count > 0)
            {
                idNhanVienLogin = string.Join(",", param.ID_NhanViens);
            }
            if (param.ID_ViTris != null && param.ID_ViTris.Count > 0)
            {
                idViTris = string.Join(",", param.ID_ViTris);
            }
            if (param.ID_BangGias != null && param.ID_BangGias.Count > 0)
            {
                idBangGias = string.Join(",", param.ID_BangGias);
            }
            if (param.TrangThaiHDs != null && param.TrangThaiHDs.Count > 0)
            {
                trangthais = string.Join(",", param.TrangThaiHDs);
            }
            if (param.PhuongThucTTs != null && param.PhuongThucTTs.Count > 0)
            {
                phuongthucTT = string.Join(",", param.PhuongThucTTs);
            }
            if (param.LaHoaDonSuaChua != null && param.LaHoaDonSuaChua.Count > 0)
            {
                trangThaiHDSC = string.Join(",", param.LaHoaDonSuaChua);
            }

            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("timeStart", param.NgayTaoHD_TuNgay));
            sql.Add(new SqlParameter("timeEnd", param.NgayTaoHD_DenNgay));
            sql.Add(new SqlParameter("ID_ChiNhanh", sIDChiNhanhs));
            sql.Add(new SqlParameter("maHD", param.MaHoaDon));
            sql.Add(new SqlParameter("ID_NhanVienLogin", idNhanVienLogin));
            sql.Add(new SqlParameter("NguoiTao", param.NguoiTao));

            switch (param.LoaiHoaDon)
            {
                case 1:
                case 2:
                case 25:
                    sql.Add(new SqlParameter("IDViTris", idViTris));
                    sql.Add(new SqlParameter("IDBangGias", idBangGias));
                    sql.Add(new SqlParameter("TrangThai", trangthais));
                    sql.Add(new SqlParameter("PhuongThucThanhToan", phuongthucTT));
                    sql.Add(new SqlParameter("ColumnSort", param.Cot_SapXep));
                    sql.Add(new SqlParameter("SortBy", param.SortBy));
                    sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
                    sql.Add(new SqlParameter("PageSize", param.PageSize));
                    sql.Add(new SqlParameter("LaHoaDonSuaChua", trangThaiHDSC));
                    sql.Add(new SqlParameter("BaoHiem", param.BaoHiem));
                    data = db.Database.SqlQuery<BH_HoaDonDTO>(" exec getlist_HoaDonBanHang @timeStart, @timeEnd, @ID_ChiNhanh, @maHD," +
                    "@ID_NhanVienLogin, @NguoiTao, @IDViTris, @IDBangGias, @TrangThai, @PhuongThucThanhToan, " +
                    "@ColumnSort, @SortBy, @CurrentPage, @PageSize, @LaHoaDonSuaChua, @BaoHiem ", sql.ToArray()).ToList();
                    break;
                case 3:
                    sql.Add(new SqlParameter("TrangThai", trangthais));
                    sql.Add(new SqlParameter("ColumnSort", param.Cot_SapXep));
                    sql.Add(new SqlParameter("SortBy", param.SortBy));
                    sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
                    sql.Add(new SqlParameter("PageSize", param.PageSize));
                    sql.Add(new SqlParameter("LaHoaDonSuaChua", trangThaiHDSC));
                    data = db.Database.SqlQuery<BH_HoaDonDTO>(" exec GetList_HoaDonDatHang @timeStart, @timeEnd, @ID_ChiNhanh, @maHD," +
                         "@ID_NhanVienLogin, @NguoiTao, @TrangThai," +
                         "@ColumnSort, @SortBy, @CurrentPage, @PageSize, @LaHoaDonSuaChua ", sql.ToArray()).ToList();
                    break;
                case 6:
                    sql.Add(new SqlParameter("TrangThai", trangthais));
                    sql.Add(new SqlParameter("ColumnSort", param.Cot_SapXep));
                    sql.Add(new SqlParameter("SortBy", param.SortBy));
                    sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
                    sql.Add(new SqlParameter("PageSize", param.PageSize));
                    data = db.Database.SqlQuery<BH_HoaDonDTO>("EXEC Getlist_HoaDonTraHang @timeStart, @timeEnd, @ID_ChiNhanh,@maHD," +
                         "@ID_NhanVienLogin, @NguoiTao, @TrangThai," +
                         "@ColumnSort, @SortBy, @CurrentPage, @PageSize ", sql.ToArray()).ToList();
                    break;
            }
            return data;
        }

        /// <summary>
        /// get list HoaDon loai 1,3,6,19 (pass object {Parmams_GetListHoaDon})
        /// </summary>
        /// <param name="listParams"></param>
        /// <returns></returns>
        public List<BH_HoaDonDTO> SP_GetListHoaDons_Where_PassObject(Params_GetListHoaDon listParams)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            //string maHDSearch = string.Empty;
            var maHoaDon = listParams.MaHoaDon;
            string sIDChiNhanhs = string.Empty;
            if (listParams.ID_ChiNhanhs != null)
            {
                sIDChiNhanhs = string.Join(",", listParams.ID_ChiNhanhs);
            }

            var loaiHoaDon = listParams.LoaiHoaDon;

            lstParam.Add(new SqlParameter("timeStart", listParams.NgayTaoHD_TuNgay));
            lstParam.Add(new SqlParameter("timeEnd", listParams.NgayTaoHD_DenNgay));
            lstParam.Add(new SqlParameter("ID_ChiNhanh", sIDChiNhanhs));

            List<BH_HoaDonDTO> data = null;
            try
            {
                // not filter txtSeach in SQL
                switch (listParams.LoaiHoaDon)
                {
                    case 1:
                        lstParam.Add(new SqlParameter("MaHD", "%%"));
                        data = db.Database.SqlQuery<BH_HoaDonDTO>("EXEC getlist_HoaDonBanHang_FindMaHang @timeStart, @timeEnd, @ID_ChiNhanh, @MaHD", lstParam.ToArray()).ToList();
                        break;
                    case 3:
                        lstParam.Add(new SqlParameter("MaHD", "%%"));
                        data = db.Database.SqlQuery<BH_HoaDonDTO>("EXEC getlist_HoaDonDatHang_FindMaHang @timeStart, @timeEnd, @ID_ChiNhanh, @MaHD", lstParam.ToArray()).ToList();
                        break;
                    case 6:
                        lstParam.Add(new SqlParameter("MaHD", "%%"));
                        lstParam.Add(new SqlParameter("MaPT", "%%"));
                        lstParam.Add(new SqlParameter("TrangThai", "%%"));
                        data = db.Database.SqlQuery<BH_HoaDonDTO>("EXEC getlist_HoaDonTraHang_FindMaHang @timeStart, @timeEnd, @ID_ChiNhanh, @MaPT, @MaHD, @TrangThai", lstParam.ToArray()).ToList();
                        break;
                    case 19: // goiDV
                        lstParam.Add(new SqlParameter("MaHD", maHoaDon));
                        data = db.Database.SqlQuery<BH_HoaDonDTO>("EXEC GetList_GoiDichVu_Where @timeStart, @timeEnd, @ID_ChiNhanh, @MaHD", lstParam.ToArray()).ToList();
                        break;
                    default:
                        break;
                }


                var lstIDViTri = listParams.ID_ViTris;
                var lstIDBangGia = listParams.ID_BangGias;
                var lstIDNhanVien = listParams.ID_NhanViens;
                var nguoitaoHD = listParams.NguoiTao;

                if (lstIDViTri != null)
                {
                    data = data.Where(hd => lstIDViTri.Contains(hd.ID_ViTri.ToString())).ToList();
                }

                if (lstIDNhanVien != null)
                {
                    if (lstIDNhanVien.Count > 0)
                    {
                        data = data.Where(hd => lstIDNhanVien.Contains(hd.ID_NhanVien.ToString()) || hd.NguoiTaoHD == nguoitaoHD).ToList();
                    }
                }

                if (lstIDBangGia != null)
                {
                    // if search BgiaChung
                    if (lstIDBangGia.Count == 1 && lstIDBangGia.IndexOf("00000000-0000-0000-0000-000000000000") > -1)
                    {
                        data = data.Where(hd => hd.ID_BangGia == Guid.Empty).ToList();
                    }
                    else
                    {
                        if (lstIDBangGia.IndexOf("00000000-0000-0000-0000-000000000000") > -1)
                        {
                            data = data.Where(hd => lstIDBangGia.Contains(hd.ID_BangGia.ToString()) || hd.ID_BangGia == Guid.Empty).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => lstIDBangGia.Contains(hd.ID_BangGia.ToString())).ToList();
                        }
                    }
                }

                switch (listParams.TrangThai)
                {
                    // DatHang: trang thai luu vao truong yeu cau
                    case 0: // Hoàn thành + Huy + Tam luu + GiaoHang (All)
                        break;
                    case 1: //  Huy + HoanThanh + TamLuu
                        if (loaiHoaDon == 3)
                        {
                            data = data.Where(hd => hd.YeuCau != "2").ToList();
                        }
                        break;
                    case 2: // Huy + HoanThanh + GiaoHang
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan != true).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau != "1").ToList();
                        }
                        break;
                    case 3: // Huy + HoanThanh
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan != false).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau != "1" && hd.YeuCau != "2").ToList();
                        }
                        break;
                    case 4: // Huy + TamLuu + GiaoHang
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan != false).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau != "3").ToList();
                        }
                        break;
                    case 5: // Huy + TamLuu
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan != false).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "4").ToList();
                        }
                        break;
                    case 6: // Huy + GiaoHang
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan == null).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "2" || hd.YeuCau == "4").ToList();
                        }
                        break;
                    case 7: // Huy
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan == null).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "4").ToList();
                        }
                        break;
                    case 8: // HoanThanh +TamLuu + GiaoHang
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan != null).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau != "4").ToList();
                        }
                        break;
                    case 9: // HoanThanh +TamLuu
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan != null).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "3").ToList();
                        }
                        break;
                    case 10: // HoanThanh + GiaoHang
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan == false).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "2" || hd.YeuCau == "3").ToList();
                        }
                        break;
                    case 11: // HoanThanh
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan == false).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "3").ToList();
                        }
                        break;
                    case 12: // TamLuu + GiaoHang
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan == true).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "1" || hd.YeuCau == "2").ToList();
                        }
                        break;
                    case 13: // TamLuu
                        if (loaiHoaDon != 3)
                        {
                            data = data.Where(hd => hd.ChoThanhToan == true).ToList();
                        }
                        else
                        {
                            data = data.Where(hd => hd.YeuCau == "1").ToList();
                        }
                        break;
                    case 14: // GiaoHang
                        if (loaiHoaDon == 3)
                        {
                            data = data.Where(hd => hd.YeuCau == "2").ToList();
                        }
                        break;
                    default: // tam luu
                        data = null;
                        break;
                }

                // phuong thuc thanh toan: The, TinMat
                if (listParams.PTThanhToan != null)
                {
                    switch (listParams.PTThanhToan)
                    {
                        case "1":
                            data = data.Where(hd => hd.TienMat >= 0).ToList();
                            break;
                        case "2":
                            data = data.Where(hd => hd.ChuyenKhoan > 0).ToList();
                            break;
                        default:
                            break;
                    }
                }

                var sort = listParams.TrangThai_SapXep;
                if (sort != 0)
                {
                    var columsort = listParams.Cot_SapXep;
                    if (sort == 1)
                    {
                        switch (columsort)
                        {
                            case "MaHoaDon":
                                data = data.OrderBy(p => p.MaHoaDon).ToList();
                                break;
                            case "MaHoaDonGoc":
                                data = data.OrderBy(p => p.MaHoaDonGoc).ToList();
                                break;
                            case "NgayLapHoaDon":
                                data = data.OrderBy(p => p.NgayLapHoaDon).ToList();
                                break;
                            case "MaKhachHang":
                                data = data.OrderBy(p => p.MaDoiTuong).ToList();
                                break;
                            case "TenKhachHang":
                                data = data.OrderBy(p => p.TenDoiTuong).ToList();
                                break;
                            case "Email":
                                data = data.OrderBy(p => p.Email).ToList();
                                break;
                            case "SoDienThoai":
                                data = data.OrderBy(p => p.DienThoai).ToList();
                                break;
                            case "DiaChi":
                                data = data.OrderBy(p => p.DiaChiKhachHang).ToList();
                                break;
                            case "KhuVuc":
                                data = data.OrderBy(p => p.KhuVuc).ToList();
                                break;
                            case "PhuongXa":
                                data = data.OrderBy(p => p.PhuongXa).ToList();
                                break;
                            case "NguoiBan":
                                data = data.OrderBy(p => p.TenNhanVien).ToList();
                                break;
                            case "NguoiTao":
                                data = data.OrderBy(p => p.NguoiTaoHD).ToList();
                                break;
                            case "GhiChu":
                                data = data.OrderBy(p => p.DienGiai).ToList();
                                break;
                            case "TongTienHang":
                                data = data.OrderBy(p => p.TongTienHang).ToList();
                                break;
                            case "GiamGia":
                                data = data.OrderBy(p => p.TongGiamGia).ToList();
                                break;
                            case "KhachCanTra":
                                data = data.OrderBy(p => p.PhaiThanhToan).ToList();
                                break;
                            case "KhachDaTra":
                                data = data.OrderBy(p => p.KhachDaTra).ToList();
                                break;
                            case "TienMat":
                                data = data.OrderBy(p => p.TienMat).ToList();
                                break;
                            case "TienATM":
                                data = data.OrderBy(p => p.TienATM).ToList();
                                break;
                            case "ChuyenKhoan":
                                data = data.OrderBy(p => p.ChuyenKhoan).ToList();
                                break;
                            case "TienDoiDiem":
                                data = data.OrderBy(p => p.TienDoiDiem).ToList();
                                break;
                            case "TienTheGiaTri":
                                data = data.OrderBy(p => p.ThuTuThe).ToList();
                                break;
                            case "GiaTriSDDV":
                                data = data.OrderBy(p => p.GiaTriSDDV).ToList();
                                break;
                            case "ThanhTienCT":
                                data = data.OrderBy(p => p.ThanhTienChuaCK).ToList();
                                break;
                            case "GiamGiaCT":
                                data = data.OrderBy(p => p.GiamGiaCT).ToList();
                                break;
                            case "VAT":
                                data = data.OrderBy(p => p.TongTienThue).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (columsort)
                        {
                            case "MaHoaDon":
                                data = data.OrderByDescending(p => p.MaHoaDon).ToList();
                                break;
                            case "MaHoaDonGoc":
                                data = data.OrderByDescending(p => p.MaHoaDonGoc).ToList();
                                break;
                            case "ThoiGian":
                                data = data.OrderByDescending(p => p.NgayLapHoaDon).ToList();
                                break;
                            case "KhachHang":
                                data = data.OrderByDescending(p => p.TenDoiTuong).ToList();
                                break;
                            case "Email":
                                data = data.OrderByDescending(p => p.Email).ToList();
                                break;
                            case "SoDienThoai":
                                data = data.OrderByDescending(p => p.DienThoai).ToList();
                                break;
                            case "DiaChi":
                                data = data.OrderByDescending(p => p.DiaChiChiNhanh).ToList();
                                break;
                            case "KhuVuc":
                                data = data.OrderByDescending(p => p.KhuVuc).ToList();
                                break;
                            case "PhuongXa":
                                data = data.OrderByDescending(p => p.PhuongXa).ToList();
                                break;
                            case "NguoiBan":
                                data = data.OrderByDescending(p => p.TenNhanVien).ToList();
                                break;
                            case "NguoiTao":
                                data = data.OrderByDescending(p => p.NguoiTaoHD).ToList();
                                break;
                            case "GhiChu":
                                data = data.OrderByDescending(p => p.DienGiai).ToList();
                                break;
                            case "TongTienHang":
                                data = data.OrderByDescending(p => p.TongTienHang).ToList();
                                break;
                            case "GiamGia":
                                data = data.OrderByDescending(p => p.TongGiamGia).ToList();
                                break;
                            case "KhachCanTra":
                                data = data.OrderByDescending(p => p.PhaiThanhToan).ToList();
                                break;
                            case "KhachDaTra":
                                data = data.OrderByDescending(p => p.KhachDaTra).ToList();
                                break;
                            case "TienMat":
                                data = data.OrderByDescending(p => p.TienMat).ToList();
                                break;
                            case "TienATM":
                                data = data.OrderByDescending(p => p.TienATM).ToList();
                                break;
                            case "ChuyenKhoan":
                                data = data.OrderByDescending(p => p.ChuyenKhoan).ToList();
                                break;
                            case "TienDoiDiem":
                                data = data.OrderByDescending(p => p.TienDoiDiem).ToList();
                                break;
                            case "TienTheGiaTri":
                                data = data.OrderByDescending(p => p.ThuTuThe).ToList();
                                break;
                            case "GiaTriSDDV":
                                data = data.OrderByDescending(p => p.GiaTriSDDV).ToList();
                                break;
                            case "ThanhTienCT":
                                data = data.OrderByDescending(p => p.ThanhTienChuaCK).ToList();
                                break;
                            case "GiamGiaCT":
                                data = data.OrderByDescending(p => p.GiamGiaCT).ToList();
                                break;
                            case "VAT":
                                data = data.OrderByDescending(p => p.TongTienThue).ToList();
                                break;
                        }
                    }
                }
                return data;
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("SP_GetListHoaDons_Where_PassObject " + e.InnerException + e.Message);
                return null;
            }
        }

        public List<BH_HoaDonDTO> GetListHoaDonsTraHang_Where(string maHoaDon, DateTime dayStart, DateTime dayEnd, string id_donvi)
        {
            List<BH_HoaDonDTO> lsrReturns = new List<BH_HoaDonDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    // HD Tra
                    var tbl1 = from hd in db.BH_HoaDon
                               join cthd in db.BH_HoaDon_ChiTiet on hd.ID equals cthd.ID_HoaDon
                               where hd.LoaiHoaDon == 6
                               group new { cthd }
                               by new
                               {
                                   ID_HoaDon = hd.ID_HoaDon,
                               } into gr
                               select new
                               {
                                   ID_HoaDon = gr.Key.ID_HoaDon,
                                   SumSoLuongTra = gr.Sum(x => (double?)x.cthd.SoLuong ?? 0)
                               };

                    // HD Goc
                    var tbl2 = from hd in db.BH_HoaDon
                               where hd.LoaiHoaDon == 0 || hd.LoaiHoaDon == 1
                               join cthd in db.BH_HoaDon_ChiTiet on hd.ID equals cthd.ID_HoaDon

                               join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                               from hd_dt in HD_DT.DefaultIfEmpty()

                               join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                               from hd_dv in HD_DV.DefaultIfEmpty()

                               join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                               from hd_nv in HD_NV.DefaultIfEmpty()

                               join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                               from hd_vt in HD_VT.DefaultIfEmpty()

                               join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                               from hd_bg in HD_BG.DefaultIfEmpty()

                               group new { cthd } by new
                               {
                                   ID = hd.ID,
                                   MaHoaDon = hd.MaHoaDon,
                                   ID_HoaDon = hd.ID_HoaDon,
                                   NgayLapHoaDon = hd.NgayLapHoaDon,
                                   TongGiamGia = hd.TongGiamGia,
                                   TongTienHang = hd.TongTienHang,
                                   PhaiThanhToan = hd.PhaiThanhToan,
                                   TongChietKhau = hd.TongChietKhau,
                                   TongChiPhi = hd.TongChiPhi,
                                   ID_NhanVien = hd.ID_NhanVien,
                                   TenNhanVien = hd_nv.TenNhanVien,
                                   TenDoiTuong = hd_dt != null ? hd_dt.TenDoiTuong : "Khách lẻ",
                                   DienGiai = hd.DienGiai,
                                   DienThoai = hd_dt.DienThoai,
                                   ID_ViTri = hd.ID_ViTri,
                                   ID_DonVi = hd.ID_DonVi,
                                   NguoiTaoHD = hd_nv != null ? hd_nv.TenNhanVien : "",
                                   ID_BangGia = hd.ID_BangGia,
                                   ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty,
                               } into gr2
                               select new
                               {
                                   ID = gr2.Key.ID,
                                   MaHoaDon = gr2.Key.MaHoaDon,
                                   ID_HoaDon = gr2.Key.ID_HoaDon,
                                   NgayLapHoaDon = gr2.Key.NgayLapHoaDon,
                                   TongGiamGia = gr2.Key.TongGiamGia,
                                   TongChietKhau = gr2.Key.TongChietKhau,
                                   TongTienHang = gr2.Key.TongTienHang,
                                   PhaiThanhToan = gr2.Key.PhaiThanhToan,
                                   TongChiPhi = gr2.Key.TongChiPhi,
                                   ID_NhanVien = gr2.Key.ID_NhanVien,
                                   TenNhanVien = gr2.Key.TenNhanVien,
                                   TenDoiTuong = gr2.Key.TenDoiTuong,
                                   DienGiai = gr2.Key.DienGiai,
                                   DienThoai = gr2.Key.DienThoai,
                                   ID_ViTri = gr2.Key.ID_ViTri,
                                   ID_DonVi = gr2.Key.ID_DonVi,
                                   NguoiTaoHD = gr2.Key.TenNhanVien,
                                   ID_BangGia = gr2.Key.ID_BangGia,
                                   ID_DoiTuong = gr2.Key.ID_DoiTuong,
                                   SumSoLuongMua = gr2.Sum(x => (double?)x.cthd.SoLuong ?? 0)
                               };

                    // left join 2 table
                    var tblLeftJoin = tbl2.SelectMany(hd => tbl1.Where(hdt => hdt.ID_HoaDon == hd.ID).DefaultIfEmpty(),
                        (hd, hdt) => new
                        {
                            ID = hd.ID,
                            MaHoaDon = hd.MaHoaDon,
                            ID_HoaDon = hd.ID_HoaDon,
                            NgayLapHoaDon = hd.NgayLapHoaDon,
                            TongGiamGia = hd.TongGiamGia,
                            TongChietKhau = hd.TongChietKhau, // PTGiam
                            TongTienHang = hd.TongTienHang,
                            PhaiThanhToan = hd.PhaiThanhToan,
                            TongChiPhi = hd.TongChiPhi,
                            ID_NhanVien = hd.ID_NhanVien,
                            TenNhanVien = hd.TenNhanVien,
                            TenDoiTuong = hd.TenDoiTuong,
                            DienGiai = hd.DienGiai,
                            DienThoai = hd.DienThoai,
                            ID_ViTri = hd.ID_ViTri,
                            ID_DonVi = hd.ID_DonVi,
                            NguoiTaoHD = hd.TenNhanVien,
                            ID_BangGia = hd.ID_BangGia,
                            ID_DoiTuong = hd.ID_DoiTuong,
                            SumSLMua = (double?)hd.SumSoLuongMua ?? 0,
                            SumSLTra = (double?)hdt.SumSoLuongTra ?? 0,
                        })
                    .Where(hd => hd.SumSLMua > hd.SumSLTra);

                    if (tblLeftJoin.Count() > 0)
                    {
                        var data = tblLeftJoin.AsEnumerable().Select(hd => new BH_HoaDonDTO
                        {
                            ID = hd.ID,
                            ID_HoaDon = hd.ID_HoaDon,
                            MaHoaDon = hd.MaHoaDon,
                            TenDoiTuongUnSign = CommonStatic.ConvertToUnSign(hd.TenDoiTuong).ToLower(),
                            TenDoiTuongStartChars = CommonStatic.GetCharsStart(hd.TenDoiTuong).ToLower(),
                            NgayLapHoaDon = hd.NgayLapHoaDon,
                            TongGiamGia = hd.TongGiamGia,
                            TongChietKhau = hd.TongChietKhau,
                            TongTienHang = hd.TongTienHang,
                            PhaiThanhToan = hd.PhaiThanhToan,
                            TongChiPhi = hd.TongChiPhi,
                            ID_NhanVien = hd.ID_NhanVien,
                            TenNhanVien = hd.TenNhanVien,
                            TenDoiTuong = hd.TenDoiTuong,
                            DienGiai = hd.DienGiai,
                            DienThoai = hd.DienThoai,
                            ID_ViTri = hd.ID_ViTri,
                            ID_DonVi = hd.ID_DonVi,
                            NguoiTaoHD = hd.NguoiTaoHD,
                            ID_BangGia = hd.ID_BangGia,
                            ID_DoiTuong = hd.ID_DoiTuong,
                        });

                        var maHDunsingn = CommonStatic.ConvertToUnSign(maHoaDon).ToLower();
                        if (maHoaDon != string.Empty && maHoaDon != null)
                        {
                            data = data.Where(hd => hd.MaHoaDon.ToLower().Contains(@maHDunsingn)
                            || hd.TenDoiTuongUnSign.Contains(@maHDunsingn) || hd.TenDoiTuongStartChars.Contains(@maHDunsingn) ||
                            (hd.DienThoai != null && hd.DienThoai.Contains(@maHDunsingn)));
                        }

                        // if dayStart and dayEnd have type string
                        // NgayLapHoaDon
                        //if (dayStart != null && dayStart != string.Empty)
                        //{
                        //    DateTime dtStart = DateTime.Parse(dayStart);
                        //    if (dayEnd != null && dayEnd != string.Empty)
                        //    {
                        //        DateTime dtEnd = DateTime.Parse(dayEnd);
                        //        if (dayStart == dayEnd)
                        //        {
                        //            data = data.Where(hd => hd.NgayLapHoaDon.Year == dtStart.Year
                        //            && hd.NgayLapHoaDon.Month == dtStart.Month
                        //            && hd.NgayLapHoaDon.Day == dtEnd.Day);
                        //        }
                        //        else
                        //        {
                        //            DateTime dtEndAdd = DateTime.Parse(dayEnd).AddDays(1);
                        //            data = data.Where(hd => hd.NgayLapHoaDon >= dtStart && hd.NgayLapHoaDon < dtEndAdd);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        data = data.Where(hd => hd.NgayLapHoaDon >= dtStart);
                        //    }
                        //}
                        //else
                        //{
                        //    if (dayEnd != null && dayEnd != string.Empty)
                        //    {
                        //        DateTime dtEnd = DateTime.Parse(dayEnd).AddDays(1);
                        //        data = data.Where(hd => hd.NgayLapHoaDon < dtEnd);
                        //    }
                        //}
                        if (dayStart == dayEnd)
                        {
                            data = data.Where(hd => hd.NgayLapHoaDon.Year == dayStart.Year
                            && hd.NgayLapHoaDon.Month == dayStart.Month
                            && hd.NgayLapHoaDon.Day == dayStart.Day);
                        }
                        else
                        {
                            data = data.Where(hd => hd.NgayLapHoaDon >= dayStart && hd.NgayLapHoaDon < dayEnd);
                        }

                        if (id_donvi != string.Empty && id_donvi != null)
                        {
                            data = data.Where(hd => hd.ID_DonVi.ToString().Contains(id_donvi));
                        }
                        return data.OrderByDescending(hd => hd.NgayLapHoaDon).ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("GetListHoaDonsTraHang_Where: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }
        /// <summary>
        /// get CTHD by ID_HoaDon after use goiDV and tra goiDV
        /// </summary>
        /// <param name="idHoaDon"></param>
        /// <returns></returns>
        public List<BH_HoaDon_ChiTietDTO> SP_GetChiTietHoaDonGoiDV_AfterUseAndTra(Guid? idHoaDon)
        {
            if (db != null)
            {
                SqlParameter param = new SqlParameter("ID_HoaDon", idHoaDon);
                try
                {
                    List<BH_HoaDon_ChiTietDTO> lst = db.Database.SqlQuery<BH_HoaDon_ChiTietDTO>(" EXEC SP_GetChiTietHoaDonGoiDV_AfterUseAndTra @ID_HoaDon", param).ToList();
                    return lst;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_GetChiTietHoaDonGoiDV_AfterUseAndTra: " + ex.InnerException + ex.Message);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// get CTHD by ID_HoaDon after tra hang, xulyDatHang
        /// </summary>
        /// <param name="idHoaDon"></param>
        /// <returns></returns>
        public List<BH_HoaDon_ChiTietDTO> SP_GetChiTietHoaDon_afterTraHang(Guid? idHoaDon)
        {
            if (db != null)
            {
                SqlParameter param = new SqlParameter("ID_HoaDon", idHoaDon);
                try
                {
                    ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                    List<BH_HoaDon_ChiTietDTO> lst = db.Database.SqlQuery<BH_HoaDon_ChiTietDTO>("EXEC GetChiTietHoaDon_afterTraHang @ID_HoaDon", param).ToList();
                    foreach (var ct in lst)
                    {
                        if (ct.ID_ChiTietDinhLuong != null && ct.ID_ChiTietDinhLuong == ct.ID)
                        {
                            ct.ThanhPhan_DinhLuong = classhoadonchitiet.SP_GetThanhPhanDinhLuong_CTHD(ct.ID);
                        }
                        if (ct.ID_ParentCombo != null && ct.ID_ParentCombo == ct.ID)
                        {
                            ct.ThanhPhanComBo = GetListComBo_ofCTHD(ct.ID_HoaDon ?? Guid.Empty, ct.ID.ToString());
                        }
                        ct.DonViTinh = db.DonViQuiDois.Where(x => x.ID_HangHoa == ct.ID_HangHoa && x.Xoa != true)
                            .Select(x => new DonViTinh
                            {
                                ID_HangHoa = ct.ID_HangHoa,
                                TenDonViTinh = x.TenDonViTinh,
                                ID_DonViQuiDoi = x.ID,
                                QuanLyTheoLoHang = ct.QuanLyTheoLoHang,
                                Xoa = false,
                                TyLeChuyenDoi = x.TyLeChuyenDoi
                            }).ToList();
                    }
                    return lst;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_GetChiTietHoaDon_afterTraHang: " + ex.InnerException + ex.Message);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public List<BH_HoaDon_ChiTietDTO> GetCTHDSuaChua_afterXuatKho(Guid? idHoaDon)
        {
            SqlParameter param = new SqlParameter("ID_HoaDon", idHoaDon);
            return db.Database.SqlQuery<BH_HoaDon_ChiTietDTO>("EXEC GetCTHDSuaChua_afterXuatKho @ID_HoaDon", param).ToList();
        }

        public bool CheckXuLyHet_DonDathang(Guid idHoaDon, Guid idDatHang)
        {
            try
            {
                if (db != null)
                {
                    var ctDHConLai = SP_GetChiTietHoaDon_afterTraHang(idDatHang).Where(x => x.SoLuongConLai > 0);
                    if (ctDHConLai != null && ctDHConLai.Count() > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true; // xulyhet
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("CheckXuLyHet_DonDathang " + ex.Message + ex.InnerException);
                return false;
            }
        }

        public List<BH_HoaDonDTO> GetListHDXuatKho(Params_GetListHoaDon param)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            var idChiNhanhs = string.Empty;
            if (param.ID_ChiNhanhs != null && param.ID_ChiNhanhs.Count > 0)
            {
                idChiNhanhs = string.Join(",", param.ID_ChiNhanhs);
            }
            var trangthais = string.Empty;
            if (param.TrangThaiHDs != null && param.TrangThaiHDs.Count > 0)
            {
                trangthais = string.Join(",", param.TrangThaiHDs);
            }
            var loaiHoaDons = string.Empty;
            if (param.LaHoaDonSuaChua != null && param.LaHoaDonSuaChua.Count > 0)
            {
                loaiHoaDons = string.Join(",", param.LaHoaDonSuaChua);
            }

            lstParam.Add(new SqlParameter("IDChiNhanhs", idChiNhanhs ?? (object)DBNull.Value));
            lstParam.Add(new SqlParameter("DateFrom", param.NgayTaoHD_TuNgay ?? (object)DBNull.Value));
            lstParam.Add(new SqlParameter("DateTo", param.NgayTaoHD_DenNgay ?? (object)DBNull.Value));
            lstParam.Add(new SqlParameter("LoaiHoaDons", loaiHoaDons ?? (object)DBNull.Value));
            lstParam.Add(new SqlParameter("TrangThais", trangthais ?? (object)DBNull.Value));
            lstParam.Add(new SqlParameter("TextSearch", param.MaHoaDon ?? (object)DBNull.Value));
            lstParam.Add(new SqlParameter("CurrentPage", param.CurrentPage));
            lstParam.Add(new SqlParameter("PageSize", param.PageSize));

            return db.Database.SqlQuery<BH_HoaDonDTO>(" EXEC getList_XuatHuy @IDChiNhanhs, @DateFrom, @DateTo," +
                     " @LoaiHoaDons, @TrangThais, @TextSearch, @CurrentPage, @PageSize", lstParam.ToArray()).ToList();
        }

        public List<BH_HoaDonDTO> GetListHDTraHang_afterUseAndTra(Params_GetListHoaDon param)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            var idChiNhanhs = string.Empty;
            if (param.ID_ChiNhanhs != null && param.ID_ChiNhanhs.Count > 0)
            {
                idChiNhanhs = string.Join(",", param.ID_ChiNhanhs);
            }
            var idNhanViens = string.Empty;
            if (param.ID_NhanViens != null && param.ID_NhanViens.Count > 0)
            {
                idNhanViens = string.Join(",", param.ID_NhanViens);
            }

            lstParam.Add(new SqlParameter("IDChiNhanhs", idChiNhanhs ?? (object)DBNull.Value));
            lstParam.Add(new SqlParameter("IDNhanViens", idNhanViens ?? (object)DBNull.Value));
            lstParam.Add(new SqlParameter("DateFrom", param.NgayTaoHD_TuNgay ?? (object)DBNull.Value));
            lstParam.Add(new SqlParameter("DateTo", param.NgayTaoHD_DenNgay ?? (object)DBNull.Value));
            lstParam.Add(new SqlParameter("TextSearch", param.MaHoaDon ?? (object)DBNull.Value));
            lstParam.Add(new SqlParameter("CurrentPage", param.CurrentPage));
            lstParam.Add(new SqlParameter("PageSize", param.PageSize));

            if (param.LoaiHoaDon == 19)
            {
                return db.Database.SqlQuery<BH_HoaDonDTO>(" EXEC GetList_GoiDichVu_afterUseAndTra @IDChiNhanhs, @IDNhanViens," +
                     " @DateFrom, @DateTo, @TextSearch, @CurrentPage, @PageSize", lstParam.ToArray()).ToList();
            }
            else
            {
                return db.Database.SqlQuery<BH_HoaDonDTO>(" EXEC getlist_HoaDon_afterTraHang_DichVu @IDChiNhanhs, @IDNhanViens," +
                    " @DateFrom, @DateTo, @TextSearch, @CurrentPage, @PageSize", lstParam.ToArray()).ToList();
            }
        }

        /// <summary>
        /// get lst HD DatHang: PhieuTam, DangGiaoHang
        /// </summary>
        /// <param name="lstParamObj"></param>
        /// <returns></returns>
        public List<BH_HoaDonDTO> GetHoaDonDatHang_afterXuLy(Params_GetListHoaDon lstParamObj)
        {
            if (db != null)
            {
                try
                {
                    var msHDSearch = string.Empty;
                    if (lstParamObj.MaHoaDon == null || lstParamObj.MaHoaDon == string.Empty)
                    {
                        msHDSearch = "%%";
                    }
                    else
                    {
                        msHDSearch = "%" + CommonStatic.ConvertToUnSign(lstParamObj.MaHoaDon).ToLower() + "%";
                    }

                    var idDonVi = "%%";
                    if (lstParamObj.ID_ChiNhanhs != null)
                    {
                        idDonVi = lstParamObj.ID_ChiNhanhs[0];
                    }

                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Add(new SqlParameter("timeStart", lstParamObj.NgayTaoHD_TuNgay));
                    lstParam.Add(new SqlParameter("timeEnd", lstParamObj.NgayTaoHD_DenNgay));
                    lstParam.Add(new SqlParameter("ID_ChiNhanh", idDonVi));
                    lstParam.Add(new SqlParameter("txtSearch", msHDSearch));
                    lstParam.Add(new SqlParameter("CurrentPage", lstParamObj.CurrentPage));
                    lstParam.Add(new SqlParameter("PageSize", lstParamObj.PageSize));

                    List<BH_HoaDonDTO> lst = db.Database.SqlQuery<BH_HoaDonDTO>(" EXEC GetHoaDonDatHang_afterXuLy @timeStart, @timeEnd, @ID_ChiNhanh, @txtSearch, @CurrentPage, @PageSize", lstParam.ToArray()).ToList();
                    return lst;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("GetHoaDonDatHang_afterXuLy: " + ex.InnerException + ex.Message);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        //trinhpv get phieu xuat huy
        public List<BH_HoaDonDTO> GetListXuatHuy(int LoaiHoaDon)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    var tbl = from hd in db.BH_HoaDon
                                  //join qct in db.Quy_HoaDon_ChiTiet on hd.ID equals qct.ID into HD_QHD
                                  //from qct_hd in HD_QHD.DefaultIfEmpty()
                              join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                              from hd_dt in HD_DT.DefaultIfEmpty()
                              where hd.LoaiHoaDon == LoaiHoaDon // && hd.ChoThanhToan == false

                              join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                              from hd_dv in HD_DV.DefaultIfEmpty()
                              where hd.LoaiHoaDon == LoaiHoaDon // && hd.ChoThanhToan == false

                              join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                              from hd_nv in HD_NV.DefaultIfEmpty()
                              join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                              from hd_vt in HD_VT.DefaultIfEmpty()
                              join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                              from hd_bg in HD_BG.DefaultIfEmpty()

                              select new BH_HoaDonDTO
                              {
                                  ID = hd.ID,
                                  MaHoaDon = hd.MaHoaDon,
                                  TenDonVi = hd_dv.TenDonVi,
                                  NgayLapHoaDon = hd.NgayLapHoaDon,
                                  TongGiamGia = hd.TongGiamGia,
                                  TongTienHang = hd.TongTienHang,
                                  PhaiThanhToan = hd.PhaiThanhToan,
                                  ID_NhanVien = hd.ID_NhanVien,
                                  TenNhanVien = hd_nv.TenNhanVien,
                                  TenDoiTuong = hd_dt.TenDoiTuong,
                                  DienGiai = hd.DienGiai,
                                  Email = hd_dt.Email,
                                  DienThoai = hd_dt.DienThoai,
                                  TenPhongBan = hd_vt.TenViTri,
                                  NguoiTaoHD = hd_nv.TenNhanVien,
                                  TenBangGia = hd_bg.TenGiaBan,
                                  ChoThanhToan = hd.ChoThanhToan,
                                  // trạng thái lưu vào trường yêu cầu
                                  YeuCau = hd.YeuCau,
                                  ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty
                              };
                    return tbl.ToList();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("GetListXuatHuy " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }

        public bool Check_MaHoaDonExist(string maHoaDon, Guid? id = null)
        {
            if (db == null || string.IsNullOrEmpty(maHoaDon))
            {
                return false;
            }
            else
            {
                maHoaDon = maHoaDon.Trim();
                if (id != null && id != Guid.Empty)
                {
                    return db.BH_HoaDon.Where(x => x.ID != id).Count(e => e.MaHoaDon == maHoaDon) > 0;
                }
                else
                {
                    return db.BH_HoaDon.Count(e => e.MaHoaDon == maHoaDon) > 0;
                }
            }
        }
        public bool Check_MaCaLamViec(string MaCaLamViec)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.NS_CaLamViec.Count(e => e.MaCa == MaCaLamViec) > 0;
            }
        }
        public bool Check_MaPhieuPhanCa(string MaPhieu)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.NS_PhieuPhanCa.Count(e => e.MaPhieu == MaPhieu) > 0;
            }
        }

        public List<BH_HoaDonDTO> GetHoaDonFrom_IDViTri(Guid id)
        {
            var tbl = (from hd in db.BH_HoaDon
                       join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into hd_kh
                       from HD_KH in hd_kh.DefaultIfEmpty()
                       where hd.ID_ViTri == id && hd.ChoThanhToan == true
                       select new BH_HoaDonDTO
                       {
                           ID = hd.ID,
                           ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty,
                           ID_NhanVien = hd.ID_NhanVien,
                           ID_BangGia = hd.ID_BangGia,
                           ID_ViTri = hd.ID_ViTri,
                           TenDoiTuong = HD_KH == null ? "" : HD_KH.TenDoiTuong,
                           NgayLapHoaDon = hd.NgayLapHoaDon,
                           TongTienHang = hd.TongTienHang,
                           PhaiThanhToan = hd.PhaiThanhToan,
                           TongGiamGia = hd.TongGiamGia, // DaThanhToan (0 - because ChothanhToan = true)
                           DienGiai = hd.DienGiai,
                           MaHoaDon = hd.MaHoaDon,
                           TenPhongBan = hd.DM_ViTri.TenViTri,
                           //Loa   hd.LoaiHoaDon
                       }).ToList();
            if (tbl != null)
                return tbl;
            else
                return null;
        }

        public List<BH_HoaDonDTO> GetHD_ByID(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    var tbl = (from hd in db.BH_HoaDon
                               join qct in db.Quy_HoaDon_ChiTiet on hd.ID equals qct.ID_HoaDonLienQuan into QCT_HD
                               from qct_hd in QCT_HD.DefaultIfEmpty()

                               join qhd in db.Quy_HoaDon on qct_hd.ID_HoaDon equals qhd.ID into QUY
                               from quy in QUY.DefaultIfEmpty()

                               join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                               from hd_dt in HD_DT.DefaultIfEmpty()

                               join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                               from hd_dv in HD_DV.DefaultIfEmpty()

                               join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                               from hd_nv in HD_NV.DefaultIfEmpty()

                               join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                               from hd_vt in HD_VT.DefaultIfEmpty()

                               join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                               from hd_bg in HD_BG.DefaultIfEmpty()
                               where hd.ID == id

                               select new BH_HoaDonDTO
                               {
                                   ID = hd.ID,
                                   ID_HoaDon = hd.ID_HoaDon,
                                   MaHoaDon = hd.MaHoaDon,
                                   TenDonVi = hd_dv.TenDonVi,
                                   DiaChiChiNhanh = hd_dv.DiaChi,
                                   DienThoaiChiNhanh = hd_dv.SoDienThoai,
                                   NgayLapHoaDon = hd.NgayLapHoaDon,
                                   TongGiamGia = hd.TongGiamGia,
                                   TongTienHang = hd.TongTienHang,
                                   PhaiThanhToan = hd.PhaiThanhToan,
                                   TongChiPhi = hd.TongChiPhi,
                                   ID_NhanVien = hd.ID_NhanVien,
                                   TenNhanVien = hd_nv.TenNhanVien,
                                   ID_DoiTuong = hd.ID_DoiTuong,
                                   TenDoiTuong = hd_dt != null ? hd_dt.TenDoiTuong : "Khách lẻ",
                                   DienGiai = hd.DienGiai,
                                   Email = hd_dt.Email,
                                   DiaChiKhachHang = hd_dt.DiaChi,
                                   DienThoai = hd_dt.DienThoai,
                                   ID_ViTri = hd.ID_ViTri,
                                   TenPhongBan = hd_vt != null ? hd_vt.TinhTrang == true ? "" : hd_vt.TenViTri : "", // if PhongBan was deleted --> TenPhongBan = ""
                                   NguoiTaoHD = hd.NguoiTao,
                                   ID_BangGia = hd.ID_BangGia,
                                   TenBangGia = hd_bg != null ? hd_bg.TenGiaBan : "Bảng giá chung",
                                   ChoThanhToan = hd.ChoThanhToan,
                                   LoaiHoaDon = hd.LoaiHoaDon,
                                   KhachDaTra = (quy == null || (quy != null && quy.TrangThai == false)) ? 0 : (double?)quy.TongTienThu ?? 0,
                                   TienMat = (quy == null || (quy != null && quy.TrangThai == false)) ? 0 : (double?)qct_hd.TienMat ?? 0,
                                   TienATM = (quy == null || (quy != null && quy.TrangThai == false)) ? 0 : (double?)qct_hd.TienGui ?? 0,
                                   TongChietKhau = (double?)hd.TongChietKhau ?? 0,
                                   TrangThai = hd.ChoThanhToan == null ? "Đã hủy" : (hd.ChoThanhToan == false ? "Hoàn thành" : "Tạm lưu"),
                                   NgayApDungGoiDV = hd.NgayApDungGoiDV.ToString(),
                                   HanSuDungGoiDV = hd.HanSuDungGoiDV.ToString(),
                                   TongTienThue = hd.TongTienThue,
                                   DiemGiaoDich = hd.DiemGiaoDich ?? 0,
                               }).ToList();
                    return tbl;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("GetHD_ByID: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }

        #endregion

        #region insert
        public string Add_HoaDon(BH_HoaDon objAdd)
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
                    db.BH_HoaDon.Add(objAdd);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        strErr = "Add_HoaDon: " + ex.InnerException + ex.Message;
                        CookieStore.WriteLog(strErr);
                    }
                }
                catch (DbEntityValidationException dbEx)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var eve in dbEx.EntityValidationErrors)
                    {
                        sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                        eve.Entry.Entity.GetType().Name,
                                                        eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                        ve.PropertyName,
                                                        ve.ErrorMessage));
                        }
                    }
                    throw new DbEntityValidationException(sb.ToString(), dbEx);
                }
            }
            return strErr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objAdd"></param>
        /// <returns></returns>
        public Guid SP_Insert_HoaDon(BH_HoaDon objAdd)
        {
            var classhoadon = new ClassBH_HoaDon(db);
            string strErr = string.Empty;
            if (db == null)
            {
                return Guid.Empty;
            }
            else
            {
                try
                {
                    var id = Guid.NewGuid();
                    var maHoaDon = string.Empty;
                    if (objAdd.MaHoaDon == null || objAdd.MaHoaDon == "")
                    {
                        // neu la chuyen ghep ban
                        if (objAdd.ChoThanhToan.Value)
                        {
                            maHoaDon = classhoadon.SP_GetAutoCode_HDDatHang();
                        }
                        else
                        {
                            maHoaDon = classhoadon.SP_GetAutoCode(objAdd.LoaiHoaDon);
                        }
                    }
                    else
                    {
                        maHoaDon = objAdd.MaHoaDon;
                    }
                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Add(new SqlParameter("ID", id));
                    lstParam.Add(new SqlParameter("MaHoaDon", maHoaDon));
                    lstParam.Add(new SqlParameter("ID_HoaDon", objAdd.ID_HoaDon ?? (object)DBNull.Value));
                    lstParam.Add(new SqlParameter("ID_NhanVien", objAdd.ID_NhanVien ?? (object)DBNull.Value));
                    lstParam.Add(new SqlParameter("ID_ViTri", objAdd.ID_ViTri ?? (object)DBNull.Value));
                    lstParam.Add(new SqlParameter("NguoiTao", objAdd.NguoiTao));
                    lstParam.Add(new SqlParameter("DienGiai", objAdd.DienGiai));
                    lstParam.Add(new SqlParameter("YeuCau", objAdd.YeuCau));
                    lstParam.Add(new SqlParameter("ID_DoiTuong", objAdd.ID_DoiTuong ?? (object)DBNull.Value));
                    lstParam.Add(new SqlParameter("NgayLapHoaDon", objAdd.NgayLapHoaDon));
                    lstParam.Add(new SqlParameter("PhaiThanhToan", objAdd.PhaiThanhToan));
                    lstParam.Add(new SqlParameter("TongGiamGia", objAdd.TongGiamGia));

                    lstParam.Add(new SqlParameter("TongChiPhi", objAdd.TongChiPhi));
                    lstParam.Add(new SqlParameter("TongTienHang", objAdd.TongTienHang));
                    lstParam.Add(new SqlParameter("ID_DonVi", objAdd.ID_DonVi));
                    lstParam.Add(new SqlParameter("TyGia", 1));
                    lstParam.Add(new SqlParameter("LoaiHoaDon", objAdd.LoaiHoaDon));
                    lstParam.Add(new SqlParameter("ID_BangGia", objAdd.ID_BangGia ?? (object)DBNull.Value));
                    lstParam.Add(new SqlParameter("ChoThanhToan", objAdd.ChoThanhToan));//neu luu tam => cho thanh toan == false
                    lstParam.Add(new SqlParameter("TongChietKhau", objAdd.TongChietKhau));
                    lstParam.Add(new SqlParameter("TongTienThue", objAdd.TongTienThue));
                    lstParam.Add(new SqlParameter("DiemGiaoDich", objAdd.DiemGiaoDich));
                    lstParam.Add(new SqlParameter("ID_KhuyenMai", objAdd.ID_KhuyenMai ?? (object)DBNull.Value));
                    lstParam.Add(new SqlParameter("KhuyeMai_GiamGia", objAdd.KhuyeMai_GiamGia));
                    lstParam.Add(new SqlParameter("KhuyenMai_GhiChu", objAdd.KhuyenMai_GhiChu));
                    lstParam.Add(new SqlParameter("NgayApDungGoiDV", objAdd.NgayApDungGoiDV ?? (object)DBNull.Value));
                    lstParam.Add(new SqlParameter("HanSuDungGoiDV", objAdd.HanSuDungGoiDV ?? (object)DBNull.Value));

                    db.Database.ExecuteSqlCommand("EXEC SP_Insert_HoaDon @ID, @MaHoaDon, @ID_HoaDon, @ID_NhanVien,@ID_ViTri," +
                         "@NguoiTao,@DienGiai,@YeuCau,@ID_DoiTuong,@NgayLapHoaDon,@PhaiThanhToan,@TongGiamGia," +
                         "@TongChiPhi, @TongTienHang, @ID_DonVi, @TyGia, @LoaiHoaDon,@ID_BangGia," +
                         "@ChoThanhToan,@TongChietKhau,@TongTienThue,@DiemGiaoDich,@ID_KhuyenMai,@KhuyeMai_GiamGia,@KhuyenMai_GhiChu," +
                         "@NgayApDungGoiDV,@HanSuDungGoiDV", lstParam.ToArray());

                    return id;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_InsertHoaDon: " + ex.InnerException + ex.Message);
                    return Guid.Empty;
                }
            }
        }

        public Guid SP_Insert_HoaDon2(BH_HoaDon objAdd)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return Guid.Empty;
            }
            else
            {
                try
                {
                    var id = Guid.NewGuid();
                    var sID_ViTri = objAdd.ID_ViTri == null ? "null" : "'" + objAdd.ID_ViTri + "'";
                    var sID_BangGia = objAdd.ID_BangGia == null ? "null" : "'" + objAdd.ID_BangGia + "'";
                    var sID_DoiTuong = objAdd.ID_DoiTuong == null ? "null" : "'" + objAdd.ID_DoiTuong + "'";
                    var sID_HoaDon = objAdd.ID_HoaDon == null ? "null" : "'" + objAdd.ID_HoaDon + "'";
                    var sID_KhuyenMai = objAdd.ID_KhuyenMai == null ? "null" : "'" + objAdd.ID_KhuyenMai + "'";
                    var isChoThanhToan = objAdd.ChoThanhToan ?? false;
                    string sChoThanhToan = isChoThanhToan ? "1" : "0";

                    var sql = "INSERT INTO BH_HoaDon (" +
                        "ID," +
                        "MaHoaDon," +
                        "ID_HoaDon," +
                        "ID_NhanVien," +
                        "ID_ViTri," +
                        "NguoiTao," +
                        "DienGiai," +
                        "YeuCau," +
                        "ID_DoiTuong," +

                       "NgayLapHoaDon," +
                       "PhaiThanhToan," +
                       "TongGiamGia," +
                       "TongChiPhi," +
                       "TongTienHang," +
                       "ID_DonVi, " +
                       "TyGia," +
                       "LoaiHoaDon," +
                       "ID_BangGia," +
                       "ChoThanhToan," +
                       "TongChietKhau," +
                       "TongTienThue," +
                       "DiemGiaoDich," +
                       "ID_KhuyenMai," +
                       "KhuyeMai_GiamGia," +
                       "KhuyenMai_GhiChu," +
                       "NgayApDungGoiDV," +
                       "HanSuDungGoiDV, " +
                       "NgayTao)" +

                       " values (" +

                        "'" + id + "'," +
                        "'" + objAdd.MaHoaDon + "'," +
                        sID_HoaDon + "," +
                        "'" + objAdd.ID_NhanVien + "'," +
                        sID_ViTri + "," +
                        "'" + objAdd.NguoiTao + "'," +
                        "'" + objAdd.DienGiai + "'," +
                        "'" + objAdd.YeuCau + "'," +
                        sID_DoiTuong + "," +

                        " Getdate()," +
                        objAdd.PhaiThanhToan + "," +
                        objAdd.TongGiamGia + "," +
                        objAdd.TongChiPhi + "," +
                        objAdd.TongTienHang + "," +
                        "'" + objAdd.ID_DonVi + "'," +
                       "1," +
                       objAdd.LoaiHoaDon + "," +
                       sID_BangGia + "," +
                       sChoThanhToan + "," +
                       objAdd.TongChietKhau + "," +
                       objAdd.TongTienThue + "," +
                       objAdd.DiemGiaoDich + "," +
                       sID_KhuyenMai + "," +
                       objAdd.KhuyeMai_GiamGia + "," +
                        "'" + objAdd.KhuyenMai_GhiChu + "'," +
                       "NULL," +
                       "NULL," +
                       "GetDate()" + ")";

                    db.Database.ExecuteSqlCommand(sql);

                    return id;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_InsertHoaDon: " + ex.InnerException + ex.Message);
                    return Guid.Empty;
                }
            }
            //return strErr;
        }

        public string SP_Insert_HoaDonChiTiet(BH_HoaDon_ChiTiet objAdd)
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
                    //SqlParameter param1 = new SqlParameter("ID", id);
                    //SqlParameter param2 = new SqlParameter("ID_DonViQuiDoi", objAdd.ID_DonViQuiDoi);
                    //SqlParameter param3 = new SqlParameter("SoThuTu", objAdd.SoThuTu);
                    //SqlParameter param4 = new SqlParameter("DonGia", objAdd.DonGia);
                    //SqlParameter param5 = new SqlParameter("GiaVon", objAdd.GiaVon);
                    //SqlParameter param6 = new SqlParameter("ID_HoaDon", objAdd.ID_HoaDon);
                    //SqlParameter param7 = new SqlParameter("SoLuong", objAdd.SoLuong);
                    //SqlParameter param8 = new SqlParameter("ThanhTien", objAdd.ThanhTien);
                    //SqlParameter param9 = new SqlParameter("ThanhToan", objAdd.ThanhToan);
                    //SqlParameter param10 = new SqlParameter("PTChietKhau", objAdd.PTChietKhau);
                    //SqlParameter param11 = new SqlParameter("TienChietKhau", objAdd.TienChietKhau);
                    //SqlParameter param12 = new SqlParameter("ThoiGian", objAdd.ThoiGian);

                    //SqlParameter param13 = new SqlParameter("GhiChu", objAdd.GhiChu);
                    //SqlParameter param14 = new SqlParameter("TangKem", objAdd.TangKem);
                    //SqlParameter param15 = new SqlParameter("ID_TangKem", objAdd.ID_TangKem);
                    //SqlParameter param16 = new SqlParameter("ID_KhuyenMai", objAdd.ID_KhuyenMai);
                    //SqlParameter param17 = new SqlParameter("ID_LoHang", objAdd.ID_LoHang);
                    //SqlParameter param18 = new SqlParameter("ID_ChiTietGoiDV", objAdd.ID_ChiTietGoiDV);
                    //SqlParameter param19 = new SqlParameter("PTThue", objAdd.PTThue);
                    //SqlParameter param20 = new SqlParameter("TienThue", objAdd.TienThue);
                    //SqlParameter param21 = new SqlParameter("LoaiThoiGianBH", objAdd.LoaiThoiGianBH);
                    //SqlParameter param22= new SqlParameter("ThoiGianBaoHanh", objAdd.ThoiGianBaoHanh);

                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Add(new SqlParameter("ID", objAdd.ID));
                    lstParam.Add(new SqlParameter("ID_DonViQuiDoi", objAdd.ID_DonViQuiDoi));
                    lstParam.Add(new SqlParameter("SoThuTu", objAdd.SoThuTu));
                    lstParam.Add(new SqlParameter("DonGia", objAdd.DonGia));
                    lstParam.Add(new SqlParameter("GiaVon", objAdd.GiaVon));
                    lstParam.Add(new SqlParameter("ID_HoaDon", objAdd.ID_HoaDon));
                    lstParam.Add(new SqlParameter("SoLuong", objAdd.SoLuong));
                    lstParam.Add(new SqlParameter("ThanhTien", objAdd.ThanhTien));
                    lstParam.Add(new SqlParameter("ThanhToan", objAdd.ThanhToan));
                    lstParam.Add(new SqlParameter("PTChietKhau", objAdd.PTChietKhau));
                    lstParam.Add(new SqlParameter("TienChietKhau", objAdd.TienChietKhau));
                    lstParam.Add(new SqlParameter("ThoiGian", objAdd.ThoiGian ?? (object)DBNull.Value));

                    lstParam.Add(new SqlParameter("GhiChu", objAdd.GhiChu));
                    lstParam.Add(new SqlParameter("TangKem", objAdd.TangKem));
                    lstParam.Add(new SqlParameter("ID_TangKem", objAdd.ID_TangKem ?? (object)DBNull.Value));
                    lstParam.Add(new SqlParameter("ID_KhuyenMai", objAdd.ID_KhuyenMai ?? (object)DBNull.Value));
                    lstParam.Add(new SqlParameter("ID_LoHang", objAdd.ID_LoHang ?? (object)DBNull.Value));
                    lstParam.Add(new SqlParameter("ID_ChiTietGoiDV", objAdd.ID_ChiTietGoiDV ?? (object)DBNull.Value));
                    lstParam.Add(new SqlParameter("PTThue", objAdd.PTThue ?? (object)DBNull.Value));
                    lstParam.Add(new SqlParameter("TienThue", objAdd.TienThue));
                    lstParam.Add(new SqlParameter("LoaiThoiGianBH", objAdd.LoaiThoiGianBH));
                    lstParam.Add(new SqlParameter("ThoiGianBaoHanh", objAdd.ThoiGianBaoHanh));

                    db.Database.ExecuteSqlCommand("EXEC SP_Insert_BHHoaDonChiTiet " +
                        "@ID, @ID_DonViQuiDoi, @SoThuTu, @DonGia, @GiaVon, @ID_HoaDon, @SoLuong, @ThanhTien, @ThanhToan, @PTChietKhau, @TienChietKhau, @ThoiGian," +
                        "@GhiChu, @TangKem ,@ID_TangKem, @ID_KhuyenMai,@ID_LoHang, @ID_ChiTietGoiDV, @PTThue, @TienThue, @LoaiThoiGianBH, @ThoiGianBaoHanh ", lstParam.ToArray());

                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_Insert_HoaDonChiTiet: " + ex.InnerException + ex.Message);
                }
            }
            return strErr;
        }

        public string SP_Insert_HoaDonChiTiet2(BH_HoaDon_ChiTiet objAdd)
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
                    var isTangKem = objAdd.TangKem ?? false;
                    string sTangKem = isTangKem ? "1" : "0";
                    string sID_TangKem = objAdd.ID_TangKem == null ? "null" : "'" + objAdd.ID_TangKem + "'";
                    string sID_LoHang = objAdd.ID_LoHang == null ? "null" : "'" + objAdd.ID_LoHang + "'";
                    var sID_KhuyenMai = objAdd.ID_KhuyenMai == null ? "null" : "'" + objAdd.ID_KhuyenMai + "'";
                    var sIDCT_GoiDV = objAdd.ID_ChiTietGoiDV == null ? "null" : "'" + objAdd.ID_ChiTietGoiDV + "'";

                    var sql = "INSERT INTO BH_HoaDon_ChiTiet(" +
                                                "ID," +
                                                "ID_DonViQuiDoi," +
                                                "SoThuTu," +
                                                "DonGia," +
                                                "GiaVon," +
                                                "ID_HoaDon," +
                                                "SoLuong," +
                                                "ThanhTien," +
                                                "ThanhToan," +
                                                "PTChietKhau," +
                                                "TienChietKhau," +
                                                "ThoiGian," +
                                                "GhiChu," +
                                                "TangKem," +
                                                "ID_TangKem," +
                                                "ID_KhuyenMai," +
                                                "ID_LoHang," +
                                                "ID_ChiTietGoiDV," +
                                                "PTThue," +
                                                "TienThue," +
                                                "LoaiThoiGianBH," +
                                                "ThoiGianBaoHanh," +
                                                "PTChiPhi," +
                                                "TienChiPhi," +
                                                "An_Hien) " +

                                                "values (" +

                                                 "'" + objAdd.ID + "', " +
                                                 "'" + objAdd.ID_DonViQuiDoi + "', " +
                                                objAdd.SoThuTu + ", " +
                                                objAdd.DonGia + ", " +
                                                objAdd.GiaVon + ", " +
                                                 "'" + objAdd.ID_HoaDon + "', " +
                                                objAdd.SoLuong + ", " +
                                                objAdd.ThanhTien + ", " +
                                                objAdd.ThanhToan + ", " +
                                                objAdd.PTChietKhau + ", " +
                                                objAdd.TienChietKhau + ", GetDate()," +

                                                 //" CONVERT(datetime,'"+ objAdd.ThoiGian.ToString() + "',20)" + ", " +

                                                 "'" + objAdd.GhiChu + "', " +
                                                 "'" + sTangKem + "', " +
                                                sID_TangKem + ", " +
                                                sID_KhuyenMai + ", " +
                                                sID_LoHang + ", " +
                                                sIDCT_GoiDV + ", " +
                                                objAdd.PTThue + ", " +
                                                objAdd.TienThue + ", " +
                                                objAdd.LoaiThoiGianBH + ", " +
                                                objAdd.ThoiGianBaoHanh + ", 0 ,0,'0')";

                    db.Database.ExecuteSqlCommand(sql);
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_Insert_HoaDonChiTiet: " + ex.InnerException + ex.Message);
                }
            }
            return strErr;
        }

        #endregion

        #region update

        public string SetDefault_IDDoiTuongInPhieuThu()
        {
            try
            {
                db.Database.ExecuteSqlCommand("exec SetDefault_IDDoiTuongInPhieuThu");
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.InnerException + ex.Message;
            }
        }

        public string SetDefault_IDDoiTuongInHoaDon()
        {
            try
            {
                db.Database.ExecuteSqlCommand("exec SetDefault_IDDoiTuongInHoaDon");
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.InnerException + ex.Message;
            }
        }

        public string Update_HoaDon(BH_HoaDon obj)
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
                    BH_HoaDon objUpd = db.BH_HoaDon.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.MaHoaDon = obj.MaHoaDon;
                    objUpd.DienGiai = obj.DienGiai;
                    objUpd.ID_NhanVien = obj.ID_NhanVien;
                    objUpd.ID_HoaDon = obj.ID_HoaDon;
                    objUpd.ID_PhieuTiepNhan = obj.ID_PhieuTiepNhan;
                    objUpd.ID_DoiTuong = obj.ID_DoiTuong;
                    objUpd.PhaiThanhToan = obj.PhaiThanhToan; // Giatritang
                    objUpd.TongChietKhau = obj.TongChietKhau; //GiaTriGiam
                    objUpd.TongTienThue = obj.TongTienThue; //TongTienlech
                    objUpd.TongGiamGia = obj.TongGiamGia; //Tổng chênh lệch
                    objUpd.TongChiPhi = obj.TongChiPhi; //SL lệch tăng
                    objUpd.TongTienHang = obj.TongTienHang; // SL lệch giảm
                    objUpd.TongThanhToan = obj.TongThanhToan; // SL lệch giảm
                    objUpd.ChoThanhToan = obj.ChoThanhToan;
                    objUpd.PTThueHoaDon = obj.PTThueHoaDon;
                    objUpd.YeuCau = obj.YeuCau;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NgaySua = DateTime.Now;
                    objUpd.NgayLapHoaDon = obj.NgayLapHoaDon;
                    objUpd.NgayApDungGoiDV = obj.NgayApDungGoiDV;// goiDV 'YYYY-MM-DD;'
                    objUpd.HanSuDungGoiDV = obj.HanSuDungGoiDV;

                    objUpd.CongThucBaoHiem = obj.CongThucBaoHiem;
                    objUpd.TongTienBHDuyet = obj.TongTienBHDuyet;
                    objUpd.KhauTruTheoVu = obj.KhauTruTheoVu;
                    objUpd.SoVuBaoHiem = obj.SoVuBaoHiem;
                    objUpd.PTGiamTruBoiThuong = obj.PTGiamTruBoiThuong;
                    objUpd.GiamTruBoiThuong = obj.GiamTruBoiThuong;
                    objUpd.BHThanhToanTruocThue = obj.BHThanhToanTruocThue;
                    objUpd.PTThueBaoHiem = obj.PTThueBaoHiem;
                    objUpd.TongTienThueBaoHiem = obj.TongTienThueBaoHiem;
                    objUpd.GiamTruThanhToanBaoHiem = obj.GiamTruThanhToanBaoHiem;
                    objUpd.PhaiThanhToanBaoHiem = obj.PhaiThanhToanBaoHiem;

                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("Update_HoaDon:" + ex.Message + ex.InnerException);
                }
            }
            return strErr;
        }

        public BH_HoaDon Update_HoaDon_DatHang(BH_HoaDon obj)
        {
            var classhoadon = new ClassBH_HoaDon(db);
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    BH_HoaDon objUpd = db.BH_HoaDon.Find(obj.ID);
                    // only update again MaHoaDon if MaHoaDon empty
                    if (obj.MaHoaDon == string.Empty)
                    {
                        if (obj.ChoThanhToan.Value == true)
                        {
                            objUpd.MaHoaDon = classhoadon.SP_GetAutoCode_HDDatHang();
                        }
                        else
                        {
                            objUpd.MaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(obj.LoaiHoaDon, obj.ID_DonVi, obj.NgayLapHoaDon);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(obj.MaHoaDon) && obj.MaHoaDon != objUpd.MaHoaDon)
                        {
                            objUpd.MaHoaDon = obj.MaHoaDon;
                        }
                    }
                    objUpd.LoaiHoaDon = obj.LoaiHoaDon;
                    objUpd.ID_NhanVien = obj.ID_NhanVien == Guid.Empty ? null : obj.ID_NhanVien;
                    objUpd.ID_ViTri = obj.ID_ViTri == Guid.Empty ? null : obj.ID_ViTri;
                    objUpd.DienGiai = obj.DienGiai;
                    objUpd.ID_DoiTuong = obj.ID_DoiTuong == null ? Guid.Empty : obj.ID_DoiTuong;
                    objUpd.PhaiThanhToan = obj.PhaiThanhToan;
                    objUpd.TongGiamGia = obj.TongGiamGia;
                    objUpd.TongChietKhau = obj.TongChietKhau;
                    objUpd.TongChiPhi = obj.TongChiPhi;
                    objUpd.ChiPhi = obj.ChiPhi;
                    objUpd.ChiPhi_GhiChu = obj.ChiPhi_GhiChu;
                    objUpd.TongTienHang = obj.TongTienHang;
                    objUpd.TongTienThue = obj.TongTienThue;
                    objUpd.PTThueHoaDon = obj.PTThueHoaDon;
                    objUpd.PTThueBaoHiem = obj.PTThueBaoHiem;
                    objUpd.TongTienThueBaoHiem = obj.TongTienThueBaoHiem;
                    objUpd.TongThueKhachHang = obj.TongThueKhachHang;
                    objUpd.CongThucBaoHiem = obj.CongThucBaoHiem;
                    objUpd.GiamTruThanhToanBaoHiem = obj.GiamTruThanhToanBaoHiem;
                    objUpd.TongTienBHDuyet = obj.TongTienBHDuyet;
                    objUpd.SoVuBaoHiem = obj.SoVuBaoHiem;
                    objUpd.KhauTruTheoVu = obj.KhauTruTheoVu;
                    objUpd.GiamTruBoiThuong = obj.GiamTruBoiThuong;
                    objUpd.PTGiamTruBoiThuong = obj.PTGiamTruBoiThuong;
                    objUpd.BHThanhToanTruocThue = obj.BHThanhToanTruocThue;
                    objUpd.ChoThanhToan = obj.ChoThanhToan;
                    objUpd.DiemGiaoDich = obj.DiemGiaoDich;
                    objUpd.SoLuongKhachHang = obj.SoLuongKhachHang;
                    objUpd.YeuCau = obj.YeuCau;
                    objUpd.NgayApDungGoiDV = obj.NgayApDungGoiDV;
                    objUpd.HanSuDungGoiDV = obj.HanSuDungGoiDV;
                    objUpd.NgayLapHoaDon = obj.NgayLapHoaDon;
                    objUpd.DiemKhuyenMai = obj.DiemKhuyenMai;
                    objUpd.ID_PhieuTiepNhan = obj.ID_PhieuTiepNhan;
                    objUpd.ID_Xe = obj.ID_Xe;
                    objUpd.ID_BaoHiem = obj.ID_BaoHiem;
                    objUpd.PhaiThanhToanBaoHiem = obj.PhaiThanhToanBaoHiem;
                    objUpd.TongThanhToan = obj.TongThanhToan;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NgaySua = DateTime.Now;
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                    return objUpd;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("Update_HoaDon_DatHang: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }

        public string Update_HoaDon_Chuyenhang(BH_HoaDon obj)
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
                    #region BH_HoaDon
                    BH_HoaDon objUpd = db.BH_HoaDon.Find(obj.ID);
                    objUpd.TongChiPhi = obj.TongChiPhi; //GiaTriGiam
                    objUpd.YeuCau = obj.YeuCau;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NgaySua = obj.NgaySua;
                    objUpd.NguoiTao = obj.NguoiTao;
                    objUpd.KhuyenMai_GhiChu = obj.KhuyenMai_GhiChu;
                    objUpd.DienGiai = obj.DienGiai;
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("Update_HoaDon_Chuyenhang: " + ex.Message + ex.InnerException);
                }
            }
            return strErr;
        }

        public string Update_HoaDon_ChuyenhangTL(BH_HoaDon obj)
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
                    #region BH_HoaDon
                    BH_HoaDon objUpd = db.BH_HoaDon.Find(obj.ID);
                    objUpd.TongChiPhi = obj.TongChiPhi; //GiaTriGiam
                    objUpd.TongTienHang = obj.TongTienHang; //GiaTriGiam
                    objUpd.YeuCau = obj.YeuCau;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NgayLapHoaDon = obj.NgayLapHoaDon;
                    objUpd.NgaySua = obj.NgaySua;
                    objUpd.KhuyenMai_GhiChu = obj.KhuyenMai_GhiChu;
                    objUpd.DienGiai = obj.DienGiai;
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("Update_HoaDon_ChuyenhangTL " + ex.Message + ex.InnerException);
                }
            }
            return strErr;
        }

        public string Update_HoaDon_NhaBep(BH_HoaDon obj)
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
                    #region BH_HoaDon
                    BH_HoaDon objUpd = db.BH_HoaDon.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.ID_NhanVien = obj.ID_NhanVien;
                    objUpd.ID_ViTri = obj.ID_ViTri;
                    objUpd.DienGiai = obj.DienGiai;
                    objUpd.ID_DoiTuong = obj.ID_DoiTuong;
                    objUpd.PhaiThanhToan = obj.PhaiThanhToan;
                    objUpd.TongGiamGia = obj.TongGiamGia;
                    objUpd.TongChiPhi = obj.TongChiPhi;
                    objUpd.TongTienHang = obj.TongTienHang;
                    objUpd.ChoThanhToan = obj.ChoThanhToan;
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("Update_HoaDon_NhaBep: " + ex.Message + ex.InnerException);
                }
            }
            return strErr;
        }

        public void Update_NgayLapHoaDonByID(Guid IDHoaDon, DateTime NgayLapHoaDon)
        {
            try
            {
                BH_HoaDon objUpd = db.BH_HoaDon.Find(IDHoaDon);
                objUpd.NgayLapHoaDon = NgayLapHoaDon;
                db.Entry(objUpd).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
        }
        #endregion


        #region updatechuyenhang
        public string Update_HDChuyenHang(BH_HoaDon obj)
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
                    #region BH_HoaDon
                    BH_HoaDon objUpd = db.BH_HoaDon.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.MaHoaDon = obj.MaHoaDon;
                    objUpd.NgayLapHoaDon = obj.NgayLapHoaDon;
                    objUpd.DienGiai = obj.DienGiai;
                    objUpd.ID_NhanVien = obj.ID_NhanVien;
                    objUpd.ChoThanhToan = obj.ChoThanhToan;
                    objUpd.YeuCau = obj.YeuCau;
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
        public string Update_OptinForm(OptinForm obj)
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
                    #region BH_HoaDon
                    OptinForm objUpd = db.OptinForm.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TrangThai = obj.TrangThai;
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
        public string Delete_OptinForm(OptinForm obj)
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
                    #region BH_HoaDon
                    OptinForm objUpd = db.OptinForm.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TrangThai = 3;
                    objUpd.NgaySua = DateTime.Now;
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
        public string Delete_DoiTuongOF(OF_DoiTuongPRC obj)
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
                    #region BH_HoaDon
                    OptinForm_DoiTuong objUpd = db.OptinForm_DoiTuong.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TrangThai = 3;
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
        public string Update_DoiTuongOF(OF_DoiTuongPRC obj)
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
                    #region BH_HoaDon
                    OptinForm_DoiTuong objUpd = db.OptinForm_DoiTuong.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TrangThai = 1;
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
        #endregion

        #region updatekiemkho
        public string Update_KiemKho(BH_HoaDon obj)
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
                    #region BH_HoaDon
                    BH_HoaDon objUpd = db.BH_HoaDon.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.MaHoaDon = obj.MaHoaDon;
                    objUpd.NgayLapHoaDon = obj.NgayLapHoaDon;
                    objUpd.DienGiai = obj.DienGiai;
                    objUpd.ID_NhanVien = obj.ID_NhanVien;
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
        #endregion

        #region delete
        static string CheckDelete_HoaDon(SsoftvnContext db, BH_HoaDon obj)
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

        public string Delete_HoaDon(Guid id)
        {
            var classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                BH_HoaDon objDel = db.BH_HoaDon.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        // delete ChiTietHD 
                        strErr = classhoadonchitiet.Delete_HoaDon_ChiTiet_ByIDHoaDon(objDel.ID);

                        // find QuyCT
                        var lstQuyCT = db.Quy_HoaDon_ChiTiet.Where(idHD => idHD.ID_HoaDonLienQuan == id);
                        if (lstQuyCT != null && lstQuyCT.Count() > 0)
                        {
                            // remove CT_QuyHD, Quy_HD
                            Quy_HoaDon_ChiTiet qct = lstQuyCT.ToList().FirstOrDefault();
                            db.Quy_HoaDon_ChiTiet.RemoveRange(lstQuyCT);
                            db.Quy_HoaDon.RemoveRange(db.Quy_HoaDon.Where(idQHD => idQHD.ID == qct.ID_HoaDon));
                        }

                        db.BH_HoaDon.Remove(objDel);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        strErr = string.Concat("Delete_HoaDon ", e.InnerException, e.Message);
                        CookieStore.WriteLog(strErr);
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
        ///  find HoaDon by NgaylapHoaDon if timeout and update status ChoThanhToan = null this HoaDon
        /// </summary>
        /// <param name="ngayLapHoaDon"></param>
        /// <param name="idChiNhanh"></param>
        /// <param name="loaiHoaDon"></param>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public string SP_DeleteHoaDon_WhenTimeOut(string ngayLapHoaDon, string idChiNhanh, string loaiHoaDon, string userLogin)
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
                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Add(new SqlParameter("NgayLapHoaDon", ngayLapHoaDon));
                    lstParam.Add(new SqlParameter("ID_DonVi", idChiNhanh));
                    lstParam.Add(new SqlParameter("LoaiHoaDon", loaiHoaDon));
                    lstParam.Add(new SqlParameter("UserLogin", userLogin));
                    db.Database.ExecuteSqlCommand("EXEC SP_DeleteHoaDon_whenTimeout @NgayLapHoaDon, @ID_DonVi, @LoaiHoaDon, @UserLogin ", lstParam.ToArray());
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_DeleteHoaDon_WhenTimeOut: " + ex.InnerException + ex.Message);
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        /// <summary>
        /// delete in HoaDonchiTiet, NVien ThucHien of HD, CTHD by ID_HoaDon
        /// </summary>
        /// <param name="idHoaDon"></param>
        /// <returns></returns>
        public string SP_DeleteHoaDon_byID(Guid idHoaDon)
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
                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Add(new SqlParameter("ID", idHoaDon));
                    db.Database.ExecuteSqlCommand("EXEC SP_DeleteHoaDon_byID @ID ", lstParam.ToArray());
                }
                catch (Exception ex)
                {
                    strErr = ex.Message + ex.InnerException;
                }
            }
            return strErr;
        }

        #endregion

        public string GetMaHDChuyenHang()
        {
            string format = "{0:0000}";
            string mahoadon = "CH0";
            string madv = db.BH_HoaDon.Where(p => p.MaHoaDon.Contains(mahoadon)).Where(p => p.MaHoaDon.Length == 7).OrderByDescending(p => p.MaHoaDon).Select(p => p.MaHoaDon).FirstOrDefault();
            if (madv == null)
            {
                mahoadon = mahoadon + string.Format(format, 1);
            }
            else
            {
                int tempstt = int.Parse(madv.Substring(mahoadon.Length, 4)) + 1;
                mahoadon = mahoadon + string.Format(format, tempstt);
            }
            return mahoadon;
        }

        #region Function Other
        public string GetAutoCode(int? loaiHoaDon)
        {
            string format = string.Empty;

            string mahoadon = string.Empty;
            if (loaiHoaDon.HasValue)
            {
                mahoadon = db.DM_LoaiChungTu.Where(p => p.ID == loaiHoaDon).Select(p => p.MaLoaiChungTu).FirstOrDefault();
            }
            else
            {
                mahoadon = "HDBL";
            }
            var lenMaChungTu = mahoadon.Length;
            switch (lenMaChungTu)
            {
                case 2:
                    format = "{0:0000000}";
                    break;
                case 3:
                    format = "{0:000000}";
                    break;
                case 4:
                    format = "{0:00000}";
                    break;
                case 5:
                    format = "{0:0000}";
                    break;
            }

            // maHD offline (contains ("O")
            var objTop = db.BH_HoaDon.Where(p => p.MaHoaDon.Contains(mahoadon))
                .Where(p => p.MaHoaDon.Length == 9 && p.LoaiHoaDon == loaiHoaDon && p.MaHoaDon.Contains("O") == false);
            if (objTop != null && objTop.Count() > 0)
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("LoaiHoaDon", loaiHoaDon));
                List<MaxCodeMaHoaDon> lst = db.Database.SqlQuery<MaxCodeMaHoaDon>("EXEC GetMaHoaDon_AuTo @LoaiHoaDon", paramlist.ToArray()).ToList();
                double tempstt = lst.FirstOrDefault().MaxCode + 1;
                mahoadon = mahoadon + string.Format(format, tempstt);
            }
            else
            {
                mahoadon = mahoadon + string.Format(format, 1);
            }
            return mahoadon;
        }

        public string SP_GetMaHoaDon_byTemp(int? loaiHoaDon, Guid idDonVi, DateTime ngayLapHoaDon)
        {
            string mahoadon = string.Empty;
            try
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("LoaiHoaDon", loaiHoaDon));
                lstParam.Add(new SqlParameter("ID_DonVi", idDonVi));
                lstParam.Add(new SqlParameter("NgayLapHoaDon", ngayLapHoaDon));
                var objReturn = db.Database.SqlQuery<SP_MaxCodeTemp>("EXEC GetMaHoaDonMax_byTemp @LoaiHoaDon, @ID_DonVi, @NgayLapHoaDon", lstParam.ToArray()).FirstOrDefault();
                mahoadon = objReturn.MaxCode;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ClassBH_HoaDon.SP_GetMaHoaDon_byTemp: " + ex.InnerException + ex.Message + ex.HResult);
                return string.Empty;
            }
            return mahoadon;
        }

        public string GetMaHoaDon_Copy(string mahoadonCopy)
        {
            string mahoadon = mahoadonCopy;
            try
            {
                SqlParameter param = new SqlParameter("MaHoaDon", mahoadonCopy);
                var objReturn = db.Database.SqlQuery<SP_MaxCodeTemp>("EXEC GetMaHoaDon_Copy @MaHoaDon", param).FirstOrDefault();
                mahoadon = objReturn.MaxCode;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ClassBH_HoaDon.GetMaHoaDon_Copy: " + ex.InnerException + ex.Message + ex.HResult);
            }
            return mahoadon;
        }

        public string SP_GetAutoCode(int? loaiHoaDon)
        {
            string format = string.Empty;
            string mahoadon = string.Empty;
            if (loaiHoaDon.HasValue)
            {
                mahoadon = db.DM_LoaiChungTu.Where(p => p.ID == loaiHoaDon).Select(p => p.MaLoaiChungTu).FirstOrDefault();
            }
            else
            {
                mahoadon = "HDBL";
            }
            var lenMaChungTu = mahoadon == null ? 0 : mahoadon.Length;
            switch (lenMaChungTu)
            {
                case 2:
                    format = "{0:00000000}";
                    break;
                case 3:
                    format = "{0:0000000}";
                    break;
                case 4:
                    format = "{0:000000}";
                    break;
                case 5:
                    format = "{0:00000}";
                    break;
                default:// use when lengh = 0
                    format = "{0:00000000}";
                    break;
            }
            try
            {
                SqlParameter param = new SqlParameter("LoaiHoaDon", loaiHoaDon);
                var objReturn = db.Database.SqlQuery<SP_MaxCode>("EXEC SP_GetMaHoaDon_Max @LoaiHoaDon", param).ToList();
                if (objReturn.Count() > 0)
                {
                    mahoadon = mahoadon + string.Format(format, objReturn.FirstOrDefault().MaxCode + 1);
                }
                else
                {
                    mahoadon = mahoadon + string.Format(format, 1);
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ClassBH_HoaDon.SP_GetAutoCode: " + ex.InnerException + ex.Message + ex.HResult);
                return mahoadon + string.Format(format, 1);
            }
            return mahoadon;
        }

        public string SP_GetAutoCode_HDDatHang()
        {
            string format = "{0:00000000}";
            string autoCode = "DH";

            try
            {
                var objReturn = db.Database.SqlQuery<SP_MaxCode>("EXEC SP_GetMaHDDatHang_Max");
                if (objReturn != null && objReturn.Count() > 0)
                {
                    autoCode = autoCode + string.Format(format, objReturn.FirstOrDefault().MaxCode + 1);
                }
                else
                {
                    autoCode = autoCode + string.Format(format, 1);
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ClassBH_HoaDon.SP_GetAutoCode_HDDatHang: " + ex.InnerException + ex.Message);
                return autoCode + new Random().Next(1000000);
            }
            return autoCode;
        }

        #endregion
    }

    public class BH_NhatKySDTheDTO
    {
        public Guid ID { get; set; }
        public string MaHoaDon { get; set; }
        public string MaHoaDonSQ { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string DienGiai { get; set; }
        public string SLoaiHoaDon { get; set; }
        public int LoaiHoaDon { get; set; }
        public int LoaiHoaDonSQ { get; set; }
        public double TienThe { get; set; }
        public double SoDu { get; set; }
        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
        public double? TongTienTang { get; set; }
        public double? TongTienGiam { get; set; }
    }

    public class BH_HoaDonTheNapDTO
    {
        public Guid ID { get; set; }
        public Guid? ID_DonVi { get; set; }
        public Guid? IDThuChi { get; set; }
        public int LoaiHoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public string NguoiTao { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string TenDonVi { get; set; }
        public string TenKhachHangUnsign { get; set; }
        public string TenKhachHangStartChar { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public double MucNap { get; set; } // TongChiPhi
        public double? KhuyenMaiPT { get; set; }
        public double KhuyenMaiVND { get; set; } //TongChietKhau
        public double TongTienNap { get; set; } //TongTienHang
        public double SoDuSauNap { get; set; } //TongTienThue
        public double? ThanhTien { get; set; }
        public double? ChietKhauPT { get; set; }
        public double ChietKhauVND { get; set; } //TongGiamGia
        public double PhaiThanhToan { get; set; } //PhaiThanhToan
        public double KhachDaTra { get; set; } //PhaiThanhToan
        public string MaPhieuThu { get; set; }
        public string GhiChu { get; set; }
        public bool? ChoThanhToan { get; set; }
        public string DiaChiKhachHang { get; set; }
        public string DienThoaiKhachHang { get; set; }
        public string DiaChiChiNhanh { get; set; }
        public string DienThoaiChiNhanh { get; set; }
        public Guid ID_DoiTuong { get; set; }
        public double? TienMat { get; set; }
        public double? TienGui { get; set; } // ck
        public double? TienATM { get; set; } // pos
        public bool? TaiKhoanPOS { get; set; }
        public string NhanVienThucHien { get; set; }
        public string MaNhanVienThucHien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }

        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
        public double? TongMucNapAll { get; set; }
        public double? TongKhuyenMaiAll { get; set; }
        public double? TongTienNapAll { get; set; }
        public double? TongChietKhauAll { get; set; }
        public double? SoDuSauNapAll { get; set; }
        public double? PhaiThanhToanAll { get; set; }
        public double? TienMatAll { get; set; }
        public double? TienATMAll { get; set; }
        public double? TienGuiAll { get; set; }
        public double? KhachDaTraAll { get; set; }
    }

    public class BH_HoaDonTheNapDTOXuatFile
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public double MucNap { get; set; } // TongChiPhi
        public double KhuyenMaiVND { get; set; } //TongChietKhau
        public double TongTienNap { get; set; } //TongTienHang
        public double SoDuSauNap { get; set; } //TongTienThue
        public double ChietKhauVND { get; set; } //TongGiamGia
        public double PhaiThanhToan { get; set; } //PhaiThanhToan
        public double TienMat { get; set; }
        public double TienATM { get; set; }
        public double TienGui { get; set; }
        public double KhachDaTra { get; set; } //PhaiThanhToan
        public string MaNhanVienThucHien { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }
    }

    public class BH_HoaDonDTO
    {
        public Guid? ID { get; set; }
        public Guid? ID_DonViQuiDoi { get; set; }//?
        public string MaHoaDon { get; set; }
        public string LoaiPhieu { get; set; }
        public double TyLeChuyenDoi { get; set; }
        public Guid? ID_HoaDon { get; set; }
        public double TonKho { get; set; }
        public double TienChietKhau { get; set; }
        public double SoLuong { get; set; }
        public double? GiaVon { get; set; }
        public string MaHoaDonGoc { get; set; }
        public Guid? ID_PhieuChi { get; set; }
        public Guid? ID_CheckIn { get; set; }
        public string MaPhieuChi { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgaySua { get; set; }
        public int? LoaiDoiTuong { get; set; }// nhaphang: 2.nhacungcap, 4.nhanvien
        public string TenDoiTuong { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDonVi { get; set; }
        public string DiaChiChiNhanh { get; set; }
        public string DienThoaiChiNhanh { get; set; }
        public string TenDonViChuyen { get; set; }
        public string TenDonViNhan { get; set; }
        public string TenNhanVien { get; set; }
        public string NguoiTao { get; set; }
        public string NguoiTaoHD { get; set; }
        public string DienGiai { get; set; }
        public string DienGiaiUnSign { get; set; }
        public string DienGiaiStartChars { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string DiaChiKhachHang { get; set; }
        public string TaiKhoanNganHang { get; set; }// stk of khachhang
        public string KhuVuc { get; set; }
        public string PhuongXa { get; set; }
        public string TrangThai { get; set; }
        public double TongTienHang { get; set; }
        public double TongChiPhi { get; set; }
        public string ChiPhi_GhiChu { get; set; }
        public double TongGiamGia { get; set; }
        public double TongChietKhau { get; set; }
        public double PhaiThanhToan { get; set; }
        public double TongTienThue { get; set; }
        public string TenPhongBan { get; set; }
        public Guid? ID_ViTri { get; set; }
        public virtual List<BH_HoaDon_ChiTietDTO> BH_HoaDon_ChiTiet { get; set; }
        public double KhachDaTra { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public Guid? ID_BangGia { get; set; }
        public string TenBangGia { get; set; }
        public bool? ChoThanhToan { get; set; }
        public string YeuCau { get; set; }
        public string LoaiYeuCau { get; set; }
        public string NguoiSua { get; set; }
        public Guid? ID_DoiTuong { get; set; }
        public Guid ID_DonVi { get; set; }
        public int LoaiHoaDon { get; set; }
        public int SoThuTu { get; set; }
        public double TongTienHDTra { get; set; }
        public int? LoaiHoaDonGoc { get; set; }
        public double TongTienHDDoiTra { get; set; } // tra hang + mua moi
        public string TenDoiTuongUnSign { get; set; }
        public string TenDoiTuongStartChars { get; set; }
        public double TienMat { get; set; }
        public double ChuyenKhoan { get; set; } // = tiengui
        public double TienATM { get; set; }// = tien POS (in list HoaDon)
        public string strLoaiHoaDon { get; set; }
        public double DuNoKH { get; set; }
        public Guid? ID_KhuyenMai { get; set; }
        public string KhuyenMai_GhiChu { get; set; }
        public double? KhuyeMai_GiamGia { get; set; }
        public double DiemGiaoDich { get; set; }
        public double DiemSauGD { get; set; }// use when get his tich diem (lst KhachHang) + tong tich diem of KH (InHoaDon)
        public bool TheoDoi { get; set; }
        public string HoaDon_HangHoa { get; set; }// string contain all MaHang,TenHang of HoaDon
        public string NgayApDungGoiDV { get; set; } // DD/MM/YYYY (nvarchar in SQL)
        public string HanSuDungGoiDV { get; set; }
        public double? TongThuHoaDon { get; set; } // use when XuLyHD nhung khong HoanTraTamUng
        public double? ThuTuThe { get; set; } // use when TraHang from HD (get Tien ThuTuTheGiaTri) + print HoaDon
        public DateTime? NgaySinh_NgayTLap { get; set; } // use when printHoaDon
        public double? TienDoiDiem { get; set; }
        public double? TienDatCoc { get; set; }
        public int? SoLuongKhachHang { get; set; }
        public DateTime? GioVao { get; set; }
        public DateTime? GioRa { get; set; }
        public string TenViTri { get; set; }
        public List<BH_NhanVienThucHienDTO> BH_NhanVienThucHiens { get; set; }
        public double? GiaTriSDDV { get; set; }// giatri sudung dichvu
        public Guid? ID_TaiKhoanPos { get; set; }// used to update HD (get TKNganHang)
        public Guid? ID_TaiKhoanChuyenKhoan { get; set; }
        public double? ThanhTienChuaCK { get; set; }
        public double? GiamGiaCT { get; set; }
        public string MaSoThue { get; set; }
        public string MaPhieuTiepNhan { get; set; }
        public Guid? ID_PhieuTiepNhan { get; set; }
        public Guid? ID_BaoHiem { get; set; }
        public string BienSo { get; set; }
        public Guid? ID_Xe { get; set; }
        public double? TongThanhToan { get; set; }
        public double? PhaiThanhToanBaoHiem { get; set; }
        public string LienHeBaoHiem { get; set; }
        public string SoDienThoaiLienHeBaoHiem { get; set; }
        public string TenBaoHiem { get; set; }
        public string MaBaoHiem { get; set; }
        public string BH_SDT { get; set; }
        public string BH_DiaChi { get; set; }
        public string BH_Email { get; set; }
        public string Gara_TrangThaiBG { get; set; }
        public double? ConNo { get; set; }
        public double? DaThanhToan { get; set; } // = khach + baohiem da tra
        public double? BaoHiemDaTra { get; set; }
        public double? PTThueHoaDon { get; set; }
        public double? PTThueBaoHiem { get; set; }
        public double? TongTienThueBaoHiem { get; set; }
        public int? SoVuBaoHiem { get; set; }
        public double? KhauTruTheoVu { get; set; }
        public double? PTGiamTruBoiThuong { get; set; }
        public double? GiamTruBoiThuong { get; set; }
        public double? BHThanhToanTruocThue { get; set; }
        public double? TongTienBHDuyet { get; set; }
        public double? TongThueKhachHang { get; set; }
        public int? CongThucBaoHiem { get; set; }

        public double? SumDaThanhToan { get; set; }
        public double? SumConNo { get; set; }
        public double? SumBaoHiemDaTra { get; set; }
        public double? SumTongTienHang { get; set; }
        public double? SumGiamGiaCT { get; set; }
        public double? SumTongGiamGia { get; set; }
        public double? SumKhachDaTra { get; set; }
        public double? SumTienMat { get; set; }
        public double? SumPOS { get; set; }
        public double? SumChuyenKhoan { get; set; }
        public double? SumKhuyeMai_GiamGia { get; set; }
        public double? SumTongChiPhi { get; set; }
        public double? SumTongTienThue { get; set; }
        public double? TongGiaTriSDDV { get; set; }
        public double? SumTongTongTienHDTra { get; set; }
        public double? SumPhaiThanhToan { get; set; }
        public double? SumPhaiThanhToanBaoHiem { get; set; }
        public double? SumTongThanhToan { get; set; }
        public double? SumTienDoiDiem { get; set; }
        public double? SumThuTuThe { get; set; }
        public double? SumTienCoc { get; set; }
        public double? SumThanhTienChuaCK { get; set; }
        public double? SumTongTienBHDuyet { get; set; }
        public double? SumBHThanhToanTruocThue { get; set; }
        public double? SumGiamTruBoiThuong { get; set; }
        public double? SumKhauTruTheoVu { get; set; }
        public double? SumTongTienThueBaoHiem { get; set; }
        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
    }
    public class BH_KiemKho_Excel
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public double TongGiamGia { get; set; }
        public double TongChiPhi { get; set; }
        public double TongTienHang { get; set; }
        public string TenDonVi { get; set; }
        public string DienGiai { get; set; }
        public string ChoThanhToan { get; set; }
    }
    public class HoaDonBaoHanhExcel
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public string DiaChiKhachHang { get; set; }
        public string KhuVuc { get; set; }
        public string TenDonVi { get; set; }
        public string TenNhanVien { get; set; }
        public string NguoiTao { get; set; }
        public string DienGiai { get; set; }
        public string TrangThai { get; set; }
    }

    public class HoaDonBanExcel
    {
        public string MaHoaDon { get; set; }
        public string MaHoaDonGoc { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public string DiaChiKhachHang { get; set; }
        public string KhuVuc { get; set; }
        public string TenDonVi { get; set; }
        public string TenNhanVien { get; set; }
        public string NguoiTao { get; set; }
        public double? ThanhTienChuaCK { get; set; }
        public double? GiamGiaCT { get; set; }
        public double? GiaTriSuDung { get; set; }// used to sudung gdv
        public double? TongTienHang { get; set; }
        public double? TongChiPhi { get; set; }
        public double? TongTienThue { get; set; }
        public double? TongGiamGia { get; set; }
        public double? TongPhaiTra { get; set; }
        public double? PhaiThanhToan { get; set; }//= khachcantra
        public double? KhachDaTra { get; set; }
        public double? TienMat { get; set; }
        public double? ChuyenKhoan { get; set; }
        public double? TienATM { get; set; }
        public double? TienDoiDiem { get; set; }
        public double? ThuTuThe { get; set; }
        public double? ConNo { get; set; }
        public string DienGiai { get; set; }
        public string TrangThai { get; set; }
    } 
    public class GoiDichVuExcel
    {
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string NgayApDungGoiDV { get; set; }
        public string HanSuDungGoiDV { get; set; }
        public string BienSo { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public string DiaChiKhachHang { get; set; }
        public string KhuVuc { get; set; }
        public string TenDonVi { get; set; }
        public string TenNhanVien { get; set; }
        public string NguoiTao { get; set; }
        public double? TongTienHang { get; set; }
        public double? TongTienThue { get; set; }
        public double? TongGiamGia { get; set; }
        public double? PhaiThanhToan { get; set; }//= khachcantra
        public double? KhachDaTra { get; set; }
        public double? TienMat { get; set; }
        public double? ChuyenKhoan { get; set; }
        public double? TienATM { get; set; }
        public double? TienDoiDiem { get; set; }
        public double? ThuTuThe { get; set; }
        public double? ConNo { get; set; }
        public string DienGiai { get; set; }
        public string TrangThai { get; set; }
    }

    public class BH_HoaDon_Excel
    {
        public string MaHoaDon { get; set; }
        public string MaHoaDonGoc { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string NgayApDungGoiDV { get; set; }// because get format tyep DD/MM/YYYY (nvarchar (in SQL))
        public string HanSuDungGoiDV { get; set; }
        public string MaPhieuTiepNhan { get; set; }
        public string BienSo { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public string DiaChiKhachHang { get; set; }
        public string KhuVuc { get; set; }
        public string MaBaoHiem { get; set; }
        public string TenBaoHiem { get; set; }
        public string TenDonVi { get; set; }
        public string TenNhanVien { get; set; }
        public string NguoiTao { get; set; }
        public double? ThanhTienChuaCK { get; set; }
        public double? GiamGiaCT { get; set; }
        public double? GiaTriSuDung { get; set; }// used to sudung gdv
        public double TongTienHang { get; set; }
        public double? TongChiPhi { get; set; }
        public double TongTienThue { get; set; }
        public double TongGiamGia { get; set; }
        public double TongPhaiTra { get; set; }
        public double PhaiThanhToan { get; set; }//= khachcantra
        public double KhachDaTra { get; set; }

        public double? TongTienBHDuyet { get; set; }
        public double? KhauTruTheoVu { get; set; }
        public double? GiamTruBoiThuong { get; set; }
        public double? BHThanhToanTruocThue { get; set; }
        public double? TongTienThueBaoHiem { get; set; }
        public double PhaiThanhToanBaoHiem { get; set; }
        public double BaoHiemDaTra { get; set; }
        public double? ThuTuCoc { get; set; }

        public double TienMat { get; set; }
        public double ChuyenKhoan { get; set; }
        public double TienATM { get; set; }
        public double? TienDoiDiem { get; set; }
        public double? ThuTuThe { get; set; }
        public double ConNo { get; set; }
        public string DienGiai { get; set; }
        public string TrangThai { get; set; }
    }
    public class BH_ChuyenHang_Excel
    {
        public string MaHoaDon { get; set; }
        public string TenDonViChuyen { get; set; }
        public string TenNhanVien { get; set; }
        public string TenDonVi { get; set; }
        public string TenDoiTuong { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public DateTime? NgayNhan { get; set; }
        public double TongTienHang { get; set; }
        public double TongChiPhi { get; set; }
        public string DienGiai { get; set; }
        public string YeuCau { get; set; }
    }
    public class BH_PhieuTraHang_Excel
    {
        public string MaHoaDon { get; set; }
        public string MaHoaDonGoc { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public string DiaChi { get; set; }
        public string KhuVuc { get; set; }
        public string ChiNhanh { get; set; }
        public string NguoiTraNhan { get; set; }
        public string NVLapHoaDon { get; set; }
        public double TongTienHang { get; set; }
        public double GiamGia { get; set; }
        public double TongSauGiamGia { get; set; }
        public double PhiTraHang { get; set; }
        public double? TongTienThue { get; set; }
        public double CanTraKhach { get; set; }
        public double DaTraKhach { get; set; }
        public double? ConNo { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }
    }
    public class BH_PhieuNhapHang_Excel
    {
        public string MaHoaDon { get; set; }
        public string MaHoaDonGoc { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string NguoiBan { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string TenDonVi { get; set; }
        public double? ThanhTienChuaCK { get; set; }
        public double? TongChietKhau { get; set; }
        public double TongTienHang { get; set; }
        public double? TongTienThue { get; set; }
        public double TongGiamGia { get; set; }
        public double? TongChiPhi { get; set; }
        public double PhaiThanhToan { get; set; }
        public double? TienMat { get; set; }
        public double? TienPOS { get; set; }
        public double? ChuyenKhoan { get; set; }
        public double? TienDatCoc { get; set; }
        public double DaThanhToan { get; set; }
        public double? ConNo { get; set; }
        public string DienGiai { get; set; }
        public string TrangThai { get; set; }
    }
    public class Excel_NhapKhoNoiBoDTO
    {
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string NguoiTao { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string TenDonVi { get; set; }
        public double? TongTienHang { get; set; }
        public double? TongGiamGia { get; set; }
        public double? TongChiPhi { get; set; }
        public double? PhaiThanhToan { get; set; }
        public double? DaThanhToan { get; set; }
        public double? ConNo { get; set; }
        public string DienGiai { get; set; }
        public string TrangThai { get; set; }
    }
    public class BH_PhieuTraHangNhap_Excel
    {
        public string MaHoaDon { get; set; }
        public string MaHoaDonGoc { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string TenDonVi { get; set; }
        public string NguoiTao { get; set; }
        public double TongTienHang { get; set; }
        public double? TongTienThue { get; set; }
        public double TongGiamGia { get; set; }
        public double KhachCanTra { get; set; }
        public double KhachDaTra { get; set; }
        public double ConNo { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }
    }

    public class BH_PhieuDatHang_Excel
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaKhachHang { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoaiKH { get; set; }
        public string DiaChiKH { get; set; }
        public string KhuVuc { get; set; }
        public string TenChiNhanh { get; set; }
        public string NguoiBan { get; set; }
        public string NguoiTao { get; set; }
        public double TongTienHang { get; set; }
        public double TongTienThue { get; set; }
        public double GiamGia { get; set; }
        public double TongChiPhi { get; set; }
        public double KhachCanTra { get; set; }
        public double KhachDaTra { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }
    }

    public class Excel_BaoGiaSuaChua
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaPhieuTiepNhan { get; set; }
        public string BienSoXe { get; set; }
        public string MaKhachHang { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoaiKH { get; set; }
        public string DiaChiKH { get; set; }
        public string KhuVuc { get; set; }
        public string TenChiNhanh { get; set; }
        public string NguoiBan { get; set; }
        public string NguoiTao { get; set; }
        public double TongTienHang { get; set; }
        public double TongTienThue { get; set; }
        public double GiamGia { get; set; }
        public double TongChiPhi { get; set; }
        public double KhachCanTra { get; set; }
        public double KhachDaTra { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }
    }

    public class SelectTonKho
    {
        public DateTime? NgayLapHoaDon { get; set; }
        public int LoaiHoaDon { get; set; }
        public string YeuCau { get; set; }
        public double SoLuong { get; set; }
        public bool? ChoThanhToan { get; set; }
        public Guid ID_DonVi { get; set; }
        public Guid? ID_Checkin { get; set; }
    }

    public class HD_QHD_QHDCT
    {
        public string MaPhieuChi { get; set; }
        public Guid? ID_PhieuChi { get; set; }
        public string MaHoaDonGoc { get; set; }
        public Guid? ID_HoaDonGoc { get; set; }
        public double TongTienHDTra { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public int? LoaiHoaDonGoc { get; set; }
        public string sLoaiHoaDon { get; set; }
        public double? SoDu { get; set; }
        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
    }

    public class ViTri
    {
        public Guid? ID { get; set; }
    }

    public class PageListDTO
    {
        public int TotalRecord { get; set; }
        public double PageCount { get; set; }
        public double TongTon { get; set; }
        public double TongTienHang { get; set; }
        public double TongGiamGia { get; set; }
        public double KhachDaTra { get; set; }

    }

    public class SP_MaxCode
    {
        public double MaxCode { get; set; }
    }

    public class SP_MaxCodeTemp
    {
        public string MaxCode { get; set; }
    }

    public class Excel_HisHoaDon
    {
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string NguoiTao { get; set; }
        public double GiaTri { get; set; }
        public string TrangThai { get; set; }
    }
}

public class Params_GetListHoaDon
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public double? LoaiHoaDon { get; set; }
    public string MaHoaDon { get; set; }
    public string MaHoaDonGoc { get; set; }
    public List<string> ID_ChiNhanhs { get; set; }
    public List<string> ID_ViTris { get; set; } = new List<string>() { "" };
    public List<string> ID_BangGias { get; set; } = new List<string>() { "" };
    public List<string> ID_NhanViens { get; set; }
    public double? TrangThai { get; set; }
    public DateTime? NgayTaoHD_TuNgay { get; set; }
    public DateTime? NgayTaoHD_DenNgay { get; set; }
    public int TrangThai_SapXep { get; set; }
    public string Cot_SapXep { get; set; }
    public string PTThanhToan { get; set; }
    public string NguoiTao { get; set; }
    public string ColumnsHide { get; set; } // use when export excel
    public string ValueText { get; set; } // use when export excel
    public string SortBy { get; set; } = "DESC";
    public List<string> TrangThaiHDs { get; set; } // 0.hoanthanh, 1. phieutam, 2. huy
    public List<string> PhuongThucTTs { get; set; } = new List<string>() { "" };// 1.mat, 2.pos, 3.ck, 4.the, empty: all = 0
    public List<string> LaHoaDonSuaChua { get; set; }
    public int BaoHiem { get; set; }
}


public class ModelHoaDon
{
    public int currentPage { get; set; }
    public int pageSize { get; set; }
    public string loaiHoaDon { get; set; }
    public string maHoaDon { get; set; }
    public string maHDGoc { get; set; }
    public List<string> id_ViTris { get; set; }
    public List<string> id_BangGias { get; set; }
    public List<string> arrChiNhanh { get; set; }
    public List<string> id_NhanViens { get; set; }
    public int? trangThai { get; set; }
    public DateTime dayStart { get; set; }
    public DateTime dayEnd { get; set; }
    public string id_donvi { get; set; }
    public string columsort { get; set; }
    public string sort { get; set; }
    public string time { get; set; }
    public List<string> tenchinhanh { get; set; }
    public string columnsHide { get; set; }
    public List<string> ArrTrangThai { get; set; }
    public List<string> ArrLoaiHoaDon { get; set; }
}

public class ModelNhatKySDThe
{
    public int loai { get; set; }
    public Guid iddt { get; set; }
    public int currentPage { get; set; }
    public int pageSize { get; set; }
    public DateTime dayStart { get; set; }
    public DateTime dayEnd { get; set; }
}

public class ModelLichSuNapThe
{
    public int loai { get; set; }
    public Guid iddt { get; set; }
    public int currentPage { get; set; }
    public int pageSize { get; set; }
    public DateTime dayStart { get; set; }
    public DateTime dayEnd { get; set; }
    public List<string> arrChiNhanh { get; set; }
}

public class ModelHoaDonTheNap
{
    public int currentPage { get; set; }
    public int pageSize { get; set; }
    public int loaiHoaDon { get; set; }
    public string maHoaDon { get; set; }
    public double? mucnaptu { get; set; }
    public double? mucnapden { get; set; }
    public double? khuyenmaitu { get; set; }
    public double? khuyenmaiden { get; set; }
    public int loaikhuyenmai { get; set; }
    public double? chietkhautu { get; set; }
    public double? chietkhauden { get; set; }
    public int loaichietkhau { get; set; }
    public int trangThai { get; set; }
    public DateTime dayStart { get; set; }
    public DateTime dayEnd { get; set; }
    public List<string> arrChiNhanh { get; set; }
    public string iddonvi { get; set; }
    //public string id_donvi { get; set; }
    //public string columsort { get; set; }
    //public string sort { get; set; }
    public string time { get; set; }
    //public List<string> tenchinhanh { get; set; }
    public string columnsHide { get; set; }
    public List<string> ArrLoaiHoaDon { get; set; }
}

public class SP_HoaDonAndSoQuy
{
    public Guid ID { get; set; }
    public string MaHoaDon { get; set; }
    public int LoaiHoaDon { get; set; }
    public DateTime NgayLapHoaDon { get; set; }
    public double GiaTri { get; set; }
    public int? LoaiThanhToan { get; set; }
}

public class KhachHang_TabHoaDon
{
    public Guid ID { get; set; }
    public int LoaiHoaDon { get; set; }
    public string MaHoaDon { get; set; }
    public DateTime NgayLapHoaDon { get; set; }
    public double PhaiThanhToan { get; set; }
    public string TenNhanVien { get; set; }
    public bool? ChoThanhToan { get; set; }
    public string NguoiTao { get; set; }
}

public class TonGoiDichVus
{
    public Guid ID_DoiTuong { get; set; }
    public string MaDoiTuong { get; set; }
    public List<GoiDV> GoiDVs { get; set; } = new List<GoiDV>();
}

public class GoiDV
{
    public string MaHoaDon { get; set; }
    public DateTime? NgayHetHan { get; set; }
    public List<TonGoiDichVu_ChiTiet> ListDichVu { get; set; } = new List<TonGoiDichVu_ChiTiet>();
}
public class ComBo
{
    public string MaHangHoa { get; set; }
    public int? LoaiHangHoa { get; set; }
    public Guid ID_DonViQuiDoi { get; set; }
    public List<TonGoiDichVu_ChiTiet> ListThanhPhan { get; set; } = new List<TonGoiDichVu_ChiTiet>();
}

public class TonGoiDichVu_ChiTiet
{
    public Guid ID_DonViQuiDoi { get; set; }
    public Guid? ID_LoHang { get; set; }
    public string MaHangHoa { get; set; }
    public double? SoLuong { get; set; }
    public double? DonGia { get; set; }
    public string GhiChu { get; set; }
}


public class NhatKyTichDiem
{
    public string MaHoaDon { get; set; }
    public DateTime NgayLapHoaDon { get; set; }
    public string SLoaiHoaDon { get; set; }
    public double GiaTri { get; set; }
    public double DiemGiaoDich { get; set; }
    public double DiemSauGD { get; set; }
}



