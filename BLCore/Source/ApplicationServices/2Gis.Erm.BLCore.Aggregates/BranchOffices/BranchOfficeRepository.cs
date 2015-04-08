using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.BranchOfficeOrganizationUnit;

// ReSharper disable CheckNamespace

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices
// ReSharper restore CheckNamespace
{
    // TODO {all, 11.02.2013}: навести порядок в Exception's -> при ошибках бизнес логики выбрасываются и ArgumentException и NotificationException и Exception.
    public class BranchOfficeRepository : IBranchOfficeRepository
    {
        private readonly IRepository<BranchOffice> _branchOfficeGenericRepository;
        private readonly IRepository<BranchOfficeOrganizationUnit> _branchOfficeOrganizationUnitGenericRepository;
        private readonly IFinder _finder;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecureFinder _secureFinder;

        public BranchOfficeRepository(IFinder finder,
                                      ISecureFinder secureFinder,
                                      IRepository<BranchOffice> branchOfficeGenericRepository,
                                      IRepository<BranchOfficeOrganizationUnit> branchOfficeOrganizationUnitGenericRepository,
                                      IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _secureFinder = secureFinder;
            _branchOfficeGenericRepository = branchOfficeGenericRepository;
            _branchOfficeOrganizationUnitGenericRepository = branchOfficeOrganizationUnitGenericRepository;
            _scopeFactory = scopeFactory;
        }

        public IEnumerable<long> GetOrganizationUnitTerritories(long organizationUnitId)
        {
            return _finder.Find<Territory>(territory => territory.OrganizationUnitId == organizationUnitId)
                          .Select(territory => territory.Id)
                          .ToArray();
        }

        public BranchOfficeOrganizationShortInformationDto GetBranchOfficeOrganizationUnitShortInfo(long organizationUnitId)
        {
            return _finder.Find<BranchOfficeOrganizationUnit>(x => x.OrganizationUnitId == organizationUnitId)
                          .Where(Specs.Find.ActiveAndNotDeleted<BranchOfficeOrganizationUnit>())
                          .Where(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.PrimaryBranchOfficeOrganizationUnit())
                          .Select(x => new BranchOfficeOrganizationShortInformationDto
                              {
                                  Id = x.Id,
                                  ShortLegalName = x.ShortLegalName,
                              })
                          .SingleOrDefault() ?? new BranchOfficeOrganizationShortInformationDto();
            // null не возвращаем, логика была рассчитана на работу с пустыми значениями.
        }

        public int Deactivate(BranchOffice branchOffice)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, BranchOffice>())
            {
                var isActiveOrdersExist = _secureFinder
                    .Find(Specs.Find.ById<BranchOffice>(branchOffice.Id))
                    .SelectMany(x => x.BranchOfficeOrganizationUnits)
                    .SelectMany(x => x.Orders)
                    .Any(x => x.IsActive);
                if (isActiveOrdersExist)
                {
                    throw new ArgumentException(BLResources.CantDeativateObjectLinkedWithActiveOrders);
                }

                var branchOfficeOrganizationUnits = _secureFinder.FindMany(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.ByBranchOffice(branchOffice.Id));
                foreach (var branchOfficeOrganizationUnit in branchOfficeOrganizationUnits)
                {
                    branchOfficeOrganizationUnit.IsActive = false;
                    _branchOfficeOrganizationUnitGenericRepository.Update(branchOfficeOrganizationUnit);
                    scope.Updated(branchOfficeOrganizationUnit);
                }

                _branchOfficeOrganizationUnitGenericRepository.Save();

                branchOffice.IsActive = false;
                _branchOfficeGenericRepository.Update(branchOffice);
                scope.Updated(branchOffice);
                var count = _branchOfficeGenericRepository.Save();

                scope.Complete();
                return count;
            }
        }

        public int Deactivate(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit)
        {
            var isActiveOrdersExist = _finder
                .Find(Specs.Find.ById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnit.Id))
                .SelectMany(x => x.Orders)
                .Any(x => x.IsActive && !x.IsDeleted);

            if (isActiveOrdersExist)
            {
                throw new ArgumentException(BLResources.CantDeativateObjectLinkedWithActiveOrders);
            }

            using (var scope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, BranchOfficeOrganizationUnit>())
            {
                branchOfficeOrganizationUnit.IsActive = false;
                _branchOfficeOrganizationUnitGenericRepository.Update(branchOfficeOrganizationUnit);
                var result = _branchOfficeOrganizationUnitGenericRepository.Save();

                scope.Updated(branchOfficeOrganizationUnit)
                     .Complete();

                return result;
            }
        }

        public void SetPrimaryBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId)
        {
            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnitId));
            if (branchOfficeOrganizationUnit == null)
            {
                throw new Exception(BLResources.CouldNotFindBranchOfficeOrganizationUnit);
            }

            var otherBranchOfficeOrganizationUnits =
                _finder.FindMany(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.ByOrganizationUnit(branchOfficeOrganizationUnit.OrganizationUnitId)
                                 & Specs.Find.ActiveAndNotDeleted<BranchOfficeOrganizationUnit>()
                                 & Specs.Find.ExceptById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnit.Id));

            using (var scope = _scopeFactory.CreateNonCoupled<SetBranchOfficeOrganizationUnitAsPrimaryIdentity>())
            {
                branchOfficeOrganizationUnit.IsPrimary = true;
                _branchOfficeOrganizationUnitGenericRepository.Update(branchOfficeOrganizationUnit);
                scope.Updated(branchOfficeOrganizationUnit);

                foreach (var notPrimarybranchOfficeOrganizationUnit in otherBranchOfficeOrganizationUnits)
                {
                    notPrimarybranchOfficeOrganizationUnit.IsPrimary = false;
                    _branchOfficeOrganizationUnitGenericRepository.Update(notPrimarybranchOfficeOrganizationUnit);
                    scope.Updated(branchOfficeOrganizationUnit);
                }

                _branchOfficeOrganizationUnitGenericRepository.Save();
                scope.Complete();
            }
        }

        public void SetPrimaryForRegionalSalesBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId)
        {
            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnitId));
            if (branchOfficeOrganizationUnit == null)
            {
                throw new Exception(BLResources.CouldNotFindBranchOfficeOrganizationUnit);
            }

            using (var scope = _scopeFactory.CreateNonCoupled<SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesIdentity>())
            {
                var otherBranchOfficeOrganizationUnits =
                    _finder.FindMany(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.ByOrganizationUnit(branchOfficeOrganizationUnit.OrganizationUnitId)
                                     & Specs.Find.ActiveAndNotDeleted<BranchOfficeOrganizationUnit>()
                                     & Specs.Find.ExceptById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnit.Id));

                branchOfficeOrganizationUnit.IsPrimaryForRegionalSales = true;
                _branchOfficeOrganizationUnitGenericRepository.Update(branchOfficeOrganizationUnit);
                scope.Updated(branchOfficeOrganizationUnit);

                foreach (var notPrimarybranchOfficeOrganizationUnit in otherBranchOfficeOrganizationUnits)
                {
                    notPrimarybranchOfficeOrganizationUnit.IsPrimaryForRegionalSales = false;
                    _branchOfficeOrganizationUnitGenericRepository.Update(notPrimarybranchOfficeOrganizationUnit);
                    scope.Updated(notPrimarybranchOfficeOrganizationUnit);
                }

                _branchOfficeOrganizationUnitGenericRepository.Save();
                scope.Complete();
            }
        }

        public long? GetPrintFormTemplateId(long branchOfficeOrganizationUnitId, TemplateCode templateCode)
        {
            var templates = _finder.Find<PrintFormTemplate>(x => !x.IsDeleted && x.IsActive && x.TemplateCode == templateCode)
                                   .OrderByDescending(x => x.Id);

            var templateId = templates
                .Where(x => x.BranchOfficeOrganizationUnitId == branchOfficeOrganizationUnitId)
                .Select(x => (long?)x.File.Id)
                .FirstOrDefault();

            if (!templateId.HasValue)
            {
                templateId = templates
                    .Where(x => x.BranchOfficeOrganizationUnitId == null)
                    .Select(x => (long?)x.File.Id)
                    .FirstOrDefault();
            }

            return templateId;
        }

        int IDeactivateAggregateRepository<BranchOffice>.Deactivate(long entityId)
        {
            var entity = _secureFinder.FindOne(Specs.Find.ById<BranchOffice>(entityId));
            return Deactivate(entity);
        }

        int IDeactivateAggregateRepository<BranchOfficeOrganizationUnit>.Deactivate(long entityId)
        {
            var entity = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(entityId));
            return Deactivate(entity);
        }

        int IActivateAggregateRepository<BranchOfficeOrganizationUnit>.Activate(long entityId)
        {
            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(entityId));

            return Activate(branchOfficeOrganizationUnit);
        }

        int IActivateAggregateRepository<BranchOffice>.Activate(long entityId)
        {
            var branchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(entityId));

            return Activate(branchOffice);
        }

        private int Activate(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<ActivateIdentity, BranchOfficeOrganizationUnit>())
            {
                var primaryBranchOfficeOrganizationUnits = GetPrimaryBranchOfficeOrganizationUnits(branchOfficeOrganizationUnit.OrganizationUnitId);

                if (primaryBranchOfficeOrganizationUnits.Primary != null)
                {
                    branchOfficeOrganizationUnit.IsPrimary = false;
                }
                if (primaryBranchOfficeOrganizationUnits.PrimaryForRegionalSales != null)
                {
                    branchOfficeOrganizationUnit.IsPrimaryForRegionalSales = false;
                }

                branchOfficeOrganizationUnit.IsActive = true;
                _branchOfficeOrganizationUnitGenericRepository.Update(branchOfficeOrganizationUnit);

                var result =  _branchOfficeOrganizationUnitGenericRepository.Save();

                scope.Updated(branchOfficeOrganizationUnit)
                     .Complete();

                return result;
            }
        }

        private PrimaryBranchOfficeOrganizationUnits GetPrimaryBranchOfficeOrganizationUnits(long organizationUnitId)
        {
            return new PrimaryBranchOfficeOrganizationUnits
                       {
                           Primary =
                               _finder.FindOne(Specs.Find.ActiveAndNotDeleted<BranchOfficeOrganizationUnit>() &&
                                               BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.ByOrganizationUnit(organizationUnitId) &&
                                               BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.Primary()),
                           PrimaryForRegionalSales =
                               _finder.FindOne(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.PrimaryForRegionalSalesOfOrganizationUnit(organizationUnitId))
                       };
        }

        private int Activate(BranchOffice branchOffice)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<ActivateIdentity, BranchOffice>())
            {
                branchOffice.IsActive = true;
                _branchOfficeGenericRepository.Update(branchOffice);

                var cnt = _branchOfficeGenericRepository.Save();
                scope.Updated(branchOffice)
                     .Complete();

                return cnt;
            }
        }
    }
}