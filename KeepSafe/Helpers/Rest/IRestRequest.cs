using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;

namespace KeepSafe.Rest
{
    public interface IRestRequest
    {
        void SetDelegate(IRestReceiver weakReference);
        Task GetRequestIgnoreSSL(string url, CancellationToken ct, int wsType, string authHeader = null, int timeout = 100, bool pingHost = false, bool ignoreNoInternet = false, bool ignoreTimeout = false);
        Task GetRequest(string url, CancellationToken ct, int wsType, string authHeader = null, int timeout = 100, bool pingHost = false, bool ignoreNoInternet = false, bool ignoreTimeout = false);
        Task PostRequestAsync(string url, string dictionary, CancellationToken ct, int wsType, string authHeader = null, int timeout = 100, bool pingHost = false, bool ignoreNoInternet = false, bool ignoreTimeout = false);
        Task PutRequestAsync(string url, string dictionary, CancellationToken ct, int wsType, string authHeader = null, int timeout = 100, bool pingHost = false, bool ignoreNoInternet = false, bool ignoreTimeout = false);
        Task DeleteRequestAsync(string url, CancellationToken ct, int wsType, string authHeader = null, int timeout = 100, bool pingHost = false, bool ignoreNoInternet = false, bool ignoreTimeout = false);
        Task MultiPartFormRequestAsync(string url, string jsonString, Dictionary<string, Stream> streams, CancellationToken ct, HttpMethod restMethod, int wsType, string authHeader = null, int timeout = 100, bool pingHost = false, bool ignoreNoInternet = false, bool ignoreTimeout = false, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "");
    }
}
