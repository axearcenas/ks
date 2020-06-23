using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace KeepSafe.Helpers.Faye
{
	public class EncryptDecryptInator : IFayeExtension
	{
		public void AddOutgoingExtension(ref Dictionary<string, object> message)
		{
			if (message.ContainsKey(BayeuxConstants.KeyData))
			{
				message[BayeuxConstants.KeyData] = message[BayeuxConstants.KeyData].ToString().EncryptEncode();
				App.Log("Encrypted Data: " + message[BayeuxConstants.KeyData]);
			}
		}

		public void AddIncomingExtension(ref JToken message)
		{
			if (message[BayeuxConstants.KeyData] != null)
			{
				message[BayeuxConstants.KeyData] = message[BayeuxConstants.KeyData].ToString().DecodeDecrypt();
				App.Log("Decrypted Data: " + message[BayeuxConstants.KeyData]);
			}
		}
	}
}
