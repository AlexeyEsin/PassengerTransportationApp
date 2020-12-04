using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Text.RegularExpressions;

namespace CLR_Functions_Library
{
    public static class Functions
    {
        [SqlFunctionAttribute()]
        public static SqlBoolean CheckRegNumber(SqlString regNumber)
        {
            var pattern = @"^[АВЕКМНОРСТУХ]\d{3}(?<!000)[АВЕКМНОРСТУХ]{2}\d{2,3}$";
            return (SqlBoolean)Regex.IsMatch(regNumber.ToString(), pattern);
        }
    }
}
