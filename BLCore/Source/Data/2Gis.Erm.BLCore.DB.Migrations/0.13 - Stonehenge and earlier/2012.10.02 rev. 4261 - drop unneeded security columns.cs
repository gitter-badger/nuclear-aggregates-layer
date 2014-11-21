using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4261, "Удаление неиспользуемых столбцов/колонок из подсистемы безопасности")]
    public sealed class Migration4261 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            OptimizeEntityAccessRights(context);
            OptimizeEntityPrivilegeDepths(context);
            OptimizeFunctionalPrivilegeDepths(context);
            OptimizeEntityTypes(context);
            DropModulesTable(context);

            OptimizeRoles(context);
            OptimizeRolePrivileges(context);
        }

        private static void OptimizeRolePrivileges(IMigrationContext context)
        {
            var table = context.Database.Tables["RolePrivileges", ErmSchemas.Security];
            var column = table.Columns["IncludeSubordinates"];

            if (column == null)
                return;

            column.Drop();
        }

        private static void OptimizeRoles(IMigrationContext context)
        {
            var table = context.Database.Tables["Roles", ErmSchemas.Security];

            var isDeletedColumn = table.Columns["IsDeleted"];
            if (isDeletedColumn != null)
            {
                context.Database.ExecuteNonQuery(@"DELETE FROM Security.Roles WHERE IsDeleted = 1");                
                isDeletedColumn.Drop();
            }

            DropColumn(context, "Roles", ErmSchemas.Security, "IsActive");
            DropColumn(context, "Roles", ErmSchemas.Security, "IsAdmin");
        }

        private static void OptimizeEntityTypes(IMigrationContext context)
        {
            DropModuleForeignKey(context);

            DropColumn(context, "EntityTypes", ErmSchemas.Security, "ModuleId");
            DropColumn(context, "EntityTypes", ErmSchemas.Security, "IsDeleted");
            DropColumn(context, "EntityTypes", ErmSchemas.Security, "IsActive");
            DropColumn(context, "EntityTypes", ErmSchemas.Security, "CreatedBy");
            DropColumn(context, "EntityTypes", ErmSchemas.Security, "ModifiedBy");
            DropColumn(context, "EntityTypes", ErmSchemas.Security, "CreatedOn");
            DropColumn(context, "EntityTypes", ErmSchemas.Security, "ModifiedOn");
            DropColumn(context, "EntityTypes", ErmSchemas.Security, "TimeStamp");
        }

        private static void DropModuleForeignKey(IMigrationContext context)
        {
            var table = context.Database.Tables["EntityTypes", ErmSchemas.Security];
            var foreignKey = table.ForeignKeys["FK_EntityTypes_Modules"];
            if (foreignKey == null)
                return;

            foreignKey.Drop();
        }

        private static void DropModulesTable(IMigrationContext context)
        {
            var table = context.Database.Tables["Modules", ErmSchemas.Security];
            if (table == null)
                return;

            table.Drop();
        }

        private static void OptimizeFunctionalPrivilegeDepths(IMigrationContext context)
        {
            DropColumn(context, "FunctionalPrivilegeDepths", ErmSchemas.Security, "IsDeleted");
            DropColumn(context, "FunctionalPrivilegeDepths", ErmSchemas.Security, "IsActive");
            DropColumn(context, "FunctionalPrivilegeDepths", ErmSchemas.Security, "CreatedBy");
            DropColumn(context, "FunctionalPrivilegeDepths", ErmSchemas.Security, "ModifiedBy");
            DropColumn(context, "FunctionalPrivilegeDepths", ErmSchemas.Security, "CreatedOn");
            DropColumn(context, "FunctionalPrivilegeDepths", ErmSchemas.Security, "ModifiedOn");
            DropColumn(context, "FunctionalPrivilegeDepths", ErmSchemas.Security, "TimeStamp");
        }

        private static void OptimizeEntityPrivilegeDepths(IMigrationContext context)
        {
            DropColumn(context, "EntityPrivilegeDepths", ErmSchemas.Security, "IsDeleted");
            DropColumn(context, "EntityPrivilegeDepths", ErmSchemas.Security, "IsActive");
            DropColumn(context, "EntityPrivilegeDepths", ErmSchemas.Security, "CreatedBy");
            DropColumn(context, "EntityPrivilegeDepths", ErmSchemas.Security, "ModifiedBy");
            DropColumn(context, "EntityPrivilegeDepths", ErmSchemas.Security, "CreatedOn");
            DropColumn(context, "EntityPrivilegeDepths", ErmSchemas.Security, "ModifiedOn");
            DropColumn(context, "EntityPrivilegeDepths", ErmSchemas.Security, "TimeStamp");
        }

        private static void OptimizeEntityAccessRights(IMigrationContext context)
        {
            DropColumn(context, "EntityAccessRights", ErmSchemas.Security, "IsDeleted");
            DropColumn(context, "EntityAccessRights", ErmSchemas.Security, "IsActive");
            DropColumn(context, "EntityAccessRights", ErmSchemas.Security, "CreatedBy");
            DropColumn(context, "EntityAccessRights", ErmSchemas.Security, "ModifiedBy");
            DropColumn(context, "EntityAccessRights", ErmSchemas.Security, "CreatedOn");
            DropColumn(context, "EntityAccessRights", ErmSchemas.Security, "ModifiedOn");
            DropColumn(context, "EntityAccessRights", ErmSchemas.Security, "TimeStamp");
        }

        private static void DropColumn(IMigrationContext context, string tableName, string tableSchema,string columnName)
        {
            var table = context.Database.Tables[tableName, tableSchema];

            var column = table.Columns[columnName];
            if (column == null)
                return;

            column.Drop();
        }
    }
}
