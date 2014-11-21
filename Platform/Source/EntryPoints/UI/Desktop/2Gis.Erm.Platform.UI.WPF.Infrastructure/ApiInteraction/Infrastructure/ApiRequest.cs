using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure
{
    public sealed class ApiRequest
    {
        private readonly string _apiEndpointAndResources;
        private readonly Dictionary<string, object> _requestParameters = new Dictionary<string, object>();

        private readonly List<object> _fromInstancesParams = new List<object>();

        public ApiRequest(string apiEndpointAndResources)
        {
            _apiEndpointAndResources = apiEndpointAndResources;
        }

        public void AddParameter(string name, string value)
        {
            _requestParameters.Add(name, value);
        }

        /// <summary>
        /// нужно передавать делегат (например, л€мбду) коотрый из экземпл€ра instance нужные свойства может поместить в NameValueCollection
        /// </summary>
        public void AddParametersFromInstance<TInstance>(TInstance instance, Action<TInstance, IDictionary<string, object>> paramsExtractor)
        {
            if (paramsExtractor == null)
            {
                throw new ArgumentNullException("paramsExtractor");
            }

            paramsExtractor(instance, _requestParameters);
        }

        public void AddParametersFromInstance(object instance)
        {
            _fromInstancesParams.Add(instance);
        }

        public void AddParameters(IEnumerable<KeyValuePair<string, object>> requestParameters)
        {
            foreach (var requestParameter in requestParameters)
            {
                _requestParameters.Add(requestParameter.Key, requestParameter.Value);
            }            
        }

        public object Content { get; set; }
        public string APIEndpointAndResources
        {
            get
            {
                return _apiEndpointAndResources;
            }
        }

        public IEnumerable<object> FromInstancesParams
        {
            get
            {
                return _fromInstancesParams;
            }
        }

        public IEnumerable<KeyValuePair<string, object>> RequestParameters
        {
            get
            {
                return _requestParameters;
            }
        }
    }
}