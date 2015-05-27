using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.UsingHandler
{
    public class ModifyAccountUsingHandlerService : IModifyBusinessModelEntityService<Account>
    {
        private readonly IBusinessModelEntityObtainer<Account> _businessModelEntityObtainer;
        private readonly IPublicService _publicService;

        public ModifyAccountUsingHandlerService(IBusinessModelEntityObtainer<Account> businessModelEntityObtainer, IPublicService publicService)
        {
            _businessModelEntityObtainer = businessModelEntityObtainer;
            _publicService = publicService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _businessModelEntityObtainer.ObtainBusinessModelEntity(domainEntityDto);

            // При операциях из UI политики безопасности НЕ игнорируем
            var response = (EditAccountResponse)_publicService.Handle(new EditAccountRequest { Entity = entity, IgnoreSecurity = false });
            return response.AccountId;
        }
    }
}