using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AccountDetails;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditAccountDetailHandler : RequestHandler<EditRequest<AccountDetail>, EmptyResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly INotifyAboutAccountDetailModificationOperationService _notifyAboutAccountDetailModificationOperationService;


        public EditAccountDetailHandler(IAccountRepository accountRepository,
                                        INotifyAboutAccountDetailModificationOperationService notifyAboutAccountDetailModificationOperationService)
        {
            _accountRepository = accountRepository;
            _notifyAboutAccountDetailModificationOperationService = notifyAboutAccountDetailModificationOperationService;
        }

        protected override EmptyResponse Handle(EditRequest<AccountDetail> request)
        {
            var accountDetail = request.Entity;
            if (accountDetail.IsNew())
            {
                _accountRepository.Create(accountDetail);
                _accountRepository.UpdateAccountBalance(new[] { accountDetail.AccountId });

                var isPlusAccountDetail = accountDetail.OperationTypeId == (int)AccountOperationTypeEnum.AccountChargeIsPlus ||
                                          accountDetail.OperationTypeId == (int)AccountOperationTypeEnum.CashIncomeIsPlus;
                if (isPlusAccountDetail)
                {
                    _notifyAboutAccountDetailModificationOperationService.Notify(accountDetail.Id);
                }
            }
            else
            {
                _accountRepository.Update(accountDetail);
                _accountRepository.UpdateAccountBalance(new[] { accountDetail.AccountId });
            }

            return Response.Empty;
        }
    }
}