using DoubleGis.Erm.BLCore.API.Aggregates.Roles.ReadModel;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Aggregates.Roles.ReadModel
{
    public sealed class RoleReadModel : IRoleReadModel
    {
        private readonly IFinder _finder;

        public RoleReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public Role GetRole(long roleId)
        {
            return _finder.Find(Specs.Find.ById<Role>(roleId)).One();
        }
    }
}
