using libDM_DoiTuong;
using libDM_HangHoa;
using libQuy_HoaDon;
using libDonViQuiDoi;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Globalization;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class BH_DieuChinhController : BaseApiController
    {
        #region insert
        [HttpPost, ActionName("PostBH_DieuChinh")]
        public IHttpActionResult PostBH_DieuChinh([FromBody] JObject data, Guid ID_DonVi, Guid ID_NhanVien, Guid? ID_HoaDon, string dateTaoPhieu)
        {
            BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                    string MaHoaDon = objHoaDon.MaHoaDon;
                    if (MaHoaDon == string.Empty || MaHoaDon == "null" || MaHoaDon == null)
                    {
                        MaHoaDon = classHoaDon.GetAutoCode(objHoaDon.LoaiHoaDon);
                    }
                    #region BH_HoaDon
                    int loaiInsert = 1;
                    if (ID_HoaDon == null)
                    {
                        ID_HoaDon = Guid.NewGuid();
                        bool a = classHoaDon.Check_MaHoaDonExist(objHoaDon.MaHoaDon);
                        if (a == true)
                        {
                            return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Mã phiếu điều chỉnh đã tồn tại trên hệ thống"));
                        }
                    }
                    else
                    {
                        loaiInsert = 2;
                        bool b = classHoaDon.Check_MaHoaDonExist(objHoaDon.MaHoaDon, ID_HoaDon);
                        if (b == true)
                        {
                            return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Mã phiếu điều chỉnh đã tồn tại trên hệ thống"));
                        }
                    }
                    string YeuCau = "Hoàn thành";
                    if (objHoaDon.ChoThanhToan == true)
                        YeuCau = "Tạm lưu";
                    DateTime timeCreat;
                    try
                    {
                        timeCreat = DateTime.Parse(dateTaoPhieu).AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                    }
                    catch
                    {
                        timeCreat = DateTime.Now;
                    }
                    List<SqlParameter> prm = new List<SqlParameter>();
                    prm.Add(new SqlParameter("ID", ID_HoaDon));
                    prm.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                    prm.Add(new SqlParameter("ID_NhanVien", objHoaDon.ID_NhanVien != null ? objHoaDon.ID_NhanVien : ID_NhanVien));
                    prm.Add(new SqlParameter("MaHoaDon", MaHoaDon));
                    prm.Add(new SqlParameter("TongGiaVonHienTai", objHoaDon.TongTienHang));
                    prm.Add(new SqlParameter("TongGiaVonMoi", objHoaDon.TongChietKhau));
                    prm.Add(new SqlParameter("TongGiaVonTang", objHoaDon.TongTienThue));
                    prm.Add(new SqlParameter("TongGiaVonGiam", objHoaDon.TongChiPhi));
                    prm.Add(new SqlParameter("ChoThanhToan", objHoaDon.ChoThanhToan));
                    prm.Add(new SqlParameter("DienGiai", objHoaDon.DienGiai));
                    prm.Add(new SqlParameter("NguoiTao", objHoaDon.NguoiTao));
                    prm.Add(new SqlParameter("loaiInsert", loaiInsert));
                    prm.Add(new SqlParameter("YeuCau", YeuCau));
                    prm.Add(new SqlParameter("timeCreate", timeCreat));
                    db.Database.ExecuteSqlCommand("exec insert_DieuChinhGiaVon @ID, @ID_DonVi, @ID_NhanVien, @MaHoaDon, @TongGiaVonHienTai, @TongGiaVonMoi, @TongGiaVonTang, @TongGiaVonGiam, @ChoThanhToan, @DienGiai, @NguoiTao, @loaiInsert, @YeuCau, @timeCreate", prm.ToArray());
                    #endregion
                    string chitiet_xh = string.Empty;
                    string noidung_xh = string.Empty;
                    #region BH_HoaDon_ChiTiet

                    List<listHangHoa_DieuChinh> objHoaDon_ChiTiet = data["objHoaDonChiTiet"].ToObject<List<listHangHoa_DieuChinh>>();
                    int i = objHoaDon_ChiTiet.Count();
                    int dk_Xoa = 0;
                    foreach (var item in objHoaDon_ChiTiet)
                    {
                        DonViQuiDoi DQ = classHoaDon.getList_DonViQuyDoi(item.ID_DonViQuiDoi);
                        DM_HangHoa hanghoa = db.DM_HangHoa.Where(p => p.ID == DQ.ID_HangHoa).FirstOrDefault();
                        List<SqlParameter> slqPRM = new List<SqlParameter>();
                        slqPRM.Add(new SqlParameter("ID_HoaDon", ID_HoaDon));
                        slqPRM.Add(new SqlParameter("ID_DonViQuiDoi", item.ID_DonViQuiDoi));
                        if (item.ID_LoHang == null)
                            slqPRM.Add(new SqlParameter("ID_LoHang", DBNull.Value));
                        else
                            slqPRM.Add(new SqlParameter("ID_LoHang", item.ID_LoHang));
                        slqPRM.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                        slqPRM.Add(new SqlParameter("SoThuTu", i));
                        slqPRM.Add(new SqlParameter("GiaVonHienTai", item.GiaVonHienTai));
                        slqPRM.Add(new SqlParameter("GiaVonMoi", item.GiaVonMoi));
                        slqPRM.Add(new SqlParameter("GiaVonTang", item.GiaVonTang));
                        slqPRM.Add(new SqlParameter("GiaVonGiam", item.GiaVonGiam));
                        slqPRM.Add(new SqlParameter("loaiInsert", loaiInsert));
                        slqPRM.Add(new SqlParameter("ChoThanhToan", objHoaDon.ChoThanhToan));
                        slqPRM.Add(new SqlParameter("dk_Xoa", dk_Xoa));
                        db.Database.ExecuteSqlCommand("exec insert_DieuChinhGiaVon_ChiTiet @ID_HoaDon, @ID_DonViQuiDoi,@ID_LoHang,@ID_DonVi, @SoThuTu, @GiaVonHienTai, @GiaVonMoi, @GiaVonTang, @GiaVonGiam,@loaiInsert, @ChoThanhToan, @dk_Xoa", slqPRM.ToArray());
                        if (chitiet_xh == string.Empty)
                        {
                            chitiet_xh = "<br> - <a style=\"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + item.GhiChu + "</a> : Giá vốn hiện tại: " + string.Format("{0:#,##0.##}", item.GiaVonHienTai).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " Giá vốn mới: " + string.Format("{0:#,##0.##}", item.GiaVonMoi).Replace(".", ",");
                            noidung_xh = " - " + DQ.MaHangHoa + " : Giá vốn hiện tại: " + string.Format("{0:#,##0.##}", item.GiaVonHienTai).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " Giá vốn mới: " + string.Format("{0:#,##0.##}", item.GiaVonMoi).Replace(".", ",");
                        }
                        else
                        {
                            chitiet_xh = chitiet_xh + "<br> - <a style=\"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + item.GhiChu + "</a> : Giá vốn hiện tại: " + string.Format("{0:#,##0.##}", item.GiaVonHienTai).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " Giá vốn mới: " + string.Format("{0:#,##0.##}", item.GiaVonMoi).Replace(".", ",");
                            noidung_xh = noidung_xh + " - " + DQ.MaHangHoa + " : Giá vốn hiện tại: " + string.Format("{0:#,##0.##}", item.GiaVonHienTai).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " Giá vốn mới: " + string.Format("{0:#,##0.##}", item.GiaVonMoi).Replace(".", ",");
                        }
                        i = i - 1;
                        dk_Xoa = dk_Xoa + 1;
                    }

                    #endregion
                    objHoaDon.ID = ID_HoaDon.Value;
                    objHoaDon.MaHoaDon = MaHoaDon;
                    objHoaDon.NgayLapHoaDon = timeCreat;
                    string str = CookieStore.GetCookieAes("SubDomain");
                    Guid? idHoaDon = null;
                    if (objHoaDon.ChoThanhToan == false)
                    {
                        idHoaDon = objHoaDon.ID;
                    }
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                    {
                        ID = Guid.NewGuid(),
                        ID_HoaDon = idHoaDon,
                        LoaiHoaDon = 18,
                        ThoiGianUpdateGV = objHoaDon.NgayLapHoaDon,
                        ID_NhanVien = ID_NhanVien,
                        ID_DonVi = objHoaDon.ID_DonVi,
                        ChucNang = "Phiếu điều chỉnh",
                        ThoiGian = DateTime.Now,
                        NoiDung = YeuCau + " phiếu điều chỉnh: " + MaHoaDon + " bao gồm: " + noidung_xh,
                        NoiDungChiTiet = YeuCau + " phiếu điều chỉnh: <a style= \"cursor: pointer\" onclick = \"FindDieuChinh('" + MaHoaDon + "')\" >" + MaHoaDon + "</a>, thời gian: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ", bao gồm: " + chitiet_xh,
                        LoaiNhatKy = 1
                    };
                    string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                    BH_HoaDonDTO objReturn = new BH_HoaDonDTO
                    {
                        ID = objHoaDon.ID,
                        MaHoaDon = objHoaDon.MaHoaDon,
                        ID_NhanVien = objHoaDon.ID_NhanVien,
                        DienGiai = objHoaDon.DienGiai,
                        NgayLapHoaDon = objHoaDon.NgayLapHoaDon,
                        TongChietKhau = objHoaDon.TongChietKhau,
                        TongChiPhi = objHoaDon.TongChiPhi,
                        TongTienHang = objHoaDon.TongTienHang,
                        ChoThanhToan = objHoaDon.ChoThanhToan,
                        ID_DonVi = objHoaDon.ID_DonVi,
                        LoaiHoaDon = objHoaDon.LoaiHoaDon,
                        TongTienThue = objHoaDon.TongTienThue
                    };
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult Post_PhieuDieuChinh([FromBody] JObject data, Guid? idNhanVien = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    try
                    {
                        ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                        ClassBH_HoaDon_ChiTiet classHoaDonCT = new ClassBH_HoaDon_ChiTiet(db);
                        classDonViQuiDoi classQuiDoi = new classDonViQuiDoi(db);
                        string err = string.Empty;
                        string noidung = "";
                        string chitiet = "";

                        BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                        List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();

                        var ngaylapHD = objHoaDon.NgayLapHoaDon;
                        var newHD = objHoaDon;
                        newHD.ID = Guid.NewGuid();
                        newHD.NgayLapHoaDon = ngaylapHD;
                        string sMaHoaDon = string.Empty;
                        if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                        {
                            sMaHoaDon = classHoaDon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                        }
                        else
                        {
                            bool exist = classHoaDon.Check_MaHoaDonExist(objHoaDon.MaHoaDon);
                            if (exist)
                            {
                                return Json(new { res = false, mes = "Mã hóa đơn đã tồn tại" });
                            }
                            sMaHoaDon = classHoaDon.GetMaHoaDon_Copy(objHoaDon.MaHoaDon);
                        }
                        newHD.MaHoaDon = sMaHoaDon;
                        newHD.NgayTao = DateTime.Now;
                        newHD.TyGia = 1;
                        err = classHoaDon.Add_HoaDon(newHD);
                        if (err == string.Empty)
                        {
                            foreach (var item in objCTHoaDon)
                            {
                                var malo = item.MaLoHang != null && item.MaLoHang != string.Empty ? string.Concat("(Lô: ", item.MaLoHang, ") : Giá vốn cũ: ") : " Giá vốn cũ:  ";
                                chitiet = string.Concat(chitiet, "- <a onclick=\"FindMaHangHoa('", item.MaHangHoa, "')\">" + item.MaHangHoa, " </a> "
                                    , malo, item.DonGia, ", Giá vốn mới: ", item.GiaVon, "</br>");

                                BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                    SoThuTu = item.SoThuTu,
                                    DonGia = item.DonGia,
                                    GiaVon = item.GiaVon,
                                    ID_HoaDon = newHD.ID,
                                    TienChietKhau = item.TienChietKhau,
                                    PTChietKhau = item.PTChietKhau,
                                    ID_LoHang = item.ID_LoHang == null ? null : item.ID_LoHang,
                                    GhiChu = item.GhiChu,
                                    TenHangHoaThayThe = item.TenHangHoaThayThe
                                };
                                err = classHoaDonCT.Add_ChiTietHoaDon(ctHoaDon);
                            }

                            #region nhatky
                            string tenChucNang = string.Empty;
                            string txtFirst = string.Empty;

                            switch (objHoaDon.LoaiHoaDon)
                            {
                                case 16:
                                    tenChucNang = "Giá vốn tiêu chuẩn";
                                    if (objHoaDon.ChoThanhToan.Value == false)
                                    {
                                        txtFirst = "Nhập giá vốn tiêu chuẩn: ";
                                    }
                                    else
                                    {
                                        txtFirst = "Lưu tạm phiếu nhập giá vốn tiêu chuẩn: ";
                                    }
                                    break;
                                case 18:
                                    tenChucNang = "Điều chỉnh giá vốn";
                                    if (objHoaDon.ChoThanhToan.Value == false)
                                    {
                                        txtFirst = "Tạo mới phiếu điều chỉnh giá vốn: ";
                                    }
                                    else
                                    {
                                        txtFirst = "Lưu tạm phiếu điều chỉnh giá vốn: ";
                                    }
                                    break;
                            }
                            noidung = string.Concat(txtFirst, sMaHoaDon, " Giá trị: ", ", Thời gian: ", ngaylapHD.ToString("dd/MM/yyy HH:mm:ss"));
                            chitiet = string.Concat(noidung, " bao gồm: <br />", chitiet);

                            HT_NhatKySuDung nky = new HT_NhatKySuDung
                            {
                                ID = Guid.NewGuid(),
                                ID_DonVi = objHoaDon.ID_DonVi,
                                LoaiHoaDon = objHoaDon.LoaiHoaDon,
                                ID_NhanVien = idNhanVien ?? objHoaDon.ID_NhanVien,
                                ChucNang = tenChucNang,
                                LoaiNhatKy = 1,
                                NoiDung = noidung,
                                NoiDungChiTiet = chitiet,
                                ThoiGian = DateTime.Now,
                                ID_HoaDon = newHD.ID,
                                ThoiGianUpdateGV = newHD.NgayLapHoaDon,
                            };
                            db.HT_NhatKySuDung.Add(nky);
                            db.SaveChanges();
                            if (objHoaDon.ChoThanhToan.Value == false)
                            {
                                new SaveDiary().AddQueueJob(nky);
                            }

                            #endregion
                            trans.Commit();
                            return Json(new { res = true, data = new { ID = newHD.ID, MaHoaDon = sMaHoaDon, NgayLapHoaDon = ngaylapHD, ID_DoiTuong = newHD.ID_DoiTuong, ID_NhanVien = newHD.ID_NhanVien } });
                        }
                        else
                        {
                            return Json(new { res = false, mes = err });
                        }
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        return Json(new { res = false, mes = e.InnerException + e.Message });
                    }
                }
            }
        }
        #endregion
        #region update
        [HttpPost, ActionName("deleteHoaDonDieuChinh")]
        public IHttpActionResult deleteHoaDonDieuChinh(Guid ID_DonVi, Guid ID_NhanVien, Guid ID_HoaDon)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var ChoThanhToan = db.BH_HoaDon.Where(x => x.ID == ID_HoaDon).FirstOrDefault().ChoThanhToan;
                    var NgayLapHoaDon = db.BH_HoaDon.Where(x => x.ID == ID_HoaDon).FirstOrDefault().NgayLapHoaDon;
                    var MaHoaDon = db.BH_HoaDon.Where(x => x.ID == ID_HoaDon).FirstOrDefault().MaHoaDon;
                    List<SqlParameter> sqlPRM = new List<SqlParameter>();
                    sqlPRM.Add(new SqlParameter("ID_HoaDon", ID_HoaDon));
                    db.Database.ExecuteSqlCommand("exec deleteHoaDonDieuChinh @ID_HoaDon", sqlPRM.ToArray());
                    //update giá vốn
                    string str = CookieStore.GetCookieAes("SubDomain");
                    Guid? idHoaDon = null;
                    if (ChoThanhToan == false)
                    {
                        idHoaDon = ID_HoaDon;
                        //Thread st1 = new Thread(() => classHoaDon.UpdateGiaVonDM_GiaVonLT(ID_HoaDon, ID_DonVi, NgayLapHoaDon, 3, str)); //1:Thêm mới, 2:Cập nhật, 3:Xóa //chạy qua khi hàm đang chạy
                        //st1.Start();
                    }
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                    {
                        ID = Guid.NewGuid(),
                        ID_HoaDon = idHoaDon,
                        LoaiHoaDon = 18,
                        ThoiGianUpdateGV = NgayLapHoaDon,
                        ID_NhanVien = ID_NhanVien,
                        ID_DonVi = ID_DonVi,
                        ChucNang = "Phiếu điều chỉnh",
                        ThoiGian = DateTime.Now,
                        NoiDung = "Hủy bỏ phiếu điều chỉnh" + MaHoaDon,
                        NoiDungChiTiet = "Hủy bỏ phiếu điều chỉnh: <a style= \"cursor: pointer\" onclick = \"FindDieuChinh('" + MaHoaDon + "')\" >" + MaHoaDon + "</a>, thời gian: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                        LoaiNhatKy = 4
                    };
                    string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, ID_HoaDon));
                }
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion
        #region Select
        [HttpGet, HttpPost]
        public IHttpActionResult GetListGiaVonTieuChuan_ChiTiet(CommonParamSearch param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassBH_HoaDon_ChiTiet classHoaDonCT = new ClassBH_HoaDon_ChiTiet(db);
                    List<PhieuDieuChinhChiTietDTO> data = classHoaDonCT.GetListGiaVonTieuChuan_ChiTiet(param);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException+ ex.Message);
                }
            }
        } 
        [HttpGet, HttpPost]
        public IHttpActionResult GetListGiaVonTieuChuan_TongHop(CommonParamSearch param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassBH_HoaDon_ChiTiet classHoaDonCT = new ClassBH_HoaDon_ChiTiet(db);
                    List<PhieuDieuChinhDTO> data = classHoaDonCT.GetListGiaVonTieuChuan_TongHop(param);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException+ ex.Message);
                }
            }
        }

        [HttpGet]
        public IHttpActionResult getListHangHoaBy_MaHangHoa(string MaHangHoa)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> sqlPRM = new List<SqlParameter>();
                sqlPRM.Add(new SqlParameter("MaHangHoa", MaHangHoa));
                List<List_DonViQuiDoi_HH> lst = db.Database.SqlQuery<List_DonViQuiDoi_HH>("exec getList_HangHoabyMaHH @MaHangHoa", sqlPRM.ToArray()).ToList();
                JsonResultExample<List_DonViQuiDoi_HH> jsonResult = new JsonResultExample<List_DonViQuiDoi_HH>
                {
                    LstData = lst
                };
                return Json(jsonResult);
            }
        }
        [HttpGet]
        public IHttpActionResult getListHangHoaBy_IDNhomHang(string ID_NhomHang, Guid ID_DonVi, int STT)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> sqlPRM = new List<SqlParameter>();
                sqlPRM.Add(new SqlParameter("ID_NhomHang", ID_NhomHang));
                sqlPRM.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                sqlPRM.Add(new SqlParameter("STT", STT));
                List<List_DonViQuiDoi_ID_NhomHang> lst = db.Database.SqlQuery<List_DonViQuiDoi_ID_NhomHang>("exec getListHangHoaBy_IDNhomHang @ID_NhomHang, @ID_DonVi, @STT", sqlPRM.ToArray()).ToList();
                JsonResultExample<List_DonViQuiDoi_ID_NhomHang> jsonResult = new JsonResultExample<List_DonViQuiDoi_ID_NhomHang>
                {
                    LstData = lst
                };
                return Json(jsonResult);
            }
        }
        public List<ListLHPages> getAllPage<T>(List<T> lstLHs, float PageSize)
        {
            List<ListLHPages> listPage = new List<ListLHPages>();
            if (lstLHs != null)
            {
                int dem = 1;
                float SoTrang = lstLHs.Count / PageSize;
                for (int i = 0; i < SoTrang; i++)
                {
                    ListLHPages LH_page = new ListLHPages();
                    LH_page.SoTrang = dem;
                    listPage.Add(LH_page);
                    dem = dem + 1;
                }
                return listPage;
            }
            else
            {
                return null;
            }
        }
        [HttpGet]
        public IHttpActionResult getList_HoaDonDieuChinh(string MaHoaDon, int PageSize, int PageNumber, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3, string chinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<HD_DieuChinhPRC> lst = new List<HD_DieuChinhPRC>();
                string maHD_search = string.Empty;
                if (MaHoaDon != null & MaHoaDon != "" & MaHoaDon != "null")
                    maHD_search = "%" + MaHoaDon + "%";
                else
                    maHD_search = "%%";
                string TrangThai1_seach = "%" + trangthai1 + "%";
                string TrangThai2_seach = "%" + trangthai2 + "%";
                string TrangThai3_seach = "%" + trangthai3 + "%";
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_ChiNhanh", chinhanh));
                paramlist.Add(new SqlParameter("MaHoaDon", maHD_search));
                paramlist.Add(new SqlParameter("timeStart", dayStart));
                paramlist.Add(new SqlParameter("timeEnd", dayEnd));
                paramlist.Add(new SqlParameter("TrangThai1", TrangThai1_seach));
                paramlist.Add(new SqlParameter("TrangThai2", TrangThai2_seach));
                paramlist.Add(new SqlParameter("TrangThai3", TrangThai3_seach));
                lst = db.Database.SqlQuery<HD_DieuChinhPRC>("exec getList_HoaDonDieuChinh @ID_ChiNhanh, @MaHoaDon, @timeStart, @timeEnd, @TrangThai1, @TrangThai2, @TrangThai3", paramlist.ToArray()).ToList();
                int rows = lst.Count();
                double giavontang = lst.Sum(x => (double?)x.TongGiaVonTang ?? 0);
                double giavongiam = lst.Sum(x => (double?)x.TongGiaVonGiam ?? 0);
                List<ListLHPages> lstPage = getAllPage<HD_DieuChinhPRC>(lst, PageSize);
                List<HD_DieuChinhPRC> lstXNT = lst.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
                JsonResultExample<HD_DieuChinhPRC> jsonobj = new JsonResultExample<HD_DieuChinhPRC>
                {
                    Rowcount = rows,
                    LstPageNumber = lstPage,
                    LstData = lstXNT,
                    _thanhtien = Math.Round(giavontang, 0, MidpointRounding.ToEven),
                    _tienvon = Math.Round(giavongiam, 0, MidpointRounding.ToEven),
                };
                return Json(jsonobj);
            }
        }
        [HttpGet]
        public void Export_HoaDonDieuChinh(string MaHoaDon, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3, string ID_ChiNhanh, string columnsHide, string time, string ChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument classOffice = new Class_officeDocument(db);

                List<HD_DieuChinhPRC> lst = new List<HD_DieuChinhPRC>();
                string maHD_search = string.Empty;
                if (MaHoaDon != null & MaHoaDon != "" & MaHoaDon != "null")
                    maHD_search = "%" + MaHoaDon + "%";
                else
                    maHD_search = "%%";
                string TrangThai1_seach = "%" + trangthai1 + "%";
                string TrangThai2_seach = "%" + trangthai2 + "%";
                string TrangThai3_seach = "%" + trangthai3 + "%";
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                paramlist.Add(new SqlParameter("MaHoaDon", maHD_search));
                paramlist.Add(new SqlParameter("timeStart", dayStart));
                paramlist.Add(new SqlParameter("timeEnd", dayEnd));
                paramlist.Add(new SqlParameter("TrangThai1", TrangThai1_seach));
                paramlist.Add(new SqlParameter("TrangThai2", TrangThai2_seach));
                paramlist.Add(new SqlParameter("TrangThai3", TrangThai3_seach));
                lst = db.Database.SqlQuery<HD_DieuChinhPRC>("exec getList_HoaDonDieuChinh @ID_ChiNhanh, @MaHoaDon, @timeStart, @timeEnd, @TrangThai1, @TrangThai2, @TrangThai3", paramlist.ToArray()).ToList();
                DataTable excel = classOffice.ToDataTable<HD_DieuChinhPRC>(lst);
                excel.Columns.Remove("ID_HoaDon");
                excel.Columns.Remove("ID_NhanVien");
                excel.Columns.Remove("ID_DonVi");
                excel.Columns.Remove("ChoThanhToan");
                excel.Columns.Remove("NguoiTao");
                excel.Columns.Remove("NguoiDieuChinh");
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/BaoCao/Teamplate_PhieuDieuChinhGiaVon.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/BaoCao/PhieuDieuChinhGiaVon.xlsx");
                fileSave = classOffice.createFolder_Download(fileSave);
                classOffice.listToOfficeExcel_Stype(fileTeamplate, fileSave, excel, 4, 28, 24, true, columnsHide, time, ChiNhanh);
                HttpResponse Response = HttpContext.Current.Response;
                classOffice.downloadFile(fileSave);
            }
        }
        [HttpGet]
        public IHttpActionResult getList_HoaDonDieuChinhChiTiet(Guid ID_HoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<HD_DieuChinhChiTietPRC> lst = new List<HD_DieuChinhChiTietPRC>();
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_HoaDon", ID_HoaDon));
                lst = db.Database.SqlQuery<HD_DieuChinhChiTietPRC>("exec getList_HoaDonDieuChinh_ChiTiet @ID_HoaDon", paramlist.ToArray()).ToList();
                double giavontang = lst.Sum(x => (double?)x.GiaVonTang ?? 0);
                double giavongiam = lst.Sum(x => (double?)x.GiaVonGiam ?? 0);
                JsonResultExample<HD_DieuChinhChiTietPRC> jsonobj = new JsonResultExample<HD_DieuChinhChiTietPRC>
                {
                    LstData = lst,
                    _thanhtien = giavontang,
                    _tienvon = giavongiam,
                };
                return Json(jsonobj);
            }
        }
        [HttpGet]
        public void Export_HoaDonDieuChinh_ChiTiet(Guid ID_HoaDon, string columnsHide, string time, string ChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument classOffice = new Class_officeDocument(db);
                List<HD_DieuChinhChiTietPRC> lst = new List<HD_DieuChinhChiTietPRC>();
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_HoaDon", ID_HoaDon));
                lst = db.Database.SqlQuery<HD_DieuChinhChiTietPRC>("exec getList_HoaDonDieuChinh_ChiTiet @ID_HoaDon", paramlist.ToArray()).ToList();
                DataTable excel = classOffice.ToDataTable<HD_DieuChinhChiTietPRC>(lst);
                excel.Columns.Remove("ID_DonViQuiDoi");
                excel.Columns.Remove("ID_LoHang");
                excel.Columns.Remove("QuanLyTheoLoHang");
                excel.Columns.Remove("TenHangHoa");
                excel.Columns.Remove("ThuocTinh_GiaTri");
                excel.Columns.Remove("TenDonViTinh");
                excel.Columns.Remove("TenLoHang");
                excel.Columns.Remove("NgaySanXuat");
                excel.Columns.Remove("NgayHetHan");
                excel.Columns.Remove("GiaVonTang");
                excel.Columns.Remove("GiaVonGiam");
                excel.Columns.Remove("ChenhLech");
                excel.Columns.Remove("SoThuTu");
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/BaoCao/Teamplate_ChiTietPhieuDieuChinhGiaVon.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/BaoCao/ChiTietPhieuDieuChinhGiaVon.xlsx");
                fileSave = classOffice.createFolder_Download(fileSave);
                classOffice.listToOfficeExcel_Stype(fileTeamplate, fileSave, excel, 4, 28, 24, true, columnsHide, time, ChiNhanh);
                HttpResponse Response = HttpContext.Current.Response;
                classOffice.downloadFile(fileSave);
            }
        }
        #endregion
        #region import
        //import danh sách hàng hóa
        [HttpPost]
        public IHttpActionResult importExcelDieuChinh()
        {
            string result = "";
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string str = classOffice.CheckFileMau_DieuChinh(excelstream);
                            if (str == null)
                            {
                                abc = classOffice.checkExcel_DieuChinh(excelstream);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpPost]
        public IHttpActionResult getList_DanhSachHangDieuChinh(Guid ID_ChiNhanh)
        {
            string result = "";
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        List<List_DonViQuiDoi_ID_NhomHang> abc = new List<List_DonViQuiDoi_ID_NhomHang>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            abc = classOffice.getList_DanhSachHangDieuChinh(excelstream, ID_ChiNhanh);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        #endregion
    }
}
