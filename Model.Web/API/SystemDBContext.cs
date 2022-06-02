using Model_banhang24vn.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Model.Web.API
{
    public class SystemDBContext
    {
        public static SsoftvnWebContext GetDBContext()
        {
            string str = CookieStore.GetCookieAes(SqlConnection.subdoamin);
            SsoftvnWebContext db = null;
            if (str == null || str == string.Empty || str.Trim() == "")
            {
                str = GetStrSubDomain();
            }
            if (str == null || str == string.Empty || str.Trim() == "")
            {
                return null;
            }
            else
            {
                db = new SsoftvnWebContext(str);
            }
            return db;
        }
        public static SsoftvnWebContext GetDBContext(string subdomain)
        {
            return new SsoftvnWebContext(subdomain);
        }
        public static void MigrationDatabase(string str)
        {
            try
            {
                var context = new SsoftvnWebContext(str);
                if (context.Database.Exists())
                {
                    var connectstring = context.Database.Connection.ConnectionString;
                    var configuration = new Migrations.Configuration();
                    configuration.TargetDatabase = new System.Data.Entity.Infrastructure.DbConnectionInfo(connectstring, "System.Data.SqlClient");
                    var migrator = new DbMigrator(configuration);
                    var migrations = migrator.GetPendingMigrations();
                    if (migrations.Any())
                    {
                        migrator.Update();
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        public static string GetStrSubDomain()
        {
            var host = HttpContext.Current.Request.Url.Host;
            var subdomain = "";
            if (host == "ssoft.vn" || host == "www.ssoft.vn")
            {
                subdomain = "BETA";
            }
            else if (!host.Equals("localhost"))
            {
                string[] blacklist = { "www", "mail" };//yourdomain
                string[] domainlist = { "webssoft", "ssoft", "open" };
                var result = host.Split('.').Where(o => !blacklist.Contains(o.ToLower())).ToArray();

                if (result.Skip(1).Any(o => domainlist.Contains(o.ToLower())))
                {
                    subdomain = result[0];
                }
                else
                {
                    subdomain = string.Join(".", result);
                }
            }
            else
            {
                subdomain = "DEMOWEBSSOFT";
            }
            var domain = new CuaHangDangKyService().GetSubdomainForDomain(subdomain);
            CookieStore.SetCookieAes(SqlConnection.subdoamin, domain, new TimeSpan(30, 0, 0, 0, 0), domain);
            return domain;
        }
    }
}
