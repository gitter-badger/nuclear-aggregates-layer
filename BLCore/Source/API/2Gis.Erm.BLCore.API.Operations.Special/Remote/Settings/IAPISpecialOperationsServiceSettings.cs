using System;

using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings
{
    public interface IAPISpecialOperationsServiceSettings : ISettings
    {
        Uri RestUrl { get; }
        string SoapEndpointName { get; }
        Uri BaseUrl { get; }
    }
}