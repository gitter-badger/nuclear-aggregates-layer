using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.PostIntegrationActivities;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export
{
    public class ImportedFirmAddressOperationsExporter : IOperationsExporter<FirmAddress, ImportedFirmAddress>
    {
        private readonly IFinder _finder;
        private readonly IPublicService _publicService;
        private readonly IOperationContextParser _operationContextParser;

        public ImportedFirmAddressOperationsExporter(IFinder finder, IPublicService publicService, IOperationContextParser operationContextParser)
        {
            _finder = finder;
            _publicService = publicService;
            _operationContextParser = operationContextParser;
        }

        public void ExportOperations(FlowDescription flowDescription,
                                     IEnumerable<PerformedBusinessOperation> operations,
                                     int packageSize,
                                     out IEnumerable<IExportableEntityDto> failedEntites)
        {
            failedEntites = Enumerable.Empty<IExportableEntityDto>();

            var firmAddresses = GetFirmAddressesToBeRefreshed(operations).ToArray();
            var localization = XElement.Parse(flowDescription.Context);
            var language = localization.Attribute("Language").Value;
            _publicService.Handle(new SyncFirmAddressesRequest
                {
                    Language = language,
                    FirmAddresses = firmAddresses
                });
        }

        public void ExportFailedEntities(FlowDescription flowDescription,
                                         IEnumerable<ExportFailedEntity> failedEntities,
                                         int packageSize,
                                         out IEnumerable<IExportableEntityDto> exportedEntites)
        {
            exportedEntites = Enumerable.Empty<IExportableEntityDto>();
        }

        private IEnumerable<long> ProcessOperationContext(PerformedBusinessOperation operation)
        {
            const EntityName FirmEntityName = EntityName.Firm;

            var groupedIds = _operationContextParser.GetGroupedIdsFromContext(operation.Context, operation.Operation, operation.Descriptor);
            var entities = groupedIds.Keys.SelectMany(x => x.Entities).Distinct().ToArray();

            if (entities.Any() && !entities.Contains(FirmEntityName) || entities.Count() > 1)
            {
                throw new InvalidOperationException("Invalid operation context for operation with identity = ImportCardsFromServiceBusIdentity");
            }

            return groupedIds.Values.SelectMany(x => x);
        }

        private IEnumerable<FirmAddress> GetFirmAddressesToBeRefreshed(IEnumerable<PerformedBusinessOperation> operations)
        {
            var firmIds = operations.SelectMany(ProcessOperationContext).Distinct().ToArray();

            return firmIds.Any() ? GetAddressesByFirm(firmIds) : Enumerable.Empty<FirmAddress>();
        }

        private IEnumerable<FirmAddress> GetAddressesByFirm(IEnumerable<long> entityIds)
        {
            return _finder.Find(Specs.Find.ByIds<Firm>(entityIds))
                          .SelectMany(firm => firm.FirmAddresses)
                          .Where(address => address.IsActive && !address.IsDeleted && !address.ClosedForAscertainment && address.AddressCode != null)
                          .ToArray();
        }
    }
}