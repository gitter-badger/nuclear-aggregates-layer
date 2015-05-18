using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeLocalMessage : EntityTypeBase<EntityTypeLocalMessage>
    {
        public override int Id
        {
            get { return EntityTypeIds.LocalMessage; }
        }

        public override string Description
        {
            get { return "LocalMessage"; }
        }
    }
}