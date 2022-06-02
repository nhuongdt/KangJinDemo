using Model.Web.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web.Service
{
   public class DM_MenuService
    {
        private SsoftvnWebContext db;
        public DM_MenuService()
        {
            db = SystemDBContext.GetDBContext();
        }
        public IQueryable<DM_Menu> getAll()
        {
            return db.DM_Menu.AsQueryable();
        }

        public DM_Menu GetMetaTags(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return new DM_Menu();
            var result = getAll().FirstOrDefault(o => o.Link.ToUpper().Equals(filePath.ToUpper()));
            return result;
        }
        public IQueryable<DM_Menu> SearchGrid( string text)
        {
            var data = db.DM_Menu.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                data = data.Where(o => o.DuongDan.ToUpper().Equals(text.ToUpper()));
            }
          
            return data.OrderBy(o=>o.ThuTuHienThi);
        }

        public void Insert(DM_Menu model)
        {
            model.ID = Guid.NewGuid();
            db.DM_Menu.Add(model);
            db.SaveChanges();
        }
        public bool Update(DM_Menu model)
        {
            var data = db.DM_Menu.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
                return false;
            data.DuongDan = model.DuongDan;
            data.Description = model.Description;
            data.ID_Loaimenu = model.ID_Loaimenu;
            data.KeyWord = model.KeyWord;
            data.Link = model.Link;
            data.ThuTuHienThi = model.ThuTuHienThi;
            data.Title = model.Title;
            data.TrangThai = model.TrangThai;
            db.SaveChanges();
            return true;
        }
        public void Delete(DM_Menu model)
        {
            var data = db.DM_Menu.FirstOrDefault(o => o.ID == model.ID);
            if (data != null)
            {
                db.DM_Menu.Remove(data);
                db.SaveChanges();
            }
        }
    }
}
