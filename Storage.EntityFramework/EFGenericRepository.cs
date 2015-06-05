using AutoMapper;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Core;

namespace NuClear.Storage.EntityFramework
{
    public sealed class EFGenericRepository<TEntity> : EFRepository<TEntity, TEntity>
        where TEntity : class, IEntity
    {
        public EFGenericRepository(IUserContext userContext,
                                   IModifiableDomainContextProvider domainContextProvider,
                                   IPersistenceChangesRegistryProvider changesRegistryProvider,
                                   IMappingEngine mappingEngine)
            : base(userContext, domainContextProvider, changesRegistryProvider, mappingEngine)
        {
        }
    }
}