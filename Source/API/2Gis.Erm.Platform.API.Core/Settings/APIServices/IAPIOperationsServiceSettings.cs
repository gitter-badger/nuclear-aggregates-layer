using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public interface IAPIOperationsServiceSettings : IAPIServiceSettings
    {
        Uri RestUrl { get; }
        Uri BaseUrl { get; }
    }
}