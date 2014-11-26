using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;

namespace DoubleGis.Erm.Platform.DAL
{
    public sealed class DomainContextMetadata
    {
        public string EntityContainerName { get; set; }
        public ConnectionStringName ConnectionStringName { get; set; }
    }
}