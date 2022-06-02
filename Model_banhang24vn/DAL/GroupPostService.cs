using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
   public class GroupPostService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());      
        IRepository<News_Categories> _GroupPost;
        IRepository<News_Articles> _Post;
        public GroupPostService()
        {
            _GroupPost = unitOfWork.GetRepository<News_Categories>();
            _Post = unitOfWork.GetRepository<News_Articles>();
        }
        public IQueryable<News_Categories> Query { get { return _GroupPost.All().OrderByDescending(o => o.CreateDate); } }


        public IQueryable<PostGroupView> Search(string text)
        {
          var   data= (from o in Query
                   join p in Query on o.ParentID equals p.ID into ps
                    from p in ps.DefaultIfEmpty()
                    select new PostGroupView
                    {
                        CreateBy = o.CreateBy,
                        CreateDate = o.CreateDate,
                        Description = o.Description,
                        ID = o.ID,
                        Name = o.Name,
                        ParentID = o.ParentID,
                        Status = o.Status,
                        ParentName = p != null ? p.Name : string.Empty
                    }).AsQueryable();

            if (!string.IsNullOrWhiteSpace(text))
            {
                text = StringExtensions.ConvertToUnSign(text);
                data = data.AsEnumerable().Where(o => StringExtensions.ConvertToUnSign(o.Name).Contains(text)
                                        || StringExtensions.ConvertToUnSign(o.Description).Contains(text)
                                        || StringExtensions.ConvertToUnSign(o.CreateBy).Contains(text)
                                        || StringExtensions.ConvertToUnSign(o.ParentName).Contains(text)).AsQueryable();
            }
            return data;
        }

        public IQueryable<News_Categories> GetGroupParent(int groupId)
        {
            var listGroupUsing = _Post.All().GroupBy(o => o.CategoryID).Select(o => o.Key).ToList();
           return GetStatus().Where(o => o.ID != groupId && !listGroupUsing.Contains(o.ID));
        }

        public IQueryable<News_Categories> GetStatus()
        {
            return Query.Where(o => o.Status == true).OrderByDescending(o => o.CreateDate);
        }

        public IQueryable<News_Categories> GetGroupchilden()
        {
            var data = (from g1 in Query
                        join g2 in Query
                        on g1.ID equals g2.ParentID
                        select g1.ID).AsEnumerable();
            var result = Query.Where(o => !data.Contains(o.ID));
            return result;
        }

        public JsonViewModel<string> Insert(News_Categories model)
        {
            var resul= new JsonViewModel<string> { ErrorCode = (int)Notification.ErrorCode.success };
            if (_GroupPost.Filter(o => o.Name == model.Name).Any())
            {
                resul.Data = "Nhóm bài viết đã tồn tại";
                resul.ErrorCode = (int)Notification.ErrorCode.error;
            }
            else
            {
                model.Url = string.Format("/blog/{0}", StaticVariable.ConvetTitleToUrl(model.Name));
                _GroupPost.Create(model);
                unitOfWork.Save();
            }
            return resul;
        }

        public JsonViewModel<string> Update(News_Categories model)
        {
            var resul = new JsonViewModel<string> { ErrorCode = (int)Notification.ErrorCode.success };
            if (_GroupPost.Filter(o => o.Name == model.Name && o.ID!=model.ID).Any())
            {
                resul.Data = "Nhóm bài viết đã tồn tại";
                resul.ErrorCode = (int)Notification.ErrorCode.error;
            }
            else
            {
                model.Url = string.Format("/blog/{0}", StaticVariable.ConvetTitleToUrl(model.Name));
                _GroupPost.Update(model);
                unitOfWork.Save();
            }
            return resul;
        }

        public JsonViewModel<string> Delete(int Id)
        {
            var result = new JsonViewModel<string> { ErrorCode = (int)Notification.ErrorCode.success };
            var model = _GroupPost.GetById(Id);
            if(model==null)
            {
                result.ErrorCode = (int)Notification.ErrorCode.notfound;
                result.Data = "Nhóm đã bị xóa hoặc không tồn tại, vui lòng kiểm tra lại";
            }
             else if (_GroupPost.All().Where(o => o.ParentID == model.ID).Any())
            {
                result.ErrorCode = (int)Notification.ErrorCode.exist;
                result.Data = "Nhóm bài viết này là nhóm cha cần xóa các nhóm con.";
            }
            else if (_Post.All().Where(o => o.CategoryID == model.ID).Any())
            {
                result.ErrorCode = (int)Notification.ErrorCode.exist;
                result.Data = "Đã có bài viết sử dụng nhóm này vui lòng xóa bài viết trước khi xóa nhóm.";
            }
            else
            {
                _GroupPost.Delete(model);
                unitOfWork.Save();
            }
            return result;
        }
    }
}
