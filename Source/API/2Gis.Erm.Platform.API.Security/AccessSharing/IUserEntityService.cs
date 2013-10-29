using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.API.Security.AccessSharing
{
    public interface IUserEntityService : ISimplifiedModelConsumer
    {
        int Add(UserEntity userEntity);
        int DeleteSharings(EntityName entityName, long entityId); 
    }
}