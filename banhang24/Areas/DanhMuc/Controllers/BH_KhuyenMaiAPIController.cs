using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using libQuy_HoaDon;
using libHT_NguoiDung;
using static libQuy_HoaDon.ClassKhuyenMai;
using libDM_HangHoa;
using System.Data.SqlClient;
using libDM_DoiTuong;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class BH_KhuyenMaiAPIController : ApiController
    {
        #region Insert
        [HttpPost, ActionName("PostBH_KhuyenMai")]
        [ResponseType(typeof(DM_KhuyenMai))]
        public IHttpActionResult PostBH_KhuyenMai([FromBody] JObject data, string dateStart, string dateEnd, Guid ID_DonVi, Guid ID_NhanVien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);

                DM_KhuyenMai objKhuyenMai = data["objKhuyenMai"].ToObject<DM_KhuyenMai>();
                bool a = classKhuyenMai.Check_MaKhuyenMai(objKhuyenMai.MaKhuyenMai);
                if (a == true)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Mã chương trình khuyến mại đã tồn tại trên hệ thống"));
                }
                else
                {
                    string chitiet_km = string.Empty;
                    string noidung_km = string.Empty;
                    List<DM_KhuyenMai_ApDung> objKhuyenMaiApDung = data["objKhuyenMaiApDung"].ToObject<List<DM_KhuyenMai_ApDung>>();
                    List<DM_KhuyenMai_ChiTiet> objKhuyenMaiChiTiet = data["objKhuyenMaiChiTiet"].ToObject<List<DM_KhuyenMai_ChiTiet>>();
                    #region DM_KhuyenMai
                    DM_KhuyenMai itemDM_KhuyenMai = new DM_KhuyenMai { };
                    itemDM_KhuyenMai.ID = Guid.NewGuid();
                    string MaKhuyenMai = string.Empty;
                    if (objKhuyenMai.MaKhuyenMai == null || objKhuyenMai.MaKhuyenMai.Trim() == "" || objKhuyenMai.MaKhuyenMai == "null")
                    {
                        MaKhuyenMai = classKhuyenMai.GetAutoCode();
                    }
                    else
                    {
                        MaKhuyenMai = objKhuyenMai.MaKhuyenMai;
                    }
                    itemDM_KhuyenMai.MaKhuyenMai = MaKhuyenMai;

                    //set thời gian tạo phiếu
                    var ngaybd1 = dateStart.Replace("AM", "SA");
                    var ngaybd = ngaybd1.Replace("PM", "CH");
                    var ngaykt1 = dateEnd.Replace("AM", "SA");
                    var ngaykt = ngaykt1.Replace("PM", "CH");
                    itemDM_KhuyenMai.TenKhuyenMai = objKhuyenMai.TenKhuyenMai;
                    itemDM_KhuyenMai.GhiChu = objKhuyenMai.GhiChu;
                    itemDM_KhuyenMai.TrangThai = objKhuyenMai.TrangThai;
                    itemDM_KhuyenMai.HinhThuc = objKhuyenMai.HinhThuc;
                    itemDM_KhuyenMai.LoaiKhuyenMai = objKhuyenMai.LoaiKhuyenMai;
                    itemDM_KhuyenMai.ThoiGianBatDau = DateTime.Parse(ngaybd);
                    itemDM_KhuyenMai.ThoiGianKetThuc = DateTime.Parse(ngaykt);
                    itemDM_KhuyenMai.NgayApDung = objKhuyenMai.NgayApDung;
                    itemDM_KhuyenMai.ThangApDung = objKhuyenMai.ThangApDung;
                    itemDM_KhuyenMai.ThuApDung = objKhuyenMai.ThuApDung;
                    itemDM_KhuyenMai.GioApDung = objKhuyenMai.GioApDung;
                    itemDM_KhuyenMai.ApDungNgaySinhNhat = objKhuyenMai.ApDungNgaySinhNhat;
                    itemDM_KhuyenMai.TatCaDonVi = objKhuyenMai.TatCaDonVi;
                    itemDM_KhuyenMai.TatCaDoiTuong = objKhuyenMai.TatCaDoiTuong;
                    itemDM_KhuyenMai.TatCaNhanVien = objKhuyenMai.TatCaNhanVien;
                    itemDM_KhuyenMai.NguoiTao = objKhuyenMai.NguoiTao;
                    itemDM_KhuyenMai.NgayTao = DateTime.Now;
                    string trangthaiKM = objKhuyenMai.TrangThai == true ? "Kích hoạt, Khuyến mại theo: " : "Chưa áp dụng, Khuyến mại theo: ";
                    string loaiKM = objKhuyenMai.LoaiKhuyenMai == 1 ? "Hóa đơn, Hình thức: " : "Hàng hóa, Hình thức: ";
                    string hinhthucKM = string.Empty;
                    if (objKhuyenMai.HinhThuc == 11)
                        hinhthucKM = "Giảm giá hóa đơn";
                    else if (objKhuyenMai.HinhThuc == 12)
                        hinhthucKM = "Tặng hàng";
                    else if (objKhuyenMai.HinhThuc == 13)
                        hinhthucKM = "Giảm giá hàng";
                    else if (objKhuyenMai.HinhThuc == 14)
                        hinhthucKM = "Tặng điểm";
                    else if (objKhuyenMai.HinhThuc == 21)
                        hinhthucKM = "Mua hàng giảm giá hàng";
                    else if (objKhuyenMai.HinhThuc == 22)
                        hinhthucKM = "Mua hàng tặng hàng";
                    else if (objKhuyenMai.HinhThuc == 23)
                        hinhthucKM = "Mua hàng tặng điểm";
                    else if (objKhuyenMai.HinhThuc == 24)
                        hinhthucKM = "Giảm giá bán theo số lượng mua";
                    noidung_km = " Tên: " + objKhuyenMai.TenKhuyenMai + ", Tình trạng: " + trangthaiKM + loaiKM + hinhthucKM + " - Điều kiện: ";
                    chitiet_km = " Tên: " + objKhuyenMai.TenKhuyenMai + ", Tình trạng: " + trangthaiKM + loaiKM + hinhthucKM + " <br><br> - Điều kiện: ";
                    string thoigiamKM = "Hiệu lực từ ngày: " + itemDM_KhuyenMai.ThoiGianBatDau.ToString("dd/MM/yyyy HH:mm") + " đến " + itemDM_KhuyenMai.ThoiGianKetThuc.ToString("dd/MM/yyyy HH:mm");
                    string thangKM = objKhuyenMai.ThangApDung != "" ? "Theo tháng: tháng " + objKhuyenMai.ThangApDung.Replace("_", ", tháng ") : string.Empty;
                    string ngayKM = objKhuyenMai.NgayApDung != "" ? "Theo ngày: ngày " + objKhuyenMai.NgayApDung.Replace("_", ", ngày ") : string.Empty;
                    string thuKM = objKhuyenMai.ThuApDung != "" ? "Theo thứ: thứ " + objKhuyenMai.ThuApDung.Replace("_", ", thứ ") : string.Empty;
                    thuKM = thuKM.Replace("thứ 8", "chủ nhật");
                    string sinhnhatKM = objKhuyenMai.ApDungNgaySinhNhat == 1 ? "Áp dụng vào ngày sinh nhật khách hàng" : objKhuyenMai.ApDungNgaySinhNhat == 2 ? "Áp dụng vào tuần sinh nhật khách hàng" : objKhuyenMai.ApDungNgaySinhNhat == 3 ? "Áp dụng vào tháng sinh nhật khách hàng" : string.Empty;
                    string GioKM = objKhuyenMai.GioApDung != "" ? "Theo giờ: " + objKhuyenMai.GioApDung.Replace("_", ", ") : string.Empty;
                    #endregion
                    string strIns = classKhuyenMai.Add_KhuyenMai(itemDM_KhuyenMai);
                    if (strIns != null && strIns != string.Empty)
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                    else
                    {
                        #region DM_KhuyenMai_ChiTiet
                        int i = 1;
                        foreach (var item in objKhuyenMaiChiTiet)
                        {
                            DM_KhuyenMai_ChiTiet KM_ChiTiet = new DM_KhuyenMai_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_KhuyenMai = itemDM_KhuyenMai.ID,
                                TongTienHang = item.TongTienHang,
                                GiamGia = item.GiamGia,
                                GiamGiaTheoPhanTram = item.GiamGiaTheoPhanTram,
                                ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                ID_NhomHangHoa = item.ID_NhomHangHoa,
                                SoLuong = item.SoLuong,
                                ID_DonViQuiDoiMua = item.ID_DonViQuiDoiMua,
                                ID_NhomHangHoaMua = item.ID_NhomHangHoaMua,
                                SoLuongMua = item.SoLuongMua,
                                GiaKhuyenMai = item.GiaKhuyenMai,
                                STT = i
                            };
                            if (objKhuyenMai.HinhThuc == 11)
                            {
                                string pt = item.GiamGiaTheoPhanTram == true ? " %" : " VNĐ";
                                noidung_km = noidung_km + "Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " Giảm giá " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt;
                                chitiet_km = chitiet_km + "<br>Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " Giảm giá " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt;
                            }
                            else if (objKhuyenMai.HinhThuc == 12)
                            {
                                string nameHH = string.Empty;
                                string nameHH_load = string.Empty;
                                DonViQuiDoi DQ = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoi);
                                if (DQ != null)
                                {
                                    nameHH = DQ.MaHangHoa;
                                    nameHH_load = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + "</a>";
                                }
                                else
                                {
                                    DM_NhomHangHoa NH = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoa);
                                    nameHH = NH.TenNhomHangHoa;
                                    nameHH_load = NH.TenNhomHangHoa;
                                }
                                noidung_km = noidung_km + "Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " tặng " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH;
                                chitiet_km = chitiet_km + "<br>Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " tặng " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH_load;
                            }
                            else if (objKhuyenMai.HinhThuc == 13)
                            {
                                string nameHH = string.Empty;
                                string nameHH_load = string.Empty;
                                DonViQuiDoi DQ = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoi);
                                if (DQ != null)
                                {
                                    nameHH = DQ.MaHangHoa;
                                    nameHH_load = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + "</a>";
                                }
                                else
                                {
                                    DM_NhomHangHoa NH = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoa);
                                    nameHH = NH.TenNhomHangHoa;
                                    nameHH_load = NH.TenNhomHangHoa;
                                }
                                string pt = item.GiamGiaTheoPhanTram == true ? " %" : " VNĐ";
                                noidung_km = noidung_km + "Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " Giảm giá " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt + " cho " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH;
                                chitiet_km = chitiet_km + "<br>Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " Giảm giá " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt + " cho " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH_load;
                            }
                            else if (objKhuyenMai.HinhThuc == 14)
                            {
                                string pt = item.GiamGiaTheoPhanTram == true ? " %" : " điểm";
                                noidung_km = noidung_km + "Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " Điểm cộng " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt;
                                chitiet_km = chitiet_km + "<br>Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " Điểm cộng " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt;
                            }
                            else if (objKhuyenMai.HinhThuc == 21)
                            {
                                string nameHH = string.Empty;
                                string nameHHT = string.Empty;
                                string nameHH_LS = string.Empty;
                                string nameHHT_LS = string.Empty;
                                DonViQuiDoi DQ = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoiMua);
                                if (DQ != null)
                                {
                                    nameHH = DQ.MaHangHoa;
                                    nameHH_LS = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + "</a>";
                                }
                                else
                                {
                                    DM_NhomHangHoa NH = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoaMua);
                                    nameHH = NH.TenNhomHangHoa;
                                    nameHH_LS = NH.TenNhomHangHoa;
                                }
                                DonViQuiDoi DQT = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoi);
                                if (DQT != null)
                                {
                                    nameHHT = DQT.MaHangHoa;
                                    nameHHT_LS = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQT.MaHangHoa + "')\" >" + DQT.MaHangHoa + "</a>";
                                }
                                else
                                {
                                    DM_NhomHangHoa NHT = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoa);
                                    nameHHT = NHT.TenNhomHangHoa;
                                    nameHHT_LS = NHT.TenNhomHangHoa;
                                }
                                string pt = item.GiamGiaTheoPhanTram == true ? " %" : " VNĐ";
                                noidung_km = noidung_km + "Mua " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH + " Giảm giá " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt + " cho " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHHT;
                                chitiet_km = chitiet_km + "<br>Mua " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH_LS + " Giảm giá " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt + " cho " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHHT_LS;
                            }
                            else if (objKhuyenMai.HinhThuc == 22)
                            {
                                string nameHH = string.Empty;
                                string nameHHT = string.Empty;
                                string nameHH_LS = string.Empty;
                                string nameHHT_LS = string.Empty;
                                DonViQuiDoi DQ = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoiMua);
                                if (DQ != null)
                                {
                                    nameHH = DQ.MaHangHoa;
                                    nameHH_LS = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + "</a>";
                                }
                                else
                                {
                                    DM_NhomHangHoa NH = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoaMua);
                                    nameHH = NH.TenNhomHangHoa;
                                    nameHH_LS = NH.TenNhomHangHoa;
                                }
                                DonViQuiDoi DQT = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoi);
                                if (DQT != null)
                                {
                                    nameHHT = DQT.MaHangHoa;
                                    nameHHT_LS = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQT.MaHangHoa + "')\" >" + DQT.MaHangHoa + "</a>";
                                }
                                else
                                {
                                    DM_NhomHangHoa NHT = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoa);
                                    nameHHT = NHT.TenNhomHangHoa;
                                    nameHHT_LS = NHT.TenNhomHangHoa;
                                }
                                noidung_km = noidung_km + "Mua " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH + " tặng " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHHT;
                                chitiet_km = chitiet_km + "<br>Mua " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH_LS + " tặng " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHHT_LS;
                            }
                            else if (objKhuyenMai.HinhThuc == 23)
                            {
                                string nameHH = string.Empty;
                                string nameHH_LS = string.Empty;
                                DonViQuiDoi DQ = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoiMua);
                                if (DQ != null)
                                {
                                    nameHH = DQ.MaHangHoa;
                                    nameHH_LS = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + "</a>";
                                }
                                else
                                {
                                    DM_NhomHangHoa NH = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoaMua);
                                    nameHH = NH.TenNhomHangHoa;
                                    nameHH_LS = NH.TenNhomHangHoa;
                                }
                                string pt = item.GiamGiaTheoPhanTram == true ? " %" : " điểm";
                                noidung_km = noidung_km + "Mua " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH + " Điểm cộng " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt;
                                chitiet_km = chitiet_km + "<br>Mua " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH_LS + " Điểm cộng " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt;
                            }
                            else if (objKhuyenMai.HinhThuc == 24)
                            {
                                string nameHH = string.Empty;
                                string nameHH_LS = string.Empty;
                                DonViQuiDoi DQ = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoiMua);
                                if (DQ != null)
                                {
                                    nameHH = DQ.MaHangHoa;
                                    nameHH_LS = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + "</a>";
                                }
                                else
                                {
                                    DM_NhomHangHoa NH = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoaMua);
                                    nameHH = NH.TenNhomHangHoa;
                                    nameHH_LS = NH.TenNhomHangHoa;
                                }
                                noidung_km = noidung_km + "Khi mua " + nameHH + " số lượng từ " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " giá " + string.Format("{0:#,##0.##}", item.GiaKhuyenMai).Replace(".", ",");
                                chitiet_km = chitiet_km + "<br>Khi mua " + nameHH_LS + " số lượng từ " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " giá " + string.Format("{0:#,##0.##}", item.GiaKhuyenMai).Replace(".", ",");
                            }
                            strIns = classKhuyenMai.Add_KhuyenMaiChiTiet(KM_ChiTiet);
                            i = i + 1;
                        }
                        noidung_km = noidung_km + ". Thời gian áp dụng:  " + thoigiamKM;
                        chitiet_km = chitiet_km + "<br><br>- Thời gian áp dụng: <br>" + thoigiamKM;
                        if (thangKM != string.Empty)
                        {
                            noidung_km = noidung_km + ". " + thangKM;
                            chitiet_km = chitiet_km + "<br>" + thangKM;
                        }
                        if (ngayKM != string.Empty)
                        {
                            noidung_km = noidung_km + ". " + ngayKM;
                            chitiet_km = chitiet_km + "<br>" + ngayKM;
                        }
                        if (thuKM != string.Empty)
                        {
                            noidung_km = noidung_km + ". " + thuKM;
                            chitiet_km = chitiet_km + "<br>" + thuKM;
                        }
                        if (GioKM != string.Empty)
                        {
                            noidung_km = noidung_km + ". " + GioKM;
                            chitiet_km = chitiet_km + "<br>" + GioKM;
                        }
                        if (sinhnhatKM != string.Empty)
                        {
                            noidung_km = noidung_km + ". " + sinhnhatKM;
                            chitiet_km = chitiet_km + "<br>" + sinhnhatKM;
                        }
                        noidung_km = noidung_km + ". Phạm vi áp dụng: ";
                        chitiet_km = chitiet_km + "<br><br>- Phạm vi áp dụng: ";
                        #endregion
                        #region DM_KhuyenMaiApDung
                        string chinhanhKM = "Toàn hệ thống";
                        string nguoibanKM = "Toàn bộ người bán";
                        string khachhangKM = "Toàn bộ khách hàng";
                        int k = 0;
                        int l = 0;
                        int m = 0;
                        Guid? checkID_DonVi = null;
                        Guid? checkID_NhanVien = null;
                        Guid? checkID_NhomKhachHang = null;
                        foreach (var item in objKhuyenMaiApDung)
                        {
                            DM_KhuyenMai_ApDung KM_Apdung = new DM_KhuyenMai_ApDung
                            {
                                ID = Guid.NewGuid(),
                                ID_KhuyenMai = itemDM_KhuyenMai.ID,
                                ID_DonVi = item.ID_DonVi,
                                ID_NhanVien = item.ID_NhanVien,
                                ID_NhomKhachHang = item.ID_NhomKhachHang,
                            };
                            if (item.ID_DonVi != null || item.ID_NhanVien != null || item.ID_NhomKhachHang != null)
                            {
                                strIns = classKhuyenMai.Add_KhuyenMaiApDung(KM_Apdung);
                            }
                        }
                        foreach (var item in objKhuyenMaiApDung.OrderBy(x => x.ID_DonVi))
                        {
                            if (item.ID_DonVi != null)
                            {
                                DM_DonVi DV = classHoaDon.getList_DonVibyID(item.ID_DonVi);
                                if (DV != null & checkID_DonVi != item.ID_DonVi)
                                {
                                    if (k == 0)
                                        chinhanhKM = DV.TenDonVi;
                                    else
                                        chinhanhKM = chinhanhKM + ", " + DV.TenDonVi;
                                    k = k + 1;
                                    checkID_DonVi = item.ID_DonVi;
                                }
                            }
                        }
                        foreach (var item in objKhuyenMaiApDung.OrderBy(x => x.ID_NhanVien))
                        {
                            if (item.ID_NhanVien != null)
                            {
                                NS_NhanVien NV = classHoaDon.getList_NhanVienbyID(item.ID_NhanVien);
                                if (NV != null & checkID_NhanVien != item.ID_NhanVien)
                                {
                                    if (l == 0)
                                        nguoibanKM = NV.TenNhanVien;
                                    else
                                        nguoibanKM = nguoibanKM + ", " + NV.TenNhanVien;
                                    l = l + 1;
                                    checkID_NhanVien = item.ID_NhanVien;
                                }
                            }
                        }
                        foreach (var item in objKhuyenMaiApDung.OrderBy(x => x.ID_NhomKhachHang))
                        {
                            if (item.ID_NhomKhachHang != null)
                            {
                                DM_NhomDoiTuong DT = classHoaDon.getList_NhomKhachHangbyID(item.ID_NhomKhachHang);
                                if (DT != null & checkID_NhomKhachHang != item.ID_NhomKhachHang)
                                {
                                    if (m == 0)
                                        khachhangKM = DT.TenNhomDoiTuong;
                                    else
                                        khachhangKM = khachhangKM + ", " + DT.TenNhomDoiTuong;
                                    m = m + 1;
                                    checkID_NhomKhachHang = item.ID_NhomKhachHang;
                                }
                            }
                        }
                        noidung_km = noidung_km + "Chi nhánh: " + chinhanhKM + ". Người bán: " + nguoibanKM + ". Khách hàng: " + khachhangKM;
                        chitiet_km = chitiet_km + "<br>Chi nhánh: " + chinhanhKM + "<br>Người bán: " + nguoibanKM + "<br>Khách hàng: " + khachhangKM;
                        #endregion
                        HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                        {
                            ID = Guid.NewGuid(),
                            ID_NhanVien = ID_NhanVien,
                            ID_DonVi = ID_DonVi,
                            ChucNang = "Khuyến mại",
                            ThoiGian = DateTime.Now,
                            NoiDung = "Thêm mới chương trình khuyến mại: " + MaKhuyenMai + noidung_km,
                            NoiDungChiTiet = "Thêm mới chương trình khuyến mại: <a style= \"cursor: pointer\" onclick = \"loadKhuyenMaibyMaKM('" + MaKhuyenMai + "')\" >" + MaKhuyenMai + "</a> <br>" + chitiet_km,
                            LoaiNhatKy = 1
                        };
                        string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                        return Json(new { data = new { itemDM_KhuyenMai.ID } });
                    }
                }
            }
        }
        [HttpPost, ActionName("PostBH_ChotSo")]
        [ResponseType(typeof(ChotSo))]
        public IHttpActionResult PostBH_ChotSo([FromBody] JObject data)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                    List<ChotSo> objectChotSo = data["objChotSo"].ToObject<List<ChotSo>>();

                    db.HT_CauHinhPhanMem.ToList().ForEach(x => x.KhoaSo = false);

                    foreach (var item in objectChotSo)
                    {
                        string str = classKhuyenMai.Delete_ChotSo(item.ID_DonVi);
                        ChotSo item_ChotSo = new ChotSo
                        {
                            ID = Guid.NewGuid(),
                            ID_DonVi = item.ID_DonVi,
                            NgayChotSo = item.NgayChotSo
                        };
                        string str1 = classKhuyenMai.Add_ChotSo(item_ChotSo);

                        db.HT_CauHinhPhanMem.Where(x => x.ID_DonVi == item.ID_DonVi).ToList().ForEach(x => x.KhoaSo = true);
                    }
                    db.SaveChanges();
                    return Json(new { res = true, mes = string.Empty });
                }
            }
            catch (Exception ex)
            {
                string err = string.Concat("PostBH_ChotSoChiTiet ", ex.InnerException, ex.Message);
                return Json(new { res = true, mes = err });
            }
        }
        [HttpPost, ActionName("PostBH_ChotSoChiTiet")]
        [ResponseType(typeof(ChotSo_HangHoa))]
        public IHttpActionResult PostBH_ChotSoChiTiet([FromBody] JObject data)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            ChotSo item_ChotSo = new ChotSo { };
            try
            {
                List<ChotSo> objChotSo_ChiTiet = data["objChotSo_ChiTiet"].ToObject<List<ChotSo>>();
                foreach (var item in objChotSo_ChiTiet)
                {
                    List<SqlParameter> paramlist = new List<SqlParameter>();
                    List<SqlParameter> paramlist_KH = new List<SqlParameter>();
                    paramlist.Add(new SqlParameter("timeEnd", item.NgayChotSo));
                    paramlist.Add(new SqlParameter("ID_ChiNhanh", item.ID_DonVi));
                    paramlist_KH.Add(new SqlParameter("timeEnd", item.NgayChotSo));
                    paramlist_KH.Add(new SqlParameter("ID_ChiNhanh", item.ID_DonVi));
                    //db.Database.ExecuteSqlCommand("exec insertChotSo_XuatNhapTon @timeEnd, @ID_ChiNhanh", paramlist.ToArray());// not use table ChotSo_HangHoa
                    //db.Database.ExecuteSqlCommand("exec insertChotSoKhachHang_CongNo @timeEnd, @ID_ChiNhanh", paramlist_KH.ToArray());
                }
                return Json(new { res = true, mes = string.Empty });
            }
            catch (Exception ex)
            {
                string err = string.Concat("PostBH_ChotSoChiTiet ", ex.InnerException, ex.Message);
                return Json(new { res = true, mes = err });
            }
        }
        #endregion
        #region Update
        [ResponseType(typeof(string))]
        public IHttpActionResult PutBH_KhuyenMai([FromBody] JObject data, string dateStart, string dateEnd, Guid ID_DonVi, Guid ID_NhanVien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);

                //Guid id = data["id"].ToObject<Guid>();
                string chitiet_km = string.Empty;
                string noidung_km = string.Empty;
                DM_KhuyenMai objKhuyenMai = data["objKhuyenMai"].ToObject<DM_KhuyenMai>();
                #region DM_KhuyenMai
                DM_KhuyenMai itemDM_KhuyenMai = new DM_KhuyenMai { };
                itemDM_KhuyenMai.ID = objKhuyenMai.ID;
                itemDM_KhuyenMai.MaKhuyenMai = objKhuyenMai.MaKhuyenMai;
                //set thời gian tạo phiếu
                var ngaybd1 = dateStart.Replace("AM", "SA");
                var ngaybd = ngaybd1.Replace("PM", "CH");
                var ngaykt1 = dateEnd.Replace("AM", "SA");
                var ngaykt = ngaykt1.Replace("PM", "CH");
                itemDM_KhuyenMai.TenKhuyenMai = objKhuyenMai.TenKhuyenMai;
                itemDM_KhuyenMai.GhiChu = objKhuyenMai.GhiChu;
                itemDM_KhuyenMai.TrangThai = objKhuyenMai.TrangThai;
                itemDM_KhuyenMai.HinhThuc = objKhuyenMai.HinhThuc;
                itemDM_KhuyenMai.LoaiKhuyenMai = objKhuyenMai.LoaiKhuyenMai;
                itemDM_KhuyenMai.ThoiGianBatDau = DateTime.Parse(ngaybd);
                itemDM_KhuyenMai.ThoiGianKetThuc = DateTime.Parse(ngaykt);
                itemDM_KhuyenMai.NgayApDung = objKhuyenMai.NgayApDung; //Sang tuần làm
                itemDM_KhuyenMai.ThangApDung = objKhuyenMai.ThangApDung;
                itemDM_KhuyenMai.ThuApDung = objKhuyenMai.ThuApDung;
                itemDM_KhuyenMai.GioApDung = objKhuyenMai.GioApDung;
                itemDM_KhuyenMai.ApDungNgaySinhNhat = objKhuyenMai.ApDungNgaySinhNhat;
                itemDM_KhuyenMai.TatCaDonVi = objKhuyenMai.TatCaDonVi;
                itemDM_KhuyenMai.TatCaDoiTuong = objKhuyenMai.TatCaDoiTuong;
                itemDM_KhuyenMai.TatCaNhanVien = objKhuyenMai.TatCaNhanVien;
                itemDM_KhuyenMai.NguoiSua = objKhuyenMai.NguoiSua;
                string trangthaiKM = objKhuyenMai.TrangThai == true ? "Kích hoạt, Khuyến mại theo: " : "Chưa áp dụng, Khuyến mại theo: ";
                string loaiKM = objKhuyenMai.LoaiKhuyenMai == 1 ? "Hóa đơn, Hình thức: " : "Hàng hóa, Hình thức: ";
                string hinhthucKM = string.Empty;
                if (objKhuyenMai.HinhThuc == 11)
                    hinhthucKM = "Giảm giá hóa đơn";
                else if (objKhuyenMai.HinhThuc == 12)
                    hinhthucKM = "Tặng hàng";
                else if (objKhuyenMai.HinhThuc == 13)
                    hinhthucKM = "Giảm giá hàng";
                else if (objKhuyenMai.HinhThuc == 14)
                    hinhthucKM = "Tặng điểm";
                else if (objKhuyenMai.HinhThuc == 21)
                    hinhthucKM = "Mua hàng giảm giá hàng";
                else if (objKhuyenMai.HinhThuc == 22)
                    hinhthucKM = "Mua hàng tặng hàng";
                else if (objKhuyenMai.HinhThuc == 23)
                    hinhthucKM = "Mua hàng tặng điểm";
                else if (objKhuyenMai.HinhThuc == 24)
                    hinhthucKM = "Giảm giá bán theo số lượng mua";
                noidung_km = " Tên: " + objKhuyenMai.TenKhuyenMai + ", Tình trạng: " + trangthaiKM + loaiKM + hinhthucKM + " - Điều kiện: ";
                chitiet_km = " Tên: " + objKhuyenMai.TenKhuyenMai + ", Tình trạng: " + trangthaiKM + loaiKM + hinhthucKM + " <br><br> - Điều kiện: ";
                string thoigiamKM = "Hiệu lực từ ngày: " + itemDM_KhuyenMai.ThoiGianBatDau.ToString("dd/MM/yyyy HH:mm") + " đến " + itemDM_KhuyenMai.ThoiGianKetThuc.ToString("dd/MM/yyyy HH:mm");
                string thangKM = objKhuyenMai.ThangApDung != "" ? "Theo tháng: tháng " + objKhuyenMai.ThangApDung.Replace("_", ", tháng ") : string.Empty;
                string ngayKM = objKhuyenMai.NgayApDung != "" ? "Theo ngày: ngày " + objKhuyenMai.NgayApDung.Replace("_", ", ngày ") : string.Empty;
                string thuKM = objKhuyenMai.ThuApDung != "" ? "Theo thứ: thứ " + objKhuyenMai.ThuApDung.Replace("_", ", thứ ") : string.Empty;
                thuKM = thuKM.Replace("thứ 8", "chủ nhật");
                string sinhnhatKM = objKhuyenMai.ApDungNgaySinhNhat == 1 ? "Áp dụng vào ngày sinh nhật khách hàng" : objKhuyenMai.ApDungNgaySinhNhat == 2 ? "Áp dụng vào tuần sinh nhật khách hàng" : objKhuyenMai.ApDungNgaySinhNhat == 3 ? "Áp dụng vào tháng sinh nhật khách hàng" : string.Empty;
                string GioKM = objKhuyenMai.GioApDung != "" ? "Theo giờ: " + objKhuyenMai.GioApDung.Replace("_", ", ") : string.Empty;

                string strUpd = classKhuyenMai.Update_DMKhuyenMai(itemDM_KhuyenMai);
                #endregion
                if (strUpd != null && strUpd != string.Empty)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                }
                else
                {
                    string strdel = classKhuyenMai.Delete_KhuyenMaiChiTiet(itemDM_KhuyenMai.ID);
                    strdel = classKhuyenMai.Delete_KhuyenMaiApDung(itemDM_KhuyenMai.ID);
                    List<DM_KhuyenMai_ApDung> objKhuyenMaiApDung = data["objKhuyenMaiApDung"].ToObject<List<DM_KhuyenMai_ApDung>>();
                    List<DM_KhuyenMai_ChiTiet> objKhuyenMaiChiTiet = data["objKhuyenMaiChiTiet"].ToObject<List<DM_KhuyenMai_ChiTiet>>();

                    #region DM_KhuyenMai_ChiTiet
                    int i = 1;
                    foreach (var item in objKhuyenMaiChiTiet)
                    {
                        DM_KhuyenMai_ChiTiet KM_ChiTiet = new DM_KhuyenMai_ChiTiet
                        {
                            ID = Guid.NewGuid(),
                            ID_KhuyenMai = itemDM_KhuyenMai.ID,
                            TongTienHang = item.TongTienHang,
                            GiamGia = item.GiamGia,
                            GiamGiaTheoPhanTram = item.GiamGiaTheoPhanTram,
                            ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                            ID_NhomHangHoa = item.ID_NhomHangHoa,
                            SoLuong = item.SoLuong,
                            ID_DonViQuiDoiMua = item.ID_DonViQuiDoiMua,
                            ID_NhomHangHoaMua = item.ID_NhomHangHoaMua,
                            SoLuongMua = item.SoLuongMua,
                            GiaKhuyenMai = item.GiaKhuyenMai,
                            STT = i
                        };
                        if (objKhuyenMai.HinhThuc == 11)
                        {
                            string pt = item.GiamGiaTheoPhanTram == true ? " %" : " VNĐ";
                            noidung_km = noidung_km + "Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " Giảm giá " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt;
                            chitiet_km = chitiet_km + "<br>Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " Giảm giá " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt;
                        }
                        else if (objKhuyenMai.HinhThuc == 12)
                        {
                            string nameHH = string.Empty;
                            string nameHH_load = string.Empty;
                            DonViQuiDoi DQ = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoi);
                            if (DQ != null)
                            {
                                nameHH = DQ.MaHangHoa;
                                nameHH_load = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + "</a>";
                            }
                            else
                            {
                                DM_NhomHangHoa NH = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoa);
                                nameHH = NH.TenNhomHangHoa;
                                nameHH_load = NH.TenNhomHangHoa;
                            }
                            noidung_km = noidung_km + "Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " tặng " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH;
                            chitiet_km = chitiet_km + "<br>Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " tặng " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH_load;
                        }
                        else if (objKhuyenMai.HinhThuc == 13)
                        {
                            string nameHH = string.Empty;
                            string nameHH_load = string.Empty;
                            DonViQuiDoi DQ = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoi);
                            if (DQ != null)
                            {
                                nameHH = DQ.MaHangHoa;
                                nameHH_load = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + "</a>";
                            }
                            else
                            {
                                DM_NhomHangHoa NH = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoa);
                                nameHH = NH.TenNhomHangHoa;
                                nameHH_load = NH.TenNhomHangHoa;
                            }
                            string pt = item.GiamGiaTheoPhanTram == true ? " %" : " VNĐ";
                            noidung_km = noidung_km + "Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " Giảm giá " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt + " cho " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH;
                            chitiet_km = chitiet_km + "<br>Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " Giảm giá " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt + " cho " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH_load;
                        }
                        else if (objKhuyenMai.HinhThuc == 14)
                        {
                            string pt = item.GiamGiaTheoPhanTram == true ? " %" : " điểm";
                            noidung_km = noidung_km + "Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " Điểm cộng " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt;
                            chitiet_km = chitiet_km + "<br>Tổng tiền hàng từ " + string.Format("{0:#,##0.##}", item.TongTienHang).Replace(".", ",") + " Điểm cộng " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt;
                        }
                        else if (objKhuyenMai.HinhThuc == 21)
                        {
                            string nameHH = string.Empty;
                            string nameHHT = string.Empty;
                            string nameHH_LS = string.Empty;
                            string nameHHT_LS = string.Empty;
                            DonViQuiDoi DQ = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoiMua);
                            if (DQ != null)
                            {
                                nameHH = DQ.MaHangHoa;
                                nameHH_LS = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + "</a>";
                            }
                            else
                            {
                                DM_NhomHangHoa NH = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoaMua);
                                nameHH = NH.TenNhomHangHoa;
                                nameHH_LS = NH.TenNhomHangHoa;
                            }
                            DonViQuiDoi DQT = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoi);
                            if (DQT != null)
                            {
                                nameHHT = DQT.MaHangHoa;
                                nameHHT_LS = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQT.MaHangHoa + "')\" >" + DQT.MaHangHoa + "</a>";
                            }
                            else
                            {
                                DM_NhomHangHoa NHT = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoa);
                                nameHHT = NHT.TenNhomHangHoa;
                                nameHHT_LS = NHT.TenNhomHangHoa;
                            }
                            string pt = item.GiamGiaTheoPhanTram == true ? " %" : " VNĐ";
                            noidung_km = noidung_km + "Mua " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH + " Giảm giá " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt + " cho " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHHT;
                            chitiet_km = chitiet_km + "<br>Mua " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH_LS + " Giảm giá " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt + " cho " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHHT_LS;
                        }
                        else if (objKhuyenMai.HinhThuc == 22)
                        {
                            string nameHH = string.Empty;
                            string nameHHT = string.Empty;
                            string nameHH_LS = string.Empty;
                            string nameHHT_LS = string.Empty;
                            DonViQuiDoi DQ = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoiMua);
                            if (DQ != null)
                            {
                                nameHH = DQ.MaHangHoa;
                                nameHH_LS = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + "</a>";
                            }
                            else
                            {
                                DM_NhomHangHoa NH = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoaMua);
                                nameHH = NH.TenNhomHangHoa;
                                nameHH_LS = NH.TenNhomHangHoa;
                            }
                            DonViQuiDoi DQT = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoi);
                            if (DQT != null)
                            {
                                nameHHT = DQT.MaHangHoa;
                                nameHHT_LS = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQT.MaHangHoa + "')\" >" + DQT.MaHangHoa + "</a>";
                            }
                            else
                            {
                                DM_NhomHangHoa NHT = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoa);
                                nameHHT = NHT.TenNhomHangHoa;
                                nameHHT_LS = NHT.TenNhomHangHoa;
                            }
                            noidung_km = noidung_km + "Mua " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH + " tặng " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHHT;
                            chitiet_km = chitiet_km + "<br>Mua " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH_LS + " tặng " + string.Format("{0:#,##0.##}", item.SoLuong).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHHT_LS;
                        }
                        else if (objKhuyenMai.HinhThuc == 23)
                        {
                            string nameHH = string.Empty;
                            string nameHH_LS = string.Empty;
                            DonViQuiDoi DQ = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoiMua);
                            if (DQ != null)
                            {
                                nameHH = DQ.MaHangHoa;
                                nameHH_LS = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + "</a>";
                            }
                            else
                            {
                                DM_NhomHangHoa NH = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoaMua);
                                nameHH = NH.TenNhomHangHoa;
                                nameHH_LS = NH.TenNhomHangHoa;
                            }
                            string pt = item.GiamGiaTheoPhanTram == true ? " %" : " điểm";
                            noidung_km = noidung_km + "Mua " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH + " Điểm cộng " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt;
                            chitiet_km = chitiet_km + "<br>Mua " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " " + nameHH_LS + " Điểm cộng " + string.Format("{0:#,##0.##}", item.GiamGia).Replace(",", "-").Replace(".", ",").Replace("-", ".") + pt;
                        }
                        else if (objKhuyenMai.HinhThuc == 24)
                        {
                            string nameHH = string.Empty;
                            string nameHH_LS = string.Empty;
                            DonViQuiDoi DQ = classHoaDon.getList_DonViQuyDoibyID(item.ID_DonViQuiDoiMua);
                            if (DQ != null)
                            {
                                nameHH = DQ.MaHangHoa;
                                nameHH_LS = "<a style= \"cursor: pointer\" onclick = \"loadHangHoabyMaHH('" + DQ.MaHangHoa + "')\" >" + DQ.MaHangHoa + "</a>";
                            }
                            else
                            {
                                DM_NhomHangHoa NH = classHoaDon.getList_NhomHangHoabyID(item.ID_NhomHangHoaMua);
                                nameHH = NH.TenNhomHangHoa;
                                nameHH_LS = NH.TenNhomHangHoa;
                            }
                            noidung_km = noidung_km + "Khi mua " + nameHH + " số lượng từ " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " giá " + string.Format("{0:#,##0.##}", item.GiaKhuyenMai);
                            chitiet_km = chitiet_km + "<br>Khi mua " + nameHH_LS + " số lượng từ " + string.Format("{0:#,##0.##}", item.SoLuongMua).Replace(",", "-").Replace(".", ",").Replace("-", ".") + " giá " + string.Format("{0:#,##0.##}", item.GiaKhuyenMai);
                        }
                        strUpd = classKhuyenMai.Add_KhuyenMaiChiTiet(KM_ChiTiet);
                        i = i + 1;
                    }
                    noidung_km = noidung_km + ". Thời gian áp dụng:  " + thoigiamKM;
                    chitiet_km = chitiet_km + "<br><br>- Thời gian áp dụng: <br>" + thoigiamKM;
                    if (thangKM != string.Empty)
                    {
                        noidung_km = noidung_km + ". " + thangKM;
                        chitiet_km = chitiet_km + "<br>" + thangKM;
                    }
                    if (ngayKM != string.Empty)
                    {
                        noidung_km = noidung_km + ". " + ngayKM;
                        chitiet_km = chitiet_km + "<br>" + ngayKM;
                    }
                    if (thuKM != string.Empty)
                    {
                        noidung_km = noidung_km + ". " + thuKM;
                        chitiet_km = chitiet_km + "<br>" + thuKM;
                    }
                    if (GioKM != string.Empty)
                    {
                        noidung_km = noidung_km + ". " + GioKM;
                        chitiet_km = chitiet_km + "<br>" + GioKM;
                    }
                    if (sinhnhatKM != string.Empty)
                    {
                        noidung_km = noidung_km + ". " + sinhnhatKM;
                        chitiet_km = chitiet_km + "<br>" + sinhnhatKM;
                    }
                    noidung_km = noidung_km + ". Phạm vi áp dụng: ";
                    chitiet_km = chitiet_km + "<br><br>- Phạm vi áp dụng: ";
                    #endregion
                    #region DM_KhuyenMaiApDung
                    string chinhanhKM = "Toàn hệ thống";
                    string nguoibanKM = "Toàn bộ người bán";
                    string khachhangKM = "Toàn bộ khách hàng";
                    int k = 0;
                    int l = 0;
                    int m = 0;
                    Guid? checkID_DonVi = null;
                    Guid? checkID_NhanVien = null;
                    Guid? checkID_NhomKhachHang = null;
                    foreach (var item in objKhuyenMaiApDung)
                    {
                        DM_KhuyenMai_ApDung KM_Apdung = new DM_KhuyenMai_ApDung
                        {
                            ID = Guid.NewGuid(),
                            ID_KhuyenMai = itemDM_KhuyenMai.ID,
                            ID_DonVi = item.ID_DonVi,
                            ID_NhanVien = item.ID_NhanVien,
                            ID_NhomKhachHang = item.ID_NhomKhachHang,
                        };
                        if (item.ID_DonVi != null || item.ID_NhanVien != null || item.ID_NhomKhachHang != null)
                        {
                            strUpd = classKhuyenMai.Add_KhuyenMaiApDung(KM_Apdung);
                        }
                    }
                    foreach (var item in objKhuyenMaiApDung.OrderBy(x => x.ID_DonVi))
                    {
                        if (item.ID_DonVi != null)
                        {
                            DM_DonVi DV = classHoaDon.getList_DonVibyID(item.ID_DonVi);
                            if (DV != null & checkID_DonVi != item.ID_DonVi)
                            {
                                if (k == 0)
                                    chinhanhKM = DV.TenDonVi;
                                else
                                    chinhanhKM = chinhanhKM + ", " + DV.TenDonVi;
                                k = k + 1;
                                checkID_DonVi = item.ID_DonVi;
                            }
                        }
                    }
                    foreach (var item in objKhuyenMaiApDung.OrderBy(x => x.ID_NhanVien))
                    {
                        if (item.ID_NhanVien != null)
                        {
                            NS_NhanVien NV = classHoaDon.getList_NhanVienbyID(item.ID_NhanVien);
                            if (NV != null & checkID_NhanVien != item.ID_NhanVien)
                            {
                                if (l == 0)
                                    nguoibanKM = NV.TenNhanVien;
                                else
                                    nguoibanKM = nguoibanKM + ", " + NV.TenNhanVien;
                                l = l + 1;
                                checkID_NhanVien = item.ID_NhanVien;
                            }
                        }
                    }
                    foreach (var item in objKhuyenMaiApDung.OrderBy(x => x.ID_NhomKhachHang))
                    {
                        if (item.ID_NhomKhachHang != null)
                        {
                            DM_NhomDoiTuong DT = classHoaDon.getList_NhomKhachHangbyID(item.ID_NhomKhachHang);
                            if (DT != null & checkID_NhomKhachHang != item.ID_NhomKhachHang)
                            {
                                if (m == 0)
                                    khachhangKM = DT.TenNhomDoiTuong;
                                else
                                    khachhangKM = khachhangKM + ", " + DT.TenNhomDoiTuong;
                                m = m + 1;
                                checkID_NhomKhachHang = item.ID_NhomKhachHang;
                            }
                        }
                    }
                    noidung_km = noidung_km + "Chi nhánh: " + chinhanhKM + ". Người bán: " + nguoibanKM + ". Khách hàng: " + khachhangKM;
                    chitiet_km = chitiet_km + "<br>Chi nhánh: " + chinhanhKM + "<br>Người bán: " + nguoibanKM + "<br>Khách hàng: " + khachhangKM;
                    #endregion
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                    {
                        ID = Guid.NewGuid(),
                        ID_NhanVien = ID_NhanVien,
                        ID_DonVi = ID_DonVi,
                        ChucNang = "Khuyến mại",
                        ThoiGian = DateTime.Now,
                        NoiDung = "Cập nhật chương trình khuyến mại: " + objKhuyenMai.MaKhuyenMai + noidung_km,
                        NoiDungChiTiet = "Cập nhật chương trình khuyến mại: <a style= \"cursor: pointer\" onclick = \"loadKhuyenMaibyMaKM('" + objKhuyenMai.MaKhuyenMai + "')\" >" + objKhuyenMai.MaKhuyenMai + "</a> <br>" + chitiet_km,
                        LoaiNhatKy = 2
                    };
                    string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                    return Json(new { data = new { itemDM_KhuyenMai.ID } });

                }
            }
        }
        #endregion
        #region Select
        public string get_CheckKhuyenMai(string MaKM)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                string tb = classKhuyenMai.Check_MaChuongTrinhKhuyenMai(MaKM);
                return tb;
            }
        }
        public string getNameNguoiDung(Guid ID_NguoiDung)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                #region "nguoitao"
                List<HT_NguoiDungDTO> lstAllVTs = _classHTND.getAllNguoiDung().ToList();
                string sNguoiTao = string.Empty;
                for (int i = 0; i < lstAllVTs.Count; i++)
                {
                    if (lstAllVTs[i].ID == ID_NguoiDung)
                    {
                        sNguoiTao = lstAllVTs[i].TenNguoiDung;
                        break;
                    }
                }
                return sNguoiTao;
                #endregion;
            }
        }
        public List<DM_NhomDoiTuong> getNhomDoiTuong()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                List<DM_NhomDoiTuong> lst = classKhuyenMai.GetNhomDoiTuong(1);
                return lst;
            }
        }
        public List<NS_NhanVien> getNhanViens(string nameChinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                List<NS_NhanVien> lst = classKhuyenMai.GetNhanVien(nameChinhanh);
                return lst;
            }
        }
        //Trả về dữ liệu theo trang selection
        public List<DM_ChuongTrinhKhuyenMai> getlistPage(List<DM_ChuongTrinhKhuyenMai> lst, List<DM_ChuongTrinhKhuyenMai> lstPage, int sohang, int Page)
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
        public int getRowsCountList(List<DM_ChuongTrinhKhuyenMai> lstLHs)
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
        public List<ListLHPages> getAllPagenew<T>(List<T> lstLHs, float sohang)
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

        public List<ListLHPages> getAllPage(List<DM_ChuongTrinhKhuyenMai> lstLHs, List<ListLHPages> listPage, float sohang)
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
        //lấy danh sách chương trình khuyến mại
        public System.Web.Http.Results.JsonResult<JsonResultExample<DM_ChuongTrinhKhuyenMaiPRC>> GetListChuongTrinhKM(string maKM, string Chinhanh, int TrangThai, int sohang, int page)
        {
            string maKM_search = string.Empty;
            if (maKM != null & maKM != "" & maKM != "null")
                maKM_search = "%" + maKM + "%";
            else
                maKM_search = "%%";
            string trangthai_Seach = string.Empty;
            if (TrangThai == 0)
                trangthai_Seach = "%%";
            else if (TrangThai == 1)
                trangthai_Seach = "%1%";
            else
                trangthai_Seach = "%0%";
            string[] ListDonVi = Chinhanh.Split(',');
            List<string> lstDonVi = new List<string>();
            for (int i = 0; i < ListDonVi.Length; i++)
            {
                lstDonVi.Add(ListDonVi[i].ToString());
            }
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<SqlParameter> parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("maKM", maKM_search));
            parameter.Add(new SqlParameter("TrangThai", trangthai_Seach));
            List<DM_ChuongTrinhKhuyenMaiPRC> lst = db.Database.SqlQuery<DM_ChuongTrinhKhuyenMaiPRC>("exec getList_ChuongTrinhKhuyenMai @maKM, @TrangThai", parameter.ToArray()).ToList();
            if (Chinhanh != null)
            {
                lst = lst.Where(x => lstDonVi.Contains(x.ID_DonVi.ToString()) || x.TatCaDonVi == true).ToList();
            }
            lst = lst.GroupBy(x => x.ID).Select(t => new DM_ChuongTrinhKhuyenMaiPRC
            {
                ID = t.FirstOrDefault().ID,
                MaKhuyenMai = t.FirstOrDefault().MaKhuyenMai,
                TenKhuyenMai = t.FirstOrDefault().TenKhuyenMai,
                GhiChu = t.FirstOrDefault().GhiChu,
                TrangThai = t.FirstOrDefault().TrangThai,
                HinhThuc = t.FirstOrDefault().HinhThuc,
                LoaiKhuyenMai = t.FirstOrDefault().LoaiKhuyenMai,
                KieuHinhThuc = t.FirstOrDefault().KieuHinhThuc,
                ThoiGianBatDau = t.FirstOrDefault().ThoiGianBatDau,
                ThoiGianKetThuc = t.FirstOrDefault().ThoiGianKetThuc,
                NgayApDung = t.FirstOrDefault().NgayApDung,
                ThangApDung = t.FirstOrDefault().ThangApDung,
                ThuApDung = t.FirstOrDefault().ThuApDung,
                GioApDung = t.FirstOrDefault().GioApDung,
                ApDungNgaySinhNhat = t.FirstOrDefault().ApDungNgaySinhNhat,
                ValueApDungSN = t.FirstOrDefault().ValueApDungSN,
                TatCaDoiTuong = t.FirstOrDefault().TatCaDoiTuong,
                TatCaDonVi = t.FirstOrDefault().TatCaDonVi,
                TatCaNhanVien = t.FirstOrDefault().TatCaNhanVien,
                NguoiTao = t.FirstOrDefault().NguoiTao
            }).ToList();
            int Row = lst.Count();
            List<ListLHPages> lstPage = getAllPagenew<DM_ChuongTrinhKhuyenMaiPRC>(lst, sohang);
            List<DM_ChuongTrinhKhuyenMaiPRC> lstKM = lst.Skip((page - 1) * sohang).Take(sohang).ToList();
            //List<ListLHPages> lstPages = getAllPage<Report_HangHoa_BanHangPRC>(lst, pageSize);
            JsonResultExample<DM_ChuongTrinhKhuyenMaiPRC> json = new JsonResultExample<DM_ChuongTrinhKhuyenMaiPRC>
            {
                Rowcount = Row,
                LstPageNumber = lstPage,
                LstData = lstKM
            };
            return Json(json);
        }
        [HttpGet, HttpPost]
        public IHttpActionResult GetListPromotion(ParamSearchPromotion lstParam)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                    List<DM_ChuongTrinhKhuyenMai> lst = classKhuyenMai.GetListPromotion(lstParam);
                    int totalRow = 0;
                    double totalPage = 0;
                    if (lst.Count() > 0)
                    {
                        totalRow = lst[0].TotalRow ?? 0;
                        totalPage = lst[0].TotalPage ?? 0;
                    }
                    return Json(new { res = true, data = lst, TotalRow = totalRow, TotalPage = totalPage });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }
        public int GetRowKM(string maKM, string Chinhanh, int TrangThai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                List<DM_ChuongTrinhKhuyenMai> lst = classKhuyenMai.getCTKhuyenMai(maKM, Chinhanh, TrangThai);
                return getRowsCountList(lst);
            }
        }
        public List<ListLHPages> getPageAll(string maKM, int TrangThai, string Chinhanh, int sohang)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                List<DM_ChuongTrinhKhuyenMai> lst = classKhuyenMai.getCTKhuyenMai(maKM, Chinhanh, TrangThai);
                List<ListLHPages> listPage = new List<ListLHPages>();
                return getAllPage(lst, listPage, sohang);
            }
        }
        public List<Object> getChiTiet_KhuyenMai(Guid ID_KhuyenMai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                List<Object> lst = classKhuyenMai.getChiTiet_KhuyenMai(ID_KhuyenMai);
                return lst;
            }
        }
        [HttpGet]
        public System.Web.Http.Results.JsonResult<JsonResultExample<DM_LichSuKhuyenMai>> getList_LichSuKhuyenMai(Guid ID_KhuyenMai, int numberPage, int PageSize)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<DM_LichSuKhuyenMai> lst = new List<DM_LichSuKhuyenMai>();
            List<SqlParameter> parama = new List<SqlParameter>();
            parama.Add(new SqlParameter("ID_KhuyenMai", ID_KhuyenMai));
            lst = db.Database.SqlQuery<DM_LichSuKhuyenMai>("exec getList_LichSuKhuyenMai @ID_KhuyenMai", parama.ToArray()).ToList();
            int RowsCount = lst.Count();
            List<DM_LichSuKhuyenMai> lstSL = lst.Skip((numberPage - 1) * PageSize).Take(PageSize).ToList();
            List<ListLHPages> lstPage = getAllPagenew<DM_LichSuKhuyenMai>(lst, PageSize);
            JsonResultExample<DM_LichSuKhuyenMai> json = new JsonResultExample<DM_LichSuKhuyenMai>
            {
                Rowcount = RowsCount,
                LstData = lstSL,
                LstPageNumber = lstPage
            };
            return Json(json);
        }
        public List<DM_ChiTietHangHoaKM> getListHangHoaKM(Guid ID_KhuyenMai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                List<DM_ChiTietHangHoaKM> lst = classKhuyenMai.getListHangHoaKM(ID_KhuyenMai);
                return lst;
            }
        }
        [HttpGet]
        public List<DM_KhuyenMai_ChiTiet> getKhuyenMaiChiTiet(Guid ID_KhuyenMai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                List<DM_KhuyenMai_ChiTiet> lst = classKhuyenMai.GetsCTKM(ct => ct.ID_KhuyenMai == ID_KhuyenMai).Select(x => new
                {
                    ID = x.ID,
                    ID_KhuyenMai = ID_KhuyenMai,
                    TongTienHang = x.TongTienHang,
                    GiamGia = x.GiamGia,
                    GiamGiaTheoPhanTram = x.GiamGiaTheoPhanTram,
                    ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                    ID_NhomHangHoa = x.ID_NhomHangHoa,
                    SoLuong = x.SoLuong,
                    ID_DonViQuiDoiMua = x.ID_DonViQuiDoiMua,
                    ID_NhomHangHoaMua = x.ID_NhomHangHoaMua,
                    SoLuongMua = x.SoLuongMua,
                    GiaKhuyenMai = x.GiaKhuyenMai

                }).AsEnumerable().Select(c => new DM_KhuyenMai_ChiTiet
                {
                    ID = c.ID,
                    ID_KhuyenMai = ID_KhuyenMai,
                    TongTienHang = c.TongTienHang,
                    GiamGia = c.GiamGia,
                    GiamGiaTheoPhanTram = c.GiamGiaTheoPhanTram,
                    ID_DonViQuiDoi = c.ID_DonViQuiDoi,
                    ID_NhomHangHoa = c.ID_NhomHangHoa,
                    SoLuong = c.SoLuong,
                    ID_DonViQuiDoiMua = c.ID_DonViQuiDoiMua,
                    ID_NhomHangHoaMua = c.ID_NhomHangHoaMua,
                    SoLuongMua = c.SoLuongMua,
                    GiaKhuyenMai = c.GiaKhuyenMai

                }).ToList();
                return lst;
            }
        }
        public List<DM_DonVi> getLisDonViKM(Guid ID_KhuyenMai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                List<DM_DonVi> lst = classKhuyenMai.getLisDonViKM(ID_KhuyenMai);
                return lst;
            }
        }
        public List<DM_NhanVienKM> getlistNhanViemKM(Guid ID_KhuyenMai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                List<DM_NhanVienKM> lst = classKhuyenMai.getlistNhanViemKM(ID_KhuyenMai);
                return lst;
            }
        }
        public List<DM_NhomKhachHangKM> getlistNhomHangKM(Guid ID_KhuyenMai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                List<DM_NhomKhachHangKM> lst = classKhuyenMai.getlistNhomHangKM(ID_KhuyenMai);
                return lst;
            }
        }

        public List<DM_KhuyenMai> GetKM_CTKhuyenMai(Guid idDonVi)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var dtNow = DateTime.Now;

            List<DM_KhuyenMai> lstReturn = new List<DM_KhuyenMai>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var data = (from ct in db.DM_KhuyenMai_ChiTiet
                            join km in db.DM_KhuyenMai on ct.ID_KhuyenMai equals km.ID
                            join ad in db.DM_KhuyenMai_ApDung on km.ID equals ad.ID_KhuyenMai into KM_AD
                            from km_ad in KM_AD.DefaultIfEmpty()
                            where km.TrangThai == true
                            && DateTime.Compare(dtNow, km.ThoiGianBatDau) >= 0 && DateTime.Compare(km.ThoiGianKetThuc, dtNow) >= 0
                            && (km.TatCaDonVi == true || km_ad.ID_DonVi == idDonVi)
                            group new { ct, km_ad } by new
                            {
                                ID_KhuyenMai = ct.ID_KhuyenMai,
                                TenKhuyenMai = km.TenKhuyenMai,
                                LoaiKhuyenMai = km.LoaiKhuyenMai,
                                NgayApDung = km.NgayApDung,
                                ThangApDung = km.ThangApDung,
                                ThuApDung = km.ThuApDung,
                                GioApDung = km.GioApDung,
                                ApDungNgaySinhNhat = km.ApDungNgaySinhNhat,
                                TatCaDoiTuong = km.TatCaDoiTuong,
                                TatCaNhanVien = km.TatCaNhanVien,
                                HinhThuc = km.HinhThuc,
                                ThoiGianKetThuc = km.ThoiGianKetThuc
                            }).ToList();

                foreach (var item in data)
                {
                    DM_KhuyenMai itemKM = new DM_KhuyenMai();
                    itemKM.ID = item.Key.ID_KhuyenMai;
                    itemKM.TenKhuyenMai = item.Key.TenKhuyenMai;
                    itemKM.LoaiKhuyenMai = item.Key.LoaiKhuyenMai;
                    itemKM.NgayApDung = item.Key.NgayApDung;
                    itemKM.ThangApDung = item.Key.ThangApDung;
                    itemKM.ThuApDung = item.Key.ThuApDung;
                    itemKM.GioApDung = item.Key.GioApDung;
                    itemKM.ApDungNgaySinhNhat = item.Key.ApDungNgaySinhNhat;
                    itemKM.TatCaDoiTuong = item.Key.TatCaDoiTuong;
                    itemKM.TatCaNhanVien = item.Key.TatCaNhanVien;
                    itemKM.HinhThuc = item.Key.HinhThuc;
                    itemKM.ThoiGianKetThuc = item.Key.ThoiGianKetThuc;

                    switch (itemKM.HinhThuc)
                    {
                        case 11:
                            itemKM.TenHinhThucKM = "Giảm giá hóa đơn";
                            break;
                        case 12:
                            itemKM.TenHinhThucKM = "Hóa đơn - Tặng hàng";
                            break;
                        case 13:
                            itemKM.TenHinhThucKM = "Hóa đơn - Giảm giá hàng";
                            break;
                        case 14:
                            itemKM.TenHinhThucKM = "Hóa đơn - Tặng điểm";
                            break;
                        case 21:
                            itemKM.TenHinhThucKM = "Mua hàng giảm giá hàng";
                            break;
                        case 22:
                            itemKM.TenHinhThucKM = "Mua hàng tặng hàng";
                            break;
                        case 23:
                            itemKM.TenHinhThucKM = "Mua hàng tặng điểm";
                            break;
                        case 24:
                            itemKM.TenHinhThucKM = "Giá bán theo số lượng mua";
                            break;
                    }

                    foreach (var itemGr in item)
                    {
                        DM_KhuyenMai_ApDung itemAD = new DM_KhuyenMai_ApDung();
                        if (itemGr.km_ad != null)
                        {
                            itemAD.ID_KhuyenMai = itemKM.ID;
                            itemAD.ID_NhomKhachHang = itemGr.km_ad.ID_NhomKhachHang;
                            itemAD.ID_NhanVien = itemGr.km_ad.ID_NhanVien;
                            itemAD.ID_DonVi = itemGr.km_ad.ID_DonVi;

                            itemKM.DM_KhuyenMai_ApDung.Add(itemAD);
                        }

                        DM_KhuyenMai_ChiTiet itemCT = new DM_KhuyenMai_ChiTiet();
                        itemCT.ID_KhuyenMai = itemKM.ID;
                        itemCT.ID = itemGr.ct.ID;
                        itemCT.ID_NhomHangHoaMua = itemGr.ct.ID_NhomHangHoaMua;
                        itemCT.ID_DonViQuiDoi = itemGr.ct.ID_DonViQuiDoi;
                        itemCT.ID_NhomHangHoa = itemGr.ct.ID_NhomHangHoa;
                        itemCT.TongTienHang = itemGr.ct.TongTienHang;
                        itemCT.GiamGia = itemGr.ct.GiamGia;
                        itemCT.GiamGiaTheoPhanTram = itemGr.ct.GiamGiaTheoPhanTram;
                        itemCT.SoLuong = itemGr.ct.SoLuong;
                        itemCT.ID_DonViQuiDoiMua = itemGr.ct.ID_DonViQuiDoiMua;
                        itemCT.SoLuongMua = itemGr.ct.SoLuongMua;
                        itemCT.GiaKhuyenMai = itemGr.ct.GiaKhuyenMai;

                        itemKM.DM_KhuyenMai_ChiTiet.Add(itemCT);
                    }
                    lstReturn.Add(itemKM);
                }
                return lstReturn;
            }
        }

        public List<DM_KhuyenMai_ChiTiet> GetChiTietNhomHangHoa_KM(Guid? nhomHangHoaId, SsoftvnContext db)
        {
            var data = new List<DM_KhuyenMai_ChiTiet>();
            if (nhomHangHoaId != null)
            {
                var ListNhomHangHoa = db.DM_NhomHangHoa.Where(o => o.ID_Parent == nhomHangHoaId).AsQueryable();
                if (ListNhomHangHoa.Any())
                {
                    foreach (var item in ListNhomHangHoa)
                    {
                        data.Add(new DM_KhuyenMai_ChiTiet() { ID_NhomHangHoa = item.ID, ID_NhomHangHoaMua = item.ID });
                        data.AddRange(GetChiTietNhomHangHoa_KM(item.ID, db));
                    }
                }
            }
            return data;
        }
        #endregion
        #region Delete
        [HttpGet]
        public string deleteKhuyenMai(Guid ID_KhuyenMai, Guid ID_DonVi, Guid ID_NhanVien)
        {
            string strErr = string.Empty;
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                List<DM_LichSuKhuyenMai> lst = new List<DM_LichSuKhuyenMai>();
                List<SqlParameter> parama = new List<SqlParameter>();
                parama.Add(new SqlParameter("ID_KhuyenMai", ID_KhuyenMai));
                lst = db.Database.SqlQuery<DM_LichSuKhuyenMai>("exec getList_LichSuKhuyenMai @ID_KhuyenMai", parama.ToArray()).ToList();
                int RowsCount = lst.Count();
                if (RowsCount > 0)
                {
                    strErr = "HD";
                }
                else
                {
                    strErr = classKhuyenMai.Delete_KhuyenMaiApDung(ID_KhuyenMai);
                    strErr += classKhuyenMai.Delete_KhuyenMaiChiTiet(ID_KhuyenMai);
                    strErr += classKhuyenMai.Delete_KhuyenMai(ID_KhuyenMai);
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                    {
                        ID = Guid.NewGuid(),
                        ID_NhanVien = ID_NhanVien,
                        ID_DonVi = ID_DonVi,
                        ChucNang = "Khuyến mại",
                        ThoiGian = DateTime.Now,
                        NoiDung = "Xóa chương trình khuyến mại " + strErr,
                        NoiDungChiTiet = "Xóa chương trình khuyến mại " + strErr,
                        LoaiNhatKy = 3
                    };
                    string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                    strErr = strIns12 + strErr;
                }
            }
            return strErr;
        }

        public DM_HangHoaDTO GetHangHoa_ByIDQuyDoi(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                return classKhuyenMai.GetHangHoa_QuyDoi(id).FirstOrDefault();
            }
        }
        public DM_HangHoaDTO getHangHoaBy_MaHangHoa(string maHH)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassKhuyenMai classKhuyenMai = new ClassKhuyenMai(db);
                return classKhuyenMai.getHangHoaBy_MaHangHoa(maHH).FirstOrDefault();
            }
        }
        public List<List_HangHoaKhuyenMai> getHangHoaKMBy_MaHangHoa(string maHH)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<List_HangHoaKhuyenMai> lst = new List<List_HangHoaKhuyenMai>();
            List<SqlParameter> sqlprm = new List<SqlParameter>();
            sqlprm.Add(new SqlParameter("MaHangHoa", maHH));
            lst = db.Database.SqlQuery<List_HangHoaKhuyenMai>("exec getList_HangHoabyMaHH @MaHangHoa", sqlprm.ToArray()).ToList();
            return lst;
        }
        #endregion
    }
}