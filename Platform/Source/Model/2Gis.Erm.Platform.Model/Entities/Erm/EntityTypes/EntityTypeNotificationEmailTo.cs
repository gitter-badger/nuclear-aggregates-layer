using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeNotificationEmailTo : EntityTypeBase<EntityTypeNotificationEmailTo>
    {
        public override int Id
        {
            get { return EntityTypeIds.NotificationEmailTo; }
        }

        public override string Description
        {
            get { return "NotificationEmailTo"; }
        }
    }
}