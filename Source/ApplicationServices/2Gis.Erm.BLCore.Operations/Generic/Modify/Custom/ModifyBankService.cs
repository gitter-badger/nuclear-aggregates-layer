using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity;
using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    public class ModifyBankService : IModifySimplifiedModelEntityService<Bank>
    {
        private readonly IDynamicDictionaryEntityService _dynamicDictionaryEntityService;
        private readonly ISimplifiedModelEntityObtainer<Bank> _bankObtainer;
        private readonly IBankReadModel _bankReadModel;

        public ModifyBankService(IDynamicDictionaryEntityService dynamicDictionaryEntityService,
                                 ISimplifiedModelEntityObtainer<Bank> bankObtainer,
                                 IBankReadModel bankReadModel)
        {
            _dynamicDictionaryEntityService = dynamicDictionaryEntityService;
            _bankObtainer = bankObtainer;
            _bankReadModel = bankReadModel;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var bank = _bankObtainer.ObtainSimplifiedModelEntity(domainEntityDto);
            var dictionaryEntityInstanceDto = _bankReadModel.GetDictionaryEntityInstanceDto(bank);
            if (bank.IsNew())
            {
                return _dynamicDictionaryEntityService.Create(dictionaryEntityInstanceDto.DictionaryEntityInstance,
                                                              dictionaryEntityInstanceDto.DictionaryEntityPropertyInstances);
            }

            _dynamicDictionaryEntityService.Update(dictionaryEntityInstanceDto.DictionaryEntityInstance, dictionaryEntityInstanceDto.DictionaryEntityPropertyInstances);
            return bank.Id;
        }
    }
}