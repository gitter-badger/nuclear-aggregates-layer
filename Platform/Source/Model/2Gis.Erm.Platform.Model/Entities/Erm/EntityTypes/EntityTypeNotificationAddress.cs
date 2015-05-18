using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeNotificationAddress : EntityTypeBase<EntityTypeNotificationAddress>
    {
        public override int Id
        {
            get { return EntityTypeIds.NotificationAddress; }
        }

        public override string Description
        {
            get { return "NotificationAddress"; }
        }
    }
}