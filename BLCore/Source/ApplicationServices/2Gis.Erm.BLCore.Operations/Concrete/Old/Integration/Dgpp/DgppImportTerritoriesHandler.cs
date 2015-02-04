using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.Dgpp;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.Dgpp
{
    public sealed class DgppImportTerritoriesHandler : RequestHandler<DgppImportTerritoriesRequest, ImportResponse>
    {
        private const string XmlHeader = "header";
        private const string XmlMessageId = "mguid";
        private const string XmlCreateDate = "createDate";
        private const string XmlExportDate = "exportDate";
        private const string XmlOrganizationUnitId = "organizationUnitId";

        private const string XmlTerritories = "territories";
        private const string XmlTerritory = "territory";
        private const string XmlTerritoryId = "id";
        private const string XmlName = "name";
        private const string XmlIsDeleted = "isDeleted";
        private const string XmlFirms = "firms";
        private const string XmlFirm = "firm";
        private const string XmlFirmId = "id";

        private const string HandlerName = "Импорт территорий из ДГПП";

        private readonly ICommonLog _logger;
        private readonly IFirmRepository _firmRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DgppImportTerritoriesHandler(ICommonLog logger, IFirmRepository firmRepository, IOperationScopeFactory operationScopeFactory)
        {
            _logger = logger;
            _firmRepository = firmRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        protected override ImportResponse Handle(DgppImportTerritoriesRequest request)
        {
            try
            {
                _logger.InfoFormat("{0}: начало", HandlerName);
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                {
                    ImportTerritoriesHeaderDto header;
                    IEnumerable<ImportTerritoryDto> territories;
                    using (var streamReader = new StreamReader(request.MessageStream, Encoding.UTF8))
                    using (var xmlReader = XmlReader.Create(streamReader, request.XmlReaderSettings))
                    {
                        ReadXml(xmlReader, out header, out territories);
                    }

                    var result = UpdateTerritories(header, territories);
                    transaction.Complete();
                    return result;
                }
            }
            catch (NotificationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NotificationException("Ошибка обработки сообщения", ex);
            }
            finally
            {
                _logger.InfoFormat("{0}: окончание", HandlerName);
            }
        }

        private ImportResponse UpdateTerritories(ImportTerritoriesHeaderDto header, IEnumerable<ImportTerritoryDto> territories)
        {
            var count = 0;
            using (var scope = _operationScopeFactory.CreateNonCoupled<ImportTerritoriesIdentity>())
            {
                foreach (var territory in territories)
                {
                    var importedEntity = _firmRepository.ImportTerritory(header, territory);
                    scope.Updated<Territory>(importedEntity.Id);
                    count += territory.Firms.Count();
                }

                scope.Complete();
            }

            return new ImportResponse { Processed = count, Total = count, OrganizationUnitId = header.OrganizationUnitId };
        }

        private void ReadXml(XmlReader xmlReader, out ImportTerritoriesHeaderDto header, out IEnumerable<ImportTerritoryDto> territories)
        {
            header = default(ImportTerritoriesHeaderDto);
            territories = default(IEnumerable<ImportTerritoryDto>);

            xmlReader.MoveToContent();
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                if (string.Equals(xmlReader.Name, XmlHeader, StringComparison.OrdinalIgnoreCase))
                {
                    header = LocalMessageHelper.Read<ImportTerritoriesHeaderDto>(HandlerName, xmlReader, XmlHeader, ReadHeader);
                    ValidateXmlHeader(header);
                }
                else if (string.Equals(xmlReader.Name, XmlTerritories, StringComparison.OrdinalIgnoreCase))
                {
                    territories = LocalMessageHelper.Read<List<ImportTerritoryDto>>(HandlerName, xmlReader, XmlTerritories, ReadTerritories);
                    ValidateTerritories(territories);
                }
            }

            if (header == null)
            {
                throw new NotificationException(string.Format("В сообщении не найден узел '{0}'", XmlHeader));
            }

            if (territories == null)
            {
                throw new NotificationException(string.Format("В сообщении не найден узел '{0}'", XmlTerritories));
            }
        }

        private void ValidateTerritories(IEnumerable<ImportTerritoryDto> territories)
        {
            foreach (var territory in territories)
            {
                if (territory.IsDeleted && territory.Firms.Any())
                {
                    throw new NotificationException(string.Format("Обнаружены фирмы на удаленной территории [{0}] - [{1}]", territory.Id, territory.Name));
                }
            }
        }

        private void ValidateXmlHeader(ImportTerritoriesHeaderDto header)
        {
            if (header.DgppId == 0)
            {
                throw new NotificationException("В сообщении не был указан код города по ДГПП");
            }

            var organizationUnit = _firmRepository.GetOrganizationUnit(header.DgppId);
            if (organizationUnit == null)
            {
                throw new NotificationException(string.Format("В системе не найден город с кодом ДГПП {0}", header.DgppId));
            }

            if (organizationUnit.InfoRussiaLaunchDate != null)
            {
                throw new NotificationException(string.Format("Город {0} переведён на InfoRussia, импорт территорий из ДГПП невозможен", organizationUnit.Name));
            }

            header.OrganizationUnitId = organizationUnit.Id;
        }

        private bool ReadTerritories(XmlReader xmlReader, ICollection<ImportTerritoryDto> accumulator)
        {
            if (!string.Equals(xmlReader.Name, XmlTerritory, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            accumulator.Add(LocalMessageHelper.Read<ImportTerritoryDto>(HandlerName, xmlReader, XmlTerritory, ReadTerritory));
            return true;
        }

        private bool ReadTerritory(XmlReader xmlReader, ImportTerritoryDto accumulator)
        {
            string value;
            if (LocalMessageHelper.TryReadElementText(xmlReader, XmlTerritoryId, out value))
            {
                accumulator.DgppId = long.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(xmlReader, XmlName, out value))
            {
                accumulator.Name = value;
            }
            else if (LocalMessageHelper.TryReadElementText(xmlReader, XmlIsDeleted, out value))
            {
                accumulator.IsDeleted = bool.Parse(value);
            }
            else if (string.Equals(xmlReader.Name, XmlFirms, StringComparison.OrdinalIgnoreCase))
            {
                accumulator.Firms = LocalMessageHelper.Read<List<long>>(HandlerName, xmlReader, XmlFirms, ReadFirms);
            }
            else
            {
                return false;
            }

            return true;
        }

        private bool ReadFirms(XmlReader xmlReader, ICollection<long> accumulator)
        {
            if (!string.Equals(xmlReader.Name, XmlFirm, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var attribute = xmlReader.GetAttribute(XmlFirmId);
            if (string.IsNullOrEmpty(attribute))
            {
                throw new NotificationException("Запись фирмы не содержит обязательного аттрибута 'id'");
            }

            accumulator.Add(long.Parse(attribute));

            return true;
        }

        private bool ReadHeader(XmlReader xmlReader, ImportTerritoriesHeaderDto accumulator)
        {
            string value;
            if (LocalMessageHelper.TryReadElementText(xmlReader, XmlMessageId, out value))
            {
                accumulator.MessageId = Guid.Parse(value);
            }
            else if (LocalMessageHelper.TryReadElementText(xmlReader, XmlCreateDate, out value))
            {
                accumulator.CreateDate = DateTime.Parse(xmlReader.Value);
            }
            else if (LocalMessageHelper.TryReadElementText(xmlReader, XmlExportDate, out value))
            {
                accumulator.ExportDate = DateTime.Parse(xmlReader.Value);
            }
            else if (LocalMessageHelper.TryReadElementText(xmlReader, XmlOrganizationUnitId, out value))
            {
                accumulator.DgppId = int.Parse(xmlReader.Value);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
