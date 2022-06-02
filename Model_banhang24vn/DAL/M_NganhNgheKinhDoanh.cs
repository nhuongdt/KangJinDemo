using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn
{
    public class M_NganhNgheKinhDoanh
    {
        public static List<NganhNgheKinhDoanh> SelectAll()
        {
            BanHang24vnContext db = new BanHang24vnContext();
            return db.NganhNgheKinhDoanhs.Where(o=>o.Status==true).ToList();
        }
        public static IQueryable<NganhNgheKinhDoanh> Gets()
        {
            BanHang24vnContext db = new BanHang24vnContext();
            return db.NganhNgheKinhDoanhs;
        }
        public static NganhNgheKinhDoanh Select_ID(Guid id)
        {
            BanHang24vnContext db = new BanHang24vnContext();
            return db.NganhNgheKinhDoanhs.Find(id);
        }

        static bool NganhNgheKinhDoanhExists(Guid id)
        {
            BanHang24vnContext db = new BanHang24vnContext();
            return db.NganhNgheKinhDoanhs.Count(e => e.ID == id) > 0;
        }

        public static string Add_NganhNgheKinhDoanh(NganhNgheKinhDoanh objAdd)
        {
            string strError = string.Empty;
            BanHang24vnContext db = new BanHang24vnContext();
            try
            {
                objAdd.ID = Guid.NewGuid();
                db.NganhNgheKinhDoanhs.Add(objAdd);
                //
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (NganhNgheKinhDoanhExists(objAdd.ID))
                {
                    return "Ngành nghề muốn cập nhật có ID trùng với nghành nghề đã được cập nhật trước trên hệ thống. Nạp lại để tạo ID mới.";
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
            }
            return strError;
        }

        public static string Update_NganhNgheKinhDoanh(NganhNgheKinhDoanh objNew)
        {
            string strError = string.Empty;
            BanHang24vnContext db = new BanHang24vnContext();

            NganhNgheKinhDoanh objUpd = db.NganhNgheKinhDoanhs.Find(objNew.ID);
            if (objUpd != null)
            {
                objUpd.MaNganhNghe = objNew.MaNganhNghe;
                objUpd.TenNganhNghe = objNew.TenNganhNghe;
            }
            db.Entry(objUpd).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return strError;
        }

        static string CheckDelete_NganhNgheKinhDoanh(NganhNgheKinhDoanh objDel)
        {
            string strError = string.Empty;
            if (objDel != null)
            {
                if (objDel.CuaHangDangKies != null && objDel.CuaHangDangKies.Count > 0)
                {
                    return "Ngành nghề đã được sử dụng để khai báo thông tin khách hàng";
                }
            }
            return strError;
        }

        public static string Delete_NganhNgheKinhDoanh(Guid id)
        {
            BanHang24vnContext db = new BanHang24vnContext();
            string strError = string.Empty;

            NganhNgheKinhDoanh objDel = db.NganhNgheKinhDoanhs.Find(id);
            if (objDel != null)
            {
                string strCheckDel = CheckDelete_NganhNgheKinhDoanh(objDel);
                if (strCheckDel == null || strCheckDel == string.Empty || strCheckDel.Trim() == "")
                {
                    try
                    {
                        //HT_Quyen_Nhom
                        if (objDel.HT_Quyen_NganhNgheKinhDoanh != null && objDel.HT_Quyen_NganhNgheKinhDoanh.Count > 0)
                            db.HT_Quyen_NganhNgheKinhDoanh.RemoveRange(objDel.HT_Quyen_NganhNgheKinhDoanh.ToList());
                        //NhomHangHoa_NganhNgheKinhDoanh
                        if (objDel.NhomHangHoa_NganhNgheKinhDoanh != null && objDel.NhomHangHoa_NganhNgheKinhDoanh.Count > 0)
                            db.NhomHangHoa_NganhNgheKinhDoanh.RemoveRange(objDel.NhomHangHoa_NganhNgheKinhDoanh.ToList());
                        //
                        db.NganhNgheKinhDoanhs.Remove(objDel);
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
            return strError;
        }
    }
}
