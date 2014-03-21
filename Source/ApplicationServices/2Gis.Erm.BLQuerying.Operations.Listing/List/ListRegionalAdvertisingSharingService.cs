using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListRegionalAdvertisingSharingService : ListEntityDtoServiceBase<RegionalAdvertisingSharing, ListRegionalAdvertisingSharingDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListRegionalAdvertisingSharingService(
            IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListRegionalAdvertisingSharingDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<RegionalAdvertisingSharing>();

            const char Delimiter = ',';

            return query
                .Select(x => new ListRegionalAdvertisingSharingDto
                {
                    Id = x.Id,
                    CreatedOn = x.CreatedOn,
                    BeginDistributionDate = x.BeginDistributionDate,
                    SourceOrganizationUnitId = x.SourceOrganizationUnitId,
                    SourceOrganizationUnitName = x.SourceOrganizationUnit.Name,
                    DestOrganizationUnitId = x.DestOrganizationUnitId,
                    DestOrganizationUnitName = x.DestOrganizationUnit.Name,
                    SourceBranchOfficeOrgUnitId = x.SourceBranchOfficeOrganizationUnitId,
                    SourceBranchOfficeOrgUnitName = x.SourceBranchOfficeOrganizationUnit.ShortLegalName,
                    DestBranchOfficeOrgUnitId = x.DestBranchOfficeOrganizationUnitId,
                    DestBranchOfficeOrgUnitName = x.DestBranchOfficeOrganizationUnit.ShortLegalName,
                    OrderNumbersCollection = x.OrdersRegionalAdvertisingSharings
                                .Select(y => y.Order.Number + (!string.IsNullOrEmpty(y.Order.RegionalNumber)
                                                                ? " (" + y.Order.RegionalNumber + ")"
                                                                : string.Empty)),
                    TotalAmount = x.TotalAmount,
                    OrderNumbers = null,
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                {
                    x.OrderNumbers = x.OrderNumbersCollection.Aggregate(string.Empty, (current, next) => string.Format("{0}{1} {2}", current, Delimiter, next)).TrimStart(Delimiter);
                    return x;
                });
        }
    }
}