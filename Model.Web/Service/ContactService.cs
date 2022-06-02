using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Web.API;
using Ssoft.Common.Common;

namespace Model.Web.Service
{
   public class ContactService
    {
        private SsoftvnWebContext db;
        public ContactService()
        {
            db = SystemDBContext.GetDBContext();
        }
        public void Insert(DM_LienHe model)
        {
            db.DM_LienHe.Add(model);
            db.SaveChanges();
        }

        public IQueryable<DM_LienHe> SearchGrid(string text,List<int> trangthai)
        {
            if (trangthai == null)
            {
                trangthai = new List<int>();
            }
            if (string.IsNullOrWhiteSpace(text))
                return db.DM_LienHe.Where(o => trangthai.Contains(o.TrangThai));
            return db.DM_LienHe.Where(o =>  o.TenNguoiLienHe.ToUpper().Contains(text.ToUpper()) 
                                                || o.Email.ToUpper().Contains(text.ToUpper())
                                                || o.SoDienThoai.ToUpper().Contains(text.ToUpper())
                                                || o.DiaChi.ToUpper().Contains(text.ToUpper())).Where(o=> trangthai.Contains(o.TrangThai));
        }

        public void UpdateContactRead(Guid id)
        {
            var model = db.DM_LienHe.FirstOrDefault(o => o.ID == id);
            if (model != null)
            {
                model.TrangThai = (int)LibEnum.IsStatus.an;
                db.SaveChanges();
            }
        }
    }
}
