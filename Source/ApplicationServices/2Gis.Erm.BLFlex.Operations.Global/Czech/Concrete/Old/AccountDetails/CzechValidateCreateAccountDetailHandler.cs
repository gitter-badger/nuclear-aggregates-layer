using DoubleGis.Erm.BL.Aggregates.Accounts;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.AccountDetails;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete.Old.AccountDetails
{
    public sealed class CzechValidateCreateAccountDetailHandler : RequestHandler<ValidateCreateAccountDetailRequest, ValidateCreateAccountDetailResponse>, ICzechAdapted
    {
        private readonly IUserContext _userContext;
        private readonly IAccountRepository _accountRepository;

        public CzechValidateCreateAccountDetailHandler(IUserContext userContext, IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _userContext = userContext;
        }

        protected override ValidateCreateAccountDetailResponse Handle(ValidateCreateAccountDetailRequest request)
        {
            var response = new ValidateCreateAccountDetailResponse();

            // В чешской версии не проверяем, что л/с - из франчайзи
            response.Validated = _accountRepository.IsCreateAccountDetailValid(request.AccountId, _userContext.Identity.Code, false);
            return response;
        }
    }
}