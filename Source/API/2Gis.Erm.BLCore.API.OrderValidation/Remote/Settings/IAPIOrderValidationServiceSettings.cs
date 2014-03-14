using System;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings
{
    public interface IAPIOrderValidationServiceSettings : ISettings
    {
        Uri RestUrl { get; }
        string SoapEndpointName { get; }
        Uri BaseUrl { get; }
    }
}