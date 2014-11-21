using System;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface IDomainContextMetadataProvider
    {
        DomainContextMetadata GetReadMetadata(Type entityType);
        DomainContextMetadata GetWriteMetadata(Type entityType);
    }
}