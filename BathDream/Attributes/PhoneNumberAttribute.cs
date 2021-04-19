using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BathDream.Attributes
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if(value is string str
               && !string.IsNullOrEmpty(str))
            {
                StringBuilder builder = new StringBuilder();
                foreach (char ch in str)
                    if (char.IsDigit(ch))
                        builder.Append(ch);

                if (builder.Length == 0)
                    return false;

                if (builder[0] == '8')
                    builder[0] = '7';

                if (builder[0] != '7' || builder.Length != 11)
                    return false;

                return true;
            }

            return false;
        }
    }
}
