﻿using DoubleGis.Erm.Platform.DAL.Specifications;
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
                    return new FindSpecification<Contact>(x => x.WorkAddress != null);
                }

                public static FindSpecification<Contact> ByBirthDate(int month, int day)
                {
                    return new FindSpecification<Contact>(x => x.BirthDate.HasValue && x.BirthDate.Value.Month == month && x.BirthDate.Value.Day == day);
                }
            }
        }
    }
}