﻿using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel
{
    public interface IOrganizationUnitReadModel : IAggregateReadModel<OrganizationUnit>
    {
        OrganizationUnit GetOrganizationUnit(long organizationUnitId);
        string GetName(long organizationUnitId);
        long GetCurrencyId(long organizationUnitId);
        IReadOnlyDictionary<int, long> GetOrganizationUnitIdsByDgppIds(IEnumerable<int> dgppIds);
    }
}