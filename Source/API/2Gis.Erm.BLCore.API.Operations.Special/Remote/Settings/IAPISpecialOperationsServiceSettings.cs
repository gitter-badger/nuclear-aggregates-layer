using System;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings
{
    public interface IAPISpecialOperationsServiceSettings : ISettings
    {
        Uri RestUrl { get; }
        string SoapEndpointName { get; }
        Uri BaseUrl { get; }
    }
}