using System;

using NuClear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings
{
    public interface IAPIMoDiServiceSettings : ISettings
    {
        Uri RestUrl { get; }
        string SoapEndpointName { get; }
        Uri BaseUrl { get; }
    }
}