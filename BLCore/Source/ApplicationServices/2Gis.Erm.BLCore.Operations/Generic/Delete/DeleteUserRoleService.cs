using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteUserRoleService : IDeleteGenericEntityService<UserRole>
    {
        private readonly IFinder _finder;
        private readonly IUserRepository _userRepository;

        public DeleteUserRoleService(IUserRepository userRepository, IFinder finder)
        {
            _userRepository = userRepository;
            _finder = finder;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var userRole = _finder.Find(new FindSpecification<UserRole>(x => x.Id == entityId)).Single();

                _userRepository.Delete(userRole);

                transaction.Complete();
            }

            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            return new DeleteConfirmationInfo
            {
                EntityCode = string.Empty,
                IsDeleteAllowed = true,
                DeleteConfirmation = string.Empty
            };
        }
    }
}