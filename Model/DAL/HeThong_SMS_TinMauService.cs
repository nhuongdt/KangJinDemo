using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAL
{
    public class HeThong_SMS_TinMauService
    {
        public static string InsertMauTin(HeThong_SMS_TinMau model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        db.HeThong_SMS_TinMau.Add(model);
                        db.SaveChanges();
                        if (model.LaMacDinh == true)
                        {
                            List<HeThong_SMS_TinMau> lst = db.HeThong_SMS_TinMau.Where(p => p.ID != model.ID && p.LoaiTin == model.LoaiTin).ToList();
                            foreach (var item in lst)
                            {
                                item.LaMacDinh = false;
                                db.Entry(item).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            List<HeThong_SMS_TinMau> lst = db.HeThong_SMS_TinMau.Where(p => p.LaMacDinh == true && p.ID != model.ID && p.LoaiTin == model.LoaiTin).ToList();
                            if (lst.Count() > 1)
                            {
                                foreach (var item in lst)
                                {
                                    item.LaMacDinh = false;
                                    db.Entry(item).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                HeThong_SMS_TinMau mautin1 = db.HeThong_SMS_TinMau.Where(p => p.ID != model.ID && p.LoaiTin == model.LoaiTin).OrderByDescending(p => p.NgayTao).FirstOrDefault();
                                mautin1.LaMacDinh = true;
                                db.Entry(mautin1).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            if (lst.Count() < 1)
                            {
                                HeThong_SMS_TinMau mautin1 = db.HeThong_SMS_TinMau.Where(p => p.ID != model.ID && p.LoaiTin == model.LoaiTin).OrderByDescending(p => p.NgayTao).FirstOrDefault();
                                mautin1.LaMacDinh = true;
                                db.Entry(mautin1).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
                return "";
            }
        }

        public static string UpdateTinMau(HeThong_SMS_TinMau model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                HeThong_SMS_TinMau mautin = GetMauTinByID(model.ID);
                mautin.NoiDung = model.NoiDung;
                mautin.LoaiTin = model.LoaiTin;
                mautin.LaMacDinh = model.LaMacDinh;
                db.Entry(mautin).State = EntityState.Modified;
                db.SaveChanges();

                if (mautin.LaMacDinh == true)
                {
                    List<HeThong_SMS_TinMau> lst = db.HeThong_SMS_TinMau.Where(p => p.ID != model.ID && p.LoaiTin == model.LoaiTin).ToList();
                    foreach (var item in lst)
                    {
                        item.LaMacDinh = false;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    List<HeThong_SMS_TinMau> lst = db.HeThong_SMS_TinMau.Where(p => p.LaMacDinh == true && p.ID != model.ID && p.LoaiTin == model.LoaiTin).ToList();
                    if (lst.Count() > 1)
                    {
                        foreach (var item in lst)
                        {
                            item.LaMacDinh = false;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    HeThong_SMS_TinMau mautin1 = db.HeThong_SMS_TinMau.Where(p => p.ID != model.ID && p.LoaiTin == model.LoaiTin).OrderByDescending(p => p.NgayTao).FirstOrDefault();
                    if (mautin1 != null)
                    {
                        mautin1.LaMacDinh = true;
                        db.Entry(mautin1).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return string.Empty;
            }
        }

        public static void DeleteMauTin(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                HeThong_SMS_TinMau objDel = db.HeThong_SMS_TinMau.Find(id);
                db.HeThong_SMS_TinMau.Remove(objDel);
                db.SaveChanges();
            }
        }

        public static HeThong_SMS_TinMau GetMauTinByID(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                return db.HeThong_SMS_TinMau.Where(o => o.ID == id).FirstOrDefault();
            }
        }
        public static List<HeThong_SMS_TinMauDTO> GetAllMauTin()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var data = from o in db.HeThong_SMS_TinMau
                           orderby o.NgayTao descending
                           select new HeThong_SMS_TinMauDTO()
                           {
                               ID = o.ID,
                               NoiDungTin = o.NoiDung,
                               LoaiTin = o.LoaiTin,
                               TenLoaiTin = (
                               o.LoaiTin == 1 ? "Giao dịch" :
                               o.LoaiTin == 2 ? "Sinh nhật" :
                               o.LoaiTin == 3 ? "Tin thường" :
                               o.LoaiTin == 4 ? "Lịch hẹn" :
                               o.LoaiTin == 5 ? "Nhắc bảo dưỡng" : string.Empty
                               ),
                               NgayTao = o.NgayTao
                           };
                return data.ToList();
            }
        }

        //GetallTin Nhắn gửi
        public static List<HeThong_SMSDTO> GetAllTinGui()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var data = from o in db.HeThong_SMS
                           join dt in db.DM_DoiTuong on o.ID_KhachHang equals dt.ID into DoiTuongDM
                           from dt_tin in DoiTuongDM.DefaultIfEmpty()
                           join dv in db.DM_DonVi on o.ID_DonVi equals dv.ID
                           join nd in db.HT_NguoiDung on o.ID_NguoiGui equals nd.ID
                           orderby o.ThoiGianGui descending
                           select new HeThong_SMSDTO
                           {
                               ID = o.ID,
                               TenNguoiGui = nd.TaiKhoan,
                               TenKhachHang = dt_tin != null ? dt_tin.TenDoiTuong : "",
                               SoDienThoai = o.SoDienThoai,
                               ThoiGianGui = o.ThoiGianGui,
                               TrangThai = o.TrangThai,
                               NoiDung = o.NoiDung,
                               LoaiTinNhan = o.LoaiTinNhan
                           };
                return data.ToList();
            }
        }

        public static List<HeThong_SMSDTO> GetListSMSSend(DateTime? from, DateTime? to, int? status, int? typeSMS)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var data = from o in db.HeThong_SMS
                           join dt in db.DM_DoiTuong on o.ID_KhachHang equals dt.ID
                           join nd in db.HT_NguoiDung on o.ID_NguoiGui equals nd.ID
                           select new HeThong_SMSDTO
                           {
                               ID = o.ID,
                               TenNguoiGui = nd.TaiKhoan,
                               TenKhachHang = dt.TenDoiTuong,
                               SoDienThoai = o.SoDienThoai,
                               ThoiGianGui = o.ThoiGianGui,
                               TrangThai = o.TrangThai,
                               NoiDung = o.NoiDung,
                               LoaiTinNhan = o.LoaiTinNhan
                           };
                if (from != null)
                {
                    data = data.Where(x => x.ThoiGianGui > from && x.ThoiGianGui < to);
                }
                switch (status)
                {
                    case 0: // all
                        break;
                    case 3:// thanhcong
                        data = data.Where(x => x.TrangThai == 100);
                        break;
                    case 4:// thatbai
                        data = data.Where(x => x.TrangThai != 100);
                        break;
                }
                switch (typeSMS)
                {
                    case 1: // giaodich
                        data = data.Where(x => x.LoaiTinNhan == 1);
                        break;
                    case 2:// sinhnhat
                        data = data.Where(x => x.LoaiTinNhan == 2);
                        break;
                    case 3:// tinthuong
                        data = data.Where(x => x.LoaiTinNhan == 3);
                        break;
                    case 4:// lichhen
                        data = data.Where(x => x.LoaiTinNhan == 4);
                        break;
                }
                return data.OrderByDescending(x => x.ThoiGianGui).ToList();
            }
        }

        //gửi tin nhắn
        public void InsertTinNhan(HeThong_SMS model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                DM_DoiTuong doituong = db.DM_DoiTuong.Where(p => p.DienThoai == model.SoDienThoai).FirstOrDefault();
                if (doituong != null)
                {
                    model.ID_KhachHang = doituong.ID;
                }
                db.HeThong_SMS.Add(model);
                db.SaveChanges();
            }
        }
    }

    public class HeThong_SMS_TinMauDTO
    {
        public Guid ID { get; set; }
        public string NoiDungTin { get; set; }
        public int LoaiTin { get; set; }
        public string TenLoaiTin { get; set; }
        public DateTime NgayTao { get; set; }
        public bool LaMacDinh { get; set; }
    }

    public class HeThong_SMSDTO
    {
        public Guid ID { get; set; }
        public string TenNguoiGui { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string NoiDung { get; set; }
        public DateTime ThoiGianGui { get; set; }
        public int TrangThai { get; set; }
        public int LoaiTinNhan { get; set; }
    }
}
