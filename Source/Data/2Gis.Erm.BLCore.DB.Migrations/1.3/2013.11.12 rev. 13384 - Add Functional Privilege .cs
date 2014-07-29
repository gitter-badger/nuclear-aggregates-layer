using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    // 2+: BL\Source\Data\2Gis.Erm.BLCore.DB.Migrations\1.2\2013.11.12 rev. 13384 - Add Functional Privilege .cs
    [Migration(13384, "Добавляем функциональную привилегию редактирования признака является ли клиент рекламным агентством", "y.baranihin")]
    public sealed class Migration13384 : TransactedMigration
    {
        public const int SetIsAdvertisingAgencyClientPropertyCode = 648;
        public const long PrivilegeId = 228623407619506177;
        public const long PrivilegeDepthId = 228623407619506433;

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(string.Format("INSERT INTO [Security].[Privileges](Id, [EntityType], [Operation]) VALUES({0}, NULL, {1})",
                                                             PrivilegeId,
                                                             SetIsAdvertisingAgencyClientPropertyCode));

            context.Connection.ExecuteNonQuery(
                string.Format(
                    "insert into [Security].[FunctionalPrivilegeDepths](Id, PrivilegeId, LocalResourceName, Priority, Mask) values({0}, {1}, 'FPrvDpthGranted', 1, 134)",
                    PrivilegeDepthId,
                    PrivilegeId));
        }
    }
}
