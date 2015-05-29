using System.Collections.Generic;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Roles.Dto
{
    public sealed class EntityPrivilegeInfo
    {
        public IEntityType EntityName { get; set; }
        public string EntityNameLocalized { get; set; }

        public IEnumerable<PrivilegeDto> PrivilegeInfoList { get; set; }
    }
}