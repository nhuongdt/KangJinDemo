using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Entity;
using System.Linq.Expressions;

namespace libDM_NhomHangHoa
{
    public class classDM_NhomHangHoa
    {
        private SsoftvnContext db;

        public classDM_NhomHangHoa(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public DM_NhomHangHoa Select_NhomHangHoa(Guid? id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_NhomHangHoa.Find(id);
            }
        }

        public bool DM_NhomHangHoaExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_NhomHangHoa.Count(e => e.ID == id) > 0;
            }
        }
        public List<DM_NhomHangHoa> Gets(Expression<Func<DM_NhomHangHoa, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query != null)
                {
                    return db.DM_NhomHangHoa.Where(query).ToList();
                }
                else
                {
                    return db.DM_NhomHangHoa.ToList();
                }
            }
        }

        public List<DM_NhomHangHoaSelect> GetAll()
        {
            List<DM_NhomHangHoaSelect> list = new List<DM_NhomHangHoaSelect>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from nHH in db.DM_NhomHangHoa
                          join nHHParent in db.DM_NhomHangHoa on nHH.ID equals nHHParent.ID_Parent
                          into child_parent
                          from c_p in child_parent.DefaultIfEmpty()
                          select new DM_NhomHangHoaSelect
                          {
                              ID = nHH.ID,
                              TenNhomHangHoa = nHH.TenNhomHangHoa,
                              ID_Parent = c_p.ID,
                              TenNhomCon = c_p.ID_Parent == Guid.Empty ? "" : c_p.TenNhomHangHoa,
                              //TenNhomCha = /*classDM_NhomHangHoa.Select_NhomHangHoa(nHH.ID_Parent) == null ? "" : */classDM_NhomHangHoa.Select_NhomHangHoa(nHH.ID_Parent).TenNhomHangHoa,
                          };

                foreach (DM_NhomHangHoaSelect item in tbl)
                {
                    DM_NhomHangHoaSelect temp = new DM_NhomHangHoaSelect();
                    temp.ID = item.ID;
                    temp.TenNhomHangHoa = item.TenNhomHangHoa;
                    temp.ID_Parent = item.ID_Parent;
                    temp.TenNhomCon = item.TenNhomCon;
                    list.Add(temp);
                }
                return list;
            }
        }

        public DM_NhomHangHoa Get(Expression<Func<DM_NhomHangHoa, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_NhomHangHoa.Where(query).FirstOrDefault();
            }
        }

        public List<DM_NhomHangHoa> getlistNhomHHByTenNhom(string TenNhomHang)
        {
            List<DM_NhomHangHoa> lst = new List<DM_NhomHangHoa>();
            if (TenNhomHang != null & TenNhomHang != "" & TenNhomHang != "null")
            {

                TenNhomHang = CommonStatic.ConvertToUnSign(TenNhomHang).ToLower();
                lst = db.DM_NhomHangHoa.Where(p => p.TrangThai != true).Where(p => p.TenNhomHangHoa.Contains(TenNhomHang) || p.TenNhomHangHoa_KhongDau.Contains(TenNhomHang) || p.TenNhomHangHoa_KyTuDau.Contains(TenNhomHang)).ToList();
                //tbl1 = tbl1.Where(x => x.TenNhomHangHoa_CV.Contains(@TenNhomHang) || x.TenNhomHangHoa_GC.Contains(@TenNhomHang));
            }
            else
            {
                lst = db.DM_NhomHangHoa.ToList();
            }
            return lst;
        }

        #endregion

        #region insert
        public string Add_NhomHangHoa(DM_NhomHangHoa objAdd)
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
                    db.DM_NhomHangHoa.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }
        #endregion

        #region update
        public string Update_NhomHangHoa(DM_NhomHangHoa objNew)
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
                    DM_NhomHangHoa objUpd = db.DM_NhomHangHoa.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.GhiChu = objNew.GhiChu;
                        objUpd.HienThi_BanThe = objNew.HienThi_BanThe;
                        objUpd.HienThi_Chinh = objNew.HienThi_Chinh;
                        objUpd.HienThi_Phu = objNew.HienThi_Phu;
                        objUpd.ID_Kho = objNew.ID_Kho;
                        objUpd.ID_Parent = objNew.ID_Parent;
                        objUpd.LaNhomHangHoa = objNew.LaNhomHangHoa;
                        //objUpd.MaNhomHangHoa = objNew.MaNhomHangHoa;
                        objUpd.MauHienThi = objNew.MauHienThi;
                        objUpd.MayIn = objNew.MayIn;
                        objUpd.TenNhomHangHoa = objNew.TenNhomHangHoa;
                        objUpd.TenNhomHangHoa_KhongDau = objNew.TenNhomHangHoa_KhongDau;
                        objUpd.TenNhomHangHoa_KyTuDau = objNew.TenNhomHangHoa_KyTuDau;
                        objUpd.NgaySua = objNew.NgaySua;
                        objUpd.NguoiSua = objNew.NguoiSua;
                        objUpd.TrangThai = objNew.TrangThai;
                        //
                        db.Entry(objUpd).State = EntityState.Modified;
                        #endregion

                      
                        #region NhomHangHoa_DonVi
                        if (objNew.NhomHangHoa_DonVi != null && objNew.NhomHangHoa_DonVi.Count > 0)
                        {
                            List<NhomHangHoa_DonVi> lstDonVis = objNew.NhomHangHoa_DonVi.ToList();

                            List<Guid> previousIds = db.NhomHangHoa_DonVi.Where(ep => ep.ID_NhomHangHoa == objNew.ID).Select(ep => ep.ID).ToList();
                            List<Guid> currentIds = lstDonVis.Select(o => o.ID).ToList();
                            List<Guid> deletedIds = previousIds.Except(currentIds).ToList();
                            foreach (var del_Id in deletedIds)
                            {
                                NhomHangHoa_DonVi deletedOrderDetail = db.NhomHangHoa_DonVi.Where(od => od.ID_NhomHangHoa == objNew.ID && od.ID == del_Id).Single();
                                db.Entry(deletedOrderDetail).State = EntityState.Deleted;
                            }
                            foreach (var orderDetail in lstDonVis)
                            {
                                if (previousIds.Contains(orderDetail.ID) && currentIds.Contains(orderDetail.ID))
                                {
                                    NhomHangHoa_DonVi objUpd_DVi = db.NhomHangHoa_DonVi.Where(od => od.ID_NhomHangHoa == objNew.ID && od.ID == orderDetail.ID).Single();
                                    if (objUpd_DVi != null)
                                    {
                                        objUpd_DVi.ID_DonVi = orderDetail.ID_DonVi;
                                        //
                                        db.Entry(objUpd_DVi).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        objUpd_DVi = new NhomHangHoa_DonVi();
                                        objUpd_DVi.ID = Guid.NewGuid();
                                        objUpd_DVi.ID_DonVi = orderDetail.ID_DonVi;
                                        objUpd_DVi.ID_NhomHangHoa = objUpd.ID;
                                        //
                                        db.Entry(orderDetail).State = EntityState.Added;
                                    }
                                }
                                else if (!previousIds.Contains(orderDetail.ID) && currentIds.Contains(orderDetail.ID))
                                {
                                    NhomHangHoa_DonVi objUpd_DVi = new NhomHangHoa_DonVi();
                                    objUpd_DVi.ID = Guid.NewGuid();
                                    objUpd_DVi.ID_DonVi = orderDetail.ID_DonVi;
                                    objUpd_DVi.ID_NhomHangHoa = objUpd.ID;
                                    //
                                    db.Entry(orderDetail).State = EntityState.Added;
                                }
                            }
                        }
                        else
                        {
                            List<NhomHangHoa_DonVi> lstDonVis = db.NhomHangHoa_DonVi.Where(p => p.ID_NhomHangHoa == objNew.ID).ToList();
                            if (lstDonVis != null && lstDonVis.Count > 0)
                                db.NhomHangHoa_DonVi.RemoveRange(lstDonVis);
                        }
                        #endregion
                        db.SaveChanges();
                    }
                    else
                    {
                        #region insert new
                        return "Không tìm thấy dữ liệu cần cập nhật trên hệ thống";
                        #endregion
                    }
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
        public string CheckDelete_NhomHangHoa(DM_NhomHangHoa objDel)
        {
            string strCheck = string.Empty;
            if (objDel != null)
            {
                if (objDel.DM_HangHoa != null && objDel.DM_HangHoa.Count > 0)
                {
                    strCheck = "Nhóm hàng hóa này đã được sử dụng để khai báo danh mục hàng hóa. Không thể xóa.";
                    return strCheck;
                }
                else if (objDel.DM_NhomHangHoa1 != null && objDel.DM_NhomHangHoa1.Count > 0)
                {
                    strCheck = "Nhóm hàng hóa này đã được sử dụng để khai báo danh mục nhóm hàng hóa cấp dưới. Không thể xóa.";
                    return strCheck;
                }
            }
            return strCheck;
        }

        public bool Check_TenNhomHangHoaExist(string tenNhomHang)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_NhomHangHoa.Count(e => e.TenNhomHangHoa == tenNhomHang && e.TrangThai != true) > 0;
            }
        }

        public bool Check_TenNhomHangHoaExistEdit(string tenNhomHang, Guid idnhomhh)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_NhomHangHoa.Count(e => e.TenNhomHangHoa == tenNhomHang && e.TrangThai != true && e.ID != idnhomhh) > 0;
            }
        }

        public string Delete_NhomHangHoa(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                DM_NhomHangHoa objDel = db.DM_NhomHangHoa.Find(id);
                if (objDel != null)
                {
                    string strCheckDel = CheckDelete_NhomHangHoa(objDel);
                    if (strCheckDel == null || strCheckDel == string.Empty || strCheckDel.Trim() == "")
                    {
                        try
                        {
                            //ChietKhauMacDinh_NhanVien
                            //if (objDel.ChietKhauMacDinh_NhanVien != null && objDel.ChietKhauMacDinh_NhanVien.Count > 0)
                            //    db.ChietKhauMacDinh_NhanVien.RemoveRange(objDel.ChietKhauMacDinh_NhanVien.ToList());
                            //NhomHangHoa_DonVi
                            if (objDel.NhomHangHoa_DonVi != null && objDel.NhomHangHoa_DonVi.Count > 0)
                                db.NhomHangHoa_DonVi.RemoveRange(objDel.NhomHangHoa_DonVi.ToList());
                            //DM_NhomHangHoa
                            db.DM_NhomHangHoa.Remove(objDel);
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
                        strErr = strCheckDel;
                        return strCheckDel;
                    }
                }
            }
            return strErr;
        }
        #endregion 

        public List<Guid> GetListIDNhomHangByID(Guid? idphongban)
        {
            if (idphongban == null)
            {
                return db.DM_NhomHangHoa.Select(p => p.ID).ToList();
            }
            else
            {
                List<Guid> lstIDPhongBanTemp = new List<Guid>();
                List<Guid> lstIDPhongBan = new List<Guid>();
                lstIDPhongBanTemp.Add(idphongban.Value);
                lstIDPhongBan.Add(idphongban.Value);
                List<DM_NhomHangHoa> lstNSPhongBan = db.DM_NhomHangHoa.Where(p => p.TrangThai != true).ToList();
                int flag = 1;
                while (flag == 1)
                {
                    flag = lstNSPhongBan.Where(p => p.ID_Parent != null).Where(p => lstIDPhongBanTemp.Contains(p.ID_Parent.Value)).Count();
                    if (flag != 0)
                    {
                        lstIDPhongBanTemp.AddRange(lstNSPhongBan.Where(p => p.ID_Parent != null).Where(p => lstIDPhongBanTemp.Contains(p.ID_Parent.Value)).Select(p => p.ID));
                        lstIDPhongBanTemp = lstIDPhongBanTemp.Except(lstIDPhongBan).ToList();
                        lstIDPhongBan.AddRange(lstIDPhongBanTemp);
                    }
                }
                return lstIDPhongBan;
            }
        }
    }
    public class DM_NhomHangHoaSelect
    {
        public Guid ID { get; set; }
        public Guid? ID_Parent { get; set; }
        public string TenNhomCon { get; set; }
        public string TenNhomHangHoa { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool LaNhomHangHoa { get; set; }
        public bool? TrangThai { get; set; }
    }

    public class NhomHangHoaParent
    {
        // bat buoc phai khai bao nhu the nay --> su dung treeview
        public Guid id { get; set; }
        public Guid? ID_Parent { get; set; }
        public string text { get; set; }
        public bool LaNhomHangHoa { get; set; }
        public List<NhomHangHoaParent> children { get; set; }
    }
}
