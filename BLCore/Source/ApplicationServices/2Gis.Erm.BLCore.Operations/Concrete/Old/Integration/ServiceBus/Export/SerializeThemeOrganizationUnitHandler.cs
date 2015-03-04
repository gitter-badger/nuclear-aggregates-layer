using System;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class SerializeThemeOrganizationUnitHandler : SerializeObjectsHandler<ThemeOrganizationUnit, ExportFlowOrdersThemeBranch>
    {
        public SerializeThemeOrganizationUnitHandler(IExportRepository<ThemeOrganizationUnit> exportRepository,
                                                     ICommonLog logger)
            : base(exportRepository, logger)
        {
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

            var element = new XElement("ThemeBranch");
            element.Add(new XAttribute("Code", data.Id));
            element.Add(new XAttribute("ThemeCode", data.ThemeId));
            if (data.OrganizationUnitDgpp.HasValue)
            {
                element.Add(new XAttribute("BranchCode", data.OrganizationUnitDgpp.Value));
            }
            
            element.Add(new XAttribute("IsDeleted", data.IsDeleted));
            element.Add(new XAttribute("IsDefault", data.IsDefault));

            return element;
        }

        protected override ISelectSpecification<ThemeOrganizationUnit, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<ThemeOrganizationUnit, IExportableEntityDto>(link => new ThemeExportDto
            {
                Id = link.Id,
                ThemeId = link.ThemeId,
                OrganizationUnitDgpp = link.OrganizationUnit.DgppId.Value,
                IsDeleted = link.IsDeleted || !link.IsActive,
                IsDefault = link.Theme.IsDefault,
            });
        }
        
        public sealed class ThemeExportDto : IExportableEntityDto
        {
            public long Id { get; set; }

            public long ThemeId { get; set; }
            public int? OrganizationUnitDgpp { get; set; }
            public bool IsDeleted { get; set; }
            public bool IsDefault { get; set; }
        }
    }
}
