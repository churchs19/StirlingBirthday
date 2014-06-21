using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shane.Church.Utility.Core.WP.Extensions
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string s)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            var items = s.Split(new char[1] {' '}, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder result = new StringBuilder();

            for(var i=0;i<items.Length; i++)
            {
                if (i > 0) result.Append(" ");

                result.Append(textInfo.ToUpper(items[i][0]));
                result.Append(textInfo.ToLower(items[i].Substring(1)));
            }

            return result.ToString();
        }
    }
}
