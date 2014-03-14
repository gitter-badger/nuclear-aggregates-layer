﻿using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Aggregates.Users.ReadModel
{
    public interface IUserReadModel : IAggregateReadModel<User>
    {
        User GetUser(long id);
        User FindAnyUserWithPrivelege(IEnumerable<long> organizationUnitId, FunctionalPrivilegeName privelege);
        User GetNotServiceUser(long userId);
        User GetOrganizationUnitDirector(long organizationUnitId);
    }
}
