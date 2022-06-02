using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Sql;
using System.Data.SqlClient;

namespace libGara
{
    public class ClassHangXe
    {
        private SsoftvnContext _db;
        public ClassHangXe(SsoftvnContext dbcontext)
        {
            _db = dbcontext;
        }
        public List<HangXe> GetListHangXes()
        {
            var data = _db.Gara_HangXe.Where(x=>x.TrangThai== 1).Select(x => new HangXe
            {
                ID = x.ID,
                MaHangXe = x.MaHangXe,
                TenHangXe = x.TenHangXe,
                TrangThai = x.TrangThai,
                Logo = x.Logo,
            }).OrderByDescending(x=>x.TenHangXe).ToList();
            return data;
        }

        public void AddHangXe(Gara_HangXe obj)
        {
            _db.Gara_HangXe.Add(obj);
            _db.SaveChanges();
        }

        public bool UpdateHangXe(Gara_HangXe obj)
        {
            Gara_HangXe hangxe = _db.Gara_HangXe.Find(obj.ID);
            hangxe.MaHangXe = obj.MaHangXe;
            hangxe.TenHangXe = obj.TenHangXe;
            hangxe.Logo = obj.Logo;
            hangxe.NgaySua = DateTime.Now;
            hangxe.NguoiSua = obj.NguoiSua;
            hangxe.TrangThai = obj.TrangThai;
            _db.SaveChanges();
            return false;
        }

        public bool DeleteHangXe()
        {
            return false;
        }
    }
}
