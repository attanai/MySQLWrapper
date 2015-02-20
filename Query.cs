using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MySQLWrapper
{
    public class Query
    {

        #region Statics...

        public static dynamic runQuery(string queryString, string connectionString, Query.ReturnType returnType)
        {
            if (returnType == ReturnType.NULL)
            {
                QueryDBNull(queryString, connectionString);
                return null;
            }
            if (returnType == ReturnType.LISTOFARRAYS)
            {
                return QueryDBListStringArray(queryString, connectionString);
            }
            if (returnType == ReturnType.BOOLEAN)
            {
                return QueryDBBool(queryString, connectionString);
            }
            if (returnType == ReturnType.LIST)
            {
                QueryDBList(queryString, connectionString);
            }
            if (returnType == ReturnType.STRING)
            {
                QueryDBString(queryString, connectionString);
            }
            return string.Empty;
        }

        /// <summary>
        /// Queries DB, returns either "Operation Successful" or an error message".
        /// </summary>
        /// <param name="Query">MySQL Query</param>
        /// <param name="myConnectionString">Connection String (SQLWrapper.connectionString).</param>
        /// <returns></returns>
        private static string QueryDBNull(string Query, string myConnectionString)
        {
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
        /// Queries DB, returns true for success.
        /// </summary>
        /// <param name="Query">MySQL Query</param>
        /// <param name="myConnectionString">Connection String (SQLWrapper.connectionString).</param>
        /// <returns></returns>
        private static bool QueryDBBool(string Query, string myConnectionString)
        {
            try
            {
                MySqlConnection myConnection = new MySqlConnection(myConnectionString);
                MySqlCommand myCommand = new MySqlCommand(Query);
                myCommand.Connection = myConnection;
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                myCommand.Connection.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Operation failed: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Returns a List of Rows, where each row is a string array
        /// </summary>
        /// <param name="Query">MySQL Query</param>
        /// <param name="myConnectionString">Connection String - SQLWrapper.connectionString.</param>
        /// <returns>List<string[]></returns>
        private static List<string[]> QueryDBListStringArray(string Query, string myConnectionString)
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
        /// <param name="queryString">MySQL Query</param>
        /// <param name="column">Name of the column header</param>
        /// <param name="myConnectionString">Connection String (SQLWrapper.connectionString).</param>
        /// <returns></returns>
        private static List<string> QueryDBList(string queryString, string myConnectionString)
        {
            List<string> op = new List<string>();
            try
            {
                MySqlConnection myConnection = new MySqlConnection(myConnectionString);
                MySqlCommand myCommand = new MySqlCommand(queryString, myConnection);
                myConnection.Open();
                MySqlDataReader myReader;
                myReader = myCommand.ExecuteReader();
                try
                {

                    while (myReader.Read())
                    {
                        op.Add(myReader.GetString(0));
                    }
                }
                finally
                {
                    myReader.Close();
                    myConnection.Close();
                }
                return op;
            }
            catch (Exception e)
            {
                op.Add("Operation failed: " + e.Message);
                return op;
            }
        }

        /// <summary>
        /// Returns a List of Rows, where each row is a string array
        /// </summary>
        /// <param name="Query">MySQL Query</param>
        /// <param name="myConnectionString">Connection String - SQLWrapper.connectionString.</param>
        /// <returns>List<string[]></returns>
        private static string QueryDBString(string Query, string myConnectionString)
        {
            string outPut = string.Empty;

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
                                outPut += myReader.GetString(i) + ",";
                            }
                            catch
                            {
                                outPut += ",";
                            }

                        }

                        outPut += Environment.NewLine;
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
                return "Operation failed: " + e.Message;
            }
        }

        #endregion

        #region Initialize...
        /// <summary>
        /// Creates A Query Object from which a query string can be built.
        /// </summary>
        /// <param name="requestType">The type of request being made</param>
        /// <param name="display"> Item to display. Accepts wildcards
        /// ie: "columns" or "Primary_Key"</param>
        /// <param name="source">Source of information for request.</param>
        /// <param name="values">Values to be modified, where Key is the name of the column and Value is the value of the column.</param>
        /// <param name="condition">Criteria being compared for result. ie: "column1='Superman'"</param>
        /// <param name="returnType">The Type of response to return.</param>
        public Query(RequestType requestType, string display, string source, Dictionary<string, string> values, string condition, ReturnType returnType)
        {
            Display = display;
            request = requestType;
            VALUES = values;
            RequestCriteria = condition;
            returns = returnType;
        }
        /// <summary>
        /// Creates A Query Object from which a query string can be built.
        /// </summary>
        /// <param name="requestType">The type of request being made</param>
        /// <param name="display"> Item to display. Accepts wildcards
        /// ie: "columns" or "Primary_Key"</param>
        /// <param name="source">Source of information for request.</param>
        /// <param name="values">Values to be modified, where Key is the name of the column and Value is the value of the column.</param>
        /// <param name="returnType">The Type of response to return.</param>
        public Query(RequestType requestType, string display, string source, Dictionary<string, string> values, ReturnType returnType)
        {
            Display = display;
            request = requestType;
            VALUES = values;
            RequestCriteria = string.Empty;
            returns = returnType;
        }
        /// <summary>
        /// Creates A Query Object from which a query string can be built.
        /// </summary>
        /// <param name="requestType">The type of request being made</param>
        /// <param name="display"> Item to display. Accepts wildcards
        /// ie: "columns" or "Primary_Key"</param>
        /// <param name="source">Source of information for request.</param>
        /// <param name="returnType">The Type of response to return.</param>
        public Query(RequestType requestType, string display, string source, ReturnType returnType)
        {
            Display = display;
            request = requestType;
            VALUES = null;
            RequestCriteria = string.Empty;
            returns = returnType;
        }
        /// <summary>
        /// Creates A Query Object from which a query string can be built.
        /// </summary>
        /// <param name="requestType">The type of request being made</param>
        /// <param name="display"> Item to display. Accepts wildcards
        /// ie: "columns" or "Primary_Key"</param>
        /// <param name="returnType">The Type of response to return.</param>
        public Query(RequestType requestType, string display, ReturnType returnType)
        {
            Display = display;
            request = requestType;
            VALUES = null;
            RequestCriteria = string.Empty;
            returns = returnType;
        }
        public Query(string queryString)
        {
            localQueryString = queryString;
        }
        /// <summary>
        /// Creates A Query Object from which a query string can be built.
        /// </summary>
        public Query()
        {
            
        }

        #endregion

        #region Locals...
        private string localQueryString = string.Empty;

        private string MakeQueryString()
        {
            string Q = "";
            try
            {
                Q += QType[(int)request];
                Q += request == RequestType.INSERT | request == RequestType.DELETE | request == RequestType.UPDATE ? string.Empty : " " + Display;

                if (source != string.Empty)
                {

                    if (request == RequestType.INSERT | request == RequestType.UPDATE)
                    {
                        Q += request == RequestType.INSERT ? " INTO " + source + " (" : " " + source + " SET ";
                        string Q2 = "Values (";
                        try
                        {
                            int valCount = VALUES.Count();
                            if (valCount > 0)
                            {
                                for (int i = 0; i < valCount; i++)
                                {
                                    try
                                    {
                                        Q += request == RequestType.INSERT ? VALUES.ElementAt(i).Key + ", " : VALUES.ElementAt(i).Key + "=" + VALUES.ElementAt(i).Value + ", ";
                                        Q2 += request == RequestType.INSERT ? VALUES.ElementAt(i).Value + ", " : string.Empty;
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }
                        Q = request == RequestType.INSERT ? Q.TrimEnd(new char[] { ' ', ',' }) + ") " : Q.TrimEnd(new char[] { ' ', ',' }) + " ";
                        Q2 = request == RequestType.INSERT ? Q2.TrimEnd(new char[] { ' ', ',' }) + ") " : string.Empty;
                        Q += Q2;
                    }
                    else
                    {
                        Q +=  " FROM " + source;
                    }

                    if (RequestCriteria != string.Empty)
                    {
                        Q += request == RequestType.INSERT ? string.Empty : " WHERE " + RequestCriteria; 
                    }

                }
                Q += ";";
            }
            catch { }
            return Q;
        }


        private Type[] RType = { typeof(List<string>), typeof(List<string[]>), typeof(string), typeof(bool), typeof(void) };
        private string[] QType = { "UPDATE", "INSERT", "DROP", "SHOW", "SELECT", "DELETE" };

        #endregion

        #region Building Blocks


        public string QueryString()
        {
            return localQueryString == string.Empty ? MakeQueryString() : localQueryString;
        }

        /// <summary>
        /// Source of information for request.
        /// </summary>
        public string source = string.Empty;

        /// <summary>
        /// Item to display. Accepts wildcards
        /// ie: "columns" or "Primary_Key"
        /// </summary>
        public string Display = "*";

        /// <summary>
        /// Values to be modified, where Key is the name of the column and Value is the value of the column.
        /// Requires Request Type to be defined as "INSERT" or "UPDATE".
        /// </summary>
        public  Dictionary<string, string> VALUES = new Dictionary<string,string>();

        /// <summary>
        /// The Type of response to return.
        /// </summary>
        public ReturnType returns = ReturnType.NULL;



        public enum ReturnType
        {
            /// <summary>
            /// Returns the first column of a query response.
            /// </summary>
            LIST = 0,
            /// <summary>
            /// Returns a query response where each row is represented as an array of strings
            /// </summary>
            LISTOFARRAYS = 1,
            //LISTOFROWS = List<Row>,
            /// <summary>
            /// Returns a query response as a string. This is the exact same output that would be returned from an SQL console
            /// </summary>
            STRING = 2,
            /// <summary>
            /// Returns True if a query was successful and False if it was not.
            /// </summary>
            BOOLEAN = 3,
            /// <summary>
            /// Returns nothing
            /// </summary>
            NULL = 4
        }

        /// <summary>
        /// The type of request being made
        /// </summary>
        public RequestType request = RequestType.SELECT;

        

        public enum RequestType
        {
            /// <summary>
            /// Changes an already existing item.
            /// </summary>
            UPDATE = 0,
            /// <summary>
            /// Adds an item.
            /// </summary>
            INSERT = 1,
            /// <summary>
            /// Removes an item.
            /// </summary>
            DROP = 2,
            /// <summary>
            /// Requests to show certain information (Such as table names or column names)
            /// </summary>
            SHOW = 3,
            /// <summary>
            /// Requests information from tables based on specified criteria.
            /// </summary>
            SELECT = 4,
            /// <summary>
            /// Removes a row.
            /// </summary>
            DELETE = 5,
        }

        /// <summary>
        /// Criteria being compared for result. ie: "column1='Superman'"
        /// </summary>
        public string RequestCriteria = string.Empty;

        #endregion
    }
}
