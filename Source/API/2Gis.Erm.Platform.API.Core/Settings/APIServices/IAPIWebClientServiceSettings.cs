using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    /// <summary>
    /// Настройки для доступа к web application обслуживающему web клиент ERM, 
    /// </summary>
    public interface IAPIWebClientServiceSettings : IAPIServiceSettings
    {
        Uri Url { get; }
    }
}