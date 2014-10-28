using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.EntityFramework;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public interface IEfDbModelConfigurationsProvider
    {
        IEnumerable<IEfDbModelConfiguration> GetConfigurations(string entityContainerName);
    }
}