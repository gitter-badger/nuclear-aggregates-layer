using NuClear.Storage.ConnectionStrings;

namespace NuClear.Storage.Core
{
    public sealed class DomainContextMetadata
    {
        public string EntityContainerName { get; set; }
        public IConnectionStringIdentity ConnectionStringName { get; set; }
    }
}