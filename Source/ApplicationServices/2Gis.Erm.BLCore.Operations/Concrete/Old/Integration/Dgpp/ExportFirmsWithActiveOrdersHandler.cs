using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.Dgpp;
using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.Dgpp
{
    public sealed class ExportFirmsWithActiveOrdersHandler : RequestHandler<ExportFirmsWithActiveOrdersRequest, EmptyResponse>
    {
        private readonly IFirmRepository _firmRepository;
        private readonly ISubRequestProcessor _subRequestProcessor;

        public ExportFirmsWithActiveOrdersHandler(ISubRequestProcessor subRequestProcessor, IFirmRepository firmRepository)
        {
            _subRequestProcessor = subRequestProcessor;
            _firmRepository = firmRepository;
        }

        protected override EmptyResponse Handle(ExportFirmsWithActiveOrdersRequest request)
        {
            var organizationUnitDtos = _firmRepository.ExportFirmWithActiveOrders();

            // creatin localmessages explicitly (not return StreamResponse)
            foreach (var organizationUnitDto in organizationUnitDtos)
            {
                var document = GetXDocument(organizationUnitDto.DgppId, organizationUnitDto.FirmDtos);

                _subRequestProcessor.HandleSubRequest(new CreateLocalMessageRequest
                    {
                        Content = XmlToMemoryStream(document),
                        IntegrationType = (int)IntegrationTypeExport.FirmsWithActiveOrdersToDgpp,
                        FileName = "Экспорт фирм с активными заказами." + organizationUnitDto.Name + ".xml",
                        ContentType = MediaTypeNames.Text.Xml,

                        Entity = new LocalMessage
                            {
                                EventDate = DateTime.UtcNow,
                                Status = (int)LocalMessageStatus.WaitForProcess,
                                OrganizationUnitId = organizationUnitDto.Id,
                            }
                    },
                    Context);
            }

            return Response.Empty;
        }

        #region serializing to xml

        private static XDocument GetXDocument(int organizationUnitDgppId, IEnumerable<FirmDto> firmDtos)
        {
            var document = new XDocument();

            var rootElement = new XElement("exchange");

            var headerElement = GetHeaderElement(organizationUnitDgppId);
            rootElement.Add(headerElement);

            var firmsElement = GetFirmsElement(firmDtos);
            rootElement.Add(firmsElement);

            document.Add(rootElement);

            return document;
        }

        private static XElement GetFirmsElement(IEnumerable<FirmDto> firmDtos)
        {
            var firmsElement = new XElement("firms");

            foreach (var firmDto in firmDtos)
            {
                var firmElement = new XElement("firm", new XAttribute("id", firmDto.Id));

                var ordersElement = GetOrdersElement(firmDto.OrderDtos);
                firmElement.Add(ordersElement);

                firmsElement.Add(firmElement);
            }

            return firmsElement;
        }

        private static XElement GetOrdersElement(IEnumerable<OrderDto> orderDtos)
        {
            var ordersElement = new XElement("orders");

            foreach (var orderDto in orderDtos)
            {
                var orderElement = new XElement("order",
                    new XAttribute("id", orderDto.Id),
                    new XAttribute("number", orderDto.Number),
                    new XAttribute("beginDistributionDate", orderDto.BeginDistributionDate),
                    new XAttribute("endDistributionDate", orderDto.EndDistributionDateFact));

                ordersElement.Add(orderElement);
            }

            return ordersElement;
        }

        private static XElement GetHeaderElement(int organizationUnitDgppId)
        {
            var headerElement = new XElement("header",
                new XElement("mguid", Guid.NewGuid()),
                new XElement("createDate", DateTime.UtcNow),
                new XElement("exportDate", DateTime.UtcNow),
                new XElement("organizationUnitId", organizationUnitDgppId));

            return headerElement;
        }

        #endregion

        private static MemoryStream XmlToMemoryStream(XDocument document)
        {
            var memoryStream = new MemoryStream();
            document.Save(memoryStream);

            return memoryStream;
        }
    }
}