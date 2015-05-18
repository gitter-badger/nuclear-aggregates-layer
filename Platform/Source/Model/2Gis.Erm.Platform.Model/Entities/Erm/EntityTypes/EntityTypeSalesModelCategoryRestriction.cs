﻿using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeSalesModelCategoryRestriction : EntityTypeBase<EntityTypeSalesModelCategoryRestriction>
    {
        public override int Id
        {
            get { return EntityTypeIds.SalesModelCategoryRestriction; }
        }

        public override string Description
        {
            get { return "SalesModelCategoryRestriction"; }
        }
    }
}