using AutoMapper;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFGenericRepository<TEntity> : EFRepository<TEntity, TEntity>
        where TEntity : class, IEntity
    {
        public EFGenericRepository(IUserContext userContext, 
                                   IModifiableDomainContextProvider domainContextProvider, 
                                   IPersistenceChangesRegistryProvider changesRegistryProvider, 
                                   IMappingEngine mappingEngine) : base(userContext, domainContextProvider, changesRegistryProvider, mappingEngine)
        {
        }
    }
}