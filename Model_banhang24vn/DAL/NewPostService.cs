using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
  public  class NewPostService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<News_Articles> _NewsPost;
        public NewPostService()
        {
            _NewsPost = unitOfWork.GetRepository<News_Articles>();
        }

        public IQueryable<News_Articles> Query { get { return _NewsPost.All().OrderByDescending(o => o.CreateDate); } }

        public int Count { get { return _NewsPost.Count; } }

        public int GetCountMonth(int year,int month)
        {
            return _NewsPost.Filter(o => o.CreateDate.HasValue && o.CreateDate.Value.Year == year && o.CreateDate.Value.Month == month).Count();
        }

        public IEnumerable<Tag> GetTagsByArticle(long Id)
        {
            return (from ar in unitOfWork.GetRepository<ArticleTag>().All()
                    join tag in unitOfWork.GetRepository<Tag>().All()
                    on ar.TagIID equals tag.ID
                    where ar.ArticleID.Equals(Id)
                    select tag).AsEnumerable();
        }

        public IQueryable<News_Articles> GetArticleByTag(string tagId)
        {
            return (from ar in _NewsPost.All()
                    join artag in unitOfWork.GetRepository<ArticleTag>().All()
                    on ar.ID equals artag.ArticleID
                    where artag.TagIID == tagId && ar.Status==true
                    select ar).AsQueryable();
        }

        public Tag GetTagById(string tagId)
        {
            return unitOfWork.GetRepository<Tag>().Find(tagId);
        }

        public int GetCountForQuy(int year, int quy)
        {

            if (quy == 1)
            {
                return _NewsPost.Filter(o => o.CreateDate != null
                                                && o.CreateDate.Value.Year == year
                                                && o.CreateDate.Value.Month >= 1
                                                && o.CreateDate.Value.Month <= 3).Count();
            }
            else if (quy == 2)
            {
                return _NewsPost.Filter(o => o.CreateDate != null
                                               && o.CreateDate.Value.Year == year
                                               && o.CreateDate.Value.Month >= 4
                                               && o.CreateDate.Value.Month <= 6).Count();
            }
            else if (quy == 3)
            {
                return _NewsPost.Filter(o => o.CreateDate != null
                                               && o.CreateDate.Value.Year == year
                                               && o.CreateDate.Value.Month >= 7
                                               && o.CreateDate.Value.Month <= 9).Count();
            }
            else
            {
                return _NewsPost.Filter(o => o.CreateDate != null
                                               && o.CreateDate.Value.Year == year
                                               && o.CreateDate.Value.Month >= 10
                                               && o.CreateDate.Value.Month <= 12).Count();
            }
        }

        public int GetCountForYear(int year)
        {
            return _NewsPost.Filter(o => o.CreateDate.HasValue 
                                    && o.CreateDate.Value.Year == year ).Count();
        }

        public int GetCountForStage(DateTime tuthang, DateTime denthang)
        {
            var data = _NewsPost.Filter(o => o.CreateDate != null
                                                 && ((o.CreateDate.Value.Day >= tuthang.Day
                                                 && o.CreateDate.Value.Month == tuthang.Month
                                                 && o.CreateDate.Value.Year == tuthang.Year)
                                                 || (o.CreateDate.Value.Month > tuthang.Month
                                                    && o.CreateDate.Value.Year == tuthang.Year)
                                                     || o.CreateDate.Value.Year > tuthang.Year)
                                                );
            return data.Where(o => (o.CreateDate.Value.Month < denthang.Month
                                    && o.CreateDate.Value.Year == denthang.Year)
                                     || (o.CreateDate.Value.Year == denthang.Year
                                        && o.CreateDate.Value.Month == denthang.Month
                                        && o.CreateDate.Value.Day <= denthang.Day)
                                    || o.CreateDate.Value.Year < denthang.Year).Count();
        }

        public long GetCountStoreagrea(DateTime date)
        {
            return _NewsPost.Filter(o => o.CreateDate != null
                                                 && (o.CreateDate.Value.Day == date.Day
                                                 && o.CreateDate.Value.Month == date.Month
                                                 && o.CreateDate.Value.Year == date.Year)).Count();
        }

        public IQueryable<News_Articles> GetCategory(int? categoryId)
        {
            return Query.Where(o => o.CategoryID == categoryId && o.Status==true).OrderByDescending(o => o.CreateDate);
        }

        public void UpDatePostSchedule(long id)
        {
            var model = _NewsPost.GetById(id);
            if(model!=null)
            {
                model.Status = true;
                model.CreateDate = model.DatePost;
                unitOfWork.Save();
            }
        }
    }
}
