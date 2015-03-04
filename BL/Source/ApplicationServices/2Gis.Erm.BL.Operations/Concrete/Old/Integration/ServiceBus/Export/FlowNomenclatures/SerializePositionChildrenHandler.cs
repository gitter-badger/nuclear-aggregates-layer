using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BL.Operations.Concrete.Old.Integration.ServiceBus.Export.FlowNomenclatures
{
    public class SerializePositionChildrenHandler : SerializeObjectsHandler<PositionChildren, ExportFlowNomenclatures_NomenclatureElementRelation>
    {
        public SerializePositionChildrenHandler(IExportRepository<PositionChildren> exportRepository, ITracer tracer) : base(exportRepository, tracer)
        {
        }

        protected override string GetXsdSchemaContent(string schemaName)
        {
            return Properties.Resources.ResourceManager.GetString(schemaName);
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entityDto)
        {
            var dto = (PositionChildrenDto)entityDto;

            return new XElement("NomenclatureElementRelation",
                                new XAttribute("Code", dto.Id),
                                new XAttribute("MasterPositionCode", dto.MasterPositionCode),
                                new XAttribute("ChildPositionCode", dto.ChildPositionCode),
                                new XAttribute("IsHidden", dto.IsHidden),
                                new XAttribute("IsDeleted", dto.IsDeleted));
        }

        protected override ISelectSpecification<PositionChildren, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<PositionChildren, IExportableEntityDto>(x => new PositionChildrenDto
                {
                    Id = x.Id,
                    ChildPositionCode = x.ChildPositionId,
                    MasterPositionCode = x.MasterPositionId,
                    IsHidden = !x.IsActive,
                    IsDeleted = x.IsDeleted
                });
        }

        #region dto

        private sealed class PositionChildrenDto : IExportableEntityDto
        {
            public long Id { get; set; }
            public long MasterPositionCode { get; set; }
            public long ChildPositionCode { get; set; }
            public bool IsHidden { get; set; }
            public bool IsDeleted { get; set; }
        }

        #endregion
    }
}