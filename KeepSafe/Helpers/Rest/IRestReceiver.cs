using System;
using Newtonsoft.Json.Linq;

namespace KeepSafe
{
    public interface IRestReceiver
    {
        void ReceiveJSONData(JObject jsonData, int wsType);
        void ReceiveError(string title, string error, int wsType);
    }
}
