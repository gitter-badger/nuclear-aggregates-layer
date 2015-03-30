using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListContactService : ListEntityDtoServiceBase<Contact, ListContactDto>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListContactService(
            IUserContext userContext,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            FilterHelper filterHelper)
        {
            _userContext = userContext;
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<Contact>();
         
            bool excludeReserve;
            Expression<Func<Contact, bool>> excludeReserveFilter = null;
            if (querySettings.TryGetExtendedProperty("ExcludeReserve", out excludeReserve))
            {
                var reserveCode = _userIdentifierService.GetReserveUserIdentity().Code;
                excludeReserveFilter = x => x.OwnerCode != reserveCode;
            }

            bool forClientAndLinkedChild;
            if (querySettings.TryGetExtendedProperty("ForClientAndLinkedChild", out forClientAndLinkedChild) && querySettings.ParentEntityId.HasValue)
            {
                query = _filterHelper.ForClientAndLinkedChild(query, querySettings.ParentEntityId.Value);
            }

            return query
            .Filter(_filterHelper, excludeReserveFilter)
            .Select(x => new ListContactDto
            {
                Id = x.Id,
                FullName = x.FullName,
                ClientId = x.ClientId,
                Client = x.Client.Name,
                JobTitle = x.JobTitle,
                MainPhoneNumber = x.MainPhoneNumber,
                MobilePhoneNumber = x.MobilePhoneNumber,
                Website = x.Website,
                HomeEmail = x.HomeEmail,
                WorkEmail = x.WorkEmail,
                OwnerCode = x.OwnerCode,
                CreateDate = x.CreatedOn,
                WorkAddress = x.WorkAddress,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                IsFired = x.IsFired,
                IsOwner = _userContext.Identity.Code == x.OwnerCode,
                AccountRole = x.AccountRole.ToStringLocalizedExpression(),
                Owner = null,
            })
            .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListContactDto dto)
        {
            dto.Owner = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;            
        }
    }
}