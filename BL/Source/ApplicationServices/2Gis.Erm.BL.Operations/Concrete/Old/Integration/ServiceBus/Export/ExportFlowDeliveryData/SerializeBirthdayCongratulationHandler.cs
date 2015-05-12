using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BL.Operations.Concrete.Old.Integration.ServiceBus.Export.ExportFlowDeliveryData
{
    public class SerializeBirthdayCongratulationHandler : SerializeObjectsHandler<BirthdayCongratulation, ExportFlowDeliveryData_LetterSendRequest>
    {
        private const string EmailFrom = "no-reply@inf.2gis.ru";
        private const string Subject = "2ГИС поздравляет Вас с Днем рождения!";
        private const string Name = "HBSendingGroup";

        private readonly IClientReadModel _clientReadModel;

        public SerializeBirthdayCongratulationHandler(IExportRepository<BirthdayCongratulation> exportRepository,
                                                      ITracer tracer,
                                                      IClientReadModel clientReadModel)
            : base(exportRepository, tracer)
        {
            _clientReadModel = clientReadModel;
        }

        protected override string GetXsdSchemaContent(string schemaName)
        {
            return Properties.Resources.ResourceManager.GetString(schemaName);
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entityDto)
        {
            var dto = (BirthdayCongratulationDto)entityDto;

            if (dto.Emails == null || !dto.Emails.Any())
            {
                throw new ThereAreNoContactsToCongratulateException();
            }

            var recipients = new XElement("Recipients");
            foreach (var recipient in dto.Emails.Select(email => new XElement("Recipient", new XAttribute("EmailTo", email))))
            {
                recipients.Add(recipient);
            }

            return new XElement("LetterSendRequest",
                                new XAttribute("EmailFrom", dto.EmailFrom),
                                new XAttribute("Subject", dto.Subject),
                                new XAttribute("Text", dto.Text),
                                new XAttribute("Name", dto.Name),
                                new XAttribute("IsAutoSend", dto.IsAutoSend),
                                recipients);
        }

        protected override ISelectSpecification<BirthdayCongratulation, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<BirthdayCongratulation, IExportableEntityDto>(x => new BirthdayCongratulationDto
                {
                    Id = x.Id,
                    CongratulationDate = x.CongratulationDate
                });
        }

        protected override IEnumerable<IExportableEntityDto> ProcessDtosAfterMaterialization(IEnumerable<IExportableEntityDto> entityDtos)
        {
            foreach (var dto in entityDtos.Cast<BirthdayCongratulationDto>())
            {
                dto.EmailFrom = EmailFrom;
                dto.Subject = Subject;
                dto.Name = Name;
                dto.Text = Properties.Resources.BirthdayLetter;
                dto.Emails = _clientReadModel.GetContactEmailsByBirthDate(dto.CongratulationDate.Month, dto.CongratulationDate.Day);
                dto.IsAutoSend = true;
            }

            return entityDtos;
        }

        #region dto

        private sealed class BirthdayCongratulationDto : IExportableEntityDto
        {
            public long Id { get; set; }
            public DateTime CongratulationDate { get; set; }
            public string EmailFrom { get; set; }
            public string Subject { get; set; }
            public string Text { get; set; }
            public string Name { get; set; }
            public bool IsAutoSend { get; set; }
            public IEnumerable<string> Emails { get; set; }
        }

        #endregion
    }
}