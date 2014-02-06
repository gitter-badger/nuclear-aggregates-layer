using System;
using System.Text;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations.Shared
{
    public static class ErmTableUtils
    {
        public const string CommonIdColumnName = "Id";

        internal const string CommonCreatedByColumnName = "CreatedBy";
        internal const string CommonCreatedOnColumnName = "CreatedOn";
        internal const string CommonModifiedByColumnName = "ModifiedBy";
        internal const string CommonModifiedOnColumnName = "ModifiedOn";

        internal const string CommonOwnerCodeColumnName = "OwnerCode";

        private const string CommonIsActiveColumnName = "IsActive";
        private const string CommonTimestampColumnName = "Timestamp";
        private const string CommonIsDeletedColumnName = "IsDeleted";

        public static void CreateAuditableEntityColumns(this Table table)
        {
            table.Columns.Add(new Column(table, CommonCreatedByColumnName, DataType.BigInt) { Nullable = false });
            table.Columns.Add(new Column(table, CommonCreatedOnColumnName, DataType.DateTime2(2)) { Nullable = false });
            table.Columns.Add(new Column(table, CommonModifiedByColumnName, DataType.BigInt) { Nullable = true });
            table.Columns.Add(new Column(table, CommonModifiedOnColumnName, DataType.DateTime2(2)) { Nullable = true });
        }

        public static void CreateStandartColumns(this Table table)
        {
            CreateActiveableEntityColumn(table);
            CreateDeleteableEntityColumn(table);
            CreateAuditableEntityColumns(table);
            CreateTimestampColumn(table);
        }

        public static void CreateSecureEntityStandartColumns(this Table table)
        {
            CreateActiveableEntityColumn(table);
            CreateDeleteableEntityColumn(table);
            CreateOwnerCodeColumn(table);
            CreateAuditableEntityColumns(table);
            CreateTimestampColumn(table);
        }

        public static void CreateDeleteableEntityColumn(this Table table)
        {
            var isDeletedColumn = new Column(table, CommonIsDeletedColumnName, DataType.Bit) { Nullable = false };
            isDeletedColumn.AddDefaultConstraint("DF_" + table.Name + CommonIsDeletedColumnName);
            isDeletedColumn.DefaultConstraint.Text = "0";
            table.Columns.Add(isDeletedColumn);
        }

        public static void CreateActiveableEntityColumn(this Table table)
        {
            var isActiveColumn = new Column(table, CommonIsActiveColumnName, DataType.Bit) { Nullable = false };
            isActiveColumn.AddDefaultConstraint("DF_" + table.Name + CommonIsActiveColumnName);
            isActiveColumn.DefaultConstraint.Text = "1";
            table.Columns.Add(isActiveColumn);
        }

        public static void CreateTimestampColumn(this Table table)
        {
            table.Columns.Add(new Column(table, CommonTimestampColumnName, DataType.Timestamp) { Nullable = false });
        }

        public static void CreatePrimaryKey(this Table table)
        {
            CreatePrimaryKey(table, CommonIdColumnName);
        }

        public static void CreatePrimaryKey(this Table table, string primaryKeyColumnName)
        {
            var primaryKey = new Index(table, "PK_" + table.Name + "_" + primaryKeyColumnName);
            var primaryKeyIndexColumn = new IndexedColumn(primaryKey, primaryKeyColumnName);
            primaryKey.IndexedColumns.Add(primaryKeyIndexColumn);
            primaryKey.IndexKeyType = IndexKeyType.DriPrimaryKey;
            primaryKey.Create();
        }

        public static void CreatePrimaryKey(this Table table, string[] primaryKeyColumnNames)
        {
            var primaryKeyIndexName = new StringBuilder("PK_" + table.Name);
            foreach (var primaryKeyColumnName in primaryKeyColumnNames)
            {
                primaryKeyIndexName.AppendFormat("_{0}", primaryKeyColumnName);
            }

            var primaryKey = new Index(table, primaryKeyIndexName.ToString());
            foreach (var primaryKeyColumnName in primaryKeyColumnNames)
            {
                var primaryKeyIndexColumn = new IndexedColumn(primaryKey, primaryKeyColumnName);
                primaryKey.IndexedColumns.Add(primaryKeyIndexColumn);    
            }
            
            primaryKey.IndexKeyType = IndexKeyType.DriPrimaryKey;
            primaryKey.Create();
        }

        public static void CreateField(this Table table, string fieldName, DataType dataType, bool nullable)
        {
            table.Columns.Add(new Column(table, fieldName, dataType) { Nullable = nullable });
        }

        public static void RemoveField(this Table table, string fieldName)
        {
            if (!table.Columns.Contains(fieldName))
            {
                return;
            }

            var column = table.Columns[fieldName];
            column.Drop();
        }

        public static void CreateForeignKey(this Table table, 
                                            string keyColumnName,
                                            SchemaQualifiedObjectName referencedTable,
                                            string referencedTableColumnName)
        {
            var foreignKey = new ForeignKey(table, "FK_" + table.Name + "_" + referencedTable.Name);
            var foreignKeyColumn = new ForeignKeyColumn(foreignKey, keyColumnName, referencedTableColumnName);
            foreignKey.Columns.Add(foreignKeyColumn);
            foreignKey.ReferencedTable = referencedTable.Name;
            foreignKey.ReferencedTableSchema = referencedTable.Schema;
            foreignKey.Create();
        }

        public static void RemoveForeignKey(this Table table,
                                            string keyColumnName,
                                            SchemaQualifiedObjectName referencedTable,
                                            string referencedTableColumnName)
        {
            ForeignKey foreignKey = null;
            var foreignKeyName = string.Format("FK_{0}_{1}", table.Name, referencedTable.Name);
            if (table.ForeignKeys.Contains(foreignKeyName))
            {
                foreignKey = table.ForeignKeys[foreignKeyName];
            }
            else
            {
                foreignKeyName = string.Format("FK_{0}_{1}_{2}_{3}", table.Name, referencedTable.Name, keyColumnName, referencedTableColumnName);
                if (table.ForeignKeys.Contains(foreignKeyName))
                {
                    foreignKey = table.ForeignKeys[foreignKeyName];
                }
            }

            if (foreignKey != null)
            {
                foreignKey.Drop();
            }
        }

        public static void CreateIndex(this Table table, string indexColumnName)
        {
            var indexTitle = string.Format("IX_{0}_{1}", table.Name, indexColumnName);
            var index = new Index(table, indexTitle) { IndexKeyType = IndexKeyType.None };
            index.IndexedColumns.Add(new IndexedColumn(index, indexColumnName));
            index.Create();
        }

        public static void InsertAndSetNonNullableColumn(this Table table, IMigrationContext context, InsertedColumnDefinition columnDefinition, string columnName, string columnValue)
        {
            var columnsToInsert = new[] { columnDefinition };
            table = EntityCopyHelper.CopyAndInsertColumns(context.Database, table, columnsToInsert);
            context.Connection.ExecuteNonQuery(string.Format("UPDATE {0} SET {1} = {2}", new SchemaQualifiedObjectName(table.Schema, table.Name), columnName, columnValue));
            table.SetNonNullableColumns(columnName);
            table.Alter();
        }

        public static IMigrationContext ChangeColumnDataType(this IMigrationContext migrationContext,
                                                             SchemaQualifiedObjectName targetTable,
                                                             string targetColumnName,
                                                             DataType targetDatatype)
        {
            var table = migrationContext.Database.GetTable(targetTable);
            if (table == null)
            {
                throw new InvalidOperationException("Can't find target table " + targetTable);
            }

            var column = table.Columns[targetColumnName];
            if (column == null)
            {
                throw new InvalidOperationException("Can't find target column " + targetColumnName);
            }

            if (column.DataType.SqlDataType == targetDatatype.SqlDataType)
            {
                // do nothing
                return migrationContext;
            }

            column.DataType = targetDatatype;
            column.Alter();

            return migrationContext;
        }

        private static void CreateOwnerCodeColumn(this Table table)
        {
            table.Columns.Add(new Column(table, CommonOwnerCodeColumnName, DataType.BigInt) { Nullable = false });
        }
    }
}
