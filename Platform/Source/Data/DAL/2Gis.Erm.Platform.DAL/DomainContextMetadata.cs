using System.Reflection;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;

namespace DoubleGis.Erm.Platform.DAL
{
    public sealed class DomainContextMetadata
    {
        public ConnectionStringName ConnectionStringName { get; set; }
        public Assembly Assembly { get; set; }
        public string PathToEdmx { get; set; }
        public string EntityContainerName { get; set; }
    }
}