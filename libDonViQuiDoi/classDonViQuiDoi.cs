using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data;
using libDM_DonVi;
using System.Data.SqlClient;

namespace libDonViQuiDoi
{
    public class classDonViQuiDoi
    {
        private SsoftvnContext db;

        public classDonViQuiDoi(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public DonViQuiDoi Select_DonViQuiDoi(string maHangHoa)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DonViQuiDois.Where(p => p.MaHangHoa == maHangHoa).FirstOrDefault();
            }
        }
        public DonViQuiDoi Select_DonViQuiDoiIdHang(Guid idhanghoa)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DonViQuiDois.Where(p => p.ID_HangHoa == idhanghoa).FirstOrDefault();
            }
        }

        //check mã hàng 
        public bool ChekMaHangDatabase(string maHangHoa)
        {
            bool dung = true;
            if (maHangHoa != "")
            {
                DonViQuiDoi objDVT_QuyDoi_New = Select_DonViQuiDoi(maHangHoa.Trim());
                if (objDVT_QuyDoi_New != null)
                {
                    dung = false;
                }
            }
            return dung;
        }
        public List<DonViQuiDoi> Select_DonViQuiDois_IDHangHoa(Guid idHHDV)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DonViQuiDois.Where(p => p.ID_HangHoa == idHHDV && p.Xoa == false).ToList();
            }
        }

        public List<DonViQuiDoi> Gets(Expression<Func<DonViQuiDoi, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DonViQuiDois.Where(query).ToList();
            }
        }
        public DonViQuiDoi Get(Expression<Func<DonViQuiDoi, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DonViQuiDois.Where(query).FirstOrDefault();
            }
        }
        public bool DonViQuiDoiExists(string strMaHangHoa)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DonViQuiDois.Count(e => e.MaHangHoa == strMaHangHoa) > 0;
            }
        }

        public DonViQuiDoi GetDonViTinhChuan(Guid idhanghoa)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DonViQuiDois.Where(p => p.ID_HangHoa == idhanghoa).Where(p => p.LaDonViChuan == true).FirstOrDefault();
            }
        }

        public List<DonViQuiDoi> GetListDonViTinhPhu(Guid idhanghoa)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DonViQuiDois.Where(p => p.ID_HangHoa == idhanghoa).Where(p => p.LaDonViChuan == false && p.Xoa != true).ToList();
            }
        }

        public string GetMaHangHoa()
        {
            string format = "{0:0000}";
            string mahanghoa = "HH0";
            string mahang = db.DonViQuiDois.Where(p => p.MaHangHoa.Contains(mahanghoa)).Where(p => p.MaHangHoa.Length == 7).OrderByDescending(p => p.MaHangHoa).Select(p => p.MaHangHoa).FirstOrDefault();
            if (mahang == null)
            {
                mahanghoa = mahanghoa + string.Format(format, 1);
            }
            else
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("LoaiHoaDon", 88));
                List<MaxCodeMaHoaDon> lst = db.Database.SqlQuery<MaxCodeMaHoaDon>("EXEC GetMaHoaDon_AuTo @LoaiHoaDon", paramlist.ToArray()).ToList();
                double tempstt = lst.FirstOrDefault().MaxCode + 1;
                mahanghoa = mahanghoa + string.Format(format, tempstt);
            }
            return mahanghoa;
        }

        public string GetMaDichVu()
        {
            string format = "{0:0000}";
            string madichvu = "DV0";
            string madv = db.DonViQuiDois.Where(p => p.MaHangHoa.Contains(madichvu)).Where(p => p.MaHangHoa.Length == 7).OrderByDescending(p => p.MaHangHoa).Select(p => p.MaHangHoa).FirstOrDefault();
            if (madv == null)
            {
                madichvu = madichvu + string.Format(format, 1);
            }
            else
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("LoaiHoaDon", 99));
                List<MaxCodeMaHoaDon> lst = db.Database.SqlQuery<MaxCodeMaHoaDon>("EXEC GetMaHoaDon_AuTo @ID_HoaDon", paramlist.ToArray()).ToList();
                double tempstt = lst.FirstOrDefault().MaxCode + 1;
                madichvu = madichvu + string.Format(format, tempstt);
            }
            return madichvu;
        }
        public class MaxCodeMaHoaDon
        {
            public double MaxCode { get; set; }
        }

        #endregion

        #region insert
        public string Add_DonViQuiDoi(DonViQuiDoi objAdd)
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
                    if (db.DonViQuiDois.Where(p => p.MaHangHoa == objAdd.MaHangHoa).FirstOrDefault() != null)
                    {
                        return "Mã hàng hóa đã tồn tại";
                    }
                    else
                    {
                        db.DonViQuiDois.Add(objAdd);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    strErr = "Add_DonViQuiDoi " + ex.Message + ex.InnerException;
                    CookieStore.WriteLog(strErr);
                }
            }
            return strErr;
        }
        #endregion

        public string AddDM_HangHoa_TonKho(Guid iddonviquidoi, double tonkho, Guid iddonvi, Guid? idlohang = null)
        {
            if (db == null)
            {
                return "Error";
            }
            else
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                var dv = _classDMDV.Gets(null);
                foreach (var item in dv)
                {
                    DM_HangHoa_TonKho hhtonkho = new DM_HangHoa_TonKho();
                    hhtonkho.ID = Guid.NewGuid();
                    hhtonkho.ID_DonViQuyDoi = iddonviquidoi;
                    hhtonkho.ID_DonVi = item.ID;
                    hhtonkho.ID_LoHang = idlohang;
                    hhtonkho.TonKho = item.ID == iddonvi ? tonkho : 0;
                    db.DM_HangHoa_TonKho.Add(hhtonkho);
                    //Add_DM_HangHoa_TonKho(hhtonkho);
                }
                db.SaveChanges();
                return "";
            }
        }

        public string AddDM_HangHoa_TonKhoKhiThemDVT(Guid iddonviquidoi, double tonkho, Guid iddonvi, Guid? idlohang = null)
        {
            if (db == null)
            {
                return "Error";
            }
            else
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                DonViQuiDoi dvtAdd = db.DonViQuiDois.Where(p => p.ID == iddonviquidoi).FirstOrDefault();
                Guid id_hanghoa = dvtAdd.ID_HangHoa;
                Guid id_donviquidoi_chuan = db.DonViQuiDois.Where(p => p.ID_HangHoa == id_hanghoa).FirstOrDefault().ID;
                List<DM_HangHoa_TonKho> dmhhtonkho = db.DM_HangHoa_TonKho.Where(p => p.ID_DonViQuyDoi == id_donviquidoi_chuan && p.ID_DonVi == iddonvi).ToList();
                var dv = _classDMDV.Gets(null);

                foreach (var itemton in dmhhtonkho)
                {
                    foreach (var item in dv)
                    {
                        DM_HangHoa_TonKho hhtonkho = new DM_HangHoa_TonKho();
                        hhtonkho.ID = Guid.NewGuid();
                        hhtonkho.ID_DonViQuyDoi = iddonviquidoi;
                        hhtonkho.ID_DonVi = item.ID;
                        hhtonkho.ID_LoHang = itemton.ID_LoHang;
                        hhtonkho.TonKho = item.ID == iddonvi ? Math.Round(itemton.TonKho / dvtAdd.TyLeChuyenDoi, 3, MidpointRounding.ToEven) : 0;
                        //Add_DM_HangHoa_TonKho(hhtonkho);
                        db.DM_HangHoa_TonKho.Add(hhtonkho);
                    }
                }
                db.SaveChanges();

                return "";
            }
        }

        public string UpdateID_LoHangChoCacChiNhanh(Guid iddonviquidoi, Guid? idlohang)
        {
            if (db == null)
            {
                return "Error";
            }
            else
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                var dv = _classDMDV.Gets(null);
                foreach (var item in dv)
                {
                    DM_HangHoa_TonKho hhtonkhoupdate = db.DM_HangHoa_TonKho.Where(p => p.ID_DonViQuyDoi == iddonviquidoi && p.ID_DonVi == item.ID && p.ID_LoHang == null).FirstOrDefault();
                    if (hhtonkhoupdate != null)
                    {
                        hhtonkhoupdate.TonKho = 0;
                        hhtonkhoupdate.ID_LoHang = idlohang;
                        //Update_DM_HangHoa_TonKho(hhtonkhoupdate);
                    }
                    else
                    {
                        hhtonkhoupdate = db.DM_HangHoa_TonKho.Where(p => p.ID_DonViQuyDoi == iddonviquidoi && p.ID_DonVi == item.ID && p.ID_LoHang == idlohang).FirstOrDefault();
                        if (hhtonkhoupdate == null)
                        {
                            DM_HangHoa_TonKho newhhton = new DM_HangHoa_TonKho();
                            newhhton.ID = Guid.NewGuid();
                            newhhton.ID_DonViQuyDoi = iddonviquidoi;
                            newhhton.ID_DonVi = item.ID;
                            newhhton.ID_LoHang = idlohang;
                            newhhton.TonKho = 0;
                            //Add_DM_HangHoa_TonKho(newhhton);
                            db.DM_HangHoa_TonKho.Add(newhhton);
                        }
                    }
                }
                db.SaveChanges();
                return "";
            }
        }

        public string Add_DM_HangHoa_TonKho(DM_HangHoa_TonKho objAdd)
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
                    db.DM_HangHoa_TonKho.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("Add_DM_HangHoa_TonKho " + ex.Message + ex.InnerException);
                }
            }
            return strErr;

        }

        #region update
        public string Update_DonViQuiDoi(DonViQuiDoi objNew)
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
                    DonViQuiDoi objUpd = db.DonViQuiDois.Find(objNew.ID);
                    objUpd.Xoa = objNew.Xoa;
                    objUpd.GhiChu = objNew.GhiChu;
                    objUpd.GiaBan = objNew.GiaBan;
                    objUpd.GiaVon = objNew.GiaVon;
                    objUpd.MaHangHoa = objNew.MaHangHoa;
                    if (objUpd.MaHangHoa == null || objUpd.MaHangHoa == string.Empty)
                    {
                        objUpd.MaHangHoa = GetMaHangHoa();
                    }
                    else
                    {
                        objUpd.MaHangHoa = objNew.MaHangHoa;
                    }
                    objUpd.ID_HangHoa = objNew.ID_HangHoa;
                    objUpd.LaDonViChuan = objNew.LaDonViChuan;
                    objUpd.TenDonViTinh = objNew.TenDonViTinh;
                    objUpd.TyLeChuyenDoi = objNew.TyLeChuyenDoi;
                    objUpd.NguoiSua = objNew.NguoiSua;
                    objUpd.NgaySua = objNew.NgaySua;

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

        public string UpdateGiaBanTuNhapHang(DonViQuiDoi objNew)
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
                    DonViQuiDoi objUpd = db.DonViQuiDois.Find(objNew.ID);
                    objUpd.GiaBan = objNew.GiaBan;
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

        public string Update_DM_HangHoa_TonKho(DM_HangHoa_TonKho objNew)
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
                    DM_HangHoa_TonKho objUpd = db.DM_HangHoa_TonKho.Find(objNew.ID);
                    objUpd.TonKho = objNew.TonKho;
                    objUpd.ID_LoHang = objNew.ID_LoHang;
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

        public string Update_DonViQuiDoiKhiTaoHD(DonViQuiDoi objNew)
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
                    DonViQuiDoi objUpd = db.DonViQuiDois.Find(objNew.ID);
                    #region update
                    objUpd.GiaNhap = objNew.GiaNhap;

                    db.Entry(objUpd).State = EntityState.Modified;
                    #endregion

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        public string Update_DMGiaVon(DM_GiaVon objNew)
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
                    DM_GiaVon objUpd = db.DM_GiaVon.Find(objNew.ID);
                    //bool checkexist = MaHangHoaExist(objNew.MaHangHoa, objUpd.MaHangHoa);
                    //if (checkexist==true)
                    //{
                    objUpd.GiaVon = objNew.GiaVon;
                    #region update
                    db.Entry(objUpd).State = EntityState.Modified;
                    #endregion

                    db.SaveChanges();
                    //}
                    //else
                    //{
                    //    return "Mã hàng hóa tồn tại trong hệ thống";
                    //}
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
        static string CheckDelete_DonViQuiDoi(DonViQuiDoi objDel)
        {
            string strCheck = string.Empty;
            if (objDel != null)
            {
                if (objDel.BH_HoaDon_ChiTiet != null && objDel.BH_HoaDon_ChiTiet.Count > 0)
                {
                    strCheck = "Đơn vị quy đổi đã được sử dụng để lập chi tiết hóa đơn. Không thể xóa.";
                    return strCheck;
                }
                if (objDel.DinhLuongDichVus != null && objDel.DinhLuongDichVus.Count > 0)
                {
                    strCheck = "Đơn vị quy đổi đã được sử dụng để lập Định lượng hàng hóa - Dịch vụ. Không thể xóa.";
                    return strCheck;
                }
                if (objDel.DM_GiaBan_ChiTiet != null && objDel.DM_GiaBan_ChiTiet.Count > 0)
                {
                    strCheck = "Đơn vị quy đổi đã được sử dụng để lập Danh mục giá bán hàng hóa - Dịch vụ. Không thể xóa.";
                    return strCheck;
                }
            }
            return strCheck;
        }

        public string Delete_DonViQuiDoi(Guid id)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                DonViQuiDoi objDel = db.DonViQuiDois.Find(id);
                if (objDel != null)
                {
                    string strCheckDel = CheckDelete_DonViQuiDoi(objDel);
                    if (strCheckDel == null || strCheckDel == string.Empty)
                    {
                        try
                        {
                            db.DonViQuiDois.Remove(objDel);
                            //
                            db.SaveChanges();
                        }
                        catch (Exception exxx)
                        {
                            return exxx.Message;
                        }
                    }
                    else
                    {
                        return strCheckDel;
                    }
                }
            }
            return string.Empty;
        }
        #endregion 


        public bool MaHangHoaExist(string Mamoi, string Macu)
        {
            bool check = false;
            IQueryable<string> mahanghoa = db.DonViQuiDois.Where(p => p.MaHangHoa == Mamoi).Select(p => p.MaHangHoa);
            if (mahanghoa != null)
            {
                if (mahanghoa.ToString() == Macu)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                check = false;
            }
            return check;
        }
    }
}
