using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListBargainService : ListEntityDtoServiceBase<Bargain, ListBargainDto>
    {
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListBargainService(
            IFinder finder, FilterHelper filterHelper,
            IUserContext userContext)
        {
            _finder = finder;
            _filterHelper = filterHelper;
            _userContext = userContext;
        }

        protected override IEnumerable<ListBargainDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Bargain>();

            var myFilter = querySettings.CreateForExtendedProperty<Bargain, bool>("ForMe", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });

            return query
                .Where(x => !x.IsDeleted)
                .Filter(_filterHelper, myFilter)
                .Select(x => new ListBargainDto
                {
                    Id = x.Id,
                    Number = x.Number,
                    CustomerLegalPersonId = x.CustomerLegalPersonId,
                    CustomerLegalPersonLegalName = x.LegalPerson.LegalName,
                    BranchOfficeId = x.BranchOfficeOrganizationUnit.BranchOfficeId,
                    BranchOfficeName = x.BranchOfficeOrganizationUnit.BranchOffice.Name,
                    CreatedOn = x.CreatedOn,
                    OwnerCode = x.OwnerCode,
                    ClientId = x.LegalPerson.ClientId,
                    ClientName = x.LegalPerson.Client.Name,
                    LegalAddress = x.LegalPerson.LegalAddress,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}
