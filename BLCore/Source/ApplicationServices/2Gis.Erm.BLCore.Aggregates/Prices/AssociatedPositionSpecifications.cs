﻿using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices
{
    public class AssociatedPositionSpecifications
    {
        public static class Find
        {
            public static FindSpecification<AssociatedPosition> AssociatedPositionsByGroup(long groupId)
            {
                return new FindSpecification<AssociatedPosition>(x => x.AssociatedPositionsGroupId == groupId);
            }
        }
    }
}
