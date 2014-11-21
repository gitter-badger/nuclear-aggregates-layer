using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public interface IOrderPersistenceService : IPersistenceService<Order>
    {
        long GenerateNextOrderUniqueNumber();
    }
}