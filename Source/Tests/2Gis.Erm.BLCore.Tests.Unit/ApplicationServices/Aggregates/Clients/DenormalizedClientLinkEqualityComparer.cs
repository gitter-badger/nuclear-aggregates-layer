using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Tests.Unit.ApplicationServices.Aggregates.Clients
{
    public sealed class DenormalizedClientLinkEqualityComparer : EqualityComparer<DenormalizedClientLink>
    {
        public override bool Equals(DenormalizedClientLink entity1, DenormalizedClientLink entity2)
        {
            return entity1.MasterClientId == entity2.MasterClientId && entity1.ChildClientId == entity2.ChildClientId;
        }

        public override int GetHashCode(DenormalizedClientLink entity)
        {
            return entity.MasterClientId.GetHashCode() ^ entity.ChildClientId.GetHashCode();
        }
    }
}