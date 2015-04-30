using System;
using System.Linq;

using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel
{
    public static partial class LegalPersonSpecs
    {
        public static class LegalPersons
        {
            public static class Find
            {
                public static FindSpecification<LegalPerson> OfType(LegalPersonType legalPersonType)
                {
                    return new FindSpecification<LegalPerson>(x => x.LegalPersonTypeEnum == legalPersonType);
                }

                public static FindSpecification<LegalPerson> ByInnAndKpp(string inn, string kpp)
                {
                    return new FindSpecification<LegalPerson>(x => x.Inn == inn && x.Kpp == kpp);
                }

                public static FindSpecification<LegalPerson> ByInnAndKppTrimmed(string inn, string kpp)
                {
                    return new FindSpecification<LegalPerson>(x => x.Inn.Trim().Equals(inn, StringComparison.OrdinalIgnoreCase) &&
                                                                   x.Kpp.Trim().Equals(kpp, StringComparison.OrdinalIgnoreCase));
                }

                public static FindSpecification<LegalPerson> ByIcOrDicTrimmed(string ic, string dic)
                {
                    return new FindSpecification<LegalPerson>(x => x.Inn.Trim().Equals(dic, StringComparison.OrdinalIgnoreCase) ||
                                                                   x.Ic.Trim().Equals(ic, StringComparison.OrdinalIgnoreCase));
                }

                public static FindSpecification<LegalPerson> ByInn(string inn)
                {
                    return new FindSpecification<LegalPerson>(x => x.Inn == inn);
                }

                public static FindSpecification<LegalPerson> ByInnTrimmed(string inn)
                {
                    return new FindSpecification<LegalPerson>(x => x.Inn.Trim().Equals(inn, StringComparison.OrdinalIgnoreCase));
                }

                public static FindSpecification<LegalPerson> ByPassport(string passportSeries, string passportNumber)
                {
                    return new FindSpecification<LegalPerson>(x => x.PassportSeries == passportSeries && x.PassportNumber == passportNumber);
                }

                public static FindSpecification<LegalPerson> ByPassportTrimmed(string passportSeries, string passportNumber)
                {
                    return new FindSpecification<LegalPerson>(x => x.PassportSeries.Trim().Equals(passportSeries, StringComparison.OrdinalIgnoreCase) &&
                                                                   x.PassportNumber.Trim().Equals(passportNumber, StringComparison.OrdinalIgnoreCase));
                }

                public static FindSpecification<LegalPerson> LegalPersonByInnAndKpp(string inn, string kpp)
                {
                    return new FindSpecification<LegalPerson>(x => x.LegalPersonTypeEnum == LegalPersonType.LegalPerson &&
                                                                   x.Inn == inn &&
                                                                   x.Kpp == kpp);
                }

                public static FindSpecification<LegalPerson> BusinessmanByInn(string inn)
                {
                    return new FindSpecification<LegalPerson>(x => x.LegalPersonTypeEnum == LegalPersonType.Businessman &&
                                                                   x.Inn == inn);
                }

                public static FindSpecification<LegalPerson> NaturalPersonByPassport(string passportSeries, string passportNumber)
                {
                    return new FindSpecification<LegalPerson>(x => x.LegalPersonTypeEnum == LegalPersonType.NaturalPerson &&
                                                                   x.PassportSeries == passportSeries &&
                                                                   x.PassportNumber == passportNumber);
                }

                public static FindSpecification<LegalPerson> InSyncWith1C()
                {
                    return new FindSpecification<LegalPerson>(x => x.IsInSyncWith1C);
                }

                public static FindSpecification<LegalPerson> WithoutActiveOrders()
                {
                    return new FindSpecification<LegalPerson>(x => !x.Orders.Any(y => y.IsActive && !y.IsDeleted));
                }

                public static FindSpecification<LegalPerson> WithActiveAndNotDeletedClient(long clientId)
                {
                    return new FindSpecification<LegalPerson>(x => x.Client.IsActive
                                                                   && !x.Client.IsDeleted
                                                                   && x.ClientId == clientId);
                }
            }
        }
    }
}