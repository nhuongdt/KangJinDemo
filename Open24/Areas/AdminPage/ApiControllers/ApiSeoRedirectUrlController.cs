using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model_banhang24vn.DAL;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.Common;
using Model_banhang24vn;
using Open24.Areas.AdminPage.Hellper;

namespace Open24.Areas.AdminPage.ApiControllers
{
    public class ApiSeoRedirectUrlController : ApiBaseController
    {
        private readonly SeoRedirectUrlService _SeoRedirectUrlService;
        public ApiSeoRedirectUrlController()
        {
            _SeoRedirectUrlService = new SeoRedirectUrlService();
            
        }
        [HttpPost]
        public IHttpActionResult SearchGrid(DataGridView model)
        {
            try
            {
                var data = _SeoRedirectUrlService.SearchText(model.Search);
                model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
                if (model.PageCount == 0 || model.PageCount == 1)
                {
                    model.PageCount = 1;
                    model.Page = 1;
                    model.Data = data.ToList();
                }
                else
                {
                    model.Data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit).ToList();
                }
                model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, new {
                    pageview=model.PageItem,
                    pagenow=model.Page,
                    data=model.Data,
                    countpage=model.PageCount,
                    isprev=model.Page>1,
                    isnext=model.PageCount>model.Page

                });
            }
            catch (Exception ex)
            {

                return Exeption(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult InsertSeoUrl(SeoRedirectUrl model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được bản ghi cần thêm mới");
                }
                 else if (!model.UrlOld.Contains(System.Configuration.ConfigurationManager.AppSettings["Webhttp"].ToString()))
                {
                    return ActionFalseNotData("url trỏ về không đúng định dạng open24");
                }
                model.CreatedBy = contant.SESSIONNGUOIDUNG.UserName;
                model.CreateDate = DateTime.Now;
                if (_SeoRedirectUrlService.Insert(model))
                {
                    return InsertSuccess();
                }
                return ActionFalseNotData("Url mới đã bị trùng vui lòng kiểm tra lại");
            }
            catch (Exception ex)
            {

                return Exeption(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult UpdateSeoUrl(SeoRedirectUrl model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được bản ghi cần cập nhật");
                }
                else if (!model.UrlOld.Contains(System.Configuration.ConfigurationManager.AppSettings["Webhttp"].ToString()))
                {
                    return ActionFalseNotData("url trỏ về không đúng định dạng open24");
                }
                if (_SeoRedirectUrlService.Update(model))
                {
                    return UpdateSuccess();
                }
                return ActionFalseNotData("Url mới đã bị trùng vui lòng kiểm tra lại");
            }
            catch (Exception ex)
            {

                return Exeption(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult DeleteSeoUrl(SeoRedirectUrl model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được bản ghi cần xóa");
                }
                
                if (_SeoRedirectUrlService.Delete(model))
                {
                    return DeleteSuccess();
                }
                return ActionFalseNotData("Bản ghi không tông tại hoặc đã bị xóa.");
            }
            catch (Exception ex)
            {

                return Exeption(ex);
            }
        }
    }
}
