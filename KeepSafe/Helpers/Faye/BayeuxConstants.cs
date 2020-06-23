using System;
namespace KeepSafe.Helpers.Faye
{
	public static class BayeuxConstants
	{
		public static string ChannelHandshake = "/meta/handshake";
		public static string ChannelConnect = "/meta/connect";
		public static string ChannelDisconnect = "/meta/disconnect";
		public static string ChannelSubscribe = "/meta/subscribe";
		public static string ChannelUnsubscribe = "/meta/unsubscribe";

		public static string KeyChannel = "channel";
		public static string KeyClientId = "clientId";
		public static string KeyMessageId = "id";
		public static string KeyData = "data";
		public static string KeySubscription = "subscription";
		public static string KeyExtension = "ext";
		public static string KeyVersion = "version";
		public static string KeyMinimumVersion = "minimumVersion";
		public static string KeySupportedConnections = "supportedConnectionTypes";
		public static string KeyConnectionType = "connectionType";

	}
}
