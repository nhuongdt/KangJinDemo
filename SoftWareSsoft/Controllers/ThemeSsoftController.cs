using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ssoft.Common.Common;
using SoftWareSsoft.Models.ThemeSsoft;
using SoftWareSsoft.Hellper;
using Model.Web.Service;
using Model.Web.API;
using Model.Web;
using System.Text;

namespace SoftWareSsoft.Controllers
{
    public class ThemeSsoftController : BaseController
    {
        public ThemeSsoftController()
        {
        }
        // GET: ThemeSsoft
        public ActionResult Index()
        {
            SetKeyWorkMeta();
            return View();
        }

        public ActionResult Product()
        {
            SetKeyWorkMeta();
            return View();
        }

        public ActionResult News()
        {
            SetKeyWorkMeta();
            return View();
        }
        public ActionResult NewsDetail(string title, int? keyId)
        {
            _NewsService.UpdateViews(keyId??0);
            var result = _NewsService.getAll().Where(o => o.ID == keyId).Select(o=>new NewsModel {
                Mota=o.Mota,
                NoiDung=o.NoiDung,
                TieuDe=o.TenBaiViet,
                NgayTao=o.NgayTao,
                NguoiTao=o.NguoiTao,
                MetaTitle=o.MetaTitle,
                MetaDescriptions = o.MetaDescriptions
            }).FirstOrDefault();
            if (result == null)
                return RedirectToAction("News");

            SetMetaSeo(result.MetaTitle, result.MetaDescriptions, string.Empty);
            return View(result);
        }

        public ActionResult Contact()
        {
          SetKeyWorkMeta();
            return View();
        }
        public ActionResult Customer()
        {
            SetKeyWorkMeta();
            return View();
        }
        public ActionResult CustomerDetail(string title, long? keyId)
        {
            var result = _CustomerService.getAll().Where(o => o.ID == keyId).Select(o => new NewsModel
            {
                Mota = o.Mota,
                NoiDung = o.NoiDung,
                TieuDe = o.TenKhachHang,
                NgayTao = o.NgayTao,
                NguoiTao = o.NguoiTao,
                MetaDescriptions=o.MetaDescription,
                MetaTitle=o.MetaTitle
            }).FirstOrDefault();
            if (result == null)
                return RedirectToAction("Customer");

            SetMetaSeo(result.MetaTitle, result.MetaDescriptions,string.Empty);
            return View(result);
        }
        public ActionResult Recruitment()
        {
            SetKeyWorkMeta();
            return View();
        }

      
        public ActionResult RecruitmentDetail(string title, long? keyId)
        {
            var model = _RecruitmentService.GetAll().Where(o => o.ID == keyId).AsEnumerable().Select(
            o => new RecruitmentModel
            {
                TieuDe = o.TieuDe,
                ConHan  = _RecruitmentService.CheckConHan(o.DenNgay),
                TinhThanh = _RecruitmentService.GetTinhThanh(o.MaTinhThanh),
                Mota=o.MoTa,
                MucLuong = _RecruitmentService.ConvertMucLuong(o.MucLuong),
                SoLuong=o.SoLuong,
                ThoiGian = o.TuNgay.ToString("dd/MM/yyyy") + " - " + o.DenNgay.ToString("dd/MM/yyyy"),
                NgayTao=o.NgayTao,
                NguoiTao=o.NguoiTao,
                ID_nhombaiviet=o.ID_NhomBaiViet,
                TenNhom=o.DM_NhomBaiViet!=null ?o.DM_NhomBaiViet.TenNhomBaiViet:string.Empty,
                MetaDescriptions = o.MetaDescription,
                MetaTitle = o.MetaTitle,
                ID =o.ID
                
            }
                ).FirstOrDefault();
            if (model == null)
                return RedirectToAction("Recruitment");

            SetMetaSeo(model.MetaTitle, model.MetaDescriptions, string.Empty);
            return View(model);
        }


        public ActionResult ProductDetail(int TypeId)
        {
            var model = new ProductDetailModel() { TypeSofware= TypeId };
            SetKeyWorkMeta();
            return View(model);
        }

        public ActionResult Introduce()
        {
            SetKeyWorkMeta();
            return View();
        }

        [ViewAuthorize]
        public ActionResult _Header()
        {
            return PartialView();
        }

        public ActionResult _Footer()
        {
            return PartialView();
        }

        //[ OutputCache(Duration = 86400)]
        //[ValidateInput(false)]
        //public ContentResult RobotsText()
        //{
        //    StringBuilder stringBuilder = new StringBuilder();

        //    stringBuilder.AppendLine("user-agent: *");
        //    stringBuilder.AppendLine("disallow: /Admin/");
        //    stringBuilder.AppendLine("allow: /Admin/Home");
        //    stringBuilder.Append("sitemap: ");
        //    stringBuilder.AppendLine(this.Url.RouteUrl("sitemap", null, this.Request.Url.Scheme).TrimEnd('/'));

        //    return this.Content(stringBuilder.ToString(), "text/plain", Encoding.UTF8);
        //}

        //public ActionResult SitemapXml()
        //{
        //    var sitemapNodes = GetSitemapNodes(this.Url);
        //    string xml = StaticVariable.GetSitemapDocument(sitemapNodes);
        //    return this.Content(xml, "text/xml", Encoding.UTF8);
        //}
        public List<SitemapNode> GetSitemapNodes(UrlHelper urlHelper)
        {
            List<SitemapNode> nodes = new List<SitemapNode>();
            nodes.Add(
              new SitemapNode()
              {
                  Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "HomeIndex"),
                  Priority = 1,
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
                   Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "lienhe"),
                   Priority = 0.80,
                   LastModified = DateTime.Now,
                   Frequency = SitemapFrequency.Weekly
               });
            nodes.Add(
               new SitemapNode()
               {
                   Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "tuyendung"),
                   Priority = 0.64,
                   LastModified = DateTime.Now,
                   Frequency = SitemapFrequency.Weekly
               });
            nodes.Add(
              new SitemapNode()
              {
                  Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "sanpham"),
                  Priority = 0.80,
                  LastModified = DateTime.Now,
                  Frequency = SitemapFrequency.Weekly
              });
            nodes.Add(
               new SitemapNode()
               {
                   Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "luckyhrm"),
                   Priority = 0.80,
                   LastModified = DateTime.Now,
                   Frequency = SitemapFrequency.Weekly
               });
            nodes.Add(
              new SitemapNode()
              {
                  Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "luckybeauty"),
                  Priority = 0.80,
                  LastModified = DateTime.Now,
                  Frequency = SitemapFrequency.Weekly
              });
            nodes.Add(
              new SitemapNode()
              {
                  Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "luckygara"),
                  Priority = 0.80,
                  LastModified = DateTime.Now,
                  Frequency = SitemapFrequency.Weekly
              });
            nodes.Add(
               new SitemapNode()
               {
                   Url = UrlHelperExtensions.AbsoluteRouteUrl(urlHelper, "gioithieu"),
                   Priority = 0.80,
                   LastModified = DateTime.Now,
                   Frequency = SitemapFrequency.Weekly
               });
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