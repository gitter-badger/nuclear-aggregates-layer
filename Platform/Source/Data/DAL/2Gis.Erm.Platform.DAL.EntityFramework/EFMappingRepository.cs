using AutoMapper;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFMappingRepository<TDomainEntity, TPersistentEntity> : EFRepository<TDomainEntity, TPersistentEntity>
        where TDomainEntity : class, IEntity
        where TPersistentEntity : class, IEntity
    {
        public EFMappingRepository(IUserContext userContext, 
                                   IModifiableDomainContextProvider domainContextProvider, 
                                   IPersistenceChangesRegistryProvider changesRegistryProvider, 
                                   IMappingEngine mappingEngine) : base(userContext, domainContextProvider, changesRegistryProvider, mappingEngine)
        {
        }
    }
}