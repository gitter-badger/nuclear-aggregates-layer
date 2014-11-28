using System;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface IEntityContainerNameResolver
    {
        string Resolve(Type entityType);
    }
}