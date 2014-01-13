using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(10148, "Добавляем функциональную привилегию редактирования заглушки РМ")]
    public sealed class Migration10148 : TransactedMigration
    {
        public const int EditDummyAdvertisementCode = 645;

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(string.Format("INSERT INTO [Security].[Privileges](Id, [EntityType], [Operation]) VALUES(673, NULL, {0})", EditDummyAdvertisementCode));

            context.Connection.ExecuteNonQuery("SET IDENTITY_INSERT [Security].[FunctionalPrivilegeDepths] ON");

            context.Connection.ExecuteNonQuery(string.Format("declare @id int; " +
                                                             "select @id = Id from [Security].[Privileges] where Operation = {0}; " +
                                                             "insert into [Security].[FunctionalPrivilegeDepths](Id, PrivilegeId, LocalResourceName, Priority, Mask) values(55, @id, 'FPrvDpthGranted', 1, 134)",
                                                             EditDummyAdvertisementCode));

            context.Connection.ExecuteNonQuery("SET IDENTITY_INSERT [Security].[FunctionalPrivilegeDepths] OFF");
        }
    }
}
