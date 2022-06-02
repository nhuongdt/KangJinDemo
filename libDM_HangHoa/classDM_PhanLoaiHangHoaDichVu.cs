using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Entity;
using System.Linq.Expressions;


namespace libDM_HangHoa
{
    public class classDM_PhanLoaiHangHoaDichVu
    {
        private SsoftvnContext db;
        public classDM_PhanLoaiHangHoaDichVu(SsoftvnContext _db)
        {
            db = _db;

        }
        #region select
        public  DM_PhanLoaiHangHoaDichVu Select_PhanLoaiHangHoaDichVu(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_PhanLoaiHangHoaDichVu.Find(id);
            }
        }

        public  IQueryable<DM_PhanLoaiHangHoaDichVu> Gets(Expression<Func<DM_PhanLoaiHangHoaDichVu, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query != null)
                    return db.DM_PhanLoaiHangHoaDichVu.Where(query).OrderBy(p => p.MaPhanLoai);
                else
                    return db.DM_PhanLoaiHangHoaDichVu.OrderBy(p => p.MaPhanLoai);
            }
        }
        public  DM_PhanLoaiHangHoaDichVu Get(Expression<Func<DM_PhanLoaiHangHoaDichVu, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_PhanLoaiHangHoaDichVu.Where(query).FirstOrDefault();
            }
        }

         bool DM_PhanLoaiHangHoaDichVuExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_PhanLoaiHangHoaDichVu.Count(e => e.ID == id) > 0;
            }
        }

        #endregion

        #region insert
        public  string Add_PhanLoaiHangHoaDichVu(DM_PhanLoaiHangHoaDichVu objAdd)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.DM_PhanLoaiHangHoaDichVu.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return string.Empty;
        }
        #endregion

        #region update
        public  string Update_PhanLoaiHangHoaDichVu(DM_PhanLoaiHangHoaDichVu objNew)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    DM_PhanLoaiHangHoaDichVu objUpd = db.DM_PhanLoaiHangHoaDichVu.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.GhiChu = objNew.GhiChu;
                        objUpd.MaPhanLoai = objNew.MaPhanLoai;
                        objUpd.TenPhanLoai = objNew.TenPhanLoai;
                        objUpd.ThoiGianBaoHanh = objNew.ThoiGianBaoHanh;

                        objUpd.NgaySua = objNew.NgaySua;
                        objUpd.NgayTao = objNew.NgayTao;
                        objUpd.NguoiSua = objNew.NguoiSua;
                        objUpd.NguoiTao = objNew.NguoiTao;
                        //
                        db.Entry(objUpd).State = EntityState.Modified;
                        #endregion
                        //
                        db.SaveChanges();
                    }
                    else
                    {
                        return "Không tìm thấy dữ liệu cần cập nhật trên hệ thống";
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
        
        static string CheckDelete_PhanLoaiHangHoaDichVu(DM_PhanLoaiHangHoaDichVu objDel)
        {
            string strCheck = string.Empty;
            if (objDel != null)
            {
                if (objDel.DM_HangHoa != null && objDel.DM_HangHoa.Count > 0)
                {
                    strCheck = "Phân loại Hàng hóa/Dịch vụ này đã được sử dụng để khai báo danh mục hàng hóa. Không thể xóa.";
                    return strCheck;
                }
            }
            return strCheck;
        }

        public  string Delete_PhanLoaiHangHoaDichVu(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                DM_PhanLoaiHangHoaDichVu objDel = db.DM_PhanLoaiHangHoaDichVu.Find(id);
                if (objDel != null)
                {
                    string strCheckDel = CheckDelete_PhanLoaiHangHoaDichVu(objDel);
                    if (strCheckDel == null || strCheckDel == string.Empty)
                    {
                        try
                        {
                            db.DM_PhanLoaiHangHoaDichVu.Remove(objDel);
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
        
    }
}
