using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public abstract class GetDomainEntityDtoServiceBase<TEntity> : IGetDomainEntityDtoService<TEntity> where TEntity : class, IEntityKey, IEntity
    {
        private readonly IUserContext _userContext;

        protected GetDomainEntityDtoServiceBase(IUserContext userContext)
        {
            _userContext = userContext;
        }

        protected IUserContext UserContext
        {
            get { return _userContext; }
        }

        public IDomainEntityDto GetDomainEntityDto(long entityId, bool readOnly, long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var currentUserCode = UserContext.Identity.Code;

            IDomainEntityDto<TEntity> domainEntityDto;
            if (entityId == 0)
            {
                domainEntityDto = CreateDto(parentEntityId, parentEntityName, extendedInfo);

                if (typeof(IAuditableEntity).IsAssignableFrom(typeof(TEntity)))
                {
                    var modifiedTime = DateTime.UtcNow;

                    domainEntityDto.SetPropertyValue<IDomainEntityDto, EntityReference>("CreatedByRef", new EntityReference(currentUserCode));
                    domainEntityDto.SetPropertyValue<IDomainEntityDto, IAuditableEntity, DateTime>(x => x.CreatedOn, modifiedTime);
                    domainEntityDto.SetPropertyValue<IDomainEntityDto, EntityReference>("ModifiedByRef", new EntityReference(currentUserCode));
                    domainEntityDto.SetPropertyValue<IDomainEntityDto, IAuditableEntity, DateTime?>(x => x.ModifiedOn, modifiedTime);
                }

                if (typeof(ICuratedEntity).IsAssignableFrom(typeof(TEntity)))
                {
                    domainEntityDto.SetPropertyValue<IDomainEntityDto, EntityReference>("OwnerRef", new EntityReference(currentUserCode));
                }

                if (typeof(IDeactivatableEntity).IsAssignableFrom(typeof(TEntity)))
                {
                    domainEntityDto.SetPropertyValue<IDomainEntityDto, IDeactivatableEntity, bool>(x => x.IsActive, true);
                }
            }
            else
            {
                domainEntityDto = GetDto(entityId);
            }

            SetDtoProperties(domainEntityDto, entityId, readOnly, parentEntityId, parentEntityName, extendedInfo);
            return domainEntityDto;
        }

        protected abstract IDomainEntityDto<TEntity> GetDto(long entityId);
        protected abstract IDomainEntityDto<TEntity> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo);

        protected virtual void SetDtoProperties(IDomainEntityDto<TEntity> domainEntityDto,
                                                long entityId,
                                                bool readOnly,
                                                long? parentEntityId,
                                                EntityName parentEntityName,
                                                string extendedInfo)
        {
            // do nothing
        }
    }
}