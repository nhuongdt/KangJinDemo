using libDM_DonVi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Service
{
    public class BusinessService
    {
        public static void UpdateBirthday(string jobSubDomain)
        {
            string conString = "server=data2.ssoft.vn;uid=sa;pwd=123asd!@#123;database=" + jobSubDomain;
            string insertcmd = "INSERT INTO HT_ThongBao(ID, ID_DonVi, NoiDungThongBao, NgayTao,LoaiThongBao, NguoiDungDaDoc)"
                             + "select NEWID(),(CASE WHEN dt.ID_DonVi IS NULL THEN  NEWID() ELSE dt.ID_DonVi END),"
                             + "'<p onclick=\"loaddadoc(''' + 'key' + ''')\"> Khách hàng <a onclick=\"loadthongbao('''+ '3' +''', ''' + dt.MaDoiTuong + ''', ''' + 'key' + ''')\">' + dt.TenDoiTuong + N'</a> có sinh nhật hôm nay</p>'"
                              + ", GETDATE(),3,'' from DM_DoiTuong dt where dt.LoaiDoiTuong = 1 and dt.TheoDoi = 0 and " +
                             "DATEPART(day, dt.NgaySinh_NgayTLap)= DATEPART(day, GETDATE())and " +
                             "DATEPART(month, dt.NgaySinh_NgayTLap)= DATEPART(month, GETDATE())";

            try
            {
                using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(conString))
                {
                    con.Open();
                    SqlCommand cmdAdd = new SqlCommand(insertcmd, con);
                    cmdAdd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("UpdateBirthday(string jobSubDomain) "+ jobSubDomain + ": " + ex.InnerException + ex.Message);
            }
        }

        public static void NotifyHanSuDungLo(string jobSubDomain)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext(jobSubDomain);
            List<DM_DonVi> lstDonVi = db.DM_DonVi.ToList();
            List<ListDMLoHetHan> lstReturn = new List<ListDMLoHetHan>();
            foreach (var item in lstDonVi)
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                var timeEnd = DateTime.Now;
                paramlist.Add(new SqlParameter("timeEnd", timeEnd));
                paramlist.Add(new SqlParameter("ID_ChiNhanh", item.ID));
                List<ListDMLoHetHan> listTon = db.Database.SqlQuery<ListDMLoHetHan>("exec GetListDM_LoHangHetHan @timeEnd, @ID_ChiNhanh", paramlist.ToArray()).ToList();
                if(listTon == null)
                {
                    listTon = new List<ListDMLoHetHan>();
                }
                lstReturn.AddRange(listTon);
            }
            foreach (var item in lstReturn)
            {
                HT_ThongBao httbCH = new HT_ThongBao();
                httbCH.ID = Guid.NewGuid();
                httbCH.ID_DonVi = item.ID_DonVi;
                httbCH.LoaiThongBao = 4; //loai = 0 thông báo hết hàng, 1: thông báo có đơn chuyển hàng, 3: thông báo ngày sinh nhật, 4: hết hạn lô hàng
                httbCH.NoiDungThongBao = "<p onclick=\"loaddadoc('" + httbCH.ID + "')\">Hàng hóa <a onclick=\"loadthongbao('4', '" + item.MaHangHoa + "','" + httbCH.ID + "')\">" + "<span class=\"blue\">" + item.MaHangHoa + " </span>" + " có lô hàng " + item.MaLoHang + " </a> đã hết hạn </p>";
                httbCH.NgayTao = DateTime.Now;
                httbCH.NguoiDungDaDoc = "";
                db.HT_ThongBao.Add(httbCH);
                db.SaveChanges();
            }
        }

        public static void BackupHistory(string jobSubDomain)
        {
            
        }

        public static List<string> GetDataBaseList()
        {
            List<string> list = new List<string>();
            string conString = "server=data2.ssoft.vn;uid=sa;pwd=123asd!@#123;database=master";
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT name FROM sys.databases", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(dr[0].ToString());
                        }
                    }
                }
                con.Close();
            }
            return list;
        }
    }
}
