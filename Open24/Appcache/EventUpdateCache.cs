using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using System.IO;
 
namespace Open24.Appcache
{
    public static class EventUpdateCache
    {
       

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

   
    }
}