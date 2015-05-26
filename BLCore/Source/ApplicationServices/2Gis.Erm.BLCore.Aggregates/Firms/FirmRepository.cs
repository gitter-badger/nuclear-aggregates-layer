using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms
{
    public class FirmRepository : IFirmRepository
    {
        // timeout should be increased due to long sql updates
        private readonly TimeSpan _importFirmPromisingCommandTimeout = TimeSpan.FromHours(1);

        private readonly IRepository<Client> _clientGenericRepository;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IFinder _finder;
        private readonly IRepository<FirmAddress> _firmAddressGenericRepository;
        private readonly IRepository<Firm> _firmGenericRepository;
        private readonly ISecureRepository<Firm> _firmGenericSecureRepository;
        private readonly IFirmPersistenceService _firmPersistanceService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecureFinder _secureFinder;

        public FirmRepository(IFinder finder,
                              IRepository<Client> clientGenericRepository,
                              IRepository<Firm> firmGenericRepository,
                              IRepository<FirmAddress> firmAddressGenericRepository,
                              ISecureRepository<Firm> firmGenericSecureRepository,
                              ISecurityServiceEntityAccess entityAccessService,
                              ISecurityServiceFunctionalAccess functionalAccessService,
                              ISecureFinder secureFinder,
                              IFirmPersistenceService firmPersistanceService,
                              IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _clientGenericRepository = clientGenericRepository;
            _firmGenericRepository = firmGenericRepository;
            _firmAddressGenericRepository = firmAddressGenericRepository;
            _firmGenericSecureRepository = firmGenericSecureRepository;
            _entityAccessService = entityAccessService;
            _functionalAccessService = functionalAccessService;
            _secureFinder = secureFinder;
            _firmPersistanceService = firmPersistanceService;
            _scopeFactory = scopeFactory;
        }

        public int Assign(Firm firm, long ownerCode)
        {
            firm.OwnerCode = ownerCode;
            _firmGenericSecureRepository.Update(firm);
            return _firmGenericSecureRepository.Save();
        }

        public Firm GetFirm(long firmId)
        {
            return _secureFinder.Find(Specs.Find.ById<Firm>(firmId)).SingleOrDefault();
        }

        public void Update(Firm firm)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Firm>())
            {
                _firmGenericRepository.Update(firm);
                _firmGenericRepository.Save();

                scope.Updated<Firm>(firm.Id);
                scope.Complete();
            }
        }

        public int Qualify(Firm firm, long currentUserCode, long reserveCode, long ownerCode, DateTime qualifyDate)
        {
            if (firm == null)
            {
                throw new ArgumentException(BLResources.CouldNotFindFirm);
            }

            if (!firm.IsActive)
            {
                throw new ArgumentException(BLResources.QualifyFirmNotActive);
            }

            if (firm.ClientId.HasValue)
            {
                throw new ArgumentException(BLResources.QualifyFirmHaveClient);
            }

            if (firm.OwnerCode != reserveCode)
            {
                throw new ArgumentException(BLResources.QualifyOwnerNotReserved);
            }

            var currentUserReserveAccess = GetMaxAccess(_functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.ReserveAccess, currentUserCode));
            switch (currentUserReserveAccess)
            {
                case ReserveAccess.None:
                    throw new SecurityException(BLResources.QualifyReserveOperationDenied);

                case ReserveAccess.Territory:
                    var hasTerritories = _finder.Find(new FindSpecification<UserTerritoriesOrganizationUnits>(x => x.UserId == currentUserCode && x.TerritoryId == firm.TerritoryId)).Any();
                    if (!hasTerritories)
                    {
                        throw new SecurityException(BLResources.QualifyCouldntAccessFirmOnThisTerritory);
                    }

                    break;
                case ReserveAccess.OrganizationUnit:
                    {
                        var hasFirmOrgUnitOrTerritories = _finder.Find<UserTerritoriesOrganizationUnits>(x => x.UserId == currentUserCode &&
                                                                                                              (x.OrganizationUnitId == firm.OrganizationUnitId ||
                                                                                                               x.TerritoryId == firm.TerritoryId))
                                                                 .Any();
                        if (!hasFirmOrgUnitOrTerritories)
                        {
                            throw new SecurityException(BLResources.QualifyCouldntAccessFirmOnThisOrgUnit);
                        }
                    }

                    break;
                case ReserveAccess.Full:
                    break;

                default:
                    throw new NotSupportedException();
            }

            firm.OwnerCode = ownerCode;
            firm.LastQualifyTime = qualifyDate;

            // Изменения логируются в вызывающем коде
            _firmGenericSecureRepository.Update(firm);
            return _firmGenericSecureRepository.Save();
        }

        public Client PerformDisqualificationChecks(long firmId, long currentUserCode)
        {
            var clientId = _secureFinder.Find(Specs.Find.ById<Firm>(firmId))
                                      .Select(x => x.ClientId)
                                      .SingleOrDefault();

            if (clientId == null)
            {
                throw new ArgumentException(BLResources.DisqualifyCantFindFirmClient);
            }

            var client = _secureFinder.FindOne(Specs.Find.ById<Client>(clientId.Value));

            var hasClientPrivileges = _entityAccessService.HasEntityAccess(EntityAccessTypes.Update,
                                                                           EntityType.Instance.Client(),
                                                                           currentUserCode,
                                                                           client.Id,
                                                                           client.OwnerCode,
                                                                           null);
            if (!hasClientPrivileges)
            {
                throw new SecurityException(BLResources.YouHasNoEntityAccessPrivilege);
            }

            var firmHasOpenOrders = _finder.Find(OrderSpecs.Orders.Find.ActiveOrdersForFirm(firmId)).Any();
            if (firmHasOpenOrders)
            {
                throw new ArgumentException(BLResources.DisqualifyFirmNeedToCloseAllOrders);
            }

            return client;
        }

        public int Disqualify(Firm firm, long currentUserCode, long reserveCode, DateTime disqulifyDate)
        {
            var client = PerformDisqualificationChecks(firm.Id, currentUserCode);

            var clientFirmsCountEqualToOne =
                _finder
                    .Find(Specs.Find.NotDeleted<Firm>() && FirmSpecs.Firms.Find.ByClient(client.Id))
                    .Count() <= 1;
            if (clientFirmsCountEqualToOne)
            {
                if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LeaveClientWithNoFirms, currentUserCode))
                {
                    throw new SecurityException(BLResources.DisqualifyClientHasOnlyOneFirm);
                }
            }

            var updateCount = 0;

            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Firm>())
            {
                firm.OwnerCode = reserveCode;
                firm.ClientId = null;
                firm.LastDisqualifyTime = disqulifyDate;

                _firmGenericSecureRepository.Update(firm);

                updateCount += _firmGenericSecureRepository.Save();
                scope.Updated<Firm>(firm.Id)
                     .Complete();
            }

            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Client>())
            {
                if (client.MainFirmId == firm.Id)
                {
                    client.MainFirmId = null;

                    _clientGenericRepository.Update(client);
                    updateCount += _clientGenericRepository.Save();
                }

                // В терминах бизнес-логики клиент поменялся в любом случае: он лишился фирмы
                scope.Updated<Client>(client.Id)
                     .Complete();
            }

            // Сбрасываем права выданные через "Шаринг прав"
            // Шаг 6. Сбрасываем все права для всех Пользователей, выданные через "Шаринга прав" на данную Фирму. 
            // TODO: Сделать шаринг прав
            return updateCount;
        }

        public Client GetFirmClient(long firmId)
        {
            return _finder.FindOne(ClientSpecs.Clients.Find.ByFirm(firmId));
        }

        public int SetFirmClient(Firm firm, long clientId)
        {
            firm.ClientId = clientId;

            // Изменения логируются в вызывающем коде
            _firmGenericSecureRepository.Update(firm);
            return _firmGenericSecureRepository.Save();
        }

        public int ChangeTerritory(Firm firm, long territoryId)
        {
            if (firm.TerritoryId == territoryId)
            {
                return 0;
            }

            firm.TerritoryId = territoryId;

            // Изменения логируются в вызывающем коде
            _firmGenericSecureRepository.Update(firm);
            return _firmGenericSecureRepository.Save();
        }

        public int? GetOrganizationUnitDgppId(long organizationUnitId)
        {
            return
                _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId) && Specs.Find.ActiveAndNotDeleted<OrganizationUnit>())
                       .Select(unit => unit.DgppId)
                       .SingleOrDefault();
        }

        public void ImportFirmPromisingValues(long userId)
        {
            var organizationUnitDgppIds = _finder.Find(new FindSpecification<OrganizationUnit>(x => x.ErmLaunchDate != null && x.DgppId != null))
                                                 .Select(x => x.DgppId.Value)
                                                 .ToArray();

            foreach (var organizationUnitDgppId in organizationUnitDgppIds)
            {
                using (var scope = _scopeFactory.CreateNonCoupled<ImportFirmPromisingIdentity>())
                {
                    var updatedFirms = _firmPersistanceService.ImportFirmPromising(organizationUnitDgppId, userId, _importFirmPromisingCommandTimeout);

                    scope.Updated<Firm>(updatedFirms);
                    scope.Complete();
                }
            }
        }

        public IEnumerable<Firm> GetFirmsByTerritory(long territoryId)
        {
            return _finder.Find(new FindSpecification<Firm>(x => x.TerritoryId == territoryId)).ToArray();
        }

        public int ChangeTerritory(IEnumerable<Firm> firms, long territoryId)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<ChangeTerritoryIdentity, Firm>())
            {
                foreach (var firm in firms)
                {
                    firm.TerritoryId = territoryId;
                    _firmGenericSecureRepository.Update(firm);
                    scope.Updated<Firm>(firm.Id);
                }

                scope.Complete();
            }

            return _firmGenericSecureRepository.Save();
        }

        public long[] GetAdvertisementIds(long firmId)
        {
            var advertisementIds =
                _finder.Find(new FindSpecification<Firm>(x => x.Id == firmId)).SelectMany(x => x.Advertisements).Where(x => !x.IsDeleted).Select(x => x.Id).ToArray();
            return advertisementIds;
        }

        int IQualifyAggregateRepository<Firm>.Qualify(long entityId, long currentUserCode, long reserveCode, long ownerCode, DateTime qualifyDate)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<Firm>(entityId)).Single();
            return Qualify(entity, currentUserCode, reserveCode, ownerCode, qualifyDate);
        }

        int IDisqualifyAggregateRepository<Firm>.Disqualify(long entityId,
                                                            long currentUserCode,
                                                            long reserveCode,
                                                            bool bypassValidation,
                                                            DateTime disqualifyDate)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<Firm>(entityId)).Single();
            return Disqualify(entity, currentUserCode, reserveCode, disqualifyDate);
        }

        int IChangeAggregateTerritoryRepository<Firm>.ChangeTerritory(long entityId, long territoryId)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<Firm>(entityId)).Single();
            return ChangeTerritory(entity, territoryId);
        }

        ChangeAggregateClientValidationResult IChangeAggregateClientRepository<Firm>.Validate(long entityId, long currentUserCode, long reserveCode)
        {
            var warnings = new List<string>();
            var securityErrors = new List<string>();
            var domainErrors = new List<string>();
            var result = new ChangeAggregateClientValidationResult(warnings, securityErrors, domainErrors);

            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ChangeFirmClient, currentUserCode))
            {
                securityErrors.Add(BLResources.NoFunctionalPrivilegeForChangingClientOfFirm);
                return result;
            }

            var firmCountForFirmClient =
                _finder
                    .Find<Firm, int>(Specs.Find.ById<Firm>(entityId) && FirmSpecs.Firms.Find.HasClient(), FirmSpecs.Firms.Select.FirmCountForFirmClient())
                    .SingleOrDefault();
            if (firmCountForFirmClient == 1)
            {
                if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LeaveClientWithNoFirms, currentUserCode))
                {
                    securityErrors.Add(BLResources.TheClientFirmIsTheOnlyOne);
                    return result;
                }

                warnings.Add(BLResources.TheClientFirmIsTheOnlyOneWarning);
            }

            var firmInfo = _finder.Find(Specs.Find.ById<Firm>(entityId)).Select(x => new { x.Id, x.Name, x.OwnerCode }).Single();
            if (firmInfo.OwnerCode == reserveCode)
            {
                domainErrors.Add(string.Format(BLResources.Firm_PleaseUseQualifyOperstion, firmInfo.Name));
                return result;
            }

            // validate security
            if (!_entityAccessService.HasEntityAccess(EntityAccessTypes.Update, EntityType.Instance.Firm(), currentUserCode, firmInfo.Id, firmInfo.OwnerCode, null))
            {
                securityErrors.Add(BLResources.YouHasNoEntityAccessPrivilege);
                return result;
            }

            return result;
        }

        int IChangeAggregateClientRepository<Firm>.ChangeClient(long entityId, long clientId, long currentUserCode, bool bypassValidation)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<Firm>(entityId)).Single();
            var clientOwnerCode = _finder.Find(Specs.Find.ById<Client>(clientId)).Select(x => x.OwnerCode).Single();

            entity.ClientId = clientId;
            entity.OwnerCode = clientOwnerCode;

            // Изменения логируются в вызывающем коде
            _firmGenericSecureRepository.Update(entity);
            return _firmGenericSecureRepository.Save();
        }

        int IAssignAggregateRepository<Firm>.Assign(long entityId, long ownerCode)
        {
            var firm = GetFirm(entityId);
            return Assign(firm, ownerCode);
        }

        public bool IsTerritoryReplaceable(long oldTerritoryId, long newTerritoryId)
        {
            // Чтобы территории были взаимозаменяемыми, они должны принадлежать одному подразделению
            var oldOrganizationUnit = _finder.Find(Specs.Find.ById<Territory>(oldTerritoryId))
                                             .Select(territory => territory.OrganizationUnit.Id)
                                             .Single();
            var newOrganizationUnit = _finder.Find(Specs.Find.ById<Territory>(newTerritoryId))
                                             .Select(territory => territory.OrganizationUnit.Id)
                                             .Single();

            return oldOrganizationUnit == newOrganizationUnit;
        }

        public void UpdateFirmAddresses(IEnumerable<FirmAddress> firmAddresses)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, FirmAddress>())
            {
                foreach (var firmAddress in firmAddresses)
                {
                    _firmAddressGenericRepository.Update(firmAddress);
                    scope.Updated<FirmAddress>(firmAddress.Id);
                }

                _firmAddressGenericRepository.Save();
                scope.Complete();
            }
        }

        private static ReserveAccess GetMaxAccess(int[] accesses)
        {
            if (!accesses.Any())
            {
                return ReserveAccess.None;
            }

            var priorities = new[] { ReserveAccess.None, ReserveAccess.Territory, ReserveAccess.OrganizationUnit, ReserveAccess.Full };

            var maxPriority = accesses.Select(x => Array.IndexOf(priorities, (ReserveAccess)x)).Max();
            return priorities[maxPriority];
        }
    }
}