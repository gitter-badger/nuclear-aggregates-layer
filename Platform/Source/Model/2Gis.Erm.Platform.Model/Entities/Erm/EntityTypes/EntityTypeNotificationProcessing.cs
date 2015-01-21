using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeNotificationProcessing : EntityTypeBase<EntityTypeNotificationProcessing>
    {
        public override int Id
        {
            get { return EntityTypeIds.NotificationProcessing; }
        }

        public override string Description
        {
            get { return "NotificationProcessing"; }
        }
    }
}