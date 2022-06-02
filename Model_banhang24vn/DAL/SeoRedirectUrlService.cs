using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
   public class SeoRedirectUrlService
    {
        UnitOfWork unitOfWork ;
        public SeoRedirectUrlService()
        {
            unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        }
        public IQueryable<SeoRedirectUrl> GetAll()
        {
            return unitOfWork.GetRepository<SeoRedirectUrl>().All();
        }

        public SeoRedirectUrl GetByUrlNew(string url) {
            if (!string.IsNullOrWhiteSpace(url)) {
                var data = unitOfWork.GetRepository<SeoRedirectUrl>().Filter(o => o.UrlNew == url.ToLower() && o.Status);
                return data.FirstOrDefault();
                   
            }
            return null;

        }

        public IQueryable<SeoRedirectUrl> SearchText(string input)
        {
            var data = unitOfWork.GetRepository<SeoRedirectUrl>().All();
            if (!string.IsNullOrWhiteSpace(input))
            {
                data=data.Where(o => o.UrlNew.Contains(input.ToLower())); ;
            }
            return data.OrderBy(o=>o.ID);
        }
        public bool Insert(SeoRedirectUrl model)
        {
            if(GetAll().Any(o=>o.UrlNew==model.UrlNew || o.UrlOld == model.UrlNew))
            {
                return false;
            }

            unitOfWork.GetRepository<SeoRedirectUrl>().Create(model);
            unitOfWork.Save();
            return true;
        }
        public bool Update(SeoRedirectUrl model)
        {
            if (GetAll().Any(o =>o.ID!=model.ID &&( o.UrlNew == model.UrlNew || o.UrlOld == model.UrlNew)))
            {
                return false;
            }

            unitOfWork.GetRepository<SeoRedirectUrl>().Update(model);
            unitOfWork.Save();
            return true;
        }
        public bool Delete(SeoRedirectUrl model)
        {
            if (GetAll().Any(o => o.ID == model.ID))
            {
                unitOfWork.GetRepository<SeoRedirectUrl>().Delete(model);
                unitOfWork.Save();
                return true;
            }
          
            return false;
        }
    }
}
