using System;
using System.IO;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public interface IOrdersWithAdvertisementMaterialsSerializerFactory
    {
        IOrdersWithAdvertisementMaterialsSerializer Create(Stream stream, Action<long, Stream> fileContentAccessor, int organizationUnitDgppId);
    }
}
