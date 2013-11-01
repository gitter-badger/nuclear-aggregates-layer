using DoubleGis.Erm.BL.Aggregates.Accounts;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.AccountDetails;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Handlers.AccountDetails
{
    public sealed class ValidateCreateAccountDetailHandler : RequestHandler<ValidateCreateAccountDetailRequest, ValidateCreateAccountDetailResponse>, IRussiaAdapted
    {
        private readonly IUserContext _userContext;
        private readonly IAccountRepository _accountRepository;

        public ValidateCreateAccountDetailHandler( IUserContext userContext, IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _userContext = userContext;
        }

        protected override ValidateCreateAccountDetailResponse Handle(ValidateCreateAccountDetailRequest request)
        {
            var response = new ValidateCreateAccountDetailResponse();

            // В российской версии проверяем, что л/с - из франчайзи
            response.Validated = _accountRepository.IsCreateAccountDetailValid(request.AccountId, _userContext.Identity.Code, true);
            return response;
        }
    }
}
