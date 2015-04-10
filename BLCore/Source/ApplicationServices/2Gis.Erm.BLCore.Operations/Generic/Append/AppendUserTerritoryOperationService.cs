using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Append
{
    public class AppendUserTerritoryOperationService : IAppendGenericEntityService<Territory, User>
    {
        private readonly IUserRepository _userRepository;

        public AppendUserTerritoryOperationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Append(AppendParams appendParams)
        {
            if (appendParams.ParentId == null || appendParams.AppendedId == null)
            {
                throw new ArgumentException(BLResources.UserIdOtTerritoryIdIsNotSpecified);
            }

            if (appendParams.ParentType != EntityName.User || appendParams.AppendedType != EntityName.Territory)
            {
                throw new ArgumentException(BLResources.EntityNamesShouldBeUserAndTerritory);
            }

            var entity = new UserTerritory { UserId = appendParams.ParentId.Value, TerritoryId = appendParams.AppendedId.Value };
            _userRepository.CreateOrUpdate(entity);
        }
    }
}
