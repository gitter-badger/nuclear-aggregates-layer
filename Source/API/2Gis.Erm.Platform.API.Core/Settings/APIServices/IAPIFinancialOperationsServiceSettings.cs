using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public interface IAPIFinancialOperationsServiceSettings : IAPIServiceSettings
    {
        Uri RestUrl { get; }
        string SoapEndpointName { get; }
        Uri BaseUrl { get; }
    }
}