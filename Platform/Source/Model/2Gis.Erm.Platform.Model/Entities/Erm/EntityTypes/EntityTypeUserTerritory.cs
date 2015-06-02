﻿using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeUserTerritory : EntityTypeBase<EntityTypeUserTerritory>
    {
        public override int Id
        {
            get { return EntityTypeIds.UserTerritory; }
        }

        public override string Description
        {
            get { return "UserTerritory"; }
        }
    }
}