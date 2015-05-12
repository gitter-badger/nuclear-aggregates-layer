using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.AutoMailer;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Xml;

using NuClear.Storage.UseCases;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.AutoMailer
{
    [UseCase(Duration = UseCaseDuration.VeryLong)]
    public sealed class ExportDataForAutoMailerHandler : RequestHandler<ExportDataForAutoMailerRequest, IntegrationResponse>
    {
        private readonly IOrderReadModel _orderReadModel;

        public ExportDataForAutoMailerHandler(IOrderReadModel orderReadModel)
        {
            _orderReadModel = orderReadModel;
        }

        public static string ClearText(string input)
        {
            return !string.IsNullOrEmpty(input) ? input.Replace(';', ',') : input;
        }

        protected override IntegrationResponse Handle(ExportDataForAutoMailerRequest request)
        {
            var startPeriodDate = request.PeriodStart.GetFirstDateOfMonth();
            var endPeriodDate = request.PeriodStart.GetEndPeriodOfThisMonth();
            var recepients = _orderReadModel.GetRecipientsForAutoMailer(startPeriodDate, endPeriodDate, request.IncludeRegionalAdvertisement);
            var data = new AutoMailerDataDto(request.SendingType, startPeriodDate, endPeriodDate, recepients);

            var xmlString = data.ToXml().ToString();

            string error;
            var xsd = Properties.Resources.ResourceManager.GetString("flowDeliveryData_SendingGroup");
            var isValidXml = xmlString.ValidateXml(xsd, out error);
            if (!isValidXml)
            {
                throw new BusinessLogicException(string.Format(BLResources.XSDValidationError, error));
            }

            return new IntegrationResponse
                {
                    Stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)),
                    ContentType = MediaTypeNames.Text.Xml,
                    FileName = "SendingGroup.xml",
                    DoNotDisplayProcessingAmount = true
                };
        }

        private class AutoMailerDataDto
        {
            private readonly MailSendingType _sendingType;
            private readonly DateTime _startDate;
            private readonly DateTime _endDate;
            private readonly IEnumerable<RecipientDto> _recipientDtos;

            public AutoMailerDataDto(MailSendingType sendingType, DateTime startDate, DateTime endDate, IEnumerable<RecipientDto> recipientDtos)
            {
                _sendingType = sendingType;
                _startDate = startDate;
                _endDate = endDate;
                _recipientDtos = recipientDtos;
            }
            
            public XElement ToXml()
            {
                var xml = new XElement("SendingGroup");

                xml.Add(new XAttribute("TypeCode", _sendingType));
                xml.Add(new XAttribute("StartDate", _startDate));
                xml.Add(new XAttribute("EndDate", _endDate));

                var recipients = new XElement("Recipients");
                xml.Add(recipients);
                foreach (var recipient in _recipientDtos)
                {
                    recipients.Add(recipient.ToXml());
                }

                return xml;
            }
        }
    }
}
