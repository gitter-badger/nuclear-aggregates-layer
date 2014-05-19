using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowCards.Deserializers
{
    public sealed class DeserializeReferenceService : IDeserializeServiceBusObjectService<ReferenceServiceBusDto>
    {
        public IServiceBusDto Deserialize(XElement xml)
        {
            var referenceDto = new ReferenceServiceBusDto();

            // Code
            var codeAttr = xml.Attribute("Code");
            if (codeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Code");
            }

            referenceDto.Code = codeAttr.Value;

            return referenceDto;
        }


        public bool Validate(XElement xml, out string errorsMessage)
        {
            errorsMessage = null;
            return true;
        }

        public bool CanDeserialize(XElement xml)
        {
            return true;
        }
    }
}