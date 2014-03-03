using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.DTO;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.ReadModel
{
    public interface IBankReadModel : ISimplifiedModelConsumerReadModel
    {
        Bank GetBank(long bankId);
        DictionaryEntityInstanceDto GetDictionaryEntityInstanceDto(Bank bank);
        bool IsBankUsed(long bankId);
    }
}