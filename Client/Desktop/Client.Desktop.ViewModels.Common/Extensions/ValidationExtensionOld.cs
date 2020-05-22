using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Desktop.ViewModels.Common.Extensions
{
    public static class ValidationExtensionOld
    {
        private static readonly List<char> ForbiddenChars = new List<char> { '.', ',', ':', ';', '#', '$' };
        private static readonly List<char> AllowablePhoneChars = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '+' };

        public static readonly int DefaultMaxLength = 5;
        public static readonly int DefaultMinLength = 2;
        public static readonly int DefaultNameLength = 255;

        public static bool ValidateRequired(this object val, out string validationError)
        {
            validationError = null;

            if (string.IsNullOrEmpty(val?.ToString()))
            {
                validationError = "Is required field";

                return false;
            }

            return true;
        }

        public static bool ValidateRequired(this int val, out string validationError)
        {
            validationError = null;

            if (val == default(int))
            {
                validationError = "Is required field";

                return false;
            }

            return true;
        }

        public static bool ValidateRequired(this double val, out string validationError)
        {
            validationError = null;

            if (Equals(val, default(double)))
            {
                validationError = "Is required field";

                return false;
            }

            return true;
        }

        public static bool ValidateBySpaces(this string val, out string validationError)
        {
            validationError = null;

            if (string.IsNullOrWhiteSpace(val))
            {
                validationError = "Incorrect value (can not used only spaces)";

                return false;
            }

            return true;
        }

        public static bool ValidateByForbiddenChars(this string val, out string validationError)
        {
            validationError = null;

            if (val.Any(x => ForbiddenChars.Contains(x)))
            {
                validationError = $"Incorrect value (can not used symbols '{string.Join("", ForbiddenChars)}')";

                return false;
            }

            return true;
        }

        public static bool ValidateByAllowableChars(this string val, out string validationError)
        {
            validationError = null;

            if (val.Any(x => AllowablePhoneChars.Contains(x)))
            {
                validationError = $"Incorrect value (only symbols can used '{string.Join("", ForbiddenChars)}')";

                return false;
            }

            return true;
        }

        public static bool ValidateByShort(this string val, out string validationError)
        {
            return ValidateByShort(val, DefaultMinLength, out validationError);
        }

        public static bool ValidateByShort(this string val, int shortLength, out string validationError)
        {
            validationError = null;

            if (val.Trim().Length < shortLength)
            {
                validationError = $"Minimum {shortLength} symbols";
                return false;
            }

            return true;
        }

        public static bool ValidateByMaxLength(this string val, out string validationError)
        {
            return ValidateByMaxLength(val, DefaultMaxLength, out validationError);
        }

        public static bool ValidateByMaxLength(this string val, int maxLength, out string validationError)
        {
            validationError = null;

            if (val.Trim().Length > maxLength)
            {
                validationError = $"Character limit exceeded. The maximum number of characters {maxLength}";
                return false;
            }

            return true;
        }

        public static bool ValidateByShortNameLength(this string val, out string validationError)
        {
            return ValidateByMaxLength(val, DefaultNameLength, out validationError);
        }

        public static bool ValidateByShortNameLength(this string val, int maxLength, out string validationError)
        {
            validationError = null;

            if (val.Trim().Length > maxLength)
            {
                validationError = $"Character limit exceeded. The maximum number of characters {maxLength}";
                return false;
            }

            return true;
        }

        public static bool ValidateMinAmount(this double val, out string validationError)
        {
            validationError = null;

            if (val <= 0)
            {
                validationError = "Must be more then 0";

                return false;
            }

            return true;
        }

        public static bool ValidateMinAmount(this int val, out string validationError)
        {
            validationError = null;

            if (val <= 0)
            {
                validationError = "Must be more then 0";

                return false;
            }

            return true;
        }

    }
}
