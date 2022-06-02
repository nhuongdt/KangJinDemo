using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Linq.Expressions;
using System.Data.Entity;
using libDonViQuiDoi;
using libDM_HangHoa;
using System.Data.SqlClient;
using Model.Service.common;

namespace libDM_GiaBan
{
    public class classDM_GiaBan_ChiTiet
    {
        private SsoftvnContext db;

        public classDM_GiaBan_ChiTiet(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public List<DM_GiaBan_ChiTiet> Gets(Expression<Func<DM_GiaBan_ChiTiet, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_GiaBan_ChiTiet.ToList();
                else
                    return db.DM_GiaBan_ChiTiet.Where(query).ToList();
            }
        }

        /// <summary>
        /// get all row in DM_GiaBan_ChiTiet
        /// </summary>
        /// <returns></returns>

        public List<SP_GiaBanChiTietDTO> SP_GettAll_BangGiaChiTiet()
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    // chỉ muốn hiển thị dữ liệu và không xử lý chúng,không cần các proxy được tạo để giám sát các đối tượng đó bởi EF
                    db.Configuration.ProxyCreationEnabled = false;
                    return db.Database.SqlQuery<SP_GiaBanChiTietDTO>("EXEC SP_GettAll_BangGiaChiTiet").ToList();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_GettAll_BangGiaChiTiet " + ex.InnerException + ex.Message);
                    return null;
                }
            }
        }

        public IQueryable<Object> SelectAll_GBChiTiet()
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    var data = from ct in db.DM_GiaBan_ChiTiet
                               join qd in db.DonViQuiDois on ct.ID_DonViQuiDoi equals qd.ID
                               into CT_QD
                               from ct_qd in CT_QD.DefaultIfEmpty()
                               join hh in db.DM_HangHoa on ct_qd.ID_HangHoa equals hh.ID
                               where ct_qd.Xoa != true
                               select new
                               {
                                   ID = ct.ID,
                                   ID_DonViQuiDoi = ct.ID_DonViQuiDoi,
                                   ID_HangHoa = ct_qd.ID_HangHoa,
                                   ID_BangGia = ct.ID_GiaBan,
                                   GiaBan = ct.GiaBan,
                               };
                    return data;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SelectAll_GBChiTiet " + ex.InnerException + ex.Message);
                    return null;
                }
            }
        }

        public List<GiaBanChiTietDTO> SelectChiTiet(string _id)
        {

            if (db == null)
            {
                return null;
            }
            else
            {
                //GiaBanChiTietDTO chitiet = new GiaBanChiTietDTO();
                if (_id != "undefined")
                {
                    Guid id = new Guid(_id);
                    return db.DM_GiaBan_ChiTiet.Where(p => p.ID_GiaBan == id).GroupJoin(db.DonViQuiDois, ct => ct.ID_DonViQuiDoi, qd => qd.ID, (ct, qd) => new { ct, qd })
                        .SelectMany(s => s.qd.DefaultIfEmpty(), (s, qd) => new
                        {
                            ID = s.ct.ID,
                            ID_HangHoa = qd.ID_HangHoa,
                            DonViTinh = qd.TenDonViTinh,
                            GiaNhapCuoi = qd.GiaNhap,
                            GiaMoi = s.ct.GiaBan,
                            MaHangHoa = qd.MaHangHoa,
                            GiaVon = qd.GiaVon,
                            GiaChung = qd.GiaBan,
                            IDQuyDoi = qd.ID,
                            ID_GiaBan = s.ct.ID_GiaBan,
                            Xoa = qd.Xoa
                        }).GroupJoin(db.DM_HangHoa, s => s.ID_HangHoa, hh => hh.ID, (s, hh) => new { s, hh })
                        .SelectMany(p => p.hh.DefaultIfEmpty(), (p, hh) => new GiaBanChiTietDTO
                        {
                            ID = p.s.ID,
                            MaHangHoa = p.s.MaHangHoa,
                            TenHangHoa = hh.TenHangHoa,
                            DonViTinh = p.s.DonViTinh,
                            GiaVon = p.s.GiaVon,
                            GiaNhapCuoi = p.s.GiaNhapCuoi,
                            GiaMoi = p.s.GiaMoi,
                            GiaChung = p.s.GiaChung,
                            IDQuyDoi = p.s.IDQuyDoi,
                            ID_NhomHang = hh.ID_NhomHang,
                            ID_GiaBan = p.s.ID_GiaBan,
                            Xoa = p.s.Xoa
                        }).OrderBy(p => p.TenHangHoa).ToList();
                }
                else
                {
                    return db.DonViQuiDois.GroupJoin(db.DM_HangHoa, qd => qd.ID_HangHoa, hh => hh.ID, (qd, hh) => new { qd, hh })
                        .SelectMany(s => s.hh.DefaultIfEmpty(), (s, hh) => new GiaBanChiTietDTO
                        {
                            ID = s.qd.ID,
                            MaHangHoa = s.qd.MaHangHoa,
                            DonViTinh = s.qd.TenDonViTinh,
                            TenHangHoa = hh.TenHangHoa,
                            GiaVon = s.qd.GiaVon,
                            GiaNhapCuoi = s.qd.GiaNhap,
                            GiaChung = s.qd.GiaBan,
                            GiaMoi = s.qd.GiaBan,
                            IDQuyDoi = s.qd.ID,
                            ID_NhomHang = hh.ID_NhomHang,
                            ID_GiaBan = Guid.Empty,
                            Xoa = s.qd.Xoa
                        }).OrderBy(p => p.TenHangHoa).Where(p => p.Xoa != true).ToList();
                }
            }
        }

        public List<GiaBanChiTietDTO> SelectChiTiet_where(string _id, string maHoaDon, Guid iddonvi)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                string ID_BangGia = "";
                if (_id == "undefined")
                {
                    ID_BangGia = "";
                }
                else
                {
                    ID_BangGia = _id;
                }
                if (maHoaDon == null)
                {
                    maHoaDon = string.Empty;
                }
                char[] whitespace = new char[] { ' ', '\t' };
                string[] textFilter = maHoaDon.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Split(whitespace);
                string[] utf8 = textFilter.Where(o => o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                string[] utf = textFilter.Where(o => !o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                paramlist.Add(new SqlParameter("ID_BangGia", ID_BangGia));
                paramlist.Add(new SqlParameter("maHoaDon", string.Join(",", utf)));
                paramlist.Add(new SqlParameter("maHoaDonVie", string.Join(",", utf8)));
                List<GiaBanChiTietDTO> listGB = db.Database.SqlQuery<GiaBanChiTietDTO>("exec LoadGiaBanChiTiet @ID_ChiNhanh, @ID_BangGia,@maHoaDon,@maHoaDonVie", paramlist.ToArray()).ToList();
                return listGB;
            }
        }

        public List<GiaBanChiTietDTO> GetBangGia_NhomHang()
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from ctBG in db.DM_GiaBan_ChiTiet
                          join dvt in db.DonViQuiDois on ctBG.ID_DonViQuiDoi equals dvt.ID into BG_QD
                          from dvt in BG_QD.DefaultIfEmpty()
                          join hh in db.DM_HangHoa on dvt.ID_HangHoa equals hh.ID
                          join nhh in db.DM_NhomHangHoa on hh.ID_NhomHang equals nhh.ID
                          select new GiaBanChiTietDTO
                          {
                              ID = ctBG.ID,
                              IDQuyDoi = dvt.ID,
                              MaHangHoa = dvt.MaHangHoa,
                              TenHangHoa = hh.TenHangHoa,
                              GiaVon = dvt.GiaVon,
                              GiaBan = dvt.GiaBan,
                              GiaNhapCuoi = dvt.GiaNhap,
                              GiaChung = dvt.GiaBan,
                              GiaMoi = ctBG.GiaBan,
                              ID_GiaBan = ctBG.ID_GiaBan,
                              ID_NhomHang = hh.ID_NhomHang,
                              DonViTinh = dvt.TenDonViTinh
                          };
                if (tbl == null)
                {
                    return null;
                }
                else
                {
                    return tbl.ToList();
                }

            }
        }

        #endregion

        #region update
        public string AddChiTietByIDNhom(string idnhomhanghoa, Guid idgiaban)
        {
            if (db == null)
            {
                return "error";
            }
            else
            {
                try
                {
                    if (idnhomhanghoa != null)
                    {
                        List<SqlParameter> paramlist = new List<SqlParameter>();
                        paramlist.Add(new SqlParameter("ListID_NhomHang", idnhomhanghoa));
                        paramlist.Add(new SqlParameter("ID_GiaBan", idgiaban));
                        paramlist.Add(new SqlParameter("ID_KhoHang", new Guid("01CD02F2-4612-4104-B790-1C0373CBD72D")));
                        paramlist.Add(new SqlParameter("ID_NgoaiTe", new Guid("406eed2d-faae-4520-aef2-12912f83dda2")));
                        //db.Database.SqlQuery("exec PutGiaBanChiTietChungCongVND @LoaiGiaChon, @giaTri", paramlist.ToArray());
                        db.Database.ExecuteSqlCommand("exec AddChiTietGia @ListID_NhomHang, @ID_GiaBan, @ID_KhoHang, @ID_NgoaiTe", paramlist.ToArray());
                        return "";
                    }
                    else
                    {
                        return "";
                    }
                }
                catch
                {
                    return "error";
                }
            }
        }

        public bool AddChiTietByIDhang(Guid iddonviqd, Guid idgiaban)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                try
                {
                    var _classDVQD = new classDonViQuiDoi(db);
                    List<DM_GiaBan_ChiTiet> listiddvqd = db.DM_GiaBan_ChiTiet.Where(p => p.ID_GiaBan == idgiaban).ToList();
                    bool alreadyExists = listiddvqd.Any(x => x.ID_DonViQuiDoi == iddonviqd);
                    if (!alreadyExists)
                    {
                        DM_GiaBan_ChiTiet ct = new DM_GiaBan_ChiTiet();
                        ct.ID = Guid.NewGuid();
                        ct.ID_KhoHang = new Guid("01CD02F2-4612-4104-B790-1C0373CBD72D");
                        ct.ID_NgoaiTe = new Guid("406eed2d-faae-4520-aef2-12912f83dda2");
                        ct.GiaBan = _classDVQD.Get(id => id.ID == iddonviqd).GiaBan;
                        ct.ID_GiaBan = idgiaban;
                        ct.NgayNhap = DateTime.Now;
                        ct.ID_DonViQuiDoi = iddonviqd;
                        db.DM_GiaBan_ChiTiet.Add(ct);
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }


        public bool AddChiTietBangGiaTuNhapHang(Guid iddonviqd, Guid idgiaban, double? giaban)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                try
                {
                    DM_GiaBan_ChiTiet ct = new DM_GiaBan_ChiTiet();
                    ct.ID = Guid.NewGuid();
                    ct.ID_KhoHang = new Guid("01CD02F2-4612-4104-B790-1C0373CBD72D");
                    ct.ID_NgoaiTe = new Guid("406eed2d-faae-4520-aef2-12912f83dda2");
                    ct.GiaBan = giaban.Value;
                    ct.ID_GiaBan = idgiaban;
                    ct.NgayNhap = DateTime.Now;
                    ct.ID_DonViQuiDoi = iddonviqd;
                    db.DM_GiaBan_ChiTiet.Add(ct);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("AddChiTietGiaBan for nhaphang", ex.InnerException + ex.Message);
                    return false;
                }
            }
        }
        public string Update_GiaBanCT(Guid id, double giaban)
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
                    #region DM_GiaBan_ChiTiet
                    DM_GiaBan_ChiTiet objUpd = db.DM_GiaBan_ChiTiet.Find(id);
                    objUpd.GiaBan = giaban;
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("UpdateGiaBan CT for nhaphang", ex.InnerException + ex.Message);
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        public string Update_GiaBanCTChung(Guid id, double giaban)
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
                    #region DM_GiaBan_ChiTiet
                    DonViQuiDoi dvqdtmp = db.DonViQuiDois.Find(id);
                    dvqdtmp.GiaBan = giaban;
                    #endregion
                    db.Entry(dvqdtmp).State = EntityState.Modified;
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

        #region delete
        public bool deleteChiTietbyID(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                try
                {
                    DM_GiaBan_ChiTiet ct = db.DM_GiaBan_ChiTiet.Find(id);
                    db.DM_GiaBan_ChiTiet.Remove(ct);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        #endregion
    }


    public class GiaBanChiTietDTO
    {
        public Guid? ID { get; set; }
        public Guid? IDQuyDoi { get; set; }
        public Guid ID_HangHoa { get; set; }
        public Guid? ID_NhomHang { get; set; }
        public Guid? ID_GiaBan { get; set; }
        public string MaHangHoa { get; set; }
        public DateTime? NgayTao { get; set; }
        public string DonViTinh { get; set; }
        public string TenHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenNhomHangHoa { get; set; }
        public string GiaTri { get; set; }
        public double GiaVon { get; set; }
        public double GiaBan { get; set; }
        public double GiaNhapCuoi { get; set; }
        public double GiaChung { get; set; }
        public double GiaMoi { get; set; }
        public bool? Xoa { get; set; }
        public bool? QuanLyTheoLoHang { get; set; }
        public string TenHangHoaUnsign { get; set; }
        public string TenHangHoaCharStart { get; set; }
        public List<HangHoa_ThuocTinh> HangHoa_ThuocTinh { get; set; }
        public string HangHoaThuocTinh { get; set; }
    }
    public class BH_GiaBan_Excel
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenNhomHangHoa { get; set; }
        public double GiaVon { get; set; }
        public double GiaNhapCuoi { get; set; }
        public double? GiaChung { get; set; }
        public double GiaMoi { get; set; }
    }

    public class SP_GiaBanChiTietDTO
    {
        public Guid ID { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid ID_HangHoa { get; set; }
        public Guid ID_BangGia { get; set; }
        public double GiaBan { get; set; }
    }
}
