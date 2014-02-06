using System.Collections.ObjectModel;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.DTO;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.ReadModel
{
    public class BankReadModel : IBankReadModel
    {
        private readonly IFinder _finder;
        private readonly IDynamicEntityPropertiesConverter<Bank, DictionaryEntityInstance, DictionaryEntityPropertyInstance> _dynamicEntityPropertiesConverter;

        public BankReadModel(IFinder finder,
                             IDynamicEntityPropertiesConverter<Bank, DictionaryEntityInstance, DictionaryEntityPropertyInstance> dynamicEntityPropertiesConverter)
        {
            _finder = finder;
            _dynamicEntityPropertiesConverter = dynamicEntityPropertiesConverter;
        }

        public Bank GetBank(long bankId)
        {
            var activityInstanceDto = GetDictionaryEntityInstanceDto(bankId).Single();
            return _dynamicEntityPropertiesConverter.ConvertFromDynamicEntityInstance(activityInstanceDto.DictionaryEntityInstance,
                                                                                      activityInstanceDto.DictionaryEntityPropertyInstances);
        }

        public DictionaryEntityInstanceDto GetDictionaryEntityInstanceDto(Bank bank)
        {
            var dto = GetDictionaryEntityInstanceDto(bank.Id).SingleOrDefault();

            var activityPropertyInstances = dto != null ? dto.DictionaryEntityPropertyInstances : new Collection<DictionaryEntityPropertyInstance>();
            var tuple = _dynamicEntityPropertiesConverter.ConvertToDynamicEntityInstance(bank, activityPropertyInstances, null);
            
            return new DictionaryEntityInstanceDto
                {
                    DictionaryEntityInstance = tuple.Item1,
                    DictionaryEntityPropertyInstances = tuple.Item2
                };
        }

        private IQueryable<DictionaryEntityInstanceDto> GetDictionaryEntityInstanceDto(long bankId)
        {
            return _finder.Find<DictionaryEntityInstance, DictionaryEntityInstanceDto>(DictionaryEntitySpecs.DictionaryEntity.Select.DictionaryEntityInstanceDto(),
                                                                                       Specs.Find.ById<DictionaryEntityInstance>(bankId));
        }
    }
}