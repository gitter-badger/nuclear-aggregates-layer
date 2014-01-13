using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3448, "Переносим все шаблоны ПФ в общую таблицу")]
    public sealed class Migration3448 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AlterBranchOfficeOrganizationUnitTable(context);

            NormalizeFileTables(context);
        }

        #region normalize file tables

        private static void NormalizeFileTables(IMigrationContext context)
        {
            AlterBargainFilesFileIdAsNotNullable(context);
            DropFirmFilesTable(context);
            AlterOperationsFileIdAsNotNullable(context);
            AlterOrderFilesFileIdAsNotNullable(context);
            WithdrawalInfosDropFileIdColumn(context);
        }

        private static void AlterBargainFilesFileIdAsNotNullable(IMigrationContext context)
        {
            var table = context.Database.Tables["BargainFiles", ErmSchemas.Billing];

            var fileIdColumn = table.Columns["FileId"];
            if (!fileIdColumn.Nullable)
                return;

            fileIdColumn.Nullable = false;
            fileIdColumn.Alter();
        }

        private static void DropFirmFilesTable(IMigrationContext context)
        {
            var table = context.Database.Tables["FirmFiles", ErmSchemas.BusinessDirectory];
            if (table == null)
                return;

            table.Drop();
        }

        private static void AlterOperationsFileIdAsNotNullable(IMigrationContext context)
        {
            var table = context.Database.Tables["Operations", ErmSchemas.Shared];

            var fileIdColumn = table.Columns["FileId"];
            if (!fileIdColumn.Nullable)
                return;

            fileIdColumn.Nullable = false;
            fileIdColumn.Alter();
        }

        private static void AlterOrderFilesFileIdAsNotNullable(IMigrationContext context)
        {
            var table = context.Database.Tables["OrderFiles", ErmSchemas.Billing];

            var fileIdColumn = table.Columns["FileId"];
            if (!fileIdColumn.Nullable)
                return;

            fileIdColumn.Nullable = false;
            fileIdColumn.Alter();
        }

        private static void WithdrawalInfosDropFileIdColumn(IMigrationContext context)
        {
            var table = context.Database.Tables["WithdrawalInfos", ErmSchemas.Billing];

            var foreignKey = table.ForeignKeys["FK_WithdrawingInfos_Files"];
            if (foreignKey != null)
            {
                foreignKey.Drop();
            }

            var fileIdColumn = table.Columns["FileId"];
            if (fileIdColumn != null)
            {
                fileIdColumn.Drop();
            }
        }

        #endregion

        private static void AlterBranchOfficeOrganizationUnitTable(IMigrationContext context)
        {
            AlterBranchOfficeOrganizationUnitIdAsNullable(context);
            AlterFileIdAsNotNullable(context);

            RenameBranchOfficeOrganizationUnit(context);
            RenameColumnsAndKeys(context);
        }

        private static void RenameColumnsAndKeys(IMigrationContext context)
        {
            var table = context.Database.Tables["PrintFormTemplates", ErmSchemas.Billing];

            var fileKindColumn = table.Columns["FileKind"];
            if (fileKindColumn != null)
            {
                fileKindColumn.Rename("TemplateCode");
            }

            var foreignKey1 = table.ForeignKeys["FK_BranchOfficeOrganizationUnitFiles_BranchOfficeOrganizationUnits"];
            if (foreignKey1 != null)
            {
                foreignKey1.Drop();

                var foreignKey = new ForeignKey(table, "FK_PrintFormTemplates_BranchOfficeOrganizationUnits");
                foreignKey.Columns.Add(new ForeignKeyColumn(foreignKey, "BranchOfficeOrganizationUnitId", "Id"));
                foreignKey.ReferencedTable = "BranchOfficeOrganizationUnits";
                foreignKey.ReferencedTableSchema = ErmSchemas.Billing;
                foreignKey.Create();
            }

            var foreignKey2 = table.ForeignKeys["FK_BranchOfficeOrganizationUnitFiles_Files1"];
            if (foreignKey2 != null)
            {
                foreignKey2.Drop();

                var foreignKey = new ForeignKey(table, "FK_PrintFormTemplates_Files");
                foreignKey.Columns.Add(new ForeignKeyColumn(foreignKey, "FileId", "Id"));
                foreignKey.ReferencedTable = "Files";
                foreignKey.ReferencedTableSchema = ErmSchemas.Shared;
                foreignKey.Create();
            }
        }

        private static void RenameBranchOfficeOrganizationUnit(IMigrationContext context)
        {
            var table = context.Database.Tables["BranchOfficeOrganizationUnitFiles", ErmSchemas.Billing];
            if (table == null)
                return;

            table.Rename("PrintFormTemplates");
        }

        private static void AlterFileIdAsNotNullable(IMigrationContext context)
        {
            var table = context.Database.Tables["BranchOfficeOrganizationUnitFiles", ErmSchemas.Billing];
            if (table == null)
                return;

            var fileIdColumn = table.Columns["FileId"];
            if (!fileIdColumn.Nullable)
                return;

            fileIdColumn.Nullable = false;
            fileIdColumn.Alter();
        }

        private static void AlterBranchOfficeOrganizationUnitIdAsNullable(IMigrationContext context)
        {
            var table = context.Database.Tables["BranchOfficeOrganizationUnitFiles", ErmSchemas.Billing];
            if (table == null)
                return;

            BranchOfficeOrganizationUnitCleanData(context);

            var branchOfficeOrganizationUnitIdColumn = table.Columns["BranchOfficeOrganizationUnitId"];
            if (branchOfficeOrganizationUnitIdColumn.Nullable)
                return;

            branchOfficeOrganizationUnitIdColumn.Nullable = true;
            branchOfficeOrganizationUnitIdColumn.Alter();
        }

        private static void BranchOfficeOrganizationUnitCleanData(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"
            DECLARE @fileIds TABLE (Id INT NOT NULL)

            INSERT INTO @fileIds
            SELECT DISTINCT FileId FROM Billing.BranchOfficeOrganizationUnitFiles

            DELETE FROM Billing.BranchOfficeOrganizationUnitFiles
            DELETE FROM Shared.FileBinaries WHERE Id IN (SELECT Id FROM @fileIds)
            DELETE FROM Shared.Files WHERE Id IN (SELECT Id FROM @fileIds)
            ");
        }
    }
}