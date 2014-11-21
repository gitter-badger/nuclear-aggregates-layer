using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    // Миграция изменилась из-за мержа в Main
    [Migration(9086, "Добавляем функциональную привилегию \"Верификация РМ\"")]
    public sealed class Migration9086 : TransactedMigration
    {
        public const int AdvertisementVerificationCode = 642;
        public const int AdvertisementVerificationPrivelegeId = 674;
        public const int AdvertisementVerificationPrivelegeDepthId = 56;

        protected override void ApplyOverride(IMigrationContext context)
        {
            var privilegesId = context.Database.GetTable(ErmTableNames.Privileges).Columns["Id"];
            if (privilegesId.Identity)
            {
                context.Connection.ExecuteNonQuery(string.Format("INSERT INTO [Security].[Privileges]( [EntityType], [Operation]) VALUES(NULL, {0})", AdvertisementVerificationCode));
            }
            else
            {
                context.Connection.ExecuteNonQuery(
                    string.Format(
                        "IF NOT EXISTS(SELECT * FROM [Security].[Privileges] WHERE Id = {1} AND Operation = {0}) INSERT INTO [Security].[Privileges]([Id], [EntityType], [Operation]) VALUES({1}, NULL, {0})",
                        AdvertisementVerificationCode,
                        AdvertisementVerificationPrivelegeId));
            }

            var functionalPrivilegeDepthsId = context.Database.GetTable(ErmTableNames.FunctionalPrivilegeDepths).Columns["Id"];
            if (functionalPrivilegeDepthsId.Identity)
            {
                context.Connection.ExecuteNonQuery(string.Format("declare @id int; " +
                                                                 "select @id = Id from [Security].[Privileges] where Operation = {0}; " +
                                                                 "insert into [Security].[FunctionalPrivilegeDepths](PrivilegeId, LocalResourceName, Priority, Mask) values(@id, 'FPrvDpthGranted', 1, 134)",
                                                                 AdvertisementVerificationCode));
        }
            else
            {
                context.Connection.ExecuteNonQuery(string.Format("IF NOT EXISTS(SELECT * FROM [Security].[FunctionalPrivilegeDepths] WHERE PrivilegeId = {0} AND Id = {1})insert into [Security].[FunctionalPrivilegeDepths](Id, PrivilegeId, LocalResourceName, Priority, Mask) values({1}, {0}, 'FPrvDpthGranted', 1, 134)",
                                                                 AdvertisementVerificationPrivelegeId,
                                                                 AdvertisementVerificationPrivelegeDepthId));
            }
        }
    }
}