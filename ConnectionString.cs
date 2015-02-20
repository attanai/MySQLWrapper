using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLWrapper
{
    public class ConnectionString
    {
        /// <summary>
        /// Creates connection string based on defined values
        /// </summary>
        /// <param name="dataBase">Database Name</param>
        /// <param name="dataSource">Data Source, ex: localhost</param>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public static string newConnectionString(string dataBase, string dataSource, string userName, string password)
        {
            return "Database=" + dataBase + ";Data Source=" + dataSource + ";User Id=" + userName + ";Password=" + password;
        }
        /// <summary>
        /// Creates connection string based on defined values
        /// </summary>
        /// <param name="dataBase">Database Name</param>
        /// <param name="dataSource">Data Source, ex: localhost</param>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public static string newConnectionString(string dataSource, string userName, string password)
        {
            return "Data Source=" + dataSource + ";User Id=" + userName + ";Password=" + password;
        }

        /// <summary>
        /// Object representing a Connection string.
        /// </summary>
        public ConnectionString()
        {

        }

        public string database = string.Empty;
        public string dataSource = string.Empty;
        public string userName = string.Empty;
        public string password = string.Empty;
        public string connectionString()
        {
            return database != string.Empty ? newConnectionString(dataSource, userName, password) : newConnectionString(database, dataSource, userName, password);
        }

    }
}
