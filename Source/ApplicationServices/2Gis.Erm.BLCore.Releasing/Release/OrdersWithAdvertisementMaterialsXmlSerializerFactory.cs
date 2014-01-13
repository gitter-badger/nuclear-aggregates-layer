using System;
using System.IO;

using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public sealed class OrdersWithAdvertisementMaterialsXmlSerializerFactory : IOrdersWithAdvertisementMaterialsSerializerFactory
    {
        private readonly ICommonLog _logger;

        public OrdersWithAdvertisementMaterialsXmlSerializerFactory(ICommonLog logger)
        {
            _logger = logger;
        }

        public IOrdersWithAdvertisementMaterialsSerializer Create(
            Stream stream,
            Action<long, Stream> fileContentAccessor, 
            int organizationUnitDgppId)
        {
            return new OrdersWithAdvertisementMaterialsXmlSerializer(stream, fileContentAccessor, organizationUnitDgppId, _logger);
        }
    }
}