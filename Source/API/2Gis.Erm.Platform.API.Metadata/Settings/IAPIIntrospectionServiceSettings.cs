using System;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Metadata.Settings
{
    public interface IAPIIntrospectionServiceSettings : ISettings
    {
        Uri RestUrl { get; }
        Uri BaseUrl { get; }
    }
}