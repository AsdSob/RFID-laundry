using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Client.Desktop.ViewModels.Common.Extensions
{
    public static class ValidationExtension
    {
        public static readonly int DefaultMaxLength = 5;
        public static readonly int DefaultMinLength = 2;
        public static readonly int DefaultNameLength = 100;
        private static readonly List<char> AllowablePhoneChars = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '+' };

        public static bool ValidateRequired(this object val, ref string validationError)
        {
            if (string.IsNullOrEmpty(val?.ToString()))
            {
                validationError += "\n * Is required field";

                return false;
            }

            return true;
        }

        public static bool ValidateRequired(this int val, ref string validationError)
        {
            if (val == default(int))
            {
                validationError += "\n* Is required field";

                return false;
            }

            return true;
        }

        public static bool ValidateRequired(this double val, ref string validationError)
        {
            if (Equals(val, default(double)))
            {
                validationError += "\n* Is required field";

                return false;
            }

            return true;
        }

        public static bool ValidateBySpaces(this string val, ref string validationError)
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                validationError += "\n* Incorrect value (can not be only spaces or empty)";

                return false;
            }

            return true;
        }

        public static bool ValidateByNameMaxLength(this string val, ref string validationError)
        {
            
            return ValidateByMaxLength(val, DefaultNameLength, ref validationError);
        }

        public static bool ValidateByMaxLength(this string val, int maxLength, ref string validationError)
        {
            if (val?.Trim().Length > maxLength)
            {
                validationError += $"\n* Character limit exceeded. The maximum number of characters {maxLength}";
                return false;
            }

            return true;
        }

        public static bool ValidateMinAmount(this int val, ref string validationError)
        {
            if (val <= 0)
            {
                validationError += "\n* Must be more then 0";

                return false;
            }

            return true;
        }

        public static bool ValidateEmail(this string val, ref string validationError)
        {
            if (!String.IsNullOrWhiteSpace(val))
            {
                var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                var match = regex.Match(val);
                if (!match.Success)
                {
                    validationError += "\n* Email not valid";
                    return false;
                }
            }

            return true;
        }


    }
}
