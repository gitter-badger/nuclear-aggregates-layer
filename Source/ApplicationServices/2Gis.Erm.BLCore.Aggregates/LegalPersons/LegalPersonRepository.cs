using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.DTO;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons
// ReSharper restore CheckNamespace
{
    public class LegalPersonRepository : ILegalPersonRepository
    {
        private readonly IDebtProcessingSettings _debtProcessingSettings;
        private readonly IFinder _finder;
        private readonly ISecureFinder _secureFinder;
        private readonly ISecureRepository<LegalPerson> _legalPersonGenericRepository;
        private readonly ISecureRepository<Account> _accountGenericRepository;
        private readonly ISecureRepository<Bargain> _bargainGenericRepository;
        private readonly ISecureRepository<Order> _orderGenericRepository;
        private readonly ISecureRepository<Limit> _limitGenericRepository;
        private readonly IRepository<LegalPersonProfile> _legalPersonProfileGenericRepository;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public LegalPersonRepository(
            IDebtProcessingSettings debtProcessingSettings,
            IFinder finder,
            ISecureRepository<LegalPerson> legalPersonGenericRepository,
            ISecurityServiceEntityAccess entityAccessService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IRepository<LegalPersonProfile> legalPersonProfileGenericRepository,
            ISecureRepository<Limit> limitGenericRepository,
            ISecureRepository<Order> orderGenericRepository,
            ISecureRepository<Bargain> bargainGenericRepository,
            ISecureRepository<Account> accountGenericRepository, 
            ISecureFinder secureFinder, 
            IIdentityProvider identityProvider, 
            IOperationScopeFactory scopeFactory)
        {
            _debtProcessingSettings = debtProcessingSettings;
            _finder = finder;
            _legalPersonGenericRepository = legalPersonGenericRepository;
            _entityAccessService = entityAccessService;
            _functionalAccessService = functionalAccessService;
            _legalPersonProfileGenericRepository = legalPersonProfileGenericRepository;
            _limitGenericRepository = limitGenericRepository;
            _orderGenericRepository = orderGenericRepository;
            _bargainGenericRepository = bargainGenericRepository;
            _accountGenericRepository = accountGenericRepository;
            _secureFinder = secureFinder;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public int Activate(LegalPerson legalPerson)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<ActivateIdentity, LegalPerson>())
            {
                var profiles = _finder.Find<LegalPersonProfile>(profile => profile.LegalPersonId == legalPerson.Id).ToArray();
                foreach (var legalPersonProfile in profiles)
                {
                    legalPersonProfile.IsActive = true;
                    _legalPersonProfileGenericRepository.Update(legalPersonProfile);
                    operationScope.Updated<LegalPersonProfile>(legalPersonProfile.Id);
                }

                legalPerson.IsActive = true;
                _legalPersonGenericRepository.Update(legalPerson);

                var result = _legalPersonGenericRepository.Save() + _legalPersonProfileGenericRepository.Save();

                operationScope.Updated<LegalPerson>(legalPerson.Id);
                operationScope.Complete();

                return result;
            }
        }

        public int Assign(LegalPerson legalPerson, long ownerCode)
        {
            legalPerson.OwnerCode = ownerCode;
            _legalPersonGenericRepository.Update(legalPerson);
            return _legalPersonGenericRepository.Save();
        }

        public void ChangeLegalRequisites(LegalPerson legalPerson, string inn, string kpp, string legalAddress)
        {
            legalPerson.Inn = inn;
            legalPerson.Kpp = kpp;
            legalPerson.LegalAddress = legalAddress;
            _legalPersonGenericRepository.Update(legalPerson);
            _legalPersonGenericRepository.Save();
        }

        public void ChangeNaturalRequisites(LegalPerson legalPerson, string passportSeries, string passportNumber, string registrationAddress)
        {
            legalPerson.PassportNumber = passportNumber;
            legalPerson.PassportSeries = passportSeries;
            legalPerson.RegistrationAddress = registrationAddress;
            _legalPersonGenericRepository.Update(legalPerson);
            _legalPersonGenericRepository.Save();
        }

        public LegalPersonForMergeDto GetInfoForMerging(long legalPersonId)
        {
            return _finder.Find<LegalPerson>(x => x.Id == legalPersonId && !x.IsDeleted && x.IsActive)
                .Select(x => new LegalPersonForMergeDto
                                        {
                                            LegalPerson = x,
                                            Accounts = x.Accounts,
                        AccountDetails = x.Accounts.SelectMany(y => y.AccountDetails.Where(z => z.IsActive && !z.IsDeleted)),
                                            Orders = x.Orders,
                                            Bargains = x.Bargains,
                                            Profiles = x.LegalPersonProfiles
                    })
                .SingleOrDefault();
        }

        public int Deactivate(LegalPerson legalPerson)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, LegalPerson>())
            {
                var profiles = _finder.Find<LegalPersonProfile>(x => x.LegalPersonId == legalPerson.Id).ToArray();
                foreach (var profile in profiles)
                {
                    profile.IsActive = false;
                    _legalPersonProfileGenericRepository.Update(profile);
                    operationScope.Updated<LegalPersonProfile>(profile.Id);
                }

                _legalPersonProfileGenericRepository.Save();

                legalPerson.IsActive = false;
                _legalPersonGenericRepository.Update(legalPerson);

                var count = _legalPersonGenericRepository.Save();
                operationScope.Updated<LegalPerson>(legalPerson.Id);
                operationScope.Complete();
                return count;
            }
        }

        public int Delete(LegalPerson legalPerson)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeleteIdentity, LegalPerson>())
            {
                var profiles = _finder.Find<LegalPersonProfile>(x => x.LegalPersonId == legalPerson.Id).ToArray();
                foreach (var profile in profiles)
                {
                    profile.IsActive = false;
                    _legalPersonProfileGenericRepository.Update(profile);
                    operationScope.Updated<LegalPersonProfile>(profile.Id);
                }

                _legalPersonProfileGenericRepository.Save();

                _legalPersonGenericRepository.Delete(legalPerson);
                var count = _legalPersonGenericRepository.Save();

                operationScope.Deleted<LegalPerson>(legalPerson.Id);
                operationScope.Complete();
                return count;
            }
        }

        public void CreateOrUpdate(LegalPerson legalPerson)
        {
            if (legalPerson.IsNew())
            {
                _identityProvider.SetFor(legalPerson);
                _legalPersonGenericRepository.Add(legalPerson);
            }
            else
            {
                _legalPersonGenericRepository.Update(legalPerson);
            }

            _legalPersonGenericRepository.Save();
        }

        public void SyncWith1CDeferred(LegalPerson legalPerson)
        {
            legalPerson.IsInSyncWith1C = true;
            _legalPersonGenericRepository.Update(legalPerson);
        }

        public void SyncWith1C(IEnumerable<LegalPerson> legalPersons)
        {
            foreach (var legalPerson in legalPersons)
            {
                SyncWith1CDeferred(legalPerson);
            }

            _legalPersonGenericRepository.Save();
        }

        public CheckForDublicatesResultDto CheckIfExistsInnDuplicate(long legalPersonId, string inn)
        {
            var dublicateEntities = _finder.Find<LegalPerson>(x => x.Id != legalPersonId)
                                           .Where(x => x.Inn.Trim().Equals(inn, StringComparison.OrdinalIgnoreCase))
                                           .ToArray();

            var result = new CheckForDublicatesResultDto
                {
                    ActiveDublicateExists = dublicateEntities.Any(x => x.IsActive && !x.IsDeleted),
                    InactiveDublicateExists = dublicateEntities.Any(x => !x.IsActive && !x.IsDeleted),
                    DeletedDublicateExists = dublicateEntities.Any(x => x.IsDeleted)
                };

            return result;
        }

        public CheckForDublicatesResultDto CheckIfExistsInnAndKppDuplicate(long legalPersonId, string inn, string kpp)
        {
            var dublicateEntities = _finder.Find<LegalPerson>(x => x.Id != legalPersonId)
                                           .Where(x => x.Inn.Trim().Equals(inn, StringComparison.OrdinalIgnoreCase) &&
                                                       x.Kpp.Trim().Equals(kpp, StringComparison.OrdinalIgnoreCase))
                                           .ToArray();

            var result = new CheckForDublicatesResultDto
        {
                    ActiveDublicateExists = dublicateEntities.Any(x => x.IsActive && !x.IsDeleted),
                    InactiveDublicateExists = dublicateEntities.Any(x => !x.IsActive && !x.IsDeleted),
                    DeletedDublicateExists = dublicateEntities.Any(x => x.IsDeleted)
                };

            return result;
        }

        public CheckForDublicatesResultDto CheckIfExistsInnOrIcDuplicate(long legalPersonId, string dic, string ic)
        {
            var dublicateEntities = _finder.Find<LegalPerson>(x => x.Id != legalPersonId)
                                           .Where(x => x.Inn.Trim().Equals(dic, StringComparison.OrdinalIgnoreCase) ||
                                                       x.Ic.Trim().Equals(ic, StringComparison.OrdinalIgnoreCase))
                                           .ToArray();

            var result = new CheckForDublicatesResultDto
        {
                ActiveDublicateExists = dublicateEntities.Any(x => x.IsActive && !x.IsDeleted),
                InactiveDublicateExists = dublicateEntities.Any(x => !x.IsActive && !x.IsDeleted),
                DeletedDublicateExists = dublicateEntities.Any(x => x.IsDeleted)
            };

            return result;
        }

        public CheckForDublicatesResultDto CheckIfExistsPassportDuplicate(long legalPersonId, string passportSeries, string passportNumber)
        {
            var dublicateEntities = _finder.Find<LegalPerson>(x => x.Id != legalPersonId)
                                           .Where(x => x.PassportSeries.Trim().Equals(passportSeries, StringComparison.OrdinalIgnoreCase) &&
                                                       x.PassportNumber.Trim().Equals(passportNumber, StringComparison.OrdinalIgnoreCase))
                                           .ToArray();

            var result = new CheckForDublicatesResultDto
        {
                    ActiveDublicateExists = dublicateEntities.Any(x => x.IsActive && !x.IsDeleted),
                    InactiveDublicateExists = dublicateEntities.Any(x => !x.IsActive && !x.IsDeleted),
                    DeletedDublicateExists = dublicateEntities.Any(x => x.IsDeleted)
                };

            return result;
        }

        public string[] SelectNotUnique1CSyncCodes(IEnumerable<string> codes)
        {
            return _finder.Find<Account>(x => x.IsActive && !x.IsDeleted && codes.Contains(x.LegalPesonSyncCode1C))
                .GroupBy(x => x.LegalPesonSyncCode1C)
                          .Where(x => x.Distinct().Count() > 1)
                          .Select(x => x.Key)
                          .ToArray();
        }

        public LegalPerson FindLegalPerson(long entityId)
        {
            return _secureFinder.Find<LegalPerson>(l => l.Id == entityId).SingleOrDefault();
        }

        public void CreateOrUpdate(LegalPersonProfile legalPersonProfile)
        {
            if (legalPersonProfile.IsNew())
            {
                _identityProvider.SetFor(legalPersonProfile);
                _legalPersonProfileGenericRepository.Add(legalPersonProfile);
            }
            else
            {
                _legalPersonProfileGenericRepository.Update(legalPersonProfile);
            }

            _legalPersonProfileGenericRepository.Save();
        }

        public int Delete(LegalPersonProfile legalPersonProfile)
        {
            legalPersonProfile.IsActive = false;
            _legalPersonProfileGenericRepository.Update(legalPersonProfile);
            return _legalPersonProfileGenericRepository.Save();
        }

        public void SetProfileAsMain(long profileId)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<UpdateIdentity, LegalPersonProfile>())
            {
                var legalPersonProfiles = _finder.Find(Specs.Find.ById<LegalPersonProfile>(profileId))
                                                 .SelectMany(x => x.LegalPerson.LegalPersonProfiles)
                                                 .ToArray();

                foreach (var profile in legalPersonProfiles)
                {
                    profile.IsMainProfile = profile.Id == profileId;

                    _legalPersonProfileGenericRepository.Update(profile);
                    operationScope.Updated<LegalPersonProfile>(profile.Id);
                }

                _legalPersonProfileGenericRepository.Save();
                operationScope.Complete();
            }
        }

        [Obsolete("Перенести в ILegalPersonReadModel + учесть разные бизнес-модели")]
        public LegalPersonWithProfiles GetLegalPersonWithProfiles(long legalPersonId)
        {
            return _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId))
                          .Select(x => new LegalPersonWithProfiles
                              {
                                  LegalPerson = x,
                                  Profiles = x.LegalPersonProfiles.Where(y => y.IsActive && !y.IsDeleted)
                              })
                          .SingleOrDefault();
        }

        public LegalPerson FindLegalPersonByProfile(long profileId)
        {
            return _finder.Find(Specs.Find.ById<LegalPersonProfile>(profileId)).Select(x => x.LegalPerson).SingleOrDefault();
        }

        public LegalPerson FindLegalPerson(string syncCodeWith1C, string innOrPassportSeries, string kppOrPassportNumber)
        {
            var legalPersons = _finder.Find<LegalPerson>(x =>
            x.LegalPersonTypeEnum == (int)LegalPersonType.LegalPerson ? (x.Inn == innOrPassportSeries && x.Kpp == kppOrPassportNumber) :
            x.LegalPersonTypeEnum == (int)LegalPersonType.Businessman ? (x.Inn == innOrPassportSeries) :
            x.LegalPersonTypeEnum == (int)LegalPersonType.NaturalPerson && (x.PassportSeries == innOrPassportSeries && x.PassportNumber == kppOrPassportNumber))
            .Where(x => x.Accounts.Any(y => y.LegalPesonSyncCode1C == syncCodeWith1C))
            .ToArray();

            if (legalPersons.Length == 0)
            {
                return null;
            }

            if (legalPersons.Length > 1)
            {
                throw new ApplicationException(string.Format("Найдено несколько юр. лиц клиентов с SyncCode1C='{0}', InnOrPassport='{1}', KppOrPassport='{2}'", syncCodeWith1C, innOrPassportSeries, kppOrPassportNumber));
            }

            return legalPersons[0];
        }

        public IEnumerable<LegalPerson> FindLegalPersonsByInnAndKpp(string inn, string kpp)
        {
            return _finder
                        .Find(Specs.Find.ActiveAndNotDeleted<LegalPerson>() && LegalPersonSpecs.LegalPersons.Find.ByInnAndKpp(inn, kpp))
                        .ToArray();
        }

        public IEnumerable<LegalPerson> FindBusinessmenByInn(string inn)
        {
            return _finder
                        .Find(Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                                && LegalPersonSpecs.LegalPersons.Find.OfType(LegalPersonType.Businessman)
                                && LegalPersonSpecs.LegalPersons.Find.ByInn(inn))
                        .ToArray();
        }

        public IEnumerable<LegalPerson> FindNaturalPersonsByPassport(string passportSeries, string passportNumber)
        {
            return _finder
                        .Find(Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                                && LegalPersonSpecs.LegalPersons.Find.OfType(LegalPersonType.NaturalPerson)
                                && LegalPersonSpecs.LegalPersons.Find.ByPassport(passportSeries, passportNumber))
                        .ToArray();
        }

        public IEnumerable<LegalPerson> FindLegalPersons(string syncCodeWith1C, long branchOfficeOrganizationUnitId)
        {
            return _finder.Find<Account>(
                x => x.IsActive && !x.IsDeleted && x.LegalPesonSyncCode1C == syncCodeWith1C &&
                     x.BranchOfficeOrganizationUnitId == branchOfficeOrganizationUnitId)
                          .Select(x => x.LegalPerson).ToArray();
        }

        public LegalPersonName GetLegalPersonNameByClientId(long clientId)
        {
            var legalPersonInfos = _finder.Find(Specs.Find.ById<Client>(clientId) && Specs.Find.ActiveAndNotDeleted<Client>())
                                          .SelectMany(client => client.LegalPersons)
                                          .Where(Specs.Find.ActiveAndNotDeleted<LegalPerson>())
                .Select(x => new LegalPersonName { Id = x.Id, Name = x.LegalName })
                                          .Take(2)
                                          .ToArray();

            if (legalPersonInfos.Length != 1)
            {
                return null;
            }

            return legalPersonInfos.Single();
        }

        public IEnumerable<LegalPersonFor1CExportDto> GetLegalPersonsForExportTo1C(long organizationUnitId, DateTime startPeriod)
        {
            return _finder.Find<LegalPersonProfile>(x => x.IsActive && !x.IsDeleted &&
                                                         x.IsMainProfile &&
                                                         x.LegalPerson.IsActive && !x.LegalPerson.IsDeleted &&
                                                         (x.LegalPerson.ModifiedOn >= startPeriod || x.ModifiedOn >= startPeriod))
                          .SelectMany(x => x.LegalPerson.Accounts
                                            .Where(y => y.IsActive && !y.IsDeleted && y.BranchOfficeOrganizationUnit.OrganizationUnitId == organizationUnitId)
                                            .Select(z => new LegalPersonFor1CExportDto
                                                {
                                                    Profile = x,
                                                    LegalPerson = x.LegalPerson,
                                                    LegalPersonSyncCode1C = z.LegalPesonSyncCode1C,
                                                }))
                          .ToArray();
        }

        public int AssignWithRelatedEntities(long legalPersonId, long ownerCode, bool isPartialAssign)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, LegalPerson>())
            {
                var legalPersonWithRelatedEntities = GetLegalPersonWithRelatedEntitiesForAssign(legalPersonId, isPartialAssign);

                legalPersonWithRelatedEntities.LegalPerson.OwnerCode = ownerCode;
                _legalPersonGenericRepository.Update(legalPersonWithRelatedEntities.LegalPerson);
                operationScope.Updated<LegalPerson>(legalPersonWithRelatedEntities.LegalPerson.Id);
                var count = _legalPersonGenericRepository.Save();

                foreach (var profile in legalPersonWithRelatedEntities.Profiles)
                {
                    profile.OwnerCode = ownerCode;
                    _legalPersonProfileGenericRepository.Update(profile);
                    operationScope.Updated<LegalPersonProfile>(profile.Id);
                }

                _legalPersonProfileGenericRepository.Save();

                foreach (var bargain in legalPersonWithRelatedEntities.Bargains)
                {
                    bargain.OwnerCode = ownerCode;
                    _bargainGenericRepository.Update(bargain);
                    operationScope.Updated<Bargain>(bargain.Id);
                }

                _bargainGenericRepository.Save();

                foreach (var order in legalPersonWithRelatedEntities.Orders)
                {
                    order.OwnerCode = ownerCode;
                    _orderGenericRepository.Update(order);
                    operationScope.Updated<Order>(order.Id);
                }

                _orderGenericRepository.Save();

                foreach (var account in legalPersonWithRelatedEntities.Accounts)
                {
                    account.OwnerCode = ownerCode;
                    _accountGenericRepository.Update(account);
                    operationScope.Updated<Account>(account.Id);
                }

                _accountGenericRepository.Save();

                foreach (var limit in legalPersonWithRelatedEntities.Limits)
                {
                    limit.OwnerCode = ownerCode;
                    _limitGenericRepository.Update(limit);
                    operationScope.Updated<Limit>(limit.Id);
                }

                _limitGenericRepository.Save();

                operationScope.Complete();

                return count;
            }
        }

        int IDeleteAggregateRepository<LegalPerson>.Delete(long entityId)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<LegalPerson>(entityId)).Single();
            return Delete(entity);
        }

        int IAssignAggregateRepository<LegalPerson>.Assign(long entityId, long ownerCode)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<LegalPerson>(entityId)).Single();
            return Assign(entity, ownerCode);
        }

        int IActivateAggregateRepository<LegalPerson>.Activate(long entityId)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<LegalPerson>(entityId)).Single();
            return Activate(entity);
        }

        int IDeactivateAggregateRepository<LegalPerson>.Deactivate(long entityId)
        {
            var entity = _secureFinder.Find(Specs.Find.ById<LegalPerson>(entityId)).Single();
            return Deactivate(entity);
        }

        int IDeleteAggregateRepository<LegalPersonProfile>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<LegalPersonProfile>(entityId)).Single();
            return Delete(entity);
        }

        void ICheckAggregateForDebtsRepository<LegalPerson>.CheckForDebts(long entityId, long currentUserCode, bool bypassValidation)
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

            var accountWithDebts = (from legalPerson in _finder.FindAll<LegalPerson>()
                                    where legalPerson.Id == entityId
                                    from account in legalPerson.Accounts
                                    let lockDetailBalance = account.Balance - (account.Locks                                    // скобки и проверки на null тут НУЖНЫ,
                                                                                   .Where(x => x.IsActive && !x.IsDeleted)      // т.к. без них возможна ситуация decimal - null = null
                                                                                   .Sum(x => (decimal?)x.PlannedAmount) ?? 0)
                                    where lockDetailBalance <= _debtProcessingSettings.MinDebtAmount
                                    select new AccountWithDebtInfo
                                        {
                                            LegalPersonName = legalPerson.ShortName,
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

        ChangeAggregateClientValidationResult IChangeAggregateClientRepository<LegalPerson>.Validate(long entityId, long currentUserCode, long reserveCode)
        {
            var warnings = new List<string>();
            var securityErrors = new List<string>();
            var domainErrors = new List<string>();
            var result = new ChangeAggregateClientValidationResult(warnings, securityErrors, domainErrors);

            var functionalPrivilegeGranted =
                _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonChangeClient, currentUserCode);
            if (!functionalPrivilegeGranted)
            {
                securityErrors.Add(BLResources.NoFunctionalPrivilegeForChangingClientForLegalPerson);
            }

            var ownerCode = _finder.Find(Specs.Find.ById<LegalPerson>(entityId)).Select(x => x.OwnerCode).Single();
            var permissions = _entityAccessService.RestrictEntityAccess(EntityName.LegalPerson,
                                                                        EntityAccessTypes.All,
                                                                        currentUserCode,
                                                                        entityId,
                                                                        ownerCode,
                                                                        null);
            if (!permissions.HasFlag(EntityAccessTypes.Assign))
            {
                throw new SecurityException(BLResources.ChangeLegalPersonClientAccessDenied);
            }

            return result;
        }

        int IChangeAggregateClientRepository<LegalPerson>.ChangeClient(long entityId, long clientId, long currentUserCode, bool bypassValidation)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var legalPerson = _secureFinder.Find(Specs.Find.ById<LegalPerson>(entityId)).Single();
                var clientOwnerCode = _finder.Find(Specs.Find.ById<Client>(clientId)).Select(x => x.OwnerCode).Single();

                if (legalPerson.OwnerCode != clientOwnerCode)
                {
                    var checkAggregateForDebtsRepository = this as ICheckAggregateForDebtsRepository<LegalPerson>;
                    checkAggregateForDebtsRepository.CheckForDebts(entityId, currentUserCode, bypassValidation);

                    AssignWithRelatedEntities(legalPerson.Id, clientOwnerCode, true);
                }

                // Подтягиваем измененную сущность.
                legalPerson = _secureFinder.Find(Specs.Find.ById<LegalPerson>(entityId)).Single();

                legalPerson.ClientId = clientId;
                _legalPersonGenericRepository.Update(legalPerson);

                var count = _legalPersonGenericRepository.Save();

                transaction.Complete();
                return count;
            }
        }

        private LegalPersonWithRelatedEntitiesForAssignDto GetLegalPersonWithRelatedEntitiesForAssign(long legalPersonId, bool isPartialAssign)
        {
            return (from legalPerson in _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId))
                    let legalPersonPrevOwner = isPartialAssign ? legalPerson.OwnerCode : (long?)null
                    select new LegalPersonWithRelatedEntitiesForAssignDto
                        {
                            LegalPerson = legalPerson,
                            Accounts = legalPerson.Accounts.Where(x => (legalPersonPrevOwner == null || x.OwnerCode == legalPersonPrevOwner)),
                            Bargains = legalPerson.Bargains.Where(x => x.IsActive && (legalPersonPrevOwner == null || x.OwnerCode == legalPersonPrevOwner)),
                            Orders = legalPerson.Orders.Where(x => x.IsActive && !x.IsDeleted &&
                                                                   (legalPersonPrevOwner == null || x.OwnerCode == legalPersonPrevOwner)),
                            Profiles = legalPerson.LegalPersonProfiles.Where(x => x.IsActive &&
                                                                                  (legalPersonPrevOwner == null || x.OwnerCode == legalPersonPrevOwner)),
                            Limits = from acc in legalPerson.Accounts
                                     from limit in acc.Limits
                                     where limit.IsActive && !limit.IsDeleted && (legalPersonPrevOwner == null || limit.OwnerCode == legalPersonPrevOwner)
                                     select limit
                        })
                .Single();
        }

        private class LegalPersonWithRelatedEntitiesForAssignDto
        {
            public LegalPerson LegalPerson { get; set; }
            public IEnumerable<LegalPersonProfile> Profiles { get; set; }
            public IEnumerable<Order> Orders { get; set; }
            public IEnumerable<Account> Accounts { get; set; }
            public IEnumerable<Limit> Limits { get; set; }
            public IEnumerable<Bargain> Bargains { get; set; }
        }
    }
}
