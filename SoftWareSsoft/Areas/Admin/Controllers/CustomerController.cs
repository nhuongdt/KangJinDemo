using Model.Web.Service;
using SoftWareSsoft.Areas.Admin.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftWareSsoft.Areas.Admin.Controllers
{
    [RBACAuthorize]
    public class CustomerController : Controller
    {
        private CustomerService _CustomerService = new CustomerService();
        // GET: Admin/Customer
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Customer()
        {
            return View();
        }
        public ActionResult EditCustomer(int? id)
        {
            ViewBag.KeyId = id;
            ViewBag.Tags = string.Join(",", _CustomerService.GetTagsNews(id ?? 0).Where(o => o.DM_Tags != null).AsEnumerable().Select(o => o.DM_Tags.TenTheTag).ToList());
            return View();
        }
    }
}