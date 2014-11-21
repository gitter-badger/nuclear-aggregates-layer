using System;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Interaction.REST
{
    public interface IRestService
    {
        string BaseUri { get; }
        string Authorization { get; }

        void Get<T>(string url, object resource, Action<T> completeCallback, Func<Exception, bool> failedCallback) 
            where T : class, new();

        void Post<T>(string url, object data, Action<T> completeCallback) 
            where T : class, new();

        void Post(string url, object data, Action<object> completeCallback);
    }
}