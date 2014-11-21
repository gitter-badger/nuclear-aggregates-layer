﻿using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel
{
    public static partial class UserSpecs
    {
        public static class UserRoles
        {
            public static class Find
            {
                public static FindSpecification<UserRole> ForUser(long userId)
                {
                    return new FindSpecification<UserRole>(x => x.UserId == userId);
                }
            }
        }
    }
}