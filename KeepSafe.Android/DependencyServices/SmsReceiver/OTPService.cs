using System;
using Android.Gms.Auth.Api.Phone;
using Android.Gms.Tasks;
using KeepSafe.Droid.DependencyServices;
using Xamarin.Forms;
using Java.Lang;
using Application = Android.App.Application;

[assembly: Dependency(typeof(OTPService))]
namespace KeepSafe.Droid.DependencyServices
{
    public class OTPService : IOTPService
    {
        public OTPService()
        {
        }

        public void ListenToSmsRetriever()
        {
            SmsRetrieverClient client = SmsRetriever.GetClient(Application.Context);
            var task = client.StartSmsRetriever();
            task.AddOnSuccessListener(new SuccessListener());
            task.AddOnFailureListener(new FailureListener());
        }

        private class SuccessListener : Java.Lang.Object, IOnSuccessListener
        {
            //public IntPtr Handle => throw new NotImplementedException();

            //public void Dispose()
            //{
            //    //
            //}

            public void OnSuccess(Java.Lang.Object result)
            {
                //
            }
        }

        private class FailureListener : Java.Lang.Object, IOnFailureListener
        {
            //public IntPtr Handle => throw new NotImplementedException();

            //public void Dispose()
            //{
            //    //
            //}

            public void OnFailure(Java.Lang.Exception e)
            {
            }
        }
    }
}
