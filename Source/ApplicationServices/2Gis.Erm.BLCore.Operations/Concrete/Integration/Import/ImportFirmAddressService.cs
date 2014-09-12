using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import
{
    public class ImportFirmAddressService : IImportFirmAddressService
    {
        private readonly IBulkCreateFirmAddressAggregateService _bulkCreateFirmAddressAggregateService;
        private readonly IBulkUpdateFirmAddressAggregateService _bulkUpdateFirmAddressAggregateService;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IIntegrationSettings _integrationSettings;

        public ImportFirmAddressService(IBulkCreateFirmAddressAggregateService bulkCreateFirmAddressAggregateService,
                                        IBulkUpdateFirmAddressAggregateService bulkUpdateFirmAddressAggregateService,
                                        IFirmReadModel firmReadModel,
                                        IOperationScopeFactory operationScopeFactory,
                                        IIntegrationSettings integrationSettings)
        {
            _bulkCreateFirmAddressAggregateService = bulkCreateFirmAddressAggregateService;
            _bulkUpdateFirmAddressAggregateService = bulkUpdateFirmAddressAggregateService;
            _firmReadModel = firmReadModel;
            _operationScopeFactory = operationScopeFactory;
            _integrationSettings = integrationSettings;
        }

        public void Import(IEnumerable<ImportFirmAddressDto> dtos)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<ImportFirmAddressIdentity>())
            {
                var addressesToImport = dtos.ToDictionary(x => x.FirmAddress.Id);

                var ids = addressesToImport.Keys;
                var existingAddresses = _firmReadModel.GetFirmAddresses(ids);

                var idsToUpdate = addressesToImport.Keys.Intersect(existingAddresses.Keys).ToArray();
                var idsToInsert = addressesToImport.Keys.Except(existingAddresses.Keys).ToArray();

                if (idsToUpdate.Any())
                {
                    var addressesToUpdate = new List<FirmAddress>();
                    foreach (var id in idsToUpdate)
                    {
                        var addressToUpdate = addressesToImport[id].FirmAddress;
                        var existingAddress = existingAddresses[id];

                        addressToUpdate.ReplicationCode = existingAddress.ReplicationCode;
                        addressToUpdate.CreatedBy = existingAddress.CreatedBy;
                        addressToUpdate.CreatedOn = existingAddress.CreatedOn;
                        addressToUpdate.Timestamp = existingAddress.Timestamp;

                        addressToUpdate.Within().SyncPropertyValue(x => x.Id, existingAddress);
                        addressToUpdate.Within().SyncPropertyValue(x => x.EntityId, existingAddress);
                        addressToUpdate.Within().SyncPropertyValue(x => x.Timestamp, existingAddress);

                        addressesToUpdate.Add(addressToUpdate);
                    }

                    _bulkUpdateFirmAddressAggregateService.Update(addressesToUpdate);
                }

                if (idsToInsert.Any())
                {
                    _bulkCreateFirmAddressAggregateService.Create(idsToInsert.Select(id => addressesToImport[id].FirmAddress).ToArray());
                }

                scope.Complete();
            }
        }
    }
}