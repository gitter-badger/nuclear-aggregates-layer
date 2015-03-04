using System;

using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Metadata.Settings
{
    // TODO {i.maslennikov, 05.03.2014}: Во всех API*Settings есть  RestUrl и BaseUrl, кое-где есть еще SoapEndpointName. Может выделить для них специфичный ISettings-контракт?
    public interface IAPIIdentityServiceSettings : ISettings
    {
        Uri RestUrl { get; }
        string SoapEndpointName { get; }
        Uri BaseUrl { get; }
    }
}