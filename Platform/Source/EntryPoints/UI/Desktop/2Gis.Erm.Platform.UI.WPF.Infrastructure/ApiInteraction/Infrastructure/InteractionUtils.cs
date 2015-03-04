using System;
using System.IO;
using System.Linq;
using System.Net;

using Newtonsoft.Json;

using Nuclear.Tracing.API;

using RestSharp;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure
{
    public static class InteractionUtils
    {
        public static IRestRequest ToRestRequest(this ApiRequest apiRequest, Method transferMethod)
        {
            var request = new RestRequest(apiRequest.APIEndpointAndResources, transferMethod);

            request.Parameters.AddRange(apiRequest.RequestParameters.Select(p => new Parameter {Name = p.Key, Value = p.Value, Type = ParameterType.GetOrPost}));

            if (apiRequest.FromInstancesParams != null)
            {
                foreach (var param in apiRequest.FromInstancesParams)
                {
                    request.AddObject(param);
                }
            }

            if (apiRequest.Content != null)
            {
                request.AddBody(apiRequest.Content);
            }

            return request;
        }

        public static ApiResponse<TResultData> ToApiResponse<TResultData>(this IRestResponse<TResultData> response) 
            where TResultData : new()
        {
            return new ApiResponse<TResultData>(
                response.IsSuccessfull(),
                (int)response.StatusCode,
                response.Content,
                response.Data) { ErrorException = response.ErrorException, ErrorMessage = response.ErrorMessage, StatusDescription = response.StatusDescription };
        }

        public static ApiResponse ToApiResponse(this IRestResponse response)
        {
            string responseContent = null;

            if (response.RawBytes != null && response.RawBytes.Length > 0)
            {
                // В response.Content не убран ByteOrderMark, что впоследствии приводит к фрустрации JObject.Parse. 
                // Так что строку с ответом получаем из RawBytes с помощью StreamReader.
                var ms = new MemoryStream(response.RawBytes, 0, response.RawBytes.Length, false);
                using (var reader = new StreamReader(ms, true))
                {
                    responseContent = reader.ReadToEnd();
                }
            }

            return new ApiResponse(
                        response.IsSuccessfull(),
                        (int)response.StatusCode,
                        responseContent)
                        {
                            ErrorException = response.ErrorException,
                            ErrorMessage = response.ErrorMessage,
                            StatusDescription = response.StatusDescription
                        };
        }

        private static bool IsSuccessfull(this IRestResponse response)
        {
            return response.ResponseStatus == ResponseStatus.Completed && response.StatusCode == HttpStatusCode.OK;
        }

        public static void IfErrorThanReportAndThrowException(this ApiResponse response, string operationDescription, ITracer logger = null)
        {
            if (!response.IsSuccessfull)
            {
                ApiExceptionDescriptor exceptionDescription = null;
                if (!string.IsNullOrEmpty(response.ResultContent))
                {
                    try
                    {
                        exceptionDescription = JsonConvert.DeserializeObject<ApiExceptionDescriptor>(response.ResultContent);
                    }
                    catch (Exception ex)
                    {
                        if (logger != null)
                        {
                            logger.ErrorFormat(ex, "Can't deserialize api exception detail");
                        }
                    }
                }

                var topLevelErrorDescription = "Api operation execution failed. " + operationDescription;
                var exception = new ApiException(topLevelErrorDescription, response.ErrorException) { ApiExceptionDescription = exceptionDescription };
                
                if (logger != null)
                {
                    logger.Error(exception, topLevelErrorDescription);
                }

                throw exception;
            }
        }
    }
}
