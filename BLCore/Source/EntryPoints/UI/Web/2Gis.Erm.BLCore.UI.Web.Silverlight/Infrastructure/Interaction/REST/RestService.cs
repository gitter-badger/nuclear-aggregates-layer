using System;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Interaction.REST
{
    public class RestService : IRestService
    {
        private readonly string _baseUri;
        public string BaseUri
        {
            get
            {
                return _baseUri;
            }
        }

        private readonly string _authorization;
        public string Authorization
        {
            get
            {
                return _authorization;
            }
        }

        public RestService(string baseUri, string authorization = null)
        {
            _baseUri = baseUri;
            _authorization = authorization;
            if (_baseUri.EndsWith("/") == false)
            {
                _baseUri = _baseUri + "/";
            }
        }

        private string GenerateGetUri(string url, object data)
        {
            var sb = new StringBuilder();
            sb.Append(BaseUri);

            if (!string.IsNullOrEmpty(url))
            {
                if (url != "/")
                {
                    sb.Append(url);
                }

                if (url.EndsWith("/") == false && !url.Contains("."))
                {
                    sb.Append("/");
                }
            }

            if (data != null)
            {
                bool isFirst = true;
                var propertyInfo = data.GetType().GetProperties().ToList();
                foreach (var property in propertyInfo)
                {
                    string value = property.GetValue(data, null).ToString();
                    if (isFirst)
                    {
                        sb.Append("?");
                        isFirst = false;
                    }
                    else
                    {
                        sb.Append("&");
                    }

                    sb.Append(property.Name);
                    sb.Append("=");
                    sb.Append(Uri.EscapeDataString(value));
                }
            }

            return sb.ToString();
        }

        private string GeneratePostUri(string url)
        {
            var sb = new StringBuilder();
            sb.Append(BaseUri);

            if (!string.IsNullOrEmpty(url))
            {
                if (url != "/")
                {
                    sb.Append(url);
                }

                if (url.EndsWith("/") == false)
                {
                    sb.Append("/");
                }
            }

            return sb.ToString();
        }
        
        public void Get<T>(string url, object resource, Action<T> completeCallback, Func<Exception, bool> failedCallback) 
            where T : class, new()
        {
            url = url ?? string.Empty;

            string fullyQualifiedUri = GenerateGetUri(url, resource);

            RestFacilitator.Get(fullyQualifiedUri, completeCallback, failedCallback, _authorization);
        }

        public void Post<T>(string url, object data, Action<T> completeCallback) 
            where T : class, new()
        {
            string fullyQualifiedUri = GeneratePostUri(url);

            string jsonString;

            try
            {
                jsonString = JsonConvert.SerializeObject(data);
            }
            catch (JsonSerializationException ex)
            {
                throw new InvalidOperationException("A serialization exception occurred.  Make sure you have [assembly: InternalsVisibleTo(\"Newtonsoft.Json\")] in your AssemblyInfo.cs file so that anonymous types can be serialized by JSON.net.", ex);
            }

            RestFacilitator.Post(fullyQualifiedUri, jsonString, completeCallback, _authorization);
        }

        public void Post(string url, object data, Action<object> completeCallback)
        {
            string fullyQualifiedUri = GeneratePostUri(url);

            string jsonString;

            try
            {
                jsonString = JsonConvert.SerializeObject(data);
            }
            catch (JsonSerializationException ex)
            {
                throw new InvalidOperationException("A serialization exception occurred.  Make sure you have [assembly: InternalsVisibleTo(\"Newtonsoft.Json\")] in your AssemblyInfo.cs file so that anonymous types can be serialized by JSON.net.", ex);
            }

            RestFacilitator.Post(fullyQualifiedUri, jsonString, completeCallback, _authorization);
        }
    }
}
