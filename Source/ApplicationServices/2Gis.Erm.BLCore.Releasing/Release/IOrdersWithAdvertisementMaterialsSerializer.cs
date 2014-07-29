using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO.ForRelease;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public interface IOrdersWithAdvertisementMaterialsSerializer : IDisposable
    {
        int Serialize(IEnumerable<OrderInfo> orderInfos);
        void Complete();
    }
}