using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public interface IUserPersistenceService : IPersistenceService<User>
    {
        int CheckUserParentnessRecursion(long userId, long parentId);
    }
}