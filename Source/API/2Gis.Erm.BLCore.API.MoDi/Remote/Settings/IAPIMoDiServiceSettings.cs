using System;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings
{
    public interface IAPIMoDiServiceSettings : ISettings
    {
        Uri RestUrl { get; }
        string SoapEndpointName { get; }
        Uri BaseUrl { get; }
    }
}