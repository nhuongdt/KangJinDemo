using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libGara
{
    public class ClassLoaiXe
    {
        private SsoftvnContext _db;
        public ClassLoaiXe(SsoftvnContext dbcontext)
        {
            _db = dbcontext;
        }
        public List<LoaiXe> GetListLoaiXes()
        {
            var data = _db.Gara_LoaiXe.Where(x => x.TrangThai == 1).Select(x => new LoaiXe
            {
                ID = x.ID,
                MaLoaiXe = x.MaLoaiXe,
                TenLoaiXe = x.TenLoaiXe,
                TrangThai = x.TrangThai,
            }).OrderByDescending(x => x.TenLoaiXe).ToList();
            return data;
        }

        public void AddLoaiXe(Gara_LoaiXe obj)
        {
            _db.Gara_LoaiXe.Add(obj);
            _db.SaveChanges();
        }

        public bool UpdateLoaiXe(Gara_LoaiXe obj)
        {
            Gara_LoaiXe hangxe = _db.Gara_LoaiXe.Find(obj.ID);
            hangxe.MaLoaiXe = obj.MaLoaiXe;
            hangxe.TenLoaiXe = obj.TenLoaiXe;
            hangxe.NgaySua = DateTime.Now;
            hangxe.NguoiSua = obj.NguoiSua;
            hangxe.TrangThai = obj.TrangThai;
            _db.SaveChanges();
            return false;
        }
    }
}
