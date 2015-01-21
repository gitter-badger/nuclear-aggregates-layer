using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeActionsHistoryDetail : EntityTypeBase<EntityTypeActionsHistoryDetail>
    {
        public override int Id
        {
            get { return EntityTypeIds.ActionsHistoryDetail; }
        }

        public override string Description
        {
            get { return "ActionsHistoryDetail"; }
        }
    }
}