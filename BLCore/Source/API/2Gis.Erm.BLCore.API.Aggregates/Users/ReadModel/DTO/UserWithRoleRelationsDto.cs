using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel.DTO
{
    public sealed class UserWithRoleRelationsDto
    {
        public User User { get; set; }
        public IEnumerable<UserRole> RolesRelations { get; set; }
    }
}
