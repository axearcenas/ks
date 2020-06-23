using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace KeepSafe.Helpers.Faye
{
	public class FayeAuthExtension : IFayeExtension
	{
		public void AddOutgoingExtension(ref Dictionary<string, object> message)
		{
			if (message["channel"].ToString() == BayeuxConstants.ChannelSubscribe)
			{
				if (message.ContainsKey("ext"))
				{
					var extension = (Dictionary<string, object>)message["ext"];

					var playerObj = new Dictionary<string, object>();

					playerObj["access_token"] = DataClass.GetInstance.Token;
					playerObj["client"] = DataClass.GetInstance.ClientId;
					playerObj["uid"] = DataClass.GetInstance.Uid;

					extension["token"] = playerObj;
					//extension["token"] = "so_fucking_lazy"; // playerObj
				}
				else
				{
					var extension = new Dictionary<string, object>();

					var playerObj = new Dictionary<string, object>();

					playerObj["access_token"] = DataClass.GetInstance.Token;
					playerObj["client"] = DataClass.GetInstance.ClientId;
					playerObj["uid"] = DataClass.GetInstance.Uid;

					extension["token"] = playerObj;
					//extension["token"] = "so_fucking_lazy";

					message["ext"] = extension;
				}
			}
		}

		public void AddIncomingExtension(ref JToken message)
		{

		}
	}
}
