using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Append
{
    public class AppendUserOrganizationUnitOperationService : IAppendGenericEntityService<OrganizationUnit, User>
    {
        private readonly IUserRepository _userRepository;

        public AppendUserOrganizationUnitOperationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Append(AppendParams appendParams)
        {
            if (appendParams.ParentId == null || appendParams.AppendedId == null)
            {
                throw new ArgumentException(BLResources.UserIdOrOrgUnitIdIsNotSpecified);
            }

            if (appendParams.ParentType != EntityName.User || appendParams.AppendedType != EntityName.OrganizationUnit)
            {
                throw new ArgumentException(BLResources.EntityNamesShouldBeUserAndOrgUnit);
            }

            var entity = new UserOrganizationUnit { UserId = appendParams.ParentId.Value, OrganizationUnitId = appendParams.AppendedId.Value };
            _userRepository.CreateOrUpdate(entity);
        }
    }
}
