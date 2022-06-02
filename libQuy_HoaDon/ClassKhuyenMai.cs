using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

using System.IO;
using System.Web;

namespace libQuy_HoaDon
{
    public class ClassKhuyenMai
    {
        private SsoftvnContext db;
        public ClassKhuyenMai(SsoftvnContext _db)
        {
            db = _db;
        }
        //Kiểm tra mã khuyến mại
        public bool Check_MaKhuyenMai(string maKhuyenMai)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                if (maKhuyenMai != null & maKhuyenMai != "null" & maKhuyenMai != "")
                    return db.DM_KhuyenMai.Count(e => e.MaKhuyenMai == maKhuyenMai) > 0;
                else
                    return false;
            }
        }
        public string Check_MaChuongTrinhKhuyenMai(string maKhuyenMai)
        {

            string tb = null;
            var lst = db.DM_KhuyenMai.Count(e => e.MaKhuyenMai == maKhuyenMai);
            if (lst > 0)
            {
                tb = "Mã chương trình khuyến mại đã tồn tại trong cơ sở dữ liệu";
            }
            return tb;
        }
        //Tự động sinh mã
        public string GetAutoCode()
        {
            string format = "{0:0000}";

            String maKM = string.Empty;
            maKM = "KM";
            // find in BH_HoaDon
            string madv = db.DM_KhuyenMai.Where(p => p.MaKhuyenMai.IndexOf(maKM) > -1).
                Where(p => p.MaKhuyenMai.Length == 6 || p.MaKhuyenMai.Length == 7 || p.MaKhuyenMai.Length == 8 || p.MaKhuyenMai.Length == 9).OrderByDescending(p => p.MaKhuyenMai).
                Select(p => p.MaKhuyenMai).FirstOrDefault();
            if (madv == null)
            {
                maKM = maKM + string.Format(format, 1);
            }
            else
            {
                int tempstt = int.Parse(madv.Substring(maKM.Length, 4)) + 1;
                maKM = maKM + string.Format(format, tempstt);
            }
            return maKM;
        }
        // add chương trình khuyến mại
        public string Add_KhuyenMai(DM_KhuyenMai objAdd)
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
                    db.DM_KhuyenMai.Add(objAdd);
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
        // add khuyến mại áp dụng
        public string Add_KhuyenMaiApDung(DM_KhuyenMai_ApDung objAdd)
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
                    db.DM_KhuyenMai_ApDung.Add(objAdd);
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
        //add Khuyến mại chi tiết
        public string Add_KhuyenMaiChiTiet(DM_KhuyenMai_ChiTiet objAdd)
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
                    db.DM_KhuyenMai_ChiTiet.Add(objAdd);
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

        // add chương trình khuyến mại
        public string Add_ChotSo(ChotSo objAdd)
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
                    db.ChotSo.Add(objAdd);
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
        // update Khuyến mại
        public string Update_DMKhuyenMai(DM_KhuyenMai obj)
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
                    #region DM_KhuyenMai
                    DM_KhuyenMai objUpd = db.DM_KhuyenMai.Find(obj.ID);
                    objUpd.MaKhuyenMai = obj.MaKhuyenMai;
                    objUpd.TenKhuyenMai = obj.TenKhuyenMai;
                    objUpd.GhiChu = obj.GhiChu;
                    objUpd.TrangThai = obj.TrangThai;
                    objUpd.HinhThuc = obj.HinhThuc;
                    objUpd.LoaiKhuyenMai = obj.LoaiKhuyenMai;
                    objUpd.ThoiGianBatDau = obj.ThoiGianBatDau;
                    objUpd.ThoiGianKetThuc = obj.ThoiGianKetThuc;
                    objUpd.NgayApDung = obj.NgayApDung; //Sang tuần làm
                    objUpd.ThangApDung = obj.ThangApDung;
                    objUpd.ThuApDung = obj.ThuApDung;
                    objUpd.GioApDung = obj.GioApDung;
                    objUpd.ApDungNgaySinhNhat = obj.ApDungNgaySinhNhat;
                    objUpd.TatCaDonVi = obj.TatCaDonVi;
                    objUpd.TatCaDoiTuong = obj.TatCaDoiTuong;
                    objUpd.TatCaNhanVien = obj.TatCaNhanVien;
                    objUpd.NguoiSua = obj.NguoiSua;
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
        // update khuyến mại chi tiết
        public string Update_DMKhuyenMaiChiTiet(DM_KhuyenMai_ChiTiet obj)
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
                    #region DM_KhuyenMai
                    DM_KhuyenMai_ChiTiet objUpd = db.DM_KhuyenMai_ChiTiet.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TongTienHang = obj.TongTienHang;
                    objUpd.GiamGia = obj.GiamGia;
                    objUpd.GiamGiaTheoPhanTram = obj.GiamGiaTheoPhanTram;
                    objUpd.ID_DonViQuiDoi = obj.ID_DonViQuiDoi;
                    objUpd.ID_NhomHangHoa = obj.ID_NhomHangHoa;
                    objUpd.SoLuong = obj.SoLuong;
                    objUpd.ID_DonViQuiDoiMua = obj.ID_DonViQuiDoiMua;
                    objUpd.ID_NhomHangHoaMua = obj.ID_NhomHangHoaMua;
                    objUpd.SoLuongMua = obj.SoLuongMua;
                    objUpd.GiaKhuyenMai = obj.GiaKhuyenMai;
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
        // update khuyến mại chi tiết
        public string Update_DMKhuyenMaiApDung(DM_KhuyenMai_ApDung obj)
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
                    #region DM_KhuyenMai
                    DM_KhuyenMai_ApDung objUpd = db.DM_KhuyenMai_ApDung.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.ID_DonVi = obj.ID_DonVi;
                    objUpd.ID_NhanVien = obj.ID_NhanVien;
                    objUpd.ID_NhomKhachHang = obj.ID_NhomKhachHang;
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
        // get nhóm khách hàng
        public List<DM_NhomDoiTuong> GetNhomDoiTuong(int LoaiDoiTuong)
        {

            var tb = from kh in db.DM_NhomDoiTuong
                     where kh.LoaiDoiTuong == LoaiDoiTuong
                     orderby kh.NgayTao
                     select new
                     {
                         kh.ID,
                         kh.MaNhomDoiTuong,
                         kh.TenNhomDoiTuong
                     };
            List<DM_NhomDoiTuong> lst = new List<DM_NhomDoiTuong>();
            foreach (var item in tb)
            {
                DM_NhomDoiTuong DM = new DM_NhomDoiTuong();
                DM.ID = item.ID;
                DM.MaNhomDoiTuong = item.MaNhomDoiTuong;
                DM.TenNhomDoiTuong = item.TenNhomDoiTuong;
                lst.Add(DM);
            }
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }
        //get nhân viên
        public List<NS_NhanVien> GetNhanVien(string lstChiNhanh)
        {

            string[] mang = lstChiNhanh.Split(',');
            List<string> LstIS = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                LstIS.Add(mang[i]);
            }
            var tb = from nv in db.NS_NhanVien
                     join ht in db.NS_QuaTrinhCongTac.Where(x => LstIS.Contains(x.ID_DonVi.ToString())) on nv.ID equals ht.ID_NhanVien
                     // join dv in db.DM_DonVi.Where(x => LstIS.Contains(x.ID.ToString())) on ht.ID_DonVi equals dv.ID
                     orderby ht.NgayLap
                     group nv by new
                     {
                         ID = nv.ID,
                         TenNhanVien = nv.TenNhanVien
                     } into g
                     select new
                     {
                         ID = g.Key.ID,
                         TenNhanVien = g.Key.TenNhanVien
                     };
            List<NS_NhanVien> lst = new List<NS_NhanVien>();
            foreach (var item in tb)
            {
                NS_NhanVien DM = new NS_NhanVien();
                DM.ID = item.ID;
                DM.TenNhanVien = item.TenNhanVien;
                lst.Add(DM);
            }
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }

        }

        public List<DM_ChuongTrinhKhuyenMai> GetListPromotion(ParamSearchPromotion lstParam)
        {
            try
            {
                var idChiNhanhs = string.Join(",", lstParam.IDChiNhanhs);
                var txt = lstParam.TextSearch;
                if (txt == null || txt == string.Empty)
                {
                    txt = "%%";
                }
                else
                {
                    txt = "%" + txt + "%";
                }
                var type = "%%";
                if (lstParam.TypePromotion != 0)
                {
                    type = lstParam.TypePromotion.ToString();
                }
                var expired = "%%";
                if (lstParam.Expired != 0)
                {
                    expired = lstParam.Expired.ToString();
                }
                var active = "%%";
                if (lstParam.StatusActive != 2)
                {
                    active = lstParam.StatusActive.ToString();
                }
                List<SqlParameter> paramSql = new List<SqlParameter>();
                paramSql.Add(new SqlParameter("IDChiNhanhs", idChiNhanhs));
                paramSql.Add(new SqlParameter("TextSearch", txt));
                paramSql.Add(new SqlParameter("TypePromotion", type));
                paramSql.Add(new SqlParameter("StatusActive", active));
                paramSql.Add(new SqlParameter("Expired", expired));
                paramSql.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
                paramSql.Add(new SqlParameter("PageSize", lstParam.PageSize));
                var data = db.Database.SqlQuery<DM_ChuongTrinhKhuyenMai>("EXEC GetListPromotion @IDChiNhanhs, @TextSearch, @TypePromotion," +
                    "@StatusActive, @Expired, @CurrentPage, @PageSize", paramSql.ToArray()).ToList();
                return data;
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("GetListPromotion " + e.InnerException + e.Message);
                return new List<DM_ChuongTrinhKhuyenMai>();
            }
        }

        //get Chương trình khuyến mai
        public List<DM_ChuongTrinhKhuyenMai> getCTKhuyenMai(string maKM, string Chinhanh, int trangthai)
        {
            string[] mang = Chinhanh.Split(',');
            List<string> LstIS = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                LstIS.Add(mang[i].ToString());
            }

            List<DM_ChuongTrinhKhuyenMai> lst = new List<DM_ChuongTrinhKhuyenMai>();
            var tb = from km in db.DM_KhuyenMai
                     join ad in db.DM_KhuyenMai_ApDung on km.ID equals ad.ID_KhuyenMai into g
                     from AK in g.DefaultIfEmpty()
                     where km.TatCaDonVi == true || LstIS.Contains(AK.ID_DonVi.ToString())
                     //orderby km.NgayTao descending
                     group km by new
                     {
                         ID = km.ID,
                         MaKhuyenMai = km.MaKhuyenMai,
                         TenKhuyenMai = km.TenKhuyenMai,
                         GhiChu = km.GhiChu,
                         TrangThai = km.TrangThai,
                         HinhThuc = km.HinhThuc,
                         LoaiKhuyenMai = km.LoaiKhuyenMai,
                         ThoiGianBatDau = km.ThoiGianBatDau,
                         ThoiGianKetThuc = km.ThoiGianKetThuc,
                         NgayApDung = km.NgayApDung,
                         ThangApDung = km.ThangApDung,
                         ThuApDung = km.ThuApDung,
                         GioApDung = km.GioApDung,
                         ApDungNgaySinhNhat = km.ApDungNgaySinhNhat,
                         TatCaDoiTuong = km.TatCaDoiTuong,
                         TatCaDonVi = km.TatCaDonVi,
                         TatCaNhanVien = km.TatCaNhanVien,
                         NguoiTao = km.NguoiTao,
                         NgayTao = km.NgayTao
                     } into k
                     select new
                     {
                         k.Key.ID,
                         k.Key.MaKhuyenMai,
                         k.Key.TenKhuyenMai,
                         k.Key.GhiChu,
                         k.Key.TrangThai,
                         k.Key.HinhThuc,
                         k.Key.LoaiKhuyenMai,
                         k.Key.ThoiGianBatDau,
                         k.Key.ThoiGianKetThuc,
                         k.Key.NgayApDung,
                         k.Key.ThangApDung,
                         k.Key.ThuApDung,
                         k.Key.GioApDung,
                         k.Key.ApDungNgaySinhNhat,
                         k.Key.TatCaDoiTuong,
                         k.Key.TatCaDonVi,
                         k.Key.TatCaNhanVien,
                         k.Key.NguoiTao,
                         k.Key.NgayTao
                     };
            if (maKM != "null" & maKM != "" & maKM != null)
            {
                tb = tb.Where(p => p.MaKhuyenMai.Contains(@maKM) || p.TenKhuyenMai.Contains(@maKM));
            }
            if (trangthai == 1)
            {
                tb = tb.Where(p => p.TrangThai == true);
            }
            if (trangthai == 2)
            {
                tb = tb.Where(p => p.TrangThai == false);
            }
            tb = tb.OrderByDescending(p => p.NgayTao);
            foreach (var item in tb)
            {
                DM_ChuongTrinhKhuyenMai DM = new DM_ChuongTrinhKhuyenMai();
                DM.ID = item.ID;
                DM.MaKhuyenMai = item.MaKhuyenMai;
                DM.TenKhuyenMai = item.TenKhuyenMai;
                DM.GhiChu = item.GhiChu;
                DM.TrangThai = item.TrangThai == true ? "Kích hoạt" : (item.TrangThai == false ? "Chưa áp dụng" : "");
                DM.HinhThuc = item.HinhThuc == 0 ? "" : (item.HinhThuc == 11 ? "Hóa đơn - Giảm giá hóa đơn" : (item.HinhThuc == 12 ? "Hóa đơn - Tặng hàng" : (item.HinhThuc == 13 ? "Hóa đơn - Giảm giá hàng" : (item.HinhThuc == 14 ? "Hóa đơn - Tặng Điểm" :
                         (item.HinhThuc == 21 ? "Hàng hóa - Mua hàng giảm giá hàng" : (item.HinhThuc == 22 ? "Hàng hóa - Mua hàng tặng hàng" : (item.HinhThuc == 23 ? "Hàng hóa - Mua hàng tặng điểm" : (item.HinhThuc == 24 ? "Hàng hóa - Mua hàng giảm giá theo số lượng mua" : ""))))))));
                DM.LoaiKhuyenMai = item.LoaiKhuyenMai;
                DM.KieuHinhThuc = item.HinhThuc;
                DM.ThoiGianBatDau = item.ThoiGianBatDau;
                DM.ThoiGianKetThuc = item.ThoiGianKetThuc;
                DM.NgayApDung = item.NgayApDung == "" ? "" : ("Ngày " + item.NgayApDung.Replace("_", ", Ngày "));
                DM.ThangApDung = item.ThangApDung == "" ? "" : ("Tháng " + item.ThangApDung.Replace("_", ", Tháng "));
                DM.ThuApDung = item.ThuApDung == "" ? "" : ("Thứ " + item.ThuApDung.Replace("_", ", Thứ "));
                DM.ThuApDung = DM.ThuApDung.Replace("Thứ 8", "Chủ nhật");
                DM.GioApDung = item.GioApDung == "" ? "" : ("Giờ " + item.GioApDung.Replace("_", ", Giờ "));
                DM.ApDungNgaySinhNhat = item.ApDungNgaySinhNhat == 0 ? "" : (item.ApDungNgaySinhNhat == 1 ? "Áp dụng vào ngày sinh nhật khách hàng" : (item.ApDungNgaySinhNhat == 2 ? "Áp dụng vào tuần sinh nhật khách hàng" : (item.ApDungNgaySinhNhat == 3 ? "Áp dụng vào tháng sinh nhật khách hàng" : "")));
                DM.ValueApDungSN = item.ApDungNgaySinhNhat;
                DM.TatCaDoiTuong = item.TatCaDoiTuong;
                DM.TatCaDonVi = item.TatCaDonVi;
                DM.TatCaNhanVien = item.TatCaNhanVien;
                DM.NguoiTao = item.NguoiTao;
                lst.Add(DM);
            }

            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }
        // get List chi tiết khuyến mại
        public List<Object> getChiTiet_KhuyenMai(Guid ID_KhuyenMai)
        {

            //List<DM_ChiTietKhuyenMai> lst = new List<DM_ChiTietKhuyenMai>();
            var tb = (from kmct in db.DM_KhuyenMai_ChiTiet
                      join km in db.DM_KhuyenMai on kmct.ID_KhuyenMai equals km.ID into KT
                      from KC in KT.DefaultIfEmpty()
                      join dvqd in db.DonViQuiDois on kmct.ID_DonViQuiDoi equals dvqd.ID into KD
                      from MD in KD.DefaultIfEmpty()
                      join hh in db.DM_HangHoa on MD.ID_HangHoa equals hh.ID into HM
                      from TH in HM.DefaultIfEmpty()
                      join nh in db.DM_NhomHangHoa on kmct.ID_NhomHangHoa equals nh.ID into NHH
                      from TNH in NHH.DefaultIfEmpty()
                      join dvqdm in db.DonViQuiDois on kmct.ID_DonViQuiDoiMua equals dvqdm.ID into TD
                      from TM in TD.DefaultIfEmpty()
                      join hhm in db.DM_HangHoa on TM.ID_HangHoa equals hhm.ID into TDM
                      from THM in TDM.DefaultIfEmpty()
                      join nhm in db.DM_NhomHangHoa on kmct.ID_NhomHangHoaMua equals nhm.ID into NHM
                      from TNHM in NHM.DefaultIfEmpty()
                      where kmct.ID_KhuyenMai == ID_KhuyenMai
                      select new
                      {
                          ID = kmct.ID,
                          ID_KhuyenMai = kmct.ID_KhuyenMai,
                          ID_DonViQuiDoi = kmct.ID_DonViQuiDoi,
                          MaHangHoaMua = TM == null ? "" : TM.MaHangHoa,
                          TenHangHoa = TH.TenHangHoa,
                          ID_DonViQuiDoiMua = kmct.ID_DonViQuiDoiMua,
                          TenHangHoaMua = THM.TenHangHoa,
                          ID_NhomHangHoa = kmct.ID_NhomHangHoa,
                          TenNhomHangHoa = TNH.TenNhomHangHoa,
                          ID_NhomHangHoaMua = kmct.ID_NhomHangHoaMua,
                          TenNhomHangHoaMua = TNHM.TenNhomHangHoa,
                          SoLuong = kmct.SoLuong,
                          SoLuongMua = kmct.SoLuongMua,
                          GiamGia = kmct.GiamGia,
                          DiemCong = kmct.GiamGia,
                          TongTienHang = kmct.TongTienHang,
                          GiaKhuyenMai = kmct.GiaKhuyenMai,
                          GiamGiaTheoPhanTram = kmct.GiamGiaTheoPhanTram,
                          STT = kmct.STT
                      });
            try
            {
                return tb.OrderBy(x => x.STT).ToList<Object>();
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("getChiTiet_KhuyenMai " + e.InnerException + e.Message);
                return null;
            }
        }
        // get list hanghoa khuyến mại
        public List<DM_ChiTietHangHoaKM> getListHangHoaKM(Guid ID_KhuyenMai)
        {

            var tb = from kmct in db.DM_KhuyenMai_ChiTiet
                     join dvqd in db.DonViQuiDois on kmct.ID_DonViQuiDoi equals dvqd.ID into KD
                     from MD in KD.DefaultIfEmpty()
                     join hh in db.DM_HangHoa on MD.ID_HangHoa equals hh.ID into HM
                     from TH in HM.DefaultIfEmpty()
                     join nh in db.DM_NhomHangHoa on kmct.ID_NhomHangHoa equals nh.ID into NHH
                     from TNH in NHH.DefaultIfEmpty()
                     join dvqdm in db.DonViQuiDois on kmct.ID_DonViQuiDoiMua equals dvqdm.ID into TD
                     from TM in TD.DefaultIfEmpty()
                     join hhm in db.DM_HangHoa on TM.ID_HangHoa equals hhm.ID into TDM
                     from THM in TDM.DefaultIfEmpty()
                     join nhm in db.DM_NhomHangHoa on kmct.ID_NhomHangHoaMua equals nhm.ID into NHM
                     from TNHM in NHM.DefaultIfEmpty()
                     where kmct.ID_KhuyenMai == ID_KhuyenMai
                     group kmct by new
                     {
                         ID_DonViQuiDoi = kmct.ID_DonViQuiDoi,
                         ID_DonViQuiDoiMua = kmct.ID_DonViQuiDoiMua,
                         ID_NhomHangHoa = kmct.ID_NhomHangHoa,
                         ID_NhomHangHoaMua = kmct.ID_NhomHangHoaMua,
                         TenHangHoa = TH.TenHangHoa,
                         TenHangHoaMua = THM.TenHangHoa,
                         TenNhomHangHoa = TNH.TenNhomHangHoa,
                         TenNhomHangHoaMua = TNHM.TenNhomHangHoa
                     } into G
                     select new
                     {
                         ID_DonViQuiDoi = G.Key.ID_DonViQuiDoi,
                         TenHangHoa = G.Key.TenHangHoa,
                         ID_DonViQuiDoiMua = G.Key.ID_DonViQuiDoiMua,
                         TenHangHoaMua = G.Key.TenHangHoaMua,
                         ID_NhomHangHoa = G.Key.ID_NhomHangHoa,
                         TenNhomHangHoa = G.Key.TenNhomHangHoa,
                         ID_NhomHangHoaMua = G.Key.ID_NhomHangHoaMua,
                         TenNhomHangHoaMua = G.Key.TenNhomHangHoaMua
                     };
            List<DM_ChiTietHangHoaKM> lst = new List<DM_ChiTietHangHoaKM>();
            foreach (var item in tb)
            {
                DM_ChiTietHangHoaKM DM = new DM_ChiTietHangHoaKM();
                DM.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                DM.TenHangHoa = item.TenHangHoa;
                DM.ID_DonViQuiDoiMua = item.ID_DonViQuiDoiMua;
                DM.TenHangHoaMua = item.TenHangHoaMua;
                DM.ID_NhomHangHoa = item.ID_NhomHangHoa;
                DM.TenNhomHangHoa = item.TenNhomHangHoa;
                DM.ID_NhomHangHoaMua = item.ID_NhomHangHoaMua;
                DM.TenNhomHangHoaMua = item.TenNhomHangHoaMua;
                lst.Add(DM);
            }
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }
        //get list Đơn Vị khuyến mại
        public List<DM_DonVi> getLisDonViKM(Guid ID_KhuyenMai)
        {

            var tb = from kmap in db.DM_KhuyenMai_ApDung
                     join dv in db.DM_DonVi on kmap.ID_DonVi equals dv.ID
                     where kmap.ID_KhuyenMai == ID_KhuyenMai
                     group kmap by new
                     {
                         ID = dv.ID,
                         TenDonVi = dv.TenDonVi,
                         DiaChi = dv.DiaChi,
                         SoDienThoai = dv.SoDienThoai
                     } into g
                     select new
                     {
                         ID = g.Key.ID,
                         TenDonVi = g.Key.TenDonVi,
                         DiaChi = g.Key.DiaChi,
                         SoDienThoai = g.Key.SoDienThoai
                     };
            List<DM_DonVi> lst = new List<DM_DonVi>();
            foreach (var item in tb)
            {
                DM_DonVi DM = new DM_DonVi();
                DM.ID = item.ID;
                DM.TenDonVi = item.TenDonVi;
                DM.DiaChi = item.DiaChi;
                DM.SoDienThoai = item.SoDienThoai;
                lst.Add(DM);
            }
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }
        public List<DM_NhanVienKM> getlistNhanViemKM(Guid ID_KhuyenMai)
        {

            var tb = from kmap in db.DM_KhuyenMai_ApDung
                     join nv in db.NS_NhanVien on kmap.ID_NhanVien equals nv.ID
                     where kmap.ID_KhuyenMai == ID_KhuyenMai
                     group kmap by new
                     {
                         ID = nv.ID,
                         ID_KhuyenMai = kmap.ID_KhuyenMai,
                         TenNhanVien = nv.TenNhanVien
                     } into g
                     select new
                     {
                         ID = g.Key.ID,
                         ID_KhuyenMai = g.Key.ID_KhuyenMai,
                         TenNhanVien = g.Key.TenNhanVien
                     };
            List<DM_NhanVienKM> lst = new List<DM_NhanVienKM>();
            foreach (var item in tb)
            {
                DM_NhanVienKM DM = new DM_NhanVienKM();
                DM.ID = item.ID;
                DM.TenNhanVien = item.TenNhanVien;
                DM.ID_KhuyenMai = item.ID_KhuyenMai;
                lst.Add(DM);
            }
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }
        public List<DM_NhomKhachHangKM> getlistNhomHangKM(Guid ID_KhuyenMai)
        {

            var tb = from kmap in db.DM_KhuyenMai_ApDung
                     join nh in db.DM_NhomDoiTuong on kmap.ID_NhomKhachHang equals nh.ID
                     where kmap.ID_KhuyenMai == ID_KhuyenMai
                     group kmap by new
                     {
                         ID = nh.ID,
                         ID_KhuyenMai = kmap.ID_KhuyenMai,
                         TenNhomDoiTuong = nh.TenNhomDoiTuong
                     } into g
                     select new
                     {
                         ID = g.Key.ID,
                         ID_KhuyenMai = g.Key.ID_KhuyenMai,
                         TenNhomDoiTuong = g.Key.TenNhomDoiTuong
                     };
            List<DM_NhomKhachHangKM> lst = new List<DM_NhomKhachHangKM>();
            foreach (var item in tb)
            {
                DM_NhomKhachHangKM DM = new DM_NhomKhachHangKM();
                DM.ID = item.ID;
                DM.TenNhomDoiTuong = item.TenNhomDoiTuong;
                DM.ID_KhuyenMai = item.ID_KhuyenMai;
                lst.Add(DM);
            }
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }
        public IQueryable<DM_KhuyenMai_ChiTiet> GetsCTKM(Expression<Func<DM_KhuyenMai_ChiTiet, bool>> query)
        {

            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_KhuyenMai_ChiTiet;
                else
                    return db.DM_KhuyenMai_ChiTiet.Where(query);
            }
        }
        public IQueryable<DM_KhuyenMai_ApDung> GetsADKM(Expression<Func<DM_KhuyenMai_ApDung, bool>> query)
        {

            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_KhuyenMai_ApDung;
                else
                    return db.DM_KhuyenMai_ApDung.Where(query);
            }
        }
        //get danh mục đơn vị quy đổi
        public List<DM_HangHoaDTO> GetHangHoa_QuyDoi(Guid id)
        {


            var tbl = from hh in db.DM_HangHoa
                      join dvt in db.DonViQuiDois on hh.ID equals dvt.ID_HangHoa into HH_QD
                      from hh_qd in HH_QD.DefaultIfEmpty()
                      where hh_qd.ID_HangHoa == id
                      select new
                      {
                          ID = hh_qd.ID_HangHoa,
                          TenHangHoa = hh.TenHangHoa,
                          MaHangHoa = hh_qd.MaHangHoa,
                          SoLuong = 1,
                          GiaBan = hh_qd.GiaBan,
                          GiaNhap = hh_qd.GiaNhap,
                          GiaVon = hh_qd.GiaVon,
                          GiamGia = 0,
                          TenDonViTinh = hh_qd.TenDonViTinh,
                          ID_DonViQuiDoi = hh_qd.ID
                      };

            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();
            foreach (var item in tbl)
            {
                DM_HangHoaDTO dM_HangHoaDTO = new DM_HangHoaDTO();
                dM_HangHoaDTO.ID = item.ID; // ID_dvquydoi
                dM_HangHoaDTO.MaHangHoa = item.MaHangHoa;
                dM_HangHoaDTO.TenHangHoa = item.TenHangHoa;
                dM_HangHoaDTO.SoLuong = 1;
                //dM_HangHoaDTO.TonKho = ClassBH_HoaDon.TinhSLTon(DateTime.Now, item.ID);
                dM_HangHoaDTO.GiaBan = item.GiaBan;
                dM_HangHoaDTO.GiaVon = item.GiaVon; // 1* DonGia
                dM_HangHoaDTO.GiaNhap = item.GiaNhap;
                dM_HangHoaDTO.GiamGia = 0;
                dM_HangHoaDTO.TenDonViTinh = item.TenDonViTinh;
                dM_HangHoaDTO.ID_DonViQuiDoi = item.ID_DonViQuiDoi;

                lst.Add(dM_HangHoaDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }

        public List<DM_HangHoaDTO> getHangHoaBy_MaHangHoa(string maHH)
        {

            var tbl = from hh in db.DM_HangHoa
                      join dvt in db.DonViQuiDois on hh.ID equals dvt.ID_HangHoa into HH_QD
                      from hh_qd in HH_QD.DefaultIfEmpty()
                      where hh_qd.MaHangHoa.Contains(maHH)

                      select new
                      {
                          ID = hh_qd.ID_HangHoa,
                          TenHangHoa = hh.TenHangHoa,
                          MaHangHoa = hh_qd.MaHangHoa,
                          SoLuong = 1,
                          GiaBan = hh_qd.GiaBan,
                          GiaNhap = hh_qd.GiaNhap,
                          GiaVon = hh_qd.GiaVon,
                          GiamGia = 0,
                          TenDonViTinh = hh_qd.TenDonViTinh,
                          ID_DonViQuiDoi = hh_qd.ID
                      };

            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();
            foreach (var item in tbl)
            {
                DM_HangHoaDTO dM_HangHoaDTO = new DM_HangHoaDTO();
                dM_HangHoaDTO.ID = item.ID; // ID_dvquydoi
                dM_HangHoaDTO.MaHangHoa = item.MaHangHoa;
                dM_HangHoaDTO.TenHangHoa = item.TenHangHoa;
                dM_HangHoaDTO.SoLuong = 1;
                dM_HangHoaDTO.GiaBan = item.GiaBan;
                dM_HangHoaDTO.GiaVon = item.GiaVon; // 1* DonGia
                dM_HangHoaDTO.GiaNhap = item.GiaNhap;
                dM_HangHoaDTO.GiamGia = 0;
                dM_HangHoaDTO.TenDonViTinh = item.TenDonViTinh;
                dM_HangHoaDTO.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                lst.Add(dM_HangHoaDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }

        public class ParamSearchPromotion
        {
            public List<string> IDChiNhanhs { get; set; }
            public string TextSearch { get; set; }
            public int TypePromotion { get; set; }
            public int StatusActive { get; set; }
            public int Expired { get; set; }
            public int CurrentPage { get; set; }
            public int PageSize { get; set; }
        }

        public class List_HangHoaKhuyenMai
        {
            public Guid ID { get; set; }
            public Guid ID_HangHoa { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public double GiaVon { get; set; }
        }
        public class DM_ChuongTrinhKhuyenMai
        {
            public Guid ID { get; set; }
            public string MaKhuyenMai { get; set; }
            public string TenKhuyenMai { get; set; }
            public string GhiChu { get; set; }
            public string TrangThai { get; set; }
            public string HinhThuc { get; set; } // hình thức khuyến mại
            public int KieuHinhThuc { get; set; }
            public int LoaiKhuyenMai { get; set; } // Khuyến mại theo
            public DateTime ThoiGianBatDau { get; set; }
            public DateTime ThoiGianKetThuc { get; set; }
            public string NgayApDung { get; set; }
            public string ThangApDung { get; set; }
            public string ThuApDung { get; set; }
            public string GioApDung { get; set; }
            public string ApDungNgaySinhNhat { get; set; } // 0 - ko set, 1 - ngày, 2 - tuần, 3 - tháng
            public int ValueApDungSN { get; set; }
            public bool TatCaDonVi { get; set; }
            public bool TatCaDoiTuong { get; set; }
            public bool TatCaNhanVien { get; set; }
            public string NguoiTao { get; set; }
            public DateTime NgayTao { get; set; }
            public string NguoiSua { get; set; }
            public DateTime? NgaySua { get; set; }

            public int? TotalRow { get; set; }
            public double? TotalPage { get; set; }
        }
        public class DM_ChuongTrinhKhuyenMaiPRC
        {
            public Guid ID { get; set; }
            public Guid? ID_DonVi { get; set; }
            public string MaKhuyenMai { get; set; }
            public string TenKhuyenMai { get; set; }
            public string GhiChu { get; set; }
            public string TrangThai { get; set; }
            public string HinhThuc { get; set; } // hình thức khuyến mại
            public int LoaiKhuyenMai { get; set; } // Khuyến mại theo
            public int KieuHinhThuc { get; set; }
            public DateTime ThoiGianBatDau { get; set; }
            public DateTime ThoiGianKetThuc { get; set; }
            public string NgayApDung { get; set; }
            public string ThangApDung { get; set; }
            public string ThuApDung { get; set; }
            public string GioApDung { get; set; }
            public string ApDungNgaySinhNhat { get; set; } // 0 - ko set, 1 - ngày, 2 - tuần, 3 - tháng
            public int ValueApDungSN { get; set; }
            public bool TatCaDoiTuong { get; set; }
            public bool TatCaDonVi { get; set; }
            public bool TatCaNhanVien { get; set; }
            public string NguoiTao { get; set; }
        }
        public class BH_KhuyenMai_ChiTiet
        {
            public Guid ID { get; set; }
            public Guid ID_KhuyenMai { get; set; }
            public double? TongTienHang { get; set; }
            public double? GiamGia { get; set; }
            public bool? GiamGiaTheoPhanTram { get; set; }
            public Guid? ID_DonViQuiDoi { get; set; } //ID hàng tặng, giảm giá
            public Guid? ID_NhomHangHoa { get; set; } //ID nhóm hàng tặng, giảm giá
            public double? SoLuong { get; set; } // số lượng hàng tặng
            public Guid? ID_DonViQuiDoiMua { get; set; } //id hàng mua
            public Guid? ID_NhomHangHoaMua { get; set; } //id nhóm hàng mua
            public double? SoLuongMua { get; set; }
            public double? GiaKhuyenMai { get; set; }
        }
        public class DM_ChiTietKhuyenMai
        {
            public Guid ID { get; set; }
            public Guid ID_KhuyenMai { get; set; }
            public double? TongTienHang { get; set; }
            public double? GiamGia { get; set; }
            public double? DiemCong { get; set; }
            public bool? GiamGiaTheoPhanTram { get; set; }
            public Guid? ID_DonViQuiDoi { get; set; } //ID hàng tặng, giảm giá
            public string TenHangHoa { get; set; } //Tên hàng hóa tặng, giảm giá
            public Guid? ID_NhomHangHoa { get; set; } //ID nhóm hàng tặng, giảm giá
            public string TenNhomHangHoa { get; set; }// Tên nhóm hàng tặng, giảm giá
            public double? SoLuong { get; set; } // số lượng hàng tặng, giảm giá
            public Guid? ID_DonViQuiDoiMua { get; set; } //id hàng mua
            public string TenHangHoaMua { get; set; } //Tên hàng hóa mua
            public Guid? ID_NhomHangHoaMua { get; set; } //id nhóm hàng mua
            public string TenNhomHangHoaMua { get; set; } //Tên nhom hàng hóa mua
            public double? SoLuongMua { get; set; }
            public double? GiaKhuyenMai { get; set; }
            public int? STT { get; set; }
        }
        public class DM_LichSuKhuyenMai
        {
            public Guid ID { get; set; }
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string TenNhanVien { get; set; }
            public double DoanhThu { get; set; }
            public double? GiaTriKhuyenMai { get; set; }
        }
        public class DM_ChiTietHangHoaKM
        {
            public Guid ID { get; set; }
            public Guid ID_KhuyenMai { get; set; }
            public Guid? ID_DonViQuiDoi { get; set; } //ID hàng tặng, giảm giá
            public string TenHangHoa { get; set; } //Tên hàng hóa tặng, giảm giá
            public Guid? ID_NhomHangHoa { get; set; } //ID nhóm hàng tặng, giảm giá
            public string TenNhomHangHoa { get; set; }// Tên nhóm hàng tặng, giảm giá
            public Guid? ID_DonViQuiDoiMua { get; set; } //id hàng mua
            public string TenHangHoaMua { get; set; } //Tên hàng hóa mua
            public Guid? ID_NhomHangHoaMua { get; set; } //id nhóm hàng mua
            public string TenNhomHangHoaMua { get; set; } //Tên nhom hàng hóa mua
        }
        public class DM_ChiTietDonViKM
        {
            public Guid ID { get; set; }
            public Guid ID_KhuyenMai { get; set; }
            public Guid? ID_DonViQuiDoi { get; set; } //ID hàng tặng, giảm giá
            public string TenHangHoa { get; set; } //Tên hàng hóa tặng, giảm giá
            public Guid? ID_NhomHangHoa { get; set; } //ID nhóm hàng tặng, giảm giá
            public string TenNhomHangHoa { get; set; }// Tên nhóm hàng tặng, giảm giá
            public Guid? ID_DonViQuiDoiMua { get; set; } //id hàng mua
            public string TenHangHoaMua { get; set; } //Tên hàng hóa mua
            public Guid? ID_NhomHangHoaMua { get; set; } //id nhóm hàng mua
            public string TenNhomHangHoaMua { get; set; } //Tên nhom hàng hóa mua

        }
        public class DM_NhanVienKM
        {
            public Guid ID { get; set; }
            public Guid ID_KhuyenMai { get; set; }
            public string TenNhanVien { get; set; }
        }
        public class DM_NhomKhachHangKM
        {
            public Guid ID { get; set; }
            public Guid ID_KhuyenMai { get; set; }
            public string TenNhomDoiTuong { get; set; }
        }

        #region delete
        //xóa khuyến mại chi tiết
        public string Delete_KhuyenMaiChiTiet(Guid ID_KhuyenMai)
        {
            string strErr = string.Empty;


            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                List<DM_KhuyenMai_ChiTiet> objDel = db.DM_KhuyenMai_ChiTiet.Where(x => x.ID_KhuyenMai == ID_KhuyenMai).ToList();
                if (objDel != null)
                {
                    try
                    {
                        db.DM_KhuyenMai_ChiTiet.RemoveRange(db.DM_KhuyenMai_ChiTiet.Where(idHD => idHD.ID_KhuyenMai == ID_KhuyenMai));
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
                    strErr = "Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
                    return strErr;
                }
            }
            return strErr;
        }

        //xóa khuyến mại áp dụng
        public string Delete_KhuyenMaiApDung(Guid ID_KhuyenMai)
        {
            string strErr = string.Empty;


            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                List<DM_KhuyenMai_ApDung> objDel = db.DM_KhuyenMai_ApDung.Where(x => x.ID_KhuyenMai == ID_KhuyenMai).ToList();
                if (objDel != null)
                {
                    try
                    {
                        db.DM_KhuyenMai_ApDung.RemoveRange(db.DM_KhuyenMai_ApDung.Where(idHD => idHD.ID_KhuyenMai == ID_KhuyenMai));
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
                    strErr = "Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
                    return strErr;
                }
            }
            return strErr;
        }
        //xóa khuyến mại
        public string Delete_KhuyenMai(Guid ID)
        {
            string strErr = string.Empty;
            try
            {
                if (db == null)
                {
                    return "Kết nối CSDL không hợp lệ";
                }
                else
                {
                    DM_KhuyenMai objDel = db.DM_KhuyenMai.Find(ID);
                    if (objDel != null)
                    {
                        db.DM_KhuyenMai.Remove(objDel);
                        db.SaveChanges();
                    }
                    else
                    {
                        strErr = "Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
                        return strErr;
                    }
                }
            }
            catch (Exception exxx)
            {
                strErr = exxx.Message;
                return strErr;
            }
            return strErr;
        }
        public string Delete_ChotSo(Guid ID_DonVi)
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
                    db.ChotSo.RemoveRange(db.ChotSo.Where(idHD => idHD.ID_DonVi == ID_DonVi));
                    db.SaveChanges();
                }
                catch (Exception exxx)
                {
                    strErr = exxx.Message;
                    return strErr;
                }
            }
            return strErr;
        }
        #endregion

    }
}
