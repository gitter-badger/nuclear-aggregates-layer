using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Georgaphy;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BL.Operations.Concrete.Integration.Import
{
    public sealed class DeserializeBuildingService : IDeserializeServiceBusObjectService<BuildingServiceBusDto>
    {
        public IServiceBusDto Deserialize(XElement xml)
        {
            var buildingDto = new BuildingServiceBusDto();

            // Code
            var codeAttr = xml.Attribute("Code");
            if (codeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Code");
            }

            buildingDto.Code = (long)codeAttr;

            // IsDeleted
            var isDeletedAttr = xml.Attribute("IsDeleted");
            if (isDeletedAttr != null)
            {
                buildingDto.IsDeleted = (bool)isDeletedAttr;

                // При выставленном IsDeleted = true, остальные поля не приходят
                if (buildingDto.IsDeleted)
                {
                    return buildingDto;
                }
            }

            // SaleTerritoryCode
            var saleTerritoryCodeAttr = xml.Attribute("SaleTerritoryCode");
            if (saleTerritoryCodeAttr != null)
            {
                buildingDto.SaleTerritoryCode = (long)saleTerritoryCodeAttr;
            }

            return buildingDto;
        }

        public bool Validate(XElement xml, out string errorsMessage)
        {
            var errors = new List<string>();

            var codeAttr = xml.Attribute("Code");
            if (codeAttr == null)
            {
                errors.Add("Не найден обязательный атрибут Code");
            }

            errorsMessage = string.Join("; ", errors);

            return !errors.Any();
        }

        public bool CanDeserialize(XElement xml)
        {
            return true;
        }
    }
}