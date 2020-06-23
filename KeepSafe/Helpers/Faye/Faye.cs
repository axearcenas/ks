using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace KeepSafe.Helpers.Faye
{
    public class Faye
    {
        static Faye _faye;
        public static Faye Instance
        {
            get
            {
                if (_faye == null)
                {
                    _faye = new Faye();
                }
                return _faye;
            }
        }

        public FayeClient client;
        List<string> Channels = new List<string>();
        bool shouldDisconnect;
        Timer timer = new Timer(1000);


        public Faye()
        {
            client = new FayeClient(1, "GAME");
#if OFFLINETABLE

#else
            client.Connect(Constants.FAYE_NOTIFICATION_URL);
            StartTimeoutTimer();

            if (client.Extensions == null)
            {
                client.AddExtension(new FayeAuthExtension());

                if (DataClass.GetInstance.CurrentServer.IsSecured)
                    client.AddExtension(new EncryptDecryptInator());
            }

            SubsribeToFayeEvents();

            timer.Elapsed += Timer_Elapsed;
#endif
        }

        public void Init(bool reconnect = false)
        {
            App.Log("Init");

            if (reconnect)
            {
                shouldDisconnect = false;
                client.Connect(Constants.FAYE_NOTIFICATION_URL);
                StartTimeoutTimer();
            }
        }

        //SUBSCRIBE
        public void Subscribe(string channel)
        {
#if OFFLINETABLE

#else
            if (client.IsConnected)
            {
                if (Channels.Contains(channel))
                {
                    App.Log("Already subscribed: " + channel);
                }
                else
                {
                    App.Log("Client connected. Sending subscribe message: " + channel);
                    client.SendBayeuxSubscribeMessage(channel);
                }
            }
            else if (client.IsConnecting)
            {
                App.Log("Client connecting... Adding to channel list: " + channel);
                Channels.Add(channel);
            }
            else
            {
                App.Log("Client disconnected...Connecting...: " + Constants.FAYE_NOTIFICATION_URL);
                Channels.Add(channel);
                shouldDisconnect = false;
                client.Connect(Constants.FAYE_NOTIFICATION_URL);
                StartTimeoutTimer();
            }
#endif
        }

        //UNSUBSCRIBE
        public void Unsubscribe(string channel)
        {
#if OFFLINETABLE

#else
            if (client.IsConnected)
            {
                if (Channels.Contains(channel))
                {
                    App.Log("Client connected. Sending unsubscribe message: " + channel);
                    client.SendBayeuxUnsubscribeMessage(channel);
                }
                else
                {
                    App.Log("Not subscribed to channel.");
                }
            }
            else
            {
                App.Log("Not yet connected. Removing from channel list: " + Channels.Remove(channel));
            }
#endif
        }


        public void UnsubscribeAll()
        {
            foreach (string channel in Channels.ToList())
            {
                if (client.IsConnected)
                {
                    App.Log("Client connected. Sending unsubscribe message: " + channel);
                    client.SendBayeuxUnsubscribeMessage(channel);
                }
                else
                {
                    App.Log("Not yet connected. Clearing channel list...");
                    Channels.Clear();
                    return;
                }
            }
        }

        void Timer_Elapsed(object sender, EventArgs e)
        {
            App.Log("Force disconnect socket.");
#if OFFLINETABLE

#else
            client.DisconnectFromWebSocket();
            timer.Stop();
            Client_Disconnected(this, new EventArgs());
#endif
        }

        void StartTimeoutTimer()
        {
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                timer.Stop();
                timer.Start();
            }
        }

        void StopTimeoutTimer()
        {
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                timer.Stop();
            }
        }

        public void Disconnect()
        {
#if OFFLINETABLE

#else
            if (client.IsConnected)
            {
                client.SendBayeuxDisconnectMessage();
            }
            else
            {
                Channels.Clear();
                shouldDisconnect = true;
            }

            client.Extensions.Clear();
            if (client.Extensions.Count == 0)
            {
                client.AddExtension(new FayeAuthExtension());

                if (DataClass.GetInstance.CurrentServer.IsSecured)
                    client.AddExtension(new EncryptDecryptInator());
            }
#endif
        }

        void SubsribeToFayeEvents()
        {
#if OFFLINETABLE

#else
            client.Connected += Client_Connected;
            client.Subscribed += Client_Subscribed;
            client.Unsubscribed += Client_Unsubscribed;
            client.Disconnected += Client_Disconnected;
            client.MessageReceived += Client_MessageReceived;
#endif
        }

        void UnsubscribeToFayeEvents()
        {
#if OFFLINETABLE

#else
            client.Connected -= Client_Connected;
            client.Subscribed -= Client_Subscribed;
            client.Unsubscribed -= Client_Unsubscribed;
            client.Disconnected -= Client_Disconnected;
            client.MessageReceived -= Client_MessageReceived;
#endif
        }

        void Client_Connected(object sender, EventArgs e)
        {
            App.Log("CONNECTED!");
            StopTimeoutTimer();
            if (Channels.Count > 0)
            {
                foreach (string channel in Channels)
                {
                    App.Log("Subscribe to: " + channel);
                    client.SendBayeuxSubscribeMessage(channel);
                }
            }

            if (Channels.Count == 0 && shouldDisconnect)
            {
                App.Log("Disconnect...");
                Disconnect();
            }
        }

        void Client_Subscribed(object sender, SubscriptionEventArgs e)
        {
            if (e.Success)
            {
                if (Channels.Contains(e.ChannelName))
                {
                    App.Log("Channel already on the list. (Pre added)");
                }
                else
                {
                    App.Log("Adding to channel list: " + e.ChannelName);
                    Channels.Add(e.ChannelName);
                }
            }
        }

        void Client_Unsubscribed(object sender, SubscriptionEventArgs e)
        {
            if (e.Success)
            {
                App.Log("Removing from channel list: " + e.ChannelName);
                Channels.Remove(e.ChannelName);
            }
        }

        async void Client_Disconnected(object sender, EventArgs e)
        {
            if (Channels.Count == 0 && shouldDisconnect)
            {
                App.Log("Ignore disconnection...");
            }
            else
            {
                //Reconnect here...
                await Task.Delay(1000);
                App.Log("Disconnected, reconnecting... " + Constants.FAYE_NOTIFICATION_URL);
                client.Connect(Constants.FAYE_NOTIFICATION_URL);
                StartTimeoutTimer();
            }
        }

        void Client_MessageReceived(object sender, FayeMessageEventArgs e)
        {

        }
    }
}
