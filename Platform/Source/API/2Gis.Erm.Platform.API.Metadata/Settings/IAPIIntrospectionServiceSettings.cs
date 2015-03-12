using System;

using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Metadata.Settings
{
    public interface IAPIIntrospectionServiceSettings : ISettings
    {
        Uri RestUrl { get; }
        Uri BaseUrl { get; }
    }
}