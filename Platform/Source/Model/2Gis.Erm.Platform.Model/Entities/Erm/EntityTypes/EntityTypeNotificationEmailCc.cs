using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeNotificationEmailCc : EntityTypeBase<EntityTypeNotificationEmailCc>
    {
        public override int Id
        {
            get { return EntityTypeIds.NotificationEmailCc; }
        }

        public override string Description
        {
            get { return "NotificationEmailCc"; }
        }
    }
}