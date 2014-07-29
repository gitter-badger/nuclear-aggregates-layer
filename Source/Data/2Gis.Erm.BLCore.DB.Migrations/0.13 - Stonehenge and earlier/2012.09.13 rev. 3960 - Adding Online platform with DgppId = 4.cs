using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3960, "Добавление платформы Online с идентификатором = 4 и обновление периода размещения у существующих платформ")]
    public sealed class Migration3960 : TransactedMigration
    {
        private const int DesktopPlatformDgppId = 1;
        private const int MobilePlatformDgppId = 2;
        private const int ApiPlatformDgppId = 3;
        private const int OnlinePlatformDgppId = 4;

        #region SQL Statements

        private const string UpdateExistingPlatformsStatement =
            @"
UPDATE [Billing].[Platforms]
SET [PlacementPeriodEnum] = 2
	,[MinPlacementPeriodEnum] = 2
WHERE [DgppId] IN ({0}, {1}, {2})";

        private const string CheckOnlinePlatformStatement =
            @"
SELECT 1
FROM [Billing].[Platforms]
WHERE [DgppId] = {0}";

        private const string InsertOnlinePlatformStatement =
            @"
INSERT INTO [Billing].[Platforms]
([DgppId], [Name], [PlacementPeriodEnum], [MinPlacementPeriodEnum], [OwnerCode], [CreatedBy], [ModifiedBy], [CreatedOn], [ModifiedOn])
VALUES
({0}, 'Online', 2, 2, -1, 1, 1, GETDATE(), GETDATE())
";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            UpdateExistingPlatforms(context);
            AddOnlinePlatform(context);
        }

        private static void UpdateExistingPlatforms(IMigrationContext context)
        {
            var commandSql = string.Format(UpdateExistingPlatformsStatement, DesktopPlatformDgppId, MobilePlatformDgppId, ApiPlatformDgppId);
            context.Connection.ExecuteNonQuery(commandSql);
        }

        private static void AddOnlinePlatform(IMigrationContext context)
        {
            var result = context.Connection.ExecuteScalar(string.Format(CheckOnlinePlatformStatement, OnlinePlatformDgppId));
            if (result != null)
            {
                return;
            }
            context.Connection.ExecuteNonQuery(string.Format(InsertOnlinePlatformStatement, OnlinePlatformDgppId));
        }
    }
}
