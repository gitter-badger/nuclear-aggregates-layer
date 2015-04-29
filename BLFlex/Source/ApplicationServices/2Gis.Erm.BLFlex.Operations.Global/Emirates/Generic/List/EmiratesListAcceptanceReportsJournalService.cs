using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Emirates.SimplifiedModel.ReadModel.AcceptanceReportsJournal;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.List
{
    public sealed class ListAcceptanceReportService : ListEntityDtoServiceBase<AcceptanceReportsJournalRecord, EmiratesListAcceptanceReportsJournalRecordDto>,
                                                      IEmiratesAdapted
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ListAcceptanceReportService(IFinder finder, FilterHelper filterHelper, ISecurityServiceUserIdentifier userIdentifierService)
        {
            _finder = finder;
            _filterHelper = filterHelper;
            _userIdentifierService = userIdentifierService;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var organizationUnitsQuery = _finder.For<OrganizationUnit>();

            return _finder.Find(AcceptanceReportsJournalSpecs.Select.AcceptanceReportsJournalRecords,
                                AcceptanceReportsJournalSpecs.Find.OnlyAcceptanceReportsJournalRecords)
                          .Select(x => new
                              {
                                  Id = x.Id,
                                  OrganizationUnitId = x.OrganizationUnitId,
                                  EndDistributionDate = x.EndDistributionDate,
                                  DocumentsAmount = x.DocumentsAmount,
                                  AuthorId = x.AuthorId,
                                  CreatedOn = x.CreatedOn,
                                  IsActive = x.IsActive,
                                  IsDeleted = x.IsDeleted
                              })
                          .Join(organizationUnitsQuery,
                                x => x.OrganizationUnitId,
                                y => y.Id,
                                (x, y) =>
                                new EmiratesListAcceptanceReportsJournalRecordDto
                                    {
                                        Id = x.Id,
                                        OrganizationUnitId = x.OrganizationUnitId,
                                        OrganizationUnitName = y.Name,
                                        EndDistributionDate = x.EndDistributionDate,
                                        DocumentsAmount = x.DocumentsAmount,
                                        AuthorId = x.AuthorId,
                                        CreatedOn = x.CreatedOn,
                                        IsActive = x.IsActive,
                                        IsDeleted = x.IsDeleted
                                    })
                          .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(EmiratesListAcceptanceReportsJournalRecordDto dto)
        {
            dto.AuthorName = _userIdentifierService.GetUserInfo(dto.AuthorId).DisplayName;
        }
    }
}