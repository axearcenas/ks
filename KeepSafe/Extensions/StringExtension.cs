﻿using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KeepSafe.Extensions
{
    public static class StringExtension
    {
        public static string AddAuth(this string url)
        {
            return url + (url.Contains("?") ? "&" : "?") + "access-token=" + DataClass.GetInstance.Token + "&client=" + DataClass.GetInstance.ClientId + "&uid=" + DataClass.GetInstance.Uid;
        }

        public static bool ContainsKey(this JObject json, string key, string value)
        {
            if (json.ContainsKey(key) && json[key].ToString() == value)
                return true;
            return false;
        }


        public static bool ContainsKey(this JToken jToken, string key)
        {
            
            if (jToken.HasValues && ((JObject)jToken).ContainsKey(key))
                return true;
            return false;
        }

        /// <summary>
        /// 1000 -> 1,000
        /// 1521.52 -> 1,521.52
        /// </summary>
        /// <returns>The comma.</returns>
        /// <param name="number">number.</param>
        public static string ToComma(this int number)
        {
            return number.ToString(number >= 1000 || number <= -1000 ? "0,0.##" : "0.##");
        }

        /// <summary>
        /// 1000 -> 1,000
        /// 1521.52 -> 1,521.52
        /// </summary>
        /// <returns>The comma.</returns>
        /// <param name="number">number.</param>
        public static string ToComma(this double number)
        {
            return number.ToString(number >= 1000 || number <= -1000 ? "0,0.##" : "0.##");
        }

        /// <summary>
        /// Clone the specified source. Para dili ma usab ang source.
        /// </summary>
        /// <returns>The clone.</returns>
        /// <param name="source">Source.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static bool IsValidEmail(this string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(Regex.Replace(str,@"(\P{Ll})(\P{Ll}\p{Ll})","$1 $2"),@"(\p{Ll})(\P{Ll})","$1 $2");
        }
    }
}
