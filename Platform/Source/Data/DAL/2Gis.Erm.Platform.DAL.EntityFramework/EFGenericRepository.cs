using AutoMapper;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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