using System;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Rubrics;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowRubrics.Deserializers
{
    public sealed class DeserializeRubricService : IDeserializeServiceBusObjectService<RubricServiceBusDto>
    {
        private readonly IIntegrationLocalizationSettings _integrationLocalizationSettings;

        public DeserializeRubricService(IIntegrationLocalizationSettings integrationLocalizationSettings)
        {
            _integrationLocalizationSettings = integrationLocalizationSettings;
        }

        public IServiceBusDto Deserialize(XElement xml)
        {
            var category = new RubricServiceBusDto();

            var code = xml.Attribute("Code");
            if (code == null || string.IsNullOrEmpty(code.Value))
            {
                throw new NotificationException();
            }

            category.Id = long.Parse(code.Value);

            var isDeleted = xml.Attribute("IsDeleted");
            category.IsDeleted = isDeleted != null && !string.IsNullOrEmpty(isDeleted.Value) && bool.Parse(isDeleted.Value);

            if (category.IsDeleted)
            {
                category.OrganizationUnitsDgppIds = Enumerable.Empty<int>();
                return category;
            }

            category.ParentId = (long?)xml.Attribute("ParentCode");
            category.Level = (int)xml.Attribute("Level");

            var isHidden = xml.Attribute("IsHidden");
            category.IsHidden = isHidden != null && bool.Parse(isHidden.Value);

            var name = xml.Elements("Names")
                          .Elements("Name")
                          .FirstOrDefault(x => string.Equals(_integrationLocalizationSettings.BasicLanguage,
                                                             x.Attribute("Lang").Value,
                                                             StringComparison.InvariantCultureIgnoreCase)) ??
                       xml.Elements("Names")
                          .Elements("Name")
                          .FirstOrDefault(x => string.Equals(_integrationLocalizationSettings.ReserveLanguage,
                                                             x.Attribute("Lang").Value,
                                                             StringComparison.InvariantCultureIgnoreCase)) ??
                       xml.Elements("Names")
                          .Elements("Name")
                          .FirstOrDefault();

            if (name == null)
            {
                throw new BusinessLogicException(BLResources.RubricDoesntContainNameTemplate);
            }

            category.Name = name.Attribute("Value").Value;
            category.Comment = string.Join("; ",
                                           xml.Elements("Groups")
                                              .Elements("Group")
                                              .Elements("Comments")
                                              .Elements("Comment")
                                              .Where(x => string.Equals(_integrationLocalizationSettings.BasicLanguage,
                                                                        x.Attribute("Lang").Value,
                                                                        StringComparison.InvariantCultureIgnoreCase))
                                              .Select(ValueAttribute));

            if (string.IsNullOrWhiteSpace(category.Comment))
            {
                category.Comment = string.Join("; ",
                                               xml.Elements("Groups")
                                                  .Elements("Group")
                                                  .Elements("Comments")
                                                  .Elements("Comment")
                                                  .Where(x => string.Equals(_integrationLocalizationSettings.ReserveLanguage,
                                                                            x.Attribute("Lang").Value,
                                                                            StringComparison.InvariantCultureIgnoreCase))
                                                  .Select(ValueAttribute));
            }

            category.OrganizationUnitsDgppIds = xml.Elements("Groups")
                                                   .Elements("Group")
                                                   .Elements("Branches")
                                                   .Elements("Branch")
                                                   .Select(CodeAttribute)
                                                   .ToArray();

            return category;
        }

        private static string ValueAttribute(XElement element)
        {
            // Атрибут обязательный по xsd, если его не будет, то пусть лучше упадет.
            var value = element.Attribute("Value");
            return value.Value;
        }

        private static int CodeAttribute(XElement element)
        {
            // Атрибут обязательный по xsd, если его не будет, то пусть лучше упадет.
            var code = element.Attribute("Code");
            return int.Parse(code.Value);
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