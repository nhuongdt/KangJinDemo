using Model_banhang24vn.Common;
using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Model_banhang24vn.DAL
{
  public  class HoTroService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<LH_HoiDap> _LH_HoiDap;
        IRepository<LH_NhomNganh> _LH_NhomNganh;
        IRepository<LH_TinhNang> _LH_TinhNang;
        IRepository<LH_NhomNganh_TinhNang> _LH_NhomNganh_TinhNang;

        public HoTroService()
        {
            _LH_HoiDap = unitOfWork.GetRepository<LH_HoiDap>();
            _LH_TinhNang = unitOfWork.GetRepository<LH_TinhNang>();
            _LH_NhomNganh= unitOfWork.GetRepository<LH_NhomNganh>();
            _LH_NhomNganh_TinhNang = unitOfWork.GetRepository<LH_NhomNganh_TinhNang>();
        }
        
        public IQueryable<LH_TinhNang> GetAllTinhNang { get { return _LH_TinhNang.All().OrderByDescending(o => o.NgayTao).AsQueryable(); } }

        public IQueryable<LH_HoiDap> GetSearchHoiDap(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return _LH_HoiDap.All().OrderByDescending(o => o.NgayTao);
            }
            input = input.ToLower();
            return _LH_HoiDap.Filter(o => o.CauHoi.ToLower().Contains(input) || input.Contains(o.CauHoi.ToLower()));
        }

        public IQueryable<LH_NhomNganh> GetSearchNhomVaiTro(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return _LH_NhomNganh.All().OrderByDescending(o => o.NgayTao);
            }
            input = input.ToLower();
            return _LH_NhomNganh.Filter(o => o.Ten.ToLower().Contains(input) || input.Contains(o.Ten.ToLower())).OrderByDescending(o => o.NgayTao);
        }

        public IQueryable<LH_TinhNang> GetSearchTinhNang(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return _LH_TinhNang.All().OrderByDescending(o => o.NgayTao);
            }
            input = input.ToLower();
            return _LH_TinhNang.Filter(o => o.Ten.ToLower().Contains(input) || input.Contains(o.Ten.ToLower()));
        }

        public void InsertHoiDap(LH_HoiDap model)
        {
            model.NgayTao = DateTime.Now;
            _LH_HoiDap.Create(model);
            unitOfWork.Save();
        }

        public bool UpdateHoiDap(LH_HoiDap model)
        {
            var data = _LH_HoiDap.Find(o => o.ID == model.ID);
            if (data == null)
            { return false; }
            else
            {
                data.CauHoi = model.CauHoi ;
                data.CauTraLoi = model.CauTraLoi;
                data.TrangThai = model.TrangThai;
                data.NgaySua = DateTime.Now;
                data.ViTri = model.ViTri;
                _LH_HoiDap.Update(data);
                unitOfWork.Save();
                return true;
            }
        }

        public void DeleteHoiDap(LH_HoiDap model)
        {
                 _LH_HoiDap.Delete(o => o.ID == model.ID);
                unitOfWork.Save();
         
        }

        public void InsertTinhNang(LH_TinhNang model)
        {
            model.NgayTao = DateTime.Now;
            model.TenTiengViet = StaticVariable.RemoveSign4VietnameseString(model.Ten);
            _LH_TinhNang.Create(model);
            unitOfWork.Save();
        }

        public bool UpdateTinhNang(LH_TinhNang model)
        {
            var data = _LH_TinhNang.Find(o => o.ID == model.ID);
            if (data == null)
            { return false; }
            else
            {
                data.NoiDung = model.NoiDung;
                if(!string.IsNullOrWhiteSpace(model.Icon) && model.Icon!=data.Icon)
                {
                    string file = "~" + data.Icon;
                    StaticVariable.deletefile(file);
                    data.Icon = model.Icon;
                }
                data.TrangThai = model.TrangThai;
                data.ID_Cha = model.ID_Cha;
                data.Ten = model.Ten;
                data.Video = model.Video;
                data.TenTiengViet = StaticVariable.RemoveSign4VietnameseString(data.Ten);
                data.NgaySua = DateTime.Now;
                data.GhiChu = model.GhiChu;
                data.ViTri = model.ViTri;
                _LH_TinhNang.Update(data);
                unitOfWork.Save();
                return true;
            }
        }

        public void DeleteTinhNang(long id)
        {
            var listdelete = new List<long>();
            listdelete.Add(id);
            GetlistIdCon(listdelete, ref listdelete);
            _LH_NhomNganh_TinhNang.Delete(o => listdelete.Contains(o.ID_TinhNang));
            var datadelete = _LH_TinhNang.Filter(o => listdelete.Contains(o.ID));
            foreach(var item in datadelete)
            {
                string file = "~" + item.Icon;
                StaticVariable.deletefile(file);
            }
            _LH_TinhNang.Delete(datadelete);
            unitOfWork.Save();

        }

        public void GetlistIdCon(List<long>listid,ref List<long> listIddelete)
        {
            var data = _LH_TinhNang.Filter(o => listid.Contains(o.ID_Cha ?? 0)).Select(o => o.ID).ToList() ;
            if (data.Count > 0)
            {
                listIddelete.AddRange(data);
                listIddelete.Distinct();
                GetlistIdCon(data,ref listIddelete);
            }
        }

        public void InsertNhomNganh(LH_NhomNganh model,List<long> listTinhNang)
        {
            if (listTinhNang == null)
            {
                listTinhNang = new List<long>();
            }
            model.NgayTao = DateTime.Now;
            _LH_NhomNganh.Create(model);
            unitOfWork.Save();
            var data = listTinhNang.Select(o =>
             new LH_NhomNganh_TinhNang
             {
                 ID_NhomNganh = model.ID,
                 ID_TinhNang=o
             });
            _LH_NhomNganh_TinhNang.Create(data);
            unitOfWork.Save();

        }

        public bool UpdateNHomNganh(LH_NhomNganh model, List<long> listTinhNang)
        {
            if (listTinhNang == null)
            {
                listTinhNang = new List<long>();
            }
            var data = _LH_NhomNganh.Find(o => o.ID == model.ID);
            if (data == null)
            { return false; }
            else
            {
                data.Ten = model.Ten;
                data.GhiChu = model.GhiChu;
                if (!string.IsNullOrWhiteSpace(model.Icon) && model.Icon != data.Icon)
                {
                    string file = "~" + data.Icon;
                    StaticVariable.deletefile(file);
                    data.Icon = model.Icon;
                }
                data.TrangThai = model.TrangThai;
                data.NgaySua = DateTime.Now;
                data.ViTri = model.ViTri;
                _LH_NhomNganh.Update(data);
                _LH_NhomNganh_TinhNang.Delete(o => o.ID_NhomNganh == data.ID && !listTinhNang.Contains(o.ID_TinhNang));
                var listold = _LH_NhomNganh_TinhNang.Filter(o => o.ID_NhomNganh == data.ID ).Select(o=>o.ID_TinhNang).ToList();
                var result  = listTinhNang.Where(o=>!listold.Contains(o)).Select(o =>
                      new LH_NhomNganh_TinhNang
                      {
                          ID_NhomNganh = model.ID,
                          ID_TinhNang = o
                      });
                _LH_NhomNganh_TinhNang.Create(result);
                unitOfWork.Save();
                return true;
            }
        }

        public void DeleteNhomNganh(long id)
        {
            _LH_NhomNganh_TinhNang.Delete(o => o.ID_NhomNganh == id);
            var data = _LH_NhomNganh.Find(o => o.ID == id);
            if (data != null)
            {
                string file ="~"+ data.Icon;
                StaticVariable.deletefile(file);
                _LH_NhomNganh.Delete(data);
            }
            unitOfWork.Save();

        }

        public IQueryable<LH_TinhNang> GetJoinNhomNganhTinhNang(long groupId)
        {
            if (groupId == 0)
            {
                return GetAllTinhNang.Where(o=>o.ID_Cha==null);
            }
            var data = from a in _LH_NhomNganh_TinhNang.All()
                       join b in _LH_TinhNang.All()
                       on a.ID_TinhNang equals b.ID
                       where groupId == a.ID_NhomNganh && b.ID_Cha==null
                       select b;
            return data;
        }

        public IQueryable<LH_TinhNang> GetSearch(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return GetAllTinhNang;
            }
            input = input.ToLower();
            return GetAllTinhNang.Where(o => o.Ten.ToLower().Contains(input) || input.Contains(o.Ten.ToLower())
                                              ||  o.TenTiengViet.ToLower().Contains(input) || input.Contains(o.TenTiengViet.ToLower()) );
        }

    }
}
