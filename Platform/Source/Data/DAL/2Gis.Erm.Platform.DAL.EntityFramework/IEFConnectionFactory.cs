using System.Data.Entity.Core.EntityClient;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public interface IEFConnectionFactory
    {
        EntityConnection CreateEntityConnection(DomainContextMetadata domainContextMetadata);
    }
}