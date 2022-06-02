using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq.Expressions;

namespace libHT
{
    public class classHT_CauHinhPhanMem
    {
        private SsoftvnContext db;

        public classHT_CauHinhPhanMem(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public HT_CauHinhPhanMem SelectByIDDonVi(Guid iddonvi)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                HT_CauHinhPhanMem param = db.HT_CauHinhPhanMem.Where(p => p.ID_DonVi == iddonvi).FirstOrDefault();
                return param;
            }
        }

        public List<HT_CauHinh_TichDiemApDung> GetsTichDiemAD(Expression<Func<HT_CauHinh_TichDiemApDung, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.HT_CauHinh_TichDiemApDung.ToList();
                else
                    return db.HT_CauHinh_TichDiemApDung.Where(query).ToList();
            }
        }

        public HT_CauHinh_TichDiemChiTiet selectbyID_CauHinh(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                HT_CauHinh_TichDiemChiTiet param = db.HT_CauHinh_TichDiemChiTiet.Where(p => p.ID_CauHinh == id).FirstOrDefault();
                return param;
            }
        }

        public HT_CauHinh_GioiHanTraHang selectGioiHanTHbyID_CauHinh(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                HT_CauHinh_GioiHanTraHang param = db.HT_CauHinh_GioiHanTraHang.Where(p => p.ID_CauHinh == id).FirstOrDefault();
                return param;
            }
        }
        #endregion
        public string add_ThietLap(HT_CauHinhPhanMem objThietLap)
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
                    db.HT_CauHinhPhanMem.Add(objThietLap);
                    db.SaveChanges();
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

        public string add_TichDiem(HT_CauHinh_TichDiemChiTiet objTichDiem)
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
                    db.HT_CauHinh_TichDiemChiTiet.Add(objTichDiem);
                    db.SaveChanges();
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

        public string add_GioiHanTH(HT_CauHinh_GioiHanTraHang objTH)
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
                    db.HT_CauHinh_GioiHanTraHang.Add(objTH);
                    db.SaveChanges();
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

        public string add_TichDiem_AD(HT_CauHinh_TichDiemApDung objTichDiemAD)
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
                    db.HT_CauHinh_TichDiemApDung.Add(objTichDiemAD);
                    db.SaveChanges();
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

        public string Insert_HTMaChungTu(HT_MaChungTu obj)
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
                    db.HT_MaChungTu.Add(obj);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    strErr = "Insert_TLMaChungTu " + e.InnerException + e.Message;
                    CookieStore.WriteLog(strErr);
                }
            }
            return strErr;
        }

        public string Update_TichDiem(HT_CauHinh_TichDiemChiTiet obj)
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
                    HT_CauHinh_TichDiemChiTiet objUpd = db.HT_CauHinh_TichDiemChiTiet.Find(obj.ID);
                    objUpd.TyLeDoiDiem = obj.TyLeDoiDiem;
                    objUpd.ThanhToanBangDiem = obj.ThanhToanBangDiem;
                    objUpd.DiemThanhToan = obj.DiemThanhToan;
                    objUpd.TienThanhToan = obj.TienThanhToan;
                    objUpd.TichDiemGiamGia = obj.TichDiemGiamGia;
                    objUpd.TichDiemHoaDonDiemThuong = obj.TichDiemHoaDonDiemThuong;
                    objUpd.ToanBoKhachHang = obj.ToanBoKhachHang;
                    objUpd.SoLanMua = obj.SoLanMua;
                    objUpd.TichDiemHoaDonGiamGia = obj.TichDiemHoaDonGiamGia;
                    objUpd.KhoiTaoTichDiem = obj.KhoiTaoTichDiem;
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

        public string Update_GioiHanTH(HT_CauHinh_GioiHanTraHang obj)
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
                    HT_CauHinh_GioiHanTraHang objUpd = db.HT_CauHinh_GioiHanTraHang.Find(obj.ID);
                    objUpd.SoNgayGioiHan = obj.SoNgayGioiHan;
                    objUpd.ChoPhepTraHang = obj.ChoPhepTraHang;
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

        public string Update_TichDiemAD(HT_CauHinh_TichDiemApDung obj)
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
                    HT_CauHinh_TichDiemApDung objUpd = db.HT_CauHinh_TichDiemApDung.Find(obj.ID);
                    objUpd.ID_NhomDoiTuong = obj.ID_NhomDoiTuong;
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

        public bool CheckSetupMaChungTu(int? loaichungtu)
        {
            bool isSetup = false;
            if (db == null)
            {
                isSetup = false;
            }
            else
            {
                try
                {
                    var ht = from tl in db.HT_CauHinhPhanMem
                             where tl.SuDungMaChungTu == 1
                             select tl.SuDungMaChungTu;
                    if (ht != null && ht.Count() > 0)
                    {
                        var tlMa = from tlma in db.HT_MaChungTu
                                   where tlma.ID_LoaiChungTu == loaichungtu
                                   select tlma.ID;
                        if (tlMa != null && tlMa.Count() > 0)
                        {
                            isSetup = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("CheckSetupMaChungTu " + ex.Message + ex.InnerException);
                }
            }
            return isSetup;
        }

        public string Update_ThietLap(HT_CauHinhPhanMem objThietLap)
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
                    #region HT_CauHinhPhanMem
                    HT_CauHinhPhanMem hT_CauHinhPhanMem = db.HT_CauHinhPhanMem.Find(objThietLap.ID);
                    hT_CauHinhPhanMem.GiaVonTrungBinh = objThietLap.GiaVonTrungBinh;
                    hT_CauHinhPhanMem.CoDonViTinh = objThietLap.CoDonViTinh;
                    hT_CauHinhPhanMem.DatHang = objThietLap.DatHang;
                    hT_CauHinhPhanMem.XuatAm = objThietLap.XuatAm;
                    hT_CauHinhPhanMem.DatHangXuatAm = objThietLap.DatHangXuatAm;
                    hT_CauHinhPhanMem.ThayDoiThoiGianBanHang = objThietLap.ThayDoiThoiGianBanHang;
                    hT_CauHinhPhanMem.TinhNangTichDiem = objThietLap.TinhNangTichDiem;
                    hT_CauHinhPhanMem.GioiHanThoiGianTraHang = objThietLap.GioiHanThoiGianTraHang;
                    hT_CauHinhPhanMem.SanPhamCoThuocTinh = objThietLap.SanPhamCoThuocTinh;
                    hT_CauHinhPhanMem.BanVaChuyenKhiHangDaDat = objThietLap.BanVaChuyenKhiHangDaDat;
                    hT_CauHinhPhanMem.TinhNangSanXuatHangHoa = objThietLap.TinhNangSanXuatHangHoa;
                    hT_CauHinhPhanMem.SuDungCanDienTu = objThietLap.SuDungCanDienTu;
                    hT_CauHinhPhanMem.KhoaSo = objThietLap.KhoaSo;
                    hT_CauHinhPhanMem.InBaoGiaKhiBanHang = objThietLap.InBaoGiaKhiBanHang;
                    hT_CauHinhPhanMem.QuanLyKhachHangTheoDonVi = objThietLap.QuanLyKhachHangTheoDonVi;
                    hT_CauHinhPhanMem.SoLuongTrenChungTu = objThietLap.SoLuongTrenChungTu;
                    hT_CauHinhPhanMem.KhuyenMai = objThietLap.KhuyenMai;
                    hT_CauHinhPhanMem.LoHang = objThietLap.LoHang;
                    hT_CauHinhPhanMem.SuDungMauInMacDinh = objThietLap.SuDungMauInMacDinh;
                    hT_CauHinhPhanMem.ApDungGopKhuyenMai = objThietLap.ApDungGopKhuyenMai;
                    hT_CauHinhPhanMem.ThongTinChiTietNhanVien = objThietLap.ThongTinChiTietNhanVien;
                    hT_CauHinhPhanMem.BanHangOffline = objThietLap.BanHangOffline;
                    hT_CauHinhPhanMem.ThoiGianNhacHanSuDungLo = objThietLap.ThoiGianNhacHanSuDungLo;
                    hT_CauHinhPhanMem.SuDungMaChungTu = objThietLap.SuDungMaChungTu;
                    #endregion
                    db.Entry(hT_CauHinhPhanMem).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("ClassHT_CauHinhPhanMem - Update_ThietLap: " + ex.Message + ex.InnerException);
                }
            }
            return strErr;
        }

        public bool DeleteData(/*bool hanghoa, bool khachhang*/)
        {
            if (db != null)
            {
                db.Database.ExecuteSqlCommand("exec XoaDuLieuHeThong");
            }
            return true;
        }
    }
}
