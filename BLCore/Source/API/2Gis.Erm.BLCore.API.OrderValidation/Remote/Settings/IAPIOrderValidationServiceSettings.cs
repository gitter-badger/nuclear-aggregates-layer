using System;

using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings
{
    public interface IAPIOrderValidationServiceSettings : ISettings
    {
        Uri RestUrl { get; }
        string SoapEndpointName { get; }
        Uri BaseUrl { get; }
    }
}