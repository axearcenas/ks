using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace KeepSafe
{
    public class Utilities
    { 
        public static string InsertEpoch(string payload)
        {
            if (string.IsNullOrEmpty(payload)) return string.Empty;
            return payload.Insert(payload.Length - 1, ",\"e\":" + TimeManager.Instance.GetDateTime(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()));
        }

        public static string InsertEpochUrl(string url)
        {
            return url + (url.Contains("?") ? "&" : "?") + "e=" + TimeManager.Instance.GetDateTime(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }


        public static StreamContent CreateStreamContent(Stream imageStream, string filepath, string jsonPath)
        {
            if (imageStream != null)
            {
                var pictureContent = new StreamContent(imageStream);
                pictureContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    FileName = "\"" + Path.GetFileNameWithoutExtension(filepath) + "" + Path.GetExtension(filepath) + "\"",
                    Name = "\"" + jsonPath + "\""
                };
                return pictureContent;
            }
            return null;
        }

        public static StringContent CreateStringContent(string data, string jsonPath)
        {
            var dataContent = new StringContent(data, Encoding.UTF8, DataClass.GetInstance.CurrentServer.IsSecured ? "text/plain" : "application/json");
            dataContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"" + jsonPath + "\"",
            };
            return dataContent;
        }
    }
}
