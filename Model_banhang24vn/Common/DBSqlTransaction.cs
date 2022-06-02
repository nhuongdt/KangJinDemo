using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Model_banhang24vn.Common
{
   public class DBSqlTransaction
    {
        SqlConnection connection;
        SqlTransaction transaction;
        SqlCommand command;
        bool oPenConect = false;

        public DBSqlTransaction()
        {
            connection = new SqlConnection(WebConfigurationManager.AppSettings["BanHang24vnContext"].ToString());
        }

        /// <summary>
        /// Mở connect sql
        /// </summary>
        public void OpenConnect()
        {
            connection.Open();
            command = connection.CreateCommand();
            oPenConect = true;
        }

        /// <summary>
        /// Đóng connect sql
        /// </summary>
        public void CloseConnect()
        {
            if(oPenConect)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Mở transaction 
        /// </summary>
        public void OpenSqlTransaction()
        {
            transaction = connection.BeginTransaction();
            command.Connection = connection;
            command.Transaction = transaction;
        }

        /// <summary>
        /// Rolback transaction 
        /// </summary>
        public void RolbackSqlTransaction()
        {
            transaction.Rollback();
        }

        /// <summary>
        /// Commit Transaction
        /// </summary>
        public void CommitSqlTransaction()
        {
            transaction.Commit();
        }

        /// <summary>
        /// Xử lý các hàm thủ tục thêm sửa xóa bằng linq to sql
        /// </summary>
        /// <param name="sqlString"></param>
        public void ExecuteNonQuerySql(string sqlString)
        {
            command.CommandText =sqlString;
            command.ExecuteNonQuery();
        }
    }
}
