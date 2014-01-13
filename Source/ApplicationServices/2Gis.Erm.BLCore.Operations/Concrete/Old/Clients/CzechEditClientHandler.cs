using DoubleGis.Erm.BL.Handlers.Infrastructure;
using DoubleGis.Erm.Common.Localization;
using DoubleGis.Erm.Core.OperationLogging;
using DoubleGis.Erm.Core.RequestResponse.Base;
using DoubleGis.Erm.Core.RequestResponse.Generic;
using DoubleGis.Erm.Model.Aggregates.ClientAggregate;
using DoubleGis.Erm.Model.Entities;
using DoubleGis.Erm.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.Handlers.Clients
{
    // 2+: BLFlex\Source\ApplicationServices\2Gis.Erm.BLFlex.Operations.Global\Czech\Generic\Modify\Old\CzechEditClientHandler .cs
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
