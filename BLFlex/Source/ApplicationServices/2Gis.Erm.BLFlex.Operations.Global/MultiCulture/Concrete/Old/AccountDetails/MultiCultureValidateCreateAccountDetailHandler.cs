using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.AccountDetails
{
    public sealed class MultiCultureValidateCreateAccountDetailHandler : RequestHandler<ValidateCreateAccountDetailRequest, ValidateCreateAccountDetailResponse>,
                                                                         ICyprusAdapted,
                                                                         IChileAdapted,
                                                                         ICzechAdapted,
                                                                         IUkraineAdapted,
                                                                         IEmiratesAdapted,
                                                                         IKazakhstanAdapted
    {
        private readonly IUserContext _userContext;
        private readonly IAccountRepository _accountRepository;

        public MultiCultureValidateCreateAccountDetailHandler(IUserContext userContext, IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _userContext = userContext;
        }

        protected override ValidateCreateAccountDetailResponse Handle(ValidateCreateAccountDetailRequest request)
        {
            var response = new ValidateCreateAccountDetailResponse();

            // В кипрской, чешской, чилийской, украинской, ОАЭ версии не проверяем, что л/с - из франчайзи
            response.Validated = _accountRepository.IsCreateAccountDetailValid(request.AccountId, _userContext.Identity.Code, false);
            return response;
        }
    }
}