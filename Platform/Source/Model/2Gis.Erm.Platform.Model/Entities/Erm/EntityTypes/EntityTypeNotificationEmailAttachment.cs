using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeNotificationEmailAttachment : EntityTypeBase<EntityTypeNotificationEmailAttachment>
    {
        public override int Id
        {
            get { return EntityTypeIds.NotificationEmailAttachment; }
        }

        public override string Description
        {
            get { return "NotificationEmailAttachment"; }
        }
    }
}