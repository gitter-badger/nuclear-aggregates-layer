using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Themes;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class SerializeThemeHandler : SerializeObjectsHandler<Theme, ExportFlowOrdersTheme>
    {
        private readonly IThemeRepository _themeRepository;

        public SerializeThemeHandler(IExportRepository<Theme> exportOperationsRepository,
                                     IThemeRepository themeRepository,
                                     ITracer tracer)
            : base(exportOperationsRepository, tracer)
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

            var element = new XElement("Theme");
            element.Add(new XAttribute("Code", data.Theme.Id));
            element.Add(new XAttribute("Name", data.Theme.Name));
            element.Add(new XAttribute("ResourceCode", data.Theme.ThemeTemplateId));
            element.Add(new XAttribute("Value", _themeRepository.GetBase64EncodedFile(data.Theme.FileId)));
            element.Add(new XAttribute("StartDate", data.Theme.BeginDistribution));
            element.Add(new XAttribute("EndDate", data.Theme.EndDistribution));
            element.Add(new XAttribute("IsDeleted", data.Theme.IsDeleted));
            element.Add(new XAttribute("Description", data.Theme.Description));

            var rubrics = new XElement("Rubrics");
            element.Add(rubrics);
            foreach (var rubricCode in data.Rubrics)
            {
                var rubric = new XElement("Rubric");
                rubric.Add(new XAttribute("RubricCode", rubricCode));
                rubrics.Add(rubric);
            }

            return element;
        }

        protected override SelectSpecification<Theme, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<Theme, IExportableEntityDto>(theme => new ThemeExportDto
            {
                Id = theme.Id,
                Theme = theme,
                Rubrics = theme.ThemeCategories
                    .Where(category => !category.IsDeleted)
                    .Select(category => category.Category.Id)
            });
        }

        public sealed class ThemeExportDto : IExportableEntityDto
        {
            public long Id { get; set; }

            public Theme Theme { get; set; }

            /// <summary> Dgpp-коды рубрик, прикрепленных к этой тематике</summary>
            public IEnumerable<long> Rubrics { get; set; }
        }
    }
}
