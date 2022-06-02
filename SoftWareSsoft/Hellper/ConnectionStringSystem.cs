using Model.Web.API;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SoftWareSsoft.Hellper
{
    public class ConnectionStringSystem
    {
        public static string GetConnectionString(string ConnectionName)
        {
            string strConn = "";
            try
            {
                System.Configuration.Configuration config = null;
                if (System.Web.HttpContext.Current != null)
                {
                    config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                }
                else
                {
                    config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
                }
                //Retreive connection string setting
                strConn = config.ConnectionStrings.ConnectionStrings[ConnectionName].ConnectionString;
            }
            catch { }
            return strConn;
        }

        public static string GetInitialCatalogString(string connectionString)
        {
            string strInitialCatalog = "";
            try
            {
                //Retreive connection string setting
                System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                if (builder != null)
                {
                    string server = builder.DataSource;
                    string database = builder.InitialCatalog;
                    strInitialCatalog = server + " / " + database;
                }
            }
            catch
            {
            }
            return strInitialCatalog;
        }

        public static string CreateConnectionString(string ConnectionName)
        {
            string databaseName = "SSOFTWEB_" + ConnectionName.ToUpper();
            string strConn = "data source=" + SqlConnection.servername + ";initial catalog=" + databaseName + ";persist security info=True;user id=" + SqlConnection.username + ";password=" + SqlConnection.password + ";MultipleActiveResultSets=True;App=EntityFramework";
            string strProviderName = "System.Data.SqlClient";
            try
            {
                //Open the app.config for modification
                //  var config =  System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
                System.Configuration.Configuration config = null;
                if (System.Web.HttpContext.Current != null)
                {
                    config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                }
                else
                {
                    config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
                }
                //Retreive connection string setting
                var connectionString = config.ConnectionStrings.ConnectionStrings[ConnectionName];
                if (connectionString == null)
                {
                    //Create connection string if it doesn't exist
                    config.ConnectionStrings.ConnectionStrings.Add(new System.Configuration.ConnectionStringSettings
                    {
                        Name = ConnectionName,
                        ConnectionString = strConn,
                        ProviderName = strProviderName //Depends on the provider, this is for SQL Server
                    });
                    config.Save(System.Configuration.ConfigurationSaveMode.Modified);
                }
                ConfigurationManager.RefreshSection("connectionStrings");
                ConfigurationManager.RefreshSection("appSettings");
                return "";
            }
            catch (Exception e)
            {
                //TODO: Handle exception
                string str = e.Message;
                return str;
            }
        }

        public static string CreateConnectionString(string ConnectionName, string strConn, string strProviderName, string physicalPath, string virtualpath)
        {
            try
            {
                //Open the app.config for modification
                //  var config =  System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
                System.Web.Configuration.WebConfigurationFileMap map = new System.Web.Configuration.WebConfigurationFileMap();
                map.VirtualDirectories.Add(virtualpath, new System.Web.Configuration.VirtualDirectoryMapping(physicalPath, true));

                System.Configuration.Configuration config = null;
                if (System.Web.HttpContext.Current != null)
                {
                    //config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                    config = System.Web.Configuration.WebConfigurationManager.OpenMappedWebConfiguration(map, virtualpath);
                }
                else
                {
                    config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
                }
                //Retreive connection string setting
                var connectionString = config.ConnectionStrings.ConnectionStrings[ConnectionName];
                if (connectionString == null)
                {
                    //Create connection string if it doesn't exist
                    config.ConnectionStrings.ConnectionStrings.Add(new System.Configuration.ConnectionStringSettings
                    {
                        Name = ConnectionName,
                        ConnectionString = strConn,
                        ProviderName = strProviderName //Depends on the provider, this is for SQL Server
                    });
                    config.Save(System.Configuration.ConfigurationSaveMode.Modified);
                }
                ConfigurationManager.RefreshSection("connectionStrings");
                ConfigurationManager.RefreshSection("appSettings");
                return "";
            }
            catch (Exception e)
            {
                //TODO: Handle exception
                string str = e.Message;
                return str;
            }
        }

        public static string RemoveConnectionString(string ConnectionName)
        {
            try
            {
                Configuration config = null;
                if (System.Web.HttpContext.Current != null)
                {
                    config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                }
                else
                {
                    config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                }
                // Clear the connection strings collection.
                ConnectionStringsSection csSection = config.ConnectionStrings;
                ConnectionStringSettingsCollection csCollection = csSection.ConnectionStrings;
                ConnectionStringSettings cs = csCollection[ConnectionName];

                // Remove it. 
                if (cs != null)
                {
                    // Remove the element.
                    csCollection.Remove(cs);

                    // Save the configuration file.
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("connectionStrings");
                    ConfigurationManager.RefreshSection("appSettings");
                    return "";
                }
                else
                    return "";
            }
            catch (ConfigurationErrorsException err)
            {
                return err.ToString();
            }
        }

        //
        public static void EditAppSetting(string key, string value)
        {
            System.Configuration.Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}