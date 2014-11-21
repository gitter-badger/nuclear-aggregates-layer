using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel
{
    public interface IBankService : ISimplifiedModelConsumer
    {
        int Create(Bank bank);
        int Update(Bank bank);
        int Delete(Bank bank);
    }
}