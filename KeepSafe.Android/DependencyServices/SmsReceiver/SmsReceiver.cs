using System;
using Android.Content;
using Android.Gms.Auth.Api.Phone;
using Android.Gms.Common.Apis;
using Android.Runtime;
using Android.Telephony;
using System.Linq;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Gms.Common.Apis;
using KeepSafe.Helpers;

namespace KeepSafe.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { SmsRetriever.SmsRetrievedAction })]
    public class SmsReceiver : BroadcastReceiver
    {
        private static readonly string[] OtpMessageBodyKeywordSet = { "passcode" }; //You must define your own Keywords
        public override void OnReceive(Context context, Intent intent)
        {
            try
            { 
                if (intent.Action != SmsRetriever.SmsRetrievedAction) return;
                var bundle = intent.Extras;
                if (bundle == null) return;
                var status = (Statuses)bundle.Get(SmsRetriever.ExtraStatus);
                switch (status.StatusCode)
                {

                    case CommonStatusCodes.Success:
                        var message = (string)bundle.Get(SmsRetriever.ExtraSmsMessage);
                        var foundKeyword = OtpMessageBodyKeywordSet.Any(k => message.ToLower().Contains(k));
                        //if (!foundKeyword) return;
                        var code = ExtractNumber(message);
                        OTPServiceHelper.SmsReceiver?.Invoke(code);
                        break;
                    case CommonStatusCodes.Timeout:
                        break;

                }

            }
            catch (System.Exception)
            {

            }
        }

        private static string ExtractNumber(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            var number = Regex.Match(text, @"\d+").Value;
            return number;
        }
    }
}
