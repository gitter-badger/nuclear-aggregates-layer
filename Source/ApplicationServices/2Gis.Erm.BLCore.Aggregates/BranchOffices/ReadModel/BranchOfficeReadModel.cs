using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel
{
    public abstract class BranchOfficeReadModel : IBranchOfficeReadModel
    {
        private readonly IFinder _finder;
        // FIXME {y.baranihin, 04.06.2014}: Верно, что _secureFinder не используется?
        private readonly ISecureFinder _secureFinder;

        protected BranchOfficeReadModel(IFinder finder, ISecureFinder secureFinder)
        {
            _finder = finder;
            _secureFinder = secureFinder;
        }

        public virtual BranchOffice GetBranchOffice(long branchOfficeId)
        {
            return _finder.FindOne(Specs.Find.ById<BranchOffice>(branchOfficeId));
        }

        public virtual BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId)
            {
            return _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnitId));
        }

        public virtual BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(string syncCode1C)
        {
            return _finder.FindOne(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.BySyncCode1C(syncCode1C));
        }

        public string GetNameOfActiveDuplicateByInn(long branchOfficeId, string inn)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<BranchOffice>()
                               && BranchOfficeSpecs.BranchOffices.Find.DuplicatesByInn(branchOfficeId, inn))
                         .Select(x => x.Name)
                         .FirstOrDefault();
        }

        public bool AreThereAnyActiveInnDuplicates(long branchOfficeId, string inn)
        {
            return _finder.Find(BranchOfficeSpecs.BranchOffices.Find.DuplicatesByInn(branchOfficeId, inn) &&
                                Specs.Find.ActiveAndNotDeleted<BranchOffice>())
                          .Any();
        }

        public PrimaryBranchOfficeOrganizationUnits GetPrimaryBranchOfficeOrganizationUnits(long organizationUnitId)
        {
            var primary = _finder.FindOne(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.PrimaryOfOrganizationUnit(organizationUnitId));
            var primaryForRegionalSales = _finder.FindOne(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.PrimaryForRegionalSalesOfOrganizationUnit(organizationUnitId));
            return new PrimaryBranchOfficeOrganizationUnits
        {
                    Primary = primary,
                    PrimaryForRegionalSales = primaryForRegionalSales
                };
        }

        public BranchOfficeOrganizationUnitNamesDto GetBranchOfficeOrganizationUnitDuplicate(long organizationUnitId,
                                                                                             long branchOfficeId,
                                                                                             long branchOfficeOrganizationUnitId)
        {
            var findSpec = Specs.Find.ActiveAndNotDeleted<BranchOfficeOrganizationUnit>() &&
                           BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.DuplicatesByOrganizationUnitAndBranchOffice(organizationUnitId,
                                                                                                                            branchOfficeId,
                                                                                                                            branchOfficeOrganizationUnitId);

            return _finder.Find(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Select.BranchOfficeAndOrganizationUnitNames(), findSpec)
                          .FirstOrDefault();
        }

        public string GetBranchOfficeName(long branchOfficeId)
        {
            return _finder.Find(Specs.Find.ById<BranchOffice>(branchOfficeId)).Select(x => x.Name).Single();
        }

        public IEnumerable<long> GetProjectOrganizationUnitIds(long projectCode)
        {
            var organizationUnitIds = _finder.Find<Project>(project => project.Id == projectCode && project.OrganizationUnitId.HasValue)
                                            .Select(project => project.OrganizationUnitId.Value)
                                            .ToArray();
            return organizationUnitIds;
        }

        public ContributionTypeEnum GetOrganizationUnitContributionType(long organizationUnitId)
        {
            var type = _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                              .SelectMany(unit => unit.BranchOfficeOrganizationUnits)
                              .Where(Specs.Find.ActiveAndNotDeleted<BranchOfficeOrganizationUnit>())
                              .Where(boou => boou.IsPrimary)
                              .Select(boou => boou.BranchOffice)
                              .Select(branchOffice => branchOffice.ContributionTypeId.Value)
                              .Single();

            return (ContributionTypeEnum)type;
        }
    }
}