using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class SerializeClientHandler : SerializeObjectsHandler<Client, ExportFlowFinancialDataClient>
    {
        private static readonly Dictionary<AccountRole, string> AccountRoleToServiceBusMap = new Dictionary<AccountRole, string>
                {
                    { AccountRole.None, "None" },
                    { AccountRole.Employee, "Employee" },
                    { AccountRole.InfluenceDecisions, "InfluenceDecisions" },
                    { AccountRole.MakingDecisions, "MakingDecisions" },
                };

        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public SerializeClientHandler(
            IExportRepository<Client> exportRepository,
            ISecurityServiceUserIdentifier securityServiceUserIdentifier,
            ITracer tracer)
            : base(exportRepository, tracer)
        {
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        protected override string GetError(IExportableEntityDto entiryDto)
        {
            var dto = (ClientDto)entiryDto;

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return string.Format("Название клиента должно быть указано: {0}", dto.Id);
            }

            var invalidContacts = dto.Contacts.Where(contactDto => string.IsNullOrWhiteSpace(contactDto.Name)).ToArray();
            if (invalidContacts.Any())
            {
                return string.Format("Названия контактов фирм обязательно должны быть указаны: {0}", string.Join(", ", invalidContacts.Select(contactDto => contactDto.Id)));
            }

            return null;
        }

        protected override ISelectSpecification<Client, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<Client, IExportableEntityDto>(client => new ClientDto
                {
                    Id = client.Id,
                    Firms = client.Firms
                                  .Where(firm => firm.IsActive && !firm.IsDeleted && !firm.ClosedForAscertainment)
                                  .Select(firm => new FirmDto { Id = firm.Id }),
                    LegalPersons = client.LegalPersons
                                         .Where(person => person.IsActive && !person.IsDeleted)
                                         .Select(person => new LegalPersonDto { Id = person.Id }),
                    Contacts = client.Contacts
                                     .Select(contact => new ContactDto
                                         {
                                             Id = contact.Id,
                                             Email = contact.WorkEmail,
                                             Name = contact.FullName,
                                             Role = contact.AccountRole,
                                             IsDeleted = contact.IsDeleted,
                                             IsHidden = contact.IsFired || !contact.IsActive,
                                         }),
                    Name = client.Name,
                    IsAdvertisingAgency = client.IsAdvertisingAgency,
                    IsHidden = !client.IsActive,
                    IsDeleted = client.IsDeleted,
                    OwnerCode = client.OwnerCode,
                    DefaultFirmCode = client.Firm.Id,
                });
        }

        protected override string GetXsdSchemaContent(string schemaName)
        {
            return Properties.Resources.ResourceManager.GetString(schemaName);
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entityDto)
        {
            var dto = (ClientDto)entityDto;
            var userInfo = _securityServiceUserIdentifier.GetUserInfo(dto.OwnerCode);
            var result = new XElement("Client",
                new XAttribute("Code", dto.Id),
                new XAttribute("Name", dto.Name),
                dto.DefaultFirmCode.HasValue ? new XAttribute("DefaultFirmCode", dto.DefaultFirmCode) : null,
                new XAttribute("IsAdvertisingAgency", dto.IsAdvertisingAgency),
                new XAttribute("CuratorLogin", userInfo.Account),
                new XAttribute("IsHidden", dto.IsHidden),
                new XAttribute("IsDeleted", dto.IsDeleted));
            result.Add(SerializeFirms(dto.Firms));
            result.Add(SerializeLegalPersons(dto.LegalPersons));
            result.Add(SerializeContacts(dto.Contacts));
            return result;
        }

        private XElement SerializeContacts(IEnumerable<ContactDto> dtos)
        {
            var result = new XElement("Contacts");
            foreach (var dto in dtos)
            {
                result.Add(new XElement("Contact",
                    new XAttribute("ContactName", dto.Name),
                    string.IsNullOrWhiteSpace(dto.Email) ? null : new XAttribute("ContactEmail", dto.Email),
                    new XAttribute("PersonType", AccountRoleToServiceBusMap[dto.Role]),
                    new XAttribute("IsHidden", dto.IsHidden),
                    new XAttribute("IsDeleted", dto.IsDeleted)));
            }

            return result;
        }

        private XElement SerializeLegalPersons(IEnumerable<LegalPersonDto> dtos)
        {
            var result = new XElement("LegalEntities");
            foreach (var dto in dtos)
            {
                result.Add(new XElement("LegalEntity", new XAttribute("Code", dto.Id)));
            }

            return result;
        }

        private XElement SerializeFirms(IEnumerable<FirmDto> dtos)
        {
            var result = new XElement("Firms");
            foreach (var firmDto in dtos)
            {
                result.Add(new XElement("Firm", new XAttribute("Code", firmDto.Id)));
            }

            return result;
        }

        #region Nested Types

        public class ClientDto : IExportableEntityDto
        {
            public long Id { get; set; }
            public IEnumerable<FirmDto> Firms { get; set; }
            public IEnumerable<LegalPersonDto> LegalPersons { get; set; }
            public IEnumerable<ContactDto> Contacts { get; set; }
            public string Name { get; set; }
            public bool IsHidden { get; set; }
            public bool IsDeleted { get; set; }
            public long OwnerCode { get; set; }
            public long? DefaultFirmCode { get; set; }
            public bool IsAdvertisingAgency { get; set; }
        }

        public class FirmDto
        {
            public long Id { get; set; }
        }

        public class LegalPersonDto
        {
            public long Id { get; set; }
        }

        public class ContactDto
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public AccountRole Role { get; set; }
            public bool IsHidden { get; set; }
            public bool IsDeleted { get; set; }
        }

        #endregion
    }
}
