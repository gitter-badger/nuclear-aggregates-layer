using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List
{
    public sealed class ListRegionalAdvertisingSharingService : ListEntityDtoServiceBase<RegionalAdvertisingSharing, ListRegionalAdvertisingSharingDto>
    {
        public ListRegionalAdvertisingSharingService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListRegionalAdvertisingSharingDto> GetListData(IQueryable<RegionalAdvertisingSharing> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            const char Delimiter = ',';

            return query
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new
                    {
                        x.Id,
                        x.CreatedOn,
                        x.BeginDistributionDate,
                        x.SourceOrganizationUnitId,
                        SourceOrganizationUnitName = x.SourceOrganizationUnit.Name,
                        x.DestOrganizationUnitId,
                        DestOrganizationUnitName = x.DestOrganizationUnit.Name,
                        SourceBranchOfficeOrgUnitId = x.SourceBranchOfficeOrganizationUnitId,
                        SourceBranchOfficeOrgUnitName = x.SourceBranchOfficeOrganizationUnit.ShortLegalName,
                        DestBranchOfficeOrgUnitId = x.DestBranchOfficeOrganizationUnitId,
                        DestBranchOfficeOrgUnitName = x.DestBranchOfficeOrganizationUnit.ShortLegalName,
                        OrderNumbersCollection = x.OrdersRegionalAdvertisingSharings
                                 .Select(y => y.Order.Number + (!string.IsNullOrEmpty(y.Order.RegionalNumber)
                                                                    ? " (" + y.Order.RegionalNumber + ")"
                                                                    : string.Empty)),
                        x.TotalAmount
                    })
                .AsEnumerable()
                .Select(x =>
                        new ListRegionalAdvertisingSharingDto
                            {
                                Id = x.Id,
                                CreatedOn = x.CreatedOn,
                                BeginDistributionDate = x.BeginDistributionDate,
                                SourceOrganizationUnitId = x.SourceOrganizationUnitId,
                                SourceOrganizationUnitName = x.SourceOrganizationUnitName,
                                DestOrganizationUnitId = x.DestOrganizationUnitId,
                                DestOrganizationUnitName = x.DestOrganizationUnitName,
                                SourceBranchOfficeOrgUnitId = x.SourceBranchOfficeOrgUnitId,
                                SourceBranchOfficeOrgUnitName = x.SourceBranchOfficeOrgUnitName,
                                DestBranchOfficeOrgUnitId = x.DestBranchOfficeOrgUnitId,
                                DestBranchOfficeOrgUnitName = x.DestBranchOfficeOrgUnitName,
                                OrderNumbers =
                                    x.OrderNumbersCollection
                                        .Aggregate(string.Empty, (current, next) => string.Format("{0}{1} {2}", current, Delimiter, next)).TrimStart(Delimiter),
                                TotalAmount = x.TotalAmount
                            });
        }
    }
}