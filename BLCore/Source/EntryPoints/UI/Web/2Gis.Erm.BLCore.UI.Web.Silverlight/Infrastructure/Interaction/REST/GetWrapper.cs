using System;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Interaction.REST
{
    public class GetWrapper<T> : RestWrapper where T : class, new()
    {
        public GetWrapper(string url, object route, string baseUri, RestService restService)
            : base(url, route, baseUri, restService)
        {
        }

        private Action<T> _callback;
        private Func<Exception, bool> _failureCallback;

        public GetWrapper<T> WhenFinished(Action<T> action)
        {
            _callback =
            t =>
            {
                action(t);
                if (NextContainer != null)
                {
                    NextContainer.Go();
                }
            };

            return this;
        }

        public GetWrapper<T> IfFailure(Func<Exception, bool> failureCallback)
        {
            _failureCallback = failureCallback;
            return this;
        }

        private Func<object> _deferredRouteRetrieval;

        public GetWrapper<T> ForRoute(Func<object> deferredRouteRetrieval)
        {
            _deferredRouteRetrieval = deferredRouteRetrieval;
            return this;
        }

        public override void Go()
        {
            if (_deferredRouteRetrieval != null)
            {
                Route = _deferredRouteRetrieval();
            }

            RestService.Get(URL, Route, _callback, _failureCallback);
        }
    }
}
