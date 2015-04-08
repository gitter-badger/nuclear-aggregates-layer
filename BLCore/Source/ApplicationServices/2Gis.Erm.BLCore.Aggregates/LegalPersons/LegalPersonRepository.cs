using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
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
            _scopeFactory = scopeFactory;
        }

        public int Activate(LegalPerson legalPerson)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<ActivateIdentity, LegalPerson>())
            {
                var profiles = _finder.FindMany(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(legalPerson.Id) && Specs.Find.NotDeleted<LegalPersonProfile>());
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

            // Изменения логируются в вызывающем коде
            _legalPersonGenericRepository.Update(legalPerson);
            return _legalPersonGenericRepository.Save();
        }

        public void ChangeLegalRequisites(LegalPerson legalPerson, string inn, string kpp, string legalAddress)
        {
            legalPerson.Inn = inn;
            legalPerson.Kpp = kpp;
            legalPerson.LegalAddress = legalAddress;

            // Изменения логируются в вызывающем коде
            _legalPersonGenericRepository.Update(legalPerson);
            _legalPersonGenericRepository.Save();
        }

        public void ChangeNaturalRequisites(LegalPerson legalPerson, string passportSeries, string passportNumber, string registrationAddress)
        {
            legalPerson.PassportNumber = passportNumber;
            legalPerson.PassportSeries = passportSeries;
            legalPerson.RegistrationAddress = registrationAddress;

            // Изменения логируются в вызывающем коде
            _legalPersonGenericRepository.Update(legalPerson);
            _legalPersonGenericRepository.Save();
        }

        public int Deactivate(LegalPerson legalPerson)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, LegalPerson>())
            {
                var profiles = _finder.FindMany(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(legalPerson.Id));
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

        [Obsolete("Используется только в ExportLegalPersonsHandler")]
        public void SyncWith1C(IEnumerable<LegalPerson> legalPersons)
        {
            foreach (var legalPerson in legalPersons)
            {
                legalPerson.IsInSyncWith1C = true;
                _legalPersonGenericRepository.Update(legalPerson);
            }

            _legalPersonGenericRepository.Save();
        }

        public CheckForDublicatesResultDto CheckIfExistsInnDuplicate(long legalPersonId, string inn)
        {
            var dublicateEntities = _finder.FindMany(Specs.Find.ExceptById<LegalPerson>(legalPersonId) &&
                                                     LegalPersonSpecs.LegalPersons.Find.ByInnTrimmed(inn));
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
            var dublicateEntities = _finder.FindMany(Specs.Find.ExceptById<LegalPerson>(legalPersonId) &&
                                                     LegalPersonSpecs.LegalPersons.Find.ByInnAndKppTrimmed(inn, kpp));
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
            var dublicateEntities = _finder.FindMany(Specs.Find.ExceptById<LegalPerson>(legalPersonId) &&
                                                     LegalPersonSpecs.LegalPersons.Find.ByIcOrDicTrimmed(ic, dic));

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
            var dublicateEntities = _finder.FindMany(Specs.Find.ExceptById<LegalPerson>(legalPersonId) &&
                                                     LegalPersonSpecs.LegalPersons.Find.ByPassportTrimmed(passportSeries, passportNumber));

            var result = new CheckForDublicatesResultDto
                {
                    ActiveDublicateExists = dublicateEntities.Any(x => x.IsActive && !x.IsDeleted),
                    InactiveDublicateExists = dublicateEntities.Any(x => !x.IsActive && !x.IsDeleted),
                    DeletedDublicateExists = dublicateEntities.Any(x => x.IsDeleted)
                };

            return result;
        }

        public LegalPerson FindLegalPerson(long entityId)
        {
            return _secureFinder.FindOne(Specs.Find.ById<LegalPerson>(entityId));
        }

        public void SetProfileAsMain(long profileId)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<UpdateIdentity, LegalPersonProfile>())
            {
                var legalPersonId = _finder.Find(Specs.Find.ById<LegalPersonProfile>(profileId)).Select(profile => profile.LegalPersonId).Single();
                var legalPersonProfiles = _finder.FindMany(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(legalPersonId));

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

        public LegalPerson FindLegalPersonByProfile(long profileId)
        {
            var ids = _finder.Find(Specs.Find.ById<LegalPersonProfile>(profileId)).Select(x => x.LegalPersonId).ToArray();
            if (ids.Length != 1)
            {
                return null;
            }

            return _finder.FindOne(Specs.Find.ById<LegalPerson>(ids.Single()));
        }

        public LegalPerson FindLegalPerson(string syncCodeWith1C, string innOrPassportSeries, string kppOrPassportNumber)
        {
            var accountIds = _finder.Find<Account>(y => y.LegalPesonSyncCode1C == syncCodeWith1C).Select(y => y.LegalPersonId).ToArray();

            var legalPersons = _finder.FindMany(Specs.Find.ByIds<LegalPerson>(accountIds) &&
                                                (LegalPersonSpecs.LegalPersons.Find.LegalPersonByInnAndKpp(innOrPassportSeries, kppOrPassportNumber) ||
                                                 LegalPersonSpecs.LegalPersons.Find.BusinessmanByInn(innOrPassportSeries) ||
                                                 LegalPersonSpecs.LegalPersons.Find.NaturalPersonByPassport(innOrPassportSeries, kppOrPassportNumber)));

            if (legalPersons.Count() > 1)
            {
                throw new ApplicationException(string.Format("Найдено несколько юр. лиц клиентов с SyncCode1C='{0}', InnOrPassport='{1}', KppOrPassport='{2}'", syncCodeWith1C, innOrPassportSeries, kppOrPassportNumber));
            }

            return legalPersons.SingleOrDefault();
        }

        public IEnumerable<LegalPerson> FindLegalPersonsByInnAndKpp(string inn, string kpp)
        {
            return _finder.FindMany(Specs.Find.ActiveAndNotDeleted<LegalPerson>() && LegalPersonSpecs.LegalPersons.Find.ByInnAndKpp(inn, kpp));
        }

        public IEnumerable<LegalPerson> FindBusinessmenByInn(string inn)
        {
            return _finder.FindMany(Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                                    && LegalPersonSpecs.LegalPersons.Find.BusinessmanByInn(inn));
        }

        public IEnumerable<LegalPerson> FindNaturalPersonsByPassport(string passportSeries, string passportNumber)
        {
            return _finder.FindMany(Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                                    && LegalPersonSpecs.LegalPersons.Find.NaturalPersonByPassport(passportSeries, passportNumber));
        }

        public IEnumerable<LegalPerson> FindLegalPersons(string syncCodeWith1C, long branchOfficeOrganizationUnitId)
        {
            var ids = _finder.Find<Account>(x => x.IsActive && !x.IsDeleted
                                              && x.LegalPesonSyncCode1C == syncCodeWith1C
                                              && x.BranchOfficeOrganizationUnitId == branchOfficeOrganizationUnitId)
                          .Select(x => x.LegalPersonId)
                          .ToArray();

            return _finder.FindMany(Specs.Find.ByIds<LegalPerson>(ids));
        }

        public IEnumerable<LegalPersonFor1CExportDto> GetLegalPersonsForExportTo1C(long organizationUnitId, DateTime startPeriod)
        {
            var data = _finder.Find<LegalPersonProfile>(x => x.IsActive && !x.IsDeleted &&
                                                             x.IsMainProfile &&
                                                             x.LegalPerson.IsActive && !x.LegalPerson.IsDeleted &&
                                                             (x.LegalPerson.ModifiedOn >= startPeriod || x.ModifiedOn >= startPeriod))
                              .SelectMany(x => x.LegalPerson.Accounts
                                                .Where(y => y.IsActive && !y.IsDeleted && y.BranchOfficeOrganizationUnit.OrganizationUnitId == organizationUnitId)
                                                .Select(z => new
                                                    {
                                                        ProfileId = x.Id,
                                                        LegalPersonId = x.LegalPersonId,
                                                        LegalPersonSyncCode1C = z.LegalPesonSyncCode1C,
                                                    }))
                              .ToArray();

            var legalPersons = _finder.FindMany(Specs.Find.ByIds<LegalPerson>(data.Select(x => x.LegalPersonId)))
                                      .ToDictionary(x => x.Id, x => x);
            var legalPersonProfiles = _finder.FindMany(Specs.Find.ByIds<LegalPersonProfile>(data.Select(x => x.ProfileId)))
                                      .ToDictionary(x => x.Id, x => x);

            return data.Select(x => new LegalPersonFor1CExportDto
                {
                    LegalPerson = legalPersons[x.LegalPersonId],
                    Profile = legalPersonProfiles[x.ProfileId],
                    LegalPersonSyncCode1C = x.LegalPersonSyncCode1C,
                });
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

        int IAssignAggregateRepository<LegalPerson>.Assign(long entityId, long ownerCode)
        {
            var entity = _secureFinder.FindOne(Specs.Find.ById<LegalPerson>(entityId));
            return Assign(entity, ownerCode);
        }

        int IActivateAggregateRepository<LegalPerson>.Activate(long entityId)
        {
            var entity = _secureFinder.FindOne(Specs.Find.ById<LegalPerson>(entityId));
            return Activate(entity);
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
            DebtsAuditor.ThrowIfAnyError(accountWithDebts);

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
                var legalPerson = _secureFinder.FindOne(Specs.Find.ById<LegalPerson>(entityId));
                var clientOwnerCode = _finder.Find(Specs.Find.ById<Client>(clientId)).Select(x => x.OwnerCode).Single();

                if (legalPerson.OwnerCode != clientOwnerCode)
                {
                    var checkAggregateForDebtsRepository = this as ICheckAggregateForDebtsRepository<LegalPerson>;
                    checkAggregateForDebtsRepository.CheckForDebts(entityId, currentUserCode, bypassValidation);

                    AssignWithRelatedEntities(legalPerson.Id, clientOwnerCode, true);
                }

                // Подтягиваем измененную сущность.
                legalPerson = _secureFinder.FindOne(Specs.Find.ById<LegalPerson>(entityId));

                legalPerson.ClientId = clientId;

                // Изменения логируются в вызывающем коде
                _legalPersonGenericRepository.Update(legalPerson);

                var count = _legalPersonGenericRepository.Save();

                transaction.Complete();
                return count;
            }
        }

        private LegalPersonWithRelatedEntitiesForAssignDto GetLegalPersonWithRelatedEntitiesForAssign(long legalPersonId, bool isPartialAssign)
        {
            var data = (from legalPerson in _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId))
                        let legalPersonPrevOwner = isPartialAssign ? legalPerson.OwnerCode : (long?)null
                        select new
                            {
                                LegalPersonId = legalPerson.Id,
                                Accounts = legalPerson.Accounts.Where(x => (legalPersonPrevOwner == null || x.OwnerCode == legalPersonPrevOwner)),
                                Bargains = legalPerson.Bargains.Where(x => x.IsActive && (legalPersonPrevOwner == null || x.OwnerCode == legalPersonPrevOwner)),
                                Orders = legalPerson.Orders.Where(x => x.IsActive && !x.IsDeleted &&
                                                                       (legalPersonPrevOwner == null || x.OwnerCode == legalPersonPrevOwner)),
                                ProfilesIds = legalPerson.LegalPersonProfiles
                                                      .Where(x => x.IsActive && (legalPersonPrevOwner == null || x.OwnerCode == legalPersonPrevOwner))
                                                      .Select(x => x.Id),
                                Limits = from acc in legalPerson.Accounts
                                         from limit in acc.Limits
                                         where limit.IsActive && !limit.IsDeleted && (legalPersonPrevOwner == null || limit.OwnerCode == legalPersonPrevOwner)
                                         select limit
                            })
                .Single();

            return new LegalPersonWithRelatedEntitiesForAssignDto
                {
                    LegalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(data.LegalPersonId)),
                    Profiles = _finder.FindMany(Specs.Find.ByIds<LegalPersonProfile>(data.ProfilesIds)),

                    Accounts = data.Accounts,
                    Bargains = data.Bargains,
                    Orders = data.Orders,
                    Limits = data.Limits,
                };
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
