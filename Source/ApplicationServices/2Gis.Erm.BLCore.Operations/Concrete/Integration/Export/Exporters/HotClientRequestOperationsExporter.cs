using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.HotClient;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Exporters
{
    public sealed class HotClientRequestOperationsExporter : IOperationsExporter<HotClientRequest, ExportToMsCrm_HotClients>
    {
        private readonly ICommonLog _logger;
        private readonly IPublicService _publicService;
        private readonly IOldOperationContextParser _oldOperationContextParser;

        public HotClientRequestOperationsExporter(ICommonLog logger, IPublicService publicService, IOldOperationContextParser oldOperationContextParser)
        {
            _logger = logger;
            _publicService = publicService;
            _oldOperationContextParser = oldOperationContextParser;
        }

        public void ExportOperations(FlowDescription flowDescription,
                                     IEnumerable<PerformedBusinessOperation> operations,
                                     int packageSize,
                                     out IEnumerable<IExportableEntityDto> failedEntites)
        {
            var failedEntitesBuffer = new List<IExportableEntityDto>();
            foreach (var operation in operations)
            {
                var operationIdentityToIds = _oldOperationContextParser.GetGroupedIdsFromContext(operation.Context, operation.Operation, operation.Descriptor);
                if (operationIdentityToIds.Count != 1)
                {
                    failedEntites = Enumerable.Empty<IExportableEntityDto>();
                    return;
                }

                var hotClientRequestId = operationIdentityToIds.First().Value.Single();
                try
                {
                    var response = (CreateHotClientResponse)_publicService.Handle(new CreateHotClientRequest { Id = hotClientRequestId });
                    if (!response.Success)
                    {
                        failedEntitesBuffer.Add(new ExportableEntityDto { Id = hotClientRequestId });
                    }
                }
                catch (BusinessLogicException ex)
                {
                    _logger.WarnEx(ex.Message);
                    failedEntitesBuffer.Add(new ExportableEntityDto { Id = hotClientRequestId });
                }
                catch (Exception ex)
                {
                    _logger.FatalFormatEx(ex, ex.Message);
                    failedEntitesBuffer.Add(new ExportableEntityDto { Id = hotClientRequestId });
                }
            }

            failedEntites = failedEntitesBuffer;
        }

        public void ExportFailedEntities(FlowDescription flowDescription,
                                         IEnumerable<ExportFailedEntity> failedEntities,
                                         int packageSize,
                                         out IEnumerable<IExportableEntityDto> exportedEntites)
        {
            var exportedEntitesBuffer = new List<IExportableEntityDto>();
            foreach (var failedEntity in failedEntities)
            {
                try
                {
                var response = (CreateHotClientResponse)_publicService.Handle(new CreateHotClientRequest { Id = failedEntity.EntityId });
                if (response.Success)
                {
                    exportedEntitesBuffer.Add(new ExportableEntityDto { Id = failedEntity.EntityId });
                }
            }
                catch (BusinessLogicException ex)
                {
                    _logger.WarnEx(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.FatalFormatEx(ex, ex.Message);
                }
            }

            exportedEntites = exportedEntitesBuffer;
        }
    }
}