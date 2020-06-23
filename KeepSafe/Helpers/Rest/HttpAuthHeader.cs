using System;
using System.Text;

namespace KeepSafe.Rest
{
    public static class HttpAuthHeader
    {
        public static string BasicAuth(string username, string KeepSafeword)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, KeepSafeword)));
        }
    }
}
