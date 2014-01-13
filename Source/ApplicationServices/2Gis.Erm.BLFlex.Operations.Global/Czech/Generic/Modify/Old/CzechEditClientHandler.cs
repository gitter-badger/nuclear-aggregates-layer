using DoubleGis.Erm.BLCore.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Modify.Old
{
    public sealed class CzechEditClientHandler : RequestHandler<EditRequest<Client>, EmptyResponse>, ICzechAdapted
    {
        private readonly IClientRepository _clientRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public CzechEditClientHandler(
            IClientRepository clientRepository,
            IOperationScopeFactory operationScopeFactory)
        {
            _clientRepository = clientRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        protected override EmptyResponse Handle(EditRequest<Client> request)
        {
            var client = request.Entity;

            using (var operationScope = _operationScopeFactory.CreateOrUpdateOperationFor(client))
            {
                var isNew = client.IsNew();

                _clientRepository.CreateOrUpdate(client);

                if (isNew)
                {
                    operationScope.Added<Client>(client.Id);
                }
                else
                {
                    operationScope.Updated<Client>(client.Id);
                }

                operationScope.Complete();
            }

            return Response.Empty;
        }
    }
}
