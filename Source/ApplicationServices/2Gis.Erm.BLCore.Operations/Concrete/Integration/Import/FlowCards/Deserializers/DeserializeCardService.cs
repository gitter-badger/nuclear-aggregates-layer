using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowCards.Deserializers
{
    public sealed class DeserializeCardService : IDeserializeServiceBusObjectService<CardServiceBusDto>
    {
        public IServiceBusDto Deserialize(XElement xml)
        {
            return new CardServiceBusDto { Content = xml };
        }

        public bool Validate(XElement xml, out string error)
        {
            error = null;
            return true;
        }

        public bool CanDeserialize(XElement xml)
        {
            return true;
        }
    }
}