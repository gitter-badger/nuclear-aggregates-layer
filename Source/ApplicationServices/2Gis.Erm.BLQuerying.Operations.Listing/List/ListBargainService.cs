using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public class ListBargainService : ListEntityDtoServiceBase<Bargain, ListBargainDto>
    {
        public ListBargainService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListBargainDto> GetListData(IQueryable<Bargain> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            return query
                .Where(x => !x.IsDeleted)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x =>
                        new ListBargainDto
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
                            });
        }
    }
}
