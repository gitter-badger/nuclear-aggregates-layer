using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import
{
    public interface IDeserializeServiceBusObjectService
    {
        bool CanDeserialize(XElement xml);
        IServiceBusDto Deserialize(XElement xml);
        bool Validate(XElement xml, out string errorsMessage);
    }

    // ReSharper disable once UnusedTypeParameter
    public interface IDeserializeServiceBusObjectService<out TServiceBusDto> : IDeserializeServiceBusObjectService where TServiceBusDto : IServiceBusDto
    {
    }
}