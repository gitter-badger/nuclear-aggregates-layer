using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel
{
    public static partial class LegalPersonSpecs
    {
        public static class LegalPersons
        {
            public static class Find
            {
                public static FindSpecification<LegalPerson> OfType(LegalPersonType legalPersonType)
                {
                    return new FindSpecification<LegalPerson>(x => x.LegalPersonTypeEnum == (int)legalPersonType);
                }

                public static FindSpecification<LegalPerson> ByInnAndKpp(string inn, string kpp)
                {
                    return new FindSpecification<LegalPerson>(x => x.Inn == inn && x.Kpp == kpp);
                }

                public static FindSpecification<LegalPerson> ByInn(string inn)
                {
                    return new FindSpecification<LegalPerson>(x => x.Inn == inn);
                }

                public static FindSpecification<LegalPerson> ByPassport(string passportSeries, string passportNumber)
                {
                    return new FindSpecification<LegalPerson>(x => x.PassportSeries == passportSeries && x.PassportNumber == passportNumber);
                }

                public static FindSpecification<LegalPerson> InSyncWith1C()
                {
                    return new FindSpecification<LegalPerson>(x => x.IsInSyncWith1C);
                }

                public static FindSpecification<LegalPerson> WithoutActiveOrders()
                {
                    return new FindSpecification<LegalPerson>(x => !x.Orders.Any(y => y.IsActive && !y.IsDeleted));
                }
            }
        }
    }
}