using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel
{
    public sealed class ReleaseReadModel : IReleaseReadModel
    {
        private readonly IFinder _finder;

        public ReleaseReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public IEnumerable<ReleaseProcessingMessage> GetReleaseValidationResults(long releaseInfoId)
        {
            return _finder.Find<ReleaseValidationResult>(x => x.ReleaseInfoId == releaseInfoId)
                .OrderBy(x => x.IsBlocking)
                .ThenBy(x => x.IsBlocking)
                .Select(x => new ReleaseProcessingMessage
                {
                    IsBlocking = x.IsBlocking,
                    Message = x.Message,
                    OrderId = x.OrderId,
                    RuleCode = x.RuleCode
                })
                .ToArray();
        }

        public Dictionary<long, ValidationReportLine> GetOrderValidationLines(IEnumerable<long> orderIds)
        {
            var userInfos = _finder.For<User>().Select(user => new { user.Id, user.DisplayName }).ToArray();
            var orderInfos = _finder.Find<Order>(o => orderIds.Contains(o.Id))
                .Select(o => new
                {
                    Id = o.Id,
                    Number = o.Number,
                    FirmName = o.Firm.Name,
                    LegalPersonName = o.LegalPerson.ShortName,
                    OwnerCode = o.OwnerCode
                })
                .ToArray();

            return orderInfos
                .Join(userInfos,
                    o => o.OwnerCode,
                    u => u.Id,
                    (o, u) => new ValidationReportLine { OrderId = o.Id, Number = o.Number, FirmName = o.FirmName, LegalPersonName = o.LegalPersonName, OwnerName = u.DisplayName })
                .ToDictionary(info => info.OrderId, info => info);
        }

        public int GetCountryCode(long organizationUnitId)
        {
            var countyIsoCode = _finder.Find(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>() && Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                                       .Select(x => x.Country.IsoCode)
                                       .SingleOrDefault();
            return !string.IsNullOrEmpty(countyIsoCode) ? int.Parse(countyIsoCode) : 0;
        }

        public bool IsReleaseMustBeLaunchedThroughExport(long organizationUnitId)
        {
            return _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId)).Select(x => x.ErmLaunchDate != null).Single();
        }

        public long GetOrganizationUnitId(int organizationUnitDgppId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>() && OrganizationUnitSpecs.Find.ByDgppId(organizationUnitDgppId))
                          .Select(x => x.Id)
                          .FirstOrDefault();
        }

        public string GetOrganizationUnitName(long organizationUnitId)
        {
            return _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId)).Select(ou => ou.Name).Single();
        }

        public ReleaseInfo GetLastFinalRelease(long organizationUnitId, TimePeriod period)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<ReleaseInfo>() &&
                                ReleaseSpecs.Releases.Find.ByOrganization(organizationUnitId) &&
                                ReleaseSpecs.Releases.Find.ForPeriod(period) &&
                                ReleaseSpecs.Releases.Find.Final())
                          .OrderByDescending(x => x.StartDate)
                          .FirstOrDefault();
        }

        public IReadOnlyCollection<ReleaseInfo> GetReleasesInDescOrder(long organizationUnitId, TimePeriod period)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<ReleaseInfo>() &&
                                ReleaseSpecs.Releases.Find.ByOrganization(organizationUnitId) &&
                                ReleaseSpecs.Releases.Find.ForPeriod(period))
                          .OrderByDescending(x => x.StartDate)
                          .ToArray();
        }

        public bool HasFinalReleaseAfterDate(long organizationUnitId, DateTime periodStartDate)
        {
            return _finder.Find(ReleaseSpecs.Releases.Find.FinalSuccessOrInProgressAfterDate(organizationUnitId, periodStartDate)).Any();
        }

        public bool HasSuccededFinalReleaseFromDate(long organizationUnitId, DateTime periodStartDate)
        {
            const int UkOrganizationUnitId = 128;

            // раскрываем УК как набор всех филиалов
            IQueryable<OrganizationUnit> organizationUnitQuery;
            if (organizationUnitId == UkOrganizationUnitId)
            {
                organizationUnitQuery = _finder.For<OrganizationUnit>()
                                               .Select(x => new
                {
                    OrganizationUnit = x,
                                                                    ContributionType = x.BranchOfficeOrganizationUnits
                                                                                        .Where(y => y.IsActive && !y.IsDeleted &&
                                                                                                    y.IsPrimaryForRegionalSales)
                                        .Select(y => y.BranchOffice)
                                        .Select(y => (ContributionTypeEnum?)y.ContributionTypeId)
                                        .FirstOrDefault(),
                })
                .Where(x => x.ContributionType == ContributionTypeEnum.Branch)
                .Select(x => x.OrganizationUnit);
            }
            else
            {
                organizationUnitQuery = _finder.Find<OrganizationUnit>(x => x.Id == organizationUnitId);
            }

            var hasSuccessedRelease = organizationUnitQuery.SelectMany(x => x.ReleaseInfos)
                                                           .Any(x => x.IsActive && !x.IsDeleted &&
                                                                     !x.IsBeta &&
                                                                     x.PeriodStartDate >= periodStartDate &&
                                                                     x.Status == ReleaseStatus.Success);
            return hasSuccessedRelease;
        }

        public ReleaseInfo GetReleaseInfo(long releaseInfoId)
        {
            return _finder.Find(Specs.Find.ById<ReleaseInfo>(releaseInfoId)).SingleOrDefault();
        }

        public bool HasFinalReleaseInProgress(long organizationUnitId, TimePeriod period)
        {
            return _finder.Find(ReleaseSpecs.Releases.Find.FinalInProgress(organizationUnitId, period)).Any();
        }
    }
}
