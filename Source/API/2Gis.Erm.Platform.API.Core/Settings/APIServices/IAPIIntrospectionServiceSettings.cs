using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public interface IAPIIntrospectionServiceSettings : IAPIServiceSettings
    {
        Uri RestUrl { get; }
        Uri BaseUrl { get; }
    }
}