using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Stickers;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowStickers.Processors
{
    public class ImportHotClientService : IImportHotClientService
    {
        private readonly ICreateHotClientRequestAggregateService _createHotClientRequestAggregateService;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public ImportHotClientService(ICreateHotClientRequestAggregateService createHotClientRequestAggregateService,
                                      IOperationScopeFactory operationScopeFactory)
        {
            _createHotClientRequestAggregateService = createHotClientRequestAggregateService;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var hotClientServiceBusDtos = dtos.Cast<HotClientServiceBusDto>();

            using (var operationScope = _operationScopeFactory.CreateNonCoupled<ImportHotClientIdentity>())
            {
                foreach (var request in hotClientServiceBusDtos)
                {
                    var entity = CreateHotClientRequest(request);
                    _createHotClientRequestAggregateService.Create(entity);
                }

                operationScope.Complete();
            }
        }

        // TODO {all, 10.04.2014}: Use ValueInjector?
        private static HotClientRequest CreateHotClientRequest(HotClientServiceBusDto dto)
        {
            return new HotClientRequest
                {
                    SourceCode = dto.SourceCode,
                    UserCode = dto.UserCode,
                    UserName = dto.UserName,
                    CreationDate = dto.CreationDate,
                    ContactName = dto.ContactName,
                    ContactPhone = dto.ContactPhone,
                    Description = dto.Description,
                    CardCode = dto.CardCode,
                    BranchCode = dto.BranchCode
                };
        } 
    }
}