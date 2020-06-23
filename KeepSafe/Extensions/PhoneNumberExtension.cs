using System;
using System.Text.RegularExpressions;
using KeepSafe;

namespace KeepSafe.Extension
{
    public static class PhoneNumberExtension
	{
        public static bool IsNumber(this string phoneEntry)
        {
            return Regex.Replace(phoneEntry, "[() -]", "").Length < 13;
        }

        public static string ToPhoneNumber(this string phoneEntry)
        {
            App.Log($"PHONE NUMBER: {Regex.Replace(phoneEntry, "[() -]", "")}");
            return Regex.Replace(phoneEntry, "[() -]", "");
        }
    }
}
