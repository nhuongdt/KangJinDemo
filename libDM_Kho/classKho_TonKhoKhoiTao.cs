using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model;
using System.Data.Entity;
using System.Linq.Expressions;
using libDonViQuiDoi;

namespace libDM_Kho
{
    public class classKho_TonKhoKhoiTao
    {
        private SsoftvnContext db;

        public classKho_TonKhoKhoiTao(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public  Kho_TonKhoKhoiTao Select_TonKhoKhoiTao(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.Kho_TonKhoKhoiTao.Find(id);
            }
        }

        public  bool Kho_TonKhoKhoiTaoExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.Kho_TonKhoKhoiTao.Count(e => e.ID == id) > 0;
            }
        }
        public  bool Kho_TonKhoKhoiTaoExists(Guid? idDonVi, Guid iddonviquidoi, Guid idKho, Guid? idLo, int nam)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.Kho_TonKhoKhoiTao.Count(e => e.ID_DonVi == idDonVi && e.ID_DonViQuiDoi == iddonviquidoi && e.ID_Kho == idKho && e.ID_LoHang == idLo && e.NamHachToan == nam) > 0;
            }
        }

        public  Kho_TonKhoKhoiTao Select_TonKhoKhoiTao(Guid? idDonVi, Guid iddonviquidoi, Guid idKho, Guid? idLo, int nam)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.Kho_TonKhoKhoiTao.Where(e => e.ID_DonVi == idDonVi && e.ID_DonViQuiDoi == iddonviquidoi && e.ID_Kho == idKho && e.ID_LoHang == idLo && e.NamHachToan == nam).FirstOrDefault();
            }
        }
        public  List<Kho_TonKhoKhoiTao> Select_TonKhoKhoiTaos_IDHangHoa(Guid idHangHoa)
        {
            classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
            List<Guid> lstiddonviquidoi = _classDVQD.Select_DonViQuiDois_IDHangHoa(idHangHoa).Select(p => p.ID).ToList();
            IQueryable<Kho_TonKhoKhoiTao> lsts = Gets(p => lstiddonviquidoi.Contains(p.ID_DonViQuiDoi));
            if (lsts != null)
                return lsts.OrderBy(p => p.NamHachToan).ToList();
            else
                return null;
        }

        public  IQueryable<Kho_TonKhoKhoiTao> Gets(Expression<Func<Kho_TonKhoKhoiTao, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                IQueryable<Kho_TonKhoKhoiTao> values = db.Kho_TonKhoKhoiTao.Where(query);
                return values;
            }
        }
        public  Kho_TonKhoKhoiTao Get(Expression<Func<Kho_TonKhoKhoiTao, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.Kho_TonKhoKhoiTao.Where(query).FirstOrDefault();
            }
        }

        #endregion

        #region insert
        public  string Add_TonKhoKhoiTao(Kho_TonKhoKhoiTao objAdd)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.Kho_TonKhoKhoiTao.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return ex.Message +ex.InnerException;
                }
            }
            return string.Empty;
        }
        #endregion

        #region update
        public  string Update_TonKhoKhoiTao(Kho_TonKhoKhoiTao objNew)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    Kho_TonKhoKhoiTao objUpd = db.Kho_TonKhoKhoiTao.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.DonGia = objNew.DonGia;
                        objUpd.ID_DonVi = objNew.ID_DonVi;
                        objUpd.ID_Kho = objNew.ID_Kho;
                        objUpd.ID_LoHang = objNew.ID_LoHang;
                        objUpd.NamHachToan = objNew.NamHachToan;
                        objUpd.NgayChungTu = objNew.NgayChungTu;
                        objUpd.SoLuong = objNew.SoLuong;
                        objUpd.ThanhTien = objNew.ThanhTien;

                        objUpd.NgaySua = objNew.NgaySua;
                        objUpd.NgayTao = objNew.NgayTao;
                        objUpd.NguoiSua = objNew.NguoiSua;
                        objUpd.NguoiTao = objNew.NguoiTao;
                        //
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
        public  string Delete_TonKhoKhoiTao(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                Kho_TonKhoKhoiTao objDel = db.Kho_TonKhoKhoiTao.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        db.Kho_TonKhoKhoiTao.Remove(objDel);
                        db.SaveChanges();
                    }
                    catch (Exception exxx)
                    {
                        return exxx.Message;
                    }
                }
            }
            return string.Empty;
        }

        public  string Delete_TonKhoKhoiTaos_IDHangHoa(Guid idHangHoa)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db == null)
            {
                return null;
            }
            else
            {
                List<Guid> lstiddonviquidoi = db.DonViQuiDois.Where(p => p.ID_HangHoa == idHangHoa).Select(p => p.ID).ToList();
                if (lstiddonviquidoi != null && lstiddonviquidoi.Count > 0)
                {
                    IQueryable<Kho_TonKhoKhoiTao> lstTonKhoKhoiTaos_Del = db.Kho_TonKhoKhoiTao.Where(p => lstiddonviquidoi.Contains(p.ID_DonViQuiDoi));
                    if (lstTonKhoKhoiTaos_Del != null && lstTonKhoKhoiTaos_Del.Count() > 0)
                    {
                        try
                        {
                            db.Kho_TonKhoKhoiTao.RemoveRange(lstTonKhoKhoiTaos_Del);
                            db.SaveChanges();
                        }
                        catch (Exception exxx)
                        {
                            return exxx.Message;
                        }
                    }
                }
            }
            return string.Empty;
        }
        #endregion
    }
}
