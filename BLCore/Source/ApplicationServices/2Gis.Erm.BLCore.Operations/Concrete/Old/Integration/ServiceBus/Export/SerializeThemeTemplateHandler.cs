using System;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Themes;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class SerializeThemeTemplateHandler : SerializeObjectsHandler<ThemeTemplate, ExportFlowOrdersResource>
    {
        private readonly IThemeRepository _themeRepository;

        public SerializeThemeTemplateHandler(IExportRepository<ThemeTemplate> exportRepository,
                                             IThemeRepository themeRepository,
                                             ITracer tracer)
            : base(exportRepository, tracer)
        {
            _themeRepository = themeRepository;
        }

        protected override string GetXsdSchemaContent(string schemaName)
        {
            return Properties.Resources.ResourceManager.GetString(schemaName);
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entityDto)
        {
            var data = entityDto as ThemeExportDto;
            if (data == null)
            {
                var message = string.Format("Invalid parameter type, expected {0}, got {1}", typeof(ThemeExportDto).Name, entityDto.GetType().Name);
                throw new ArgumentException(message);
            }

            var element = new XElement("Resource");
            element.Add(new XAttribute("Code", data.ThemeTemplate.Id));

            element.Add(new XAttribute("Value", _themeRepository.GetBase64EncodedFile(data.ThemeTemplate.FileId)));
            element.Add(new XAttribute("IsDeleted", data.ThemeTemplate.IsDeleted));
            element.Add(new XAttribute("IsSkyScraper", data.ThemeTemplate.IsSkyScraper));
            
            return element;
        }
        
        protected override ISelectSpecification<ThemeTemplate, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<ThemeTemplate, IExportableEntityDto>(template => new ThemeExportDto
            {
                Id = template.Id,
                ThemeTemplate = template,
            });
        }

        public sealed class ThemeExportDto : IExportableEntityDto
        {
            public long Id { get; set; }
            public ThemeTemplate ThemeTemplate { get; set; }
        }
    }
}
