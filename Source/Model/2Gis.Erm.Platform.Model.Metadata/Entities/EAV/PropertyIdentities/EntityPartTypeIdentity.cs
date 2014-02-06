using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class EntityPartTypeIdentity : EntityPropertyIdentityBase<EntityPartTypeIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.EntityPartTypePropertyId; }
        }

        public override string Description
        {
            get { return "EntityPartType"; }
        }

        public override string PropertyName
        {
            get { return "EntityPartType"; }
        }

        public override Type PropertyType
        {
            get { return typeof(EntityPartType); }
        }
    }
}