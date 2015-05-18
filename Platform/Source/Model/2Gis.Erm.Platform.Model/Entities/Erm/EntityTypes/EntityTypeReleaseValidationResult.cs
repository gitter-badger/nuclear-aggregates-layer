﻿using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeReleaseValidationResult : EntityTypeBase<EntityTypeReleaseValidationResult>
    {
        public override int Id
        {
            get { return EntityTypeIds.ReleaseValidationResult; }
        }

        public override string Description
        {
            get { return "ReleaseValidationResult"; }
        }
    }
}