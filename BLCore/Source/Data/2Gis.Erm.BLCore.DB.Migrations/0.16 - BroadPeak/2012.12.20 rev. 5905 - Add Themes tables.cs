using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5905, "Добавляем таблицы Themes, ThemeTemplates, ThemeCategories, ThemeOrganizationUnits")]
    public sealed class Migration5905 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddThemeTemplatesTable(context);
            AddThemesTable(context);
            AddThemeCategoriesTable(context);
            AddThemeOrganizationUnitsTable(context);
        }

        private void AddThemeTemplatesTable(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.ThemeTemplates.Name, ErmTableNames.ThemeTemplates.Schema];
            if (table != null)
                return;

            table = new Table(context.Database, ErmTableNames.ThemeTemplates.Name, ErmTableNames.ThemeTemplates.Schema);

            table.Columns.Add(new Column(table, "Id", DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 });
            table.CreateField("FileId", DataType.Int, false);
            table.CreateField("TemplateCode", DataType.Int, false);
            ErmTableUtilsForOldIntKeys.CreateSecureEntityStandartColumns(table);

            table.Create();
            table.CreatePrimaryKey();
            table.CreateForeignKey("FileId", ErmTableNames.Files, "Id");
        }

        private void AddThemesTable(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.Themes.Name, ErmTableNames.Themes.Schema];
            if (table != null)
                return;

            table = new Table(context.Database, ErmTableNames.Themes.Name, ErmTableNames.Themes.Schema);

            table.Columns.Add(new Column(table, "Id", DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 });
            table.CreateField("ThemeTemplateId", DataType.Int, false);
            table.CreateField("BannerFileId", DataType.Int, false);
            table.CreateField("Name", DataType.NVarChar(256), false);
            table.CreateField("Description", DataType.NVarChar(256), false);
            table.CreateField("BeginDistribution", DataType.DateTime2(2), false);
            table.CreateField("EndDistribution", DataType.DateTime2(2), false);
            table.CreateField("IsDefault", DataType.Bit, false);
            ErmTableUtilsForOldIntKeys.CreateSecureEntityStandartColumns(table);

            table.Create();
            table.CreatePrimaryKey();
            table.CreateForeignKey("ThemeTemplateId", ErmTableNames.ThemeTemplates, "Id");
            table.CreateForeignKey("BannerFileId", ErmTableNames.Files, "Id");
        }

        private void AddThemeCategoriesTable(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.ThemeCategories.Name, ErmTableNames.ThemeCategories.Schema];
            if (table != null)
                return;

            table = new Table(context.Database, ErmTableNames.ThemeCategories.Name, ErmTableNames.ThemeCategories.Schema);

            table.Columns.Add(new Column(table, "Id", DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 });
            table.CreateField("ThemeId", DataType.Int, false);
            table.CreateField("CategoryId", DataType.Int, false);
            ErmTableUtilsForOldIntKeys.CreateAuditableEntityColumns(table);
            table.CreateTimestampColumn();

            table.Create();
            table.CreatePrimaryKey();
            table.CreateForeignKey("ThemeId", ErmTableNames.Themes, "Id");
            table.CreateForeignKey("CategoryId", ErmTableNames.Categories, "Id");
        }

        private void AddThemeOrganizationUnitsTable(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.ThemeOrganizationUnits.Name, ErmTableNames.ThemeOrganizationUnits.Schema];
            if (table != null)
                return;

            table = new Table(context.Database, ErmTableNames.ThemeOrganizationUnits.Name, ErmTableNames.ThemeOrganizationUnits.Schema);

            table.Columns.Add(new Column(table, "Id", DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 });
            table.CreateField("ThemeId", DataType.Int, false);
            table.CreateField("OrganizationUnitId", DataType.Int, false);
            ErmTableUtilsForOldIntKeys.CreateAuditableEntityColumns(table);
            table.CreateTimestampColumn();

            table.Create();
            table.CreatePrimaryKey();
            table.CreateForeignKey("ThemeId", ErmTableNames.Themes, "Id");
            table.CreateForeignKey("OrganizationUnitId", ErmTableNames.OrganizationUnits, "Id");
        }
    }
}