using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLWrapper
{
    public class Table
    {

       
		public static List<string> list;
		public static bool create;
		public static bool drop;
        public static string saveTableData()
        {
            return string.Empty;
        }
        public static string refreshTableData()
        {
            return string.Empty;
        }
		public Table()
        {}
		public List<Column> Columns;
		//public List<Row> Rows;
		public string ID;
        public string Tag;

    }
}
