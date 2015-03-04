using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Shared;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BL.Operations.Concrete.Old.Integration.ServiceBus.Export.FlowNomenclatures
{
    public class SerializePositionHandler : SerializeObjectsHandler<Position, ExportFlowNomenclatures_NomenclatureElement>
    {
        public SerializePositionHandler(IExportRepository<Position> exportRepository, ITracer logger)
            : base(exportRepository, logger)
        {
        }

        protected override string GetXsdSchemaContent(string schemaName)
        {
            return Properties.Resources.ResourceManager.GetString(schemaName);
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entityDto)
        {
            var dto = (PositionDto)entityDto;

            return new XElement("NomenclatureElement",
                                new XAttribute("Code", dto.Id),
                                new XAttribute("Name", dto.Name),
                                new XAttribute("PlatformCode", dto.PlatformCode),
                                new XAttribute("AdvModel", dto.SalesModel.ConvertToServiceBusSalesModel()),
                                new XAttribute("IsHidden", dto.IsHidden),
                                new XAttribute("IsDeleted", dto.IsDeleted),
                                new XAttribute("ProductCode", dto.ProductCode),
                                new XAttribute("LinkObjectType", dto.LinkObjectType));
        }

        protected override ISelectSpecification<Position, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<Position, IExportableEntityDto>(x => new PositionDto
            {
                Id = x.Id,
                Name = x.Name,
                PlatformCode = x.Platform.DgppId,
                SalesModel = x.SalesModel,
                IsHidden = !x.IsActive,
                IsDeleted = x.IsDeleted,
                ProductCode = x.ExportCode,
                LinkObjectType = x.BindingObjectTypeEnum,
            });
        }

        #region dto

        private sealed class PositionDto : IExportableEntityDto
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public long PlatformCode { get; set; }
            public SalesModel SalesModel { get; set; }
            public bool IsHidden { get; set; }
            public bool IsDeleted { get; set; }
            public long ProductCode { get; set; }
            public PositionBindingObjectType LinkObjectType { get; set; }
        }

        #endregion
    }
}