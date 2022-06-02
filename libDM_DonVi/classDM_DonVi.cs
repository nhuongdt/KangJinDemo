using libHT_NguoiDung;
using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;

namespace libDM_DonVi
{
    public class classDM_DonVi
    {
        private SsoftvnContext db;

        public classDM_DonVi(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public IQueryable<DM_DonVi> Gets(Expression<Func<DM_DonVi, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_DonVi;
                else
                    return db.DM_DonVi.Where(query);
            }
        }

        public List<DM_DonVi_ChotSo> ChotSo_GetListDonVi()
        {
            var sql = @" select distinct dv.ID, dv.TenDonVi, cs.NgayChotSo, ch.KhoaSo
                        from DM_DonVi dv
                        left  join ChotSo cs on cs.ID_DonVi = dv.ID
                        left join HT_CauHinhPhanMem ch on dv.ID = ch.ID_DonVi
                        where dv.TrangThai is null or dv.TrangThai= 1";
            return db.Database.SqlQuery<DM_DonVi_ChotSo>(sql).ToList();
        }

        public List<DM_DonVi> GetDonVi_User(Guid ID_NguoiDung)
        {
            var tb = from dv in db.DM_DonVi
                     join qtct in db.NS_QuaTrinhCongTac on dv.ID equals qtct.ID_DonVi
                     join nv in db.NS_NhanVien on qtct.ID_NhanVien equals nv.ID
                     join nd in db.HT_NguoiDung on nv.ID equals nd.ID_NhanVien
                     where nd.ID == ID_NguoiDung && (dv.TrangThai == true || dv.TrangThai == null)
                     orderby dv.TenDonVi
                     select new
                     {
                         dv.ID,
                         dv.MaDonVi,
                         dv.TenDonVi,
                         dv.DiaChi,
                         dv.SoDienThoai
                     };
            List<DM_DonVi> lst = new List<DM_DonVi>();
            foreach (var item in tb)
            {
                DM_DonVi DM = new DM_DonVi();
                DM.ID = item.ID;
                DM.MaDonVi = item.MaDonVi;
                DM.TenDonVi = item.TenDonVi;
                DM.DiaChi = item.DiaChi;
                DM.SoDienThoai = item.SoDienThoai;
                lst.Add(DM);
            }
            if (lst.Count > 0)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }
        public DM_DonVi Get(Expression<Func<DM_DonVi, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_DonVi.Where(query).FirstOrDefault();
            }
        }

        public static List<DM_DonVi> Select_DMDonVi_User(string userName)
        {
            return null;
        }
        public static List<DM_DonVi> Select_DMDonVi_IDParent(Guid idDonVi_parent)
        {
            return null;
        }
        public DM_DonVi Select_DonVi(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_DonVi.Find(id);
            }
        }

        public List<DM_DonViDTO> getListDVByIDNguoiDung(Guid? idnhanvien)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from nv in db.NS_NhanVien
                          join qtct in db.NS_QuaTrinhCongTac on nv.ID equals qtct.ID_NhanVien
                          join dv in db.DM_DonVi on qtct.ID_DonVi equals dv.ID
                          where nv.ID == idnhanvien && (dv.TrangThai == true || dv.TrangThai == null)
                          orderby dv.TenDonVi
                          select new DM_DonViDTO
                          {
                              ID = dv.ID,
                              TenDonVi = dv.TenDonVi,
                              DiaChi = dv.DiaChi,
                              SoDienThoai = dv.SoDienThoai
                          };
                return tbl.Distinct().ToList();
            }
        }

        public List<HT_DonVi_VaiTro> getListIDDonViVaiTro(Guid? idnhanvien, Guid idnguoidung)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from nv in db.NS_NhanVien
                          join qtct in db.NS_QuaTrinhCongTac on nv.ID equals qtct.ID_NhanVien
                          join dv in db.DM_DonVi on qtct.ID_DonVi equals dv.ID
                          join ndnhom in db.HT_NguoiDung_Nhom.Where(p => p.IDNguoiDung == idnguoidung) on qtct.ID_DonVi equals ndnhom.ID_DonVi into nd
                          from ndvaitro in nd.DefaultIfEmpty()
                          where nv.ID == idnhanvien && nv.TrangThai != 0 && nv.DaNghiViec != true && (dv.TrangThai == true || dv.TrangThai == null)
                          select new HT_DonVi_VaiTro
                          {
                              ID = ndvaitro.ID == null ? Guid.Empty : ndvaitro.ID,
                              ID_DonVi = qtct.ID_DonVi,
                              TenDonVi = dv.TenDonVi,
                              ID_VaiTro = ndvaitro.IDNhomNguoiDung == null ? Guid.Empty : ndvaitro.IDNhomNguoiDung,
                              IDNguoiDung = idnguoidung
                          };
                tbl = tbl.GroupBy(p => p.ID_DonVi).Select(p => new HT_DonVi_VaiTro
                {
                    ID = p.FirstOrDefault().ID,
                    ID_DonVi = p.Key,
                    TenDonVi = p.FirstOrDefault().TenDonVi,
                    ID_VaiTro = p.FirstOrDefault().ID_VaiTro,
                    IDNguoiDung = p.FirstOrDefault().IDNguoiDung
                });
                return tbl.ToList();

            }
        }

        public List<DM_DonViDTO> getListDVByID(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from dv in db.DM_DonVi
                          where dv.ID == id
                          select new DM_DonViDTO
                          {
                              ID = dv.ID,
                              TenDonVi = dv.TenDonVi,
                              MaDonVi = dv.MaDonVi,
                              DiaChi = dv.DiaChi,
                              SoDienThoai = dv.SoDienThoai,
                          };
                return tbl.ToList();
            }
        }

        private bool DM_DonViExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_DonVi.Count(e => e.ID == id) > 0;
            }
        }
        #endregion

        #region Insert
        public string Add_DonVi(DM_DonVi dM_DonVi)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                strErr = "Kết nối CSDL không hợp lệ";
                return strErr;
            }
            else
            {
                try
                {
                    db.DM_DonVi.Add(dM_DonVi);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
            return strErr;
        }

        #endregion

        #region Update_DonVi
        public string Update_DonVi(DM_DonVi objNew)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    DM_DonVi objUpd = db.DM_DonVi.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.DiaChi = objNew.DiaChi;
                        objUpd.HienThi_Chinh = objNew.HienThi_Chinh;
                        objUpd.HienThi_Phu = objNew.HienThi_Phu;
                        objUpd.ID_NganHang = objNew.ID_NganHang;
                        objUpd.ID_Parent = objNew.ID_Parent;
                        objUpd.KiTuDanhMa = objNew.KiTuDanhMa;
                        objUpd.MaDonVi = objNew.MaDonVi;
                        objUpd.MaSoThue = objNew.MaSoThue;
                        objUpd.SoDienThoai = objNew.SoDienThoai;
                        objUpd.SoFax = objNew.SoFax;
                        objUpd.SoTaiKhoan = objNew.SoTaiKhoan;
                        objUpd.TenDonVi = objNew.TenDonVi;
                        objUpd.Website = objNew.Website;
                        objUpd.TrangThai = objNew.TrangThai;
                        objUpd.NgaySua = objNew.NgaySua;
                        objUpd.NgayTao = objNew.NgayTao;
                        objUpd.NguoiSua = objNew.NguoiSua;
                        objUpd.NguoiTao = objNew.NguoiTao;
                        db.Entry(objUpd).State = EntityState.Modified;
                        db.SaveChanges();
                        #endregion
                    }
                    else
                    {
                        return "Không tìm thấy thông tin dữ liệu cần sửa.";
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return string.Empty;
        }
        #endregion

        #region delete
        public string CheckDelete_DonVi(DM_DonVi objDel)
        {
            string strErr = string.Empty;
            if (db.BH_HoaDon.Count(p => p.ID_CheckIn == objDel.ID) > 0)
                return "Chi nhánh có hóa đơn chuyển hàng đang đợi nhận không thể xóa";
            if (db.BH_HoaDon.Count(p => p.ID_DonVi == objDel.ID) > 0)
                return "Chi nhánh đã phát sinh giao dịch, không thể xóa";
            return strErr;
        }

        public string Delete_DonVi(Guid id)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                DM_DonVi objDel = db.DM_DonVi.Find(id);
                if (objDel != null)
                {
                    string strCheckDel = CheckDelete_DonVi(objDel);
                    if (strCheckDel == null || strCheckDel == string.Empty)
                    {
                        try
                        {
                            IQueryable<NhomHangHoa_DonVi> lstNhomHHDVs = db.NhomHangHoa_DonVi.Where(p => p.ID_DonVi == id);
                            if (lstNhomHHDVs != null && lstNhomHHDVs.ToList().Count > 0)
                            {
                                db.NhomHangHoa_DonVi.RemoveRange(lstNhomHHDVs);
                                db.SaveChanges();
                            }
                            IQueryable<Kho_DonVi> lstDVis = db.Kho_DonVi.Where(p => p.ID_DonVi == id);
                            if (lstDVis != null && lstDVis.ToList().Count > 0)
                            {
                                db.Kho_DonVi.RemoveRange(lstDVis);
                                db.SaveChanges();
                            }
                            IQueryable<NhomDoiTuong_DonVi> lstNhomDoiTuongs = db.NhomDoiTuong_DonVi.Where(p => p.ID_DonVi == id);
                            if (lstNhomDoiTuongs != null && lstNhomDoiTuongs.ToList().Count > 0)
                            {
                                db.NhomDoiTuong_DonVi.RemoveRange(lstNhomDoiTuongs);
                                db.SaveChanges();
                            }
                            //
                            db.DM_DonVi.Remove(objDel);
                            db.SaveChanges();
                            return string.Empty;
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
                else
                {
                    return "Chi nhánh không tồn tại";
                }
            }
        }
        #endregion
    }
    public class DM_DonVi_byUser
    {
        public Guid ID { get; set; }
        public string TenDonVi { get; set; }
        public string SoDienThoai { get; set; }
        public int checkSearch { get; set; }
    }
    public class DM_DonViDTO
    {
        public Guid ID { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string TenDonViKTD { get; set; }
        public string TenDonViBoDAu { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public bool? TrangThai { get; set; }
    }
    public class DM_DonVi_ChotSo
    {
        public Guid ID { get; set; }
        public string TenDonVi { get; set; }
        public string TenDonVi_GC { get; set; }
        public string TenDonVi_CV { get; set; }
        public DateTime? NgayChotSo { get; set; }
        public bool? KhoaSo { get; set; }
    }
}
