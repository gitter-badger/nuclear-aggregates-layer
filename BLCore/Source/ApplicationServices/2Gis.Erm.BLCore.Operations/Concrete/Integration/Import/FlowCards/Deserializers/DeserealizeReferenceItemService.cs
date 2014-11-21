using System;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowCards.Deserializers
{
    public sealed class DeserealizeReferenceItemService : IDeserializeServiceBusObjectService<ReferenceItemServiceBusDto>
    {
        private readonly IIntegrationLocalizationSettings _integrationLocalizationSettings;

        public DeserealizeReferenceItemService(IIntegrationLocalizationSettings integrationLocalizationSettings)
        {
            _integrationLocalizationSettings = integrationLocalizationSettings;
        }

        public IServiceBusDto Deserialize(XElement xml)
        {
            var referenceItemDto = new ReferenceItemServiceBusDto();

            // ReferenceCode
            var referenceCodeAttr = xml.Attribute("ReferenceCode");
            if (referenceCodeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут ReferenceCode");
            }

            referenceItemDto.ReferenceCode = referenceCodeAttr.Value;

            // Code
            var codeAttr = xml.Attribute("Code");
            if (codeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Code");
            }

            referenceItemDto.Code = (int)codeAttr;

            var name =
                xml.Elements("Localizations").Elements("Localization")
                   .FirstOrDefault(
                                   x =>
                                   string.Equals(_integrationLocalizationSettings.BasicLanguage,
                                                 x.Attribute("Lang").Value,
                                                 StringComparison.InvariantCultureIgnoreCase)) ??
                xml.Elements("Localizations").Elements("Localization")
                   .FirstOrDefault(
                                   x =>
                                   string.Equals(_integrationLocalizationSettings.ReserveLanguage,
                                                 x.Attribute("Lang").Value,
                                                 StringComparison.InvariantCultureIgnoreCase)) ??
                xml.Elements("Localizations").Elements("Localization")
                   .FirstOrDefault();

            if (name == null)
            {
                throw new BusinessLogicException(BLResources.ReferenceItemDoesntContainName);
            }

            // Name
            var nameAttr = name.Attribute("Name");
            if (nameAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Name");
            }

            referenceItemDto.Name = nameAttr.Value;

            // IsDeleted
            var isDeletedAttr = xml.Attribute("IsDeleted");
            if (isDeletedAttr != null)
            {
                referenceItemDto.IsDeleted = (bool)isDeletedAttr;
            }

            return referenceItemDto;
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