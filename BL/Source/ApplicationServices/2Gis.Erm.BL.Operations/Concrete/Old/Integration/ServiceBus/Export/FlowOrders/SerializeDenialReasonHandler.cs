using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BL.Operations.Concrete.Old.Integration.ServiceBus.Export.FlowOrders
{
    public sealed class SerializeDenialReasonHandler : SerializeObjectsHandler<DenialReason, ExportFlowOrders_DenialReason>
    {
        public SerializeDenialReasonHandler(IExportRepository<DenialReason> exportOperationsRepository,
                                            ITracer tracer)
            : base(exportOperationsRepository, tracer)
        {
        }

        protected override string GetXsdSchemaContent(string schemaName)
        {
            return Properties.Resources.ResourceManager.GetString(schemaName);
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entiryDto)
        {
            var denialReasonDto = (DenialReasonDto)entiryDto;

            var denialReasonElement = new XElement("DenialReason",
                                                   new XAttribute("Code", denialReasonDto.Id),
                                                   new XAttribute("Name", denialReasonDto.Name),
                                                   new XAttribute("ProofLink", denialReasonDto.ProofLink),
                                                   new XAttribute("TypeCode", denialReasonDto.TypeCode),
                                                   new XAttribute("IsHidden", denialReasonDto.IsHidden));

            if (denialReasonDto.Description != null)
            {
                denialReasonElement.Add(new XAttribute("Description", denialReasonDto.Description));
            }

            return denialReasonElement;
        }

        protected override ISelectSpecification<DenialReason, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<DenialReason, IExportableEntityDto>(x => new DenialReasonDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    ProofLink = x.ProofLink,
                    TypeCode = (int)x.Type,
                    IsHidden = !x.IsActive,
                });
        }


        #region nested types

        private sealed class DenialReasonDto : IExportableEntityDto
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ProofLink { get; set; }
            public int TypeCode { get; set; }
            public bool IsHidden { get; set; }
        }

        #endregion
    }
}