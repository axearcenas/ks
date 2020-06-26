using System;
using KeepSafe.Helpers;
using Xamarin.Essentials;

namespace KeepSafe.iOS.DependencyServices
{
    public class OTPService : IOTPService
    {
        public OTPService()
        {
        }

        public void ListenToSmsRetriever()
        {
            Clipboard.ClipboardContentChanged += Clipboard_ClipboardContentChanged;
        }

        private async void Clipboard_ClipboardContentChanged(object sender, EventArgs e)
        {
            if (Clipboard.HasText)
            {
                var code = await Clipboard.GetTextAsync();
                OTPServiceHelper.SmsReceiver?.Invoke(code);
            }
        }
    }
}
