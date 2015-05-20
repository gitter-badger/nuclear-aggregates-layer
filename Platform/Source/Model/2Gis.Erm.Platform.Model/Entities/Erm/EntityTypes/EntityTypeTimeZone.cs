using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeTimeZone : EntityTypeBase<EntityTypeTimeZone>
    {
        public override int Id
        {
            get { return EntityTypeIds.TimeZone; }
        }

        public override string Description
        {
            get { return "TimeZone"; }
        }
    }
}