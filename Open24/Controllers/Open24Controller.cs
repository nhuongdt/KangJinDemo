using Model_banhang24vn;
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView.Client;
using Model_banhang24vn.DAL;
using Open24.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Open24.Controllers
{
    public class Open24Controller : BaseController
    {
        // GET: Open24
        private News_UserService _News_UserService = new News_UserService();
        [ViewAuthorize]
        public ActionResult Index()
        {
            setkeywordsTitle();
            return View();
        }
        [ViewAuthorize]
        public ActionResult KhachHang()
        {
            setkeywordsTitle();
            return View();
        }

        public ActionResult GioiThieu()
        {
            setkeywordsTitle();
            return View();
        }

        [ViewAuthorize]
        public ActionResult HoTroPhanMem()
        {
            setkeywordsTitle();
            return View();
        }

        [ViewAuthorize]
        public ActionResult TinTuc()
        {
            setkeywordsTitle();
            return View();
        }

        [ViewAuthorize]
        public ActionResult LandingPage()
        {
            return View();
        }
        [ViewAuthorize]
        public ActionResult TinTucGroup(string Category)
        {
            var url = "/blog/" + Category;
            var category = new GroupPostService().Query.FirstOrDefault(o => o.Url == url);
            if (category == null) return RedirectToAction("TinTuc");
            setkeywords(category.Description, string.Format("{0},{1},{2}", Notification.TagsTinTuc, category.Name, StaticVariable.RemoveSign4VietnameseString(category.Name)));
            ViewBag.CategoryId = category.ID;
            ViewBag.Title = category.Name;
            return View();
        }

        [ViewAuthorize]
        public ActionResult TinTucTags(string tagId)
        {
            var tag = new NewPostService().GetTagById(tagId);
            var menuTag = new MenuTagsService().GetByAction(Request.RequestContext.RouteData.Values["controller"].ToString(), "tintuc");
            if (tag != null)
            {
                var keywork = (menuTag != null ? menuTag.Tags + "," + StaticVariable.RemoveSign4VietnameseString(menuTag.Tags) : string.Empty) + "," + tag.Name + "," + tag.KeyWords;
                var Description = (menuTag != null ? menuTag.Description : string.Empty) + "," + tag.Name + "," + tag.KeyWords;
                setkeywords(Description, keywork);
                ViewBag.Title = tag.Name;
                return View(tag);
            }
            return RedirectToAction("TinTuc");
        }

        [ViewAuthorize(Title = "/tin-tuc/chi-tiet")]
        public ActionResult TinTucChiTiet(string title)
        {
            if (title == null)
            {
                return RedirectToAction("TinTuc");
            }
            var array = title.Split('-');
            int keyId = -1;
            int.TryParse(array[(array.Length - 1)], out keyId);
            var _M_News_Post = new M_News_Post();
            var model = new newsDetailView();
            model.NewsModel = _M_News_Post.GetArticleNewsDetailUpdateView(title, keyId );
            if (model.NewsModel == null)
            {
                return RedirectToAction("TinTuc");
            }
            ViewBag.CreateDate = model.NewsModel.CreateDate.HasValue ? model.NewsModel.CreateDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            var user = _News_UserService.Getbykey(model.NewsModel.CreatedBy ?? new Guid());
            if (user != null)
            {
                model.CreateByUser = user.Name;
            }
            model.ListRlatedArticles = _M_News_Post.GetListRlatedArticles(model.NewsModel.CategoryID ?? -1, model.NewsModel.Tag, model.NewsModel.ID);
            var tag = new NewPostService().GetTagsByArticle(keyId);
            ViewBag.Tags = tag;
            var menuTag = new MenuTagsService().GetMetaTags(Request.Url.AbsoluteUri);
            if (tag.Any())
            {
                setkeywords(model.NewsModel.Summary, string.Format("{0}, {1}, {2}",
                    menuTag != null ? menuTag.Tags  : Notification.TagsTinTuc,
                    string.Join(",", tag.Select(o => o.Name)),
                    string.Join(",", tag.Select(o => o.KeyWords))));
            }
            else
            {
                setkeywords(model.NewsModel.Summary, menuTag != null ? menuTag.Tags: Notification.TagsTinTuc);
            }
            model.CategoriesModel = new GroupPostService().Query.FirstOrDefault(o => o.ID == model.NewsModel.CategoryID);
            return View(model);
        }

        [ViewAuthorize(Title = "/khach-hang/chi-tiet")]
        public ActionResult KhachHangChiTiet( string title)
        {
            if (title == null)
            {
                return RedirectToAction("KhachHang");
            }
            var array = title.Split('-');
            int keyId = -1;
            int.TryParse(array[(array.Length - 1)], out keyId);
            ViewBag.CustomerId = keyId;
            var data = new CustomerService().GetByDetail(keyId);
            if (data == null)
            {
                return RedirectToAction("KhachHang");
            }
            ViewBag.httpUrl = Request.Url.AbsoluteUri;
            ViewBag.TenKhachHang = data.Name;
            setkeywords(data.Note, data.Name + "," + StaticVariable.RemoveSign4VietnameseString(data.Name));
            return View();
        }

        [ViewAuthorize]
        public ActionResult NhanSu()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.NhanSu;
            return View();
        }

        [ViewAuthorize]
        public ActionResult GaraOto()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.OtoXeMay;
            return View();
        }

        [ViewAuthorize]
        public ActionResult NhaHang()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.NhaHangCafe;
            return View();
        }

        [ViewAuthorize]
        public ActionResult NhaThuoc()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.NhaThuoc;
            return View();
        }

        [ViewAuthorize]
        public ActionResult PhuTung()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.PhuTung;
            return View();
        }

        [ViewAuthorize]
        public ActionResult Spa()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.SpaTMV;
            return View();
        }

        [ViewAuthorize]
        public ActionResult Salon()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.SalonGym;
            return View();
        }

        [ViewAuthorize]
        public ActionResult PhongKham()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.PhongKham;
            return View();
        }

        [ViewAuthorize]
        public ActionResult SieuThi()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.SieuThi;
            return View();
        }

        [ViewAuthorize]
        public ActionResult MyPham()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.MyPham;
            return View();
        }

        [ViewAuthorize]
        public ActionResult VanPhongPham()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.VanPhongPham;
            return View();
        }

        [ViewAuthorize]
        public ActionResult NongSan()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.NongSanTP;
            return View();
        }

        [ViewAuthorize]
        public ActionResult DoChoi()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.DoChoiTreEm;
            return View();
        }

        [ViewAuthorize]
        public ActionResult NoiThat()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.NoiThat;
            return View();
        }

        [ViewAuthorize]
        public ActionResult ThoiTrang()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.ThoiTrangPK;
            return View();
        }

        [ViewAuthorize]
        public ActionResult LinhVucKhac()
        {
            setkeywordsTitle();
            ViewBag.NganhNghe = MaNganhNgheKinhDoanh.Other;
            return View();
        }

        [ViewAuthorize]
        public ActionResult thanhcong()
        {
            setkeywordsTitle();
            return View();
        }

        [ViewAuthorize]
        public ActionResult hoptac()
        {
            setkeywordsTitle();
            return View();
        }

        public List<SitemapNode> GetSitemapNodes(UrlHelper urlHelper)
        {
            List<SitemapNode> nodes = new List<SitemapNode>();
            nodes.Add(
              new SitemapNode()
              {
                  Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "HomeIndex"),
                  Priority = 1.00,
                  LastModified = DateTime.Now,
                  Frequency = SitemapFrequency.Weekly
              });

            nodes.Add(
                new SitemapNode()
                {
                    Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "tintuc"),
                    Priority = 0.80,
                    LastModified = DateTime.Now,
                    Frequency = SitemapFrequency.Weekly
                });
            nodes.Add(
               new SitemapNode()
               {
                   Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "khachhang"),
                   Priority = 0.80,
                   LastModified = DateTime.Now,
                   Frequency = SitemapFrequency.Weekly
               });


            nodes.Add(
               new SitemapNode()
               {
                   Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "nhahang"),
                   Priority = 0.80,
                   LastModified = DateTime.Now,
                   Frequency = SitemapFrequency.Weekly
               });
            nodes.Add(
              new SitemapNode()
              {
                  Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "spathammy"),
                  Priority = 0.80,
                  LastModified = DateTime.Now,
                  Frequency = SitemapFrequency.Weekly
              });

            nodes.Add(
              new SitemapNode()
              {
                  Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "salon"),
                  Priority = 0.80,
                  LastModified = DateTime.Now,
                  Frequency = SitemapFrequency.Weekly
              });

            nodes.Add(
              new SitemapNode()
              {
                  Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "thoitrang"),
                  Priority = 0.80,
                  LastModified = DateTime.Now,
                  Frequency = SitemapFrequency.Weekly
              });
            nodes.Add(
             new SitemapNode()
             {
                 Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "nhathuoc"),
                 Priority = 0.80,
                 LastModified = DateTime.Now,
                 Frequency = SitemapFrequency.Weekly
             });
            nodes.Add(
             new SitemapNode()
             {
                 Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "phongkham"),
                 Priority = 0.80,
                 LastModified = DateTime.Now,
                 Frequency = SitemapFrequency.Weekly
             });
            nodes.Add(
             new SitemapNode()
             {
                 Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "sieuthi"),
                 Priority = 0.80,
                 LastModified = DateTime.Now,
                 Frequency = SitemapFrequency.Weekly
             });
            nodes.Add(
             new SitemapNode()
             {
                 Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "noithat"),
                 Priority = 0.80,
                 LastModified = DateTime.Now,
                 Frequency = SitemapFrequency.Weekly
             });
            nodes.Add(
             new SitemapNode()
             {
                 Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "vanphongpham"),
                 Priority = 0.80,
                 LastModified = DateTime.Now,
                 Frequency = SitemapFrequency.Weekly
             });
            nodes.Add(
             new SitemapNode()
             {
                 Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "mypham"),
                 Priority = 0.80,
                 LastModified = DateTime.Now,
                 Frequency = SitemapFrequency.Weekly
             });
            nodes.Add(
             new SitemapNode()
             {
                 Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "dochoi"),
                 Priority = 0.80,
                 LastModified = DateTime.Now,
                 Frequency = SitemapFrequency.Weekly
             });
            nodes.Add(
             new SitemapNode()
             {
                 Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "phuongtien"),
                 Priority = 0.80,
                 LastModified = DateTime.Now,
                 Frequency = SitemapFrequency.Weekly
             });
            nodes.Add(
             new SitemapNode()
             {
                 Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "phutung"),
                 Priority = 0.80,
                 LastModified = DateTime.Now,
                 Frequency = SitemapFrequency.Weekly
             });
            nodes.Add(
             new SitemapNode()
             {
                 Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "nongsan"),
                 Priority = 0.80,
                 LastModified = DateTime.Now,
                 Frequency = SitemapFrequency.Weekly
             });
            nodes.Add(
             new SitemapNode()
             {
                 Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "Other"),
                 Priority = 0.80,
                 LastModified = DateTime.Now,
                 Frequency = SitemapFrequency.Weekly
             });
            nodes.Add(
             new SitemapNode()
             {
                 Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "gioitieukhachhang"),
                 Priority = 0.80,
                 LastModified = DateTime.Now,
                 Frequency = SitemapFrequency.Weekly
             });
            //nodes.Add(
            // new SitemapNode()
            // {
            //     Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "nongsan"),
            //     Priority = 0.80,
            //     LastModified = DateTime.Now,
            //     Frequency = SitemapFrequency.Weekly
            // });
            //nodes.Add(
            // new SitemapNode()
            // {
            //     Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "DangKy"),
            //     Priority = 0.80,
            //     LastModified = DateTime.Now,
            //     Frequency = SitemapFrequency.Weekly
            // });
            //nodes.Add(
            // new SitemapNode()
            // {
            //     Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "hotrophanmem"),
            //     Priority = 0.80,
            //     LastModified = DateTime.Now,
            //     Frequency = SitemapFrequency.Weekly
            // });
            //nodes.Add(
            // new SitemapNode()
            // {
            //     Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "giadung"),
            //     Priority = 0.80,
            //     LastModified = DateTime.Now,
            //     Frequency = SitemapFrequency.Weekly
            // });
            return nodes;
        }
    }

    public static class UrlHelperExtensions
    {
        public static string AbsoluteRouteUrl(
            this UrlHelper urlHelper,
            string routeName,
            object routeValues = null)
        {
            string scheme = urlHelper.RequestContext.HttpContext.Request.Url.Scheme;
            return urlHelper.RouteUrl(routeName, routeValues, scheme);
        }
    }
}