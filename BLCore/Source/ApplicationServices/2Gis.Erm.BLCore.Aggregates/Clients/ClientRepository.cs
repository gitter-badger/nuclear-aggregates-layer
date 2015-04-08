using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Client;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients
{
    public class ClientRepository : IClientRepository
    {
        private static readonly DateTime MinMssqlDatetime = new DateTime(1753, 1, 1);
        private static readonly DateTime MaxMssqlDatetime = new DateTime(9999, 12, 31, 23, 59, 59, 997);

        private readonly IDebtProcessingSettings _debtProcessingSettings;
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly ISecureRepository<Client> _clientGenericSecureRepository;
        private readonly ISecureRepository<Contact> _contactGenericSecureRepository;
        private readonly IRepository<Firm> _firmGenericRepository;
        private readonly IRepository<LegalPerson> _legalPersonGenericRepository;
        private readonly IRepository<LegalPersonProfile> _legalPersonProfileGenericRepository;
        private readonly IRepository<Account> _accountGenericRepository;
        private readonly IRepository<Limit> _limitGenericRepository;
        private readonly IRepository<Bargain> _bargainGenericRepository;
        private readonly IRepository<Deal> _dealGenericRepository;
        private readonly IRepository<Order> _orderGenericRepository;
        private readonly IRepository<OrderPosition> _orderPositionGenericRepository;
        private readonly IRepository<Contact> _contactGenericRepository;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IClientPersistenceService _clientPersistenceService;
        private readonly ISecureFinder _secureFinder;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IOperationScopeFactory _scopeFactory;

        public ClientRepository(
            IDebtProcessingSettings debtProcessingSettings,
            IFinder finder,
            ISecureRepository<Client> clientGenericSecureRepository,
            IRepository<Firm> firmGenericRepository,
            IRepository<LegalPerson> legalPersonGenericRepository,
            IRepository<LegalPersonProfile> legalPersonProfileGenericRepository,
            IRepository<Account> accountGenericRepository,
            IRepository<Limit> limitGenericRepository,
            IRepository<Bargain> bargainGenericRepository,
            IRepository<Deal> dealGenericRepository,
            IRepository<Order> orderGenericRepository,
            IRepository<OrderPosition> orderPositionGenericRepository,
            IRepository<Contact> contactGenericRepository,
            ISecureRepository<Contact> contactGenericSecureRepository,
            ISecurityServiceFunctionalAccess functionalAccessService, 
            ISecurityServiceUserIdentifier userIdentifierService, 
            IUserContext userContext, 
            ISecureFinder secureFinder, 
            IClientPersistenceService clientPersistenceService, 
            IIdentityProvider identityProvider, 
            IOperationScopeFactory operationScopeFactory, 
            IOperationScopeFactory scopeFactory)
        {
            _debtProcessingSettings = debtProcessingSettings;
            _finder = finder;
            _clientGenericSecureRepository = clientGenericSecureRepository;
            _firmGenericRepository = firmGenericRepository;
            _legalPersonGenericRepository = legalPersonGenericRepository;
            _accountGenericRepository = accountGenericRepository;
            _limitGenericRepository = limitGenericRepository;
            _bargainGenericRepository = bargainGenericRepository;
            _dealGenericRepository = dealGenericRepository;
            _orderGenericRepository = orderGenericRepository;
            _orderPositionGenericRepository = orderPositionGenericRepository;
            _contactGenericRepository = contactGenericRepository;
            _functionalAccessService = functionalAccessService;
            _userIdentifierService = userIdentifierService;
            _userContext = userContext;
            _secureFinder = secureFinder;
            _clientPersistenceService = clientPersistenceService;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
            _scopeFactory = scopeFactory;
            _legalPersonProfileGenericRepository = legalPersonProfileGenericRepository;
            _contactGenericSecureRepository = contactGenericSecureRepository;
        }

        public int SetMainFirm(Client client, long? mainFirmId)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<SetMainFirmIdentity>())
            {
                client.MainFirmId = mainFirmId;
                _clientGenericSecureRepository.Update(client);
                var cnt = _clientGenericSecureRepository.Save();

                scope.Updated(client)
                     .Complete();

                return cnt;
            }
        }

        public int Assign(Client client, long ownerCode)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<AssignIdentity, Client>())
            {
                client.OwnerCode = ownerCode;
                _clientGenericSecureRepository.Update(client);
                var cnt = _clientGenericSecureRepository.Save();

                scope.Updated(client)
                     .Complete();

                return cnt;
            }
        }

        public int AssignWithRelatedEntities(long clientId, long ownerCode, bool isPartialAssign)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, Client>())
            {
                var relatedEntities = (from client in _finder.Find(Specs.Find.ById<Client>(clientId))
                                       let clientPrevOwner = isPartialAssign ? client.OwnerCode : (long?)null
                                       select new
                                       {
                                           Firms = client.Firms.Where(x => clientPrevOwner == null || x.OwnerCode == clientPrevOwner),
                                           Deals = client.Deals.Where(x => !x.IsDeleted && x.IsActive && (clientPrevOwner == null || x.OwnerCode == clientPrevOwner)),
                                           Contacts = client.Contacts.Where(x => !x.IsDeleted && x.IsActive && (clientPrevOwner == null || x.OwnerCode == clientPrevOwner)),
                                           LegalPersonsWithRelated = from legalPerson in client.LegalPersons
                                                                     where clientPrevOwner == null || legalPerson.OwnerCode == clientPrevOwner
                                                                     select new
                                                                     {
                                                                         LegalPersonId = legalPerson.Id,
                                                                         Accounts = legalPerson.Accounts.Where(x => (clientPrevOwner == null || x.OwnerCode == clientPrevOwner)),
                                                                         Bargains = legalPerson.Bargains.Where(x => x.IsActive && (clientPrevOwner == null || x.OwnerCode == clientPrevOwner)),
                                                                         ProfileIds = legalPerson.LegalPersonProfiles.Where(x => x.IsActive && (clientPrevOwner == null || x.OwnerCode == clientPrevOwner)).Select(x => x.Id),
                                                                         Limits = from acc in legalPerson.Accounts
                                                                                  from limit in acc.Limits
                                                                                  where limit.IsActive && !limit.IsDeleted && (clientPrevOwner == null || limit.OwnerCode == clientPrevOwner)
                                                                                  select limit
                                                                     },
                                           OrdersWithPositions = from firm in client.Firms
                                                                 from order in firm.Orders
                                                                 where order.IsActive && !order.IsDeleted &&
                                                                       order.WorkflowStepId != OrderState.Archive && (clientPrevOwner == null || order.OwnerCode == clientPrevOwner)
                                                                 select new
                                                                 {
                                                                     Order = order,
                                                                     order.OrderPositions
                                                                 },
                                       })
                                      .Single();

                var clientToAssign = _finder.FindOne(Specs.Find.ById<Client>(clientId));
                clientToAssign.OwnerCode = ownerCode;
                _clientGenericSecureRepository.Update(clientToAssign);
                operationScope.Updated<Client>(clientToAssign.Id);

                var count = _clientGenericSecureRepository.Save();

                foreach (var firm in relatedEntities.Firms)
                {
                    firm.OwnerCode = ownerCode;
                    _firmGenericRepository.Update(firm);
                    operationScope.Updated<Firm>(firm.Id);
                }

                _firmGenericRepository.Save();

                var legalPersons = _finder.FindMany(Specs.Find.ByIds<LegalPerson>(relatedEntities.LegalPersonsWithRelated.Select(x => x.LegalPersonId)));
                foreach (var legalPerson in legalPersons)
                {
                    legalPerson.OwnerCode = ownerCode;
                    _legalPersonGenericRepository.Update(legalPerson);
                    operationScope.Updated<LegalPerson>(legalPerson.Id);
                }

                var legalPersonProfiles = _finder.FindMany(Specs.Find.ByIds<LegalPersonProfile>(relatedEntities.LegalPersonsWithRelated.SelectMany(x => x.ProfileIds)));
                foreach (var profile in legalPersonProfiles)
                {
                    profile.OwnerCode = ownerCode;
                    _legalPersonProfileGenericRepository.Update(profile);
                    operationScope.Updated<LegalPersonProfile>(profile.Id);
                }

                foreach (var legalPersonsWithRelated in relatedEntities.LegalPersonsWithRelated)
                {
                    foreach (var account in legalPersonsWithRelated.Accounts)
                    {
                        account.OwnerCode = ownerCode;
                        _accountGenericRepository.Update(account);
                        operationScope.Updated<Account>(account.Id);
                    }

                    foreach (var limit in legalPersonsWithRelated.Limits)
                    {
                        limit.OwnerCode = ownerCode;
                        _limitGenericRepository.Update(limit);
                        operationScope.Updated<Limit>(limit.Id);
                    }

                    foreach (var bargain in legalPersonsWithRelated.Bargains)
                    {
                        bargain.OwnerCode = ownerCode;
                        _bargainGenericRepository.Update(bargain);
                        operationScope.Updated<Bargain>(bargain.Id);
                    }
                }

                _legalPersonGenericRepository.Save();
                _legalPersonProfileGenericRepository.Save();
                _accountGenericRepository.Save();
                _limitGenericRepository.Save();
                _bargainGenericRepository.Save();

                foreach (var deal in relatedEntities.Deals)
                {
                    deal.OwnerCode = ownerCode;
                    _dealGenericRepository.Update(deal);
                    operationScope.Updated<Deal>(deal.Id);
                }

                _dealGenericRepository.Save();

                foreach (var orderWithPositions in relatedEntities.OrdersWithPositions)
                {
                    var order = orderWithPositions.Order;

                    order.OwnerCode = ownerCode;
                    _orderGenericRepository.Update(order);
                    operationScope.Updated<Order>(order.Id);
                    
                    foreach (var orderPosition in orderWithPositions.OrderPositions)
                    {
                        orderPosition.OwnerCode = ownerCode;
                        _orderPositionGenericRepository.Update(orderPosition);
                        operationScope.Updated<OrderPosition>(orderPosition.Id);
                    }
                }

                _orderGenericRepository.Save();
                _orderPositionGenericRepository.Save();

                foreach (var contact in relatedEntities.Contacts)
                {
                    contact.OwnerCode = ownerCode;
                    _contactGenericRepository.Update(contact);
                    operationScope.Updated<Contact>(contact.Id);
                }

                _contactGenericRepository.Save();

                operationScope.Complete();

                return count;
            }
        }

        public int Qualify(Client client, long currentUserCode, long reserveCode, long ownerCode, DateTime qualifyDate)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<QualifyIdentity, Client>())
            {
                if (!client.IsActive)
                {
                    throw new ArgumentException(BLResources.QualifyClientNotActive);
                }

                if (client.OwnerCode != reserveCode)
                {
                    throw new ArgumentException(BLResources.QualifyClientMustInReserve);
                }

                var reserveRights = GetMaxAccessForReserve(_functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.ReserveAccess, currentUserCode));
                switch (reserveRights)
                {
                    case ReserveAccess.None:
                        throw new ArgumentException(BLResources.QualifyReserveOperationDenied);

                    case ReserveAccess.Territory:
                    {
                        var withinTerritories = _finder.Find<UserTerritoriesOrganizationUnits>(x => x.UserId == currentUserCode &&
                                                                                                    x.TerritoryId == client.TerritoryId)
                                                       .Any();
                        if (!withinTerritories)
                        {
                            throw new SecurityException(BLResources.QualifyCouldntAccessClientOnThisTerritory);
                        }
                    }

                        break;
                    case ReserveAccess.OrganizationUnit:
                    {
                        var clientOrganizationUnit = _finder.Find(Specs.Find.ById<Client>(client.Id))
                                                            .Select(x => x.Territory.OrganizationUnitId)
                                                            .Single();
                        var withinFirmOrgUnitsOrTerritories = _finder
                            .Find<UserTerritoriesOrganizationUnits>(x => x.UserId == currentUserCode &&
                                                                         (x.OrganizationUnitId == clientOrganizationUnit || x.TerritoryId == client.TerritoryId))
                            .Any();
                        if (!withinFirmOrgUnitsOrTerritories)
                        {
                            throw new SecurityException(BLResources.QualifyCouldntAccessClientOnThisOrgUnit);
                        }
                    }

                        break;
                    case ReserveAccess.Full:
                        break;
                    default:
                        throw new NotSupportedException();
                }

                client.LastQualifyTime = qualifyDate;
                _clientGenericSecureRepository.Update(client);
                operationScope.Updated<Client>(client.Id);
                _clientGenericSecureRepository.Save();

                var count = AssignWithRelatedEntities(client.Id, ownerCode, false);

                var clientFirms = _finder.Find(FirmSpecs.Firms.Find.ByClient(client.Id)).ToArray();
                foreach (var firm in clientFirms)
                {
                    firm.LastQualifyTime = qualifyDate;
                    _firmGenericRepository.Update(firm);
                    operationScope.Updated<Firm>(firm.Id);
                }

                _firmGenericRepository.Save();

                operationScope.Complete();

                return count;
            }
        }

        public int Disqualify(Client client, long currentUserCode, long reserveCode, bool bypassValidation, DateTime disqualifyDate)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DisqualifyIdentity, Client>())
            {
                if (client.OwnerCode == reserveCode)
                {
                    throw new ArgumentException(BLResources.DisqualifyEntityMustNotInReserve);
                }

                var checkAggregateForDebtsRepository = this as ICheckAggregateForDebtsRepository<Client>;
                checkAggregateForDebtsRepository.CheckForDebts(client.Id, currentUserCode, bypassValidation);

                var hasDeals = _finder.Find(Specs.Find.ActiveAndNotDeleted<Deal>() && DealSpecs.Deals.Find.ForClient(client.Id)).Any();
                if (hasDeals)
                {
                    throw new ArgumentException(BLResources.DisqualifyNeedToCloseAllOpportunities);
                }

                // Проверяем наличие активных Заказов, связанных с данной Фирмой, если есть активные Заказы, 
                // выдается сообщение "Нельзя передать в резерв Фирму с активными заказами". 
                var hasOpenOrders = _finder.Find(OrderSpecs.Orders.Find.ActiveOrdersForClient(client.Id)).Any();
                if (hasOpenOrders)
                {
                    throw new ArgumentException(BLResources.DisqualifyFirmNeedToCloseAllOrders);
                }

                client.LastDisqualifyTime = disqualifyDate;
                _clientGenericSecureRepository.Update(client);
                scope.Updated<Client>(client.Id);
                _clientGenericSecureRepository.Save();

                // FIXME {all, 23.09.2014}: is it correct to pass bypassValidation as isPartialAssign?
                var count = AssignWithRelatedEntities(client.Id, reserveCode, bypassValidation);

                scope.Complete();
                return count;
            }
        }

        public int ChangeTerritory(Client client, long territoryId)
        {
            if (client.TerritoryId == territoryId)
            {
                return 0;
            }

            // Изменения логируются в вызывающем коде
            client.TerritoryId = territoryId;
            _clientGenericSecureRepository.Update(client);
            return _clientGenericSecureRepository.Save();
        }

        public void ValidateOwnerIsNotReserve(Client client)
        {
            var entityOwnerCode = client.OwnerCode;

            // check reserve user
            var reserveUser = _userIdentifierService.GetReserveUserIdentity();
            if (entityOwnerCode == reserveUser.Code)
            {
                throw new NotificationException(string.Format(CultureInfo.CurrentCulture, BLResources.PleaseUseQualifyOperation, reserveUser.DisplayName));
        }
        }

        public Tuple<Client, Client> MergeErmClients(long mainClientId, long appendedClientId, Client masterClient, bool assignAllObjects)
        {
            var mainClient = _finder.FindOne(Specs.Find.ById<Client>(mainClientId));
            var appendedClient = _finder.FindOne(Specs.Find.ById<Client>(appendedClientId));

            ValidateOwnerIsNotReserve(mainClient);
            ValidateOwnerIsNotReserve(appendedClient);
            PerformSecurityChecks(mainClient, appendedClient);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                Deactivate(appendedClient);

                if (assignAllObjects)
                {
                    // Сначала прокинем куратора в сущности добавляемого клиента
                    AssignWithRelatedEntities(appendedClient.Id, masterClient.OwnerCode, false);
                }
                else
                {
                    Assign(appendedClient, masterClient.OwnerCode);
                }

                var relatedEntities = _secureFinder.Find(Specs.Find.ById<Client>(appendedClient.Id))
                                                   .Select(x => new
                                                       {
                                                           Deals = x.Deals.Where(y => !y.IsDeleted),
                                                           Firms = x.Firms.Where(y => !y.IsDeleted),
                                                           Contacts = x.Contacts.Where(y => !y.IsDeleted),
                                                           LegalPersonIds = x.LegalPersons.Where(y => !y.IsDeleted).Select(y => y.Id),
                                                       })
                                                   .Single();

                using (var operationScope = _scopeFactory.CreateSpecificFor<ChangeClientIdentity, Deal>())
                {
                    foreach (var deal in relatedEntities.Deals)
                    {
                        deal.ClientId = mainClient.Id;
                        _dealGenericRepository.Update(deal);
                        operationScope.Updated<Deal>(deal.Id);
                    }

                    _dealGenericRepository.Save();
                    operationScope.Complete();
                }

                using (var operationScope = _scopeFactory.CreateSpecificFor<ChangeClientIdentity, Contact>())
                {
                    foreach (var contact in relatedEntities.Contacts)
                    {
                        contact.ClientId = mainClient.Id;
                        _contactGenericRepository.Update(contact);
                        operationScope.Updated<Contact>(contact.Id);
                    }

                    _contactGenericRepository.Save();
                    operationScope.Complete();
                }

                using (var operationScope = _scopeFactory.CreateSpecificFor<ChangeClientIdentity, Firm>())
                {
                    foreach (var firm in relatedEntities.Firms)
                    {
                        firm.ClientId = mainClient.Id;
                        _firmGenericRepository.Update(firm);
                        operationScope.Updated<Firm>(firm.Id);
                    }

                    _firmGenericRepository.Save();
                    operationScope.Complete();
                }

                using (var operationScope = _scopeFactory.CreateSpecificFor<ChangeClientIdentity, LegalPerson>())
                {
                    var legalPersons = _finder.FindMany(Specs.Find.ByIds<LegalPerson>(relatedEntities.LegalPersonIds));

                    foreach (var legalPerson in legalPersons)
                    {
                        legalPerson.ClientId = mainClient.Id;
                        _legalPersonGenericRepository.Update(legalPerson);
                        operationScope.Updated<LegalPerson>(legalPerson.Id);
                    }

                    _legalPersonGenericRepository.Save();
                    operationScope.Complete();
                }

                CopyClientInfo(mainClient, masterClient);
                mainClient.OwnerCode = masterClient.OwnerCode;

                // Изменения логируются в вызывающем коде
                _clientGenericSecureRepository.Update(mainClient);
                _clientGenericSecureRepository.Save();

                transaction.Complete();
            }

            return new Tuple<Client, Client>(mainClient, appendedClient);
        }

        public void Deactivate(Client client)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, Client>())
            {
                client.IsActive = false;
                _clientGenericSecureRepository.Update(client);
                _clientGenericSecureRepository.Save();
                scope.Updated(client)
                     .Complete();
            }
        }

        public IEnumerable<Client> GetClientsByTerritory(long territoryId)
        {
            return _finder.FindMany(ClientSpecs.Clients.Find.ByTerritory(territoryId));
        }

        public void ChangeTerritory(IEnumerable<Client> clients, long territoryId)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ChangeTerritoryIdentity, Client>())
            {
                foreach (var client in clients)
                {
                    client.TerritoryId = territoryId;
                    _clientGenericSecureRepository.Update(client);
                    operationScope.Updated<Client>(client.Id);
                }

                _clientGenericSecureRepository.Save();
                operationScope.Complete();
            }
        }

        public void CalculatePromising(long modifiedBy)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<CalculateClientPromisingIdentity>())
            {
                var changedEntities = _clientPersistenceService.CalculateClientPromising(modifiedBy, TimeSpan.FromHours(1));

                scope.ApplyChanges<Client>(changedEntities)
                     .Complete();
            }
        }

        public void CreateOrUpdate(Contact contact)
        {
            if (contact.BirthDate < MinMssqlDatetime || contact.BirthDate > MaxMssqlDatetime)
            {
                throw new NotificationException(BLResources.BirthDateShouldBeMssqlDatetype);
            }

            var fullName = new StringBuilder();

            // generate fullname from pieces
            if (!string.IsNullOrWhiteSpace(contact.LastName))
            {
                fullName.Append(contact.LastName);
                fullName.Append(' ');
            }

            if (!string.IsNullOrWhiteSpace(contact.FirstName))
            {
                fullName.Append(contact.FirstName);
                fullName.Append(' ');
            }

            if (!string.IsNullOrWhiteSpace(contact.MiddleName))
            {
                fullName.Append(contact.MiddleName);
            }

            contact.FullName = fullName.ToString().TrimEnd(' ');

            // Изменения логируются в вызывающем коде
            if (contact.IsNew())
            {
                _identityProvider.SetFor(contact);
                _contactGenericSecureRepository.Add(contact);
            }
            else
            {
                _contactGenericSecureRepository.Update(contact);
            }

            _contactGenericSecureRepository.Save();
        }

        int IAssignAggregateRepository<Client>.Assign(long entityId, long ownerCode)
        {
            var entity = _secureFinder.FindOne(Specs.Find.ById<Client>(entityId));
            return Assign(entity, ownerCode);
        }

        int IAssignAggregateRepository<Contact>.Assign(long entityId, long ownerCode)
        {
            // Изменения логируются в вызывающем коде
            var entity = _finder.Find(Specs.Find.ById<Contact>(entityId)).Single();
            entity.OwnerCode = ownerCode;
            _contactGenericSecureRepository.Update(entity);
            return _contactGenericSecureRepository.Save();
        }

        int IQualifyAggregateRepository<Client>.Qualify(long entityId, long currentUserCode, long reserveCode, long ownerCode, DateTime qualifyDate)
        {
            var entity = _secureFinder.FindOne(Specs.Find.ById<Client>(entityId));
            return Qualify(entity, currentUserCode, reserveCode, ownerCode, qualifyDate);
        }

        int IDisqualifyAggregateRepository<Client>.Disqualify(long entityId, long currentUserCode, long reserveCode, bool bypassValidation, DateTime disqualifyDate)
        {
            var entity = _secureFinder.FindOne(Specs.Find.ById<Client>(entityId));
            return Disqualify(entity, currentUserCode, reserveCode, bypassValidation, disqualifyDate);
        }

        void ICheckAggregateForDebtsRepository<Client>.CheckForDebts(long entityId, long currentUserCode, bool bypassValidation)
        {
            if (bypassValidation)
            {
                var hasProcessAccountsWithDebtsPermissionGranted =
                _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ProcessAccountsWithDebts, currentUserCode);
                if (!hasProcessAccountsWithDebtsPermissionGranted)
                {
                    throw new SecurityException(BLResources.ProcessAccountsWithDebtsOperationIsNotAllowed);
                }

                return;
            }

            var accountWithDebts = (from client in _finder.FindAll<Client>()
                                    where client.Id == entityId
                                    from legalPerson in client.LegalPersons
                                    from account in legalPerson.Accounts
                                    let lockDetailBalance = account.Balance - (account.Locks
                                                                                  .Where(x => x.IsActive && !x.IsDeleted)       // скобки и проверки на null тут НУЖНЫ,
                                                                                  .Sum(x => (decimal?)x.PlannedAmount) ?? 0)    // т.к. без них возможна ситуация decimal - null = null
                                    where lockDetailBalance <= _debtProcessingSettings.MinDebtAmount
                                    select new AccountWithDebtInfo
                                    {
                                        ClientName = client.Name,
                                        LegalPersonName = legalPerson.ShortName,
                                        AccountNumber = account.Id,
                                        LockDetailBalance = lockDetailBalance
                                    })
                    .ToArray();
            DebtsAuditor.ThrowIfAnyError(accountWithDebts);
        }
        
        int IChangeAggregateTerritoryRepository<Client>.ChangeTerritory(long entityId, long territoryId)
        {
            var entity = _secureFinder.FindOne(Specs.Find.ById<Client>(entityId));
            return ChangeTerritory(entity, territoryId);
        }

        [Obsolete("Используется только в DgppImportFirmsHandler")]
        public int HideFirm(long firmId)
        {
            var clients = _finder.FindMany(ClientSpecs.Clients.Find.ByMainFirm(firmId)).ToArray();
            var deals = _finder.Find<Deal>(deal => deal.MainFirmId == firmId).ToArray();

            foreach (var client in clients)
            {
                client.MainFirmId = null;
                _clientGenericSecureRepository.Update(client);
            }

            foreach (var deal in deals)
            {
                deal.MainFirmId = null;
                _dealGenericRepository.Update(deal);
            }

            return _clientGenericSecureRepository.Save()
                   + _dealGenericRepository.Save();
        }

        private static void CopyClientInfo(Client clientTo, Client clientFrom)
        {
            clientTo.Name = clientFrom.Name;
            clientTo.Comment = clientFrom.Comment;
            clientTo.MainAddress = clientFrom.MainAddress;
            clientTo.MainPhoneNumber = clientFrom.MainPhoneNumber;
            clientTo.AdditionalPhoneNumber1 = clientFrom.AdditionalPhoneNumber1;
            clientTo.AdditionalPhoneNumber2 = clientFrom.AdditionalPhoneNumber2;
            clientTo.Email = clientFrom.Email;
            clientTo.Fax = clientFrom.Fax;
            clientTo.Website = clientFrom.Website;
            clientTo.InformationSource = clientFrom.InformationSource;
            clientTo.MainFirmId = clientFrom.MainFirmId;
            clientTo.TerritoryId = clientFrom.TerritoryId;
        }

        private static ReserveAccess GetMaxAccessForReserve(int[] accesses)
        {
            if (!accesses.Any())
            {
                return ReserveAccess.None;
            }

            var priorities = new[] { ReserveAccess.None, ReserveAccess.Territory, ReserveAccess.OrganizationUnit, ReserveAccess.Full };

            var maxPriority = accesses.Select(x => Array.IndexOf(priorities, (ReserveAccess)x)).Max();
            return priorities[maxPriority];
        }

        private static MergeClientsAccess GetMaxAccessForMerge(int[] accesses)
        {
            if (!accesses.Any())
            {
                return MergeClientsAccess.None;
            }

            var priorities = new[]
                {
                    MergeClientsAccess.None, 
                    MergeClientsAccess.User, 
                    MergeClientsAccess.Department, 
                    MergeClientsAccess.DepartmentWithChilds,
                    MergeClientsAccess.Full
                };

            var maxPriority = accesses.Select(x => Array.IndexOf(priorities, (MergeClientsAccess)x)).Max();
            return priorities[maxPriority];
        }

        private void PerformSecurityChecks(Client mainClient, ICuratedEntity appendedClient)
        {
            var privilegeDepth = GetMaxAccessForMerge(_functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.MergeClients, _userContext.Identity.Code));

            switch (privilegeDepth)
            {
                case MergeClientsAccess.None:
                    throw new NotificationException(BLResources.AccessDenied);
                case MergeClientsAccess.User:
                    if (appendedClient.OwnerCode != _userContext.Identity.Code || mainClient.OwnerCode != _userContext.Identity.Code)
                    {
                        throw new NotificationException(BLResources.AccessDenied);
                    }

                    break;
                case MergeClientsAccess.Department:
                    {
                        var mainUser = _userIdentifierService.GetUserInfo(mainClient.OwnerCode);
                        var appendedUser = _userIdentifierService.GetUserInfo(appendedClient.OwnerCode);
                        if (!_userIdentifierService.UsersInSameDepartment(_userContext.Identity.Code, mainUser.Code) || 
                            !_userIdentifierService.UsersInSameDepartment(_userContext.Identity.Code, appendedUser.Code))
                        {
                            throw new NotificationException(BLResources.AccessDenied);
                        }
                    }

                    break;
                case MergeClientsAccess.DepartmentWithChilds:
                    {
                        var mainUser = _userIdentifierService.GetUserInfo(mainClient.OwnerCode);
                        var appendedUser = _userIdentifierService.GetUserInfo(appendedClient.OwnerCode);
                        if (!_userIdentifierService.UsersInSameDepartmentTree(_userContext.Identity.Code, mainUser.Code) || 
                            !_userIdentifierService.UsersInSameDepartmentTree(_userContext.Identity.Code, appendedUser.Code))
                        {
                            throw new NotificationException(BLResources.AccessDenied);
                        }
                    }

                    break;
            }
            }
    }
}