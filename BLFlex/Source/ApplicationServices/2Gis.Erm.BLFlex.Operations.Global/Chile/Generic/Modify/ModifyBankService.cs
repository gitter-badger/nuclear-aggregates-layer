using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify
{
    public class ModifyBankService : IModifySimplifiedModelEntityService<Bank>
    {
        private readonly ISimplifiedModelEntityObtainer<Bank> _bankObtainer;
        private readonly IBankService _bankService;

        public ModifyBankService(ISimplifiedModelEntityObtainer<Bank> bankObtainer,
                                 IBankService bankService)
        {
            _bankObtainer = bankObtainer;
            _bankService = bankService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var bank = _bankObtainer.ObtainSimplifiedModelEntity(domainEntityDto);
            if (bank.IsNew())
            {
                _bankService.Create(bank);
            }
            else
            {
                _bankService.Update(bank);
            }

            return bank.Id;
        }
    }
}