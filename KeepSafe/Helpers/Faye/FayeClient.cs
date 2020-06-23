using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace KeepSafe.Helpers.Faye
{
    public class SubscriptionEventArgs : EventArgs
    {
        public string ChannelName { get; private set; }
        public string Error { get; private set; }
        public bool Success { get; private set; }
        public SubscriptionEventArgs(string channelName, string error = "")
        {
            ChannelName = channelName;
            Error = error;
            Success = (Error == string.Empty);
        }
    }

    public class FayeMessageEventArgs : EventArgs
    {
        public string ChannelName { get; private set; }
        public string Data { get; private set; }
        public FayeMessageEventArgs(string channelName, string data) : base()
        {
            ChannelName = channelName;
            Data = data;
        }
    }

    public class FayeClient
    {

        protected enum ConnectionNegotiationState
        {
            None,
            HandshakeSent,
            HandshakeSuccessful,
            HandshakeFailure,
            ConnectionRequest,
            ConnectionSuccessful,
            ConnectionFailure,
        }

        protected ConnectionNegotiationState _connectionState;

        protected string minimumVersion = "2.0";
        protected string version = "2.9.1";

        protected int messageId = 0;

        #region Events
        public event EventHandler Connected;
        public event EventHandler Disconnected;

        //public event EventHandler<int> ServerResponseTime;

        public event EventHandler<SubscriptionEventArgs> Subscribed;
        public event EventHandler<SubscriptionEventArgs> Unsubscribed;

        public event EventHandler<FayeMessageEventArgs> MessageReceived;
        #endregion

        #region Properties
        public enum ClientState
        {
            Unconnected,
            Connecting,
            Connected
        }
        public ClientState State { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// Websocket client for underlying transport.
        /// </summary>
        public Websockets.IWebSocketConnection WebsocketClient { get; set; }

        public bool IsConnected { get { return State == ClientState.Connected; } }

        public bool IsConnecting { get { return State == ClientState.Connecting; } }

        public string ClientId { get; protected set; }

        public List<string> Subscriptions { get; protected set; }

        public List<IFayeExtension> Extensions { get; protected set; }
        #endregion

        #region Initializers
        public FayeClient(int loggerLevel = 1, string name = "")
        {
            _loggerLevel = loggerLevel;
            fayeName = name;
        }

        public FayeClient(string url)
        {
            Url = url;
        }
        #endregion

        #region Faye methods
        public void Connect()
        {
            Connect(Url);
        }

        public void Connect(string url)
        {
            Url = url;
            if ((WebsocketClient != null && WebsocketClient.IsOpen))
            {
                Info("[Connect] A websocket connection is already open");
                if (IsConnected)
                {
                    SendBayeuxConnectMessage();
                }
                return;
            }

            ConnectToWebsocket();
        }

        public void Publish(string channelName, string message)
        {
            message = InsertEpoch(message);


            Info("Publishing to [{0}]: {1}", channelName, message);
            try
            {

                Dictionary<string, object> publishMessage = new Dictionary<string, object>() {
                    { BayeuxConstants.KeyChannel, channelName },
                    { BayeuxConstants.KeyClientId, ClientId },
                    { BayeuxConstants.KeyMessageId, messageId++ },
                    { BayeuxConstants.KeyData, message}
                };
                //var publishMessage = new {
                //  channel = channelName,
                //  clientId = ClientId,
                //  id = messageId++,
                //  data = message
                //};

                AddExtensions(publishMessage);

                var publishMessageJson = "[" + JsonConvert.SerializeObject(publishMessage) + "]";
                Info("Publish message: {0}", publishMessageJson);

                WebsocketClient.Send(publishMessageJson);
            }
            catch (Exception ex)
            {
                Error("Publish error: {0}", ex.Message);
            }
        }

        public void AddExtension(IFayeExtension ext)
        {
            if (Extensions == null)
                Extensions = new List<IFayeExtension>();

            Extensions.Add(ext);
        }

        public bool RemoveExtension(IFayeExtension ext)
        {
            if (Extensions == null)
                return false;

            return Extensions.Remove(ext);
        }

        public void ClearExtensions()
        {
            if (Extensions != null)
                Extensions.Clear();
        }
        #endregion

        #region Bayeux protocol messages
        public void SendBayeuxHandshake()
        {
            Info("Sending handshake");
            try
            {
                Dictionary<string, object> handshakeMessage = new Dictionary<string, object>() {
                    { BayeuxConstants.KeyChannel, BayeuxConstants.ChannelHandshake },
                    { BayeuxConstants.KeyVersion, version },
                    { BayeuxConstants.KeyMinimumVersion, minimumVersion },
                    { BayeuxConstants.KeySupportedConnections, new string[] { "long-polling", "callback-polling", "websocket"} }
                };

                var handshakeMessageJson = "[" + JsonConvert.SerializeObject(handshakeMessage) + "]";
                Info("Handshake message: {0}", handshakeMessageJson);

                WebsocketClient.Send(handshakeMessageJson);

                State = ClientState.Connecting;

                _connectionState = ConnectionNegotiationState.HandshakeSent;
            }
            catch (Exception ex)
            {
                Error("Handshake sending error: {0}", ex.Message);
            }
        }

        public void SendBayeuxConnectMessage()
        {
            Info("Sending connect message");
            try
            {

                Dictionary<string, object> connectMessage = new Dictionary<string, object>() {
                    { BayeuxConstants.KeyChannel, BayeuxConstants.ChannelConnect },
                    { BayeuxConstants.KeyClientId, ClientId },
                    { BayeuxConstants.KeyConnectionType, "websocket" }
                };
                //var connectMessage = new {
                //  channel = BayeuxConstants.ChannelConnect,
                //  clientId = ClientId,
                //  connectionType = "websocket"
                //};

                var connectMessageJson = "[" + JsonConvert.SerializeObject(connectMessage) + "]";
                Info("Connect message: {0}", connectMessageJson);

                WebsocketClient.Send(connectMessageJson);

                _connectionState = ConnectionNegotiationState.ConnectionRequest;
            }
            catch (Exception ex)
            {
                Error("Connection error: {0}", ex.Message);

                State = ClientState.Unconnected;
                _connectionState = ConnectionNegotiationState.ConnectionFailure;
            }
        }

        public void SendBayeuxDisconnectMessage()
        {
            Info("Sending disconnect message");
            try
            {
                Dictionary<string, object> disconnectMessage = new Dictionary<string, object>() {
                    { BayeuxConstants.KeyChannel, BayeuxConstants.ChannelDisconnect },
                    { BayeuxConstants.KeyClientId, ClientId }
                };
                //var disconnectMessage = new {
                //  channel = BayeuxConstants.ChannelDisconnect,
                //  clientId = ClientId
                //};

                var disconnectMessageJson = "[" + JsonConvert.SerializeObject(disconnectMessage) + "]";
                Info("Disconnect message: {0}", disconnectMessageJson);

                WebsocketClient.Send(disconnectMessageJson);

                _connectionState = ConnectionNegotiationState.ConnectionRequest;
            }
            catch (Exception ex)
            {
                Error("Disconnection error: {0}", ex.Message);

                State = ClientState.Connected;
            }
        }

        public void SendBayeuxSubscribeMessage(string channel)
        {
            Info("Sending subscribe message to {0}", channel);

            try
            {
                Dictionary<string, object> subscribeMessage = new Dictionary<string, object>() {
                    { BayeuxConstants.KeyChannel, BayeuxConstants.ChannelSubscribe },
                    { BayeuxConstants.KeyClientId, ClientId },
                    { BayeuxConstants.KeySubscription, channel }
                };
                //var subscribeMessage = new {
                //  channel = BayeuxConstants.ChannelSubscribe,
                //  clientId = ClientId,
                //  subscription = channel,
                //  ext = ""
                //};

                AddExtensions(subscribeMessage);

                var subscribeMessageJson = "[" + JsonConvert.SerializeObject(subscribeMessage) + "]";
                Info("Subscribe message: {0}", subscribeMessageJson);

                WebsocketClient.Send(subscribeMessageJson);

            }
            catch (Exception ex)
            {
                Error("Subscription error: {0}", ex.Message);
            }

        }

        public void SendBayeuxUnsubscribeMessage(string channel)
        {
            Info("Sending unsubscribe message to {0}", channel);

            try
            {
                Dictionary<string, object> subscribeMessage = new Dictionary<string, object>() {
                    { BayeuxConstants.KeyChannel, BayeuxConstants.ChannelUnsubscribe },
                    { BayeuxConstants.KeyClientId, ClientId },
                    { BayeuxConstants.KeySubscription, channel }
                };

                //var subscribeMessage = new {
                //  channel = BayeuxConstants.ChannelSubscribe,
                //  clientId = ClientId,
                //  subscription = channel
                //};

                var subscribeMessageJson = "[" + JsonConvert.SerializeObject(subscribeMessage) + "]";
                Info("Subscribe message: {0}", subscribeMessageJson);

                WebsocketClient.Send(subscribeMessageJson);

            }
            catch (Exception ex)
            {
                Error("Subscription error: {0}", ex.Message);
            }
        }

        public void StartHeartbeatMechanism(int timeout)
        {
            Task.Run(async () =>
            {
                PingPong("Starting heartbeat: " + timeout.ToString());
                await Task.Delay(timeout - 1000);

                // detect if we have disconnected, because then we shouldn't send any more connect requests
                if (State == ClientState.Unconnected)
                    return;

                PingPong("Timing out, sending another connect message");
                Connect();
            });
        }

        #endregion

        #region Websockets
        public void ConnectToWebsocket()
        {
            Info("Opening websocket connection");
            if (WebsocketClient == null)
            {
                WebsocketClient = Websockets.WebSocketFactory.Create();

                // subscribe to events
                WebsocketClient.OnOpened += WebsocketClient_OnOpened;
                WebsocketClient.OnClosed += WebsocketClient_OnClosed;
                WebsocketClient.OnDispose += WebsocketClient_OnDispose;
                WebsocketClient.OnMessage += WebsocketClient_OnMessage;
                WebsocketClient.OnError += (obj) =>
                {
                    Error("Websocket error: {0}", obj);
                    DisconnectFromWebSocket();
                    WebsocketDispose();
                    //await Task.Delay(1000); //wait 1 sec to reconnect
                    //Disconnected?.Invoke(this, EventArgs.Empty);
                };
            }

            try
            {
                WebsocketClient.Open(Url);
            }
            catch(Exception ex)
            {
                Error("Websocket Open error: {0}", ex.Message);
            }
        }

        public void DisconnectFromWebSocket()
        {
            Info("Closing websocket connection");
            try
            {
                WebsocketClient.Close();
                WebsocketDispose();
            }
            catch (Exception ex)
            {
                Error("Websocket disconnection error: {0}", ex.Message);
            }
        }

        void WebsocketDispose()
        {
            Info("Cleaning up websocket");

            // unsubscribe to events

            if (WebsocketClient != null)
            {
                WebsocketClient.OnOpened -= WebsocketClient_OnOpened;
                WebsocketClient.OnClosed -= WebsocketClient_OnClosed;
                WebsocketClient.OnDispose -= WebsocketClient_OnDispose;
                WebsocketClient.OnMessage -= WebsocketClient_OnMessage;
            }

            WebsocketClient = null;

            State = ClientState.Unconnected;
        }

        // event handling
        void WebsocketClient_OnOpened()
        {
            Info("Websocket opened");

            if (!IsConnected)
            {
                Info("No previous connection, sending handshake");
                SendBayeuxHandshake();
            }
            else
            {
                Info("Previous connection, sending connect message");
                SendBayeuxConnectMessage();
            }
        }

        void WebsocketClient_OnClosed()
        {
            Info("Websocket closed");

            var handler = Disconnected;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            WebsocketDispose();
        }

        void WebsocketClient_OnDispose(Websockets.IWebSocketConnection obj)
        {
            Info("Websocket disposed");
        }

        void WebsocketClient_OnMessage(string obj)
        {
            Debug("Websocket message received: \"{0}\"", obj);


            try
            {

                var j = JArray.Parse(obj);

                foreach (var message in j)
                {
                    var channelName = message["channel"].Value<string>();

                    // handshake
                    if (channelName == BayeuxConstants.ChannelHandshake)
                    {
                        if (message["successful"].Value<bool>())
                        {
                            ClientId = message["clientId"].Value<string>();
                            _connectionState = ConnectionNegotiationState.HandshakeSuccessful;

                            State = ClientState.Connected;

                            // raise Connected event
                            var handler = Connected;
                            if (handler != null)
                                handler(this, EventArgs.Empty);

                            SendBayeuxConnectMessage();
                        }
                        else
                        {
                            Error("Handshake error: {0}", message["error"].Value<string>());
                            _connectionState = ConnectionNegotiationState.HandshakeFailure;

                            State = ClientState.Unconnected;
                        }


                        // connect
                    }
                    else if (channelName == BayeuxConstants.ChannelConnect)
                    {

                        // check if successful
                        if (message["successful"].Value<bool>())
                        {
                            _connectionState = ConnectionNegotiationState.ConnectionSuccessful;

                            //if (!IsConnected) {
                            StartHeartbeatMechanism(message["advice"]["timeout"].Value<int>());
                            //}

                        }
                        else
                        {
                            Error("Connect error: {0}", message["error"].Value<string>());

                            _connectionState = ConnectionNegotiationState.ConnectionFailure;

                            State = ClientState.Unconnected;
                        }


                        // disconnect
                    }
                    else if (channelName == BayeuxConstants.ChannelDisconnect)
                    {
                        Info("Disconnection response");
                        string error = "";
                        bool successful = message["successful"].Value<bool>();
                        Debug("disconnection successful? {0}", successful);
                        if (!successful)
                        {
                            error = message["error"].Value<string>();
                            Error("Disconnection error: {0}", error);
                        }
                        else
                        {

                            if (Subscriptions != null)
                                Subscriptions.Clear();

                            ClientId = "";
                            State = ClientState.Unconnected;

                            WebsocketClient.Close();
                        }

                        // subscribe
                    }
                    else if (channelName == BayeuxConstants.ChannelSubscribe)
                    {
                        Info("Subscription response for {0}", message["subscription"].Value<string>());
                        string error = "";
                        bool successful = message["successful"].Value<bool>();
                        Debug("Subscription successful? {0}", successful);
                        if (!successful)
                        {
                            error = message["error"].Value<string>();
                        }
                        else
                        {

                            string subscribedChannel = message["subscription"].ToString();

                            Debug("subscribed channel: {0}", subscribedChannel);

                            if (Subscriptions == null)
                            {
                                Subscriptions = new List<string>();
                            }

                            // add channel to Subscriptions
                            if (!Subscriptions.Contains(subscribedChannel))
                                Subscriptions.Add(subscribedChannel);

                            var handler = Subscribed;
                            if (handler != null)
                                handler(this, new SubscriptionEventArgs(subscribedChannel, error));


                        }

                        // unsubscribe
                    }
                    else if (channelName == BayeuxConstants.ChannelUnsubscribe)
                    {
                        Info("Unsubscribe response for {0}", message["subscription"].Value<string>());
                        string error = "";
                        bool successful = message["successful"].Value<bool>();
                        Debug("Unsubscribed successful? {0}", successful);
                        if (!successful)
                        {
                            error = message["error"].Value<string>();
                        }
                        else
                        {

                            string subscribedChannel = message["subscription"].ToString();

                            Debug("channel to unsubscribe: {0}", subscribedChannel);

                            var handler = Unsubscribed;
                            if (handler != null)
                                handler(this, new SubscriptionEventArgs(subscribedChannel, error));


                            if (Subscriptions != null)
                            {
                                // remove channel from Subscriptions
                                if (!Subscriptions.Contains(subscribedChannel))
                                    Subscriptions.Remove(subscribedChannel);
                            }
                        }

                        // message from existing channel subscription
                    }
                    else if (Subscriptions.Contains(channelName))
                    {
                        if (message["data"] != null)
                        {
                            AddExtensions(message);

                            var msg = message["data"].ToString();
                            Debug("Got message from [{0}]", channelName);
                            Debug("Message: {0}", msg);

                            MessageReceived?.Invoke(this, new FayeMessageEventArgs(channelName, msg));
                        }
                        if (message["successful"] != null)
                        {
                            bool successful = message["successful"].Value<bool>();
                            var msgId = message["id"].Value<string>();
                            Debug("Message {0} published successfully? {1}", msgId, successful);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error("Error parsing message: {0}", ex.Message);
                Error("Offending message: {0}", ex.Data);
            }


        }

        #endregion

        #region Private methods
        protected void AddExtensions(Dictionary<string, object> msg, bool outgoing = true)
        {
            if (Extensions == null)
                return;

            foreach (var extension in Extensions)
            {
                Info("Adding extension... " + extension.ToString());
                if (outgoing)
                {
                    extension.AddOutgoingExtension(ref msg);
                    PrettyPrintDict(msg);
                }
            }
        }

        protected void AddExtensions(JToken msg, bool outgoing = false)
        {
            if (Extensions == null)
                return;

            foreach (var extension in Extensions)
            {
                Info("Adding extension... " + extension.ToString());
                if (!outgoing)
                {
                    extension.AddIncomingExtension(ref msg);
                    PrettyPrintDict(msg);
                }
            }
        }

        private void PrettyPrintDict(Dictionary<string, object> msg)
        {
            var json = JsonConvert.SerializeObject(msg);
            Info("Dictionary: {0}", json);
        }

        private void PrettyPrintDict(JToken msg)
        {
            var json = JsonConvert.SerializeObject(msg);
            Info("Dictionary: {0}", json);
        }

        // logging
        private int _loggerLevel = 1;
        string fayeName = String.Empty;
        private void Log(int logLevel, string format, params object[] values)
        {
            if (_loggerLevel >= logLevel)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss tt") + " [FayeClient] {0}", string.Format(format, values));
#else
                var str = String.Format("[FayeClient][{0}] {1}",fayeName, string.Format(format, values));
                App.Log(str);
#endif
            }

        }

        private void Info(string format, params object[] values)
        {
            Log(2, format, values);
        }

        private void Debug(string format, params object[] values)
        {
            Log(3, format, values);
        }

        private void PingPong(string format, params object[] values)
        {
            Log(1, format, values);
        }

        private void Error(string format, params object[] values)
        {
            Log(0, format, values);
        }

        private string InsertEpoch(string payload)
        {
            return payload.Insert(payload.Length - 1, ",\"e\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }
        #endregion
    }
}
