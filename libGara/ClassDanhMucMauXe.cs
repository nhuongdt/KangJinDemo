using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;

namespace libGara
{
    public class ClassDanhMucMauXe
    {
        private SsoftvnContext _db;
        public ClassDanhMucMauXe(SsoftvnContext dbcontext)
        {
            _db = dbcontext;
        }
        public List<MauXe> GetListMauXes()
        {
            var xx = (from mau in _db.Gara_MauXe
                      join loai in _db.Gara_LoaiXe on mau.ID_LoaiXe equals loai.ID
                      join hang in _db.Gara_HangXe on mau.ID_HangXe equals hang.ID
                      where mau.TrangThai == 1
                      select new MauXe
                      {
                          ID = mau.ID,
                          TenMauXe = mau.TenMauXe,
                          ID_HangXe = mau.ID_HangXe,
                          ID_LoaiXe = mau.ID_LoaiXe,
                          TenLoaiXe = loai.TenLoaiXe,
                          TenHangXe = hang.TenHangXe,
                          GhiChu = mau.GhiChu,
                          TrangThai = mau.TrangThai
                      }).OrderByDescending(x => x.TenMauXe).ToList();
            return xx;
        }
        public List<Xe> JqAuto_SearchMauXe(string txt)
        {
            SqlParameter sql = new SqlParameter("TextSeach", txt);
            return _db.Database.SqlQuery<Xe>("JqAuto_SearchMauXe @TextSeach", sql).ToList();
        }

        public void AddMauXe(Gara_MauXe obj)
        {
            _db.Gara_MauXe.Add(obj);
            _db.SaveChanges();
        }

        public bool UpdateMauXe(Gara_MauXe obj)
        {
            Gara_MauXe hangxe = _db.Gara_MauXe.Find(obj.ID);
            hangxe.TenMauXe = obj.TenMauXe;
            hangxe.ID_HangXe = obj.ID_HangXe;
            hangxe.ID_LoaiXe = obj.ID_LoaiXe;
            hangxe.GhiChu = obj.GhiChu;
            hangxe.NgaySua = DateTime.Now;
            hangxe.NguoiSua = obj.NguoiSua;
            hangxe.TrangThai = obj.TrangThai;
            _db.SaveChanges();
            return false;
        }

    }
}
