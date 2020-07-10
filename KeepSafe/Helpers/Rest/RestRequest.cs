using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Xamarin.Forms;
using System.Net;
using System.Linq;
using Xamarin.Essentials;
using Rg.Plugins.Popup.Services;
using KeepSafe.Helpers;
using ImTools;

namespace KeepSafe.Rest
{
    public class RestRequest : IRestRequest
    {
        static RestRequest _restRequest;
        WeakReference<IRestReceiver> _webServiceDelegate;
        public IRestReceiver WebServiceDelegate
        {
            get
            {
                IRestReceiver webServiceDelegate;
                return _webServiceDelegate.TryGetTarget(out webServiceDelegate) ? webServiceDelegate : null;
            }

            set
            {
                _webServiceDelegate = new WeakReference<IRestReceiver>(value);
            }
        }

        public void SetDelegate(IRestReceiver weakReference)
        {
            WebServiceDelegate = weakReference;
        }

        public static RestRequest GetInstance
        {
            get { if (_restRequest == null) _restRequest = new RestRequest(); return _restRequest; }
        }

        public async Task GetRequestIgnoreSSL(string url, CancellationToken ct, int wsType, string authHeader = null, int timeout = 100, bool pingHost = false, bool ignoreNoInternet = false, bool ignoreTimeout = false)
        {
            bool ShouldRetry = true;
            App.Log("Request URL: " + url);
            while (ShouldRetry)
            {
                ShouldRetry = false;
                try
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        var handler = new TimeoutHandler
                        {
                            DefaultTimeout = TimeSpan.FromSeconds(100),
                            InnerHandler = new HttpClientHandler()
                            {
                                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                                {
                                    //byKeepSafe to ignore ssl certificate
                                    return true;
                                }
                            }
                        };

                        using (var client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.MaxResponseContentBufferSize = 256000;

                            if (!string.IsNullOrEmpty(authHeader))
                            {
                                client.DefaultRequestHeaders.Add("access-token", DataClass.GetInstance.Token);
                                client.DefaultRequestHeaders.Add("client", DataClass.GetInstance.ClientId);
                                client.DefaultRequestHeaders.Add("uid", DataClass.GetInstance.Uid);
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                            }

                            client.Timeout = Timeout.InfiniteTimeSpan;

                            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

                            request.SetTimeout(TimeSpan.FromSeconds(timeout)); //Per-request timeout

                            using (var response = await client.SendAsync(request, ct))
                            {
                                await RequestAsync(response, ct, wsType);
                            }
                        }
                    }
                    else // No Internet
                    {
                        if (ignoreNoInternet == false) // No Internet, Retry?
                            ShouldRetry = await ShouldRetryAsync(Constants.NO_INTERNET_TITLE, Constants.NO_INTERNET_CONTENT, ct, wsType);
                        else
                            WebServiceDelegate?.ReceiveError(Constants.NO_INTERNET_TITLE, Constants.NO_INTERNET_CONTENT, wsType);
                    }
                }
                catch (TimeoutException) when (ignoreTimeout == false) // Timeout, Retry?
                {
                    ShouldRetry = await ShouldRetryAsync("Connection timed out", Constants.NO_INTERNET_CONTENT, ct, wsType);
                }
            }
        }

        public async Task GetRequest(string url, CancellationToken ct, int wsType, string authHeader = null, int timeout = 100, bool pingHost = false, bool ignoreNoInternet = false, bool ignoreTimeout = false)
        {
            bool ShouldRetry = true;
            App.Log("Request URL: " + url);

            while (ShouldRetry)
            {
                ShouldRetry = false;
                try
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        var handler = new TimeoutHandler
                        {
                            DefaultTimeout = TimeSpan.FromSeconds(100),
                            InnerHandler = new HttpClientHandler()
                        };

                        using (var client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.MaxResponseContentBufferSize = 256000;

                            if (!string.IsNullOrEmpty(authHeader))
                            {
                                client.DefaultRequestHeaders.Add("access-token", DataClass.GetInstance.Token);
                                client.DefaultRequestHeaders.Add("client", DataClass.GetInstance.ClientId);
                                client.DefaultRequestHeaders.Add("uid", DataClass.GetInstance.Uid);
                                App.Log("Request Header [access-token]: " + DataClass.GetInstance.Token);
                                App.Log("Request Header [client]: " + DataClass.GetInstance.ClientId);
                                App.Log("Request Header [uid]: " + DataClass.GetInstance.Uid);
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                            }

                            client.Timeout = Timeout.InfiniteTimeSpan;

                            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

                            request.SetTimeout(TimeSpan.FromSeconds(timeout)); //Per-request timeout

                            using (var response = await client.SendAsync(request, ct))
                            {
                                await RequestAsync(response, ct, wsType);
                            }
                        }
                    }
                    else // No Internet
                    {
                        if (ignoreNoInternet == false) // No Internet, Retry?
                            ShouldRetry = await ShouldRetryAsync(Constants.NO_INTERNET_TITLE, Constants.NO_INTERNET_CONTENT, ct, wsType);
                        else
                            WebServiceDelegate?.ReceiveError(Constants.NO_INTERNET_TITLE, Constants.NO_INTERNET_CONTENT, wsType);
                    }
                }
                catch (TimeoutException) when (ignoreTimeout == false) // Timeout, Retry?
                {
                    ShouldRetry = await ShouldRetryAsync("Connection timed out", Constants.NO_INTERNET_CONTENT, ct, wsType);
                }
            }
        }

        public async Task PostRequestAsync(string url, string dictionary, CancellationToken ct, int wsType, string authHeader = null, int timeout = 100, bool pingHost = false, bool ignoreNoInternet = false, bool ignoreTimeout = false)
        {
            bool ShouldRetry = true;
            dictionary = Utilities.InsertEpoch(dictionary);

            App.Log($"URL: {url} Contents : {dictionary}");
            if (DataClass.GetInstance.CurrentServer.IsSecured)
            {
                dictionary = dictionary.EncryptEncode();
                App.Log("EncryptEncode : " + dictionary);
            }

            while (ShouldRetry)
            {
                ShouldRetry = false;
                try
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        var handler = new TimeoutHandler
                        {
                            DefaultTimeout = TimeSpan.FromSeconds(100),
                            InnerHandler = new HttpClientHandler()
                        };

                        using (var client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.MaxResponseContentBufferSize = 256000;

                            if (!string.IsNullOrEmpty(authHeader))
                            {
                                client.DefaultRequestHeaders.Add("access-token", DataClass.GetInstance.Token);
                                client.DefaultRequestHeaders.Add("client", DataClass.GetInstance.ClientId);
                                client.DefaultRequestHeaders.Add("uid", DataClass.GetInstance.Uid);
                                App.Log("Request Header [access-token]: " + DataClass.GetInstance.Token);
                                App.Log("Request Header [client]: " + DataClass.GetInstance.ClientId);
                                App.Log("Request Header [uid]: " + DataClass.GetInstance.Uid);
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                            }

                            client.Timeout = Timeout.InfiniteTimeSpan;

                            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(url));

                            request.SetTimeout(TimeSpan.FromSeconds(timeout)); //Per-request timeout

                            request.Content = new StringContent(dictionary, Encoding.UTF8, DataClass.GetInstance.CurrentServer.IsSecured ? "text/plain" : "application/json");

                            using (var response = await client.SendAsync(request, ct))
                            {
                                await RequestAsync(response, ct, wsType);
                            }
                        }
                    }
                    else // No Internet
                    {
                        if (ignoreNoInternet == false) // No Internet, Retry?
                            ShouldRetry = await ShouldRetryAsync(Constants.NO_INTERNET_TITLE, Constants.NO_INTERNET_CONTENT, ct, wsType);
                        else
                            WebServiceDelegate?.ReceiveError(Constants.NO_INTERNET_TITLE, Constants.NO_INTERNET_CONTENT, wsType);
                    }
                }
                catch (TimeoutException) when (ignoreTimeout == false) // Timeout, Retry?
                {
                    ShouldRetry = await ShouldRetryAsync("Connection timed out", Constants.NO_INTERNET_CONTENT, ct, wsType);
                }
            }
        }

        public async Task PutRequestAsync(string url, string dictionary, CancellationToken ct, int wsType, string authHeader = null, int timeout = 100, bool pingHost = false, bool ignoreNoInternet = false, bool ignoreTimeout = false)
        {
            bool ShouldRetry = true;
            dictionary = Utilities.InsertEpoch(dictionary);

            App.Log($"URL: {url} Contents : {dictionary}");
            if (DataClass.GetInstance.CurrentServer.IsSecured)
            {
                dictionary = dictionary.EncryptEncode();
                App.Log("EncryptEncode : " + dictionary);
            }

            while (ShouldRetry)
            {
                ShouldRetry = false;
                try
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        var handler = new TimeoutHandler
                        {
                            DefaultTimeout = TimeSpan.FromSeconds(100),
                            InnerHandler = new HttpClientHandler()
                        };

                        using (var client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.MaxResponseContentBufferSize = 256000;

                            if (!string.IsNullOrEmpty(authHeader))
                            {
                                client.DefaultRequestHeaders.Add("access-token", DataClass.GetInstance.Token);
                                client.DefaultRequestHeaders.Add("client", DataClass.GetInstance.ClientId);
                                client.DefaultRequestHeaders.Add("uid", DataClass.GetInstance.Uid);
                                App.Log("Request Header [access-token]: " + DataClass.GetInstance.Token);
                                App.Log("Request Header [client]: " + DataClass.GetInstance.ClientId);
                                App.Log("Request Header [uid]: " + DataClass.GetInstance.Uid);
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                            }

                            client.Timeout = Timeout.InfiniteTimeSpan;

                            var request = new HttpRequestMessage(HttpMethod.Put, new Uri(url));

                            request.SetTimeout(TimeSpan.FromSeconds(timeout)); //Per-request timeout

                            request.Content = new StringContent(dictionary, Encoding.UTF8, DataClass.GetInstance.CurrentServer.IsSecured ? "text/plain" : "application/json");

                            using (var response = await client.SendAsync(request, ct))
                            {
                                await RequestAsync(response, ct, wsType);
                            }
                        }
                    }
                    else // No Internet
                    {
                        if (ignoreNoInternet == false) // No Internet, Retry?
                            ShouldRetry = await ShouldRetryAsync(Constants.NO_INTERNET_TITLE, Constants.NO_INTERNET_CONTENT, ct, wsType);
                        else
                            WebServiceDelegate?.ReceiveError(Constants.NO_INTERNET_TITLE, Constants.NO_INTERNET_CONTENT, wsType);
                    }
                }
                catch (TimeoutException) when (ignoreTimeout == false) // Timeout, Retry?
                {
                    ShouldRetry = await ShouldRetryAsync("Connection timed out", Constants.NO_INTERNET_CONTENT, ct, wsType);
                }
            }
        }

        public async Task DeleteRequestAsync(string url, CancellationToken ct, int wsType, string authHeader = null, int timeout = 100, bool pingHost = false, bool ignoreNoInternet = false, bool ignoreTimeout = false)
        {
            bool ShouldRetry = true;
            App.Log("URL: " + url);

            while (ShouldRetry)
            {
                ShouldRetry = false;
                try
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        var handler = new TimeoutHandler
                        {
                            DefaultTimeout = TimeSpan.FromSeconds(100),
                            InnerHandler = new HttpClientHandler()
                        };

                        using (var client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.MaxResponseContentBufferSize = 256000;

                            if (!string.IsNullOrEmpty(authHeader))
                            {
                                client.DefaultRequestHeaders.Add("access-token", DataClass.GetInstance.Token);
                                client.DefaultRequestHeaders.Add("client", DataClass.GetInstance.ClientId);
                                client.DefaultRequestHeaders.Add("uid", DataClass.GetInstance.Uid);
                                App.Log("Request Header [access-token]: " + DataClass.GetInstance.Token);
                                App.Log("Request Header [client]: " + DataClass.GetInstance.ClientId);
                                App.Log("Request Header [uid]: " + DataClass.GetInstance.Uid);
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                            }

                            client.Timeout = Timeout.InfiniteTimeSpan;

                            var request = new HttpRequestMessage(HttpMethod.Delete, new Uri(url));

                            request.SetTimeout(TimeSpan.FromSeconds(timeout)); //Per-request timeout

                            using (var response = await client.SendAsync(request, ct))
                            {
                                await RequestAsync(response, ct, wsType);
                            }
                        }
                    }
                    else // No Internet
                    {
                        if (ignoreNoInternet == false) // No Internet, Retry?
                            ShouldRetry = await ShouldRetryAsync(Constants.NO_INTERNET_TITLE, Constants.NO_INTERNET_CONTENT, ct, wsType);
                        else
                            WebServiceDelegate?.ReceiveError(Constants.NO_INTERNET_TITLE, Constants.NO_INTERNET_CONTENT, wsType);
                    }
                }
                catch (TimeoutException) when (ignoreTimeout == false) // Timeout, Retry?
                {
                    ShouldRetry = await ShouldRetryAsync("Connection timed out", Constants.NO_INTERNET_CONTENT, ct, wsType);
                }
            }
        }
       
        public async Task MultiPartFormRequestAsync(string url, string jsonString, Dictionary<string, Stream> streams, CancellationToken ct, HttpMethod restMethod, int wsType, string authHeader = null, int timeout = 100, bool pingHost = false, bool ignoreNoInternet = false, bool ignoreTimeout = false, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            bool ShouldRetry = true;
            jsonString = Utilities.InsertEpoch(jsonString);
            App.Log("URL: " + url);
            App.Log("Contents: " + jsonString);

            while (ShouldRetry)
            {
                ShouldRetry = false;
                try
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        var handler = new TimeoutHandler
                        {
                            DefaultTimeout = TimeSpan.FromSeconds(100),
                            InnerHandler = new HttpClientHandler()
                        };

                        using (var client = new HttpClient(handler))
                        {
                            var uri = new Uri(url);
                            var json = JObject.Parse(jsonString);
                            var multipartFormData = new MultipartFormDataContent();

                            if (!string.IsNullOrEmpty(authHeader))
                            {
                                client.DefaultRequestHeaders.Add("access-token", DataClass.GetInstance.Token);
                                client.DefaultRequestHeaders.Add("client", DataClass.GetInstance.ClientId);
                                client.DefaultRequestHeaders.Add("uid", DataClass.GetInstance.Uid);
                                App.Log("Request Header [access-token]: " + DataClass.GetInstance.Token);
                                App.Log("Request Header [client]: " + DataClass.GetInstance.ClientId);
                                App.Log("Request Header [uid]: " + DataClass.GetInstance.Uid);
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                            }

                            foreach (var keyValuePair in json)
                            {
                                if (keyValuePair.Value.HasValues) //user[id] = 1, user[name] = name
                                {
                                    var innerJson = JObject.Parse(keyValuePair.Value.ToString());
                                    foreach (var innerKeyValuePair in innerJson)
                                    {
                                        var jsonPath = $"{keyValuePair.Key}[{innerKeyValuePair.Key}]";
                                        if (streams != null && streams.ContainsKey(jsonPath))
                                        {
                                            multipartFormData.Add(Utilities.CreateStreamContent(streams[jsonPath], innerKeyValuePair.Value.ToString(), jsonPath));
                                        }
                                        else if (!DataClass.GetInstance.CurrentServer.IsSecured) // except on IsSecured
                                        {
                                            multipartFormData.Add(Utilities.CreateStringContent(innerKeyValuePair.Value.ToString(), jsonPath));
                                        }
                                    }
                                }
                                else if (!DataClass.GetInstance.CurrentServer.IsSecured) //user_id = 1, except on IsSecured
                                {
                                    multipartFormData.Add(Utilities.CreateStringContent(keyValuePair.Value.ToString(), keyValuePair.Key));
                                }
                            }

                            if (DataClass.GetInstance.CurrentServer.IsSecured)
                            {
                                multipartFormData.Add(Utilities.CreateStringContent(jsonString.EncryptEncode(), "payload"));
                            }

                            var multipartFormDataString = JsonConvert.SerializeObject(multipartFormData);
                            App.Log("MultiPartFormData: " + multipartFormDataString);
                            HttpResponseMessage response = restMethod == HttpMethod.Put ? await client.PutAsync(uri, multipartFormData, ct) : await client.PostAsync(uri, multipartFormData, ct);
                            App.Log("Response MultiPartFormData: " + response);
                            await RequestAsync(response, ct, wsType);
                        }
                    }
                    else // No Internet
                    {
                        if (ignoreNoInternet == false) // No Internet, Retry?
                            ShouldRetry = await ShouldRetryAsync(Constants.NO_INTERNET_TITLE, Constants.NO_INTERNET_CONTENT, ct, wsType);
                        else
                            WebServiceDelegate?.ReceiveError(Constants.NO_INTERNET_TITLE, Constants.NO_INTERNET_CONTENT, wsType);
                    }
                }
                catch (TimeoutException) when (ignoreTimeout == false) // Timeout, Retry?
                {
                    ShouldRetry = await ShouldRetryAsync("Connection timed out", Constants.NO_INTERNET_CONTENT, ct, wsType);
                }
            }
        }

        async Task<bool> ShouldRetryAsync(string title, string content, CancellationToken ct, int wsType)
        {
            //TODO add LogInErrorPopUp => CustomPopup
            //LogInErrorPopUp alertPage = null;

            //// Only show one instance of no internet connection popup
            //await MainThread.InvokeOnMainThreadAsync(async () =>
            //{
            //    alertPage = (LogInErrorPopUp)PopupNavigation.Instance.PopupStack.FirstOrDefault(obj => obj is LogInErrorPopUp);
            //    if (alertPage == null || (alertPage != null && alertPage.CustomMessageModel.Content != Constants.NO_INTERNET_CONTENT))
            //    {
            //        var model = new Models.CustomMessageModel(title, "exclamation-circle", 2, content, "Try again");
            //        alertPage = new LogInErrorPopUp(model) { Result = new TaskCompletionSource<bool>() };
            //        await PopupNavigation.Instance.PushAsync(alertPage);
            //    }
            //});

            PopupHelper.RemoveLoading();
            //var result = await alertPage.Result.Task;
            var result = false;

            App.Log("ShouldRetry? " + result.ToString());
            if (result && ct.IsCancellationRequested == false) // Retry
                PopupHelper.ShowLoading();
            else
                WebServiceDelegate?.ReceiveError(Constants.NO_INTERNET_TITLE, Constants.NO_INTERNET_CONTENT, wsType);

            return result;
        }

        async Task RequestAsync(HttpResponseMessage response, CancellationToken ct, int wsType)
        {
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (DataClass.GetInstance.CurrentServer.IsSecured && response.Content.Headers.ContentType.MediaType.Equals("text/plain"))
            {
                App.Log("DecodeDecrypt : " + result);
                result = result.DecodeDecrypt();
            }
            if (response.IsSuccessStatusCode)
            {
                if (response.Headers.Contains("access-token") )
                {
                    var token = response.Headers.GetValues("access-token").FirstOrDefault();
                    if (!string.IsNullOrEmpty(token))
                    {
                        DataClass.GetInstance.Token = token;
                        App.Log("Token: " + DataClass.GetInstance.Token);
                    }
                }

                if (response.Headers.Contains("client"))
                {
                    var clientId = response.Headers.GetValues("client").FirstOrDefault();
                    if (!string.IsNullOrEmpty(clientId))
                    {
                        DataClass.GetInstance.ClientId = clientId;
                        App.Log("ClientId: " + DataClass.GetInstance.ClientId);
                    }
                }

                if (response.Headers.Contains("uid"))
                {
                    var uid = response.Headers.GetValues("uid").FirstOrDefault();
                    if (!string.IsNullOrEmpty(uid))
                    {
                        DataClass.GetInstance.Uid = uid;
                        App.Log("Uid: " + DataClass.GetInstance.Uid);
                    }
                }

                var json = JObject.Parse(result);

                // =========================================
                // Show popup message
                if (json.ContainsKey("notification_message"))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        //PopupHelper.ShowCustomMessagePopup(json["notification_message"].ToObject<Models.CustomMessageModel>());
                    });
                }
                // =========================================

                App.Log($"Request Success? : {response.IsSuccessStatusCode} Success Response: {json}");
                WebServiceDelegate?.ReceiveJSONData(json, wsType);
            }
            else
            {
                if(!string.IsNullOrEmpty(result) && !result.Contains("<html"))
                {
                    var json = JObject.Parse(result);
                    App.Log($"Request Success? : {response.IsSuccessStatusCode} ERROR[ {response.ReasonPhrase} ] Error Response: {json}");

                    if (json["status"] != null || response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        if (response.StatusCode == HttpStatusCode.Unauthorized && json["status"] == null)
                            json.Add("status", 401);

                        if (json["status"].ToString().Equals("401") && DataClass.GetInstance.LoginType != UserType.None)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                // TODO add CustomAlertPopup
                                //var customAlert = (CustomAlertPopup)PopupNavigation.Instance.PopupStack.FirstOrDefault(obj => obj is CustomAlertPopup);

                                //if (customAlert != null && customAlert.CustomAlertContent.Title == "New Update Available") // Prioritize update available
                                //{
                                //    // Do not display unauthorize
                                //}
                                //else if (json["custom_message"] != null)
                                //{
                                //    PopupHelper.ShowCustomMessagePopup(json["custom_message"].ToObject<Models.CustomMessageModel>());
                                //}
                                //else if(json["errors"] != null) // Backward compatibility
                                //{
                                //    Utilities.ShowCustomMessagePopup(new Models.CustomMessageModel("", "exclamation-circle", 2, json["errors"].ToString(), "Okay") { IsForcedLogout = true });
                                //}
                                //else // Backward compatibility
                                //{
                                //    Utilities.ShowCustomMessagePopup(new Models.CustomMessageModel("", "exclamation-circle", 2, "Your session has expired.\nPlease login again.", "Okay") { IsForcedLogout = true });
                                //}
                                PopupHelper.RemoveLoading();
                                await App.Current.MainPage.DisplayAlert(response.ReasonPhrase, "Unauthorize access!,You will be logout","Okay");
                                App.Logout();
                            });
                        }
                        else
                        {
                            WebServiceDelegate?.ReceiveJSONData(json, wsType);
                        }
                    }
                    else if (json["error"] != null)
                    {
                        WebServiceDelegate?.ReceiveError(response.ReasonPhrase, json["error"].ToString(), wsType);
                    }
                    else if (json["errors"] != null)
                    {
                        WebServiceDelegate?.ReceiveJSONData(json, wsType);
                    }
                    else
                    {
                        WebServiceDelegate?.ReceiveError(response.ReasonPhrase, "Unknown Error. Please Check!", wsType);
                    }
                }
                else
                {
                    //300, 404, 500
                    //await PopupNavigation.Instance.PushAsync(new Rg.Plugins.Popup.Pages.PopupPage()
                    //{
                    //    Content = new ScrollView() { Content = new Label() { Text = result, TextType = TextType.Html , BackgroundColor = Color.White}, Margin = new Thickness(20) }
                    //});
                    if (result.Contains("\"error\""))
                    {
                        var errorJson = JObject.Parse(result);
                        if(errorJson.ContainsKey("error"))
                            WebServiceDelegate?.ReceiveError(response.ReasonPhrase, errorJson["error"].ToString(), wsType);
                        else
                            WebServiceDelegate?.ReceiveError("Error!", response.ReasonPhrase, wsType);
                    }
                    else if (response.StatusCode == HttpStatusCode.BadGateway) // Maintenance
                    {
                        WebServiceDelegate?.ReceiveError("Error!", Constants.BAD_GATEWAY_CONTENT, wsType);
                    }
                    else
                    {
                        WebServiceDelegate?.ReceiveError("Error!", response.ReasonPhrase, wsType);
                    }
                }
            }
        }
    }
}

// NOTE
// server side changes:
//   override devise-token-auth response if unauthorize error, see KeepSafe-rails source code
