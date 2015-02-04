using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListContactService : ListEntityDtoServiceBase<Contact, ListContactDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListContactService(
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            FilterHelper filterHelper)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<Contact>();

            bool forClientAndLinkedChild;
            if (querySettings.TryGetExtendedProperty("ForClientAndLinkedChild", out forClientAndLinkedChild) && querySettings.ParentEntityId.HasValue)
            {
                query = _filterHelper.ForClientAndLinkedChild(query, querySettings.ParentEntityId.Value);
            }

            return query
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