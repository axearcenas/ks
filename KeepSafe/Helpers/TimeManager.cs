using System;
using System.Threading.Tasks;
using KeepSafe.Helpers.Faye;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KeepSafe
{
    public class TimeManager
    {
        static TimeManager timeManager;
        public static TimeManager Instance
        {
            get
            {
                if (timeManager == null)
                {
                    timeManager = new TimeManager();
                }
                return timeManager;
            }
        }

        string _meta_server_request = "/server/meta";
        string _meta_server_response = $"/server/meta/{(DataClass.GetInstance.User == null ? "0" : DataClass.GetInstance.User.Id.ToString())}";

        TaskCompletionSource<bool> _init = new TaskCompletionSource<bool>();
        bool _initializing;
        bool _initialized;

        DateTime _requestTime;
        TimeSpan _responseTime; //TODO adjust time based on response time
        DateTime _serverTime;
        TimeSpan _adjustedTime;

        public async Task<bool> Init()
        {
            if (_initialized == false && _initializing == false)
            {
                _initializing = true;
                SubscribeToFaye();
                await Task.WhenAny(_init.Task, Task.Delay(60000)); //60 seconds
                UnsubscribeToFaye();
                _initializing = false;
            }
            return _initialized;
        }

        public DateTime GetDateTime(DateTime date)
        {
            if (_initialized)
            {
                return date + _adjustedTime;
            }
            return date;
        }

        public double GetDateTime(double milliseconds)
        {
            if (_initialized)
            {
                return milliseconds + _adjustedTime.TotalMilliseconds;
            }
            return milliseconds;
        }

        void SubscribeToFaye()
        {
            _meta_server_response = $"/server/meta/{(DataClass.GetInstance.User == null ? "0" : DataClass.GetInstance.User.Id.ToString())}";
            Faye.Instance.client.Subscribed += Client_Subscribed;
            Faye.Instance.client.MessageReceived += Client_MessageReceived;
            Faye.Instance.Subscribe(_meta_server_response);
        }

        void UnsubscribeToFaye()
        {
            Faye.Instance.Unsubscribe(_meta_server_response);
            Faye.Instance.client.Subscribed -= Client_Subscribed;
            Faye.Instance.client.MessageReceived -= Client_MessageReceived;
        }

        void Client_Subscribed(object sender, SubscriptionEventArgs e)
        {
            if (e.ChannelName == _meta_server_response)
            {
                _requestTime = DateTime.UtcNow;
                var request = JsonConvert.SerializeObject(new
                {
                    request = "time",
                    id = DataClass.GetInstance.User.Id
                });
                Faye.Instance.client.Publish(_meta_server_request, request);
            }
        }

        void Client_MessageReceived(object sender, FayeMessageEventArgs e)
        {
            var json = JObject.Parse(e.Data);
            App.Log("Message from " + e.ChannelName);
            App.Log("\r\n" + json);
            if (e.ChannelName == _meta_server_response && json["response"] != null)
            {
                if (json["response"]["time"] != null)
                {
                    _responseTime = _requestTime - DateTime.UtcNow;
                    _serverTime = DateTime.Parse(json["response"]["time"].ToString());
                    _adjustedTime = _serverTime - _requestTime;
                    _initialized = true;

                    App.Log("Time difference: " + _adjustedTime.TotalMilliseconds + "ms");

                    if (_init.Task.IsCompleted == false)
                        _init.SetResult(true);
                }
            }
        }
    }
}
