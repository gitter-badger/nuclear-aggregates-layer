using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.AutoMailer;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Xml;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.AutoMailer
{
    [UseCase(Duration = UseCaseDuration.VeryLong)]
    public sealed class ExportDataForAutoMailerHandler : RequestHandler<ExportDataForAutoMailerRequest, IntegrationResponse>
    {
        private readonly IOrderRepository _orderRepository;

        public ExportDataForAutoMailerHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public static string ClearText(string input)
        {
            return !string.IsNullOrEmpty(input) ? input.Replace(';', ',') : input;
        }

        protected override IntegrationResponse Handle(ExportDataForAutoMailerRequest request)
        {
            var startPeriodDate = request.PeriodStart.GetFirstDateOfMonth();
            var endPeriodDate = request.PeriodStart.GetEndPeriodOfThisMonth();
            var data = new AutoMailerDataDto
                {
                    StartDate = startPeriodDate,
                    EndDate = endPeriodDate,
                    SendingType = request.SendingType
                };

            data.Recipients = _orderRepository.GetRecipientsForAutoMailer(startPeriodDate, endPeriodDate, request.IncludeRegionalAdvertisement);

            var xmlString = data.ToXml().ToString();
            var xmlSchemaSet = XmlValidator.CreateXmlSchemaSetForXsd(BLCore.Operations.Properties.Resources.ResourceManager.GetString("flowDeliveryData_SendingGroup"));

            string error;
            var isValidXml = XmlValidator.Validate(xmlString, xmlSchemaSet, out error);
            if (!isValidXml)
            {
                throw new BusinessLogicException(string.Format(BLResources.XSDValidationError, error));
            }

            return new IntegrationResponse
                {
                    Stream = new MemoryStream(
                        Encoding.UTF8.GetBytes(xmlString)),
                    ContentType = MediaTypeNames.Text.Xml,
                    FileName = "SendingGroup.xml",
                    DoNotDisplayProcessingAmount = true
                };
        }

        private class AutoMailerDataDto
        {
            public MailSendingType SendingType { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public IEnumerable<RecipientDto> Recipients { get; set; } 
            public XElement ToXml()
            {
                var xml = new XElement("SendingGroup");

                xml.Add(new XAttribute("TypeCode", SendingType));
                xml.Add(new XAttribute("StartDate", StartDate));
                xml.Add(new XAttribute("EndDate", EndDate));

                var recipients = new XElement("Recipients");
                xml.Add(recipients);
                foreach (var recipient in Recipients)
                {
                    recipients.Add(recipient.ToXml());
                }

                return xml;
            }
        }
    }
}
