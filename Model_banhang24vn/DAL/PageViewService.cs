using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
  public  class PageViewService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<PageView> _PageView;
        public PageViewService()
        {
            _PageView = unitOfWork.GetRepository<PageView>();
        }

        public object GetAllGroup()
        {
            return _PageView.All().GroupBy(o => o.PageName).Select(o => new { PageName=o.Key, Count=o.Sum(c=>c.Count) }).AsEnumerable().OrderByDescending(o=>o.Count);
        }
        public IQueryable<PageView> GetAll()
        {
            return _PageView.All();
        }
        public void AddView(string url)
        {
            url = url.Trim();
            int datenow = int.Parse(DateTime.Now.ToString("yyyyMM"));
            var model = _PageView.Find(url,datenow);
            if(model==null)
            {
                _PageView.Create(new PageView { PageName = url, MonthDate = datenow, Count = 1 });
            }
            else
            {
                model.Count += 1;
            }
            unitOfWork.Save();
        }
    }
}
