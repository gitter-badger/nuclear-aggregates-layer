using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel.ReadModel
{
    public interface IBankReadModel : ISimplifiedModelConsumerReadModel
    {
        Bank GetBank(long bankId);
        string GetBankName(long bankId);
        bool IsBankUsed(long bankId);
    }
}