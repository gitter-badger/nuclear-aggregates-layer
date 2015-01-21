using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeNotificationEmail : EntityTypeBase<EntityTypeNotificationEmail>
    {
        public override int Id
        {
            get { return EntityTypeIds.NotificationEmail; }
        }

        public override string Description
        {
            get { return "NotificationEmail"; }
        }
    }
}