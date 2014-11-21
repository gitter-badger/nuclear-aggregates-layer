using System;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Settings
{
    /// <summary>
    /// Настройки для доступа к web application обслуживающему web клиент ERM, 
    /// </summary>
    public interface IAPIWebClientServiceSettings : ISettings
    {
        Uri Url { get; }
    }
}