using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Configuration;
using LogicalCK;

namespace CkEditorUpload.Controllers
{
    public class RichContentController : Controller
    {
		public ViewResult FileBrowser()
		{
			return View();
		}

		public ViewResult ImageViewer()
		{
			return View();
		}

		public ViewResult Thumb()
		{
			return View();
		}
         
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult UploadImage(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
		{

			//http://stackoverflow.com/a/4088194/167670
			//http://arturito.net/2010/11/03/file-and-image-upload-with-asp-net-mvc2-with-ckeditor-wysiwyg-rich-text-editor/
			//http://haacked.com/archive/2010/07/16/uploading-files-with-aspnetmvc.aspx

			if (upload.ContentLength <= 0)
				return null;

			// here logic to upload image
			// and get file path of the image

			var uploadFolder = WebConfigurationManager.AppSettings["LogicalUpload.Directory"];
			var uploadDir = new DirectoryInfo(Server.MapPath(uploadFolder));

			var fileName = Path.GetFileName(upload.FileName);
			fileName = ImageHelper.ScrubBadUrlCharacters(fileName);
			fileName = uploadDir.GetUniqueFileName(fileName);

			var path = Path.Combine(Server.MapPath(uploadFolder), fileName);
			upload.SaveAs(path);

			//var url = string.Format("{0}{1}{2}/{3}", Request.Url.GetLeftPart(UriPartial.Authority),
			//	Request.ApplicationPath == "/" ? string.Empty : Request.ApplicationPath,
			//	uploadFolder.Replace("~", ""), fileName);
			var url = string.Format("{0}/{1}", uploadFolder.Replace("~", ""), fileName);

			// passing message success/failure
			const string message = "Image was saved correctly";

			// since it is an ajax request it requires this string
			var output = string.Format(
				"<html><body><script>window.parent.CKEDITOR.tools.callFunction({0}, \"{1}\", \"{2}\");</script></body></html>",
				CKEditorFuncNum, url, message);

			return Content(output);
		}
    }
}