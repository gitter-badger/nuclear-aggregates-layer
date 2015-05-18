using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify.Old
{
    public sealed class MultiCultureEditClientHandler : RequestHandler<EditRequest<Client>, EmptyResponse>, IChileAdapted, ICyprusAdapted, ICzechAdapted,
                                                        IUkraineAdapted, IEmiratesAdapted, IKazakhstanAdapted
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ICreateClientAggregateService _createClientService;
        private readonly IUpdateAggregateRepository<Client> _updateClientService;

        public MultiCultureEditClientHandler(IOperationScopeFactory operationScopeFactory,
                                             ICreateClientAggregateService createClientService,
                                             IUpdateAggregateRepository<Client> updateClientService)
        {
            _operationScopeFactory = operationScopeFactory;
            _createClientService = createClientService;
            _updateClientService = updateClientService;
        }

        protected override EmptyResponse Handle(EditRequest<Client> request)
        {
            var client = request.Entity;
            
            // COMMENT {y.baranihin, 17.06.2014}: При отдельной реализации Create и Update получаем в дальнейшем возможность проще растащить этот код на отдельные операции
            if (client.IsNew())
            {
                using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Client>())
                {
                    FirmAddress mainFirmAddress;
                    _createClientService.Create(client, out mainFirmAddress);
                    operationScope.Added<Client>(client.Id);

                    operationScope.Complete();
                }
            }
            else
            {
                using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Client>())
                {
                    _updateClientService.Update(client);
                    operationScope.Updated<Client>(client.Id);

                    operationScope.Complete();
                }
            }

            return Response.Empty;
        }
    }
}
