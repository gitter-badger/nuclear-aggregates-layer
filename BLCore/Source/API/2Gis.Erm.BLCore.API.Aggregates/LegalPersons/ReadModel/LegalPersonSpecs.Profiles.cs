using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

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