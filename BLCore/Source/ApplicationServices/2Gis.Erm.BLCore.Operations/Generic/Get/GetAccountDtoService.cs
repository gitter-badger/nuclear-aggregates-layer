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
    public class GetAccountDtoService : GetDomainEntityDtoServiceBase<Account>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifier;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly ISecureFinder _finder;

        public GetAccountDtoService(
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


        protected override IDomainEntityDto<Account> GetDto(long id)
        {
            var dto = _finder.Find<Account>(x => x.Id == id)
                             .Select(entity => new AccountDomainEntityDto
                                 {
                                     Id = entity.Id,
                                     AccountDetailBalance = entity.Balance,
                                     BranchOfficeOrganizationUnitRef = new EntityReference { Id = entity.BranchOfficeOrganizationUnitId, Name = entity.BranchOfficeOrganizationUnit.ShortLegalName },
                                     LegalPersonRef = new EntityReference { Id = entity.LegalPersonId, Name = entity.LegalPerson.LegalName },
                                     CurrencyRef = new EntityReference { Id = entity.BranchOfficeOrganizationUnit.OrganizationUnit.Country.CurrencyId, Name = entity.BranchOfficeOrganizationUnit.OrganizationUnit.Country.Currency.Name },
                                     LegalPersonSyncCode1C = entity.LegalPesonSyncCode1C,
                                     Timestamp = entity.Timestamp,
                                     CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                     CreatedOn = entity.CreatedOn,
                                     IsActive = entity.IsActive,
                                     IsDeleted = entity.IsDeleted,
                                     ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                     ModifiedOn = entity.ModifiedOn,
                                     OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null }
                                 })
                             .SingleOrDefault();

            if (dto == null)
            {
                throw new NotificationException(BLResources.CurrentUserHasNoReadEntityPermission);
            }

            var lockDeatilCost = _finder.Find<Account>(x => x.Id == dto.Id)
                                        .SelectMany(x => x.Locks)
                                        .Where(x => !x.IsDeleted && x.IsActive)
                                        .Sum(x => (decimal?)x.PlannedAmount) ?? 0;
            dto.LockDetailBalance = dto.AccountDetailBalance - lockDeatilCost;


            if (dto.OwnerRef.Id == _userIdentifier.GetReserveUserIdentity().Code)
            {
                dto.OwnerCanBeChanged = false;
            }
            else
            {
                // Проверка: может ли текущий пользователь сменить текущего куратора.
                // TODO {all}: Похоже на уг, нужно разобраться
                dto.OwnerCanBeChanged = _userContext.Identity.SkipEntityAccessCheck || _entityAccessService.HasEntityAccess(EntityAccessTypes.Assign,
                                                                                                                            EntityType.Instance.Account(),
                                                                                                                            _userContext.Identity.Code,
                                                                                                                            dto.Id,
                                                                                                                            _userContext.Identity.Code,
                                                                                                                            dto.OwnerRef.Id.Value);
            }

            return dto;
        }

        protected override IDomainEntityDto<Account> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            if (parentEntityId == null)
            {
                throw new ArgumentNullException("parentEntityId", @"Parent type should be specified!");
            }

            var dto = new AccountDomainEntityDto();

            if (parentEntityName.Equals(EntityType.Instance.LegalPerson()))
            {
                dto.LegalPersonRef = new EntityReference(parentEntityId.Value,
                                                         _finder.Find<LegalPerson>(x => x.Id == parentEntityId).Select(x => x.LegalName).SingleOrDefault());
            }

            return dto;
        }
    }
}