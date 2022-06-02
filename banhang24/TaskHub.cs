using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using Model;
using libDM_DoiTuong;

namespace banhang24
{
    public class TaskHub : Hub
    {
        public BH_HoaDon ThongBaoNhaBep([FromBody]JObject data)
        {
            BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
            List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();

            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classHoaDonChiTiet = new ClassBH_HoaDon_ChiTiet(db);

                if (objHoaDon.ID == Guid.Empty)
                {
                    // insert
                    #region BH_HoaDon
                    BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                    itemBH_HoaDon.ID = Guid.NewGuid();

                    itemBH_HoaDon.MaHoaDon = classhoadon.SP_GetAutoCode_HDDatHang();
                    itemBH_HoaDon.ID_NhanVien = objHoaDon.ID_NhanVien;
                    itemBH_HoaDon.ID_ViTri = objHoaDon.ID_ViTri;
                    itemBH_HoaDon.NguoiTao = objHoaDon.NguoiTao;
                    itemBH_HoaDon.DienGiai = objHoaDon.DienGiai;
                    itemBH_HoaDon.ID_DoiTuong = objHoaDon.ID_DoiTuong;
                    itemBH_HoaDon.NgayLapHoaDon = DateTime.Now;
                    itemBH_HoaDon.PhaiThanhToan = objHoaDon.PhaiThanhToan;
                    itemBH_HoaDon.TongGiamGia = objHoaDon.TongGiamGia;
                    itemBH_HoaDon.TongChiPhi = objHoaDon.TongChiPhi;
                    itemBH_HoaDon.TongTienHang = objHoaDon.TongTienHang;
                    itemBH_HoaDon.NgayTao = DateTime.Now;
                    itemBH_HoaDon.ID_DonVi = new Guid("d93b17ea-89b9-4ecf-b242-d03b8cde71de");
                    itemBH_HoaDon.TyGia = 1;
                    itemBH_HoaDon.LoaiHoaDon = objHoaDon.LoaiHoaDon; // Hoa don nhap hang
                                                                     // neu luu tam => cho thanh toan == false
                    itemBH_HoaDon.ChoThanhToan = objHoaDon.ChoThanhToan;
                    itemBH_HoaDon.TongChietKhau = 0;
                    itemBH_HoaDon.TongTienThue = 0;
                    #endregion

                    string strIns = classhoadon.Add_HoaDon(itemBH_HoaDon);
                    #region BH_ChiTietHoaDon
                    foreach (var item in objCTHoaDon)
                    {
                        BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                        {
                            ID = Guid.NewGuid(),
                            ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                            DonGia = item.DonGia,
                            ID_HoaDon = itemBH_HoaDon.ID,
                            SoLuong = item.SoLuong,
                            ThanhTien = item.ThanhTien,
                            TienChietKhau = item.TienChietKhau,
                            GiaVon = item.GiaVon,
                            Bep_SoLuongHoanThanh = item.Bep_SoLuongHoanThanh,
                            Bep_SoLuongYeuCau = item.Bep_SoLuongYeuCau,
                            Bep_SoLuongChoCungUng = item.Bep_SoLuongChoCungUng,
                            ThoiGian = item.ThoiGian
                            // tien giam 
                        };

                        strIns = classHoaDonChiTiet.Add_ChiTietHoaDon(ctHoaDon);
                        itemBH_HoaDon.BH_HoaDon_ChiTiet.Add(ctHoaDon);
                    }
                    #endregion

                    return itemBH_HoaDon;
                }
                else
                {
                    #region "Update Hoa Don"
                    BH_HoaDon objUpHD = db.BH_HoaDon.Find(objHoaDon.ID);
                    if (objUpHD != null)
                    {
                        objUpHD.ID_NhanVien = objHoaDon.ID_NhanVien == Guid.Empty ? null : objHoaDon.ID_NhanVien;
                        objUpHD.ID_ViTri = objHoaDon.ID_ViTri;
                        objUpHD.DienGiai = objHoaDon.DienGiai;
                        objUpHD.ID_DoiTuong = objHoaDon.ID_DoiTuong == Guid.Empty ? Guid.Empty : objHoaDon.ID_DoiTuong;
                        // objUpHD.NgayLapHoaDon = objHoaDon.NgayLapHoaDon; // lau NgayLapHD la ngay cu truoc do
                        objUpHD.PhaiThanhToan = objHoaDon.PhaiThanhToan;
                        objUpHD.TongGiamGia = objHoaDon.TongGiamGia;
                        objUpHD.TongChiPhi = objHoaDon.TongChiPhi;
                        objUpHD.TongTienHang = objHoaDon.TongTienHang;
                        objUpHD.ChoThanhToan = objHoaDon.ChoThanhToan;

                        db.Entry(objUpHD).State = EntityState.Modified;
                        db.SaveChanges();
                        #endregion
                        #region " BH_HoaDon_ChiTiet "
                        // delete all CTHD with ID_HoaDon
                        string sErr = classHoaDonChiTiet.Delete_HoaDon_ChiTiet_ByIDHoaDon(objHoaDon.ID);
                        if (sErr == string.Empty)
                        {
                            // insert again
                            double? _giaVon = 0;

                            foreach (var item in objCTHoaDon)
                            {
                                // neu nhap giam gia => don gia = gia von
                                if (item.TienChietKhau > 0)
                                {
                                    _giaVon = item.GiaVon;
                                }
                                // else: lay gia moi
                                else
                                {
                                    _giaVon = item.DonGia;
                                }

                                BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                    DonGia = item.DonGia,
                                    GiaVon = _giaVon,
                                    ID_HoaDon = objUpHD.ID,
                                    SoLuong = item.SoLuong,
                                    ThanhTien = item.ThanhTien,
                                    TienChietKhau = item.TienChietKhau, // tien giam 
                                    ThoiGian = item.ThoiGian,
                                    Bep_SoLuongHoanThanh = item.Bep_SoLuongHoanThanh,
                                    Bep_SoLuongYeuCau = item.Bep_SoLuongYeuCau,
                                    Bep_SoLuongChoCungUng = item.Bep_SoLuongChoCungUng,
                                };
                                classHoaDonChiTiet.Add_ChiTietHoaDon(ctHoaDon);
                                objUpHD.BH_HoaDon_ChiTiet.Add(ctHoaDon); // add lstCTHD into HoaDon
                            }
                        }
                        #endregion
                    }
                    else
                    {
                    }
                }

                return objHoaDon;
            }
            //var lstItem = (from p in db.BH_HoaDon select p).ToList();
            //string lstId = JsonConvert.SerializeObject(lstItem);

            //Clients.All.addNewOrder(lstId);
        }
    }
}