﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
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
        private const int ImportFirmPrimisingCommandTimeout = 3600;

        private readonly IRepository<CategoryFirmAddress> _categoryFirmAddressGenericRepository;
        private readonly IRepository<Client> _clientGenericRepository;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IFinder _finder;
        private readonly IRepository<FirmAddress> _firmAddressGenericRepository;
        private readonly IRepository<FirmAddressService> _firmAddressServiceGenericRepository;
        private readonly IRepository<FirmContact> _firmContactGenericRepository;
        private readonly IRepository<Firm> _firmGenericRepository;
        private readonly ISecureRepository<Firm> _firmGenericSecureRepository;
        private readonly IFirmPersistenceService _firmPersistanceService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecureFinder _secureFinder;
        private readonly IRepository<Territory> _territoryGenericRepository;

        public FirmRepository(IFinder finder,
                              IRepository<Client> clientGenericRepository,
                              IRepository<Firm> firmGenericRepository,
                              IRepository<FirmAddress> firmAddressGenericRepository,
                              IRepository<FirmContact> firmContactGenericRepository,
                              IRepository<CategoryFirmAddress> categoryFirmAddressGenericRepository,
                              IRepository<FirmAddressService> firmAddressServiceGenericRepository,
                              IRepository<Territory> territoryGenericRepository,
                              ISecureRepository<Firm> firmGenericSecureRepository,
                              ISecurityServiceEntityAccess entityAccessService,
                              ISecurityServiceFunctionalAccess functionalAccessService,
                              ISecureFinder secureFinder,
                              IFirmPersistenceService firmPersistanceService,
                              IIdentityProvider identityProvider,
                              IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _clientGenericRepository = clientGenericRepository;
            _firmGenericRepository = firmGenericRepository;
            _firmAddressGenericRepository = firmAddressGenericRepository;
            _firmContactGenericRepository = firmContactGenericRepository;
            _categoryFirmAddressGenericRepository = categoryFirmAddressGenericRepository;
            _firmAddressServiceGenericRepository = firmAddressServiceGenericRepository;
            _firmGenericSecureRepository = firmGenericSecureRepository;
            _entityAccessService = entityAccessService;
            _functionalAccessService = functionalAccessService;
            _secureFinder = secureFinder;
            _firmPersistanceService = firmPersistanceService;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
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
            var clientId = _secureFinder.Find(Specs.Find.ById<Firm>(firmId))
                                      .Select(x => x.ClientId)
                                      .SingleOrDefault();

            if (clientId == null)
            {
                throw new ArgumentException(BLResources.DisqualifyCantFindFirmClient);
            }

            var client = _secureFinder.FindOne(Specs.Find.ById<Client>(clientId.Value));

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
            return _finder.FindOne(ClientSpecs.Clients.Find.ByFirm(firmId));
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
                       ?? new Firm { Id = dto.DgppId, OwnerCode = context.ReserveUserId };

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
                var addressIdsToDelete = _finder.Find<Firm>(f => f.Id == dto.DgppId)
                                               .SelectMany(f => f.FirmAddresses)
                                               .Where(f => !f.IsDeleted)
                                               .Where(f => !addressDgppIds.Contains(f.Id))
                                               .Select(
                                                       f =>
                                                       new
                                                           {
                                                               AddressId = f.Id,
                                                               CategoryFirmAddresses = f.CategoryFirmAddresses.Where(category => !category.IsDeleted)
                                                           })
                                               .ToArray();

                var addressesToDelete = _finder.FindMany(Specs.Find.ByIds<FirmAddress>(addressIdsToDelete.Select(x => x.AddressId)));

                foreach (var addressToDelete in addressIdsToDelete)
                {
                    foreach (var categoryFirmAddress in addressToDelete.CategoryFirmAddresses)
                    {
                        _categoryFirmAddressGenericRepository.Delete(categoryFirmAddress);
                    }
                }

                foreach (var address in addressesToDelete)
                {
                    _firmAddressGenericRepository.Delete(address);
                }

                _categoryFirmAddressGenericRepository.Save();
                _firmAddressGenericRepository.Save();

                transaction.Complete();

                return result;
            }
        }

        public void ImportAddressContacts(FirmAddress firmAddress, ImportFirmAddressDto dto)
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

        public void ImportAddressCategories(FirmAddress firmAddress, ImportFirmAddressDto dto, FirmImportContext context)
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
            var advertisementIds =
                _finder.Find<Firm>(x => x.Id == firmId).SelectMany(x => x.Advertisements).Where(x => !x.IsDeleted).Select(x => x.Id).ToArray();
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
                                                         DisplayService =
                                                               (y != null)
                                                                   ? (y.DisplayService
                                                                          ? AdditionalServiceDisplay.Display
                                                                          : AdditionalServiceDisplay.DoNotDisplay)
                                                                   : AdditionalServiceDisplay.Default,
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

                var firmAddressServices =
                    _finder.Find<FirmAddressService>(x => firmAddressIds.Contains(x.FirmAddressId) && x.ServiceId == additionalService.Id).ToArray();
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
                                                                DisplayService =
                                                                    (y != null)
                                                                        ? (y.DisplayService
                                                                               ? AdditionalServiceDisplay.Display
                                                                               : AdditionalServiceDisplay.DoNotDisplay)
                                                                        : AdditionalServiceDisplay.Default,
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

        // TODO {a.rechkalov, 19.03.2014}: Перенести в Read-model
        // DONE {all, 13.05.2014}:done

        // TODO {a.rechkalov, 19.03.2014}: Перенести в Read-model
        // DONE {all, 13.05.2014}:done

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