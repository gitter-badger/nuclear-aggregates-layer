using System;
using System.Threading;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Interaction.REST
{
    public class SynchronousCallRestService : IRestService
    {
        private class OnSuccessProxy<TState>
        {
            private readonly Action<TState> _callback;
            private readonly ManualResetEvent _synchronizationEvent;

            public OnSuccessProxy(Action<TState> callback, ManualResetEvent synchronizationEvent)
            {
                _callback = callback;
                _synchronizationEvent = synchronizationEvent;
            }

            public void Callback(TState state)
            {
                _callback(state);
                try
                {
                    _synchronizationEvent.Set();
                }
                catch
                {
                }
            }
        }

        private class OnErrorProxy
        {
            private readonly Func<Exception, bool> _failureCallback;
            private readonly ManualResetEvent _synchronizationEvent;

            public OnErrorProxy(Func<Exception, bool> failureCallback, ManualResetEvent synchronizationEvent)
            {
                _failureCallback = failureCallback;
                _synchronizationEvent = synchronizationEvent;
            }

            public bool Callback(Exception exception)
            {
                if (!_failureCallback(exception))
                {
                    return false;
                }

                try
                {
                    _synchronizationEvent.Set();
                }
                catch
                {
                }
                return true;
            }
        }

        private readonly RestService _restService;

        public SynchronousCallRestService(string baseUri, string authorization = null)
        {
            _restService = new RestService(baseUri, authorization);
        }

        #region Implementation of IRestService

        public string BaseUri
        {
            get
            {
                return _restService.BaseUri;
            }
        }

        public string Authorization
        {
            get
            {
                return _restService.Authorization;
            }
        }

        private const int DefaultCallTimeoutMs = 60000;

        public void Get<T>(string url, object resource, Action<T> completeCallback, Func<Exception, bool> failedCallback) 
            where T : class, new()
        {
            using (var synchEvent = new ManualResetEvent(false))
            {
                _restService.Get<T>(
                    url, 
                    resource, 
                    (new OnSuccessProxy<T>(completeCallback, synchEvent)).Callback,
                    failedCallback != null ? (new OnErrorProxy(failedCallback, synchEvent)).Callback : (Func<Exception, bool>)null);
                var isWaitSuccessful = synchEvent.WaitOne(DefaultCallTimeoutMs);
                if (!isWaitSuccessful)
                {
                    bool isThrowRequired = true;
                    var exception = new TimeoutException("Timeout expired: " + DefaultCallTimeoutMs + "ms. Can't get requested (GET request) resource from url: " + url
                                  + ". Resource: " + resource);
                    if (failedCallback != null)
                    {
                        isThrowRequired = !failedCallback(exception);
                    }

                    if (isThrowRequired)
                    {
                        throw exception;
                    }
                }
            }
        }

        public void Post<T>(string url, object data, Action<T> completeCallback) 
            where T : class, new()
        {
            using (var synchEvent = new ManualResetEvent(false))
            {
                _restService.Post<T>(
                    url,
                    data,
                    (new OnSuccessProxy<T>(completeCallback, synchEvent)).Callback);
                var isWaitSuccessful = synchEvent.WaitOne(DefaultCallTimeoutMs);
                if (!isWaitSuccessful)
                {
                    throw new TimeoutException("Timeout expired: " + DefaultCallTimeoutMs + "ms. Can't get requested (POST request) resource from url: " + url);
                }
            }
        }

        public void Post(string url, object data, Action<object> completeCallback)
        {
            using (var synchEvent = new ManualResetEvent(false))
            {
                _restService.Post(
                    url,
                    data,
                    (new OnSuccessProxy<object>(completeCallback, synchEvent)).Callback);
                var isWaitSuccessful = synchEvent.WaitOne(DefaultCallTimeoutMs);
                if (!isWaitSuccessful)
                {
                    throw new TimeoutException("Timeout expired: " + DefaultCallTimeoutMs + "ms. Can't get requested (POST request) resource from url: " + url);
                }
            }
        }

        #endregion
    }
}
