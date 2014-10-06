using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public interface IEFObjectContextFactory
    {
        ObjectContext CreateObjectContext(DomainContextMetadata domainContextMetadata);
    }
}