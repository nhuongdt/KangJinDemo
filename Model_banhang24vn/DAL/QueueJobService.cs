using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
    public class QueueJobService
    {
        BanHang24vnContext db;
        //public QueueJobService()
        //{
        //    //db = new BanHang24vnContext();
        //}

        public void Insert(QueueJob qj)
        {
            try
            {
                using (db = new BanHang24vnContext())
                {
                    db.QueueJobs.Add(qj);
                    db.SaveChanges();
                }
            }
            catch
            { }
        }
    }
}
