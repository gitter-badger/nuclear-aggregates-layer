﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Georgaphy;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class ImportBuildingAggregateService : IImportBuildingAggregateService
    {
        // timeout should be increased due to long sql updates (15:00:00 min = 900 sec)
        private const int ImportCommandTimeout = 900;

        private readonly IFirmPersistenceService _firmPersistanceService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportBuildingAggregateService(IFirmPersistenceService firmPersistanceService, IOperationScopeFactory scopeFactory)
        {
            _firmPersistanceService = firmPersistanceService;
            _scopeFactory = scopeFactory;
        }

        public void ImportBuildingFromServiceBus(IEnumerable<BuildingServiceBusDto> buildingDtos,
                                                 string regionalTerritoryLocaleSpecificWord,
                                                 bool enableReplication)
        {
            var filteredBuildingDtos = buildingDtos.Where(x => x.SaleTerritoryCode != null || x.IsDeleted);

            var buildingServiceBusDtos = filteredBuildingDtos as BuildingServiceBusDto[] ?? filteredBuildingDtos.ToArray();
            if (!buildingServiceBusDtos.Any())
            {
                return;
            }

            // Обработка активных зданий
            var activeBuildingDtos = buildingServiceBusDtos.Where(dto => !dto.IsDeleted).ToArray();
            if (activeBuildingDtos.Any())
            {
                var xml = SerializeBuildingDtos(activeBuildingDtos, "buildings", "building");

                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Building>())
                {
                    // TODO {all, 08.07.2014}: пока UpdateBuildings возвращает только измененные фирмы, если необходимо будет логироать все изменения (например, по клиентам для целей репликации в CRM) - нужно дорабатывать хранимку
                    var updatedFirms = _firmPersistanceService.UpdateBuildings(xml, ImportCommandTimeout, regionalTerritoryLocaleSpecificWord, enableReplication);

                    scope.Updated<Firm>(updatedFirms)
                         .Complete();
                }
            }

            // Обработка удалённых зданий
            var deletedBuildingCodes = buildingServiceBusDtos.Where(dto => dto.IsDeleted).Select(dto => dto.Code).ToArray();
            if (deletedBuildingCodes.Any())
            {
                var xml = SerializeDeletedBuildingCodes(deletedBuildingCodes);
                _firmPersistanceService.DeleteBuildings(xml, ImportCommandTimeout);
            }
        }

        private static string SerializeBuildingDtos(IEnumerable<BuildingServiceBusDto> items, string root, string elementName)
        {
            var stringBuilder = new StringBuilder();

            using (var writer = XmlWriter.Create(stringBuilder, new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                writer.WriteStartElement(root);

                foreach (var item in items)
                {
                    writer.WriteStartElement(elementName);

                    var attributes =
                        new[]
                            {
                                new KeyValuePair<string, object>("Code", item.Code),
                                new KeyValuePair<string, object>("SaleTerritoryCode", item.SaleTerritoryCode),
                                new KeyValuePair<string, object>("IsDeleted", item.IsDeleted)
                            }
                            .Where(x => x.Value != null);
                    foreach (var attribute in attributes)
                    {
                        writer.WriteStartAttribute(attribute.Key);
                        writer.WriteValue(attribute.Value);
                        writer.WriteEndAttribute();
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            return stringBuilder.ToString();
        }

        private static string SerializeDeletedBuildingCodes(IEnumerable<long> codes)
        {
            var root = new XElement("root");
            foreach (var code in codes)
            {
                root.Add(new XElement("code", code));
            }

            return root.ToString();
        }
    }
}