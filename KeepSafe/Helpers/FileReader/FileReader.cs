using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Threading;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using System.Linq;

namespace KeepSafe.Helpers.FileReader
{
    public class FileReader : IFileReader
    {
        //file folder path
        static string GlobalFileFolderPath = "KeepSafe.Resources.Files";
        string FileFolderPath;

        bool IsUsingGlobal = true;

        public string GetFileFolderPath()
        {
            return IsUsingGlobal ? GlobalFileFolderPath : FileFolderPath ?? GlobalFileFolderPath;
        }

        static FileReader fileReader;
        WeakReference<IFileConnector> _fileReaderDelegate;
        public IFileConnector FileReaderDelegate
        {
            get
            {
                IFileConnector fileReaderDelegate;
                return _fileReaderDelegate.TryGetTarget(out fileReaderDelegate) ? fileReaderDelegate : null;
            }

            set
            {
                _fileReaderDelegate = new WeakReference<IFileConnector>(value);
            }
        }

        public static FileReader GetInstance
        {
            get
            {
                if (fileReader == null)
                    fileReader = new FileReader();

                return fileReader;
            }

        }

        public void SetDelegate(IFileConnector weakReference)
        {
            FileReaderDelegate = weakReference;
        }

        public async Task ReadFile(string fileName, CancellationToken ct, int wsType)
        {
            var assembly = typeof(FileReader).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream($"{GetFileFolderPath()}.{fileName}" );
            if (stream == null)
            {
                FileReaderDelegate?.ReceiveError("Error", $"{ GetFileFolderPath()}.{ fileName}" + " is either not created yet or the BUILD ACTION is not EMBEDDED RESOURCE", wsType);
            }
            using (var reader = new StreamReader(stream))
            {
                var json = await reader.ReadToEndAsync();
                var jObject = JObject.Parse(json);
                App.Log("Response: " + JObject.Parse(json));
                ProcessResponse(jObject);
                FileReaderDelegate?.ReceiveJSONData(jObject, wsType);
            }
        }

        public async Task CreateDummyResponse(string response, CancellationToken ct, int wsType)
        {
            await Task.Delay(16);
            var jObject = JObject.Parse(response);
            App.Log("Response: " + jObject);
            ProcessResponse(jObject);
            FileReaderDelegate?.ReceiveJSONData(jObject, wsType);
        }

        void ProcessResponse(JObject json)
        {
            // =========================================
            // Show popup message
            //TODO Fix
            //if (json.ContainsKey("custom_message") && json.ContainsKey("status", "200"))
            //{
            //    Device.BeginInvokeOnMainThread(() =>
            //    {
            //        var customAlert = (CustomAlertPopup)PopupNavigation.Instance.PopupStack.FirstOrDefault(obj => obj is CustomAlertPopup);

            //        if (customAlert != null && customAlert.CustomAlertContent.Title == "New Update Available") // Prioritize update available
            //        {
            //            // Do not display unauthorize
            //        }
            //        else
            //        {
            //            Utilities.ShowCustomMessagePopup(json["custom_message"].ToObject<Models.CustomMessageModel>());
            //        }
            //    });
            //}
            // =========================================
        }

        public FileReader()
        {
        }

        public FileReader(string fileFolderPath)
        {
            IsUsingGlobal = false;
            FileFolderPath = fileFolderPath;
        }

    }
}
