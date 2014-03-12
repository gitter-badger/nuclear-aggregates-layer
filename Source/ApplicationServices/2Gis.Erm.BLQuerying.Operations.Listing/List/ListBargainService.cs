using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListBargainService : ListEntityDtoServiceBase<Bargain, ListBargainDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListBargainService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListBargainDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Bargain>();

            return query
                .Where(x => !x.IsDeleted)
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListBargainDto
                {
                    Id = x.Id,
                    Number = x.Number,
                    CustomerLegalPersonId = x.CustomerLegalPersonId,
                    CustomerLegalPersonLegalName = x.LegalPerson.LegalName,
                    BranchOfficeId = x.BranchOfficeOrganizationUnit.BranchOfficeId,
                    BranchOfficeName = x.BranchOfficeOrganizationUnit.BranchOffice.Name,
                    CreatedOn = x.CreatedOn,
                    ClientId = x.LegalPerson.ClientId,
                    ClientName = x.LegalPerson.Client.Name,
                    LegalAddress = x.LegalPerson.LegalAddress
                })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}
