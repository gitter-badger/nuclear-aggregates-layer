using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class SerializeFirmAddressHandler : SerializeObjectsHandler<FirmAddress, ExportFlowCardExtensionsCardCommercial>
    {
        public SerializeFirmAddressHandler(IExportRepository<FirmAddress> exportRepository,
                                           ITracer logger)
            : base(exportRepository, logger)
        {
        }

        protected override string GetXsdSchemaContent(string schemaName)
        {
            return Properties.Resources.ResourceManager.GetString(schemaName);
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entityDto)
        {
            var data = entityDto as FirmAddressExportDto;
            if (data == null)
            {
                var message = string.Format("Invalid parameter type, expected {0}, got {1}", typeof(FirmAddressExportDto).Name, entityDto.GetType().Name);
                throw new ArgumentException(message);
            }

            var element = new XElement("CardCommercial");
            element.Add(new XAttribute("Code", data.FirmAddressDgppId));
            var attributeArray = new XElement("Fields");
            element.Add(attributeArray);
            foreach (var service in data.Services)
            {
                var attribute = new XElement("BoolField");
                attribute.Add(new XAttribute("Code", service.Code));
                attribute.Add(new XAttribute("BranchCode", service.BranchCode));
                attribute.Add(new XAttribute("Value", service.Value));

                attributeArray.Add(attribute);
            }

            return element;
        }

        protected override ISelectSpecification<FirmAddress, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<FirmAddress, IExportableEntityDto>(firmAddress => new FirmAddressExportDto
            {
                Id = firmAddress.Id,
                FirmAddressDgppId = firmAddress.Id,
                Services = firmAddress.FirmAddressServices.Select(service => new ServiceExportDto
                    {
                        Code = service.AdditionalFirmService.ServiceCode,
                        BranchCode = firmAddress.Firm.OrganizationUnit.DgppId.Value,
                        Value = service.DisplayService,
                    })
            });
        }

        public sealed class FirmAddressExportDto : IExportableEntityDto
        {
            public long Id { get; set; }
            public long FirmAddressDgppId { get; set; }

            public IEnumerable<ServiceExportDto> Services { get; set; }
        }

        public sealed class ServiceExportDto
        {
            public string Code { get; set; }
            public int BranchCode { get; set; }
            public bool Value { get; set; }
        }
    }
}
