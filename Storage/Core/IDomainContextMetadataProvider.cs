using System;

namespace NuClear.Storage.Core
{
    public interface IDomainContextMetadataProvider
    {
        DomainContextMetadata GetReadMetadata(Type entityType);
        DomainContextMetadata GetWriteMetadata(Type entityType);
    }
}