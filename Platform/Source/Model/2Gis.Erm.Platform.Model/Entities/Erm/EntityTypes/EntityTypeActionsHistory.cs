using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeActionsHistory : EntityTypeBase<EntityTypeActionsHistory>
    {
        public override int Id
        {
            get { return EntityTypeIds.ActionsHistory; }
        }

        public override string Description
        {
            get { return "ActionsHistory"; }
        }
    }
}