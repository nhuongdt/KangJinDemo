using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using System.IO;
using System.Web.Configuration;

namespace banhang24.AppCache
{
    public static class EventUpdateCache
    {
        /// <summary>
        /// Tạo mỗi subdomain 1 app cache
        /// </summary>
        public static void CreatFIleAppcache()
        {
            var subDomain = CookieStore.GetCookieAes("SubDomain");

            var shop = CookieStore.GetCookieAes("shop").ToUpper();
            var fileRead = string.Empty;
            var typeShop = string.Empty;

            // check type Shop
            switch (shop)
            {
                case "C1D14B5A-6E81-4893-9F73-E11C63C8E6BC": // nha hang
                    typeShop = "HoaDon";
                    fileRead = "/AppCache/HoaDon.appcache";
                    break;
                //case "AC9DF2ED-FF08-488F-9A64-08433E541020": // spa
                //case "83894499-AEFA-4F58-96B4-5EC1A0B16A76":
                //    typeShop = "Spa";
                //    fileRead = "/AppCache/Spa.appcache"; ; // not create
                //    break;
                default: // ban le
                    typeShop = "BanLe";
                    fileRead = "/AppCache/BanLe.appcache"; 
                    break;
            }

            var path = "/AppCache/CacheSubDomain/" + subDomain + "/" + typeShop + "_manifest.appcache";

            var mapPathFile = HttpContext.Current.Server.MapPath(path);
            var file = HttpContext.Current.Server.MapPath(fileRead);

            if (!Directory.Exists(HttpContext.Current.Server.MapPath("/AppCache/CacheSubDomain/" + subDomain + "/")))
            {
                // create folder
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("/AppCache/CacheSubDomain/" + subDomain + "/"));
                // Create a file to write to.
                if (System.IO.File.Exists(file))
                {
                    var text = System.IO.File.ReadAllText(file);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        using (StreamWriter sw = File.CreateText(mapPathFile))
                        {
                            sw.Write(text);
                            sw.Dispose();
                        }
                    }
                }
            }
            else// Cập nhật file
            {
                // check file exist
                if (!File.Exists(mapPathFile))// lỗi do chưa tạo được file => Tạo mới
                {
                    // Create a file to write to.
                    if (System.IO.File.Exists(file))
                    {
                        var text = System.IO.File.ReadAllText(file);
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            using (StreamWriter sw = File.CreateText(mapPathFile))
                            {
                                sw.Write(text);
                                sw.Dispose();
                            }
                        }
                    }
                }
                else //Cập nhật
                {
                    // Create a file to write to.
                    if (System.IO.File.Exists(file))
                    {
                        var textNew = System.IO.File.ReadAllLines(file);
                        if (textNew.Any())
                        {
                            if (!string.IsNullOrWhiteSpace(System.IO.File.ReadAllText(mapPathFile))) // File khách hàng có dữ liệu
                            {
                                //if (textNew[1].Split('_').Length >= 2)
                                //{
                                //    long NumberVersion = long.Parse(textNew[1].Split('_')[1].Trim());
                                //    textNew[1] = (string.Format("#Version _ {0} _ {1}", ++NumberVersion, DateTime.Now.ToString("dd-MM-yyyy")));

                                //    // update version for cache in file main
                                //    System.IO.File.WriteAllLines(file, textNew.ToArray());
                                //}
                                var textOld = System.IO.File.ReadAllLines(mapPathFile);
                                if ( !textOld[2].Equals(textNew[2]))
                                {
                                    // Nếu file js chung có thay đổi cập nhật lại thông tin file cache cho khách
                                   

                                        var listDelete = new List<string>();
                                        for (int i = 0; i < textOld.Length; i++)
                                        {
                                            if (textOld[i].Equals("#EndCacheMain"))
                                            {
                                                listDelete.Add(textOld[i]);
                                                break;
                                            }
                                            else
                                            {
                                                listDelete.Add(textOld[i]);
                                            }
                                        }
                                        System.IO.File.WriteAllLines(mapPathFile, textNew.Concat(textOld.Where(o => !listDelete.Contains(o)).ToArray()).ToArray());
                                    }
                                
                            }
                            else// File khách hàng chưa có dữ liệu
                            {
                                System.IO.File.WriteAllLines(mapPathFile, textNew.ToArray());
                            }
                        }
                    }
                }
                // overwrite
            }
        }

        /// <summary>
        /// Thêm mới file vào cahce
        /// </summary>
        /// <param name="path"> Đường dẫn tới file cache dùng "Server.MapPath(pathName)"</param>
        /// <param name="pathImage">Định dạng "/ đường dẫn/ tên ảnh"</param>
        public static void AddFileImeagesCache(string path, string pathImage)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.AppendAllLines(path, new string[] { pathImage });
            }
        }
        

        /// <summary>
        ///  Cập nhật lại file trong cache
        /// </summary>
        /// <param name="pathImageNew"> Đường dẫn ảnh mới</param>
        /// <param name="pathImageOld"> đường dẫn ảnh cũ</param>
        public static void UpdateFileImeagesCache(string path, string pathImageNew, string pathImageOld)
        {
            if (System.IO.File.Exists(path))
            {
                var text = System.IO.File.ReadAllLines(path).ToList().Where(o => !o.Equals(pathImageOld)).ToList();
                text.Add(pathImageNew);
                System.IO.File.WriteAllLines(path, text.ToArray());
            }
        }

        /// <summary>
        /// Xóa file trong cache
        /// </summary>
        /// <param name="path"> Đường dẫn tới file cache</param>
        /// <param name="pathImage">Định dạng "/ đường dẫn/ tên ảnh"</param>
        public static void DeleteFileImeagesCache(string path, string pathImage)
        {
            if (System.IO.File.Exists(path))
            {
                string[] text = System.IO.File.ReadAllLines(path).Where(o => !o.Equals(pathImage)).ToArray();
                System.IO.File.WriteAllLines(path, text);
            }
        }

        /// <summary>
        ///  lấy ra file path của từng subdomain
        /// </summary>
        /// <param name="html"></param>
        /// <returns>/AppCache/CacheSubDomain/subdomain/HoaDon|Spa|BanLemanifest.appcache</returns>
        public static string WriteStringCache(this HtmlHelper html)
        {
            var subDomain = CookieStore.GetCookieAes("SubDomain");
            var shop = CookieStore.GetCookieAes("shop").ToUpper();
            var file = string.Empty;
            switch (shop)
            {
                case "C1D14B5A-6E81-4893-9F73-E11C63C8E6BC": // nha hang
                    file = "HoaDon";
                    break;
                //case "AC9DF2ED-FF08-488F-9A64-08433E541020": // spa
                //case "83894499-AEFA-4F58-96B4-5EC1A0B16A76":
                //    file = "Spa";
                //    break;
                default: // ban le
                    file = "BanLe";
                    break;
            }
            return "/AppCache/CacheSubDomain/" + subDomain + "/" + file + "_manifest.appcache";
        }

        
        public static string WriteStringLinkOptinform(this HtmlHelper html,string subdomain,string folderfile)
        {
            var result = "https://" + subdomain + ".open24.vn" + folderfile;
            var test = "https://localhost:44382" + folderfile;
            var test1 = "http://" + subdomain + ".open24.test" + folderfile;
            return test;
        }

        public static string WriteRenderFile(this HtmlHelper html, IHtmlString file)
        {
            var FileRender = file.ToString().Split('?');
            return FileRender[0]+"?v="+ WebConfigurationManager.AppSettings["VersionCache"].ToString();
        }
    }
}