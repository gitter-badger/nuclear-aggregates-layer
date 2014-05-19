using System;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Append
{
    public class AppendUserRoleService : IAppendGenericEntityService<Role, User> 
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppendUserRoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Append(AppendParams appendParams)
        {
            if (appendParams.ParentId == null || appendParams.AppendedId == null)
            {
                throw new ArgumentException(BLResources.UserIdOrRoleIdIsNotSpecified);
            }

            if (appendParams.ParentType != EntityName.User || appendParams.AppendedType != EntityName.Role)
            {
                throw new ArgumentException(BLResources.EntityNamesShouldBeUserAndRole);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var entity = new UserRole { UserId = appendParams.ParentId.Value, RoleId = appendParams.AppendedId.Value };
                using (var scope = _unitOfWork.CreateScope())
                {
                    var userRepository = scope.CreateRepository<IUserRepository>();

                    userRepository.CreateOrUpdate(entity);

                    scope.Complete();
                }
                transaction.Complete();
            }
        }
    }
}
