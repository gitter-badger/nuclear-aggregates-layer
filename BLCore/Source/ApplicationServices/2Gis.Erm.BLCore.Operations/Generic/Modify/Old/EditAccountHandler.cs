using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditAccountHandler : RequestHandler<EditAccountRequest, EditAccountResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        
        public EditAccountHandler(IAccountRepository accountRepository, IOperationScopeFactory scopeFactory)
        {
            _accountRepository = accountRepository;
            _scopeFactory = scopeFactory;
        }

        protected override EditAccountResponse Handle(EditAccountRequest request)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity, Account>())
            {
                var account = request.IgnoreSecurity
                     ? _accountRepository.CreateAccount(request.Entity.LegalPersonId, request.Entity.BranchOfficeOrganizationUnitId)
                     : _accountRepository.SecureCreateAccount(request.Entity.LegalPersonId, request.Entity.BranchOfficeOrganizationUnitId);

                operationScope
                    .Added<Account>(account.Id)
                    .Complete();

                return new EditAccountResponse { AccountId = account.Id };   
            }
        }
    }
}