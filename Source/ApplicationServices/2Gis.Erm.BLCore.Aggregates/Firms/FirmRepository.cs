using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Firms.DTO.CardForErm;
using DoubleGis.Erm.BLCore.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms
{
    public class FirmRepository : IFirmRepository
    {
        // timeout should be increased due to long sql updates (15:00:00 min = 900 sec)
        private const int ImportCommandTimeout = 900;
        private const int ImportFirmPrimisingCommandTimeout = 3600;

        private readonly IFinder _finder;
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IRepository<Firm> _firmGenericRepository;
        private readonly IRepository<FirmAddress> _firmAddressGenericRepository;
        private readonly IRepository<FirmContact> _firmContactGenericRepository;
        private readonly IRepository<CategoryFirmAddress> _categoryFirmAddressGenericRepository;
        private readonly IRepository<Client> _clientGenericRepository;
        private readonly IRepository<CityPhoneZone> _cityPhoneZoneGenericRepository;
        private readonly IRepository<Reference> _refrenceGenericRepository;
        private readonly IRepository<ReferenceItem> _refrenceItemGenericRepository;
        private readonly IRepository<CardRelation> _cardRelationGenericRepository;
        private readonly IRepository<Territory> _territoryGenericRepository;
        private readonly IRepository<FirmAddressService> _firmAddressServiceGenericRepository;
        private readonly ISecureRepository<Firm> _firmGenericSecureRepository;
        private readonly IRepository<Territory> _territorySecureRepository;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ISecureFinder _secureFinder;
        private readonly IFirmPersistenceService _firmPersistanceService;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IRepository<DepCard> _depCardRepository;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public FirmRepository(
            IFinder finder,
            IMsCrmSettings msCrmSettings,
            IRepository<Client> clientGenericRepository,
            IRepository<Firm> firmGenericRepository,
            IRepository<FirmAddress> firmAddressGenericRepository,
            IRepository<FirmContact> firmContactGenericRepository,
            IRepository<CategoryFirmAddress> categoryFirmAddressGenericRepository,
            IRepository<CityPhoneZone> cityPhoneZoneGenericRepository,
            IRepository<Reference> refrenceGenericRepository,
            IRepository<ReferenceItem> refrenceItemGenericRepository,
            IRepository<CardRelation> cardRelationGenericRepository,
            IRepository<FirmAddressService> firmAddressServiceGenericRepository,
            IRepository<Territory> territoryGenericRepository,
            ISecureRepository<Firm> firmGenericSecureRepository,
            IRepository<Territory> territorySecureRepository,
            ISecurityServiceEntityAccess entityAccessService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            ISecureFinder secureFinder,
            IFirmPersistenceService firmPersistanceService,
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory, 
            IRepository<DepCard> depCardRepository, 
            ISecurityServiceUserIdentifier securityServiceUserIdentifier)
        {
            _finder = finder;
            _msCrmSettings = msCrmSettings;
            _clientGenericRepository = clientGenericRepository;
            _firmGenericRepository = firmGenericRepository;
            _firmAddressGenericRepository = firmAddressGenericRepository;
            _firmContactGenericRepository = firmContactGenericRepository;
            _categoryFirmAddressGenericRepository = categoryFirmAddressGenericRepository;
            _cityPhoneZoneGenericRepository = cityPhoneZoneGenericRepository;
            _refrenceGenericRepository = refrenceGenericRepository;
            _refrenceItemGenericRepository = refrenceItemGenericRepository;
            _cardRelationGenericRepository = cardRelationGenericRepository;
            _firmAddressServiceGenericRepository = firmAddressServiceGenericRepository;
            _firmGenericSecureRepository = firmGenericSecureRepository;
            _territorySecureRepository = territorySecureRepository;
            _entityAccessService = entityAccessService;
            _functionalAccessService = functionalAccessService;
            _secureFinder = secureFinder;
            _firmPersistanceService = firmPersistanceService;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
            _depCardRepository = depCardRepository;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
            _territoryGenericRepository = territoryGenericRepository;
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

        public void Update(FirmAddress firmAddress)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, FirmAddress>())
            {
                _firmAddressGenericRepository.Update(firmAddress);
                _firmAddressGenericRepository.Save();
                scope.Updated<FirmAddress>(firmAddress.Id)
                     .Complete();
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
                    var hasTerritories =
                        _finder.Find<UserTerritoriesOrganizationUnits>(x => x.UserId == currentUserCode && x.TerritoryId == firm.TerritoryId).Any();
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
            _firmGenericSecureRepository.Update(firm);
            return _firmGenericSecureRepository.Save();
        }

        public Client PerformDisqualificationChecks(long firmId, long currentUserCode)
        {
            var client = _secureFinder.Find(Specs.Find.ById<Firm>(firmId))
                                      .Select(x => x.Client)
                                      .SingleOrDefault();

            if (client == null)
            {
                throw new ArgumentException(BLResources.DisqualifyCantFindFirmClient);
            }

            var hasClientPrivileges = _entityAccessService.HasEntityAccess(EntityAccessTypes.Update,
                                                                           EntityName.Client,
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
            return _finder.Find(Specs.Find.ById<Firm>(firmId)).Select(x => x.Client).SingleOrDefault();
        }

        public int SetFirmClient(Firm firm, long clientId)
        {
            firm.ClientId = clientId;
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
            _firmGenericSecureRepository.Update(firm);
            return _firmGenericSecureRepository.Save();
        }

        public IEnumerable<long> GetFirmAddressesIds(long firmId)
        {
            return _finder.Find(FirmSpecs.Addresses.Find.ActiveAddresses(firmId))
                .Select(address => address.Id)
                .ToArray();
        }

        public long GetOrderFirmId(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId)).Select(x => x.FirmId).Single();
        }

        public IEnumerable<long> GetFirmNonArchivedOrderIds(long firmId)
        {
            return _finder.Find(OrderSpecs.Orders.Find.ActiveOrdersForFirm(firmId)).Select(x => x.Id).ToArray();
        }

        public IEnumerable<OrganizationUnitDto> ExportFirmWithActiveOrders()
        {
            // Выгрузка фирм идет только на текущий период, поскольку версионирования данных в системе не выполняется, 
            // то всегда выгрузка имеет смысл только в контексте "на текущий период"
            var currentMonthLastDate = DateTime.Today.GetEndPeriodOfThisMonth();

            // выгрузка по городам, переведённым на ERM, но не переведённым на InfoRussia
            var organizationUnitDtos = _finder.Find<OrganizationUnit>(x => x.IsActive && !x.IsDeleted)
                .Where(x => x.DgppId != null && x.ErmLaunchDate != null && x.InfoRussiaLaunchDate == null)
                .Select(x => new OrganizationUnitDto
                    {
                        Id = x.Id,
                        DgppId = x.DgppId.Value,
                        Name = x.Name,

                        FirmDtos = x.Firms
                                 .Where(y => y.IsActive && !y.IsDeleted)
                                 .Select(y => new FirmDto
                                     {
                                         OrderDtos = y.Orders
                                                  .Where(z => z.IsActive && !z.IsDeleted &&
                                                              z.EndDistributionDateFact >= currentMonthLastDate &&
                                                              (z.WorkflowStepId == (int)OrderState.Approved ||
                                                               z.WorkflowStepId == (int)OrderState.OnTermination ||
                                                               z.WorkflowStepId == (int)OrderState.Archive))
                                                  .Select(z => new OrderDto
                                                      {
                                                          Id = z.Id,
                                                          Number = z.Number,
                                                          BeginDistributionDate = z.BeginDistributionDate,
                                                          EndDistributionDateFact = z.EndDistributionDateFact,
                                                      }),
                                     })
                                 .Where(y => y.OrderDtos.Any()),
                    })
                .Where(x => x.FirmDtos.Any())
                .ToArray();

            return organizationUnitDtos;
        }

        public CompactFirmDto GetFirmInBrief(long firmId)
        {
            return _finder.Find<Firm>(x => x.Id == firmId && !x.IsDeleted)
                .Select(x => new CompactFirmDto
                {
                    FirmId = x.Id,
                    FirmName = x.Name,
                    OrganizationUnitId = x.OrganizationUnitId,
                    OrganizationUnitName = x.OrganizationUnit.Name,
                    OwnerCode = x.OwnerCode
                })
                .SingleOrDefault();
        }

        public OrganizationUnit GetOrganizationUnit(int organizationUnitDgppId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>()).SingleOrDefault(x => x.DgppId == organizationUnitDgppId);
        }

        public FirmAndClientDto GetFirmAndClientByFirmAddress(long firmAddressCode)
        {
            return _finder.Find(Specs.Find.ById<FirmAddress>(firmAddressCode))
                          .Where(x => !x.IsDeleted)
                          .Select(x => new FirmAndClientDto
                              {
                                  Firm = x.Firm != null && !x.Firm.IsDeleted ? x.Firm : (Firm)null,
                                  Client = x.Firm != null && !x.Firm.IsDeleted ? (!x.Firm.Client.IsDeleted ? x.Firm.Client : (Client)null) : (Client)null
                              })
                          .FirstOrDefault();
        }

        public int? GetOrganizationUnitDgppId(long organizationUnitId)
        {
            return
                _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId) && Specs.Find.ActiveAndNotDeleted<OrganizationUnit>())
                .Select(unit => unit.DgppId)
                .SingleOrDefault();
        }

        public Territory ImportTerritory(ImportTerritoriesHeaderDto header, ImportTerritoryDto territoryDto)
        {
                var territory = _finder.Find(Specs.Find.ById<Territory>(territoryDto.DgppId)).SingleOrDefault()
                            ?? new Territory { OrganizationUnitId = header.OrganizationUnitId, Id = territoryDto.DgppId };

            territory.Name = territoryDto.Name;
            territory.IsActive = !territoryDto.IsDeleted; // в старом варианте именно так.

            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(territory))
            {
                if (territory.IsNew())
                {
                    _territoryGenericRepository.Add(territory);
                    scope.Added<Territory>(territory.Id);
                }
                else
                {
                    _territoryGenericRepository.Update(territory);
                    scope.Updated<Territory>(territory.Id);
                }

                _territoryGenericRepository.Save();
                scope.Complete();
            }

            // update firm and client
            // Старый вариант использовал число 4096. Почему не 42? Не знаю.
            const int FirmUpdateBlockSize = 4096;
            var position = 0;

            while (position < territoryDto.Firms.Count())
            {
                var slice = territoryDto.Firms.Skip(position).Take(FirmUpdateBlockSize);
                position += FirmUpdateBlockSize;

                var updateInfos = _finder.Find<Firm>(firm => slice.Contains(firm.Id))
                                         .Select(x => new
                                             {
                                                 Firm = x,
                                                 x.Client,
                                                 IsClientOnActiveTerritory = (bool?)x.Client.Territory.IsActive,
                                             })
                                         .ToArray();

                var firmsToUpdate = updateInfos.Select(x => x.Firm).ToArray();
                var clientsToUpdate = updateInfos.Where(x => x.IsClientOnActiveTerritory == false &&
                                                             (x.Client.MainFirmId == null || x.Client.MainFirmId == x.Firm.Id))
                                                 .Select(x => x.Client)
                                                 .DistinctBy(x => x.Id)
                                                 .ToArray();

                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Firm>())
                {
                    foreach (var firm in firmsToUpdate)
                    {
                        firm.TerritoryId = territory.Id;
                        _firmGenericSecureRepository.Update(firm);
                        scope.Updated<Firm>(firm.Id);
                    }

                    _firmGenericSecureRepository.Save();
                    scope.Complete();
                }

                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Client>())
                {
                    foreach (var client in clientsToUpdate)
                    {
                        client.TerritoryId = territory.Id;
                        _clientGenericRepository.Update(client);
                        scope.Updated<Client>(client.Id);
                    }

                    _clientGenericRepository.Save();
                    scope.Complete();
                }
            }

            return territory;
        }

        // ключ - Дгпп, значение - Ерм
        public IEnumerable<long> GetTerritoriesOfOrganizationUnit(long organizationUnitId)
        {
            return _finder.Find<Territory>(x => x.OrganizationUnit.DgppId != null && x.OrganizationUnitId == organizationUnitId)
                .Select(x => x.Id)
                .ToArray();
        }

        // ключ - Дгпп, значение - Ерм
        public IDictionary<int, long> GetOrganizationUnits()
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>())
                .Where(unit => unit.DgppId != null)
                .Select(unit => new { unit.DgppId, unit.Id })
                .ToDictionary(x => (int)x.DgppId, x => x.Id);
        }

        // ключ - Дгпп, значение - Ерм
        public IEnumerable<long> GetCategoriesOfOrganizationUnit(long organizationUnitId)
        {
            return _finder.Find<Category>(x => x.CategoryOrganizationUnits.Any(y => y.OrganizationUnitId == organizationUnitId))
                          .Select(x => x.Id)
                          .ToArray();
        }

        public Firm ImportFirmFromDgpp(ImportFirmDto dto, FirmImportContext context)
        {
            if (!context.Territories.Contains(dto.TerritoryDgppId))
            {
                var message = string.Format(BLResources.TerritoryWithDgppIdNotFound, dto.TerritoryDgppId);
                throw new ArgumentException(message);
            }

            long organizationUnitId;
            if (!context.OrganizationUnits.TryGetValue(dto.OrganizationUnitDgppId, out organizationUnitId))
            {
                throw new ArgumentException(string.Format(BLResources.CannotFindOrgUnitById, dto.OrganizationUnitDgppId));
            }

            var firm = _finder.Find<Firm>(f => f.Id == dto.DgppId).SingleOrDefault()
                       ?? new Firm { Id = dto.DgppId, OwnerCode = context.ReserveUserIdentity.Code };

            firm.TerritoryId = dto.TerritoryDgppId;
            firm.OrganizationUnitId = organizationUnitId;
            firm.ClosedForAscertainment = dto.IsClosedForAscertainment;
            firm.IsDeleted = dto.IsDeleted;
            firm.IsActive = dto.IsActive;
            firm.Name = dto.Name;

            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(firm))
            {
                if (firm.IsNew())
                {
                    _firmGenericRepository.Add(firm);
                    scope.Added<Firm>(firm.Id);
                }
                else
                {
                    _firmGenericRepository.Update(firm);
                    scope.Updated<Firm>(firm.Id);
                }

                _firmGenericRepository.Save();
                scope.Complete();
            }

            return firm;
        }

        public IEnumerable<FirmAddress> ImportFirmAddresses(Firm firm, ImportFirmDto dto, FirmImportContext context)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                // Предварительная выборка модифицируемых адресов из базы данных для оптимизации обработки
                var addressDgppIds = dto.Addresses.Select(addressDto => addressDto.DgppId).ToArray();
                var firmAddresses = _finder.Find<FirmAddress>(x => addressDgppIds.Contains(x.Id)).ToDictionary(address => address.Id);

                // Обработка адресов, указанных в сообщении
                var result = new List<FirmAddress>();
                foreach (var firmAddressDto in dto.Addresses)
                {
                    var firmAddressDtoClosure = firmAddressDto;
                    FirmAddress firmAddress;
                    if (!firmAddresses.TryGetValue(firmAddressDto.DgppId, out firmAddress) || firmAddress == null)
                    {
                        firmAddress = new FirmAddress { Id = firmAddressDtoClosure.DgppId };
                    }

                    firmAddress.FirmId = firm.Id;
                    firmAddress.Address = firmAddressDtoClosure.Address;
                    firmAddress.ClosedForAscertainment = firmAddressDtoClosure.IsClosedForAscertainment;
                    firmAddress.PaymentMethods = firmAddressDtoClosure.PaymentMethods;
                    firmAddress.SortingPosition = firmAddressDtoClosure.SortingPosition;
                    firmAddress.WorkingTime = firmAddressDtoClosure.WorkingTime;
                    firmAddress.IsActive = !firm.IsDeleted && firmAddressDtoClosure.IsActive;
                    firmAddress.IsDeleted = firm.IsDeleted || !firmAddressDtoClosure.IsActive;
                    firmAddress.IsLocatedOnTheMap = true;

                    if (firmAddress.IsNew())
                    {
                        _firmAddressGenericRepository.Add(firmAddress);
                    }
                    else
                    {
                        _firmAddressGenericRepository.Update(firmAddress);
                    }

                    result.Add(firmAddress);
                }

                // Не указанные в сообщении адреса помечаются удалёнными, равно как и всё с ними связанное.
                var addressesToDelete = _finder.Find<Firm>(f => f.Id == dto.DgppId)
                    .SelectMany(f => f.FirmAddresses)
                    .Where(f => !f.IsDeleted)
                    .Where(f => !addressDgppIds.Contains(f.Id))
                        .Select(f => new { Address = f, CategoryFirmAddresses = f.CategoryFirmAddresses.Where(category => !category.IsDeleted) })
                        .ToArray();

                foreach (var addressToDelete in addressesToDelete)
                {
                    foreach (var categoryFirmAddress in addressToDelete.CategoryFirmAddresses)
                    {
                        _categoryFirmAddressGenericRepository.Delete(categoryFirmAddress);
                    }

                    _firmAddressGenericRepository.Delete(addressToDelete.Address);
                }

                _categoryFirmAddressGenericRepository.Save();
                _firmAddressGenericRepository.Save();

                transaction.Complete();

                return result;
            }
        }

        public void ImportAddressContacts(FirmAddress firmAddress, DTO.ImportFirmAddressDto dto)
        {
            var existingFirmContacts = _finder.Find<FirmAddress>(x => x.Id == firmAddress.Id).SelectMany(x => x.FirmContacts).ToArray();
            var contacts = dto.Contacts.OrderBy(x => x.SortingPosition).ToArray();

            for (var i = 0; i < contacts.Length; i++)
            {
                var firmContactDto = contacts[i];
                var firmContact = (i < existingFirmContacts.Length) ? existingFirmContacts[i] : new FirmContact();

                firmContact.FirmAddressId = firmAddress.Id;
                firmContact.ContactType = firmContactDto.ContactType;
                firmContact.Contact = firmContactDto.Contact;
                firmContact.SortingPosition = i + 1;
                if (firmContact.IsNew())
                {
                    _identityProvider.SetFor(firmContact);
                    _firmContactGenericRepository.Add(firmContact);
                }
                else
                {
                    _firmContactGenericRepository.Update(firmContact);
                }
            }

            // delete unneeded
            for (var i = contacts.Length; i < existingFirmContacts.Length; i++)
            {
                _firmContactGenericRepository.Delete(existingFirmContacts[i]);
            }

            _firmContactGenericRepository.Save();
        }

        public void ImportAddressCategories(FirmAddress firmAddress, DTO.ImportFirmAddressDto dto, FirmImportContext context)
        {
            var existingCategoryFirmAddresses = _finder.Find<FirmAddress>(x => x.Id == firmAddress.Id)
                .SelectMany(x => x.CategoryFirmAddresses)
                .ToDictionary(x => x.CategoryId);

            foreach (var category in dto.Categories)
            {
                if (!context.Categories.Contains(category.DgppId))
                {
                    var message = string.Format(BLResources.CannotFindOrgUnitWithDgppIdWhileCreatingFirmAddressWithDgppId, category.DgppId, firmAddress.Id);
                    throw new ArgumentException(message);
                }

                CategoryFirmAddress categoryFirmAddress;
                if (!existingCategoryFirmAddresses.TryGetValue(category.DgppId, out categoryFirmAddress))
                {
                    categoryFirmAddress = new CategoryFirmAddress { CategoryId = category.DgppId };
                }

                categoryFirmAddress.FirmAddressId = firmAddress.Id;
                categoryFirmAddress.SortingPosition = category.SortingPosition;
                categoryFirmAddress.IsPrimary = category.IsPrimary;
                categoryFirmAddress.IsActive = true;
                categoryFirmAddress.IsDeleted = false;
                if (categoryFirmAddress.IsNew())
                {
                    _identityProvider.SetFor(categoryFirmAddress);
                    _categoryFirmAddressGenericRepository.Add(categoryFirmAddress);
                }
                else
                {
                    _categoryFirmAddressGenericRepository.Update(categoryFirmAddress);
                }
            }

            // delete unneeded categories
            var categoryDgppIds = dto.Categories.Select(x => x.DgppId).ToArray();

            var categoriesToDelete = _finder.Find<FirmAddress>(x => x.Id == firmAddress.Id)
                .SelectMany(x => x.CategoryFirmAddresses)
                .Where(x => !x.IsDeleted)
                .Where(x => !categoryDgppIds.Contains(x.Category.Id))
                .ToArray();

            foreach (var categoryToDelete in categoriesToDelete)
            {
                _categoryFirmAddressGenericRepository.Delete(categoryToDelete);
            }

            _categoryFirmAddressGenericRepository.Save();
        }

        public void DeleteFirmRelatedObjects(Firm firm)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var addresses = _finder.Find<FirmAddress>(address => address.FirmId == firm.Id).Select(x => new { FirmAddress = x, x.CategoryFirmAddresses });

                foreach (var firmAddressInfo in addresses)
                {
                    var firmAddress = firmAddressInfo.FirmAddress;

                    firmAddress.IsActive = false;
                    firmAddress.ClosedForAscertainment = true;
                    firmAddress.IsDeleted = true;
                    _firmAddressGenericRepository.Update(firmAddress);

                    foreach (var categoryFirmAddress in firmAddressInfo.CategoryFirmAddresses)
                    {
                        categoryFirmAddress.IsActive = false;
                        categoryFirmAddress.IsDeleted = true;

                        _categoryFirmAddressGenericRepository.Update(categoryFirmAddress);
                    }
                }

                _firmAddressGenericRepository.Save();
                _categoryFirmAddressGenericRepository.Save();

                transaction.Complete();
            }
        }

        public void ImportFirmPromisingValues(long userId)
        {
            var organizationUnitDgppIds = _finder.Find<OrganizationUnit>(x => x.ErmLaunchDate != null && x.DgppId != null)
                                                 .Select(x => x.DgppId.Value)
                                                 .ToArray();

            foreach (var organizationUnitDgppId in organizationUnitDgppIds)
            {
                _firmPersistanceService.ImportFirmPromising(organizationUnitDgppId, userId, ImportFirmPrimisingCommandTimeout, false);
            }
        }

        public ImportFirmsResultDto ImportFirmFromServiceBus(ImportFirmServiceBusDto dto, long userId, long reserveUserId, int pregeneratedIdsAmount, string regionalTerritoryLocaleSpecificWord)
        {
            var result = new ImportFirmsResultDto();
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                if (dto.ReferenceDtos.Any())
                {
                    foreach (var referenceDto in dto.ReferenceDtos)
                    {
                        ProcessReferenceDto(referenceDto);
                    }

                    _refrenceGenericRepository.Save();
                }

                if (dto.ReferenceItemDtos.Any())
                {
                    foreach (var referenceItemDto in dto.ReferenceItemDtos)
                    {
                        ProcessReferenceItemDto(referenceItemDto);
                    }

                    _refrenceItemGenericRepository.Save();
                }

                if (!string.IsNullOrEmpty(dto.CardsXml))
                {
                    result.FirmIdsOfImportedCards = _firmPersistanceService.ImportCardsFromXml(dto.CardsXml, userId, reserveUserId, ImportCommandTimeout, pregeneratedIdsAmount, regionalTerritoryLocaleSpecificWord);
                }

                if (dto.CardRelationDtos.Any())
                {
                    foreach (var cardRelationDto in dto.CardRelationDtos)
                    {
                        ProcessCardRelationDto(cardRelationDto);
                    }

                    _cardRelationGenericRepository.Save();
                }

                if (!string.IsNullOrEmpty(dto.FirmXml))
                {
                    _firmPersistanceService.ImportFirmFromXml(dto.FirmXml, userId, reserveUserId, ImportCommandTimeout, _msCrmSettings.EnableReplication, regionalTerritoryLocaleSpecificWord);
                }

                transaction.Complete();
            }

            return result;
        }

        public IEnumerable<Firm> GetFirmsByTerritory(long territoryId)
        {
            return _finder.Find<Firm>(x => x.TerritoryId == territoryId).ToArray();
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

            return _firmGenericRepository.Save();
        }

        public long[] GetAdvertisementIds(long firmId)
        {
            var advertisementIds = _finder.Find<Firm>(x => x.Id == firmId).SelectMany(x => x.Advertisements).Where(x => !x.IsDeleted).Select(x => x.Id).ToArray();
            return advertisementIds;
        }

        public IEnumerable<AdditionalServicesDto> GetFirmAdditionalServices(long firmId)
        {
            var firmAddressIds = _finder.Find<Firm>(x => x.Id == firmId).SelectMany(x => x.FirmAddresses)
                .Where(x => !x.IsDeleted && x.IsActive && !x.ClosedForAscertainment)
                .OrderBy(x => x.Id)
                .Select(x => x.Id)
                .ToArray();

            var dataToGroup = _finder.Find<AdditionalFirmService>(x => x.IsManaged)
                .SelectMany(x => x.FirmAddressServices.Where(y => firmAddressIds.Contains(y.FirmAddressId)).DefaultIfEmpty(),
            (x, y) => new
            {
                x.ServiceCode,
                x.Description,
                FirmAddressId = (long?)y.FirmAddressId,
                DisplayService = (y != null) ? (y.DisplayService ? AdditionalServiceDisplay.Display : AdditionalServiceDisplay.DoNotDisplay) : AdditionalServiceDisplay.Default,
            })
                .ToArray();

            var additionalServices = dataToGroup.GroupBy(x => new { x.ServiceCode, x.Description }, group => new { group.FirmAddressId, group.DisplayService })
                                                .Select(x =>
                                                    {
                                                        var additionalService = new AdditionalServicesDto
                                                            {
                                                                ServiceCode = x.Key.ServiceCode,
                                                                Description = x.Key.Description
                                                            };

                                                        var isGroupAllDefault =
                                                            x.All(y => y.DisplayService == AdditionalServiceDisplay.Default && y.FirmAddressId == null);
                                                        if (isGroupAllDefault)
                                                        {
                                                            additionalService.DisplayService = AdditionalServiceDisplay.Default;
                                                            return additionalService;
                                                        }

                                                        var isGroupAllDisplay = x.All(y => y.DisplayService == AdditionalServiceDisplay.Display) &&
                                                                                x.Where(y => y.FirmAddressId != null)
                                                                                 .OrderBy(y => y.FirmAddressId)
                                                                                 .Select(y => y.FirmAddressId.Value)
                                                                                 .SequenceEqual(firmAddressIds);
                                                        if (isGroupAllDisplay)
                                                        {
                                                            additionalService.DisplayService = AdditionalServiceDisplay.Display;
                                                            return additionalService;
                                                        }

                                                        var isGroupAllDoNotDisplay = x.All(y => y.DisplayService == AdditionalServiceDisplay.DoNotDisplay) &&
                                                                                     x.Where(y => y.FirmAddressId != null)
                                                                                      .OrderBy(y => y.FirmAddressId)
                                                                                      .Select(y => y.FirmAddressId.Value)
                                                                                      .SequenceEqual(firmAddressIds);
                                                        if (isGroupAllDoNotDisplay)
                                                        {
                                                            additionalService.DisplayService = AdditionalServiceDisplay.DoNotDisplay;
                                                            return additionalService;
                                                        }

                                                        additionalService.DisplayService = AdditionalServiceDisplay.DependsOnAddress;
                                                        return additionalService;
                                                    })
                                                .ToArray();

            return additionalServices;
        }

        public void SetFirmAdditionalServices(long firmId, IEnumerable<AdditionalServicesDto> additionalServices)
        {
            var firmAddressIds = _finder.Find<Firm>(x => x.Id == firmId)
                .SelectMany(x => x.FirmAddresses)
                .OrderBy(x => x.Id)
                .Select(x => x.Id)
                .ToArray();

            foreach (var additionalServicesDto in additionalServices.Where(x => x.DisplayService != AdditionalServiceDisplay.DependsOnAddress))
            {
                var serviceCode = additionalServicesDto.ServiceCode;
                var additionalService = _finder.Find<AdditionalFirmService>(x => x.ServiceCode == serviceCode).Single();

                var firmAddressServices = _finder.Find<FirmAddressService>(x => firmAddressIds.Contains(x.FirmAddressId) && x.ServiceId == additionalService.Id).ToArray();
                var firmAddressServicesIds = firmAddressServices.Select(x => x.FirmAddressId).ToArray();

                foreach (var firmAddressId in firmAddressIds)
                {
                    FirmAddressService firmAddressService;

                    var index = Array.IndexOf(firmAddressServicesIds, firmAddressId);
                    if (index == -1)
                    {
                        if (additionalServicesDto.DisplayService == AdditionalServiceDisplay.Default)
                        {
                            continue;
                        }

                        firmAddressService = new FirmAddressService
                        {
                            FirmAddressId = firmAddressId,
                            ServiceId = additionalService.Id,
                            DisplayService = additionalServicesDto.DisplayService == AdditionalServiceDisplay.Display,
                        };

                        using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, FirmAddressService>())
                        {
                            _identityProvider.SetFor(firmAddressService);
                            _firmAddressServiceGenericRepository.Add(firmAddressService);
                            _firmAddressServiceGenericRepository.Save();
                            scope.Added<FirmAddressService>(firmAddressService.Id)
                                 .Complete();
                        }

                        continue;
                    }

                    firmAddressService = firmAddressServices[index];

                    // delete
                    if (additionalServicesDto.DisplayService == AdditionalServiceDisplay.Default)
                    {
                        using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, FirmAddressService>())
                        {
                            _firmAddressServiceGenericRepository.Delete(firmAddressService);
                            _firmAddressServiceGenericRepository.Save();
                            scope.Deleted<FirmAddressService>(firmAddressService.Id)
                                 .Complete();
                        }

                        continue;
                    }

                    // update
                    using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, FirmAddressService>())
                    {
                        firmAddressService.DisplayService = additionalServicesDto.DisplayService == AdditionalServiceDisplay.Display;
                        _firmAddressServiceGenericRepository.Update(firmAddressService);
                        _firmAddressServiceGenericRepository.Save();
                        scope.Updated<FirmAddressService>(firmAddressService.Id)
                             .Complete();
                    }
                }
            }
        }

        public IEnumerable<AdditionalServicesDto> GetFirmAddressAdditionalServices(long firmAddressId)
        {
            var additionalServices = _finder.Find<AdditionalFirmService>(x => x.IsManaged)
                .SelectMany(x => x.FirmAddressServices.Where(y => y.FirmAddressId == firmAddressId).DefaultIfEmpty(),
            (x, y) => new AdditionalServicesDto
            {
                ServiceCode = x.ServiceCode,
                Description = x.Description,
                DisplayService = (y != null) ? (y.DisplayService ? AdditionalServiceDisplay.Display : AdditionalServiceDisplay.DoNotDisplay) : AdditionalServiceDisplay.Default,
            })
                .ToArray();

            return additionalServices;
        }

        public void SetFirmAddressAdditionalServices(long firmAddressId, IEnumerable<AdditionalServicesDto> additionalServices)
        {
            var existingServices = _finder.Find<FirmAddress>(x => x.Id == firmAddressId).SelectMany(x => x.FirmAddressServices).ToArray();

            foreach (var additionalServicesDto in additionalServices)
            {
                var serviceCode = additionalServicesDto.ServiceCode;
                var additionalService = _finder.Find<AdditionalFirmService>(x => x.ServiceCode == serviceCode).Single();

                var firmAddressService = existingServices.FirstOrDefault(x => x.ServiceId == additionalService.Id);
                if (firmAddressService == null)
                {
                    if (additionalServicesDto.DisplayService == AdditionalServiceDisplay.Default)
                    {
                        continue;
                    }

                    firmAddressService = new FirmAddressService
                    {
                        FirmAddressId = firmAddressId,
                        ServiceId = additionalService.Id,
                        DisplayService = additionalServicesDto.DisplayService == AdditionalServiceDisplay.Display,
                    };

                    using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, FirmAddressService>())
                    {
                        _identityProvider.SetFor(firmAddressService);
                        _firmAddressServiceGenericRepository.Add(firmAddressService);
                        _firmAddressServiceGenericRepository.Save();
                        scope.Added<FirmAddressService>(firmAddressService.Id)
                             .Complete();
                    }

                    continue;
                }

                if (additionalServicesDto.DisplayService == AdditionalServiceDisplay.Default)
                {
                    using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, FirmAddressService>())
                    {
                        _firmAddressServiceGenericRepository.Delete(firmAddressService);
                        _firmAddressServiceGenericRepository.Save();
                        scope.Deleted<FirmAddressService>(firmAddressService.Id)
                             .Complete();
                    }

                    continue;
                }

                firmAddressService.DisplayService = additionalServicesDto.DisplayService == AdditionalServiceDisplay.Display;
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, FirmAddressService>())
                {
                    firmAddressService.DisplayService = additionalServicesDto.DisplayService == AdditionalServiceDisplay.Display;
                    _firmAddressServiceGenericRepository.Update(firmAddressService);
                    _firmAddressServiceGenericRepository.Save();
                    scope.Updated<FirmAddressService>(firmAddressService.Id)
                         .Complete();
                }
            }
        }

        public IEnumerable<FirmContact> GetContacts(long firmAddressId)
        {
            var depCardsQuery = _finder.Find<DepCard>(x => !x.IsHiddenOrArchived);

            var cardRelations = _finder.FindAll<CardRelation>()
                                       .Where(cardRelation => cardRelation.PosCardCode == firmAddressId && !cardRelation.IsDeleted)
                                       .OrderBy(cardRelation => cardRelation.OrderNo)
                                       .Join(depCardsQuery,
                                             cardRelation => cardRelation.DepCardCode,
                                             depCard => depCard.Id,
                                             (cardRelation, depCard) => depCard)
                                       .SelectMany(depCard => depCard.FirmContacts)
                                       .OrderBy(contact => contact.SortingPosition)
                                       .ToArray();

            var firmAddressContacts = _finder.FindAll<FirmContact>()
                                             .Where(contact => contact.FirmAddressId == firmAddressId)
                                             .OrderBy(contact => contact.SortingPosition)
                                             .ToArray();

            return firmAddressContacts.Union(cardRelations).ToArray();
        }

        public void ImportTerritoryFromServiceBus(IEnumerable<TerritoryServiceBusDto> territoryDtos)
        {
            var territoryServiceBusDtos = territoryDtos as TerritoryServiceBusDto[] ?? territoryDtos.ToArray();
            if (!territoryServiceBusDtos.Any())
            {
                return;
            }

            var context = GetOrganizationUnits();
            foreach (var territoryDto in territoryServiceBusDtos.Where(dto => !dto.IsDeleted))
            {
                ProcessActiveTerritory(territoryDto, context);
            }

            foreach (var territoryDto in territoryServiceBusDtos.Where(dto => dto.IsDeleted))
            {
                ProcessDeletedTerritory(territoryDto);
            }

            _territorySecureRepository.Save();
        }

        public void ImportBuildingFromServiceBus(IEnumerable<BuildingServiceBusDto> buildingDtos, string regionalTerritoryLocaleSpecificWord, bool enableReplication)
        {
            var filteredBuildingDtos = buildingDtos.Where(x => x.SaleTerritoryCode != null || x.IsDeleted);

            var buildingServiceBusDtos = filteredBuildingDtos as BuildingServiceBusDto[] ?? filteredBuildingDtos.ToArray();
            if (!buildingServiceBusDtos.Any())
            {
                return;
            }

            // Обработка активных зданий
            var activeBuildingDtos = buildingServiceBusDtos.Where(dto => !dto.IsDeleted).ToArray();
            if (activeBuildingDtos.Any())
            {
                var xml = SerializeBuildingDtos(activeBuildingDtos, "buildings", "building");
            _firmPersistanceService.UpdateBuildings(xml, ImportCommandTimeout, regionalTerritoryLocaleSpecificWord, enableReplication);
        }

            // Обработка удалённых зданий
            var deletedBuildingCodes = buildingServiceBusDtos.Where(dto => dto.IsDeleted).Select(dto => dto.Code).ToArray();
            if (deletedBuildingCodes.Any())
            {
                var xml = SerializeDeletedBuildingCodes(deletedBuildingCodes);
                _firmPersistanceService.DeleteBuildings(xml, ImportCommandTimeout);
            }
        }

        private static string SerializeDeletedBuildingCodes(IEnumerable<long> codes)
        {
            var root = new XElement("root");
            foreach (var code in codes)
            {
                root.Add(new XElement("code", code));
            }

            return root.ToString();
        }

        public void ImportCityPhoneZonesFromServiceBus(IEnumerable<CityPhoneZone> cityPhoneZones)
        {
            foreach (var cityPhoneZone in cityPhoneZones)
            {
                var cityPhoneZoneCode = cityPhoneZone.Id;

                var cityPhoneZoneExists = _finder.Find<CityPhoneZone>(x => x.Id == cityPhoneZoneCode).Any();
                if (cityPhoneZoneExists)
                {
                    return;
                }

                _cityPhoneZoneGenericRepository.Add(cityPhoneZone);
            }
            
            _cityPhoneZoneGenericRepository.Save();
        }

        int IQualifyAggregateRepository<Firm>.Qualify(long entityId, long currentUserCode, long reserveCode, long ownerCode, DateTime qualifyDate)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<Firm>(entityId)).Single();
            return Qualify(entity, currentUserCode, reserveCode, ownerCode, qualifyDate);
        }

        int IDisqualifyAggregateRepository<Firm>.Disqualify(long entityId, long currentUserCode, long reserveCode, bool bypassValidation, DateTime disqualifyDate)
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
                    .Find<Firm, int>(
                        FirmSpecs.Firms.Select.FirmCountForFirmClient(), 
                        Specs.Find.ById<Firm>(entityId) && FirmSpecs.Firms.Find.HasClient())
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
            if (!_entityAccessService.HasEntityAccess(EntityAccessTypes.Update, EntityName.Firm, currentUserCode, firmInfo.Id, firmInfo.OwnerCode, null))
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

        public void ImportFirmContacts(long firmAddressId, IEnumerable<ImportFirmContactDto> firmContacts, bool isDepCard)
        {
            var dtoContacts = firmContacts.ToDictionary(x => x.SortingPosition);
            var existingContacts = isDepCard
                                       ? _finder.Find<FirmContact>(x => x.CardId == firmAddressId).ToDictionary(x => x.SortingPosition)
                                       : _finder.Find<FirmContact>(x => x.FirmAddressId == firmAddressId).ToDictionary(x => x.SortingPosition);

            // TODO {a.tukaev, 26.11.2013}: Не забывай про StyleCop
            // DONE {d.ivanov, 04.12.2013}: Ок
            var sortingPositionsToUpdate = dtoContacts.Keys.Intersect(existingContacts.Keys).ToArray();
            var sortingPositionsToInsert = dtoContacts.Keys.Except(existingContacts.Keys).ToArray();
            var sortingPositionsToDelete = existingContacts.Keys.Except(dtoContacts.Keys).ToArray();

            if (sortingPositionsToUpdate.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, FirmContact>())
                {
                    foreach (var sortingPosition in sortingPositionsToUpdate)
                    {
                        var dtoContact = dtoContacts[sortingPosition];
                        var existingContact = existingContacts[sortingPosition];

                        existingContact.ContactType = dtoContact.ContactType;
                        existingContact.Contact = dtoContact.Contact;

                        _firmContactGenericRepository.Update(existingContact);
                        scope.Updated<FirmContact>(existingContact.Id);
                    }

                    scope.Complete();
                }
            }

            if (sortingPositionsToInsert.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, FirmContact>())
                {
                    foreach (var sortingPosition in sortingPositionsToInsert)
                    {
                        var dtoContact = dtoContacts[sortingPosition];
                        var contact = new FirmContact
                        {
                            ContactType = dtoContact.ContactType,
                            Contact = dtoContact.Contact,
                            SortingPosition = dtoContact.SortingPosition
                        };

                        if (isDepCard)
                        {
                            contact.CardId = firmAddressId;
                        }
                        else
                        {
                            contact.FirmAddressId = firmAddressId;
                        }

                        _identityProvider.SetFor(contact);
                        _firmContactGenericRepository.Add(contact);
                        scope.Added<FirmContact>(contact.Id);
                    }

                    scope.Complete();
                }
            }

            if (sortingPositionsToDelete.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, FirmContact>())
                {
                    foreach (var sortingPosition in sortingPositionsToDelete)
                    {
                        var existingContact = existingContacts[sortingPosition];
                        _firmContactGenericRepository.Delete(existingContact);
                        scope.Deleted<FirmContact>(existingContact.Id);
                    }

                    scope.Complete();
                }
            }

            _firmContactGenericRepository.Save();
        }

        public void ImportCategoryFirmAddresses(long firmAddressId, IEnumerable<ImportCategoryFirmAddressDto> categoryFirmAddresses)
        {
            var dtoCategories = categoryFirmAddresses.ToDictionary(x => (long)x.Code);
            var existingCategories = _finder.Find<CategoryFirmAddress>(x => x.FirmAddressId == firmAddressId).ToDictionary(x => x.CategoryId);

            // COMMENT {a.tukaev, 26.11.2013}: cids - имелось ввиду categoryIds? Может так и написать? 
            // DONE {d.ivanov, 04.12.2013}: Сделано
            var categoryIdsToUpdate = dtoCategories.Keys.Intersect(existingCategories.Keys).ToArray();
            var categoryIdsToInsert = dtoCategories.Keys.Except(existingCategories.Keys).ToArray();
            var categoryIdsToDelete = existingCategories.Keys.Except(dtoCategories.Keys).ToArray();

            if (categoryIdsToUpdate.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, CategoryFirmAddress>())
                {
                    foreach (var categoryId in categoryIdsToUpdate)
                    {
                        var dtoCategory = dtoCategories[categoryId];
                        var existingCategory = existingCategories[categoryId];

                        existingCategory.IsPrimary = dtoCategory.IsPrimary;
                        existingCategory.SortingPosition = dtoCategory.SortingPosition;

                        existingCategory.IsActive = true;
                        existingCategory.IsDeleted = false;

                        _categoryFirmAddressGenericRepository.Update(existingCategory);
                        scope.Updated<CategoryFirmAddress>(existingCategory.Id);
                    }

                    scope.Complete();
                }
            }

            if (categoryIdsToInsert.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, CategoryFirmAddress>())
                {
                    foreach (var categoryId in categoryIdsToInsert)
                    {
                        var dtoCategory = dtoCategories[categoryId];
                        var category = new CategoryFirmAddress
                            {
                                CategoryId = dtoCategory.Code,
                                FirmAddressId = firmAddressId,
                                IsActive = true,
                                IsPrimary = dtoCategory.IsPrimary,
                                SortingPosition = dtoCategory.SortingPosition
                            };

                        _identityProvider.SetFor(category);
                        _categoryFirmAddressGenericRepository.Add(category);
                        scope.Added<CategoryFirmAddress>(category.Id);
                    }

                    scope.Complete();
                }
            }

            if (categoryIdsToDelete.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, CategoryFirmAddress>())
                {
                    foreach (var categoryId in categoryIdsToDelete)
                    {
                        var existingCategory = existingCategories[categoryId];
                        _categoryFirmAddressGenericRepository.Delete(existingCategory);
                        scope.Deleted<CategoryFirmAddress>(existingCategory.Id);
                    }

                    scope.Complete();
                }
            }

            _categoryFirmAddressGenericRepository.Save();
        }

        public void ImportFirmAddresses(IEnumerable<DTO.CardForErm.ImportFirmAddressDto> firmAddresses, string regionalTerritoryName)
        {
            var dtoAddresses = firmAddresses.ToDictionary(x => x.Code);

            var ids = dtoAddresses.Keys;
            var existingAddresses = _finder.Find<FirmAddress>(x => ids.Contains(x.Id)).ToDictionary(x => x.Id);

            var idsToUpdate = dtoAddresses.Keys.Intersect(existingAddresses.Keys).ToArray();
            var idsToInsert = dtoAddresses.Keys.Except(existingAddresses.Keys).ToArray();

            // Создаем пустые фирмы, если нужно
            CreateBlankFirms(dtoAddresses.Values, regionalTerritoryName);

            if (idsToUpdate.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, FirmAddress>())
                {
                    foreach (var id in idsToUpdate)
                    {
                        var dtoAddress = dtoAddresses[id];
                        var existingAddress = existingAddresses[id];

                        existingAddress.FirmId = dtoAddress.FirmCode;
                        existingAddress.TerritoryId = dtoAddress.TerritoryCode;
                        existingAddress.IsActive = dtoAddress.IsActive;
                        existingAddress.ClosedForAscertainment = dtoAddress.ClosedForAscertainment;
                        existingAddress.IsLocatedOnTheMap = dtoAddress.IsLinked;
                        existingAddress.IsDeleted = dtoAddress.IsDeleted;
                        existingAddress.Address = dtoAddress.Address;
                        existingAddress.ReferencePoint = null;
                        existingAddress.WorkingTime = dtoAddress.Schedule;
                        existingAddress.PaymentMethods = dtoAddress.Payment;

                        _firmAddressGenericRepository.Update(existingAddress);
                        scope.Updated<FirmAddress>(existingAddress.Id);
                    }

                    scope.Complete();
                }
            }

            if (idsToInsert.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, FirmAddress>())
                {
                    foreach (var id in idsToInsert)
                    {
                        var dtoAddress = dtoAddresses[id];
                        var address = new FirmAddress
                            {
                                Id = dtoAddress.Code,
                                FirmId = dtoAddress.FirmCode,
                                Address = dtoAddress.Address,
                                WorkingTime = dtoAddress.Schedule,
                                PaymentMethods = dtoAddress.Payment,
                                IsLocatedOnTheMap = dtoAddress.IsLinked,
                                TerritoryId = dtoAddress.TerritoryCode,
                                ClosedForAscertainment = dtoAddress.ClosedForAscertainment,
                                IsActive = dtoAddress.IsActive,
                                IsDeleted = dtoAddress.IsDeleted
                            };

                        _firmAddressGenericRepository.Add(address);
                        scope.Added<FirmAddress>(address.Id);
                    }

                    scope.Complete();
                }
            }

            _firmAddressGenericRepository.Save();
        }

        public IEnumerable<CategoryGroup> GetFirmAddressCategoryGroups(long firmAddressId)
        {
            var organizationUnitId = _finder.Find(Specs.Find.ById<FirmAddress>(firmAddressId))
                                            .Select(address => address.Firm.Territory.OrganizationUnitId)
                                            .SingleOrDefault();

            var categoryIds = _finder.Find(Specs.Find.ById<FirmAddress>(firmAddressId))
                                     .SelectMany(address => address.Firm.FirmAddresses)
                                     .Where(Specs.Find.ActiveAndNotDeleted<FirmAddress>())
                                     .SelectMany(address => address.CategoryFirmAddresses)
                                     .Where(Specs.Find.ActiveAndNotDeleted<CategoryFirmAddress>())
                                     .Select(categoryFirmAddress => categoryFirmAddress.CategoryId)
                                     .Distinct()
                                     .ToArray();

            var groups = _finder.Find(Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>())
                                .Where(link => link.OrganizationUnitId == organizationUnitId && categoryIds.Contains(link.CategoryId))
                                .Select(link => link.CategoryGroup)
                                .Distinct()
                                .ToArray();

            return groups;
        }

        public IEnumerable<string> GetAddressesNames(IEnumerable<long> firmAddressIds)
        {
            return _finder.Find(Specs.Find.ByIds<FirmAddress>(firmAddressIds))
                          .Select(x => x.Address)
                          .ToArray();
        }

        public long GetFirmAddressOrganizationUnitId(long firmAddressId)
        {
            var organizationUnitId = _finder.Find(Specs.Find.ById<FirmAddress>(firmAddressId))
                                            .Select(address => address.Firm.Territory.OrganizationUnitId)
                                            .Single();

            return organizationUnitId;
        }

        public IEnumerable<long> GetProjectOrganizationUnitIds(long projectCode)
        {
            var organizationUnitIds = _finder.Find<Project>(project => project.Code == projectCode && project.OrganizationUnitId.HasValue)
                                            .Select(project => project.OrganizationUnitId.Value)
                                            .ToArray();
            return organizationUnitIds;
        }

        public void ImportDepCards(IEnumerable<ImportDepCardDto> importDepCardDtos)
        {
            var dtoCards = importDepCardDtos.ToDictionary(x => x.Code);

            var ids = dtoCards.Keys;
            var existingCards = _finder.Find<DepCard>(x => ids.Contains(x.Id)).ToDictionary(x => x.Id);

            var idsToUpdate = dtoCards.Keys.Intersect(existingCards.Keys).ToArray();
            var idsToInsert = dtoCards.Keys.Except(existingCards.Keys).ToArray();

            if (idsToUpdate.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, DepCard>())
                {
                    foreach (var id in idsToUpdate)
                    {
                        var existingCard = existingCards[id];
                        existingCard.IsHiddenOrArchived = dtoCards[id].IsHiddenOrArchived;
                        _depCardRepository.Update(existingCard);
                        scope.Updated<DepCard>(id);
                    }

                    scope.Complete();
                }
            }

            if (idsToInsert.Any())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, DepCard>())
                {
                    foreach (var id in idsToInsert)
                    {
                        var newCard = new DepCard { Id = id, IsHiddenOrArchived = dtoCards[id].IsHiddenOrArchived };
                        _depCardRepository.Add(newCard);
                        scope.Added<DepCard>(id);
                    }

                    scope.Complete();
                }
            }

            _depCardRepository.Save();
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

        private static string SerializeBuildingDtos(IEnumerable<BuildingServiceBusDto> items, string root, string elementName)
        {
            var stringBuilder = new StringBuilder();

            using (var writer = XmlWriter.Create(stringBuilder, new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                writer.WriteStartElement(root);

                foreach (var item in items)
                {
                    writer.WriteStartElement(elementName);

                    var attributes =
                        new[]
                            {
                                new KeyValuePair<string, object>("Code", item.Code), 
                                new KeyValuePair<string, object>("SaleTerritoryCode", item.SaleTerritoryCode), 
                                new KeyValuePair<string, object>("IsDeleted", item.IsDeleted)
                            }
                        .Where(x => x.Value != null);
                    foreach (var attribute in attributes)
                    {
                        writer.WriteStartAttribute(attribute.Key);
                        writer.WriteValue(attribute.Value);
                        writer.WriteEndAttribute();
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            return stringBuilder.ToString();
        }

        private void CreateBlankFirms(IEnumerable<DTO.CardForErm.ImportFirmAddressDto> addresses, string regionalTerritoryName)
        {
            var addressesByFirmCode = addresses.ToLookup(x => x.FirmCode);
            var firmIds = addressesByFirmCode.Select(x => x.Key);
            var existingFirmIds = _finder.Find<Firm>(x => firmIds.Contains(x.Id)).Select(x => x.Id).ToArray();

            var firmIdsToInsert = firmIds.Except(existingFirmIds).ToArray();
            if (!firmIdsToInsert.Any())
            {
                return;
            }

            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, Firm>())
            {
                foreach (var firmId in firmIdsToInsert)
                {
                    var organizationUnitDgppId = addressesByFirmCode[firmId].Select(x => x.BranchCode).First();

                    var territory = _finder.Find<OrganizationUnit>(x => x.IsActive && !x.IsDeleted && x.DgppId == organizationUnitDgppId)
                                           .SelectMany(x => x.Territories.Where(t => t.IsActive && t.Name.Contains(regionalTerritoryName)))
                                           .Select(x => new { x.Id, x.OrganizationUnitId })
                                           .SingleOrDefault();

                    if (territory == null)
                    {
                        throw new NotificationException(string.Format("Cannot find either organization unit with DgppId = {0} or its regional territory", organizationUnitDgppId));
                    }

                    var firm = new Firm
                        {
                            Id = firmId,
                            Name = string.Format("Пустая фирма #{0}", firmId),
                            ReplicationCode = Guid.NewGuid(),
                            TerritoryId = territory.Id,
                            ClosedForAscertainment = true,
                            OwnerCode = _securityServiceUserIdentifier.GetReserveUserIdentity().Code,
                            IsActive = false,
                            OrganizationUnitId = territory.OrganizationUnitId,
                        };

                    _firmGenericRepository.Add(firm);
                    scope.Added<Firm>(firm.Id);
                }

                scope.Complete();
            }

            _firmGenericRepository.Save();
        }

        private void ProcessReferenceDto(ReferenceServiceBusDto referenceDto)
        {
            var referenceExists = _finder.Find<Reference>(x => x.CodeName == referenceDto.Code).Any();
            if (referenceExists)
            {
                return;
            }

            var reference = new Reference { CodeName = referenceDto.Code };
            _identityProvider.SetFor(reference);
            _refrenceGenericRepository.Add(reference);
        }

        private void ProcessReferenceItemDto(ReferenceItemServiceBusDto referenceItemDto)
        {
            var reference = _finder.Find<Reference>(x => x.CodeName == referenceItemDto.ReferenceCode).SingleOrDefault();
            if (reference == null)
            {
                throw new ArgumentException(string.Format(BLResources.ReferenceWithIdNotFound, referenceItemDto.ReferenceCode));
            }

            var referenceItem = _finder.Find<ReferenceItem>(x => x.Code == referenceItemDto.Code && x.ReferenceId == reference.Id).SingleOrDefault() ??
                                new ReferenceItem { Code = referenceItemDto.Code, ReferenceId = reference.Id };

            referenceItem.Name = referenceItemDto.Name;
            referenceItem.IsDeleted = referenceItemDto.IsDeleted;

            if (referenceItem.IsNew())
            {
                _identityProvider.SetFor(referenceItem);
                _refrenceItemGenericRepository.Add(referenceItem);
            }
            else
            {
                _refrenceItemGenericRepository.Update(referenceItem);
            }
        }

        private void ProcessCardRelationDto(CardRelationServiceBusDto cardRelationDto)
        {
            // create or update card relation
            var objectExists = true;
            var cardRelation = _finder.Find<CardRelation>(x => x.Id == cardRelationDto.Code).SingleOrDefault();
            if (cardRelation == null)
            {
                cardRelation = new CardRelation { Id = cardRelationDto.Code };
                objectExists = false;
            }

            cardRelation.DepCardCode = cardRelationDto.DepartmentCardCode;
            cardRelation.PosCardCode = cardRelationDto.PointOfServiceCardCode;
            cardRelation.OrderNo = cardRelationDto.DepartmentCardSortingPosition;
            cardRelation.IsDeleted = cardRelationDto.IsDeleted;

            if (objectExists)
            {
                _cardRelationGenericRepository.Update(cardRelation);
            }
            else
            {
                _cardRelationGenericRepository.Add(cardRelation);
            }
        }

        private void ProcessActiveTerritory(TerritoryServiceBusDto territoryDto, IDictionary<int, long> dgppToErmIds)
        {
            var territory = _finder.Find<Territory>(t => t.Id == territoryDto.Code).SingleOrDefault() ??
                new Territory { Id = territoryDto.Code };

            long organizationUnitId;
            if (!dgppToErmIds.TryGetValue(territoryDto.OrganizationUnitDgppId, out organizationUnitId))
            {
                throw new ArgumentException(string.Format("Импорт. Не найдено отделние организации с кодом '{0}'", territoryDto.OrganizationUnitDgppId));
            }

            territory.Name = territoryDto.Name;
            territory.IsActive = !territoryDto.IsDeleted;
            territory.OrganizationUnitId = organizationUnitId;

            if (territory.IsNew())
            {
                _territorySecureRepository.Add(territory);
            }
            else
            {
                _territorySecureRepository.Update(territory);
            }
        }

        // FIXME {all, 16.12.2013}: А как-же пользователи, фирмы и прочее, привязанное к этой территории? Нельзя просто так взять и деактивировать территорию. 
        //                          Мы так поступаем уже давно и решили не менять логику в рамках задачи по изменению схемы. Тем не менее когда-то это нужно сделать.
        private void ProcessDeletedTerritory(TerritoryServiceBusDto territoryDto)
        {
            var territory = _finder.Find<Territory>(t => t.Id == territoryDto.Code).SingleOrDefault();
            if (territory == null)
            {
                return;
            }

            territory.IsActive = false;
            _territorySecureRepository.Update(territory);
        }
    }
}
