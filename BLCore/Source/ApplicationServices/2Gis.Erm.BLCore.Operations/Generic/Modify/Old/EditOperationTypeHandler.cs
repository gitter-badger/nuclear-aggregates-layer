using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditOperationTypeHandler : RequestHandler<EditRequest<OperationType>, EmptyResponse>
    {
        private readonly IAccountRepository _accountRepository;

        public EditOperationTypeHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        protected override EmptyResponse Handle(EditRequest<OperationType> request)
        {
            var operationType = request.Entity;
            if (operationType.IsNew())
            {
                _accountRepository.Create(operationType);
            }
            else
            {
                _accountRepository.Update(operationType);
            }

            return Response.Empty;
        }
    }
}