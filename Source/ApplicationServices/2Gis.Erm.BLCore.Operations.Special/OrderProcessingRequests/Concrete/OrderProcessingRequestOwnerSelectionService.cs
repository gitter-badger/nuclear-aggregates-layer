using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    // FIXME {all, 13.01.2014}: нужна конвертация в readmodel с перемещением в нужную сборку
    public sealed class OrderProcessingRequestOwnerSelectionService : IOrderProcessingRequestOwnerSelectionService
    {
        private const int DirectorRoleId = 2;
        private readonly IFinder _finder;
        private readonly string _reserveUserAccountName;

        public OrderProcessingRequestOwnerSelectionService(IFinder finder, IAppSettings appSettings)
        {
            _finder = finder;
            _reserveUserAccountName = appSettings.ReserveUserAccount;
        }

        public User GetOwner(long userId)
        {
            return _finder.Find(UserSpecifications.Find.ActiveNotService() & Specs.Find.ById<User>(userId))
                          .SingleOrDefault();
        }

        public User GetOrganizationUnitDirector(long organizationUnitId)
        {
            return _finder.Find(UserSpecifications.Find.ActiveNotService())
                          .FirstOrDefault(user => user.UserRoles.Any(role => role.RoleId == DirectorRoleId)
                                                  && user.UserOrganizationUnits.Any(unit => unit.OrganizationUnitId == organizationUnitId));
        }

        public User GetReserveUser()
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<User>())
                          .SingleOrDefault(user => user.Account == _reserveUserAccountName);
        }
    }
}
