﻿using System;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class EntityTypeNameIdentity : EntityPropertyIdentityBase<EntityTypeNameIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.EntityTypeNamePropertyId; }
        }

        public override string Description
        {
            get { return "EntityTypeName"; }
        }

        public override string PropertyName
        {
            get { return "EntityTypeName"; }
        }

        public override Type PropertyType
        {
            get { return typeof(EntityName); }
        }
    }
}