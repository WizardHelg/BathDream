using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BathDream.ClassExtensions
{
    public static class StringExtensions
    {
        public static string GetPhoneNumber(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            StringBuilder builder = new StringBuilder();
            foreach (char ch in str)
                if (char.IsDigit(ch))
                    builder.Append(ch);

            if (builder.Length == 0)
                return null;

            if (builder[0] == '8')
                builder[0] = '7';

            if (builder[0] != '7' || builder.Length != 11)
                return null;

            return builder.ToString();
        }

        public static string GetPhoneNumberToView(this string str) => $"+7 ({str.Substring(1, 3)}) {str.Substring(4, 3)}-{str.Substring(7, 2)}-{str.Substring(9, 2)}";
    }
}
