using System.Collections.ObjectModel;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.DTO;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel.ReadModel.Banks
{
    public sealed class BankReadModel : IBankReadModel
    {
        private readonly IFinder _finder;
        private readonly IDictionaryEntityPropertiesConverter<Bank> _dynamicEntityEntityPropertiesConverter;

        public BankReadModel(IFinder finder,
                             IDictionaryEntityPropertiesConverter<Bank> dynamicEntityEntityPropertiesConverter)
        {
            _finder = finder;
            _dynamicEntityEntityPropertiesConverter = dynamicEntityEntityPropertiesConverter;
        }

        public Bank GetBank(long bankId)
        {
            var activityInstanceDto = GetDictionaryEntityInstanceDto(bankId).Single();
            return _dynamicEntityEntityPropertiesConverter.ConvertFromDynamicEntityInstance(activityInstanceDto.DictionaryEntityInstance,
                                                                                      activityInstanceDto.DictionaryEntityPropertyInstances);
        }

        public DictionaryEntityInstanceDto GetDictionaryEntityInstanceDto(Bank bank)
        {
            var dto = GetDictionaryEntityInstanceDto(bank.Id).SingleOrDefault();

            var bankPropertyInstances = dto != null ? dto.DictionaryEntityPropertyInstances : new Collection<DictionaryEntityPropertyInstance>();
            var tuple = _dynamicEntityEntityPropertiesConverter.ConvertToDynamicEntityInstance(bank, bankPropertyInstances, null);

            return new DictionaryEntityInstanceDto
                {
                    DictionaryEntityInstance = tuple.Item1,
                    DictionaryEntityPropertyInstances = tuple.Item2
                };
        }

        public bool IsBankUsed(long bankId)
        {
            return _finder.Find(BLCore.Aggregates.SimplifiedModel.DictionaryEntity.ReadModel.DictionaryEntitySpecs.DictionaryEntity.Find.ByEntityName(EntityName.Bank))
                          .Any(x => x.DictionaryEntityPropertyInstances.Any(y => y.PropertyId == BankIdIdentity.Instance.Id && y.NumericValue == bankId));
        }

        private IQueryable<DictionaryEntityInstanceDto> GetDictionaryEntityInstanceDto(long bankId)
        {
            return _finder.Find<DictionaryEntityInstance, DictionaryEntityInstanceDto>(BLCore.Aggregates.SimplifiedModel.DictionaryEntity.ReadModel.DictionaryEntitySpecs.DictionaryEntity.Select.DictionaryEntityInstanceDto(),
                                                                                       Specs.Find.ById<DictionaryEntityInstance>(bankId));
        }
    }
}