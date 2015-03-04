using System;

using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Settings
{
    public interface IAPIOperationsServiceSettings : ISettings
    {
        Uri RestUrl { get; }
        Uri BaseUrl { get; }
    }
}