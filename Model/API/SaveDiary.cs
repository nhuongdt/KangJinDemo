using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Model
{
    public class SaveDiary
    {
        public static string add_Diary(HT_NhatKySuDung objNhatKySuDung)
        {
            string erStr = string.Empty;

            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    db.Database.CommandTimeout = 60 * 60;
                    db.HT_NhatKySuDung.Add(objNhatKySuDung);
                    db.SaveChanges();
                    if (objNhatKySuDung.ID_HoaDon != null)
                    {
                        new SaveDiary().AddQueueJob(objNhatKySuDung);
                        //    BH_HoaDon hd = db.BH_HoaDon.Where(p => p.ID == objNhatKySuDung.ID_HoaDon.Value).FirstOrDefault();

                        //    List<SqlParameter> paramlistluyke = new List<SqlParameter>();
                        //    paramlistluyke.Add(new SqlParameter("IDHoaDonInput", hd.ID));
                        //    paramlistluyke.Add(new SqlParameter("IDChiNhanhInput", hd.ID_DonVi));
                        //    paramlistluyke.Add(new SqlParameter("ThoiGian", hd.LoaiHoaDon == 10 && (hd.YeuCau == "4" || hd.YeuCau == "3") && hd.NgaySua != null ? hd.NgayLapHoaDon : objNhatKySuDung.ThoiGianUpdateGV));
                        //    paramlistluyke.Add(new SqlParameter("Loai", objNhatKySuDung.LoaiNhatKy));
                        //    db.Database.ExecuteSqlCommand("exec UpdateTonLuyKeTheoHoaDon @IDHoaDonInput, @IDChiNhanhInput, @ThoiGian, @Loai", paramlistluyke.ToArray());

                        //    if (hd.LoaiHoaDon == 10 && (hd.YeuCau == "4" || hd.YeuCau == "3") && hd.NgaySua != null)
                        //    {
                        //        List<SqlParameter> paramlistluyke1 = new List<SqlParameter>();
                        //        paramlistluyke1.Add(new SqlParameter("IDHoaDonInput", hd.ID));
                        //        paramlistluyke1.Add(new SqlParameter("IDChiNhanhInput", hd.ID_CheckIn));
                        //        paramlistluyke1.Add(new SqlParameter("ThoiGian", hd.NgaySua));
                        //        paramlistluyke1.Add(new SqlParameter("Loai", objNhatKySuDung.LoaiNhatKy));
                        //        db.Database.ExecuteSqlCommand("exec UpdateTonLuyKeTheoHoaDon @IDHoaDonInput, @IDChiNhanhInput, @ThoiGian, @Loai", paramlistluyke1.ToArray());
                        //    }
                    }
                }
                //string str = CookieStore.GetCookieAes("SubDomain");

                //Task.Run(() => TaskUpdateAll(str, objNhatKySuDung));
                ////updatekiemke.Start();
            }
            catch (Exception ex)
            {
                erStr = ex.Message + ex.InnerException;
                CookieStore.WriteLog(string.Concat("ErrorWhenaddNKSD: " , objNhatKySuDung.ID_HoaDon , objNhatKySuDung.LoaiHoaDon,  ex.Message , ex.InnerException));
            }

            return erStr;
        }

        public void AddQueueJob(HT_NhatKySuDung nky)
        {
            try
            {
                Model_banhang24vn.DAL.QueueJobService queueJobService = new Model_banhang24vn.DAL.QueueJobService();
                Model_banhang24vn.QueueJob qj = new Model_banhang24vn.QueueJob();
                string subdomain = CookieStore.GetCookieAes("SubDomain");
                qj.ID = Guid.NewGuid();
                qj.IDNhatKySuDung = nky.ID;
                qj.SoLanDaChay = 0;
                qj.Subdomain = subdomain;
                qj.ThoiGianTao = DateTime.Now;
                qj.TrangThai = 0;
                queueJobService.Insert(qj);
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://qj.open24.vn/");
                //httpClient.BaseAddress = new Uri("https://localhost:44309/");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.GetAsync("api/Queues/AddQueue/" + subdomain);
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("AddQueueJob: " + ex.InnerException + ex.Message);
            }
        }


        // chi save Nhat ky, don't update TonKho, GiaVon
        public static string Add_ListDiary(List<HT_NhatKySuDung> lstObj)
        {
            string erStr = string.Empty;

            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    db.HT_NhatKySuDung.AddRange(lstObj);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                erStr = "Add_ListDiary: " + ex.Message + ex.InnerException;
                CookieStore.WriteLog(erStr);
            }
            return erStr;
        }

        public async static Task<string> TaskUpdateAll(string str, HT_NhatKySuDung objNhatKySuDung)
        {
            SystemDBContext systemDBContext = new SystemDBContext();
            using (SsoftvnContext db = systemDBContext.GetDBContextNonStatic(str))
            {
                db.Database.CommandTimeout = 60 * 60;
                Task<string> strUpdatekk = UpdateKiemKeByHoaDon(str, objNhatKySuDung, db);
                Task<string> strUpdateTon = UpdateTonByHoaDon(str, objNhatKySuDung, db);
                Task<string> strUpdateGiaVon = UpdateGiaVonByHoaDon(str, objNhatKySuDung, db);
                await Task.WhenAll(strUpdatekk, strUpdateTon, strUpdateGiaVon);
                return string.Concat(strUpdatekk, strUpdateTon, strUpdateGiaVon);
                //CookieStore.WriteLog("Hoàn thành update kiểm kê, tồn kho, giá vốn", str);
            }
        }

        public static DateTime ReturnTimeUpDateGiaVon(DateTime ngaylaphdold, DateTime ngaynew)
        {
            DateTime NgayHDEdit = ngaynew;
            if (ngaynew < ngaylaphdold)
            {
                NgayHDEdit = ngaynew;
            }
            else
            {
                NgayHDEdit = ngaylaphdold;
            }
            return NgayHDEdit;
        }

        public async static Task<string> UpdateKiemKeByHoaDon(string str, HT_NhatKySuDung objNhatKySuDung, SsoftvnContext db)
        {
            try
            {
                //db.Database.Connection.Open();
                //db.Database.CommandTimeout = 60 * 60;
                if (objNhatKySuDung.ID_HoaDon != null)
                {
                    BH_HoaDon hd = db.BH_HoaDon.Where(p => p.ID == objNhatKySuDung.ID_HoaDon.Value).FirstOrDefault();
                    switch (objNhatKySuDung.LoaiHoaDon)
                    {
                        case 1:
                        case 4:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 18:
                            List<SqlParameter> paramlist = new List<SqlParameter>();
                            paramlist.Add(new SqlParameter("IDHoaDonInput", hd.ID));
                            paramlist.Add(new SqlParameter("IDChiNhanhInput", hd.ID_DonVi));
                            paramlist.Add(new SqlParameter("ThoiGian", ReturnTimeUpDateGiaVon(objNhatKySuDung.ThoiGianUpdateGV.Value, hd.NgayLapHoaDon)));
                            db.Database.ExecuteSqlCommand("exec UpdateKiemKeTuHoaDon @IDHoaDonInput, @IDChiNhanhInput, @ThoiGian", paramlist.ToArray());
                            break;
                        case 10:
                            switch (hd.YeuCau)
                            {
                                case "1":
                                    List<SqlParameter> paramlist2 = new List<SqlParameter>();
                                    paramlist2.Add(new SqlParameter("IDHoaDonInput", hd.ID));
                                    paramlist2.Add(new SqlParameter("IDChiNhanhInput", hd.ID_DonVi));
                                    paramlist2.Add(new SqlParameter("ThoiGian", objNhatKySuDung.ThoiGianUpdateGV));
                                    db.Database.ExecuteSqlCommand("exec UpdateKiemKeTuHoaDon @IDHoaDonInput, @IDChiNhanhInput, @ThoiGian", paramlist2.ToArray());
                                    break;
                                case "3":
                                case "4":
                                    if (hd.NgaySua != null)
                                    {
                                        List<SqlParameter> paramlist3 = new List<SqlParameter>();
                                        paramlist3.Add(new SqlParameter("IDHoaDonInput", hd.ID));
                                        paramlist3.Add(new SqlParameter("IDChiNhanhInput", hd.ID_CheckIn));
                                        paramlist3.Add(new SqlParameter("ThoiGian", hd.NgaySua));
                                        db.Database.ExecuteSqlCommand("exec UpdateKiemKeTuHoaDon @IDHoaDonInput, @IDChiNhanhInput, @ThoiGian", paramlist3.ToArray());
                                    }
                                    List<SqlParameter> paramlist1 = new List<SqlParameter>();
                                    paramlist1.Add(new SqlParameter("IDHoaDonInput", hd.ID));
                                    paramlist1.Add(new SqlParameter("IDChiNhanhInput", hd.ID_DonVi));
                                    paramlist1.Add(new SqlParameter("ThoiGian", hd.NgayLapHoaDon));
                                    db.Database.ExecuteSqlCommand("exec UpdateKiemKeTuHoaDon @IDHoaDonInput, @IDChiNhanhInput, @ThoiGian", paramlist1.ToArray());
                                    break;
                            }
                            break;
                    }
                }
                return await Task.FromResult("");
            }
            catch (Exception ex)
            {
                CookieStore.WriteLogRunTask("Update kiểm kê by IDHoaDon: ----Begin InnerException----: idhoadon: " + objNhatKySuDung.ID_HoaDon + ex.InnerException + "----End InnerException----. ----Begin Message----: " + ex.Message + "----End Message----", str);
                return await Task.FromResult(ex.Message);
            }
        }

        public async static Task<string> UpdateTonByHoaDon(string str, HT_NhatKySuDung objNhatKySuDung, SsoftvnContext db)
        {
            int i = 0;
            while (i < 3)
            {
                try
                {
                    //db.Database.Connection.Open();
                    //db.Database.CommandTimeout = 60 * 60;
                    if (objNhatKySuDung.ID_HoaDon != null)
                    {
                        List<SqlParameter> paramlist = new List<SqlParameter>();
                        paramlist.Add(new SqlParameter("IDHoaDonInput", objNhatKySuDung.ID_HoaDon));
                        db.Database.ExecuteSqlCommand("exec UpdateTonForDM_hangHoa_TonKho @IDHoaDonInput", paramlist.ToArray());
                    }
                    //return await Task.FromResult("");
                    break;
                }
                catch (Exception ex)
                {
                    i++;
                    CookieStore.WriteLogRunTask("Update tồn kho by IDHoaDon retry " + i + ": ----Begin InnerException----: idhoadon: " + objNhatKySuDung.ID_HoaDon + ex.InnerException + "----End InnerException----. ----Begin Message----: " + ex.Message + "----End Message----", str);
                    //return await Task.FromResult(ex.Message);
                    Thread.Sleep(1000);

                }
            }
            return await Task.FromResult("");
        }

        public async static Task<string> UpdateGiaVonByHoaDon(string str, HT_NhatKySuDung objNhatKySuDung, SsoftvnContext db)
        {
            try
            {
                //db.Database.Connection.Open();
                //db.Database.CommandTimeout = 60 * 60;
                if (objNhatKySuDung.ID_HoaDon != null)
                {
                    BH_HoaDon hd = db.BH_HoaDon.Where(p => p.ID == objNhatKySuDung.ID_HoaDon.Value).FirstOrDefault();
                    switch (objNhatKySuDung.LoaiHoaDon)
                    {
                        case 1:
                        case 4:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 18:
                            List<SqlParameter> paramlist = new List<SqlParameter>();
                            paramlist.Add(new SqlParameter("IDHoaDonInput", hd.ID));
                            paramlist.Add(new SqlParameter("IDChiNhanhInput", hd.ID_DonVi));
                            paramlist.Add(new SqlParameter("ThoiGian", objNhatKySuDung.ThoiGianUpdateGV));
                            db.Database.ExecuteSqlCommand("exec UpdateGiaVonVer2 @IDHoaDonInput, @IDChiNhanhInput, @ThoiGian", paramlist.ToArray());
                            break;
                        case 10:
                            switch (hd.YeuCau)
                            {
                                case "1":
                                    List<SqlParameter> paramlist2 = new List<SqlParameter>();
                                    paramlist2.Add(new SqlParameter("IDHoaDonInput", hd.ID));
                                    paramlist2.Add(new SqlParameter("IDChiNhanhInput", hd.ID_DonVi));
                                    paramlist2.Add(new SqlParameter("ThoiGian", objNhatKySuDung.ThoiGianUpdateGV));
                                    db.Database.ExecuteSqlCommand("exec UpdateGiaVonVer2 @IDHoaDonInput, @IDChiNhanhInput, @ThoiGian", paramlist2.ToArray());
                                    break;
                                case "3":
                                case "4":
                                    if (hd.NgaySua != null)
                                    {
                                        List<SqlParameter> paramlist3 = new List<SqlParameter>();
                                        paramlist3.Add(new SqlParameter("IDHoaDonInput", hd.ID));
                                        paramlist3.Add(new SqlParameter("IDChiNhanhInput", hd.ID_CheckIn));
                                        paramlist3.Add(new SqlParameter("ThoiGian", hd.NgaySua));
                                        db.Database.ExecuteSqlCommand("exec UpdateGiaVonVer2 @IDHoaDonInput, @IDChiNhanhInput, @ThoiGian", paramlist3.ToArray());
                                    }
                                    List<SqlParameter> paramlist1 = new List<SqlParameter>();
                                    paramlist1.Add(new SqlParameter("IDHoaDonInput", hd.ID));
                                    paramlist1.Add(new SqlParameter("IDChiNhanhInput", hd.ID_DonVi));
                                    paramlist1.Add(new SqlParameter("ThoiGian", hd.NgayLapHoaDon));
                                    db.Database.ExecuteSqlCommand("exec UpdateGiaVonVer2 @IDHoaDonInput, @IDChiNhanhInput, @ThoiGian", paramlist1.ToArray());
                                    break;
                            }
                            break;
                    }
                }
                return await Task.FromResult("");
            }
            catch (Exception ex)
            {
                CookieStore.WriteLogRunTask("Update ton kho - gia von: ----Begin InnerException----: idhoadon: " + objNhatKySuDung.ID_HoaDon + ex.InnerException + "----End InnerException----. ----Begin Message----: " + ex.Message + "----End Message----", str);
                return await Task.FromResult(ex.Message);
            }
        }
        public static string add_DiaryLT(HT_NhatKySuDung objNhatKySuDung, string str)
        {
            string erStr = string.Empty;
            SsoftvnContext db = SystemDBContext.GetDBContext(str);
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.Database.CommandTimeout = 60 * 30;
                    db.HT_NhatKySuDung.Add(objNhatKySuDung);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("ErrorWhenaddNKSD: " + ex.Message + ex.InnerException);
                }
            }
            return erStr;
        }

    }
}
