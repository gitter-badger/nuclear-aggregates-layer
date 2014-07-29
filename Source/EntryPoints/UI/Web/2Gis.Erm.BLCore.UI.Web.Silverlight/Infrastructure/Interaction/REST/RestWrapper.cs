namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Interaction.REST
{
    public abstract class RestWrapper
    {
        private readonly string _baseUri;
        private readonly RestService _restService;
        private readonly string _url;
        private RestWrapper _nextContainer;

        protected RestWrapper(string url, object route, string baseUri, RestService restService)
        {
            _url = url;
            Route = route;
            _baseUri = baseUri;
            _restService = restService;
        }

        protected RestWrapper NextContainer
        {
            get
            {
                return _nextContainer;
            }
        }
        protected RestService RestService
        {
            get
            {
                return _restService;
            }
        }
        protected object Route { get; set; }
        protected string URL
        {
            get
            {
                return _url;
            }
        }

        public GetWrapper<T> ThenGet<T>(string url, object route) where T : class, new()
        {
            _nextContainer = new GetWrapper<T>(url, route, _baseUri, RestService);
            return NextContainer as GetWrapper<T>;
        }

        public GetWrapper<T> ThenGet<T>(string url) where T : class, new()
        {
            _nextContainer = new GetWrapper<T>(url, null, _baseUri, RestService);
            return NextContainer as GetWrapper<T>;
        }

        public PostWrapper ThenPost(string url, object route)
        {
            _nextContainer = new PostWrapper(url, route, _baseUri, RestService);
            return NextContainer as PostWrapper;
        }

        public PostWrapper ThenPost(string url)
        {
            _nextContainer = new PostWrapper(url, null, _baseUri, RestService);
            return NextContainer as PostWrapper;
        }

        public PostWrapper ThenPostAndExpect<T>(string url, object route) where T : class, new()
        {
            _nextContainer = new PostWrapper(url, route, _baseUri, RestService);
            return NextContainer as PostWrapper;
        }

        public PostWrapper ThenPostAndExpect<T>(string url) where T : class, new()
        {
            _nextContainer = new PostWrapper(url, null, _baseUri, RestService);
            return NextContainer as PostWrapper;
        }

        public abstract void Go();
    }
}
