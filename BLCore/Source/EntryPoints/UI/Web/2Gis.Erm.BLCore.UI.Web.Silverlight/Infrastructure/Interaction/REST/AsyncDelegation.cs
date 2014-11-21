namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Interaction.REST
{
    public class AsyncDelegation
    {
        private readonly RestService _restService;
        private RestWrapper _restServiceContainer;
        
        public string BaseUri { get; set; }

        public AsyncDelegation(RestService restService)
        {
            _restService = restService;
            BaseUri = restService.BaseUri;
        }

        public GetWrapper<T> Get<T>(string url, object route) where T : class, new()
        {
            _restServiceContainer = new GetWrapper<T>(url, route, BaseUri, _restService);
            return _restServiceContainer as GetWrapper<T>;
        }

        public GetWrapper<T> Get<T>(string url) where T : class, new()
        {
            _restServiceContainer = new GetWrapper<T>(url, null, BaseUri, _restService);
            return _restServiceContainer as GetWrapper<T>;
        }

        public PostWrapper Post(string url, object route)
        {
            _restServiceContainer = new PostWrapper(url, route, BaseUri, _restService);
            return _restServiceContainer as PostWrapper;
        }

        public PostWrapper Post(string url)
        {
            _restServiceContainer = new PostWrapper(url, null, BaseUri, _restService);
            return _restServiceContainer as PostWrapper;
        }

        public void Go()
        {
            if (_restServiceContainer != null)
            {
                _restServiceContainer.Go();
            }
        }
    }
}
