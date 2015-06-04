using System;

namespace NuClear.Storage.Core
{
    public interface IEntityContainerNameResolver
    {
        string Resolve(Type entityType);
    }
}