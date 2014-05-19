using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Roles.Dto
{
    public sealed class EntityPrivilegeInfo
    {
        public EntityName EntityName { get; set; }
        public string EntityNameLocalized { get; set; }

        public IEnumerable<PrivilegeDto> PrivilegeInfoList { get; set; }
    }
}