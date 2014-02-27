using System.Collections.ObjectModel;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.DTO;
using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.ReadModel
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

        public bool IsBankUsed(long bankId)
        {
            return _finder.Find(DictionaryEntitySpecs.DictionaryEntity.Find.ByEntityName(EntityName.Bank))
                          .Any(x => x.DictionaryEntityPropertyInstances.Any(y => y.PropertyId == BankIdIdentity.Instance.Id && y.NumericValue == bankId));
        }

        private IQueryable<DictionaryEntityInstanceDto> GetDictionaryEntityInstanceDto(long bankId)
        {
            return _finder.Find<DictionaryEntityInstance, DictionaryEntityInstanceDto>(DictionaryEntitySpecs.DictionaryEntity.Select.DictionaryEntityInstanceDto(),
                                                                                       Specs.Find.ById<DictionaryEntityInstance>(bankId));
        }
    }
}