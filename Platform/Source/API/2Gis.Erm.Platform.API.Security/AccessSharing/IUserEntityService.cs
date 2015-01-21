using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Simplified;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.API.Security.AccessSharing
{
    public interface IUserEntityService : ISimplifiedModelConsumer
    {
        int Add(UserEntity userEntity);
        int DeleteSharings(IEntityType entityName, long entityId); 
    }
}