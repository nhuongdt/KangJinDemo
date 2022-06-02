using Model_banhang24vn.Cache;
using Model_banhang24vn.Common;
using Model_banhang24vn.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
 
namespace Open24.Areas.AdminPage.Hellper
{
    public class ScheduleTimer
    {
        private static Timer aTimer;
        private static long time = 1000 * 60 * 5;//5 phút
        public static void SetTimer()
        {
            aTimer = new System.Timers.Timer(time);
            aTimer.Elapsed += UpdateStatusPost;
            aTimer.Enabled = true;
        }

        private static void UpdateStatusPost(Object source, ElapsedEventArgs e)
       {
            aTimer.Stop();
            aTimer.Dispose();
            var _NewPostService = new NewPostService();

            // lấy danh sách các bài đăng hôm nay
            var ListTime = _NewPostService.Query.Where(o => o.DatePost != null
                                                             && o.DatePost.Value <= DateTime.Now
                                                            && (o.Status == false || o.Status == null)).OrderBy(o => o.DatePost).FirstOrDefault();

            if (ListTime != null)
            {
                _NewPostService.UpDatePostSchedule(ListTime.ID);
                var cache = new CacheHelper();
                cache.Invalidate(CacheKey.News_Home_Slider);
                cache.Invalidate(CacheKey.News_NewDate);
            }
            SetTimer();
        }
    }
}