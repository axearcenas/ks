using System;
using Xamarin.Forms;

namespace KeepSafe.Helpers
{
    public class OTPServiceHelper
    {
        public static Action<string> SmsReceiver;
        public OTPServiceHelper()
        {
        }

        public static void ListenToSmsRetriever()
        {
            DependencyService.Get<IOTPService>()?.ListenToSmsRetriever();
        }

    }
}
