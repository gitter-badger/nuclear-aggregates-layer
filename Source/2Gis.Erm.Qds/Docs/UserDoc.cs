using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Docs
{
    public sealed class UserDoc
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<string> TerritoryIds { get; set; }
        public ICollection<string> OrganizationUnitIds { get; set; }

        public ICollection<OperationPermission> Permissions { get; set; }
        public ICollection<string> Tags { get; set; }
    }
}