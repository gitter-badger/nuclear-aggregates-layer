using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowCards.Deserializers
{
    public sealed class DeserializeCardRelationService : IDeserializeServiceBusObjectService<CardRelationServiceBusDto>
    {
        public IServiceBusDto Deserialize(XElement xml)
        {
            var сardRelationDto = new CardRelationServiceBusDto();

            // Задача 2704 - временно отключена, пока не готовы изменения в IR
            // Code
            var codeAttr = xml.Attribute("Code");
            if (codeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Code");
            }

            сardRelationDto.Code = (long)codeAttr;

            // Card1Code
            var card1CodeAttr = xml.Attribute("Card1Code");
            if (card1CodeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Card1Code");
            }

            сardRelationDto.PointOfServiceCardCode = (long)card1CodeAttr;

            // Card2Code
            var card2CodeAttr = xml.Attribute("Card2Code");
            if (card2CodeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Card2Code");
            }

            сardRelationDto.DepartmentCardCode = (long)card2CodeAttr;

            // OrderNo
            var orderNoAttr = xml.Attribute("OrderNo");
            if (orderNoAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут OrderNo");
            }

            // в erm для простоты sorting position должен начинаться с 1
            сardRelationDto.DepartmentCardSortingPosition = (int)orderNoAttr + 1;

            // IsDeleted
            var isDeletedAttr = xml.Attribute("IsDeleted");
            if (isDeletedAttr != null)
            {
                сardRelationDto.IsDeleted = (bool)isDeletedAttr;
            }

            return сardRelationDto;
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