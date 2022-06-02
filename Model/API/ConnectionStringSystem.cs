using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using System.Data.SqlClient;

namespace Model
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
            string databaseName = "SSOFT_" + ConnectionName.ToUpper();
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
                    ConfigurationManager.RefreshSection("connectionStrings");
                    ConfigurationManager.RefreshSection("appSettings");
                }
                return "";
            }
            catch (Exception e)
            {
                //TODO: Handle exception
                string str = e.Message;
                return str;
            }
        }

        public static string AddUpdateConnectionString(string name, string path)
        {
            try
            {
                bool isNew = false;
                //string path = Server.MapPath("~/Web.Config");
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNodeList list = doc.DocumentElement.SelectNodes(string.Format("/connectionStrings/add[@name='{0}']", name));
                XmlNode node;
                isNew = list.Count == 0;
                if (isNew)
                {
                    node = doc.CreateNode(XmlNodeType.Element, "add", null);
                    XmlAttribute attribute = doc.CreateAttribute("name");
                    attribute.Value = name;
                    node.Attributes.Append(attribute);

                    attribute = doc.CreateAttribute("connectionString");
                    attribute.Value = "";
                    node.Attributes.Append(attribute);

                    attribute = doc.CreateAttribute("providerName");
                    attribute.Value = "System.Data.SqlClient";
                    node.Attributes.Append(attribute);
                }
                else
                {
                    node = list[0];
                }
                string databaseName = "SSOFT_" + name.ToUpper();
                string conString = node.Attributes["connectionString"].Value;
                SqlConnectionStringBuilder conStringBuilder = new SqlConnectionStringBuilder(conString);
                conStringBuilder.InitialCatalog = databaseName;
                conStringBuilder.DataSource = SqlConnection.servername;
                conStringBuilder.PersistSecurityInfo = true;
                conStringBuilder.UserID = SqlConnection.username;
                conStringBuilder.Password = SqlConnection.password;
                if (isNew)
                {
                    node.Attributes["connectionString"].Value = conStringBuilder.ConnectionString;
                    doc.DocumentElement.SelectNodes("/connectionStrings")[0].AppendChild(node);
                    doc.Save(path);
                }
                ConfigurationManager.RefreshSection("connectionStrings");
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
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
