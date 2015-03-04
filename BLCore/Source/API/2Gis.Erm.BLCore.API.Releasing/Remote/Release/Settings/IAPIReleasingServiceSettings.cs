using System;

using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Releasing.Remote.Release.Settings
{
    public interface IAPIReleasingServiceSettings : ISettings
    {
        Uri RestUrl { get; }
        string SoapEndpointName { get; }
        Uri BaseUrl { get; }
    }
}