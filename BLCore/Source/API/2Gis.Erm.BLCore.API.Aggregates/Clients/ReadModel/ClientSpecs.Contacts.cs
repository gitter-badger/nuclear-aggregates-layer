using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel
{
    public static partial class ClientSpecs
    {
        public static class Contacts
        {
            public static class Find
            {
                public static FindSpecification<Contact> WithWorkEmail()
                {
                    return new FindSpecification<Contact>(x => x.WorkEmail != null);
                }

                public static FindSpecification<Contact> ByBirthDate(int month, int day)
                {
                    return new FindSpecification<Contact>(x => x.BirthDate.HasValue && x.BirthDate.Value.Month == month && x.BirthDate.Value.Day == day);
                }

                public static FindSpecification<Contact> ByClientId(long clientId)
                {
                    return new FindSpecification<Contact>(x => x.ClientId == clientId);
                }

                public static FindSpecification<Contact> IsNotFired()
                {
                    return new FindSpecification<Contact>(x => !x.IsFired);
                }

                public static FindSpecification<Contact> ByClientIds(IEnumerable<long?> clientAndChild)
                {
                    return new FindSpecification<Contact>(x => clientAndChild.Contains(x.ClientId));
                }
            }
        }
    }
}