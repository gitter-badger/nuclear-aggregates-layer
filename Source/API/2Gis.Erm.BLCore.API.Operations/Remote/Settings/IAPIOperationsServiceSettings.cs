using System;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Settings
{
    public interface IAPIOperationsServiceSettings : ISettings
    {
        Uri RestUrl { get; }
        Uri BaseUrl { get; }
    }
}