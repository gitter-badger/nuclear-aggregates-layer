using System;
using System.IO;
using System.Net;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Interaction.REST
{
    public static class RestFacilitator
    {
        public static void Get<T>(string resource, Action<T> onComplete, Func<Exception, bool> onFailure, string authorization) 
            where T : class, new()
        {
            string uriPath = resource;

            var uri = new Uri(uriPath, UriKind.Absolute);

            var httpWebRequest = WebRequest.CreateHttp(uri);

            ApplyAuthorization(httpWebRequest, authorization);

            var responseReady = new AsyncCallback(
            e =>
            {
                var requestResource = resource;
                var request = (HttpWebRequest)e.AsyncState;

                HttpWebResponse response = null;
                T result;

                try
                {
                    try
                    {
                        response = (HttpWebResponse)request.EndGetResponse(e);
                    }
                    catch (Exception ex)
                    {
                        if (onFailure != null)
                        {
                            var isHandled = onFailure(ex);
                            if (!isHandled)
                            {
                                throw;
                            }

                            return;
                        }

                        throw new InvalidOperationException("Unable to call: " + requestResource + ".", ex);
                    }
                    
                    using (Stream stream = response.GetResponseStream())
                    {
                        result = new RestConverter().ConstructObject<T>(stream);
                    }
                }
                finally
                {
                    if (response != null)
                    {
                        response.Dispose();
                    }
                }

                onComplete(result);
            });

            httpWebRequest.BeginGetResponse(responseReady, httpWebRequest);
        }

        public static void Post<T>(string resource, string jsonString, Action<T> onComplete, string authorization) 
            where T : class, new()
        {
            string uriPath = resource;

            var uri = new Uri(uriPath, UriKind.Absolute);

            var request = WebRequest.CreateHttp(uri);

            request.Method = "POST";

            ApplyAuthorization(request, authorization);
            
            var readResponse = new AsyncCallback(
            e =>
            {
                T result;
                using (var response = (HttpWebResponse)request.EndGetResponse(e))
                {
                    using (var stream = response.GetResponseStream())
                    {
                        result = new RestConverter().ConstructObject<T>(stream);
                    }
                }

                onComplete(result);
            });

            var writeRequest = new AsyncCallback(
            e =>
            {
                request.ContentType = "application/json";

                using (var stream = request.EndGetRequestStream(e))
                {
                    using (var streamWriter = new StreamWriter(stream))
                    {
                        streamWriter.Write(jsonString);
                    }
                }

                request.BeginGetResponse(readResponse, null);
            });

            request.BeginGetRequestStream(writeRequest, null);
        }

        public static void Post(string resource, string jsonString, Action<object> onComplete, string authorization)
        {
            string uriPath = resource;

            var uri = new Uri(uriPath, UriKind.Absolute);

            var request = WebRequest.CreateHttp(uri);

            ApplyAuthorization(request, authorization);

            request.Method = "POST";

            var readResponse = new AsyncCallback(e => onComplete(null));

            var writeRequest = new AsyncCallback(
            e =>
            {
                request.ContentType = "application/json";

                using (var stream = request.EndGetRequestStream(e))
                {
                    using (var streamWriter = new StreamWriter(stream))
                    {
                        streamWriter.Write(jsonString);
                    }
                }

                request.BeginGetResponse(readResponse, null);
            });

            request.BeginGetRequestStream(writeRequest, null);
        }

        private static void ApplyAuthorization(HttpWebRequest request, string authorization)
        {
            if (string.IsNullOrEmpty(authorization))
            {
                return;
            }

            request.Headers[HttpRequestHeader.ProxyAuthorization] = authorization;
        }
    }
}
