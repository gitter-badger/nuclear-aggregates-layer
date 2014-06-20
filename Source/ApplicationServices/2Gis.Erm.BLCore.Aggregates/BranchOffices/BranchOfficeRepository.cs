using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

// ReSharper disable CheckNamespace

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices
// ReSharper restore CheckNamespace
{
    // TODO {all, 11.02.2013}: навести порядок в Exception's -> при ошибках бизнес логики выбрасываются и ArgumentException и NotificationException и Exception.
    public class BranchOfficeRepository : IBranchOfficeRepository
    {
        private readonly IFinder _finder;
        private readonly ISecureFinder _secureFinder;
        private readonly IRepository<BranchOffice> _branchOfficeGenericRepository;
        private readonly IRepository<BranchOfficeOrganizationUnit> _branchOfficeOrganizationUnitGenericRepository;

        public BranchOfficeRepository(IFinder finder,
                                      ISecureFinder secureFinder,
                                      IRepository<BranchOffice> branchOfficeGenericRepository,
                                      IRepository<BranchOfficeOrganizationUnit> branchOfficeOrganizationUnitGenericRepository)
        {
            _finder = finder;
            _secureFinder = secureFinder;
            _branchOfficeGenericRepository = branchOfficeGenericRepository;
            _branchOfficeOrganizationUnitGenericRepository = branchOfficeOrganizationUnitGenericRepository;
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
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
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

                var branchOfficeOrganizationUnits = _secureFinder.FindMany(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.BelongsToBranchOffice(branchOffice.Id));
            foreach (var branchOfficeOrganizationUnit in branchOfficeOrganizationUnits)
            {
                branchOfficeOrganizationUnit.IsActive = false;
                _branchOfficeOrganizationUnitGenericRepository.Update(branchOfficeOrganizationUnit);
            }

                _branchOfficeOrganizationUnitGenericRepository.Save();

            branchOffice.IsActive = false;
                _branchOfficeGenericRepository.Update(branchOffice);
                var count = _branchOfficeGenericRepository.Save();

                transaction.Complete();
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

            branchOfficeOrganizationUnit.IsActive = false;
            _branchOfficeOrganizationUnitGenericRepository.Update(branchOfficeOrganizationUnit);
            return _branchOfficeOrganizationUnitGenericRepository.Save();
        }

        public void SetPrimaryBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId)
        {
            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnitId));
            if (branchOfficeOrganizationUnit == null)
            {
                throw new Exception(BLResources.CouldNotFindBranchOfficeOrganizationUnit);
            }

            branchOfficeOrganizationUnit.IsPrimary = true;
            _branchOfficeOrganizationUnitGenericRepository.Update(branchOfficeOrganizationUnit);

            var otherBranchOfficeOrganizationUnits =
                _finder.FindMany(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.BelongsToOrganizationUnit(branchOfficeOrganizationUnit.OrganizationUnitId)
                                 & Specs.Find.ActiveAndNotDeleted<BranchOfficeOrganizationUnit>()
                                 & Specs.Find.ExceptById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnit.Id));

            foreach (var notPrimarybranchOfficeOrganizationUnit in otherBranchOfficeOrganizationUnits)
            {
                notPrimarybranchOfficeOrganizationUnit.IsPrimary = false;
                _branchOfficeOrganizationUnitGenericRepository.Update(notPrimarybranchOfficeOrganizationUnit);
            }

            _branchOfficeOrganizationUnitGenericRepository.Save();
        }

        public void SetPrimaryForRegionalSalesBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId)
        {
            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnitId));
            if (branchOfficeOrganizationUnit == null)
            {
                throw new Exception(BLResources.CouldNotFindBranchOfficeOrganizationUnit);
            }

            branchOfficeOrganizationUnit.IsPrimaryForRegionalSales = true;
            _branchOfficeOrganizationUnitGenericRepository.Update(branchOfficeOrganizationUnit);

            var otherBranchOfficeOrganizationUnits =
                _finder.FindMany(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.BelongsToOrganizationUnit(branchOfficeOrganizationUnit.OrganizationUnitId)
                                 & Specs.Find.ActiveAndNotDeleted<BranchOfficeOrganizationUnit>()
                                 & Specs.Find.ExceptById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnit.Id));

            foreach (var notPrimarybranchOfficeOrganizationUnit in otherBranchOfficeOrganizationUnits)
            {
                notPrimarybranchOfficeOrganizationUnit.IsPrimaryForRegionalSales = false;
                _branchOfficeOrganizationUnitGenericRepository.Update(notPrimarybranchOfficeOrganizationUnit);
            }

            _branchOfficeOrganizationUnitGenericRepository.Save();
         }

        public long? GetPrintFormTemplateId(long branchOfficeOrganizationUnitId, TemplateCode templateCode)
        {
            var templates = _finder.Find<PrintFormTemplate>(x => !x.IsDeleted && x.IsActive && x.TemplateCode == (int)templateCode)
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

        private int Activate(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit)
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

            return _branchOfficeOrganizationUnitGenericRepository.Save();
        }

        private PrimaryBranchOfficeOrganizationUnits GetPrimaryBranchOfficeOrganizationUnits(long organizationUnitId)
        {
            return new PrimaryBranchOfficeOrganizationUnits
            {
                    Primary = 
                        _finder.FindOne(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.PrimaryOfOrganizationUnit(organizationUnitId)),

                    PrimaryForRegionalSales =
                        _finder.FindOne(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.PrimaryForRegionalSalesOfOrganizationUnit(organizationUnitId))
                };
        }

        int IActivateAggregateRepository<BranchOffice>.Activate(long entityId)
        {
            var branchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(entityId));

            return Activate(branchOffice);
        }

        private int Activate(BranchOffice branchOffice)
        {
            branchOffice.IsActive = true;
            _branchOfficeGenericRepository.Update(branchOffice);

            return _branchOfficeGenericRepository.Save();
        }
    }
}