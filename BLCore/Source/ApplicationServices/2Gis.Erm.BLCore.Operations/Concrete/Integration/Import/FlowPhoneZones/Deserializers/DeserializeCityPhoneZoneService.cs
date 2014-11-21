using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.PhoneZones;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowPhoneZones.Deserializers
{
    public sealed class DeserializeCityPhoneZoneService : IDeserializeServiceBusObjectService<CityPhoneZoneServiceBusDto>
    {
        public IServiceBusDto Deserialize(XElement xml)
        {
            var cityPhoneZone = new CityPhoneZoneServiceBusDto();

            // Code
            var codeAttr = xml.Attribute("Code");
            if (codeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Code");
            }

            cityPhoneZone.Id = (int)codeAttr;

            // Name
            var nameAttr = xml.Attribute("Name");
            if (nameAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Name");
            }

            cityPhoneZone.Name = nameAttr.Value;

            // CityCode
            var cityCodeAttr = xml.Attribute("CityCode");
            if (cityCodeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут CityCode");
            }

            cityPhoneZone.CityCode = (long)cityCodeAttr;

            // IsDefault
            var isDefaultAttr = xml.Attribute("IsDefault");
            if (isDefaultAttr != null)
            {
                cityPhoneZone.IsDefault = (bool)isDefaultAttr;
            }

            // IsDeleted
            var isDeletedAttr = xml.Attribute("IsDeleted");
            if (isDeletedAttr != null)
            {
                cityPhoneZone.IsDeleted = (bool)isDeletedAttr;
            }

            return cityPhoneZone;
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