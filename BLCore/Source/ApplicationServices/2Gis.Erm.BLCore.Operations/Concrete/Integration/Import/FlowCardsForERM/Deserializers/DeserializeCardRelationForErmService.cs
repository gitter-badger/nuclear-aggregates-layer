using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowCardsForERM.Deserializers
{
    public class DeserializeCardRelationForErmService : IDeserializeServiceBusObjectService<CardRelationForErmServiceBusDto>
    {
        public bool CanDeserialize(XElement xml)
        {
            return true;
        }

        public IServiceBusDto Deserialize(XElement xml)
        {
            return new CardRelationForErmServiceBusDto
                {
                    Code = (long)xml.Attribute("Code"),
                    Card1Code = (long)xml.Attribute("Card1Code"),
                    Card2Code = (long)xml.Attribute("Card2Code"),
                    BranchCode = (long)xml.Attribute("BranchCode"),
                    OrderNo = (int)xml.Attribute("OrderNo"),
                    IsDeleted = (bool?)xml.Attribute("IsDeleted") ?? false,
                };
        }

        public bool Validate(XElement xml, out string error)
        {
            error = null;
            return true;
        }
    }
}