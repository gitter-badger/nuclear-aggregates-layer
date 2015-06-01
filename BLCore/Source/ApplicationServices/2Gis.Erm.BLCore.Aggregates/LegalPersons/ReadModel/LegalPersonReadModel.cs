using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.LegalPersons;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel
{
    public sealed class LegalPersonReadModel : ILegalPersonReadModel
    {
        private readonly IFinder _finder;

        public LegalPersonReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public string GetLegalPersonName(long legalPersonId)
        {
            return _finder.FindObsolete(Specs.Find.ById<LegalPerson>(legalPersonId)).Select(x => x.LegalName).Single();
        }

        public PaymentMethod? GetPaymentMethod(long legalPersonId)
        {
            return _finder.Find(LegalPersonSpecs.Profiles.Find.MainByLegalPersonId(legalPersonId))
                          .Map(q => q.Select(x => x.PaymentMethod))
                          .One();
        }

        public string GetActiveLegalPersonNameWithSpecifiedInn(string inn)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                                && LegalPersonSpecs.LegalPersons.Find.ByInn(inn))
                          .Map(q => q.Select(x => x.LegalName))
                          .Top();
        }

        public LegalPersonType GetLegalPersonType(long legalPersonId)
        {
            return _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId))
                          .Map(q => q.Select(x => x.LegalPersonTypeEnum))
                          .One();
        }

        public LegalPerson GetLegalPerson(long legalPersonId)
        {
            return _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId)).One();
        }

        public IEnumerable<LegalPerson> GetLegalPersons(IEnumerable<long> legalPersonIds)
        {
            return _finder.Find(Specs.Find.ByIds<LegalPerson>(legalPersonIds)).Many();
        }

        public LegalPersonProfile GetLegalPersonProfile(long legalPersonProfileId)
        {
            return _finder.Find(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId)).One();
        }

        public IEnumerable<LegalPersonProfile> GetProfilesByLegalPerson(long legalPersonId)
        {
            return _finder.Find(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(legalPersonId) && Specs.Find.ActiveAndNotDeleted<LegalPersonProfile>()).Many();
        }

        public bool HasAnyLegalPersonProfiles(long legalPersonId)
        {
            return _finder.Find(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(legalPersonId) && Specs.Find.ActiveAndNotDeleted<LegalPersonProfile>()).Any();
        }

        public IEnumerable<long> GetLegalPersonProfileIds(long legalPersonId)
        {
            return _finder.Find(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(legalPersonId) && Specs.Find.ActiveAndNotDeleted<LegalPersonProfile>())
                          .Map(q => q.Select(profile => profile.Id))
                          .Many();
        }

        public long? GetMainLegalPersonProfileId(long legalPersonId)
        {
            return
                _finder.Find(LegalPersonSpecs.Profiles.Find.MainByLegalPersonId(legalPersonId)
                             && Specs.Find.ActiveAndNotDeleted<LegalPersonProfile>())
                       .Map(q => q.Select(x => (long?)x.Id))
                       .One();
        }

        public int? GetLegalPersonOrganizationDgppid(long legalPersonId)
        {
            return _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId))
                          .Map(q => q.Select(x => x.Client.Territory.OrganizationUnit.DgppId))
                          .One();
        }

        public bool DoesLegalPersonHaveActiveNotArchivedAndNotRejectedOrders(long legalPersonId)
        {
            return _finder.FindObsolete(Specs.Find.ById<LegalPerson>(legalPersonId))
                          .Select(x => x.Orders
                                        .Any(y => y.IsActive && !y.IsDeleted &&
                                                  y.WorkflowStepId != OrderState.Archive &&
                                                  y.WorkflowStepId != OrderState.Rejected))
                          .Single();
        }

        public IEnumerable<string> SelectNotUnique1CSyncCodes(IEnumerable<string> codes)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Account>() && AccountSpecs.Accounts.Find.ByLegalPersonSyncCodes1C(codes))
                          .Map(q => q.GroupBy(x => x.LegalPesonSyncCode1C)
                                     .Where(x => x.Distinct().Count() > 1)
                                     .Select(x => x.Key))
                          .Many();
        }

        public LegalPersonAndProfilesExistenceDto GetLegalPersonWithProfileExistenceInfo(long legalPersonId)
        {
            return new LegalPersonAndProfilesExistenceDto
            {
                LegalPerson = _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId)).One(),
                LegalPersonHasProfiles =
                    _finder.Find(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(legalPersonId) && Specs.Find.ActiveAndNotDeleted<LegalPersonProfile>()).Any()
            };
        }

        public IEnumerable<LegalPersonAndProfilesExistenceDto> GetLegalPersonsWithProfileExistenceInfo(IEnumerable<long> legalPersonIds)
        {
            var legalPersons = GetLegalPersons(legalPersonIds);
            var profileExistence = _finder.Find(Specs.Find.ByIds<LegalPerson>(legalPersonIds))
                                          .Map(q => q.Select(x => new
                                              {
                                                  Id = x.Id,
                                                  HasProfiles = x.LegalPersonProfiles.Any(y => y.IsActive && !y.IsDeleted)
                                              }))
                                          .Map(x => x.Id, y => y.HasProfiles);

            return profileExistence.Select(x => new LegalPersonAndProfilesExistenceDto
                                                    {
                                                        LegalPersonHasProfiles = x.Value,
                                                        LegalPerson = legalPersons.Single(y => y.Id == x.Key)
                                                    })
                                   .ToArray();
        }

        public bool IsThereLegalPersonProfileDuplicate(long legalPersonProfileId, long legalPersonId, string name)
        {
            return _finder.Find(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(legalPersonId) && LegalPersonSpecs.Profiles.Find.DuplicateByName(legalPersonProfileId, name)).Any();
        }

        public IEnumerable<ValidateLegalPersonDto> GetLegalPersonDtosToValidateForWithdrawalOperation(
            long organizationUnitId,
            DateTime periodStartDate,
            DateTime periodEndDate)
        {
            return _finder.Find(AccountSpecs.Locks.Find.BySourceOrganizationUnit(organizationUnitId) &&
                                AccountSpecs.Locks.Find.ForPeriod(periodStartDate, periodEndDate) &&
                                Specs.Find.ActiveAndNotDeleted<Lock>())
                          .Map(q => q.Select(x =>
                                             new ValidateLegalPersonDto
                                                 {
                                                     LegalPersonId = x.Order.LegalPersonId.Value,
                                                     SyncCode1C = x.Account.LegalPesonSyncCode1C
                                                 }))
                          .Many();
        }
    }
}