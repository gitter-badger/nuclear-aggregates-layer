using System.Collections.Generic;

namespace NuClear.Storage.EntityFramework
{
    public interface IEFDbModelConfigurationsProvider
    {
        IEnumerable<IEFDbModelConfiguration> GetConfigurations(string entityContainerName);
        IEnumerable<IEFDbModelConvention> GetConventions(string entityContainerName);
    }
}