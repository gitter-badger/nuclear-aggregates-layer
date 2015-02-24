using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.LegalPersons;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

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
            return _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId)).Select(x => x.LegalName).Single();
        }

        public PaymentMethod? GetPaymentMethod(long legalPersonId)
        {
            return _finder.Find(LegalPersonSpecs.Profiles.Find.MainByLegalPersonId(legalPersonId))
                          .Select(x => x.PaymentMethod)
                          .SingleOrDefault();
        }

        public string GetActiveLegalPersonNameWithSpecifiedInn(string inn)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                                && LegalPersonSpecs.LegalPersons.Find.ByInn(inn))
                          .Select(x => x.LegalName)
                          .FirstOrDefault();
        }

        public LegalPersonType GetLegalPersonType(long legalPersonId)
        {
            return _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId))
                          .Select(x => x.LegalPersonTypeEnum)
                          .SingleOrDefault();
        }


        public LegalPerson GetLegalPerson(long legalPersonId)
        {
            return _finder.FindOne(Specs.Find.ById<LegalPerson>(legalPersonId));
        }

        public IEnumerable<LegalPerson> GetLegalPersons(IEnumerable<long> legalPersonIds)
        {
            return _finder.FindMany(Specs.Find.ByIds<LegalPerson>(legalPersonIds));
        }

        public LegalPersonProfile GetLegalPersonProfile(long legalPersonProfileId)
        {
            return _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId));
        }

        public IEnumerable<LegalPersonProfile> GetProfilesByLegalPerson(long legalPersonId)
        {
            return _finder.FindMany(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(legalPersonId) && Specs.Find.ActiveAndNotDeleted<LegalPersonProfile>());
        }

        public bool HasAnyLegalPersonProfiles(long legalPersonId)
        {
            return _finder.Find(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(legalPersonId)).Any();
        }

        public IEnumerable<long> GetLegalPersonProfileIds(long legalPersonId)
        {
            return _finder.Find(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(legalPersonId) && Specs.Find.ActiveAndNotDeleted<LegalPersonProfile>())
                          .Select(profile => profile.Id)
                          .ToArray();
        }

        public long? GetMainLegalPersonProfileId(long legalPersonId)
        {
            return
                _finder.Find(LegalPersonSpecs.Profiles.Find.MainByLegalPersonId(legalPersonId)
                             && Specs.Find.ActiveAndNotDeleted<LegalPersonProfile>())
                       .Select(x => (long?)x.Id)
                       .SingleOrDefault();
        }

        public int? GetLegalPersonOrganizationDgppid(long legalPersonId)
        {
            return _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId))
                          .Select(x => x.Client.Territory.OrganizationUnit.DgppId)
                          .SingleOrDefault();
        }

        public bool DoesLegalPersonHaveActiveNotArchivedAndNotRejectedOrders(long legalPersonId)
        {
            return _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId))
                          .Select(x => x.Orders
                                        .Any(y => y.IsActive && !y.IsDeleted &&
                                                  y.WorkflowStepId != OrderState.Archive &&
                                                  y.WorkflowStepId != OrderState.Rejected))
                          .Single();
        }

        public IEnumerable<string> SelectNotUnique1CSyncCodes(IEnumerable<string> codes)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Account>() && AccountSpecs.Accounts.Find.ByLegalPersonSyncCodes1C(codes))
                          .GroupBy(x => x.LegalPesonSyncCode1C)
                          .Where(x => x.Distinct().Count() > 1)
                          .Select(x => x.Key)
                          .ToArray();
        }

        public LegalPersonAndProfilesExistanceDto GetLegalPersonWithProfileExistanceInfo(long legalPersonId)
        {
            return new LegalPersonAndProfilesExistanceDto
            {
                LegalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(legalPersonId)),
                LegalPersonHasProfiles =
                    _finder.Find(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(legalPersonId) && Specs.Find.ActiveAndNotDeleted<LegalPersonProfile>()).Any()
            };
        }

        public IEnumerable<LegalPersonAndProfilesExistanceDto> GetLegalPersonsWithProfileExistanceInfo(IEnumerable<long> legalPersonIds)
        {
            var legalPersons = GetLegalPersons(legalPersonIds);
            var profileExistance = _finder.Find(Specs.Find.ByIds<LegalPerson>(legalPersonIds))
                                          .Select(x => new
                                                           {
                                                               Id = x.Id,
                                                               HasProfiles = x.LegalPersonProfiles.Any(y => y.IsActive && !y.IsDeleted)
                                                           })
                                          .ToDictionary(x => x.Id, y => y.HasProfiles);

            return profileExistance.Select(x => new LegalPersonAndProfilesExistanceDto
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

        public IDictionary<long, IEnumerable<ValidateLegalPersonDto>> GetLegalPersonDtosToValidate(IEnumerable<long> organizationUnitIds,
                                                                                                   DateTime periodStartDate,
                                                                                                   DateTime periodEndDate)
        {
            return
                _finder.Find(AccountSpecs.Locks.Find.BySourceOrganizationUnits(organizationUnitIds) &&
                             AccountSpecs.Locks.Find.ForPeriod(periodStartDate, periodEndDate) &&
                             Specs.Find.NotDeleted<Lock>() &&
                             Specs.Find.InactiveEntities<Lock>())
                       .Select(x => new
                                        {
                                            x.Order.SourceOrganizationUnitId,
                                            LegalPersonDto = new ValidateLegalPersonDto
                                                                 {
                                                                     LegalPersonId = x.Order.LegalPersonId.Value,
                                                                     SyncCode1C = x.Account.LegalPesonSyncCode1C
                                                                 }
                                        })
                       .GroupBy(x => x.SourceOrganizationUnitId)
                       .ToDictionary(x => x.Key, y => y.Select(z => z.LegalPersonDto));
        }
    }
}