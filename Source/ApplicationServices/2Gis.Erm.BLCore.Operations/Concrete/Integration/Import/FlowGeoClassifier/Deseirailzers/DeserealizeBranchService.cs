using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.GeoClassifier;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowGeoClassifier.Deseirailzers
{
    public sealed class DeserealizeBranchService : IDeserializeServiceBusObjectService<BranchServiceBusDto>
    {
        private readonly IIntegrationLocalizationSettings _integrationLocalizationSettings;

        public DeserealizeBranchService(IIntegrationLocalizationSettings integrationLocalizationSettings)
        {
            _integrationLocalizationSettings = integrationLocalizationSettings;
        }

        public IServiceBusDto Deserialize(XElement xml)
        {
            var projectDto = new BranchServiceBusDto();

            // Code
            var codeAttr = xml.Attribute("Code");
            if (codeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Code");
            }

            projectDto.Code = (int)codeAttr;

            var nameAttr = xml.XPathSelectElements("./Localizations/Localization")
                              .FirstOrDefault(x => x.Attribute("Lang").Value == _integrationLocalizationSettings.BasicLanguage);

            if (nameAttr == null)
            {
                nameAttr = xml.XPathSelectElements("./Localizations/Localization")
                              .FirstOrDefault(x => x.Attribute("Lang").Value == _integrationLocalizationSettings.ReserveLanguage);
            }

            if (nameAttr == null)
            {
                throw new BusinessLogicException("Проект не содержит названия ни на базовом, ни на резервном языке");
            }

            projectDto.DisplayName = nameAttr.Attribute("Name").Value;

            projectDto.DefaultLang = xml.Attribute("DefaultLang").Value;

            // NameLat
            var nameLatAttr = xml.Attribute("NameLat");
            if (nameLatAttr != null)
            {
                projectDto.NameLat = nameLatAttr.Value;
            }

            // IsDeleted
            var isDeletedAttr = xml.Attribute("IsDeleted");
            if (isDeletedAttr != null)
            {
                projectDto.IsDeleted = (bool)isDeletedAttr;
            }

            return projectDto;
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