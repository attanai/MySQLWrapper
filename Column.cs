using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLWrapper
{
    public class Column
    {
        /// <summary>
        /// List columns available in table.
        /// </summary>
        /// <param name="table">The Table object that contains columns.</param>
        /// <returns></returns>
        public static List<Column> list(Table table)
        {
            return table.Columns;
        }

        /// <summary>
        /// Adds column to table. Note: Change is stored locally until 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool create(Column column, Table table)
        {
            try
            {
                table.Columns.Add(column);
                return true;
            }
            catch
            {
                return false;
            }
        }

		public static bool drop(Column column, Table table)
        {
            try
            {
                table.Columns.Remove(column);
                return true;
            }
            catch
            {
                return false;
            }
        }
		public Column()
        {}
		public string ID;
		public Type type;
        public bool Auto_Increment;
		public bool Primary_key;
		public bool Unique;
		public List<string> values;
    }
}
