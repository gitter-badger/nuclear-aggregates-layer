using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.PostIntegrationActivities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Exporters
{
    // TODO {all, 26.03.2014}: непосредственно используется finder, хотя данный тип не является readmodel (хотя пока и является simplifiedmodelconsumer) - учесть при рефакторинге обработки perfomed business operations
    public sealed class ImportedFirmAddressOperationsExporter : IOperationsExporter<FirmAddress, ImportedFirmAddress>
    {
        private readonly IOperationResolver _operationResolver;
        private readonly IOperationContextParser _operationContextParser;
        private readonly IFinder _finder;
        private readonly IPublicService _publicService;
        private readonly ITracer _logger;

        public ImportedFirmAddressOperationsExporter(
            IOperationResolver operationResolver,
            IOperationContextParser operationContextParser,
            IFinder finder, 
            IPublicService publicService, 
            ITracer logger)
        {
            _operationResolver = operationResolver;
            _operationContextParser = operationContextParser;
            _finder = finder;
            _publicService = publicService;
            _logger = logger;
        }

        public void ExportOperations(FlowDescription flowDescription,
                                     IEnumerable<PerformedBusinessOperation> operations,
                                     int packageSize,
                                     out IEnumerable<IExportableEntityDto> failedEntites)
        {
            failedEntites = Enumerable.Empty<IExportableEntityDto>();

            var firmAddresses = GetFirmAddressesToBeRefreshed(operations).ToArray();
            if (!firmAddresses.Any())
            {
                _logger.Warn("No one firm addresses have to be syncronized");
                return;
            }

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
            var operationIdentity = _operationResolver.ResolveOperation(operation);
            if (!operationIdentity.OperationIdentity.Equals(ImportCardIdentity.Instance))
            {
                _logger.WarnFormat("Specified operation {0}, can't trigger firm addresses synchronization, ignore operation and do nothing", operationIdentity);
                return Enumerable.Empty<long>();
            }

            EntityChangesContext entityChangesContext;
            string report;
            if (!_operationContextParser.TryParse(operation.Context, out entityChangesContext, out report))
            {
                throw new InvalidOperationException("Can't parse operation context. Detail: " + report);
            }

            var targetEntityType = typeof(Firm);
            var changedFirmIds = new List<long>();

            ConcurrentDictionary<long, int> changes;
            if (entityChangesContext.AddedChanges.TryGetValue(targetEntityType, out changes))
            {
                changedFirmIds.AddRange(changes.Keys);
            }

            if (entityChangesContext.UpdatedChanges.TryGetValue(targetEntityType, out changes))
            {
                changedFirmIds.AddRange(changes.Keys);
            }

            if (entityChangesContext.DeletedChanges.TryGetValue(targetEntityType, out changes))
            {
                changedFirmIds.AddRange(changes.Keys);
            }

            return changedFirmIds;
        }

        private IEnumerable<FirmAddress> GetFirmAddressesToBeRefreshed(IEnumerable<PerformedBusinessOperation> operations)
        {
            var firmIds = operations.SelectMany(ProcessOperationContext).Distinct().ToArray();
            return firmIds.Any() 
                ? GetAddressesByFirm(firmIds) 
                : Enumerable.Empty<FirmAddress>();
        }

        private IEnumerable<FirmAddress> GetAddressesByFirm(IEnumerable<long> entityIds)
        {
            return
                _finder.FindMany(FirmSpecs.Addresses.Find.ByFirmIds(entityIds) && Specs.Find.ActiveAndNotDeleted<FirmAddress>() &&
                                 FirmSpecs.Addresses.Find.NotClosed() && FirmSpecs.Addresses.Find.WithAddressCode());
        }
    }
}