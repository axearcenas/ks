﻿using System;
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

        public static bool IsValidPhoneNumber(this string phoneEntry)
        {
            string phoneNumber = Regex.Replace(phoneEntry, "[() - +]", "");
            bool isNumber = long.TryParse(phoneNumber, out long result);
            return phoneNumber.Length == (phoneEntry.StartsWith("0") ? 11 : !phoneEntry.StartsWith("63") || !phoneEntry.StartsWith("+63") ? 10 :  12) && isNumber;
        }



        public static string ToPhoneNumber(this string phoneEntry)
        {
            App.Log($"PHONE NUMBER: {Regex.Replace(phoneEntry, "[() -]", "")}");
            return Regex.Replace(phoneEntry, "[() -]", "");
        }
    }
}
