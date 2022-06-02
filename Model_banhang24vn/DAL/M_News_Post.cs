using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model_banhang24vn;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using Model_banhang24vn.Common;

namespace Model_banhang24vn
{
    public class M_News_Post
    {
        #region select
        public static IQueryable<m_Article> GetAllArticle()
        {
            BanHang24vnContext context = new BanHang24vnContext();
            var data = from article in context.News_Articles
                       join cate in context.News_Categories on article.CategoryID equals cate.ID
                       //join user in context.News_User on article.CreatedBy equals user.ID
                       select new
                       {
                           ID = article.ID,
                           Title = article.Title,
                           Summary = article.Summary,
                           Image = article.UrlImage,
                           CategoryID = article.CategoryID,
                           CategoryName = cate.Name,
                           CreateDate = article.CreateDate,
                           CreatedBy = article.CreatedBy,
                           link = article.Url,
                           View=article.ViewCount,
                           Gender=article.Gender
                       };
            var test = from a in data
                       join b in context.News_User on a.CreatedBy equals b.ID
                       into buser
                       from c in buser.DefaultIfEmpty()
                       select new m_Article
                       {
                           ID = a.ID,
                           Title = a.Title,
                           Summary = a.Summary,
                           Image = a.Image,
                           CategoryID = a.CategoryID,
                           CategoryName = a.CategoryName,
                           CreateDate = a.CreateDate,
                           UserName = c != null ? c.UserName : "",
                           link = a.link,
                           View=a.View,
                           Gender = a.Gender
                       };
            if (test == null)
            {
                return null;
            }
            else
            {
                return test.AsQueryable();
            }
        }

        public static IQueryable<News_Articles> GetDataBycategory(int? CategoryId)
        {
            BanHang24vnContext context = new BanHang24vnContext();
            return context.News_Articles.Where(o => o.CategoryID == CategoryId);
        }
        public static IQueryable<News_Articles> GetAllNews()
        {
            BanHang24vnContext context = new BanHang24vnContext();
            return context.News_Articles;
        }

        public static IQueryable<m_Article> GetDataForSearch(string text)
        {
            if(string.IsNullOrWhiteSpace(text))
            {
                return GetAllArticle();
            }
            text = StringExtensions.ConvertToUnSign(text);
            BanHang24vnContext context = new BanHang24vnContext();
            
            var data = from article in context.News_Articles.AsEnumerable()
                       join cate in context.News_Categories.AsEnumerable() on article.CategoryID equals cate.ID
                       where StringExtensions.ConvertToUnSign(article.Title).Contains(text)
                            || StringExtensions.ConvertToUnSign(cate.Name).Contains(text)
                       select new
                       {
                           ID = article.ID,
                           Title = article.Title,
                           Summary = article.Summary,
                           Image = article.UrlImage,
                           CategoryID = article.CategoryID,
                           CategoryName = cate.Name,
                           CreateDate = article.CreateDate,
                           CreatedBy = article.CreatedBy,
                           link = article.Url,
                           View=article.ViewCount,
                           Gender = article.Gender
                       };
            var test = from a in data
                       join b in context.News_User on a.CreatedBy equals b.ID
                       into buser
                       from c in buser.DefaultIfEmpty()
                       select new m_Article
                       {
                           ID = a.ID,
                           Title = a.Title,
                           Summary = a.Summary,
                           Image = a.Image,
                           CategoryID = a.CategoryID,
                           CategoryName = a.CategoryName,
                           CreateDate = a.CreateDate,
                           UserName = c != null ? c.UserName : "",
                           link = a.link,
                           View = a.View,
                           Gender = a.Gender
                       };
            if (test == null)
            {
                return null;
            }
            else
            {
                return test.AsQueryable();
            }
        }

        public static IQueryable<m_Article> GetAllArticleNews()
        {
            BanHang24vnContext context = new BanHang24vnContext();
            var data = from article in context.News_Articles
                       join cate in context.News_Categories on article.CategoryID equals cate.ID
                       join catetype in context.News_CategoriesType on cate.CategoryTypeID equals catetype.ID
                       where catetype.Position == 1
                       select new m_Article
                       {
                           ID = article.ID,
                           Title = article.Title,
                           Summary = article.Summary,
                           Image = article.UrlImage,
                           CategoryID = article.CategoryID,
                           CategoryName = cate.Name,
                           //CategoryTypeID = cate.ID,
                           //CategoryTypeName = catetype.Name,
                           CreateDate = article.CreateDate
                       };
            if (data == null)
            {
                return null;
            }
            else
            {
                return data;
            }

            //return context.News_Articles.OrderByDescending(p => p.CreateDate);
        }

        public static IQueryable<m_Article> GetArticleNews()
        {
            BanHang24vnContext context = new BanHang24vnContext();
            var data = from article in context.News_Articles
                       where article.Status == true
                       select new m_Article
                       {
                           ID = article.ID,
                           Title = article.Title,
                           Summary = article.Summary,
                           Image = article.UrlImage,
                           CategoryID = article.CategoryID,
                           CreateDate = article.CreateDate,
                           Url = article.Url
                       };
            if (data == null)
            {
                return null;
            }
            else
            {
                return data;
            }

            //return context.News_Articles.OrderByDescending(p => p.CreateDate);
        }

        public static IQueryable<m_Article> GetNextArticleNews()
        {
            BanHang24vnContext context = new BanHang24vnContext();
            var data = (from article in context.News_Articles
                        where article.Status == true
                        select new m_Article
                        {
                            ID = article.ID,
                            Title = article.Title,
                            Summary = article.Summary,
                            Image = article.UrlImage,
                            CategoryID = article.CategoryID,
                            CreateDate = article.CreateDate,
                            Url = article.Url
                        }).OrderByDescending(p => p.CreateDate).Skip(6).Take(4);
            if (data == null)
            {
                return null;
            }
            else
            {
                return data;
            }

            //return context.News_Articles.OrderByDescending(p => p.CreateDate);
        }

        public static IQueryable<m_ArticleDetailNews> GetArticleNewsDetail(long id)
        {
            BanHang24vnContext db = new BanHang24vnContext();
            var data = from article in db.News_Articles
                       where article.ID == id && article.Status == true
                       select new m_ArticleDetailNews
                       {
                           ID = article.ID,
                           Title = article.Title,
                           Summary = article.Summary,
                           UrlImage = article.UrlImage,
                           Content = article.Content,
                           CreatedBy = article.CreatedBy,
                           UpdatedBy = article.UpdatedBy,
                           CreateDate = article.CreateDate,
                           UpdateDate = article.UpdateDate,
                           Tag = article.Tag
                       };
            if (data == null)
            {
                return null;
            }
            else
            {
                return data;
            }
        }

        public News_Articles GetArticleNewsDetail1(string title, Guid id)
        {
            BanHang24vnContext db = new BanHang24vnContext();

            return db.News_Articles.Find(id);
        }
        public News_Articles GetArticleNewsDetailUpdateView(string title, long keyId)
        {
            BanHang24vnContext db = new BanHang24vnContext();
            var result = db.News_Articles.FirstOrDefault(o => o.ID == keyId);
            if (result != null)
            {
                result.ViewCount = (result.ViewCount == null) ? 1 : result.ViewCount + 1;
                db.SaveChanges();
                return result;
            }
            return null;
        }
        public News_Articles GetArticleNewsDetailkey(string title, long keyId)
        {
            BanHang24vnContext db = new BanHang24vnContext();
            return db.News_Articles.FirstOrDefault(o => o.ID == keyId);
        }

        public List<News_Articles> GetListRlatedArticles(int categoryID,string tags,long keyId)
        {
            BanHang24vnContext db = new BanHang24vnContext();

            return db.News_Articles.Where(o => o.CategoryID == categoryID && o.ID != keyId).OrderBy(o => o.CreateDate).Take(5).ToList() ;
        }

        public static IQueryable<News_Categories> getallCategories(int id)
        {
            BanHang24vnContext context = new BanHang24vnContext();
            return context.News_Categories.Where(o=>o.Status==true).OrderByDescending(p => p.ID);
        }

        public static News_Categories GetCategorybyId(int CategoryId)
        {
            BanHang24vnContext context = new BanHang24vnContext();
            return context.News_Categories.Where(p => p.ID == CategoryId).FirstOrDefault();
        }

        public static IQueryable<m_ArticleAll> getCateIDArticleforUpdate(long id)
        {
            BanHang24vnContext context = new BanHang24vnContext();

            var data = from article in context.News_Articles
                       join cate in context.News_Categories on article.CategoryID equals cate.ID
                       where article.ID == id
                       select new m_ArticleAll
                       {
                           ID = article.ID,
                           Title = article.Title,
                           Summary = article.Summary,
                           UrlImage = article.UrlImage,
                           Content = article.Content,
                           Tag = article.Tag,
                           CategoryID = article.CategoryID,
                           CreatedBy = article.CreatedBy,
                           UpdatedBy = article.UpdatedBy,
                           CreateDate = article.CreateDate,
                           UpdateDate = article.UpdateDate,
                           Status = article.Status,
                           Salary = article.Salary,
                           Address = article.Address,
                           Experience = article.Experience,
                           Position = article.Position,
                           Degree = article.Degree,
                           WorkingForm = article.WorkingForm,
                           NumberOfRecruits = article.NumberOfRecruits,
                           Gender = article.Gender,
                           Trades = article.Trades,
                           ExpirationDate = article.ExpirationDate,
                           CategoryName = cate.Name,
                           Url = article.Url,
                           DatePost=article.DatePost
                       };

            if (data == null)
            {
                return null;
            }
            else
            {
                return data;//context.News_Articles.Find(id);
            }
        }

        public static IQueryable<News_CategoriesType> getallCategoryType(int pid)
        {
            BanHang24vnContext context = new BanHang24vnContext();
            return context.News_CategoriesType.Where(p => p.Position == pid).OrderByDescending(p => p.ID);
        }

        public static IQueryable<NganhNgheKinhDoanh> getNganhNgheKinhDoanh()
        {
            BanHang24vnContext context = new BanHang24vnContext();
            return context.NganhNgheKinhDoanhs.OrderByDescending(p => p.ID);
        }

        public static IQueryable<CuaHangDangKy> getCuaHangDangKy()
        {
            BanHang24vnContext context = new BanHang24vnContext();
            return context.CuaHangDangKies;
        }

        public static IQueryable<News_Categories> getListCateGroup()
        {
            BanHang24vnContext context = new BanHang24vnContext();
            return context.News_Categories;
        }

        public static List<m_Category> getCateGroup()
        {
            BanHang24vnContext context = new BanHang24vnContext();

            //var data = from cate in context.News_Categories
            //           where cate.ParentID == null
            //           select new m_Category
            //           {
            //               ID = cate.ID,
            //               text = cate.Name,
            //               nodes = (from pa in context.News_Categories
            //                        where pa.ParentID == cate.ID
            //                        select new m_CategoryParent
            //                        {
            //                            parentID = pa.ID,
            //                            text = pa.Name,
            //                            nodes = (from pa2 in context.News_Categories
            //                                     where (pa.ID == pa2.ID && pa.ParentID != null) && pa.ID == pa2.ID// .ParentID == cate.ID && cate.ID == pa2.ParentID
            //                                     select new m_CategoryParent2
            //                                     {
            //                                         parentID = pa2.ID,
            //                                         text = pa2.Name
            //                                     }).ToList()
            //                        }).ToList()
            //           };

            //var data = from parents in context.News_Categories
            //           from all in context.News_Categories
            //           where parents.ValidFlag == 1 &&
            //           parents.ParentID == null &&
            //           all.ValidFlag == 1 && (
            //                (all.ParentID == null && all.ID == parents.ID) || all.ParentID == parents.ID
            //           )
            //           select new m_Category
            //           {
            //               ID = all.ID,
            //               text = all.Name,
            //               nodes = (from pa in context.News_Categories
            //                        select new m_CategoryParent
            //                        {
            //                            parentID = pa.ID,
            //                            text = pa.Name
            //                        }).ToList()
            //           };
            
            var dataNews_Categories = context.News_Categories.Where(o => o.ParentID == null).ToList();
            var test = (from a in dataNews_Categories
                        select new m_Category
                        {
                            ID = a.ID,
                            text = a.Name,
                            nodes = GetParents(context, a.ID)
                        }).ToList();

            if (test == null)
            {
                return null;
            }
            else
            {
                return test;//context.News_Articles.Find(id);
            }

            //return context.News_Categories;

        }
        public static int UpdateArticlesPin(long? id)
        {
            BanHang24vnContext context = new BanHang24vnContext();
            var model = context.News_Articles.Find(id);
            if (model == null)
                return (int)Notification.ErrorCode.error;
            if (model.Gender == true)
            {
                model.Gender = false;
            }
            else
            {
                model.Gender = true;
            }
            context.News_Articles.Where(o => o.ID != model.ID && o.Gender == true).ToList().ForEach(o => o.Gender = false);
            context.SaveChanges();
            return (int)Notification.ErrorCode.success;
        }

        private static object GetParents(BanHang24vnContext context, int parentId)
        {
            var test1 = (from a in context.News_Categories
                         where a.ParentID == parentId
                         select new m_CategoryParent
                         {
                             parentID = a.ID,
                             text = a.Name,
                             nodes = (from b in context.News_Categories
                                      where b.ParentID == a.ID
                                      select new
                                      {
                                          parentID = b.ID,
                                          text = b.Name,
                                          nodes = (from c in context.News_Categories
                                                   where c.ParentID == b.ID
                                                   select new
                                                   {
                                                       parentID = c.ID,
                                                       text = c.Name,
                                                   }).ToList()
                                      }).ToList()
                         }).ToList();
            return test1;

        }


        #endregion

        #region insert
        public static string InsertArticle(News_Articles model,Guid? nguoidungId)
        {
            string status = "";
            BanHang24vnContext context = new BanHang24vnContext();
            try
            {
                var ar = context.News_Articles.Find(model.ID);
                if (ar == null)
                {

                    if(model.DatePost!=null && (model.DatePost.Value.Year>DateTime.Now.Year 
                                            || (model.DatePost.Value.Year == DateTime.Now.Year && model.DatePost.Value.Month > DateTime.Now.Month)
                                            || (model.DatePost.Value.Year == DateTime.Now.Year && model.DatePost.Value.Month == DateTime.Now.Month && model.DatePost.Value.Day >= DateTime.Now.Day)))
                    {
                        model.Status = false;
                    }
                    else
                    {
                        model.DatePost = null;
                    }
                    model.CreatedBy = nguoidungId;
                    context.News_Articles.Add(model);
                    context.SaveChanges();
                    var data = context.News_Articles.Find(model.ID);
                    data.Url =string.Format("/tin-tuc/{0}-{1}.html", model.Url, data.ID);
                    //Xử lý tag
                    if (!string.IsNullOrEmpty(model.Tag))
                    {
                        string[] tags = model.Tag.Split(',');

                        List<string> tagsDelete = new List<string>();
                        foreach (var tag in tags)
                        {
                            var tagId = StaticVariable.ToUnsignString(tag.Trim());
                            if (!tagsDelete.Any(o => o.Equals(tagId)))
                            {
                                tagsDelete.Add(tagId);
                                //insert to to tag table
                                if (!context.Tags.Any(o => o.ID.Equals(tagId)))
                                {
                                    context.Tags.Add(new Tag { ID = tagId, Name = tag.Trim(), KeyWords = StaticVariable.RemoveSign4VietnameseString(tag.Trim()) });
                                }
                                //insert to content tag
                                context.ArticleTags.Add(new ArticleTag { ArticleID = model.ID, TagIID = tagId });
                            }
                        }
                    }
                    context.SaveChanges();

                    if (model.Gender == true)
                    {
                        context.News_Articles.Where(o => o.ID != model.ID && o.Gender == true).ToList().ForEach(o => o.Gender = false);
                        context.SaveChanges();
                    }
                    status = "Created";
                }
                else
                {

                    ar.ID = model.ID;
                    ar.UpdatedBy = nguoidungId;
                    ar.Title = model.Title;
                    ar.Summary = model.Summary;
                    if (model.UrlImage != "")
                    {
                        ar.UrlImage = model.UrlImage;
                    }
                    ar.Content = model.Content;
                    ar.Tag = model.Tag;
                    ar.CategoryID = model.CategoryID;
                    ar.UpdateDate = model.UpdateDate;
                    ar.Status = model.Status;
                    if (model.DatePost != null && (model.DatePost.Value.Year > DateTime.Now.Year
                                            || (model.DatePost.Value.Year == DateTime.Now.Year && model.DatePost.Value.Month > DateTime.Now.Month)
                                            || (model.DatePost.Value.Year == DateTime.Now.Year && model.DatePost.Value.Month == DateTime.Now.Month && model.DatePost.Value.Day >= DateTime.Now.Day)))
                    {
                        ar.Status = false;
                        ar.DatePost = model.DatePost;
                    }
                    else
                    {
                        ar.DatePost = null;
                    }
                    ar.Salary = model.Salary;
                    ar.Address = model.Address;
                    ar.Experience = model.Experience;
                    ar.Position = model.Position;
                    ar.Degree = model.Degree;
                    ar.WorkingForm = model.WorkingForm;
                    ar.NumberOfRecruits = model.NumberOfRecruits;
                    ar.Gender = model.Gender;
                    ar.Trades = model.Trades;
                    ar.ExpirationDate = model.ExpirationDate;
                    ar.Url = string.Format("/tin-tuc/{0}-{1}.html", model.Url, ar.ID); 
                    //context.News_Articles.Add(ar);

                    context.Entry(ar).State = EntityState.Modified;
                    //Xử lý tag
                    if (!string.IsNullOrEmpty(model.Tag))
                    {
                        string[] tags = model.Tag.Split(',');
                        List<string> tagsDelete = new List<string>();
                        foreach (var tag in tags)
                        {
                            var tagId = StaticVariable.ToUnsignString(tag.Trim());
                            if (!tagsDelete.Any(o => o.Equals(tagId)))
                            {
                                tagsDelete.Add(tagId);
                                //insert to to tag table
                                if (!context.Tags.Any(o => o.ID.Equals(tagId)))
                                {
                                    context.Tags.Add(new Tag { ID = tagId, Name = tag.Trim(), KeyWords = StaticVariable.RemoveSign4VietnameseString(tag.Trim()) });
                                }
                                //insert to content tag
                                if (!context.ArticleTags.Any(o => o.TagIID.Equals(tagId) && o.ArticleID.Equals(model.ID)))
                                {
                                    context.ArticleTags.Add(new ArticleTag { ArticleID = model.ID, TagIID = tagId });
                                }
                            }
                        }

                        var listTagDelete = context.ArticleTags.Where(o => !tagsDelete.Contains(o.TagIID) && o.ArticleID==model.ID).ToList();
                        context.ArticleTags.RemoveRange(listTagDelete);
                    }
                    context.SaveChanges();
                    status = "Updated";
                }

            }
            catch (Exception ex)
            {
                status = ex.Message + ex.InnerException;
                throw;
            }
            return status;
        }
        public static string insertCategory(News_Categories model)
        {
            string status = "";
            BanHang24vnContext context = new BanHang24vnContext();
            try
            {
                if(model.CategoryTypeID==null)
                {
                    model.CategoryTypeID = 1;
                }
                context.News_Categories.Add(model);
                context.SaveChanges();
                status = "Created";
            }
            catch (Exception ex)
            {
                status = ex.Message + ex.InnerException;
                throw;
            }
            return status;
        }


        public static string UpdateDangKyV2(CuaHangDangKy objDangKy)
        {
            try
            {
                BanHang24vnContext db = new BanHang24vnContext();
                var data = db.CuaHangDangKies.FirstOrDefault(o => o.SoDienThoai.Equals(objDangKy.SoDienThoai) && o.version== (int)Notification.VersionStore.chuadangky);
                if (data == null)
                {
                    if (db.CuaHangDangKies.Any(o => o.SoDienThoai.Equals(objDangKy.SoDienThoai)))
                    {
                        return "Số điện thoại đã được đăng ký, vui lòng thử lại sau";

                    }
                    return UpdateDangKyV1(objDangKy);
                }

                data.SubDomain = objDangKy.SubDomain;
                data.TenCuaHang = objDangKy.TenCuaHang;
                data.UserKT = objDangKy.UserKT;
                data.MatKhauKT = objDangKy.MatKhauKT;
                data.version = objDangKy.version;
                db.SaveChanges();
                return "";
            }
            catch
            {
                return "Đã xảy ra lỗi, vui lòng thử lại sau";
            }
        }
        public static string UpdateDangKyV1(CuaHangDangKy objDangKy)
        {
            BanHang24vnContext db = new BanHang24vnContext();
            try
            {
                var data = db.CuaHangDangKies.FirstOrDefault(o => o.SoDienThoai == objDangKy.SoDienThoai && o.version == (int)Notification.VersionStore.chuadangky);
                if (data == null)
                {
                    db.CuaHangDangKies.Add(objDangKy);
                }
                else
                {
                    data.HoTen = objDangKy.HoTen;
                    data.ID_NganhKinhDoanh = objDangKy.ID_NganhKinhDoanh;
                    data.Email = objDangKy.Email;
                    data.NgayTao = DateTime.Now;
                    data.KhuVuc_DK = objDangKy.KhuVuc_DK;
                    data.DiaChiIP_DK = objDangKy.DiaChiIP_DK;
                    data.HeDieuHanh_DK = objDangKy.HeDieuHanh_DK;
                    data.ThietBi_DK = objDangKy.ThietBi_DK;
                    data.TrinhDuyet_DK = objDangKy.TrinhDuyet_DK;
                    data.MaKichHoat = objDangKy.MaKichHoat;
                    data.HanSuDung = objDangKy.HanSuDung;
                    data.TrangThai = objDangKy.TrangThai;
                    data.version = objDangKy.version;
                    data.ID_GoiDichVu = objDangKy.ID_GoiDichVu;
                    data.IsCreateDatabase = objDangKy.IsCreateDatabase;
                }
                db.SaveChanges();
            }
            catch
            {
                return "Đã xảy ra lỗi, vui lòng thử lại sau";
            }
            return "";
        }

        public static string AddNewCuaHangDangKy(CuaHangDangKy objDangKy)
        {
            string strReturn = "";
            BanHang24vnContext db = new BanHang24vnContext();
            try
            {
                //objDangKy.NgayTao = DateTime.Now;
                //
                db.CuaHangDangKies.Add(objDangKy);
                db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }
            //catch (Exception ex)
            //{
            //    return ex.Message;
            //}
            return strReturn;
        }

        public static string InsertContact(Contact model)
        {
            string status = "";
            BanHang24vnContext context = new BanHang24vnContext();
            try
            {
                model.ID = Guid.NewGuid();
                context.Contacts.Add(model);
                context.SaveChanges();
                status = "Created";
            }
            catch (Exception ex)
            {
                status = ex.Message + ex.InnerException;
                throw;
            }
            return status;
        }

        //public static string InsertCateGroup(News_Categories model)
        //{
        //    string status = "";
        //    BanHang24vnContext context = new BanHang24vnContext();
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        status = ex.ToString();
        //    }
        //}

        #endregion

        #region delete
        public static JsonViewModel<string> deleteArticle(long id)
        {
            var result = new JsonViewModel<string>();
            result.ErrorCode = (int)Notification.ErrorCode.error;
            BanHang24vnContext context = new BanHang24vnContext();
            if (context == null)
            {
                result.Data= "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    News_Articles ar = context.News_Articles.Find(id);
                    if (ar != null)
                    {
                        result.Data = ar.UrlImage;
                        context.News_Articles.Remove(ar);
                        var listDelete=  context.ArticleTags.Where(o => o.ArticleID.Equals(id)).AsEnumerable();
                        context.ArticleTags.RemoveRange(listDelete);
                        context.SaveChanges();
                        result.ErrorCode = (int)Notification.ErrorCode.success;
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    result.Data = ex.Message;
                }
            }
            return result;
        }
        #endregion
    }

    #region model
    public class m_Title
    {
        public Guid ID { get; set; }
        public int? PostGroupID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? NumberOfPosts { get; set; }
        public string Avatar { get; set; }
        public string UrlTitle { get; set; }
        public string KeywordTitle { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CrreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public bool? Status { get; set; }
    }
    public class m_Post
    {
        public Guid ID { get; set; }

        public int? TitleID { get; set; }

        public string Content { get; set; }

        public string Avatar { get; set; }

        public string Video { get; set; }

        public bool? Status { get; set; }

        public string Salary { get; set; }

        public string Address { get; set; }

        public string Experience { get; set; }

        public string Position { get; set; }

        public string Degree { get; set; }

        public string WorkingForm { get; set; }

        public string NumberOfRecruits { get; set; }

        public bool? Gender { get; set; }

        public string Trades { get; set; }

        public DateTime? ExpirationDate { get; set; }
    }
    public class m_Article
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }
        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Url { get; set; }
        public int? CreatedBy { get; set; }
        public string UserName { get; set; }
        public string link { get; set; }
        public int? View { get; set; }
        public bool? Gender { get; set; }
    }
    public class m_ArticleAll
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string UrlImage { get; set; }
        public string Content { get; set; }
        public string Tag { get; set; }
        public int? CategoryID { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? Status { get; set; }
        public string Salary { get; set; }
        public string Address { get; set; }
        public string Experience { get; set; }
        public string Position { get; set; }
        public string Degree { get; set; }
        public string WorkingForm { get; set; }
        public string NumberOfRecruits { get; set; }
        public bool? Gender { get; set; }
        public string Trades { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Url { get; set; }
        public string CategoryName { get; set; }
        public DateTime? DatePost { get; set; }
    }
    public class m_ArticleDetailNews
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string UrlImage { get; set; }
        public string Content { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Tag { get; set; }
    }
    public class m_NganhNgheKinhDoanh
    {
        public Guid? ID { get; set; }
        public string MaNganhNghe { get; set; }
        public string TenNganhNghe { get; set; }
    }
    public class m_CuaHangDangKy
    {
        public string SubDomain { get; set; }
        public string SoDienThoai { get; set; }
        public string TenTaiKhoan { get; set; }
        public string MatKhau { get; set; }
    }
    public class PageListDTO
    {
        public int TotalRecord { get; set; }
        public double PageCount { get; set; }
    }

    public class m_Category
    {
        public int ID { get; set; }
        public string text { get; set; }

        public object nodes { get; set; }
    }
    public class m_CategoryParent
    {
        public int parentID { get; set; }
        public string text { get; set; }

        public object nodes { get; set; }
    }
    //public class m_CategoryParent2
    //{
    //    public int? parentID { get; set; }
    //    public string text { get; set; }
    //}
    #endregion
}
