using System;
using System.Web.Http;
using Model;
using System.Web.Http.Description;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using libDM_DoiTuong;
using System.Linq;
using libQuy_HoaDon;
using libHT_NguoiDung;
using libDM_Kho;
using System.Data.Entity;
using System.IO;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel;
using System.Web.UI;
using System.Data;
using static libQuy_HoaDon.Class_Report;
using System.Data.SqlClient;
using Model.Service.common;
using System.Threading;
using libDM_HangHoa;
using libDonViQuiDoi;
using System.Globalization;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class BH_XuatHuyAPIController : BaseApiController
    {
        [HttpPost, HttpPut]
        public IHttpActionResult PutBH_XuatKho([FromBody] JObject data, Guid ID_NhanVien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                        ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                        ClassXuatHuy _classXH = new ClassXuatHuy(db);
                        classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                        BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                        List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();

                        DateTime timeCreat = objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                        string csER = _classXH.checkNgayChotSo(objHoaDon.ID_DonVi, Guid.NewGuid(), timeCreat, "tạo");
                        if (!string.IsNullOrEmpty(csER))
                        {
                            return Json(new
                            {
                                res = false,
                                mes = csER
                            });
                        }

                        bool maExist = classHoaDon.Check_MaHoaDonExist(objHoaDon.MaHoaDon, objHoaDon.ID);
                        if (maExist)
                        {
                            return Json(new
                            {
                                res = false,
                                mes = "Mã hóa đơn đã tồn tại"
                            });
                        }

                        #region BH_HoaDon
                        string YeuCau = "Hoàn thành";
                        switch (objHoaDon.ChoThanhToan)
                        {
                            case false:
                                objHoaDon.YeuCau = "Hoàn thành";
                                break;
                            case true:
                                objHoaDon.YeuCau = "Tạm lưu";
                                break;
                            default:
                                objHoaDon.YeuCau = "Hủy bỏ";
                                break;
                        }
                        BH_HoaDon hd = db.BH_HoaDon.Find(objHoaDon.ID);
                        DateTime ngayLapOld = hd.NgayLapHoaDon;
                        objHoaDon.NgayLapHoaDon = timeCreat;
                        if (string.IsNullOrEmpty(objHoaDon.MaHoaDon))
                        {
                            objHoaDon.MaHoaDon = classHoaDon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                        }
                        classHoaDon.Update_HoaDon(objHoaDon);
                        #endregion

                        #region BH_HoaDon_ChiTiet
                        string sErr = classhoadonchitiet.Delete_HoaDon_ChiTiet_ByIDHoaDon(objHoaDon.ID);
                        if (!string.IsNullOrEmpty(sErr))
                        {
                            return Json(new
                            {
                                res = false,
                                mes = sErr
                            });
                        }
                        foreach (var item in objCTHoaDon)
                        {
                            BH_HoaDon_ChiTiet ct = new BH_HoaDon_ChiTiet()
                            {
                                ID = Guid.NewGuid(),
                                ID_HoaDon = objHoaDon.ID,
                                ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                ID_LoHang = item.ID_LoHang,
                                SoThuTu = item.SoThuTu,
                                SoLuong = item.SoLuong,
                                GiaVon = item.GiaVon,
                                DonGia = item.GiaVon ?? 0,
                                ThanhTien = item.ThanhTien,
                                GhiChu = item.GhiChu,
                                ID_ChiTietGoiDV = item.ID_ChiTietGoiDV,
                                ID_ChiTietDinhLuong = item.ID_ChiTietDinhLuong,
                                ChatLieu = item.ChatLieu,
                            };
                            db.BH_HoaDon_ChiTiet.Add(ct);

                            foreach (var tpdl in item.ThanhPhan_DinhLuong)
                            {
                                BH_HoaDon_ChiTiet cttp = new BH_HoaDon_ChiTiet()
                                {
                                    ID = Guid.NewGuid(),
                                    ID_HoaDon = objHoaDon.ID,
                                    ID_DonViQuiDoi = tpdl.ID_DonViQuiDoi,
                                    ID_LoHang = tpdl.ID_LoHang,
                                    ID_ChiTietDinhLuong = ct.ID,
                                    SoThuTu = tpdl.SoThuTu,
                                    SoLuong = tpdl.SoLuong,
                                    GiaVon = tpdl.GiaVon,
                                    DonGia = tpdl.GiaVon ?? 0,
                                    ThanhTien = tpdl.ThanhTien,
                                    GhiChu = tpdl.GhiChu,
                                    ID_ChiTietGoiDV = tpdl.ID_ChiTietGoiDV,
                                    ChatLieu = tpdl.ChatLieu,
                                };
                                db.BH_HoaDon_ChiTiet.Add(cttp);
                            }
                        }
                        #endregion
                        db.SaveChanges();
                        trans.Commit();

                        return Json(new
                        {
                            res = true,
                            data = new
                            {
                                objHoaDon.ID,
                                objHoaDon.MaHoaDon,
                                objHoaDon.ID_NhanVien,
                                objHoaDon.DienGiai,
                                objHoaDon.ID_DoiTuong,
                                objHoaDon.NgayLapHoaDon,
                                objHoaDon.ChoThanhToan,
                                objHoaDon.ID_DonVi,
                                objHoaDon.LoaiHoaDon,
                                NgayLapHoaDonOld = ngayLapOld,
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        return Json(new
                        {
                            res = false,
                            mes = e.InnerException + e.Message
                        });
                    }
                }
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult UpdateAgain_XuatKho([FromBody] JObject data, Guid? idNhanVien)
        {
            string style1 = "<a style= \"cursor: pointer\" onclick = \"";
            string style2 = "('";
            string style3 = "')\" >";
            string style4 = "</a>";
            string styleMaHD = string.Empty;
            string styleHangHoa = string.Empty;
            string inforOld = string.Empty;
            string err = string.Empty;

            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                        List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                        ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);

                        bool maExist = classhoadon.Check_MaHoaDonExist(objHoaDon.MaHoaDon, objHoaDon.ID);
                        if (maExist)
                        {
                            return Json(new
                            {
                                res = false,
                                mes = "Mã hóa đơn đã tồn tại"
                            });
                        }


                        ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                        DateTime ngaylapHD = objHoaDon.NgayLapHoaDon.AddMilliseconds(DateTime.Now.Millisecond);
                        BH_HoaDon hdOld = db.BH_HoaDon.Find(objHoaDon.ID);
                        var ngayLapOld = hdOld.NgayLapHoaDon;

                        #region "Get cthd old was delete"
                        var cthdOld = classhoadonchitiet.Gets(x => x.ID_HoaDon == objHoaDon.ID); // get cthd old
                                                                                                 // if date new < date old: date old = date new - milisencond
                        string sDateOld = hdOld.NgayLapHoaDon.ToString("yyyy-MM-dd HH:mm");
                        string sDateNew = objHoaDon.NgayLapHoaDon.ToString("yyyy-MM-dd HH:mm");
                        if (string.Compare(sDateNew, sDateOld) == 0)
                        {
                            ngaylapHD = hdOld.NgayLapHoaDon;
                        }
                        // compare cthd old & new --> get cthd was delete
                        var cthdDelete = (from ctold in cthdOld
                                          join ctnew in objCTHoaDon on
                                          new { ctold.ID_DonViQuiDoi, ctold.ID_LoHang }
                                          equals new { ctnew.ID_DonViQuiDoi, ctnew.ID_LoHang }
                                          into ctDelete
                                          from de in ctDelete.DefaultIfEmpty()
                                          where de == null
                                          select ctold).ToList();

                        var ctDelete_newID = cthdDelete.Select(x =>
                       new BH_HoaDon_ChiTiet
                       {
                           ID = Guid.NewGuid(),
                           ID_HoaDon = x.ID_HoaDon,
                           ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                           ID_LoHang = x.ID_LoHang,
                           SoLuong = x.SoLuong,
                           PTChietKhau = x.PTChietKhau,
                           TienChietKhau = x.TienChietKhau,
                           ThanhTien = x.ThanhTien,
                           ThanhToan = x.ThanhToan,
                           ChatLieu = "5", // ct delete assign chatlie="5" !important
                           GiaVon = x.GiaVon,
                           TienThue = x.TienThue,
                           PTChiPhi = x.PTChiPhi,
                           TienChiPhi = x.TienChiPhi,
                       }).ToList();
                        // style diary inforHD old
                        try
                        {
                            inforOld = " <b> Thông tin hóa đơn cũ: </b> <br /> " + db.Database.SqlQuery<string>(" SELECT dbo.Diary_GetInforOldInvoice('" + objHoaDon.ID + "')").First();
                        }
                        catch (Exception e)
                        {
                            CookieStore.WriteLog("Diary_GetInforOldInvoice " + e.InnerException + e.Message);
                        }
                        #endregion

                        string YeuCau = "Hoàn thành";
                        switch (objHoaDon.ChoThanhToan)
                        {
                            case false:
                                objHoaDon.YeuCau = "Hoàn thành";
                                break;
                            case true:
                                objHoaDon.YeuCau = "Tạm lưu";
                                break;
                            default:
                                objHoaDon.YeuCau = "Hủy bỏ";
                                break;
                        }

                        objHoaDon.NgayLapHoaDon = ngaylapHD;
                        var objUp = classhoadon.Update_HoaDon_DatHang(objHoaDon);

                        styleMaHD = string.Concat(style1, "LoadHoaDon_byMaHD", style2,
                   objUp.MaHoaDon, style3, objUp.MaHoaDon, style4);

                        // delete all CTHD with ID_HoaDon
                        err = classhoadonchitiet.Delete_HoaDon_ChiTiet_ByIDHoaDon(objHoaDon.ID);
                        if (err != string.Empty)
                        {
                            CookieStore.WriteLog("Delete_HoaDon_ChiTiet_ByIDHoaDon " + err);
                            return Json(new
                            {
                                res = false,
                                mes = err,
                            });
                        }

                        #region " BH_HoaDon_ChiTiet "

                        // insert again cthd old was delete (ChatLieu = 5) into hdUpdate --> caculator TonLuyKe
                        err = classhoadonchitiet.Add_ChiTietHoaDon(ctDelete_newID);

                        if (err != string.Empty)
                        {
                            return Json(new
                            {
                                res = false,
                                mes = err,
                            });
                        }

                        foreach (var item in objCTHoaDon)
                        {
                            var sMaLoHang = string.Empty;

                            List<BH_HoaDon_ChiTiet> lstCT = new List<BH_HoaDon_ChiTiet>();
                            BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                SoThuTu = item.SoThuTu,
                                DonGia = item.DonGia,
                                GiaVon = item.GiaVon,
                                ID_HoaDon = objHoaDon.ID,
                                SoLuong = item.SoLuong,
                                ThanhTien = item.ThanhTien,
                                ThanhToan = item.ThanhToan,
                                PTChietKhau = item.PTChietKhau,
                                TienChietKhau = item.TienChietKhau,
                                GhiChu = item.GhiChu,
                                TangKem = item.TangKem,
                                ID_TangKem = item.ID_TangKem,
                                ID_KhuyenMai = item.ID_KhuyenMai,
                                ID_LoHang = item.ID_LoHang,
                                ID_ChiTietGoiDV = item.ID_ChiTietGoiDV,
                                ID_ChiTietDinhLuong = item.ID_ChiTietDinhLuong,
                                PTThue = item.PTThue,
                                TienThue = item.TienThue,
                                LoaiThoiGianBH = item.LoaiThoiGianBH,
                                ThoiGianBaoHanh = item.ThoiGianBaoHanh,
                                ThoiGianThucHien = item.ThoiGianThucHien,
                                ID_ViTri = item.ID_ViTri,
                                ThoiGian = item.ThoiGian,
                                ThoiGianHoanThanh = item.ThoiGianHoanThanh,
                                QuaThoiGian = item.QuaThoiGian,
                                ChatLieu = item.ChatLieu,// 1.TraHD, 2.TraGoiDV, 3.HDXuLy DH, 4.Sudung GoiDV, else: empty/null
                                DiemKhuyenMai = item.DiemKhuyenMai,
                            };

                            #region DinhLuong_DichVu

                            // get TP_DinhLuong from .js
                            if (item.ThanhPhan_DinhLuong != null && item.ThanhPhan_DinhLuong.Count > 0)
                            {
                                foreach (var itemDL in item.ThanhPhan_DinhLuong)
                                {
                                    BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                        ID_LoHang = itemDL.ID_LoHang,
                                        SoLuong = itemDL.SoLuong,
                                        GiaVon = itemDL.GiaVon,
                                        ID_ChiTietDinhLuong = ctHoaDon.ID,
                                        ID_HoaDon = objHoaDon.ID,
                                        ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV,
                                        GhiChu = itemDL.GhiChu,
                                        ChatLieu = itemDL.ChatLieu,
                                    };
                                    lstCT.Add(ctHoaDon_DL);
                                }
                            }

                            lstCT.Add(ctHoaDon);
                            err = classhoadonchitiet.Add_ChiTietHoaDon(lstCT);

                            if (item.ID_LoHang != null)
                            {
                                sMaLoHang = string.Concat(" (Số lô: ", item.MaLoHang, ")");
                            }
                            styleHangHoa += string.Concat(style1, "loadHangHoabyMaHH", style2, item.MaHangHoa, style3, item.MaHangHoa, style4, sMaLoHang,
                                ": Số lượng: ", item.SoLuong, ", Giá trị: ", item.DonGia == 0 ? "0" : item.DonGia.ToString("#,#", CultureInfo.InvariantCulture), "<br />");

                            #endregion
                        }

                        #endregion

                        #region "NhatKy ThaoTac - don't use SP update TonKho of Phuong"
                        db.SaveChanges();
                        trans.Commit();
                        // update giavon + tonkho at js
                        #endregion

                        return Json(new
                        {
                            res = true,
                            data = new
                            {
                                objHoaDon.ID,
                                objHoaDon.NgayLapHoaDon,
                                objUp.MaHoaDon,
                                NgayLapHoaDonOld = ngayLapOld,
                                ChiTietOld = inforOld,
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        return Json(new
                        {
                            res = false,
                            mes = e.InnerException + e.Message,
                        });
                    }
                }
            }
        }

        [ResponseType(typeof(string))]
        [HttpPost, HttpPut]
        public IHttpActionResult PutBH_HDChuyenHang([FromBody] JObject data, Guid ID_NhanVien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                BH_HoaDon BH_HoaDon = data["objNewHoaDon"].ToObject<BH_HoaDon>();
                var NgayLapHoaDon = db.BH_HoaDon.Where(x => x.ID == BH_HoaDon.ID).FirstOrDefault().NgayLapHoaDon;
                var ID_DonVi = db.BH_HoaDon.Where(x => x.ID == BH_HoaDon.ID).FirstOrDefault().ID_DonVi;
                // kiểm tra chốt sổ
                string loaiEdit = "hủy bỏ";
                if (BH_HoaDon.YeuCau != "Hủy bỏ")
                    loaiEdit = "sửa";
                string csER = _classXH.checkNgayChotSo(BH_HoaDon.ID_DonVi, BH_HoaDon.ID, BH_HoaDon.NgayLapHoaDon, loaiEdit);
                if (csER != null && csER != string.Empty)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, csER));
                }
                else
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    string strUpd = _classXH.Update_HDChuyenHang(BH_HoaDon);
                    List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                    int k = 1;
                    string dk = string.Empty;
                    int loaiNK = 1;
                    string chitiet_xh = string.Empty;
                    string noidung_xh = string.Empty;
                    if (BH_HoaDon.YeuCau != "Hủy bỏ")
                    {
                        dk = "Cập nhật ";
                        chitiet_xh = dk + " phiếu xuất kho: <a style= \"cursor: pointer\" onclick = \"loadXuatKhobyMaPH('" + BH_HoaDon.MaHoaDon + "')\" >" + BH_HoaDon.MaHoaDon + "</a>, thời gian: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ", bao gồm: ";
                        noidung_xh = dk + " phiếu xuất kho: " + BH_HoaDon.MaHoaDon + " thời gian: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ", bao gồm: ";
                        loaiNK = 2; // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                    }
                    else
                    {
                        dk = "Hủy bỏ ";
                        chitiet_xh = dk + " phiếu xuất kho: " + BH_HoaDon.MaHoaDon + " thời gian: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                        noidung_xh = dk + " phiếu xuất kho: " + BH_HoaDon.MaHoaDon + " thời gian: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                        loaiNK = 4; // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                    }

                    if (objCTHoaDon != null)
                    {
                        foreach (var item in objCTHoaDon)
                        {
                            BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                            {
                                ID = item.ID,
                                ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                DonGia = 0,
                                ID_HoaDon = BH_HoaDon.ID,
                                SoLuong = item.SoLuong,
                                ThanhTien = (double?)(item.GiaVon * item.SoLuong) ?? 0,
                                TienChietKhau = 0,
                                GhiChu = BH_HoaDon.DienGiai,
                                GiaVon = item.GiaVon,
                            };
                            DonViQuiDoi DQ = db.DonViQuiDois.Where(p => p.ID == item.ID_DonViQuiDoi).FirstOrDefault();
                            chitiet_xh = chitiet_xh + "<br> - <a style=\"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + "</a> : " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " Giá trị: " + string.Format("{0:#,##0.##}", ctHoaDon.ThanhTien).Replace(".", ",");
                            noidung_xh = noidung_xh + " - " + DQ.MaHangHoa + " : " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " Giá trị: " + string.Format("{0:#,##0.##}", ctHoaDon.ThanhTien).Replace(".", ",");
                            string strIns = _classXH.Update_ChiTietHoaDon(ctHoaDon, k);
                            k = k + 1;
                        }
                    }
                    ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                    classhoadonchitiet.UpdateTonKhoGiaVon_whenUpdateCTHD(BH_HoaDon.ID, ID_DonVi, NgayLapHoaDon);

                    //BH_HoaDon HD = ClassXuatHuy.getlist_BH_HoaDon(BH_HoaDon.ID);
                    if (strUpd != null && strUpd != string.Empty)
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                    else
                    {
                        BH_HoaDonDTO objReturn = new BH_HoaDonDTO();
                        objReturn.ID = BH_HoaDon.ID;
                        objReturn.MaHoaDon = BH_HoaDon.MaHoaDon;
                        objReturn.LoaiPhieu = "";
                        objReturn.NgayLapHoaDon = DateTime.Parse(BH_HoaDon.NgayLapHoaDon.ToString());
                        objReturn.TongTienHang = BH_HoaDon.TongTienHang;
                        objReturn.TenDonVi = BH_HoaDon.DM_DonVi != null ? BH_HoaDon.DM_DonVi.TenDonVi : "";
                        objReturn.TenNhanVien = BH_HoaDon.NS_NhanVien != null ? BH_HoaDon.NS_NhanVien.TenNhanVien : "";
                        objReturn.TenDoiTuong = BH_HoaDon.DM_DoiTuong != null ? BH_HoaDon.DM_DoiTuong.TenDoiTuong : "";
                        objReturn.DienGiai = BH_HoaDon.DienGiai;
                        objReturn.YeuCau = BH_HoaDon.YeuCau;
                        objReturn.ChoThanhToan = BH_HoaDon.ChoThanhToan;

                        string str = CookieStore.GetCookieAes("SubDomain");
                        Guid? idHoaDon = null;
                        if (BH_HoaDon.ChoThanhToan == false)
                        {
                            idHoaDon = BH_HoaDon.ID;
                            //Thread st1 = new Thread(() => classHoaDon.UpdateGiaVonDM_GiaVonLT(BH_HoaDon.ID, ID_DonVi, NgayLapHoaDon, 3, str)); //1:Thêm mới, 2:Cập nhật, 3:Xóa //chạy qua khi hàm đang chạy
                            //st1.Start();
                        }
                        HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                        {
                            ID = Guid.NewGuid(),
                            ID_HoaDon = idHoaDon,
                            LoaiHoaDon = 8,
                            ThoiGianUpdateGV = NgayLapHoaDon,
                            ID_NhanVien = ID_NhanVien,
                            ID_DonVi = BH_HoaDon.ID_DonVi,
                            ChucNang = "Xuất kho",
                            ThoiGian = DateTime.Now,
                            //NoiDung = dk + "phiếu xuất hủy: " + BH_HoaDon.MaHoaDon,
                            NoiDung = noidung_xh,
                            NoiDungChiTiet = chitiet_xh,
                            LoaiNhatKy = loaiNK
                        };
                        db.HT_NhatKySuDung.Add(hT_NhatKySuDung);
                        db.SaveChanges();
                        //string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                        return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                    }
                }
            }
        }
        [HttpPost, HttpPut]
        public IHttpActionResult EditBH_HDChuyenHang([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                //Guid id = data["id"].ToObject<Guid>();
                BH_HoaDon BH_HoaDon = data["objNewHoaDon"].ToObject<BH_HoaDon>();

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string strUpd = _classXH.Update_HDChuyenHang(BH_HoaDon);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                {
                    BH_HoaDonDTO objReturn = new BH_HoaDonDTO();
                    objReturn.ID = BH_HoaDon.ID;
                    objReturn.MaHoaDon = BH_HoaDon.MaHoaDon;
                    objReturn.NgayLapHoaDon = BH_HoaDon.NgayLapHoaDon;
                    objReturn.TongTienHang = BH_HoaDon.TongTienHang;
                    objReturn.TenDonVi = BH_HoaDon.DM_DonVi != null ? BH_HoaDon.DM_DonVi.TenDonVi : "";
                    objReturn.TenNhanVien = BH_HoaDon.NS_NhanVien != null ? BH_HoaDon.NS_NhanVien.TenNhanVien : "";
                    objReturn.TenDoiTuong = BH_HoaDon.DM_DoiTuong != null ? BH_HoaDon.DM_DoiTuong.TenDoiTuong : "";
                    objReturn.DienGiai = BH_HoaDon.DienGiai;
                    objReturn.YeuCau = BH_HoaDon.YeuCau;
                    objReturn.ChoThanhToan = BH_HoaDon.ChoThanhToan;
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }

        [HttpPost, HttpPut]
        public IHttpActionResult PostBH_XuatKho([FromBody] JObject data, Guid ID_NhanVien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                        ClassXuatHuy _classXH = new ClassXuatHuy(db);
                        classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                        BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                        List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                        DateTime timeCreat = objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                        bool maExist = classHoaDon.Check_MaHoaDonExist(objHoaDon.MaHoaDon);
                        if (maExist)
                        {
                            return Json(new
                            {
                                res = false,
                                mes = "Mã hóa đơn đã tồn tại"
                            });
                        }
                        string chotso = _classXH.checkNgayChotSo(objHoaDon.ID_DonVi, Guid.NewGuid(), timeCreat, "tạo");
                        if (!string.IsNullOrEmpty(chotso))
                        {
                            return Json(new
                            {
                                res = false,
                                mes = chotso
                            });
                        }
                        #region BH_HoaDon
                        string sMaHoaDon = string.Empty;
                        Guid idHoaDon = Guid.NewGuid();
                        Guid? idHoaDon2 = null;
                        switch (objHoaDon.ChoThanhToan)
                        {
                            case false:
                                idHoaDon2 = idHoaDon;
                                objHoaDon.YeuCau = "Hoàn thành";
                                break;
                            case true:
                                objHoaDon.YeuCau = "Tạm lưu";
                                break;
                            default:
                                objHoaDon.YeuCau = "Hủy bỏ";
                                break;
                        }

                        if (string.IsNullOrEmpty(objHoaDon.MaHoaDon))
                        {
                            sMaHoaDon = classHoaDon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, timeCreat);
                        }
                        else
                        {
                            sMaHoaDon = objHoaDon.MaHoaDon;
                        }
                        objHoaDon.ID = idHoaDon;
                        objHoaDon.MaHoaDon = sMaHoaDon;
                        objHoaDon.NgayLapHoaDon = timeCreat;
                        db.BH_HoaDon.Add(objHoaDon);
                        #endregion

                        #region BH_HoaDon_ChiTiet
                        string chitiet_xh = string.Empty;
                        string noidung_xh = string.Empty;
                        foreach (var item in objCTHoaDon)
                        {
                            BH_HoaDon_ChiTiet ct = new BH_HoaDon_ChiTiet()
                            {
                                ID = Guid.NewGuid(),
                                ID_HoaDon = idHoaDon,
                                ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                ID_LoHang = item.ID_LoHang,
                                SoThuTu = item.SoThuTu,
                                SoLuong = item.SoLuong,
                                GiaVon = item.GiaVon,
                                DonGia = item.GiaVon ?? 0,
                                ThanhTien = item.ThanhTien,
                                GhiChu = item.GhiChu,
                                ID_ChiTietGoiDV = item.ID_ChiTietGoiDV,
                                ID_ChiTietDinhLuong = item.ID_ChiTietDinhLuong,
                                ChatLieu = item.ChatLieu,// used to check xuat sd goibaoduong
                            };

                            if (item.ThanhPhan_DinhLuong != null && item.ThanhPhan_DinhLuong.Count > 0)
                            {
                                foreach (var tpdl in item.ThanhPhan_DinhLuong)
                                {
                                    BH_HoaDon_ChiTiet cttp = new BH_HoaDon_ChiTiet()
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_HoaDon = idHoaDon,
                                        ID_DonViQuiDoi = tpdl.ID_DonViQuiDoi,
                                        ID_LoHang = tpdl.ID_LoHang,
                                        ID_ChiTietDinhLuong = ct.ID,
                                        SoThuTu = tpdl.SoThuTu,
                                        SoLuong = tpdl.SoLuong,
                                        GiaVon = tpdl.GiaVon,
                                        DonGia = tpdl.GiaVon ?? 0,
                                        ThanhTien = tpdl.ThanhTien,
                                        GhiChu = tpdl.GhiChu,
                                        ID_ChiTietGoiDV = tpdl.ID_ChiTietGoiDV,
                                        ChatLieu = tpdl.ChatLieu,
                                    };
                                    db.BH_HoaDon_ChiTiet.Add(cttp);
                                }
                            }
                            db.BH_HoaDon_ChiTiet.Add(ct);
                        }

                        #endregion
                        db.SaveChanges();
                        trans.Commit();

                        return Json(new
                        {
                            res = true,
                            data = new { ID = idHoaDon, MaHoaDon = sMaHoaDon, NgayLapHoaDon = timeCreat }
                        });
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        return Json(new
                        {
                            res = false,
                            mes = e.Message + e.InnerException
                        });
                    }
                }
            }
        }

        #region Phân Trang
        //Trả về dữ liệu theo trang selection
        public List<BC_XuatHuy> getlistPage(List<BC_XuatHuy> lst, List<BC_XuatHuy> lstPage, int sohang, int Page)
        {
            if (lst != null)
            {
                for (int j = (Page - 1) * sohang; j < Page * sohang; j++)
                {
                    if (j < lst.Count)
                    {
                        lstPage.Add(lst[j]);
                    }
                }
            }
            if (lstPage.Count > 0)
                return lstPage;
            else
                return null;
        }
        //Lấy số bản ghi trong data
        public int getRowsCountList(List<BC_XuatHuy> lstLHs)
        {
            if (lstLHs != null)
            {
                return lstLHs.Count;
            }
            else
            {
                return 0;
            }
        }
        //Lấy số trang hiển thị danh sách
        public List<ListLHPages> getAllPage(List<BC_XuatHuy> lstLHs, List<ListLHPages> listPage, float sohang)
        {
            if (lstLHs != null)
            {
                int dem = 1;
                float SoTrang = lstLHs.Count / sohang;
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
        #endregion
        public string Check_MHDExist(string maHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                string tb = null;
                bool chk = classHoaDon.Check_MaHoaDonExist(maHoaDon);
                if (chk == true)
                {
                    tb = "Mã phiếu xuất hủy đã tồn tại trong cơ sở dữ liệu";
                }
                return tb;
            }
        }
        public List<BH_HoaDonDTO> GetListHoaDons_QuyHD2(int loaiHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classHoaDonChiTiet = new ClassBH_HoaDon_ChiTiet(db);
                ClassDM_HangHoa classDMHangHoa = new ClassDM_HangHoa(db);
                ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);

                var lstAllHDs = classHoaDon.GetListXuatHuy(loaiHoaDon).OrderByDescending(od => od.NgayLapHoaDon);

                try
                {
                    List<BH_HoaDonDTO> lsrReturns = new List<BH_HoaDonDTO>();
                    foreach (BH_HoaDonDTO item in lstAllHDs)
                    {
                        double khachTra = 0;
                        Quy_HoaDon_ChiTiet quyHD_CT = _classQHDCT.Gets(idHD => idHD.ID_HoaDonLienQuan == item.ID).FirstOrDefault();
                        if (quyHD_CT != null)
                        {
                            //khachTra = quyHD_CT.TienGui == 0 ? quyHD_CT.TienMat : quyHD_CT.TienGui;
                            khachTra = quyHD_CT.TienGui + quyHD_CT.TienMat;
                        }
                        BH_HoaDonDTO itemData = new BH_HoaDonDTO
                        {
                            ID = item.ID,
                            MaHoaDon = item.MaHoaDon,
                            TenNhanVien = item.TenNhanVien,
                            TenDonVi = item.TenDonVi,
                            NgayLapHoaDon = item.NgayLapHoaDon,
                            TongGiamGia = item.TongGiamGia,
                            TongTienHang = item.TongTienHang,
                            PhaiThanhToan = item.PhaiThanhToan,
                            TenDoiTuong = item.TenDoiTuong == null ? "Khách lẻ" : item.TenDoiTuong,
                            DienGiai = item.DienGiai,
                            Email = item.Email,
                            DienThoai = item.DienThoai,
                            TenPhongBan = item.TenPhongBan,
                            KhachDaTra = khachTra,
                            ID_NhanVien = item.ID_NhanVien,
                            NguoiTaoHD = item.NguoiTaoHD,
                            TenBangGia = item.TenBangGia,
                            ChoThanhToan = item.ChoThanhToan,
                            YeuCau = item.YeuCau,
                            ID_DoiTuong = item.ID_DoiTuong,
                            TrangThai = item.ChoThanhToan == null ? "Đã hủy" : (item.ChoThanhToan == false ? "Hoàn thành" : "Tạm lưu"),
                            // phai viet nhu duoi day de tranh loi (LINQ to Entities does not recognize the method and this method)
                            BH_HoaDon_ChiTiet = classHoaDonChiTiet.Gets(ct => ct.ID_HoaDon == item.ID).Select(x => new
                            {
                                ID = x.ID,
                                ID_HoaDon = item.ID,
                                DonGia = x.DonGia,
                                GiaVon = x.GiaVon,
                                SoLuong = x.SoLuong,
                                ThanhTien = x.ThanhTien,
                                ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                                ID_HangHoa = x.DonViQuiDoi.ID_HangHoa,
                                MaHangHoa = x.DonViQuiDoi.MaHangHoa,
                                GiamGia = x.TienChietKhau,
                                ThoiGian = x.ThoiGian,
                                GhiChu = x.GhiChu
                            }).AsEnumerable().Select(c => new BH_HoaDon_ChiTietDTO
                            {

                                ID = c.ID,
                                ID_HoaDon = c.ID_HoaDon,
                                DonGia = c.DonGia,
                                GiaVon = c.GiaVon,
                                SoLuong = c.SoLuong,
                                ThanhTien = c.ThanhTien,
                                ID_DonViQuiDoi = c.ID_DonViQuiDoi,
                                MaHangHoa = c.MaHangHoa,
                                TenHangHoa = classDMHangHoa.Select_HangHoaPRG(c.ID_HangHoa).FirstOrDefault().TenHangHoa,
                                ThuocTinh_GiaTri = classDMHangHoa.Select_HangHoaPRG(c.ID_HangHoa).FirstOrDefault().ThuocTinh_GiaTri,
                                GiamGia = c.GiamGia,
                                ThoiGian = c.ThoiGian,
                                GhiChu = c.GhiChu
                            }).ToList()

                        };
                        lsrReturns.Add(itemData);
                    }

                    return lsrReturns;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("BH_XuatHuyAPI_GetListHoaDons_QuyHD2: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }

        //Xuất báo cáo
        [HttpPost]
        public IHttpActionResult ExportExelXH(Params_GetListHoaDon param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                Class_officeDocument classOffice = new Class_officeDocument(db);
                try
                {
                    List<BH_HoaDonDTO> lst = classHoaDon.GetListHDXuatKho(param);
                    List<BC_XuatHuyPRC> lstEx = new List<BC_XuatHuyPRC>();

                    foreach (var item in lst)
                    {
                        BC_XuatHuyPRC DM = new BC_XuatHuyPRC();
                        DM.MaHoaDon = item.MaHoaDon;
                        DM.NgaySua = item.NgaySua;
                        DM.MaHoaDonSuaChua = item.MaHoaDonGoc;
                        DM.NgayLapHoaDon = item.NgayLapHoaDon;
                        DM.LoaiPhieu = item.LoaiPhieu;
                        DM.TenChiNhanh = item.TenDonVi;
                        DM.TenDoiTuong = item.TenDoiTuong;
                        DM.TenNhanVien = item.TenNhanVien;
                        DM.TongTienHang = item.TongTienHang;
                        DM.DienGiai = item.DienGiai;
                        DM.YeuCau = item.YeuCau;
                        lstEx.Add(DM);
                    }
                    DataTable excel = classOffice.ToDataTable<BC_XuatHuyPRC>(lstEx);
                    excel.Columns.Remove("ID");
                    excel.Columns.Remove("ID_NhanVien");
                    excel.Columns.Remove("ID_DonVi");
                    excel.Columns.Remove("ID_HoaDon");
                    excel.Columns.Remove("NguoiTaoHD");
                    excel.Columns.Remove("ID_PhieuTiepNhan");
                    excel.Columns.Remove("ChoThanhToan");
                    excel.Columns.Remove("LoaiHoaDon");
                    excel.Columns.Remove("showTime");

                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_DanhSachXuatHuy.xlsx");
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/DanhSachXuatKho.xlsx");
                    fileSave = classOffice.createFolder_Download(fileSave);
                    var valExcel1 = string.Empty;
                    if (param.NgayTaoHD_TuNgay == new DateTime(2016, 1, 1))
                    {
                        valExcel1 = "Toàn thời gian";
                    }
                    else
                    {
                        valExcel1 = param.NgayTaoHD_TuNgay + " - " + param.NgayTaoHD_DenNgay;
                    }
                    classOffice.listToOfficeExcel_Stype(fileTeamplate, fileSave, excel, 4, 28, 24, true, param.ColumnsHide, valExcel1, param.ValueText);

                    var index = fileSave.IndexOf(@"\Template");
                    fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                    fileSave = fileSave.Replace(@"\", "/");

                    return Json(new { res = true, url = fileSave });
                }
                catch (Exception ex)
                {
                    return Json(new { res = false, mes = ex.InnerException + ex.Message });
                }
            }
        }
        [HttpGet]
        public void ExportExcelXH_ChiTiet(Guid ID_HoaDon, string ColumnsHide, string time, string ChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument classOffice = new Class_officeDocument(db);
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("ID_HoaDon", ID_HoaDon));
                prm.Add(new SqlParameter("ID_ChiNhanh", new Guid(ChiNhanh)));
                List<XH_HoaDon_ChiTietPRC> lst = db.Database.SqlQuery<XH_HoaDon_ChiTietPRC>("exec getList_HangHoaXuatHuybyID @ID_HoaDon, @ID_ChiNhanh", prm.ToArray()).ToList();
                DataTable excel = classOffice.ToDataTable<XH_HoaDon_ChiTietPRC>(lst);
                excel.Columns.Remove("ID");
                excel.Columns.Remove("ID_HoaDon");
                excel.Columns.Remove("ID_DonViQuiDoi");
                excel.Columns.Remove("ID_LoHang");
                excel.Columns.Remove("ID_HangHoa");
                excel.Columns.Remove("QuanLyTheoLoHang");
                excel.Columns.Remove("NgaySanXuat");
                excel.Columns.Remove("NgayHetHan");
                excel.Columns.Remove("MaHoaDon");
                excel.Columns.Remove("TenNhanVien");
                excel.Columns.Remove("TenHangHoa");
                excel.Columns.Remove("ThuocTinh_GiaTri");
                excel.Columns.Remove("TonKho");
                excel.Columns.Remove("TrangThaiMoPhieu");
                excel.Columns.Remove("SoLuongXuatHuy");
                excel.Columns.Remove("DonGia");
                excel.Columns.Remove("GiamGia");
                excel.Columns.Remove("NgayLapHoaDon");
                excel.Columns.Remove("ID_NhanVien");
                excel.Columns.Remove("SoThuTu");
                excel.Columns.Remove("ChatLieu");
                // get tendonvi by ID
                string teamplateFile = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_DanhSachXuatHuy_ChiTiet.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/DanhSachXuatKho_ChiTiet.xlsx");
                fileSave = classOffice.createFolder_Download(fileSave);
                classOffice.listToOfficeExcel(teamplateFile, fileSave, excel, 4, 28, 24, true, null);
                HttpResponse Response = HttpContext.Current.Response;
                classOffice.downloadFile(fileSave);
            }
        }

        public IHttpActionResult JqAutoHangHoa_withGiaVonTieuChuan(CommonParamSearch param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassDM_HangHoa classHangHoa = new ClassDM_HangHoa(db);
                    List<DieuChinhGiaVon_HangHoaDTO> data = classHangHoa.JqAutoHangHoa_withGiaVonTieuChuan(param);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }
        // getlist danh sách hàng có tồn kho

        public List<jqAutoResult_HangHoa_Search> getListHangHoa_TonKho(string maHH, Guid ID_ChiNhanh, Guid ID_NguoiDung)
        {
            List<Search_HangHoa_XuatNhapTonPRC> lst = new List<Search_HangHoa_XuatNhapTonPRC>();
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (!string.IsNullOrEmpty(maHH))
            {

                char[] whitespace = new char[] { ' ', '\t' };
                string[] textFilter = maHH.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Split(whitespace);
                string[] utf8 = textFilter.Where(o => o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                string[] utf = textFilter.Where(o => !o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("MaHH", string.Join(",", utf)));
                paramlist.Add(new SqlParameter("MaHH_TV", string.Join(",", utf8)));
                paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                paramlist.Add(new SqlParameter("ID_NguoiDung", ID_NguoiDung));
                //var tbl_timeCSt = from cs in db.ChotSo
                //                  where cs.ID_DonVi == ID_ChiNhanh
                //                  select cs;
                //if (tbl_timeCSt.Count() > 0)
                //{
                //    lst = db.Database.SqlQuery<Search_HangHoa_XuatNhapTonPRC>("exec Search_DMHangHoa_TonKho_ChotSo @MaHH, @MaHH_TV, @ID_ChiNhanh, @ID_NguoiDung", paramlist.ToArray()).ToList();
                //}
                //else
                //{
                lst = db.Database.SqlQuery<Search_HangHoa_XuatNhapTonPRC>("exec Search_DMHangHoa_TonKho @MaHH, @MaHH_TV, @ID_ChiNhanh, @ID_NguoiDung", paramlist.ToArray()).ToList();
                //}

                return lst.Select(p => new jqAutoResult_HangHoa_Search
                {
                    label = p.TenHangHoa,
                    value = p.MaHangHoa,
                    actual = p.MaHangHoa.ToString(),
                    data = p
                }).ToList();
            }
            else
            {
                return null;
            }
        }
        public List<jqAutoResult_HangHoaLoHang_Search> getListHangHoaLoHang_TonKho(string maHH, Guid ID_ChiNhanh, Guid ID_NguoiDung)
        {
            //List<jqAutoResult_HangHoa> lst = ClassXuatHuy.getListHangHoa_XNT(maHH, ID_ChiNhanh);
            //return lst;

            List<Search_HangHoaLoHang_XuatNhapTonPRC> lst = new List<Search_HangHoaLoHang_XuatNhapTonPRC>();
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (!string.IsNullOrEmpty(maHH))
            {

                //char[] whitespace = new char[] { ' ', '\t' };
                //string[] textFilter = maHH.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Split(whitespace);
                //string[] utf8 = textFilter.Where(o => o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                //string[] utf = textFilter.Where(o => !o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                var txtSearch = string.Empty;
                if (maHH != null)
                {
                    txtSearch = maHH;
                }
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("MaHH", txtSearch));
                paramlist.Add(new SqlParameter("MaHH_TV", txtSearch));
                paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                paramlist.Add(new SqlParameter("ID_NguoiDung", ID_NguoiDung));
                //var tbl_timeCSt = from cs in db.ChotSo
                //                  where cs.ID_DonVi == ID_ChiNhanh
                //                  select cs;
                //if (tbl_timeCSt.Count() > 0)
                //{
                //    lst = db.Database.SqlQuery<Search_HangHoaLoHang_XuatNhapTonPRC>("exec Search_DMHangHoaLoHang_TonKho_ChotSo @MaHH, @MaHH_TV, @ID_ChiNhanh, @ID_NguoiDung", paramlist.ToArray()).ToList();
                //}
                //else
                //{
                lst = db.Database.SqlQuery<Search_HangHoaLoHang_XuatNhapTonPRC>("exec Search_DMHangHoaLoHang_TonKho @MaHH, @MaHH_TV, @ID_ChiNhanh, @ID_NguoiDung", paramlist.ToArray()).ToList();
                //}

                return lst.Select(p => new jqAutoResult_HangHoaLoHang_Search
                {
                    label = p.TenHangHoa,
                    value = p.MaHangHoa,
                    actual = p.MaHangHoa.ToString(),
                    data = p
                }).ToList();
            }
            else
            {
                return null;
            }
        }
        public List<Search_HangHoaLoHang_XuatNhapTonPRC> getListHangHoaLoHangBy_MaHangHoa(string maHH, Guid ID_ChiNhanh)
        {
            List<Search_HangHoaLoHang_XuatNhapTonPRC> lst = new List<Search_HangHoaLoHang_XuatNhapTonPRC>();
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (!string.IsNullOrEmpty(maHH))
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("MaHH", maHH));
                paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                //var tbl_timeCSt = from cs in db.ChotSo
                //                  where cs.ID_DonVi == ID_ChiNhanh
                //                  select cs;
                //if (tbl_timeCSt.Count() > 0)
                //{
                //lst = db.Database.SqlQuery<Search_HangHoaLoHang_XuatNhapTonPRC>("exec getListHangHoaLoHang_ChotSo_ByMaHangHoa @MaHH, @ID_ChiNhanh", paramlist.ToArray()).ToList();
                //}
                //else
                //{
                lst = db.Database.SqlQuery<Search_HangHoaLoHang_XuatNhapTonPRC>("exec getListHangHoaLoHang_ByMaHangHoa @MaHH, @ID_ChiNhanh", paramlist.ToArray()).ToList();
                //}
                return lst;
            }
            else
            {
                return null;
            }
        }
        public List<Search_HangHoaLoHang_XuatNhapTonPRC> getListHangHoaBy_MaHangHoa(string maHH, Guid ID_ChiNhanh)
        {
            List<Search_HangHoaLoHang_XuatNhapTonPRC> lst = new List<Search_HangHoaLoHang_XuatNhapTonPRC>();
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (!string.IsNullOrEmpty(maHH))
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("MaHH", maHH));
                paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                //var tbl_timeCSt = from cs in db.ChotSo
                //                  where cs.ID_DonVi == ID_ChiNhanh
                //                  select cs;
                //if (tbl_timeCSt.Count() > 0)
                //{
                //    lst = db.Database.SqlQuery<Search_HangHoaLoHang_XuatNhapTonPRC>("exec getListHangHoaLoHang_ChotSo_ByMaHangHoa @MaHH, @ID_ChiNhanh", paramlist.ToArray()).ToList();
                //}
                //else
                //{
                lst = db.Database.SqlQuery<Search_HangHoaLoHang_XuatNhapTonPRC>("exec getListHangHoaLoHang_ByMaHangHoa @MaHH, @ID_ChiNhanh", paramlist.ToArray()).ToList();
                //}
                return lst;
            }
            else
            {
                return null;
            }
            //List<Report_HangHoa_XuatNhapTon_Union> lst = ClassXuatHuy.getListHangHoaBy_MaHangHoa(maHH, ID_ChiNhanh);
            //return lst;
        }
        public List<libQuy_HoaDon.List_TenDonViTinh> GetList_TenDonViTinh(string MaHH)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                List<libQuy_HoaDon.List_TenDonViTinh> lst = _classXH.GetList_TenDonViTinh(MaHH);
                return lst;
            }
        }
        //load theo chi nhánh
        public List<BH_HoaDonDTO> getallChinhanh(int loaihoadon, int sohang, int page, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3, string chinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                ClassDM_HangHoa classDMHangHoa = new ClassDM_HangHoa(db);
                ClassBH_HoaDon_ChiTiet classHoaDonChiTiet = new ClassBH_HoaDon_ChiTiet(db);
                ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);

                List<BC_XuatHuy> lst = _classXH.getAllChiNhanh(loaihoadon, dayStart, dayEnd, trangthai1, trangthai2, trangthai3, chinhanh);
                List<BC_XuatHuy> ListPage = new List<BC_XuatHuy>();
                getlistPage(lst, ListPage, sohang, page);
                try
                {
                    List<BH_HoaDonDTO> lsrReturns = new List<BH_HoaDonDTO>();
                    foreach (BC_XuatHuy item in ListPage)
                    {
                        double khachTra = 0;
                        Quy_HoaDon_ChiTiet quyHD_CT = _classQHDCT.Gets(idHD => idHD.ID_HoaDonLienQuan == item.ID).FirstOrDefault();
                        if (quyHD_CT != null)
                        {
                            khachTra = quyHD_CT.TienGui + quyHD_CT.TienMat;
                        }
                        BH_HoaDonDTO itemData = new BH_HoaDonDTO
                        {
                            ID = item.ID,
                            MaHoaDon = item.MaHoaDon,
                            TenNhanVien = item.TenNhanVien,
                            TenDonVi = item.TenDonVi,
                            NgayLapHoaDon = item.NgayLapHoaDon,
                            TongTienHang = item.TongTienHang,
                            DienGiai = item.DienGiai,
                            ID_NhanVien = item.ID_NhanVien,
                            NguoiTaoHD = item.NguoiTaoHD,
                            ChoThanhToan = item.ChoThanhToan,
                            YeuCau = item.YeuCau,
                            ID_DoiTuong = item.ID_DoiTuong,
                            // phai viet nhu duoi day de tranh loi (LINQ to Entities does not recognize the method and this method)
                            BH_HoaDon_ChiTiet = classHoaDonChiTiet.Gets(ct => ct.ID_HoaDon == item.ID).Select(x => new
                            {
                                ID = x.ID,
                                ID_HoaDon = item.ID,
                                DonGia = x.DonGia,
                                GiaVon = x.GiaVon,
                                SoLuong = x.SoLuong,
                                ThanhTien = x.ThanhTien,
                                ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                                ID_HangHoa = x.DonViQuiDoi.ID_HangHoa,
                                MaHangHoa = x.DonViQuiDoi.MaHangHoa,
                                GiamGia = x.TienChietKhau,
                                ThoiGian = x.ThoiGian,
                                GhiChu = x.GhiChu
                            }).AsEnumerable().Select(c => new BH_HoaDon_ChiTietDTO
                            {

                                ID = c.ID,
                                ID_HoaDon = c.ID_HoaDon,
                                DonGia = c.DonGia,
                                GiaVon = c.GiaVon,
                                SoLuong = c.SoLuong,
                                ThanhTien = c.ThanhTien,
                                ID_DonViQuiDoi = c.ID_DonViQuiDoi,
                                MaHangHoa = c.MaHangHoa,
                                TenHangHoa = classDMHangHoa.Select_HangHoaPRG(c.ID_HangHoa).FirstOrDefault().TenHangHoa,
                                ThuocTinh_GiaTri = classDMHangHoa.Select_HangHoaPRG(c.ID_HangHoa).FirstOrDefault().ThuocTinh_GiaTri,
                                GiamGia = c.GiamGia,
                                ThoiGian = c.ThoiGian,
                                GhiChu = c.GhiChu
                            }).ToList()

                        };
                        lsrReturns.Add(itemData);
                    }

                    return lsrReturns;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("BH_XuatHuyAPI_getallChinhanh: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }
        public int getRowCountCN(int loaihoadon, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3, string chinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                List<BC_XuatHuy> lst = _classXH.getAllChiNhanh(loaihoadon, dayStart, dayEnd, trangthai1, trangthai2, trangthai3, chinhanh);
                return getRowsCountList(lst);
            }
        }
        public List<ListLHPages> getPageCN(int loaihoadon, int sohang, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3, string chinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                List<BC_XuatHuy> lst = _classXH.getAllChiNhanh(loaihoadon, dayStart, dayEnd, trangthai1, trangthai2, trangthai3, chinhanh);
                List<ListLHPages> listPage = new List<ListLHPages>();
                return getAllPage(lst, listPage, sohang);
            }
        }
        // load trạng thái all
        public List<BH_HoaDonDTO> getallTrangThai(int loaihoadon, int sohang, int page, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                ClassDM_HangHoa classDMHangHoa = new ClassDM_HangHoa(db);
                ClassBH_HoaDon_ChiTiet classHoaDonChiTiet = new ClassBH_HoaDon_ChiTiet(db);
                ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);

                List<BC_XuatHuy> lst = _classXH.getAllTrangThai(loaihoadon, dayStart, dayEnd, trangthai1, trangthai2, trangthai3);
                List<BC_XuatHuy> ListPage = new List<BC_XuatHuy>();
                getlistPage(lst, ListPage, sohang, page);
                try
                {
                    List<BH_HoaDonDTO> lsrReturns = new List<BH_HoaDonDTO>();
                    foreach (BC_XuatHuy item in ListPage)
                    {
                        double khachTra = 0;
                        Quy_HoaDon_ChiTiet quyHD_CT = _classQHDCT.Gets(idHD => idHD.ID_HoaDonLienQuan == item.ID).FirstOrDefault();
                        if (quyHD_CT != null)
                        {
                            khachTra = quyHD_CT.TienGui + quyHD_CT.TienMat;
                        }
                        BH_HoaDonDTO itemData = new BH_HoaDonDTO
                        {
                            ID = item.ID,
                            MaHoaDon = item.MaHoaDon,
                            TenNhanVien = item.TenNhanVien,
                            TenDonVi = item.TenDonVi,
                            NgayLapHoaDon = item.NgayLapHoaDon,
                            TongTienHang = item.TongTienHang,
                            DienGiai = item.DienGiai,
                            ID_NhanVien = item.ID_NhanVien,
                            NguoiTaoHD = item.NguoiTaoHD,
                            ChoThanhToan = item.ChoThanhToan,
                            YeuCau = item.YeuCau,
                            ID_DoiTuong = item.ID_DoiTuong,
                            // phai viet nhu duoi day de tranh loi (LINQ to Entities does not recognize the method and this method)
                            BH_HoaDon_ChiTiet = classHoaDonChiTiet.Gets(ct => ct.ID_HoaDon == item.ID).Select(x => new
                            {
                                ID = x.ID,
                                ID_HoaDon = item.ID,
                                DonGia = x.DonGia,
                                GiaVon = x.GiaVon,
                                SoLuong = x.SoLuong,
                                ThanhTien = x.ThanhTien,
                                ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                                ID_HangHoa = x.DonViQuiDoi.ID_HangHoa,
                                MaHangHoa = x.DonViQuiDoi.MaHangHoa,
                                GiamGia = x.TienChietKhau,
                                ThoiGian = x.ThoiGian,
                                GhiChu = x.GhiChu
                            }).AsEnumerable().Select(c => new BH_HoaDon_ChiTietDTO
                            {

                                ID = c.ID,
                                ID_HoaDon = c.ID_HoaDon,
                                DonGia = c.DonGia,
                                GiaVon = c.GiaVon,
                                SoLuong = c.SoLuong,
                                ThanhTien = c.ThanhTien,
                                ID_DonViQuiDoi = c.ID_DonViQuiDoi,
                                MaHangHoa = c.MaHangHoa,
                                TenHangHoa = classDMHangHoa.Select_HangHoaPRG(c.ID_HangHoa).FirstOrDefault().TenHangHoa,
                                ThuocTinh_GiaTri = classDMHangHoa.Select_HangHoaPRG(c.ID_HangHoa).FirstOrDefault().ThuocTinh_GiaTri,
                                GiamGia = c.GiamGia,
                                ThoiGian = c.ThoiGian,
                                GhiChu = c.GhiChu
                            }).ToList()

                        };
                        lsrReturns.Add(itemData);
                    }

                    return lsrReturns;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("BH_XuatHuyAPI_getallTrangThai: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }
        public int getRowCountTT(int loaihoadon, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                List<BC_XuatHuy> lst = _classXH.getAllTrangThai(loaihoadon, dayStart, dayEnd, trangthai1, trangthai2, trangthai3);
                return getRowsCountList(lst);
            }
        }
        public List<ListLHPages> getPageTT(int loaihoadon, int sohang, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                List<BC_XuatHuy> lst = _classXH.getAllTrangThai(loaihoadon, dayStart, dayEnd, trangthai1, trangthai2, trangthai3);
                List<ListLHPages> listPage = new List<ListLHPages>();
                return getAllPage(lst, listPage, sohang);
            }
        }
        public List<HT_NguoiDungDTO> GetListNguoiDung()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                List<HT_NguoiDungDTO> lstAllVTs = _classXH.getAllNguoiDung().ToList();
                return lstAllVTs;
            }
        }
        //load danh sách phiếu hủy
        public List<BH_HoaDonDTO> getallPhieuHuy(int loaihoadon, int sohang, int page, DateTime dayStart, DateTime dayEnd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                ClassBH_HoaDon_ChiTiet classHoaDonChiTiet = new ClassBH_HoaDon_ChiTiet(db);
                ClassDM_HangHoa classDMHangHoa = new ClassDM_HangHoa(db);
                ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);

                List<BC_XuatHuy> lst = _classXH.getAllPhieuHuy(loaihoadon, dayStart, dayEnd);
                List<BC_XuatHuy> ListPage = new List<BC_XuatHuy>();
                getlistPage(lst, ListPage, sohang, page);
                try
                {
                    List<BH_HoaDonDTO> lsrReturns = new List<BH_HoaDonDTO>();
                    foreach (BC_XuatHuy item in ListPage)
                    {
                        double khachTra = 0;
                        Quy_HoaDon_ChiTiet quyHD_CT = _classQHDCT.Gets(idHD => idHD.ID_HoaDonLienQuan == item.ID).FirstOrDefault();
                        if (quyHD_CT != null)
                        {
                            khachTra = quyHD_CT.TienGui + quyHD_CT.TienMat;
                        }
                        BH_HoaDonDTO itemData = new BH_HoaDonDTO
                        {
                            ID = item.ID,
                            MaHoaDon = item.MaHoaDon,
                            TenNhanVien = item.TenNhanVien,
                            TenDonVi = item.TenDonVi,
                            NgayLapHoaDon = item.NgayLapHoaDon,
                            TongTienHang = item.TongTienHang,
                            DienGiai = item.DienGiai,
                            ID_NhanVien = item.ID_NhanVien,
                            NguoiTaoHD = item.NguoiTaoHD,
                            ChoThanhToan = item.ChoThanhToan,
                            YeuCau = item.YeuCau,
                            ID_DoiTuong = item.ID_DoiTuong,
                            // phai viet nhu duoi day de tranh loi (LINQ to Entities does not recognize the method and this method)
                            BH_HoaDon_ChiTiet = classHoaDonChiTiet.Gets(ct => ct.ID_HoaDon == item.ID).Select(x => new
                            {
                                ID = x.ID,
                                ID_HoaDon = item.ID,
                                DonGia = x.DonGia,
                                GiaVon = x.GiaVon,
                                SoLuong = x.SoLuong,
                                ThanhTien = x.ThanhTien,
                                ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                                ID_HangHoa = x.DonViQuiDoi.ID_HangHoa,
                                MaHangHoa = x.DonViQuiDoi.MaHangHoa,
                                GiamGia = x.TienChietKhau,
                                ThoiGian = x.ThoiGian,
                                GhiChu = x.GhiChu
                            }).AsEnumerable().Select(c => new BH_HoaDon_ChiTietDTO
                            {

                                ID = c.ID,
                                ID_HoaDon = c.ID_HoaDon,
                                DonGia = c.DonGia,
                                GiaVon = c.GiaVon,
                                SoLuong = c.SoLuong,
                                ThanhTien = c.ThanhTien,
                                ID_DonViQuiDoi = c.ID_DonViQuiDoi,
                                MaHangHoa = c.MaHangHoa,
                                TenHangHoa = classDMHangHoa.Select_HangHoaPRG(c.ID_HangHoa).FirstOrDefault().TenHangHoa,
                                ThuocTinh_GiaTri = classDMHangHoa.Select_HangHoaPRG(c.ID_HangHoa).FirstOrDefault().ThuocTinh_GiaTri,
                                GiamGia = c.GiamGia,
                                ThoiGian = c.ThoiGian,
                                GhiChu = c.GhiChu
                            }).ToList()

                        };
                        lsrReturns.Add(itemData);
                    }

                    return lsrReturns;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("BH_XuatHuyAPI_getallPhieuHuy: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }

        public int getRowCountPH(int loaihoadon, DateTime dayStart, DateTime dayEnd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                List<BC_XuatHuy> lst = new List<BC_XuatHuy>();
                lst = _classXH.getAllPhieuHuy(loaihoadon, dayStart, dayEnd);
                return getRowsCountList(lst);
            }
        }
        public List<ListLHPages> getPagePH(int loaihoadon, int sohang, DateTime dayStart, DateTime dayEnd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                List<BC_XuatHuy> lst = _classXH.getAllPhieuHuy(loaihoadon, dayStart, dayEnd);
                List<ListLHPages> listPage = new List<ListLHPages>();
                return getAllPage(lst, listPage, sohang);
            }
        }
        //tìm kiếm hóa đơn

        public List<BH_HoaDonDTO> getListSeach(int kieutimkiem, string giatriSeach, int sohang, int page, int loaihoadon, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                ClassBH_HoaDon_ChiTiet classHoaDonChiTiet = new ClassBH_HoaDon_ChiTiet(db);
                ClassDM_HangHoa classDMHangHoa = new ClassDM_HangHoa(db);
                ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);

                List<BC_XuatHuy> lst = new List<BC_XuatHuy>();
                if (kieutimkiem == 1)
                {
                    lst = _classXH.seachMaXH(giatriSeach, loaihoadon, IDchinhanh);
                }
                if (kieutimkiem == 2)
                {
                    lst = _classXH.seachMaHH(giatriSeach, loaihoadon, IDchinhanh);
                }
                if (kieutimkiem == 3)
                {
                    lst = _classXH.seachIDNhanVien(giatriSeach, loaihoadon, IDchinhanh);
                }
                if (kieutimkiem == 4)
                {
                    lst = _classXH.seachTenND(giatriSeach, loaihoadon, IDchinhanh);
                }
                List<BC_XuatHuy> ListPage = new List<BC_XuatHuy>();
                getlistPage(lst, ListPage, sohang, page);

                try
                {
                    List<BH_HoaDonDTO> lsrReturns = new List<BH_HoaDonDTO>();
                    foreach (BC_XuatHuy item in ListPage)
                    {
                        double khachTra = 0;
                        Quy_HoaDon_ChiTiet quyHD_CT = _classQHDCT.Gets(idHD => idHD.ID_HoaDonLienQuan == item.ID).FirstOrDefault();
                        if (quyHD_CT != null)
                        {
                            khachTra = quyHD_CT.TienGui + quyHD_CT.TienMat;
                        }
                        BH_HoaDonDTO itemData = new BH_HoaDonDTO
                        {
                            ID = item.ID,
                            MaHoaDon = item.MaHoaDon,
                            TenNhanVien = item.TenNhanVien,
                            TenDonVi = item.TenDonVi,
                            NgayLapHoaDon = item.NgayLapHoaDon,
                            TongGiamGia = item.TongGiamGia,
                            TongTienHang = item.TongTienHang,
                            PhaiThanhToan = item.PhaiThanhToan,
                            TenDoiTuong = item.TenDoiTuong == null ? "Khách lẻ" : item.TenDoiTuong,
                            DienGiai = item.DienGiai,
                            Email = item.Email,
                            DienThoai = item.DienThoai,
                            TenPhongBan = item.TenPhongBan,
                            KhachDaTra = khachTra,
                            ID_NhanVien = item.ID_NhanVien,
                            NguoiTaoHD = item.NguoiTaoHD,
                            TenBangGia = item.TenBangGia,
                            ChoThanhToan = item.ChoThanhToan,
                            YeuCau = item.YeuCau,
                            ID_DoiTuong = item.ID_DoiTuong,
                            TrangThai = item.ChoThanhToan == null ? "Đã hủy" : (item.ChoThanhToan == false ? "Hoàn thành" : "Tạm lưu"),
                            // phai viet nhu duoi day de tranh loi (LINQ to Entities does not recognize the method and this method)
                            BH_HoaDon_ChiTiet = classHoaDonChiTiet.Gets(ct => ct.ID_HoaDon == item.ID).Select(x => new
                            {
                                ID = x.ID,
                                ID_HoaDon = item.ID,
                                DonGia = x.DonGia,
                                GiaVon = x.GiaVon,
                                SoLuong = x.SoLuong,
                                ThanhTien = x.ThanhTien,
                                ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                                ID_HangHoa = x.DonViQuiDoi.ID_HangHoa,
                                MaHangHoa = x.DonViQuiDoi.MaHangHoa,
                                GiamGia = x.TienChietKhau,
                                ThoiGian = x.ThoiGian,
                                GhiChu = x.GhiChu
                            }).AsEnumerable().Select(c => new BH_HoaDon_ChiTietDTO
                            {

                                ID = c.ID,
                                ID_HoaDon = c.ID_HoaDon,
                                DonGia = c.DonGia,
                                GiaVon = c.GiaVon,
                                SoLuong = c.SoLuong,
                                ThanhTien = c.ThanhTien,
                                ID_DonViQuiDoi = c.ID_DonViQuiDoi,
                                MaHangHoa = c.MaHangHoa,
                                TenHangHoa = classDMHangHoa.Select_HangHoaPRG(c.ID_HangHoa).FirstOrDefault().TenHangHoa,
                                ThuocTinh_GiaTri = classDMHangHoa.Select_HangHoaPRG(c.ID_HangHoa).FirstOrDefault().ThuocTinh_GiaTri,
                                GiamGia = c.GiamGia,
                                ThoiGian = c.ThoiGian,
                                GhiChu = c.GhiChu
                            }).ToList()

                        };
                        lsrReturns.Add(itemData);
                    }

                    return lsrReturns;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("BH_XuatHuyAPI_getListSeach: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }

        public int getRowCountSeach(int kieutimkiem, string giatriSeach, int loaihoadon, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                List<BC_XuatHuy> lst = new List<BC_XuatHuy>();
                if (kieutimkiem == 1)
                {
                    lst = _classXH.seachMaXH(giatriSeach, loaihoadon, IDchinhanh);
                }
                if (kieutimkiem == 2)
                {
                    lst = _classXH.seachMaHH(giatriSeach, loaihoadon, IDchinhanh);
                }
                if (kieutimkiem == 3)
                {
                    lst = _classXH.seachIDNhanVien(giatriSeach, loaihoadon, IDchinhanh);
                }
                if (kieutimkiem == 4)
                {
                    lst = _classXH.seachTenND(giatriSeach, loaihoadon, IDchinhanh);
                }
                return getRowsCountList(lst);
            }
        }
        public List<ListLHPages> getPageSeach(int kieutimkiem, string giatriSeach, int sohang, int loaihoadon, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                List<BC_XuatHuy> lst = new List<BC_XuatHuy>();
                if (kieutimkiem == 1)
                {
                    lst = _classXH.seachMaXH(giatriSeach, loaihoadon, IDchinhanh);
                }
                if (kieutimkiem == 2)
                {
                    lst = _classXH.seachMaHH(giatriSeach, loaihoadon, IDchinhanh);
                }
                if (kieutimkiem == 3)
                {
                    lst = _classXH.seachIDNhanVien(giatriSeach, loaihoadon, IDchinhanh);
                }
                if (kieutimkiem == 4)
                {
                    lst = _classXH.seachTenND(giatriSeach, loaihoadon, IDchinhanh);
                }
                List<ListLHPages> listPage = new List<ListLHPages>();
                return getAllPage(lst, listPage, sohang);
            }
        }
        // Lấy danh sách người dùng
        public static HT_NguoiDung getallNguoiDung()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                HT_NguoiDung lst = _classXH.getAllNguoidung();
                if (lst != null)
                {
                    return lst;
                }
                else
                {
                    return null;
                }
            }
        }
        // xóa phiếu hủy
        public IHttpActionResult DeleteBH_HoaDon(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                string strDel = _classXH.Delete_HoaDon(id);
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        //update chothanhtoan
        public string UpdateHD_ChoThanToan(Guid id, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classKho_TonKhoKhoiTao _classTKKT = new classKho_TonKhoKhoiTao(db);
                ClassBH_HoaDon_ChiTiet classHoaDonChiTiet = new ClassBH_HoaDon_ChiTiet(db);
                ClassDM_HangHoa classDMHangHoa = new ClassDM_HangHoa(db);
                ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                classDonViQuiDoi classDonViQuiDoi = new classDonViQuiDoi(db);

                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    BH_HoaDon item = db.BH_HoaDon.Find(id);
                    if (item != null)
                    {
                        item.ChoThanhToan = null;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();

                        List<BH_HoaDon> allHD = classHoaDon.Gets(loaiHD => loaiHD.ChoThanhToan == false).Where(p => p.NgayLapHoaDon > item.NgayLapHoaDon).OrderBy(p => p.NgayLapHoaDon).ToList();
                        if (allHD.Count > 0)
                        {
                            foreach (var itemxoa in item.BH_HoaDon_ChiTiet)
                            {
                                double giavonnew = 0;
                                double soluongxoa = 0;
                                Guid idqdnew = Guid.Empty;
                                DonViQuiDoi dvqd1 = classDonViQuiDoi.Get(idqd => idqd.ID == itemxoa.ID_DonViQuiDoi);
                                List<Guid> idquydoiitemxoa = classDonViQuiDoi.Select_DonViQuiDois_IDHangHoa(dvqd1.ID_HangHoa).Select(p => p.ID).ToList();
                                double ton = classHoaDon.TinhSLTonHHKK(dvqd1.ID_HangHoa, iddonvi, item.NgayLapHoaDon).Value;
                                double giatritoncu = itemxoa.GiaVon.Value * (ton + itemxoa.SoLuong) - (itemxoa.SoLuong * (itemxoa.DonGia - itemxoa.TienChietKhau) * (1 - (item.TongGiamGia / item.TongTienHang)));
                                foreach (BH_HoaDon itemHD in allHD)
                                {
                                    List<BH_HoaDon_ChiTiet> lhoadonchitiet = itemHD.BH_HoaDon_ChiTiet.Where(p => idquydoiitemxoa.Contains(p.ID_DonViQuiDoi)).ToList();
                                    if (lhoadonchitiet != null)
                                    {
                                        foreach (var item1 in lhoadonchitiet)
                                        {
                                            DonViQuiDoi dvqd = classDonViQuiDoi.Get(idqd => idqd.ID == item1.ID_DonViQuiDoi);

                                            Kho_TonKhoKhoiTao khokhoitao = _classTKKT.Get(idkho => idkho.ID_DonViQuiDoi == item1.ID_DonViQuiDoi);

                                            if (dvqd != null)
                                            {
                                                //dvqd.GiaVon = Math.Round(((((item1.GiaVon ?? 0) * khokhoitao.SoLuong) - (itemxoa.DonGia * itemxoa.SoLuong)) / (khokhoitao.SoLuong - itemxoa.SoLuong)), MidpointRounding.ToEven);
                                                //classDonViQuiDoi.Update_DonViQuiDoi(dvqd);

                                                BH_HoaDon_ChiTiet bhct = db.BH_HoaDon_ChiTiet.Find(item1.ID);
                                                bhct.GiaVon = Math.Round((giatritoncu + ((item1.DonGia - item1.TienChietKhau) * (1 - (itemHD.TongGiamGia / itemHD.TongTienHang))) * item1.SoLuong) / (ton + item1.SoLuong * dvqd.TyLeChuyenDoi), MidpointRounding.ToEven);
                                                giavonnew = bhct.GiaVon.Value;
                                                idqdnew = item1.ID_DonViQuiDoi;
                                                soluongxoa = itemxoa.SoLuong;
                                                classHoaDonChiTiet.Update_ChiTietHoaDonXH(bhct);
                                            }
                                            switch (itemHD.LoaiHoaDon)
                                            {
                                                case 1:
                                                    ton = ton - item1.SoLuong * dvqd.TyLeChuyenDoi;
                                                    break;
                                                case 3:
                                                    ton = ton - item1.SoLuong * dvqd.TyLeChuyenDoi;
                                                    break;
                                                case 4:
                                                    ton = ton + item1.SoLuong * dvqd.TyLeChuyenDoi;
                                                    break;
                                                default:
                                                    break;
                                            }
                                            giatritoncu = giatritoncu + (item1.DonGia - item1.TienChietKhau) * item1.SoLuong;
                                        }
                                        DonViQuiDoi dvqdnew = classDonViQuiDoi.Get(idqd => idqd.ID_HangHoa == dvqd1.ID_HangHoa && idqd.LaDonViChuan == true);
                                        dvqdnew.GiaVon = giavonnew;
                                        classDonViQuiDoi.Update_DonViQuiDoi(dvqdnew);

                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var itemxoa in item.BH_HoaDon_ChiTiet)
                            {
                                Guid idhoadon = classHoaDon.Gets(loaiHD => loaiHD.ChoThanhToan == false).Where(p => p.NgayLapHoaDon < item.NgayLapHoaDon).OrderByDescending(p => p.NgayLapHoaDon).Select(p => p.ID).First();
                                DonViQuiDoi dvqd1 = classDonViQuiDoi.Get(idqd => idqd.ID == itemxoa.ID_DonViQuiDoi);
                                List<Guid> idquydoiitemxoa = classDonViQuiDoi.Select_DonViQuiDois_IDHangHoa(dvqd1.ID_HangHoa).Select(p => p.ID).ToList();
                                //double giavon = classHoaDonChiTiet.Get(p => p.ID_HoaDon == idhoadon && idquydoiitemxoa.Contains(p.ID_DonViQuiDoi)).GiaVon.Value;
                                var objGiavon = classHoaDonChiTiet.Get(p => p.ID_HoaDon == idhoadon && idquydoiitemxoa.Contains(p.ID_DonViQuiDoi));
                                double _gvon = 0;
                                if (objGiavon != null)
                                {
                                    _gvon = objGiavon.GiaVon.Value;
                                }

                                DonViQuiDoi dvqdnew = classDonViQuiDoi.Get(idqd => idqd.ID_HangHoa == dvqd1.ID_HangHoa && idqd.LaDonViChuan == true);
                                dvqdnew.GiaVon = _gvon;
                                classDonViQuiDoi.Update_DonViQuiDoi(dvqdnew);
                            }
                        }
                        return "";
                    }
                    else
                    {
                        return "Update lỗi";
                    }
                }
            }
        }
        //load hàng hóa sao chep
        public List<BH_HoaDonDTO> getHHcoppy(Guid ID_HHCopy)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBH_HoaDon_ChiTiet classHoaDonChiTiet = new ClassBH_HoaDon_ChiTiet(db);
                    ClassDM_HangHoa classDMHangHoa = new ClassDM_HangHoa(db);

                    List<BH_HoaDonDTO> lsrReturns = new List<BH_HoaDonDTO>();
                    BH_HoaDonDTO itemData = new BH_HoaDonDTO
                    {
                        // phai viet nhu duoi day de tranh loi (LINQ to Entities does not recognize the method and this method)
                        BH_HoaDon_ChiTiet = classHoaDonChiTiet.Gets(ct => ct.ID_HoaDon == ID_HHCopy).Select(x => new
                        {
                            ID = x.ID,
                            ID_HoaDon = ID_HHCopy,
                            DonGia = x.DonGia,
                            GiaVon = x.GiaVon,
                            SoLuong = x.SoLuong,
                            ThanhTien = x.ThanhTien,
                            ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                            ID_HangHoa = x.DonViQuiDoi.ID_HangHoa,
                            MaHangHoa = x.DonViQuiDoi.MaHangHoa,
                            GiamGia = x.TienChietKhau,
                            ThoiGian = x.ThoiGian,
                            GhiChu = x.GhiChu,
                            SoThuTu = x.SoThuTu
                        }).AsEnumerable().Select(c => new BH_HoaDon_ChiTietDTO
                        {
                            ID = c.ID,
                            ID_HoaDon = c.ID_HoaDon,
                            DonGia = c.DonGia,
                            GiaVon = c.GiaVon,
                            SoLuong = c.SoLuong,
                            ThanhTien = c.ThanhTien,
                            ID_DonViQuiDoi = c.ID_DonViQuiDoi,
                            MaHangHoa = c.MaHangHoa,
                            TenHangHoa = classDMHangHoa.Select_HangHoaPRG(c.ID_HangHoa).FirstOrDefault().TenHangHoa,
                            ThuocTinh_GiaTri = classDMHangHoa.Select_HangHoaPRG(c.ID_HangHoa).FirstOrDefault().ThuocTinh_GiaTri,
                            GiamGia = c.GiamGia,
                            ThoiGian = c.ThoiGian,
                            GhiChu = c.GhiChu,
                            SoThuTu = c.SoThuTu
                        }).OrderBy(x => x.SoThuTu).ToList()

                    };
                    lsrReturns.Add(itemData);
                    return lsrReturns;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("BH_XuatHuyAPI_getHHcoppy: " + ex.Message + ex.InnerException);
                return null;
            }
        }
        // lấy danh sách xuất hủy full
        public List<ListLHPages> getAllPageXH<T>(List<T> lstLHs, float sohang)
        {
            List<ListLHPages> listPage = new List<ListLHPages>();
            if (lstLHs != null)
            {
                int dem = 1;
                float SoTrang = lstLHs.Count / sohang;
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
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult getListSelect_TenDonViTinh(Guid ID_DonViQuiDoi, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("ID_DonViQuiDoi", ID_DonViQuiDoi));
            sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
            List<TenDonViTinh_PRC> lst = db.Database.SqlQuery<TenDonViTinh_PRC>("exec getList_TenDonViTinh @ID_DonViQuiDoi, @ID_ChiNhanh", sql.ToArray()).ToList();
            List<TenDonViTinh_PRC> lst_GR = lst.GroupBy(x => x.ID_DonViQuiDoi).Select(t => new TenDonViTinh_PRC
            {
                ID_DonViQuiDoi = t.FirstOrDefault().ID_DonViQuiDoi,
                MaHangHoa = t.FirstOrDefault().MaHangHoa,
                TenDonViTinh = t.FirstOrDefault().TenDonViTinh,
                TyLeChuyenDoi = t.FirstOrDefault().TyLeChuyenDoi,
                TrangThai = t.FirstOrDefault().TrangThai
            }).ToList();
            JsonResultExample<TenDonViTinh_PRC> jsonobj = new JsonResultExample<TenDonViTinh_PRC>
            {
                LstData = lst,
                LstDataPrint = lst_GR
            };
            return Json(jsonobj);
        }

        [HttpGet]
        public IHttpActionResult PhieuXuatKho_XacNhanXuat(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var hd = db.BH_HoaDon.Find(idHoaDon);
                    DateTime dt = DateTime.Now;
                    if (hd != null)
                    {
                        db.Database.ExecuteSqlCommand("exec DISABLE TRIGGER dbo.UpdateNgayGiaoDichGanNhat_DMDoiTuong ON dbo.BH_HoaDon");
                        dt = hd.NgayLapHoaDon;// keep ngaylapold & return
                        hd.ChoThanhToan = false;
                        hd.NgaySua = DateTime.Now;
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("exec Enable TRIGGER dbo.UpdateNgayGiaoDichGanNhat_DMDoiTuong ON dbo.BH_HoaDon");
                    }
                    return ActionTrueData(dt);
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog(string.Concat("PhieuXuatKho_XacNhanXuat ", idHoaDon, ex.InnerException, ex.Message));
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }
        [HttpPost]
        public IHttpActionResult getDmXuatHuy(Params_GetListHoaDon param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                try
                {
                    List<BH_HoaDonDTO> lst = classHoaDon.GetListHDXuatKho(param);
                    int count = 0;
                    double gtriXuat = 0;
                    if (lst != null && lst.Count > 0)
                    {
                        count = lst[0].TotalRow ?? 0;
                        gtriXuat = lst[0].SumTongTienHang ?? 0;
                    }
                    return Json(new
                    {
                        res = true,
                        Rowcount = count,
                        LstData = lst,
                        _thanhtien = gtriXuat
                    });
                }
                catch (Exception ex)
                {
                    return Json(new { res = false, mes = ex.InnerException + ex.Message });
                }
            }
        }
        public IHttpActionResult getList_HangHoaXuatHuybyID(Guid ID_HoaDon, Guid ID_ChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("ID_HoaDon", ID_HoaDon));
                prm.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                List<XH_HoaDon_ChiTietPRC> lst = db.Database.SqlQuery<XH_HoaDon_ChiTietPRC>("exec getList_HangHoaXuatHuybyID @ID_HoaDon, @ID_ChiNhanh", prm.ToArray()).ToList();
                double soluong = (double?)lst.Sum(x => x.SoLuong) ?? 0;
                double giatri = (double?)lst.Sum(x => x.SoLuong * x.GiaVon) ?? 0;

                ClassBH_HoaDon_ChiTiet classHDCT = new ClassBH_HoaDon_ChiTiet(db);
                var _ClassDVQD = new classDonViQuiDoi(db);
                foreach (var item in lst)
                {
                    item.ThanhPhan_DinhLuong = classHDCT.SP_GetThanhPhanDinhLuong_CTHD(item.ID, 8);
                    item.DonViTinh = _ClassDVQD.Gets(ct => ct.ID_HangHoa == item.ID_HangHoa && ct.Xoa != true).Select(x => new DonViTinh
                    {
                        ID_HangHoa = x.ID_HangHoa,
                        TenDonViTinh = x.TenDonViTinh,
                        ID_DonViQuiDoi = x.ID,
                        QuanLyTheoLoHang = item.QuanLyTheoLoHang,
                        Xoa = false,
                        TyLeChuyenDoi = x.TyLeChuyenDoi
                    }).ToList();
                }

                JsonResultExample<XH_HoaDon_ChiTietPRC> jsonobj = new JsonResultExample<XH_HoaDon_ChiTietPRC>
                {
                    LstDataPrint = lst,
                    _tienvon = Math.Round(soluong, 3, MidpointRounding.ToEven),
                    _thanhtien = Math.Round(giatri, 0, MidpointRounding.ToEven),
                };
                return Json(jsonobj);
            }
        }

        public int getRowCountAll(string maXH, string hangHoa, string nhanVien, string nguoiTao, int loaihoadon, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3, string chinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                List<BC_XuatHuy> lst = _classXH.getAllXH(maXH, hangHoa, nhanVien, nguoiTao, loaihoadon, dayStart, dayEnd, trangthai1, trangthai2, trangthai3, chinhanh);
                return getRowsCountList(lst);
            }
        }
        public List<ListLHPages> getPageAll(string maXH, string hangHoa, string nhanVien, string nguoiTao, int loaihoadon, int sohang, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3, string chinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                List<BC_XuatHuy> lst = _classXH.getAllXH(maXH, hangHoa, nhanVien, nguoiTao, loaihoadon, dayStart, dayEnd, trangthai1, trangthai2, trangthai3, chinhanh);
                List<ListLHPages> listPage = new List<ListLHPages>();
                return getAllPage(lst, listPage, sohang);
            }
        }
        [HttpGet]
        public List<jqAutoResult> filterDanhMucHangHoa(Guid id_donvi, string txtSearch)
        {
            List<DM_HangHoa_XNT> listTon = new List<DM_HangHoa_XNT>();
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (txtSearch != null & txtSearch != "" & txtSearch != "null")
            {
                txtSearch = CommonStatic.ConvertToUnSign(txtSearch).ToLower();
                List<SqlParameter> paramlist = new List<SqlParameter>();
                Guid ID_ChiNhanh = id_donvi;
                string Search = "%" + txtSearch + "%";
                paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                paramlist.Add(new SqlParameter("Search", Search));
                listTon = db.Database.SqlQuery<DM_HangHoa_XNT>("exec filter_DanhMucHangHoa  @Search, @ID_ChiNhanh", paramlist.ToArray()).ToList();
            }
            return listTon.Select(p => new jqAutoResult
            {
                label = p.TenHangHoa,
                value = p.MaHangHoa,
                actual = p.ID_DonViQuiDoi.ToString(),
                data = p
            }).ToList();
        }
        //import danh sách hàng hóa
        [HttpPost]
        public IHttpActionResult ImfortExcelXuatHuy()
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
                            string str = classOffice.CheckFileMau_XuatHuy(excelstream);
                            if (str == null)
                            {
                                abc = classOffice.checkExcel_XuatHuy(excelstream);
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
        public IHttpActionResult getList_DanhSachHangXuatHuy(Guid ID_ChiNhanh)
        {
            string result = "";
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        List<Report_HangHoa_XuatHuy_Import> lstCT = new List<Report_HangHoa_XuatHuy_Import>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            lstCT = classOffice.getList_DanhSachHangXuatHuy(excelstream, ID_ChiNhanh);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, lstCT));
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

        //import danh sách hàng hóa điều chuyển
        [HttpPost]
        public IHttpActionResult ImfortExcelDieuChuyen()
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
                            string str = classOffice.CheckFileMau_DieuChuyen(excelstream);
                            if (str == null)
                            {
                                abc = classOffice.checkExcel_DieuChuyen(excelstream);
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
        public IHttpActionResult getList_DanhSachHangDieuChuyen(Guid iddonvi)
        {
            string result = "";
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        List<DM_HangHoaDTO> abc = new List<DM_HangHoaDTO>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            abc = classOffice.getList_DanhSachHangDieuChuyen(excelstream, iddonvi);
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
        // danh sách lô hàng
        [HttpGet]
        //public IHttpActionResult getListDM_LoHang(Guid ID_DonViQuiDoi)
        //{
        //    List<ListDM_LoHang> lst = ClassXuatHuy.getList_DMLoHang(ID_DonViQuiDoi);
        //    JsonResultExample<ListDM_LoHang> json = new JsonResultExample<ListDM_LoHang>
        //    {
        //        LstData = lst
        //    };
        //    return Json(json);
        //}
        public IHttpActionResult getListDMLoHang_byID(Guid ID_DonViQuiDoi, Guid ID_ChiNhanh, Guid ID_LoHang)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl_timeCSt = from cs in db.ChotSo
                              where cs.ID_DonVi == ID_ChiNhanh
                              select new
                              {
                                  cs.NgayChotSo
                              };
            string timeCS = string.Empty;
            try
            {
                timeCS = tbl_timeCSt.FirstOrDefault().NgayChotSo.ToString("yyyy-MM-dd");
            }
            catch
            {
                timeCS = "2016-01-01";
            }
            List<SqlParameter> sqlPRM = new List<SqlParameter>();
            sqlPRM.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
            sqlPRM.Add(new SqlParameter("ID_DonViQuiDoi", ID_DonViQuiDoi));
            sqlPRM.Add(new SqlParameter("ID_LoHang", ID_LoHang));
            sqlPRM.Add(new SqlParameter("timeChotSo", timeCS));
            List<ListDM_LoHang> lstLH = db.Database.SqlQuery<ListDM_LoHang>("exec getList_DMLoHangTonKho_byID @ID_ChiNhanh, @ID_DonViQuiDoi, @ID_LoHang, @timeChotSo", sqlPRM.ToArray()).ToList();
            JsonResultExample<ListDM_LoHang> json = new JsonResultExample<ListDM_LoHang>
            {
                LstData = lstLH
            };
            return Json(json);
        }
        [HttpGet]
        public IHttpActionResult getListDM_LoHang(Guid ID_DonViQuiDoi, Guid ID_ChiNhanh, Guid ID_NhanVien)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl_timeCSt = from cs in db.ChotSo
                              where cs.ID_DonVi == ID_ChiNhanh
                              select new
                              {
                                  cs.NgayChotSo
                              };
            string timeCS = "2016-01-01";
            if (tbl_timeCSt != null && tbl_timeCSt.Count() > 0)
            {
                timeCS = tbl_timeCSt.FirstOrDefault().NgayChotSo.ToString("yyyy-MM-dd");
            }

            List<SqlParameter> sqlPRM = new List<SqlParameter>();
            sqlPRM.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
            sqlPRM.Add(new SqlParameter("ID_DonViQuiDoi", ID_DonViQuiDoi));
            sqlPRM.Add(new SqlParameter("timeChotSo", timeCS));
            sqlPRM.Add(new SqlParameter("ID_NguoiDung", ID_NhanVien));
            List<ListDM_LoHang> lst = db.Database.SqlQuery<ListDM_LoHang>("exec getList_DMLoHang_TonKho @ID_ChiNhanh, @ID_DonViQuiDoi, @timeChotSo, @ID_NguoiDung", sqlPRM.ToArray()).ToList();
            JsonResultExample<ListDM_LoHang> json = new JsonResultExample<ListDM_LoHang>
            {
                LstData = lst
            };
            return Json(json);
        }
        [HttpGet]
        public IHttpActionResult getListDM_LoHangbyMaLoHang(string maHangHoa, string MaLoHang, Guid ID_ChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassXuatHuy _classXH = new ClassXuatHuy(db);
                List<ListDM_LoHang> lst = _classXH.getList_DMLoHangbyMaLoHang(maHangHoa, MaLoHang, ID_ChiNhanh);
                JsonResultExample<ListDM_LoHang> json = new JsonResultExample<ListDM_LoHang>
                {
                    LstData = lst
                };
                return Json(json);
            }
        }
        [HttpGet]
        public List<HT_CauHinhPhanMem> Chexk_CauHinhPhanMem(Guid ID_DonVi)
        {
            try
            {
                SsoftvnContext db = SystemDBContext.GetDBContext();
                List<HT_CauHinhPhanMem> lst = db.HT_CauHinhPhanMem.Where(x => x.ID_DonVi == ID_DonVi).ToList();
                return lst;
            }
            catch (Exception ex)
            {
                string str = CookieStore.GetCookieAes("SubDomain");
                CookieStore.WriteLog("Chexk_CauHinhPhanMem(Guid ID_DonVi): " + ex.InnerException + ex.Message, str);
                return null;
            }

        }
    }
    public class jqAutoResult
    {
        public string label { get; set; }
        public string value { get; set; }
        public string actual { get; set; }
        public DM_HangHoa_XNT data { get; set; }
    }
}
