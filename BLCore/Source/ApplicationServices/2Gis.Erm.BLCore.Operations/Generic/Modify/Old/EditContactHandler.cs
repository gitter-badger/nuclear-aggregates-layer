using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditContactHandler : RequestHandler<EditRequest<Contact>, EmptyResponse>
    {
        private readonly IClientService _clientRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public EditContactHandler(IClientService clientRepository, IOperationScopeFactory operationScopeFactory)
        {
            _clientRepository = clientRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        protected override EmptyResponse Handle(EditRequest<Contact> request)
        {
            var contact = request.Entity;

            using (var operationScope = _operationScopeFactory.CreateOrUpdateOperationFor(contact))
            {
                var isNew = contact.IsNew();
                _clientRepository.CreateOrUpdate(contact);

                if (isNew)
                {
                    operationScope.Added<Contact>(contact.Id);
                }
                else
                {
                    operationScope.Updated<Contact>(contact.Id);
                }

                operationScope.Complete();
            }

            return Response.Empty;
        }
    }
}