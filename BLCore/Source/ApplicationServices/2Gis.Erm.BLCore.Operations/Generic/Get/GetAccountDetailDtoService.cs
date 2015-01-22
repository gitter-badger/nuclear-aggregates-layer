using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetAccountDetailDtoService : GetDomainEntityDtoServiceBase<AccountDetail>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifier;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly ISecureFinder _finder;

        public GetAccountDetailDtoService(
            IUserContext userContext,
            ISecurityServiceUserIdentifier userIdentifier,
            ISecurityServiceEntityAccess entityAccessService,
            ISecureFinder finder)
            : base(userContext)
        {
            _userContext = userContext;
            _userIdentifier = userIdentifier;
            _entityAccessService = entityAccessService;
            _finder = finder;
        }

        protected override IDomainEntityDto<AccountDetail> GetDto(long entityId)
        {
            var accountDetailAndParentOwnerCodeDto = _finder.Find<AccountDetail>(x => x.Id == entityId)
                             .Select(entity => new 
                                 {
                                 AccountDetail = new AccountDetailDomainEntityDto
                                 {
                                     Id = entity.Id,
                                     AccountRef = new EntityReference { Id = entity.AccountId, Name = null },
                                     OperationTypeRef = new EntityReference { Id = entity.OperationTypeId, Name = entity.OperationType.Name },
                                     Description = entity.Description,
                                     TransactionDate = entity.TransactionDate,
                                     Amount = entity.Amount,
                                     OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                                     IsActive = entity.IsActive,
                                     IsDeleted = entity.IsDeleted,
                                     CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                     CreatedOn = entity.CreatedOn,
                                     ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                     ModifiedOn = entity.ModifiedOn,
                                     Timestamp = entity.Timestamp
                                 },
                                 ParentAccountOwnerCode = entity.Account.OwnerCode
                                 })
                             .Single();

            var dto = accountDetailAndParentOwnerCodeDto.AccountDetail;

            var reserveUserCode = _userIdentifier.GetReserveUserIdentity().Code;
            if (dto.OwnerRef.Id == reserveUserCode || accountDetailAndParentOwnerCodeDto.ParentAccountOwnerCode == reserveUserCode)
            {
                dto.OwnerCanBeChanged = false;
            }
            else
            {
                // Проверка: может ли текущий пользователь сменить текущего куратора.
                dto.OwnerCanBeChanged = _userContext.Identity.SkipEntityAccessCheck
                                        || _entityAccessService.HasEntityAccess(
                                            EntityAccessTypes.Assign,
                                            EntityType.Instance.AccountDetail(),
                                            _userContext.Identity.Code,
                                            dto.Id,
                                            _userContext.Identity.Code,
                                            dto.OwnerRef.Id);
            }

            return dto;
        }

        protected override IDomainEntityDto<AccountDetail> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            var dto = new AccountDetailDomainEntityDto { TransactionDate = DateTime.Today, OwnerCanBeChanged = false };

            if (parentEntityName.Equals(EntityType.Instance.Account()))
            {
                if (parentEntityId == null)
                {
                    throw new NotificationException(BLResources.IdentifierNotSet);
                }

                dto.AccountRef = new EntityReference { Id = parentEntityId.Value };

                // как куратор операции выставляется куратор родительского лицевого счёта
                dto.OwnerRef = new EntityReference { Id = _finder.Find<Account>(x => x.Id == parentEntityId).Select(x => x.OwnerCode).Single() };
                dto.IsActive = true;
            }

            return dto;
        }
    }
}