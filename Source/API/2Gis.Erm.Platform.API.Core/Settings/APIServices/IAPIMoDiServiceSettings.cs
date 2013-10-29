using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public interface IAPIMoDiServiceSettings : IAPIServiceSettings
    {
        Uri RestUrl { get; }
        string SoapEndpointName { get; }
        Uri BaseUrl { get; }
    }
}