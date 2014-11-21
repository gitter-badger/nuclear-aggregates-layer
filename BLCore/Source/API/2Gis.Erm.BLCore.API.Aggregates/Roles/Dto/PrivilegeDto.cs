using DoubleGis.Erm.Platform.API.Security.EntityAccess;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Roles.Dto
{
    public sealed class PrivilegeDto
    {
        public long PrivilegeId { get; set; }
        public EntityAccessTypes Operation { get; set; }
        public string NameLocalized { get; set; }

        public EntityPrivilegeDepthState PrivilegeDepthMask { get; set; }
    }
}