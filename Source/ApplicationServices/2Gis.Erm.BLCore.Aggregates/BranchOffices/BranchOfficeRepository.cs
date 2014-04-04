using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.DTO;
using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
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
        private readonly ICheckInnService _checkInnService;

        public BranchOfficeRepository(IFinder finder,
                                      ISecureFinder secureFinder,
                                      IRepository<BranchOffice> branchOfficeGenericRepository,
                                      IRepository<BranchOfficeOrganizationUnit> branchOfficeOrganizationUnitGenericRepository,
                                      ICheckInnService checkInnService)
        {
            _finder = finder;
            _secureFinder = secureFinder;
            _branchOfficeGenericRepository = branchOfficeGenericRepository;
            _branchOfficeOrganizationUnitGenericRepository = branchOfficeOrganizationUnitGenericRepository;
            _checkInnService = checkInnService;
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
                .SingleOrDefault() ?? new BranchOfficeOrganizationShortInformationDto(); // null не возвращаем, логика была рассчитана на работу с пустыми значениями.
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

                var branchOfficeOrganizationUnits = _secureFinder
                .Find(Specs.Find.ById<BranchOffice>(branchOffice.Id))
                .SelectMany(x => x.BranchOfficeOrganizationUnits)
                .ToArray();
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

        public void CreateOrUpdate(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit)
        {
            var branchOfficeActiveAndNotDeleted = _finder.Find<BranchOffice>(x => x.Id == branchOfficeOrganizationUnit.BranchOfficeId && x.IsActive && !x.IsDeleted).Any();
            if (!branchOfficeActiveAndNotDeleted)
            {
                throw new NotificationException(BLResources.ActiveAndNotDeletedBranchOfficeRequired);
            }

            var organizationUnitActiveAndNotDeleted = _finder.Find<OrganizationUnit>(x => x.Id == branchOfficeOrganizationUnit.OrganizationUnitId && x.IsActive && !x.IsDeleted).Any();
            if (!organizationUnitActiveAndNotDeleted)
            {
                throw new NotificationException(BLResources.ActiveAndNotDeletedOrganizationUnitRequired);
            }

            var branchOfficeOrganizationUnitDto =
                _finder.Find<BranchOfficeOrganizationUnit>(x => x.BranchOfficeId == branchOfficeOrganizationUnit.BranchOfficeId &&
                        x.OrganizationUnitId == branchOfficeOrganizationUnit.OrganizationUnitId &&
                        x.IsActive && !x.IsDeleted &&
                        x.Id != branchOfficeOrganizationUnit.Id)
            .Select(x => new
            {
                OrganizationUnitName = x.OrganizationUnit.Name,
                BranchOfficeName = x.BranchOffice.Name,
                        })
                    .SingleOrDefault();

            if (branchOfficeOrganizationUnitDto != null)
            {
                throw new NotificationException(string.Format(CultureInfo.CurrentCulture,
                                                              BLResources.OrgUnitForBranchOfficeAlreadyExist,
                                                              branchOfficeOrganizationUnitDto.OrganizationUnitName,
                                                              branchOfficeOrganizationUnitDto.BranchOfficeName));
            }

            var primaryBranchOfficeOrganizationUnits = GetPrimaryBranchOfficeOrganizationUnits(branchOfficeOrganizationUnit.OrganizationUnitId);
            if (primaryBranchOfficeOrganizationUnits.Primary == null)
            {
                // если нет основного то назначим
                branchOfficeOrganizationUnit.IsPrimary = true;
            }
            else if (primaryBranchOfficeOrganizationUnits.Primary.Id != branchOfficeOrganizationUnit.Id)
            {
                // если основной уже есть, то признак снимется
                branchOfficeOrganizationUnit.IsPrimary = false;
            }

            if (primaryBranchOfficeOrganizationUnits.PrimaryForRegionalSales == null)
            {
                // первый добавленный автоматически будет назначен основным
                branchOfficeOrganizationUnit.IsPrimaryForRegionalSales = true;
            }
            else if (primaryBranchOfficeOrganizationUnits.PrimaryForRegionalSales.Id != branchOfficeOrganizationUnit.Id)
            {
                // если основной уже есть, то признак снимется
                branchOfficeOrganizationUnit.IsPrimaryForRegionalSales = false;
            }

            if (branchOfficeOrganizationUnit.IsNew())
            {
                _branchOfficeOrganizationUnitGenericRepository.Add(branchOfficeOrganizationUnit);
            }
            else
            {
                _branchOfficeOrganizationUnitGenericRepository.Update(branchOfficeOrganizationUnit);
            }

            _branchOfficeOrganizationUnitGenericRepository.Save();
        }

        public void CreateOrUpdate(BranchOffice branchOffice)
        {
            var innExists = _finder.Find<BranchOffice>(office => office.Id != branchOffice.Id &&
                                                                 office.Inn == branchOffice.Inn && 
                                                                 office.IsActive && !office.IsDeleted)
                                   .Any();

            if (innExists)
            {
                throw new ArgumentException(BLResources.CannotSaveBranchOfficeInnExists);
            }

            string innErrorMessage;
            if (_checkInnService.TryGetErrorMessage(branchOffice.Inn, out innErrorMessage))
            {
                throw new ArgumentException(innErrorMessage);
            }

            if (branchOffice.IsNew())
            {
                _branchOfficeGenericRepository.Add(branchOffice);
            }
            else
            {
                _branchOfficeGenericRepository.Update(branchOffice);
            }

            _branchOfficeGenericRepository.Save();
        }

        public void SetPrimaryBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId)
        {
            var branchOfficeOrganizationUnit = _finder.Find<BranchOfficeOrganizationUnit>(x => x.Id == branchOfficeOrganizationUnitId).SingleOrDefault();
            if (branchOfficeOrganizationUnit == null)
            {
                throw new Exception(BLResources.CouldNotFindBranchOfficeOrganizationUnit);
            }

            branchOfficeOrganizationUnit.IsPrimary = true;
            _branchOfficeOrganizationUnitGenericRepository.Update(branchOfficeOrganizationUnit);

            var otherBranchOfficeOrganizationUnits = _finder.Find<OrganizationUnit>(x => x.Id == branchOfficeOrganizationUnit.OrganizationUnitId)
                            .SelectMany(x => x.BranchOfficeOrganizationUnits)
                            .Where(x => x.IsActive && !x.IsDeleted)
                            .Where(x => x.Id != branchOfficeOrganizationUnit.Id).ToArray();

            foreach (var notPrimarybranchOfficeOrganizationUnit in otherBranchOfficeOrganizationUnits)
            {
                notPrimarybranchOfficeOrganizationUnit.IsPrimary = false;
                _branchOfficeOrganizationUnitGenericRepository.Update(notPrimarybranchOfficeOrganizationUnit);
            }

            _branchOfficeOrganizationUnitGenericRepository.Save();
        }

        public void SetPrimaryForRegionalSalesBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId)
        {
            var branchOfficeOrganizationUnit = _finder.Find<BranchOfficeOrganizationUnit>(x => x.Id == branchOfficeOrganizationUnitId).SingleOrDefault();
            if (branchOfficeOrganizationUnit == null)
            {
                throw new Exception(BLResources.CouldNotFindBranchOfficeOrganizationUnit);
            }

            branchOfficeOrganizationUnit.IsPrimaryForRegionalSales = true;
            _branchOfficeOrganizationUnitGenericRepository.Update(branchOfficeOrganizationUnit);

            var otherBranchOfficeOrganizationUnits = _finder.Find<OrganizationUnit>(x => x.Id == branchOfficeOrganizationUnit.OrganizationUnitId)
                            .SelectMany(x => x.BranchOfficeOrganizationUnits)
                            .Where(x => x.IsActive && !x.IsDeleted)
                            .Where(x => x.Id != branchOfficeOrganizationUnit.Id).ToArray();

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
            var entity = _secureFinder.Find(Specs.Find.ById<BranchOffice>(entityId)).Single();
            return Deactivate(entity);
        }

        int IDeactivateAggregateRepository<BranchOfficeOrganizationUnit>.Deactivate(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<BranchOfficeOrganizationUnit>(entityId)).Single();
            return Deactivate(entity);
        }

        int IActivateAggregateRepository<BranchOfficeOrganizationUnit>.Activate(long entityId)
        {
            var branchOfficeOrganizationUnit = _finder.Find(Specs.Find.ById<BranchOfficeOrganizationUnit>(entityId)).Single();

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
            var primaryBranchOfficeOrganizationUnits =
                _finder.Find<OrganizationUnit>(x => x.Id == organizationUnitId)
                       .Select(x => new PrimaryBranchOfficeOrganizationUnits
            {
                               Primary = x.BranchOfficeOrganizationUnits
                                          .Where(y => y.IsActive && !y.IsDeleted)
                                          .FirstOrDefault(y => y.IsPrimary),
                               PrimaryForRegionalSales = x.BranchOfficeOrganizationUnits
                                                          .Where(y => y.IsActive && !y.IsDeleted)
                                                          .FirstOrDefault(y => y.IsPrimaryForRegionalSales),
                           })
                       .Single();

            return primaryBranchOfficeOrganizationUnits;
        }
    }
}