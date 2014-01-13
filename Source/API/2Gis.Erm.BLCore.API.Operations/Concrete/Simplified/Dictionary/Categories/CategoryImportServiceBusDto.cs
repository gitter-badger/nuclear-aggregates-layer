using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories
{
    public sealed class CategoryImportServiceBusDto
    {
        public long Id;
        public string Name;
        public long? ParentId;
        public int Level;
        public string Comment;
        public bool IsHidden;
        public bool IsDeleted;
        public IEnumerable<int> OrganizationUnitsDgppIds;
    }
}