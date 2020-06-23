using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace KeepSafe.Helpers.Faye
{
	public interface IFayeExtension
	{
		void AddOutgoingExtension(ref Dictionary<string, object> message);
		void AddIncomingExtension(ref JToken message);
	}
}
