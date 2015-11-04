using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Extensions
{

    public static class SQLExtensions
    {
        public static bool? GetBoolFromNullable(this SqlDataReader reader, string column)
        {
            Nullable<bool> boolValue = null;
            var tmp = reader[column];
            if (tmp.ToString().IsNotEmpty())
                boolValue = Convert.ToBoolean(tmp);
            return boolValue;
        }

        public static int? GetIntValueFromNullable(this SqlDataReader reader, string column)
        {
            int? intValue = null;
            var tmp = reader[column];
            if (tmp.ToString().IsNotEmpty())
                intValue = Convert.ToInt32(tmp);
            return intValue;
        }


        public static short? GetShortValueFromNullable(this SqlDataReader reader, string column)
        {
            short? shortValue = null;
            var tmp = reader[column];
            if (tmp.ToString().IsNotEmpty())
                shortValue = (short)Convert.ToInt32(tmp);
            return shortValue;
        }

        public static DateTime? GetDateTimeFromNullable(this SqlDataReader reader, string column)
        {
            DateTime? datetimeValue = null;
            var tmp = reader[column];
            if (tmp.ToString().IsNotEmpty())
                datetimeValue = Convert.ToDateTime(tmp);
            return datetimeValue;
        }



    }
}
