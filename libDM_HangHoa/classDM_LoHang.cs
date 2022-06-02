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
    public class classDM_LoHang
    {
        private SsoftvnContext db;

        public classDM_LoHang(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public  DM_LoHang Select_LoHang(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_LoHang.Find(id);
            }
        }

        public  DM_LoHang Select_LoHang_HangHoa(string LoHang, Guid idHangHoa)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_LoHang.Where(p => p.ID_HangHoa == idHangHoa && p.MaLoHang.Trim().ToLower() == LoHang.Trim().ToLower()).FirstOrDefault();
            }
        }

        public  bool CheckMaLoHangTrung(string malohang, Guid idhanghoa)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                List<DM_LoHang> lst = db.DM_LoHang.Where(p => p.MaLoHang == malohang && p.ID_HangHoa == idhanghoa).ToList();
                return lst.Count() > 0;
            }
        }

        public  bool DM_LoHangExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_LoHang.Count(e => e.ID == id) > 0;
            }
        }

        public  bool DM_LoHangExists(string LoHang)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_LoHang.Count(e => e.MaLoHang.Trim().ToLower() == LoHang.Trim().ToLower()) > 0;
            }
        }

        public  bool DM_LoHangExists(string LoHang, Guid idHangHoa)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_LoHang.Count(e => e.ID_HangHoa == idHangHoa && e.MaLoHang.Trim().ToLower() == LoHang.Trim().ToLower()) > 0;
            }
        }
        public List<DM_LoHang> Select_LoHangs_IDHangHoa(Guid idHangHoa)
        {
            IQueryable<DM_LoHang> lsts = Gets(p => p.ID_HangHoa == idHangHoa);
            if (lsts != null)
                return lsts.OrderBy(p => p.MaLoHang).ToList();
            else
                return null;
        }

        public  List<DM_LoHangDTO> getlistLoHang(Guid idhanghoa)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from lohang in db.DM_LoHang
                          where lohang.ID_HangHoa == idhanghoa
                          select new DM_LoHangDTO
                          {
                              ID = lohang.ID,
                              ID_HangHoa = lohang.ID_HangHoa,
                              MaLoHang = lohang.MaLoHang,
                              NgaySanXuat = lohang.NgaySanXuat,
                              NgayHetHan = lohang.NgayHetHan,
                              TenLoHang = lohang.TenLoHang,
                          };
                return tbl.ToList();
            }
        }

        public  IQueryable<DM_LoHang> Gets(Expression<Func<DM_LoHang, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                IQueryable<DM_LoHang> values = db.DM_LoHang.Where(query);
                return values;
            }
        }
        public  DM_LoHang Get(Expression<Func<DM_LoHang, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_LoHang.Where(query).FirstOrDefault();
            }
        }

        #endregion

        #region insert
        public  string Add_LoHang(DM_LoHang objAdd)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.DM_LoHang.Add(objAdd);
                    db.SaveChanges();
                    return null;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public string Inserts(List<DM_LoHang> lst)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.DM_LoHang.AddRange(lst);
                    db.SaveChanges();
                    return null;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        #endregion

        #region update
        public  string Update_LoHang(DM_LoHang objNew)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    DM_LoHang objUpd = db.DM_LoHang.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        //objUpd.ID_HangHoa = objNew.ID_HangHoa;
                        objUpd.MaLoHang = objNew.MaLoHang;
                        //objUpd.TenLoHang = objNew.TenLoHang;
                        objUpd.NgayHetHan = objNew.NgayHetHan;
                        objUpd.NgaySanXuat = objNew.NgaySanXuat;

                        objUpd.NgaySua = DateTime.Now;
                        //objUpd.NgayTao = objNew.NgayTao;
                        objUpd.NguoiSua = objNew.NguoiSua;
                        //objUpd.NguoiTao = objNew.NguoiTao;
                        //
                        objUpd.TrangThai = objNew.TrangThai;
                        db.Entry(objUpd).State = EntityState.Modified;
                        //
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

        #region delete
        public string CheckDelete_LoHang(DM_LoHang objDel)
        {
            string strCheck = string.Empty;
            if (objDel != null)
            {
                if (objDel.BH_HoaDon_ChiTiet != null && objDel.BH_HoaDon_ChiTiet.Count > 0)
                {
                    strCheck = "Lô hàng này đã được sử dụng để lập 'Hóa đơn'. Không thể xóa.";
                    return strCheck;
                }
                if (objDel.Kho_TonKhoKhoiTao != null && objDel.Kho_TonKhoKhoiTao.Count > 0)
                {
                    strCheck = "Lô hàng này đã được sử dụng để khai báo 'Tồn kho khởi tạo' của hàng hóa. Không thể xóa.";
                    return strCheck;
                }
            }
            return strCheck;
        }

        public  string Delete_LoHang(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                DM_LoHang objDel = db.DM_LoHang.Find(id);
                if (objDel != null)
                {
                    string strCheckDel = CheckDelete_LoHang(objDel);
                    if (strCheckDel == null || strCheckDel == string.Empty)
                    {
                        try
                        {
                            db.DM_LoHang.Remove(objDel);
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

        public string Delete_LoHangs_IDHangHoa(Guid idHangHoa)
        {
            string strDel = string.Empty;
            List<DM_LoHang> lstLoHangs_Del = Select_LoHangs_IDHangHoa(idHangHoa);
            if (lstLoHangs_Del != null && lstLoHangs_Del.Count > 0)
            {
                foreach (DM_LoHang item in lstLoHangs_Del)
                {
                    string strDel_Item = Delete_LoHang(item.ID);
                    if (strDel_Item != null && strDel_Item != string.Empty && strDel_Item.Trim() != "")
                    {
                        return strDel_Item;
                    }
                }
            }
            return strDel;
        }
        #endregion
    }

    public class DM_LoHangDTO
    {
        public Guid ID { get; set; }

        public Guid ID_HangHoa { get; set; }

        public string MaLoHang { get; set; }

        public DateTime? NgaySanXuat { get; set; }

        public DateTime? NgayHetHan { get; set; }

        public string NguoiTao { get; set; }

        public DateTime? NgayTao { get; set; }

        public string NguoiSua { get; set; }

        public DateTime? NgaySua { get; set; }

        public string TenLoHang { get; set; }

        public bool? TrangThai { get; set; }
    }
}
