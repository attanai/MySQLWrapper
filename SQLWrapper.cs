using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ServerMap
{
    public class SQLWrapper
    {


        #region Objects...

        /// <summary>
        /// String writer for column portion of table creation.
        /// </summary>
        /// <param name="columnName">Name of Column</param>
        /// <param name="type">Type of Column (ie: int or text)</param>
        /// <param name="allowNullValues">Can this value be blank?</param>
        /// <param name="autoIncrement">Does this value increase automatically (without user input)?</param>
        /// <param name="primaryKey">Is this valuse the Primary Key?</param>
        /// <returns></returns>
        public static string column(string columnName, string type, bool allowNullValues, bool autoIncrement, bool primaryKey)
        {
            string op = "";
            op = op + columnName + " " + type;
            if (!allowNullValues)
            {
                op = op + " NOT NULL";
            }
            if (autoIncrement)
            {
                op = op + " AUTO_INCREMENT";
            }
            if (primaryKey)
            {
                op = op + " PRIMARY KEY";
            }
            return op;
        }
        /// <summary>
        /// String writer for column portion of table creation.
        /// </summary>
        /// <param name="columnName">Name of Column</param>
        /// <param name="type">Type of Column (ie: int or text)</param>
        /// <returns></returns>
        public static string column(string columnName, string type)
        {
            string op = column(columnName, type, false, false, false);
            return op;
        }
        /// <summary>
        /// String writer for column portion of table creation.
        /// </summary>
        /// <param name="columnName">Name of Column</param>
        /// <param name="timestamp">Is this value a Time Stamp?</param>
        /// <returns></returns>
        public static string column(string columnName, bool timestamp)
        {
            string op = "";
            if (timestamp)
            {
                op = column(columnName, " TIMESTAMP", false, false, false);
            }
            return op;
        }

        /// <summary>
        /// Creates connection string based on defined values
        /// </summary>
        /// <param name="dataBase">Database Name</param>
        /// <param name="dataSource">Data Source, ex: localhost</param>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public static string ConnectionString(string dataBase, string dataSource, string userName, string password)
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
        public static string ConnectionString(string dataSource, string userName, string password)
        {
            return "Data Source=" + dataSource + ";User Id=" + userName + ";Password=" + password;
        }

        #endregion


        #region userInfo

        /// <summary>
        /// Get List<string> of user account names</string>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<string> getUserNames(string connectionString)
        {
            List<string> outPut = new List<string>();
            try
            {
                MySqlConnection myConnection = new MySqlConnection(connectionString);
                MySqlCommand myCommand = new MySqlCommand("SELECT user FROM mysql.user WHERE user != 'root' && user != '';", myConnection);
                myConnection.Open();

                MySqlDataReader myReader;
                myReader = myCommand.ExecuteReader();
                try
                {

                    while (myReader.Read())
                    {
                        outPut.Add(myReader.GetString(0));
                    }

                }
                finally
                {
                    myReader.Close();
                    myConnection.Close();
                }
                return outPut;
            }
            catch (Exception e)
            {
                outPut.Add("Operation failed: " + e.Message);
                return outPut;
            }


        }

        public static List<string> getUserPermissions(string userName, string connectionString)
        {
            List<string> output = new List<string>();

            List<string[]> userData = ReadTableRow("SHOW GRANTS FOR " + userName + ";", connectionString);

            foreach (string[] s in userData)
            {
                if (s[0].ToUpper().Contains("TFSERVERMAP"))
                {
                    if (s[0].ToUpper().Contains("ALL PRIVILEGES"))
                    {
                        output = userPermissions;
                    }
                    else
                    {
                        string privs = s[0].ToUpper().Replace("GRANT ", "").Replace(" ON ", ",");

                        string[] privsB = privs.Split(',');

                        foreach (string t in privsB)
                        {
                            if (!t.ToUpper().Contains("TFSERVERMAP"))
                            {
                                output.Add(t.Trim());
                            }
                        }

                    }
                }
            }


            return output;
        }

        public static List<string> userPermissions = new string[] { "SELECT", "INSERT", "UPDATE", "DELETE", "CREATE", "DROP", "REFERENCES", "INDEX", "ALTER", "CREATE TEMPORARY TABLES", "LOCK TABLES", "EXECUTE", "CREATE VIEW", "SHOW VIEW", "CREATE ROUTINE", "ALTER ROUTINE", "EVENT", "TRIGGER" }.ToList<string>();

        #endregion


        #region Table Methods
        /// <summary>
        /// Gets a list of column names and types for a given table
        /// </summary>
        /// <param name="tableName">Name of table</param>
        /// <param name="connectionString">Connection String (SQLWrapper.connectionString)</param>
        /// <returns></returns>
        public static Dictionary<string, System.Type> getColumnNamesAndTypes(string tableName, string connectionString)
        {

            Dictionary<string, System.Type> ret = new Dictionary<string, System.Type>();
            try
            {
                string Query = "SELECT * from " + tableName + " WHERE Primary_Key<5;";
                MySqlConnection myConnection = new MySqlConnection(connectionString);
                MySqlCommand myCommand = new MySqlCommand(Query, myConnection);
                myConnection.Open();
                MySqlDataReader myReader;
                myReader = myCommand.ExecuteReader();
                try
                {
                    for (int f = 0; f < 100; f++)
                    {
                        try
                        {
                            ret.Add(myReader.GetName(f), myReader.GetFieldType(f));
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    myReader.Close();
                    myConnection.Close();
                }
                return ret;
            }
            catch (Exception e)
            {
                ret.Add("Operation failed: " + e.Message, typeof(bool));
                return ret;
            }
        }

        /// <summary>
        /// Gets a list of column names for a given table
        /// </summary>
        /// <param name="tableName">Name of table</param>
        /// <param name="connectionString">Connection String (SQLWrapper.connectionString)</param>
        /// <returns></returns>
        public static List<string> getColumnNames(string tableName, string connectionString)
        {

            List<string> ret = new List<string>();
            try
            {
                string Query = "SELECT * from " + tableName + " WHERE Primary_Key<5;";
                MySqlConnection myConnection = new MySqlConnection(connectionString);
                MySqlCommand myCommand = new MySqlCommand(Query, myConnection);
                myConnection.Open();
                MySqlDataReader myReader;
                myReader = myCommand.ExecuteReader();
                try
                {
                    for (int f = 0; f < 100; f++)
                    {
                        try
                        {
                            ret.Add(myReader.GetName(f));
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    myReader.Close();
                    myConnection.Close();
                }
                return ret;
            }
            catch (Exception e)
            {
                ret.Add("Operation failed: " + e.Message);
                return ret;
            }
        }

      
        /// <summary>
        /// Creates mySQL table
        /// </summary>
        /// <param name="tableID">Name of table.</param>
        /// <param name="columns">List of columns (SQLWrapper.column).</param>
        /// <param name="connectionString">Connection String (SQLWrapper.connectionString).</param>
        public static void createTable(string tableID, List<string> columns, string connectionString)
        {
            bool op = false;
            string query = "CREATE TABLE " + tableID + " (";
            foreach (string blip in columns)
            {
                query = query + Environment.NewLine + blip;
                if (blip != columns.Last<string>())
                {
                    query = query + ",";
                }
            }
            query = query + Environment.NewLine + ");";
            QueryDB(query, connectionString);
            
        }

        /// <summary>
        /// Deletes MySQL table.
        /// </summary>
        /// <param name="tableName">Name of table.</param>
        /// <param name="connectionString">Connection String (SQLWrapper.connectionString).</param>
        public static void deleteTable(string tableName, string connectionString)
        {
            QueryDB("DROP TABLE IF EXISTS " + tableName + ";", connectionString);
        }

        /// <summary>
        /// Get list of Tables in DB
        /// </summary>
        /// <param name="myConnectionString">Connection String (SQLWrapper.connectionString).</param>
        /// <returns></returns>
        public static List<string> getTables(string myConnectionString)
        {
            List<string> outPut = new List<string>();
            try
            {
                MySqlConnection myConnection = new MySqlConnection(myConnectionString);
                MySqlCommand myCommand = new MySqlCommand("SHOW tables;", myConnection);
                myConnection.Open();

                MySqlDataReader myReader;
                myReader = myCommand.ExecuteReader();
                try
                {

                    while (myReader.Read())
                    {
                        outPut.Add(myReader.GetString(0));
                    }

                }
                finally
                {
                    myReader.Close();
                    myConnection.Close();
                }
                return outPut;
            }
            catch (Exception e)
            {
                outPut.Add("Operation failed: " + e.Message);
                return outPut;
            }
        }


        #endregion


        #region DB Query Methods

        /// <summary>
        /// Queries DB, returns either "Operation Successful" or an error message".
        /// </summary>
        /// <param name="Query">MySQL Query</param>
        /// <param name="myConnectionString">Connection String (SQLWrapper.connectionString).</param>
        /// <returns></returns>
        public static string QueryDB(string Query, string myConnectionString)
        {


            // If the connection string is null, use a default.
            if (myConnectionString == "")
            {
                myConnectionString = "Database=Test;Data Source=localhost;User Id=username;Password=pass";
            }
            try
            {
                MySqlConnection myConnection = new MySqlConnection(myConnectionString);
                MySqlCommand myCommand = new MySqlCommand(Query);
                myCommand.Connection = myConnection;
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                myCommand.Connection.Close();
                return "Operation Successful";
            }
            catch (Exception e)
            {
                Console.WriteLine("Operation failed: " + e.Message);
                return "Operation failed: " + e.Message;
            }
        }

        

         

        /// <summary>
        /// Query MySQL - Returns Key Value Pair, where Key is a list of (string) column names and value is a list of string
        /// </summary>
        /// <param name="Query">MySQL Query</param>
        /// <param name="myConnectionString">Connection String (SQLWrapper.connectionString).</param>
        /// <returns></returns>
        public static KeyValuePair<List<string>, List<string[]>> ReadTable(string Query, string myConnectionString)
        {
            List<string[]> value = new List<string[]>();
            List<string> columnNames = new List<string>();
            try
            {
                MySqlConnection myConnection = new MySqlConnection(myConnectionString);
                MySqlCommand myCommand = new MySqlCommand(Query, myConnection);
                myConnection.Open();
                int numColumns = 100;
                MySqlDataReader myReader;
                myReader = myCommand.ExecuteReader();
                try
                {

                    
                    for (int f = 0; f < 100; f++)
                    {
                        try
                        {
                            columnNames.Add(myReader.GetName(f));
                        }
                        catch
                        {
                            numColumns = f;
                            break;
                        }
                    }

                    while (myReader.Read())
                    {

                        List<string> newlist = new List<string>();


                        for (int i = 0; i < numColumns; i++)
                        {
                            try
                            {
                                newlist.Add(myReader.GetString(i));
                            }
                            catch
                            {
                                newlist.Add("");
                            }
                            
                        }

                        string[] newObj = newlist.ToArray();
                        value.Add(newObj);
                    }

                }
                finally
                {
                    myReader.Close();
                    myConnection.Close();
                }
                KeyValuePair<List<string>, List<string[]>> ret = new KeyValuePair<List<string>, List<string[]>>(columnNames, value);
                return ret;
            }
            catch (Exception e)
            {
                string[] newObj = new string[1];
                newObj[0] = "Operation failed: " + e.Message;
                value.Add(newObj);
                columnNames.Add("Operation Failed");
                return new KeyValuePair<List<string>, List<string[]>>(columnNames, value);
            }
        }

        /// <summary>
        /// Returns a List of Rows, where each row is a string array
        /// </summary>
        /// <param name="Query">MySQL Query</param>
        /// <param name="myConnectionString">Connection String - SQLWrapper.connectionString.</param>
        /// <returns>List<string[]></returns>
        public static List<string[]> ReadTableRow(string Query, string myConnectionString)
        {
            
            

                List<string[]> outPut = new List<string[]>();

                try
                {
                    

                    MySqlConnection myConnection = new MySqlConnection(myConnectionString);
                    MySqlCommand myCommand = new MySqlCommand(Query, myConnection);
                    myConnection.Open();
                    int numColumns = 100;
                    MySqlDataReader myReader;
                    myReader = myCommand.ExecuteReader();
                    try
                    {


                        for (int f = 0; f < 100; f++)
                        {
                            try
                            {
                                string g = myReader.GetName(f);
                            }
                            catch
                            {
                                numColumns = f;
                                break;
                            }
                        }

                        while (myReader.Read())
                        {

                            List<string> newlist = new List<string>();


                            for (int i = 0; i < numColumns; i++)
                            {
                                try
                                {
                                    newlist.Add(myReader.GetString(i));
                                }
                                catch
                                {
                                    newlist.Add("");
                                }

                            }

                            string[] newObj = newlist.ToArray();
                            outPut.Add(newObj);
                        }

                    }
                    finally
                    {
                        myReader.Close();
                        myConnection.Close();
                    }


                    return outPut;

                }

                catch (Exception e)
                {
                    string[] newObj = new string[1];
                    newObj[0] = "Operation failed: " + e.Message;
                    outPut.Add(newObj);
                    return outPut;
                }
        }

        /// <summary>
        /// Returns output from single specified column.
        /// </summary>
        /// <param name="Query">MySQL Query</param>
        /// <param name="column">Name of the column header</param>
        /// <param name="myConnectionString">Connection String (SQLWrapper.connectionString).</param>
        /// <returns></returns>
        public static string ReadDBTable(string Query,string column, string myConnectionString)
        {
            string oP = "";
            try
            {
                MySqlConnection myConnection = new MySqlConnection(myConnectionString);
                MySqlCommand myCommand = new MySqlCommand(Query, myConnection);
                myConnection.Open();
                MySqlDataReader myReader;
                myReader = myCommand.ExecuteReader();
                try
                {

                    while (myReader.Read())
                    {
                        oP = oP + myReader.GetString(column) + Environment.NewLine;
                    }
                }
                finally
                {
                    myReader.Close();
                    myConnection.Close();
                }
                return oP;
            }
            catch (Exception e)
            {
                return "Operation failed: " + e.Message;
            }
        }

        #endregion

    }

}

