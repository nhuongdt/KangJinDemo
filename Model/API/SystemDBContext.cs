using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SystemDBContext
    {
        public static SsoftvnContext GetDBContext()
        {
            string str = CookieStore.GetCookieAes("SubDomain");
            SsoftvnContext db = null;
            if (str == null || str == string.Empty || str.Trim() == "")
            {
                return null;
            }
            else
            {
                db = new SsoftvnContext(str);
            }
            return db;
        }
        public static SsoftvnContext GetDBContext(string subdomain)
        {
            return  new SsoftvnContext(subdomain);
        }

        public SsoftvnContext GetDBContextNonStatic(string subdomain)
        {
            return new SsoftvnContext(subdomain);
        }
        public static void MigrationDatabase(string str)
        {
            try
            {
                var context = new SsoftvnContext(str);
                if (context.Database.Exists())
                {
                    var connectstring = context.Database.Connection.ConnectionString;
                    var configuration = new Migrations.Configuration();
                    configuration.TargetDatabase = new System.Data.Entity.Infrastructure.DbConnectionInfo(connectstring, "System.Data.SqlClient");
                    configuration.CommandTimeout = 20*60;
                    var migrator = new DbMigrator(configuration);
                    var migrations = migrator.GetPendingMigrations();
                    if (migrations.Any())
                    {
                        migrator.Update();
                    }
                }
            }
            catch (Exception ex) {
                CookieStore.WriteLog("Migrationdatabase: " + ex.InnerException + ex.Message, str);
            }
        }
    }
}
