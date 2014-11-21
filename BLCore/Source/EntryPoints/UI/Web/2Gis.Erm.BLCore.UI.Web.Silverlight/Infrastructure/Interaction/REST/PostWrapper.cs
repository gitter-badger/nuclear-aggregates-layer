using System;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Interaction.REST
{
    public class PostWrapper : RestWrapper
    {
        public PostWrapper(string url, object route, string baseUri, RestService restService)
            : base(url, route, baseUri, restService)
        {
        }

        public PostWrapper IgnoreResponse()
        {
            return WhenFinished((state) => { });
        }

        public PostWrapper WhenFinished(Action<object> action)
        {
            Action<object> callback =
            state =>
            {
                action(state);
                if (NextContainer != null)
                {
                    NextContainer.Go();
                }
            };

            _post = () => RestService.Post(URL, Route, callback);

            return this;
        }

        public PostWrapper WhenFinished<T>(Action<T> action) where T : class, new()
        {
            Action<T> callback =
            t =>
            {
                action(t);
                if (NextContainer != null)
                {
                    NextContainer.Go();
                }
            };

            _post = () => RestService.Post(URL, Route, callback);

            return this;
        }

        private Action _post;

        public override void Go()
        {
            _post();
        }
    }
}
