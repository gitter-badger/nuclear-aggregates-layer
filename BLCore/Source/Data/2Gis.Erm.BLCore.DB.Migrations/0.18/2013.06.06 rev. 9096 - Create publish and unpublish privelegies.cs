using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(9096, "Добавляем функциональные привилегии \"Публикация шаблонов РМ\"")]
    public sealed class Migration9096 : TransactedMigration
    {
        public const int AdvertisementTemplatePublishCode = 643;
        public const int AdvertisementTemplateUnpublishCode = 644;

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(string.Format("INSERT INTO [Security].[Privileges](Id, [EntityType], [Operation]) VALUES(671, NULL, {0})", AdvertisementTemplatePublishCode));

            context.Connection.ExecuteNonQuery("SET IDENTITY_INSERT [Security].[FunctionalPrivilegeDepths] ON");

            context.Connection.ExecuteNonQuery(string.Format("declare @id int; " +
                                                             "select @id = Id from [Security].[Privileges] where Operation = {0}; " +
                                                             "insert into [Security].[FunctionalPrivilegeDepths](Id, PrivilegeId, LocalResourceName, Priority, Mask) values(53 , @id, 'FPrvDpthGranted', 1, 134)",
                                                             AdvertisementTemplatePublishCode));

            context.Connection.ExecuteNonQuery(string.Format("INSERT INTO [Security].[Privileges](Id, [EntityType], [Operation]) VALUES(672, NULL, {0})", AdvertisementTemplateUnpublishCode));

            context.Connection.ExecuteNonQuery(string.Format("declare @id int; " +
                                                             "select @id = Id from [Security].[Privileges] where Operation = {0}; " +
                                                             "insert into [Security].[FunctionalPrivilegeDepths](Id, PrivilegeId, LocalResourceName, Priority, Mask) values(54, @id, 'FPrvDpthGranted', 1, 134)",
                                                             AdvertisementTemplateUnpublishCode));

            context.Connection.ExecuteNonQuery("SET IDENTITY_INSERT [Security].[FunctionalPrivilegeDepths] OFF");
        }
    }
}
