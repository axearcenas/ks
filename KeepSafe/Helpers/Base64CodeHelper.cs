using System;
using System.Text;
using AesEverywhere;

namespace KeepSafe.Helpers
{
    public static class Base64CodeHelper
    {

        static string Base64Encode(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        static string Base64Decode(string str)
        {
            var base64 = Convert.FromBase64String(str);
            var bytes = Encoding.UTF8.GetString(base64);
            return bytes;
        }

        public static string EncryptEncode(this string data)
        {
            return Base64Encode(AES256.Instance.Encrypt(data, Constants.BASE64_PASSKEY));
        }

        public static string DecodeDecrypt(this string data)
        {
            return AES256.Instance.Decrypt(Base64Decode(data), Constants.BASE64_PASSKEY);
        }
    }
}
