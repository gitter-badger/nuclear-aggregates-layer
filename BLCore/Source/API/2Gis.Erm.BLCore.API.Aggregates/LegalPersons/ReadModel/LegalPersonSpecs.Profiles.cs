using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel
{
    public static partial class LegalPersonSpecs
    {
        public static class Profiles
        {
            public static class Find
            {
                public static FindSpecification<LegalPersonProfile> MainByLegalPersonId(long legalPersonId)
                {
                    return new FindSpecification<LegalPersonProfile>(x => x.IsMainProfile && x.LegalPersonId == legalPersonId);
                }

                public static FindSpecification<LegalPersonProfile> ByLegalPersonId(long legalPersonId)
                {
                    return new FindSpecification<LegalPersonProfile>(x => x.LegalPersonId == legalPersonId);
                }

                public static FindSpecification<LegalPersonProfile> DuplicateByName(long legalPersonProfileId, string name)
                {
                    return new FindSpecification<LegalPersonProfile>(x => x.Id != legalPersonProfileId && x.Name == name);
                }
            }
        }
    }
}