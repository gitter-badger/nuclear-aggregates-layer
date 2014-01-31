﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

using DoubleGis.Erm.BLCore.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts
{
    // TODO {all, 14.02.2013}: вынести методы для работы с OperationType в SimplifiedModel service (см. например ContributionTypeService)
    public class AccountRepository : IAccountRepository
    {
        private const string OperationTypeDebitForOrderPayment = "11";

        private readonly IAppSettings _appSettings;
        private readonly IFinder _finder;
        private readonly ISecureFinder _secureFinder;
        private readonly IRepository<Account> _accountGenericRepository;
        private readonly IRepository<AccountDetail> _accountDetailGenericRepository;
        private readonly ISecureRepository<Account> _accountGenericSecureRepository;
        private readonly ISecureRepository<AccountDetail> _accountDetailGenericSecureRepository;
        private readonly ISecureRepository<Limit> _limitGenericSecureRepository;
        private readonly IRepository<OperationType> _operationTypeGenericRepository;
        private readonly IRepository<Lock> _lockGenericRepository;
        private readonly IRepository<LockDetail> _lockDetailGenericRepository;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public AccountRepository(IAppSettings appSettings,
                                 IFinder finder,
                                 ISecureFinder secureFinder,
                                 IRepository<Account> accountGenericRepository,
                                 IRepository<AccountDetail> accountDetailGenericRepository,
                                 ISecureRepository<Account> accountGenericSecureRepository,
                                 ISecureRepository<AccountDetail> accountDetailGenericSecureRepository,
                                 ISecureRepository<Limit> limitGenericSecureRepository,
                                 IRepository<OperationType> operationTypeGenericRepository,
                                 IRepository<Lock> lockGenericRepository,
                                 IRepository<LockDetail> lockDetailGenericRepository,
                                 ISecurityServiceFunctionalAccess functionalAccessService,
                                 IIdentityProvider identityProvider,
                                 IOperationScopeFactory scopeFactory)
        {
            _appSettings = appSettings;
            _finder = finder;
            _secureFinder = secureFinder;
            _accountGenericRepository = accountGenericRepository;
            _accountDetailGenericRepository = accountDetailGenericRepository;
            _accountGenericSecureRepository = accountGenericSecureRepository;
            _accountDetailGenericSecureRepository = accountDetailGenericSecureRepository;
            _limitGenericSecureRepository = limitGenericSecureRepository;
            _operationTypeGenericRepository = operationTypeGenericRepository;
            _lockGenericRepository = lockGenericRepository;
            _lockDetailGenericRepository = lockDetailGenericRepository;
            _functionalAccessService = functionalAccessService;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public bool IsActiveLocksExists(long orderId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Lock>() && AccountSpecs.Locks.Find.ForOrder(orderId)).Any();
        }

        public bool IsNonDeletedLocksExists(long orderId)
        {
            return _finder.Find(Specs.Find.NotDeleted<Lock>() && AccountSpecs.Locks.Find.ForOrder(orderId)).Any();
        }


        public int GetNonDeletedLocksCount(long orderId)
        {
            return _finder.Find(Specs.Find.NotDeleted<Lock>() && AccountSpecs.Locks.Find.ForOrder(orderId)).Count();
        }

        public int GetActiveLocksCount(long orderId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Lock>() && AccountSpecs.Locks.Find.ForOrder(orderId)).Count();
        }

        public int GetInactiveLocksCount(long orderId)
        {
            return _finder.Find(Specs.Find.InactiveEntities<Lock>()).Count(x => x.OrderId == orderId);
        }

        public int Create(Lock @lock)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity>(EntityName.Lock))
            {
                _identityProvider.SetFor(@lock);
                _lockGenericRepository.Add(@lock);
                count = _lockGenericRepository.Save();

                operationScope
                    .Added<Lock>(@lock.Id)
                    .Complete();
            }

            return count;
        }

        public int CreateLockDetailForLock(long lockId, long priceId, decimal amount)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity>(EntityName.LockDetail))
            {
                var lockDetail = new LockDetail { LockId = lockId, PriceId = priceId, Amount = amount, IsActive = true };

                _identityProvider.SetFor(lockDetail);
                _lockDetailGenericRepository.Add(lockDetail);
                count = _lockDetailGenericRepository.Save();

                operationScope
                    .Added<LockDetail>(lockDetail.Id)
                    .Complete();
            }

            return count;
        }

        public Limit GetLimitById(long id)
        {
            return _finder.Find(Specs.Find.ById<Limit>(id)).SingleOrDefault();
        }

        public Limit GetLimitByReplicationCode(Guid replicationCode)
        {
            return _finder.Find<Limit>(limit => limit.ReplicationCode == replicationCode).SingleOrDefault();
        }

        public int DeleteLocksForPeriod(long destinationOrganizationUnitId, TimePeriod period)
        {
            var processedItems = 0;
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeleteIdentity>(EntityName.Lock))
            {
                var lockInfos =
                    _finder.Find(AccountSpecs.Locks.Find.ByDestinationOrganizationUnit(destinationOrganizationUnitId, period))
                    .Select(l => new { Lock = l, l.LockDetails });

                foreach (var lockInfo in lockInfos)
                {
                    decimal balance = 0m;

                    if (lockInfo.LockDetails != null)
                    {
                        foreach (var lockDetail in lockInfo.LockDetails)
                        {
                            balance += lockDetail.Amount;
                            
                            _lockDetailGenericRepository.Delete(lockDetail);
                            operationScope.Deleted<LockDetail>(lockDetail.Id);
                        }

                        _lockDetailGenericRepository.Save();
                    }
                    
                    lockInfo.Lock.Balance = balance;
                    _lockGenericRepository.Delete(lockInfo.Lock);
                    operationScope.Deleted<Lock>(lockInfo.Lock.Id);

                    _lockGenericRepository.Save();
                    ++processedItems;
                }
                
                operationScope.Complete();
            }

            return processedItems;
        }

        public int Update(AccountDetail accountDetail)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<UpdateIdentity>(EntityName.AccountDetail))
            {
                _accountDetailGenericSecureRepository.Update(accountDetail);
                count = _accountDetailGenericSecureRepository.Save();

                operationScope
                    .Updated<AccountDetail>(accountDetail.Id)
                    .Complete();
            }

            return count;
        }

        public int Update(Lock @lock)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<UpdateIdentity>(EntityName.Lock))
            {
                _lockGenericRepository.Update(@lock);
                count = _lockGenericRepository.Save();

                operationScope.Updated<Lock>(@lock.Id).Complete();
            }

            return count;
        }

        public int Update(LockDetail lockDetail)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<UpdateIdentity>(EntityName.LockDetail))
            {
                _lockDetailGenericRepository.Update(lockDetail);
                count = _lockDetailGenericRepository.Save();

                operationScope.Updated<LockDetail>(lockDetail.Id).Complete();
            }

            return count;
        }

        public Account CreateAccount(long legalPersonId, long branchOfficeOrganizationUnitId)
        {
            var accounts = _finder.Find(AccountSpecs.Accounts.Find.Existing(legalPersonId, branchOfficeOrganizationUnitId)).ToArray();
            if (accounts.Length > 1)
            {
                throw new NotificationException(BLResources.MultipleAccountsForLegalPersonAndBranchOfficeOrgUnit);
            }

            return accounts.FirstOrDefault() ?? CreateAccountImpl(legalPersonId, branchOfficeOrganizationUnitId);
        }

        public Account SecureCreateAccount(long legalPersonId, long branchOfficeOrganizationUnitId)
        {
            var account = _secureFinder.Find(AccountSpecs.Accounts.Find.Existing(legalPersonId, branchOfficeOrganizationUnitId)).SingleOrDefault();
            return account ?? CreateAccountImpl(legalPersonId, branchOfficeOrganizationUnitId);
        }

        public int Create(OperationType operationType)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity>(EntityName.OperationType))
            {
                _operationTypeGenericRepository.Add(operationType);

                count = _operationTypeGenericRepository.Save();

                operationScope
                    .Added<OperationType>(operationType.Id)
                    .Complete();
            }

            return count;
        }

        public int Update(OperationType operationType)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<UpdateIdentity>(EntityName.OperationType))
            {
                _operationTypeGenericRepository.Update(operationType);
                count = _operationTypeGenericRepository.Save();

                operationScope
                    .Updated<OperationType>(operationType.Id)
                    .Complete();
            }

            return count;
        }

        public int Create(Limit limit)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity>(EntityName.Limit))
            {
                _identityProvider.SetFor(limit);

                var accountOwnerCode = _finder.Find(Specs.Find.ById<Account>(limit.AccountId))
                                              .Select(x => x.OwnerCode)
                                              .Single();
                limit.OwnerCode = accountOwnerCode;
                
                _limitGenericSecureRepository.Add(limit);
                count = _limitGenericSecureRepository.Save();

                operationScope
                    .Added<Limit>(limit.Id)
                    .Complete();
            }

            return count;
        }

        public int Update(Limit limit)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<UpdateIdentity>(EntityName.Limit))
            {
                var accountOwnerCode = _finder.Find(Specs.Find.ById<Account>(limit.AccountId))
                                              .Select(x => x.OwnerCode)
                                              .Single();
                limit.OwnerCode = accountOwnerCode;

                _limitGenericSecureRepository.Update(limit);
                count = _limitGenericSecureRepository.Save();

                operationScope
                    .Updated<Limit>(limit.Id)
                    .Complete();
            }

            return count;
        }

        public int Delete(LockDetail entity)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeleteIdentity>(EntityName.LockDetail))
            {
                _lockDetailGenericRepository.Delete(entity);
                count = _lockDetailGenericRepository.Save();

                operationScope
                    .Deleted<LockDetail>(entity.Id)
                    .Complete();
            }

            return count;
        }

        public int Activate(Limit limit)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<ActivateIdentity>(EntityName.Limit))
            {
                limit.IsActive = true;
                limit.CloseDate = null;
                _limitGenericSecureRepository.Update(limit);
                count = _limitGenericSecureRepository.Save();

                operationScope
                    .Updated<Limit>(limit.Id)
                    .Complete();
            }

            return count;
        }

        public int Delete(Lock entity)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeleteIdentity>(EntityName.Lock))
            {
                _lockGenericRepository.Delete(entity);
                count = _lockGenericRepository.Save();

                operationScope
                    .Deleted<Lock>(entity.Id)
                    .Complete();
            }

            return count;
        }

        public GetLockDetailDto GetLockDetail(long entityId)
        {
            return _finder.Find(Specs.Find.ById<LockDetail>(entityId))
                .Select(x => new GetLockDetailDto { LockDetail = x, Lock = x.Lock })
                .SingleOrDefault();
        }

        public IDictionary<long, decimal> GetBalanceByAccounts(IEnumerable<long> accountIds)
        {
            var distinctAccountIds = accountIds.Distinct().ToArray();
            return (from account in _finder.Find(Specs.Find.ByIds<Account>(distinctAccountIds))
                    let balance = account.AccountDetails
                        .Where(x => !x.IsDeleted)
                        .Sum(y => (decimal?)(y.OperationType.IsPlus ? y.Amount : -y.Amount))
                    select new { account.Id, balance }).ToDictionary(x => x.Id, x => x.balance == null ? 0m : x.balance.Value);
        }

        public int Create(AccountDetail accountDetail)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity>(EntityName.AccountDetail))
            {
                _identityProvider.SetFor(accountDetail);
                accountDetail.OwnerCode = FindAccount(accountDetail.AccountId).OwnerCode;   // как куратор операции выставляется куратор родительского лицевого счёта. Куратор выставляется только при создании операции по лицевому счету

                _accountDetailGenericSecureRepository.Add(accountDetail);
                count = _accountDetailGenericSecureRepository.Save();

                operationScope
                    .Added<AccountDetail>(accountDetail.Id)
                    .Complete();
            }
            return count;
        }

        public int Create(IEnumerable<AccountDetail> accountDetails)
        {
            int count;

            // TODO {all, 11.09.2013}: В процессе рефакторинга перевести на операцию c bulk*identity
            using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity>(EntityName.AccountDetail))
            {
                foreach (var accountDetail in accountDetails)
                {
                    _identityProvider.SetFor(accountDetail);
                    
                    _accountDetailGenericRepository.Add(accountDetail);
                    operationScope.Added<AccountDetail>(accountDetail.Id);
                }

                count = _accountDetailGenericRepository.Save();
                operationScope.Complete();
            }

            return count;
        }

        public int Update(Account account)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<UpdateIdentity>(EntityName.Account))
            {
                _accountGenericSecureRepository.Update(account);
                count = _accountGenericSecureRepository.Save();

                operationScope
                    .Updated<Account>(account.Id)
                    .Complete();
            }

            return count;
        }

        public IReadOnlyCollection<OperationType> GetOperationsInSyncWith1C()
        {
            return _finder.Find<OperationType>(x => x.IsInSyncWith1C && x.IsActive && !x.IsDeleted).ToArray();
        }

        public int Delete(Account account)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeleteIdentity>(EntityName.Account))
            {
                _accountGenericSecureRepository.Delete(account);
                count = _accountGenericSecureRepository.Save();

                operationScope
                    .Deleted<Account>(account.Id)
                    .Complete();
            }

            return count;
        }

        public int Delete(AccountDetail accountDetail)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeleteIdentity>(EntityName.AccountDetail))
            {
                _accountDetailGenericSecureRepository.Delete(accountDetail);
                count = _accountDetailGenericSecureRepository.Save();

                operationScope
                    .Deleted<AccountDetail>(accountDetail.Id)
                    .Complete();
            }

            return count;
        }

        public int Delete(IEnumerable<AccountDetail> accountDetails)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeleteIdentity>(EntityName.AccountDetail))
            {
                foreach (var accountDetail in accountDetails)
                {
                    _accountDetailGenericSecureRepository.Delete(accountDetail);
                    operationScope.Deleted<AccountDetail>(accountDetail.Id);
                }

                count = _accountDetailGenericSecureRepository.Save();
                operationScope.Complete();
            }

            return count;
        }

        public int Delete(Limit limit)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeleteIdentity>(EntityName.Limit))
            {
                _limitGenericSecureRepository.Delete(limit);
                count = _limitGenericSecureRepository.Save();

                operationScope
                    .Deleted<Limit>(limit.Id)
                    .Complete();
            }

            return count;
        }

        public int Delete(OperationType entity)
        {
            int count;
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeleteIdentity>(EntityName.OperationType))
            {
                _operationTypeGenericRepository.Delete(entity);
                count = _operationTypeGenericRepository.Save();

                operationScope
                    .Deleted<OperationType>(entity.Id)
                    .Complete();
            }

            return count;
        }

        public IEnumerable<OperationType> GetOperationTypes(string syncCode1C)
        {
            return _finder.Find<OperationType>(x => x.IsActive && !x.IsDeleted && x.SyncCode1C == syncCode1C && x.IsInSyncWith1C).ToArray();
        }

        public int Deactivate(Limit limit)
        {
            throw new NotSupportedException(BLResources.OperationIsDiabled);
        }

        public void UpdateAccountBalance(IEnumerable<long> accountIds)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<UpdateIdentity>(EntityName.Account))
            {
                var accountInfos = _finder.Find<Account>(x => accountIds.Contains(x.Id))
                                          .Select(x => new
                                              {
                                                  Account = x,
                                                  NewBalance = x.AccountDetails
                                                                .Where(y => !y.IsDeleted)
                                                                .Sum(y => (decimal?)(y.OperationType.IsPlus ? y.Amount : -y.Amount))
                                              })
                                          .ToArray();

                foreach (var accountInfo in accountInfos)
                {
                    accountInfo.Account.Balance = accountInfo.NewBalance ?? 0m;
                    _accountGenericRepository.Update(accountInfo.Account);

                    operationScope.Updated<Account>(accountInfo.Account.Id);
                }

                _accountGenericRepository.Save();
                operationScope.Complete();
            }
        }

        public decimal? CalculateDebitForOrderPaymentSum(long orderId)
        {
            // Списание в счет оплаты БЗ
            // Определяется через неактивную блокировку на операции лицевого счета
            return _finder.Find(Specs.Find.InactiveEntities<Lock>() && AccountSpecs.Locks.Find.ForOrder(orderId))
                          .Where(l => l.AccountDetail.OperationType.SyncCode1C == OperationTypeDebitForOrderPayment
                                      && l.AccountDetail.IsActive && !l.AccountDetail.IsDeleted)
                          .Sum(l => (decimal?)l.AccountDetail.Amount);
        }

        public bool IsCreateAccountDetailValid(long accountId, long userCode, bool checkContributionType)
        {
            var branchOfficeInfo = _finder.Find<Account>(x => x.Id == accountId)
                .Select(x => new { x.BranchOfficeOrganizationUnit.BranchOfficeId, x.BranchOfficeOrganizationUnit.BranchOffice.ContributionTypeId })
                .FirstOrDefault();

            if (checkContributionType && (branchOfficeInfo == null || branchOfficeInfo.ContributionTypeId != (int)ContributionTypeEnum.Franchisees))
            {
                return false;
            }

            var isCurrentUserInBranchOffice = _finder.Find<UserTerritoriesOrganizationUnits>(x => x.UserId == userCode)
                .SelectMany(x => x.OrganizationUnit.BranchOfficeOrganizationUnits)
                .Select(x => x.BranchOffice)
                .Any(x => x.Id == branchOfficeInfo.BranchOfficeId);
            return isCurrentUserInBranchOffice;
        }

        public Account FindAccount(long entityId)
        {
            return _secureFinder.Find(Specs.Find.ById<Account>(entityId)).SingleOrDefault();
        }

        public AccountDetail GetAccountDetail(long entityId)
        {
            return _secureFinder.Find(Specs.Find.ById<AccountDetail>(entityId)).Single();
        }

        public Limit FindLimit(long entityId)
        {
            return _finder.Find(Specs.Find.ById<Limit>(entityId)).SingleOrDefault();
        }

        public OperationTypeDto GetOperationTypeDto(long entityId)
        {
            // CR: {a.bakhturin}:{312}:{Minor}:{15.04.2011}: т.е. неактивные позиции тоже не будут позволять удалить тип операции
            return _finder.Find(Specs.Find.ById<OperationType>(entityId))
                .Select(operationType => new OperationTypeDto
                {
                    OperationType = operationType,
                    AllAccountDetailsIsDeleted = operationType.AccountDetails.All(accountDetail => accountDetail.IsDeleted),
                })
               .SingleOrDefault();
        }

        public AccountDetail[] GetAccountDetailsForImportFrom1COperation(string branchOfficeOrganizationUnit1CCode, DateTime transactionPeriodStart, DateTime transactionPeriodEnd)
        {
            var operationTypeIds = _finder.Find<OperationType>(x => x.IsActive && x.IsDeleted == false && x.IsInSyncWith1C).Select(x => x.Id).ToArray();

            return _finder.Find<BranchOfficeOrganizationUnit>(x => !x.IsDeleted && branchOfficeOrganizationUnit1CCode == x.SyncCode1C)
                          .SelectMany(x => x.Accounts)
                          .Where(x => !x.IsDeleted)
                          .SelectMany(x => x.AccountDetails)
                          .Where(x => !x.IsDeleted &&
                                      operationTypeIds.Contains(x.OperationTypeId) &&
                                      x.TransactionDate >= transactionPeriodStart &&
                                      x.TransactionDate <= transactionPeriodEnd)
                          .ToArray();
        }

        public IEnumerable<AccountInfoForImportFrom1C> GetAccountsForImportFrom1C(IEnumerable<string> branchOfficeSyncCodes, DateTime transactionPeriodStart, DateTime transactionPeriodEnd)
        {
            return _finder.Find<Account>(a => !a.IsDeleted && branchOfficeSyncCodes.Contains(a.BranchOfficeOrganizationUnit.SyncCode1C)).Select(x => new AccountInfoForImportFrom1C
                {
                    Id = x.Id,
                    AccountDetails = x.AccountDetails.Where(y => !y.IsDeleted && y.OperationType.IsActive &&
                                                                 !y.OperationType.IsDeleted && y.OperationType.IsInSyncWith1C &&
                                                                 y.TransactionDate >= transactionPeriodStart && y.TransactionDate <= transactionPeriodEnd),
                    BranchOfficeLegalName = x.BranchOfficeOrganizationUnit.ShortLegalName,
                    BranchOfficeSyncCode1C = x.BranchOfficeOrganizationUnit.SyncCode1C,
                    LegalPersonName = x.LegalPerson.LegalName,
                    OwnerCode = x.OwnerCode
                }).ToArray();
        }


        public Account FindAccount(long branchOfficeOrganizationUnitId, long legalPersonId)
        {
            return _finder.Find<Account>(x => x.IsActive && !x.IsDeleted &&
                                              x.BranchOfficeOrganizationUnitId == branchOfficeOrganizationUnitId &&
                                              x.LegalPersonId == legalPersonId)
                .SingleOrDefault();
        }

        public IEnumerable<Account> GetAccountsByLegalPerson(string legalPersonSyncCode1C)
        {
            return _finder.Find(AccountSpecs.Accounts.Find.ByLegalPersonSyncCode1C(legalPersonSyncCode1C)).ToArray();
        }

        public void RecalculateLimitValue(Limit limit, DateTime periodStart, DateTime periodEnd)
        {
            limit.Amount = RecalculateLimitValue(limit.AccountId, periodStart, periodEnd);
        }

        public bool IsLimitRecalculationAvailable(long limitId)
        {
            var limit = _finder.Find(Specs.Find.ById<Limit>(limitId)).Single();

            // заказы, по которым считается лимит плюс заказы в состоянии "на расторжении"
            var orderOrganizationUnits = _finder.Find(Specs.Find.ActiveAndNotDeleted<Order>())
                .Where(order => order.AccountId == limit.AccountId &&
                    (order.BeginDistributionDate < limit.EndPeriodDate && order.EndDistributionDateFact > limit.StartPeriodDate) &&
                    (order.WorkflowStepId == (int)OrderState.Approved || order.WorkflowStepId == (int)OrderState.OnTermination))
                .Select(order => order.DestOrganizationUnitId)
                .Distinct()
                .ToArray();

            var releaseOrganizationUnits = _finder.Find(Specs.Find.ActiveAndNotDeleted<ReleaseInfo>())
                .Where(info => (info.PeriodStartDate < limit.EndPeriodDate && info.PeriodEndDate > limit.StartPeriodDate) &&
                               !info.IsBeta &&
                               info.Status == (int)ReleaseStatus.Success)
                .Select(info => info.OrganizationUnitId)
                .Distinct()
                .ToArray();

            var existsOrderWithoutFinalRelease = orderOrganizationUnits.Except(releaseOrganizationUnits).Any();

            // если заказов нет вообще (а не только заказов, по которым не проведена сборка), то пересчёт лимита возможен
            return !orderOrganizationUnits.Any() || existsOrderWithoutFinalRelease;
        }

        public void RecalculateLockValue(Lock lockEntity)
        {
            lockEntity.Balance = _finder.Find(Specs.Find.ActiveAndNotDeleted<LockDetail>())
                .Where(detail => detail.LockId == lockEntity.Id)
                .Sum(detail => (decimal?)detail.Amount ?? 0);
        }

        public LimitDto InitializeLimitForAccount(long accountId, DateTime periodStart, DateTime periodEnd)
        {
            var dto = _finder.Find(Specs.Find.ById<Account>(accountId))
                .Select(x => new LimitDto
                {
                    LegalPersonId = x.LegalPerson.Id,
                    LegalPersonName = x.LegalPerson.LegalName,
                    BranchOfficeOrganizationUnitId = x.BranchOfficeOrganizationUnitId,
                    BranchOfficeId = x.BranchOfficeOrganizationUnit.BranchOffice.Id,
                    BranchOfficeName = x.BranchOfficeOrganizationUnit.BranchOffice.Name,
                    LegalPersonOwnerId = x.LegalPerson.OwnerCode,
                })
                .Single();

            dto.Amount = RecalculateLimitValue(accountId, periodStart, periodEnd);
            return dto;
        }

        public bool IsLimitExists(long accountId, DateTime periodStartDate, DateTime periodEndDate, long excludeLimitId)
        {
            return _finder.Find(Specs.Find.ById<Account>(accountId))
                .SelectMany(account => account.Limits)
                .Where(limit => !limit.IsDeleted)
                .Any(limit => limit.StartPeriodDate == periodStartDate && limit.EndPeriodDate == periodEndDate && limit.Id != excludeLimitId);
        }

        public IEnumerable<AccountFor1CExportDto> GetAccountsForExortTo1C(long organizationUnitId)
        {
            return (from account in _finder.FindAll<Account>()
                    let lpSyncCode1C = account.LegalPesonSyncCode1C
                    from order in account.LegalPerson.Orders
                    where !order.IsDeleted &&
                          (order.SourceOrganizationUnitId == organizationUnitId ||
                           order.DestOrganizationUnitId == organizationUnitId) &&
                           (order.WorkflowStepId == (int)OrderState.Approved ||
                           order.WorkflowStepId == (int)OrderState.OnRegistration ||
                           order.WorkflowStepId == (int)OrderState.OnApproval)
                    select new AccountFor1CExportDto
                    {
                        LegalPersonSyncCode1C = lpSyncCode1C,
                        BranchOfficeOrganizationUnitSyncCode1C = order.BranchOfficeOrganizationUnit.SyncCode1C
                    })
                .Distinct()
                .ToArray();
        }

        int IAssignAggregateRepository<Account>.Assign(long entityId, long ownerCode)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity>(EntityName.Account))
            {
                var limits = 
                    _secureFinder
                        .Find(AccountSpecs.Limits.Find.ForAccount(entityId) && Specs.Find.ActiveAndNotDeleted<Limit>())
                        .ToArray();

                foreach (var limit in limits)
                {
                    limit.OwnerCode = ownerCode;
                    _limitGenericSecureRepository.Update(limit);

                    operationScope.Updated<Limit>(limit.Id);
                }

                _limitGenericSecureRepository.Save();

                var entity = _secureFinder.Find(Specs.Find.ById<Account>(entityId)).Single();
                entity.OwnerCode = ownerCode;
                _accountGenericSecureRepository.Update(entity);
                var count = _accountGenericSecureRepository.Save();

                operationScope
                    .Updated<Account>(entity.Id)
                    .Complete();

                return count;
            }
        }

        int IAssignAggregateRepository<AccountDetail>.Assign(long entityId, long ownerCode)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity>(EntityName.AccountDetail))
            {
                var entity = _secureFinder.Find(Specs.Find.ById<AccountDetail>(entityId)).Single();
                entity.OwnerCode = ownerCode;
                _accountDetailGenericSecureRepository.Update(entity);
                var count = _accountDetailGenericSecureRepository.Save();

                operationScope
                    .Updated<AccountDetail>(entity.Id)
                    .Complete();

                return count;
            }
        }

        int IAssignAggregateRepository<Limit>.Assign(long entityId, long ownerCode)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity>(EntityName.Limit))
            {
                var entity = _secureFinder.Find(Specs.Find.ById<Limit>(entityId)).Single();
                entity.OwnerCode = ownerCode;
                _limitGenericSecureRepository.Update(entity);
                var count = _limitGenericSecureRepository.Save();

                operationScope
                    .Updated<Limit>(entity.Id)
                    .Complete();

                return count;
            }
        }

        int IDeactivateAggregateRepository<Limit>.Deactivate(long entityId)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<Limit>(entityId)).Single();
            return Deactivate(entity);
        }

        int IActivateAggregateRepository<Limit>.Activate(long entityId)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<Limit>(entityId)).Single();
            return Activate(entity);
        }

        int IDeleteAggregateRepository<Lock>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<Lock>(entityId)).Single();
            return Delete(entity);
        }

        int IDeleteAggregateRepository<LockDetail>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<LockDetail>(entityId)).Single();
            return Delete(entity);
        }

        int IDeleteAggregateRepository<Limit>.Delete(long entityId)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<Limit>(entityId)).Single();
            return Delete(entity);
        }

        int IDeleteAggregateRepository<OperationType>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<OperationType>(entityId)).Single();
            return Delete(entity);
        }

        void ICheckAggregateForDebtsRepository<Account>.CheckForDebts(long entityId, long currentUserCode, bool bypassValidation)
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

            var accountWithDebts = (from account in _finder.Find(Specs.Find.ById<Account>(entityId))
                                    let lockDetailBalance = account.Balance - (account.Locks
                                                                                   .Where(x => x.IsActive && !x.IsDeleted)      // скобки и проверки на null тут НУЖНЫ,
                                                                                   .Sum(x => (decimal?)x.PlannedAmount) ?? 0)  // т.к. без них возможна ситуация decimal - null = null
                                    where lockDetailBalance <= _appSettings.MinDebtAmount
                                    select new AccountWithDebtInfo
                                    {

                                        LegalPersonName = account.LegalPerson.ShortName,
                                        AccountNumber = account.Id,
                                        LockDetailBalance = lockDetailBalance
                                    })
                .ToArray();
            var errorMessage = CheckForDebtsHelper.CollectErrors(accountWithDebts);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new ProcessAccountsWithDebtsException(errorMessage);
            }
        }

        private decimal RecalculateLimitValue(long accountId, DateTime periodStart, DateTime periodEnd)
        {
            var lockSum = _finder.Find(Specs.Find.ActiveAndNotDeleted<Lock>())
                .Where(@lock => @lock.AccountId == accountId && @lock.PeriodStartDate < periodStart && @lock.PeriodEndDate < periodEnd)
                .Sum(@lock => (decimal?)@lock.PlannedAmount) ?? 0;

            var orderReleaseSum = _finder.Find(Specs.Find.ActiveAndNotDeleted<Order>())
                .Where(order => order.AccountId == accountId)
                .Where(order => order.WorkflowStepId == (int)OrderState.Approved)
                .SelectMany(order => order.OrderReleaseTotals)
                .Where(total => total.ReleaseBeginDate == periodStart && total.ReleaseEndDate == periodEnd)
                .Sum(total => (decimal?)total.AmountToWithdraw) ?? 0;

            var account = _finder.Find(Specs.Find.ById<Account>(accountId)).Single();
            var accountBalance = account.Balance - (lockSum + orderReleaseSum);
            return accountBalance > 0 ? 0 : Math.Abs(accountBalance);
        }

        private Account CreateAccountImpl(long legalPersonId, long branchOfficeOrganizationUnitId)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity>(EntityName.Account))
            {
                // Проверяем, есть ли ДРУГОЕ юр.лицо клиента с такими же ИНН/КПП, но имеющее лицевой счет
                var legalPersonInfo = _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId))
                                             .Select(x => new { x.Inn, x.Kpp, x.OwnerCode })
                                             .Single();
                var isAccountExists = _finder.Find<LegalPerson>(x => x.Id != legalPersonId &&
                                                                     x.Inn.Equals(legalPersonInfo.Inn, StringComparison.Ordinal) &&
                                                                     x.Kpp.Equals(legalPersonInfo.Kpp, StringComparison.Ordinal) &&
                                                                     x.Accounts.Any(y => !y.IsDeleted &&
                                                                                         y.BranchOfficeOrganizationUnitId == branchOfficeOrganizationUnitId))
                                             .Any();
                if (isAccountExists)
                {
                    throw new ArgumentException(BLResources.LegalPersonWithTheSameInnKppAndAccountExists);
                }

                var syncCode1C = _secureFinder.Find(Specs.Find.ById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnitId))
                                              .Select(x => x.OrganizationUnit.SyncCode1C)
                                              .Single();
                var account = new Account
                    {
                        LegalPersonId = legalPersonId,
                        BranchOfficeOrganizationUnitId = branchOfficeOrganizationUnitId,
                        Balance = 0.0m,
                        LegalPesonSyncCode1C = string.Format("{0}-{1}-{2}", syncCode1C, branchOfficeOrganizationUnitId, legalPersonId),
                        OwnerCode = legalPersonInfo.OwnerCode,
                        IsActive = true
                    };

                _identityProvider.SetFor(account);
                _accountGenericRepository.Add(account);
                _accountGenericRepository.Save();

                operationScope
                    .Added<Account>(account.Id)
                    .Complete();

                return account;
            }
        }

        #region nested 

        public class AccountInfoForImportFrom1C
        {
            public long Id { get; set; }
            public IEnumerable<AccountDetail> AccountDetails { get; set; }
            public string LegalPersonName { get; set; }
            public string BranchOfficeLegalName { get; set; }
            public string BranchOfficeSyncCode1C { get; set; }
            public long OwnerCode { get; set; }
        }

        #endregion
    }
}
