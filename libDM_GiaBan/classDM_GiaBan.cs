using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.SqlClient;

namespace libDM_GiaBan
{
    public class classDM_GiaBan
    {
        private SsoftvnContext db;

        public classDM_GiaBan(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public List<DM_GiaBan> Gets(Expression<Func<DM_GiaBan, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_GiaBan.ToList();
                else
                    return db.DM_GiaBan.Where(query).ToList();
            }
        }

        public List<DM_GiaBan_ApDung> GetsGBAD(Expression<Func<DM_GiaBan_ApDung, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_GiaBan_ApDung.ToList();
                else
                    return db.DM_GiaBan_ApDung.Where(query).ToList();
            }
        }

        public DM_GiaBan Select_GiaBan(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_GiaBan.Find(id);
            }
        }

        public List<DM_GiaBanDTO> selectallGiaBanAD(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from gb in db.DM_GiaBan
                          join gbad in db.DM_GiaBan_ApDung on gb.ID equals gbad.ID_GiaBan
                          into GB_AP
                          from gb_ad in GB_AP.DefaultIfEmpty()
                          where gb.ID == id
                          select new
                          {
                              ID = gb.ID,
                              TenGiaBan = gb.TenGiaBan,
                              TuNgay = gb.TuNgay,
                              DenNgay = gb.DenNgay,
                              ApDung = gb.ApDung,
                              GhiChu = gb.GhiChu,
                              TatCaDoiTuong = gb.TatCaDoiTuong,
                              TatCaDonVi = gb.TatCaDonVi,
                              TatCaNhanVien = gb.TatCaNhanVien,
                              ID_GiaBanAD = gb.ID,
                              ID_GiaBan = gb.ID,
                              NgayTrongTuan = gb.NgayTrongTuan,
                              LoaiChungTuApDung = gb.LoaiChungTuApDung
                              //ID_DonVi = gbad.ID_DonVi,
                              //ID_NhanVien = gbad.ID_NhanVien,
                              //ID_NhomKhachHang = gbad.ID_NhomKhachHang
                          };
                List<DM_GiaBanDTO> lst = new List<DM_GiaBanDTO>();
                foreach (var item in tbl)
                {
                    DM_GiaBanDTO dM_GiaBanDTO = new DM_GiaBanDTO();
                    dM_GiaBanDTO.ID = item.ID; // ID_dvquydoi
                    dM_GiaBanDTO.TenGiaBan = item.TenGiaBan;
                    dM_GiaBanDTO.NgayTrongTuan = item.NgayTrongTuan;
                    dM_GiaBanDTO.LoaiChungTuApDung = item.LoaiChungTuApDung;
                    dM_GiaBanDTO.TuNgay = item.TuNgay;
                    dM_GiaBanDTO.DenNgay = item.DenNgay;
                    dM_GiaBanDTO.ApDung = item.ApDung;
                    dM_GiaBanDTO.GhiChu = item.GhiChu; // 1* DonGia
                    dM_GiaBanDTO.TatCaDoiTuong = item.TatCaDoiTuong;
                    dM_GiaBanDTO.TatCaDonVi = item.TatCaDonVi;
                    dM_GiaBanDTO.TatCaNhanVien = item.TatCaNhanVien;
                    dM_GiaBanDTO.ID_GiaBanAD = item.ID_GiaBanAD;
                    dM_GiaBanDTO.ID_GiaBan = item.ID_GiaBan;
                    //dM_GiaBanDTO.ID_DonVi = item.ID_DonVi;
                    //dM_GiaBanDTO.ID_NhanVien = item.ID_NhanVien;
                    //dM_GiaBanDTO.ID_NhomKhachHang = item.ID_NhomKhachHang;

                    lst.Add(dM_GiaBanDTO);
                }
                if (lst.Count > 0)
                    return lst;
                else
                    return null;
            }
        }

        public List<DM_DonVi> getLisDonViGB(Guid id_giaban)
        {
            var tb = from kmap in db.DM_GiaBan_ApDung
                     join dv in db.DM_DonVi on kmap.ID_DonVi equals dv.ID
                     where kmap.ID_GiaBan == id_giaban
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

        public List<NS_NhanVien> getlistNhanVienBG(Guid id_giaban)
        {
            var tb = from kmap in db.DM_GiaBan_ApDung
                     join nv in db.NS_NhanVien on kmap.ID_NhanVien equals nv.ID
                     where kmap.ID_GiaBan == id_giaban
                     group kmap by new
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

        public List<DM_NhomDoiTuong> getlistNhomKHangBG(Guid id_giaban)
        {
            var tb = from kmap in db.DM_GiaBan_ApDung
                     join nh in db.DM_NhomDoiTuong on kmap.ID_NhomKhachHang equals nh.ID
                     where kmap.ID_GiaBan == id_giaban
                     group kmap by new
                     {
                         ID = nh.ID,
                         TenNhomDoiTuong = nh.TenNhomDoiTuong
                     } into g
                     select new
                     {
                         ID = g.Key.ID,
                         TenNhomDoiTuong = g.Key.TenNhomDoiTuong
                     };
            List<DM_NhomDoiTuong> lst = new List<DM_NhomDoiTuong>();
            foreach (var item in tb)
            {
                DM_NhomDoiTuong DM = new DM_NhomDoiTuong();
                DM.ID = item.ID;
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

        public DM_GiaBan Select_GiaBan(string TenGiaBan, Guid? id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (id == null)
                {
                    return db.DM_GiaBan.Where(p => p.TenGiaBan == TenGiaBan).FirstOrDefault();
                }
                else
                {
                    return db.DM_GiaBan.Where(p => p.ID != id).Where(p => p.TenGiaBan == TenGiaBan).FirstOrDefault();
                }
            }
        }

        public DM_GiaBan Get(Expression<Func<DM_GiaBan, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_GiaBan.Where(query).FirstOrDefault();
            }
        }
        #endregion

        #region add update
        public string AddDM_GiaBan(DM_GiaBan dmgiaban)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                strErr = "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.DM_GiaBan.Add(dmgiaban);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        public string UpdateDM_GiaBan(DM_GiaBan dmgiaban)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                strErr = "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    DM_GiaBan update = db.DM_GiaBan.Find(dmgiaban.ID);
                    update.TenGiaBan = dmgiaban.TenGiaBan;
                    update.ApDung = dmgiaban.ApDung;
                    update.TuNgay = dmgiaban.TuNgay;
                    update.DenNgay = dmgiaban.DenNgay;
                    update.TatCaDoiTuong = dmgiaban.TatCaDoiTuong;
                    update.TatCaDonVi = dmgiaban.TatCaDonVi;
                    update.TatCaNhanVien = dmgiaban.TatCaNhanVien;
                    update.NguoiSua = dmgiaban.NguoiSua;
                    update.NgaySua = dmgiaban.NgaySua;
                    update.GhiChu = dmgiaban.GhiChu;
                    update.NgayTrongTuan = dmgiaban.NgayTrongTuan;
                    update.LoaiChungTuApDung = dmgiaban.LoaiChungTuApDung;
                    db.Entry(update).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return strErr = ex.Message;
                }
            }
            return strErr;
        }
        #endregion

        #region
        public string Delete_GiaBan(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_GiaBan", id));
                List<DM_HangHoaDTO> listTon = db.Database.SqlQuery<DM_HangHoaDTO>("exec XoaBangGia @ID_GiaBan", paramlist.ToArray()).ToList();

            }
            return strErr;
        }

        public string DeleteAChiTietGiaBan(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                List<DM_GiaBan_ChiTiet> objDel = db.DM_GiaBan_ChiTiet.Where(p => p.ID == id).ToList();
                if (objDel != null)
                {
                    try
                    {
                        db.DM_GiaBan_ChiTiet.RemoveRange(objDel);
                        db.SaveChanges();
                    }
                    catch (Exception exxx)
                    {
                        strErr = exxx.Message + exxx.InnerException;
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

        #endregion
    }

    public class DM_GiaBanDTO
    {
        public Guid ID { get; set; }
        public string TenGiaBan { get; set; }
        public bool ApDung { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string GhiChu { get; set; }
        public string NguoiTao { get; set; }
        public string NgayTrongTuan { get; set; }
        public string LoaiChungTuApDung { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
        public bool TatCaDoiTuong { get; set; }
        public bool TatCaDonVi { get; set; }
        public bool TatCaNhanVien { get; set; }
        public Guid ID_GiaBanAD { get; set; }
        public Guid ID_GiaBan { get; set; }
        public Guid? ID_DonVi { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public Guid? ID_NhomKhachHang { get; set; }
    }
}
