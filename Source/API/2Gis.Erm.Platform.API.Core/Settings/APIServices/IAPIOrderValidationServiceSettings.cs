using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public interface IAPIOrderValidationServiceSettings : IAPIServiceSettings
    {
        Uri RestUrl { get; }
        string SoapEndpointName { get; }
        Uri BaseUrl { get; }
    }
}