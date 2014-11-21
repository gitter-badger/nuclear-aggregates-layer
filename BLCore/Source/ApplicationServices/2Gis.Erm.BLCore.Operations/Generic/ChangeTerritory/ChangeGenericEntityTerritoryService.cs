﻿using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeTerritory;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.ChangeTerritory
{
    public class ChangeGenericEntityTerritoryService<TEntity> : IChangeGenericEntityTerritoryService<TEntity> where TEntity : class, IEntityKey
    {
        public void ChangeTerritory(long entityId, long territoryId)
        {
            throw new NotSupportedException("Change territory operation is supported by Firm and Client only");
        }
    }
}