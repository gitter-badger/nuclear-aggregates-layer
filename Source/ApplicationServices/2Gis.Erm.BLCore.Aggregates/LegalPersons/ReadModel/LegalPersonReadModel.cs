﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs;
using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
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
                          .Select(x => (PaymentMethod?)x.PaymentMethod)
                          .SingleOrDefault();
        }

        public LegalPersonType GetLegalPersonType(long legalPersonId)
        {
            return _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId))
                          .Select(x => (LegalPersonType)x.LegalPersonTypeEnum)
                          .SingleOrDefault();
        }


        public LegalPerson GetLegalPerson(long legalPersonId)
        {
            return _finder.FindOne(Specs.Find.ById<LegalPerson>(legalPersonId));
        }

        public LegalPersonProfile GetLegalPersonProfile(long legalPersonProfileId)
        {
            return _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId));
        }

        public IEnumerable<LegalPersonProfile> GetProfilesByLegalPerson(long legalPersonId)
        {
            return _finder.FindMany(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(legalPersonId));
        }

        public bool HasAnyLegalPersonProfiles(long legalPersonId)
                          {
            return _finder.Find(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(legalPersonId)).Any();
        }
    }
}