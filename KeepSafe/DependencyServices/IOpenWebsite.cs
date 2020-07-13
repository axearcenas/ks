using System;
using System.Threading.Tasks;

namespace KeepSafe.DependencyServices
{
    public interface IOpenWebsite
    {
        Task OpenUrl(string uri);
    }
}
