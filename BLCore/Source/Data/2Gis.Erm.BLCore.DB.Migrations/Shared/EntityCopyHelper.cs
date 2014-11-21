using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations.Shared
{
    public class EntityCopyHelper
    {
        public static Table RemoveIdentity(Database database, Table table)
        {
            var newIdDefinition = new InsertedColumnDefinition(0, x => new Column(x, "_Id", DataType.BigInt) { Nullable = false, Identity = false });
            var columnsToInsertList = new[] { newIdDefinition };

            var copiedtable = CopyTableSchema(table, columnsToInsertList);
            var oldIdColumn = copiedtable.Columns["Id"];
            var newIdColumn = copiedtable.Columns["_Id"];
            if (oldIdColumn == null)
            {
                throw new Exception("Old identity column does not exists");
            }

            if (newIdColumn == null)
            {
                throw new Exception("New identity column does not exists");
            }

            oldIdColumn.Drop();
            newIdColumn.Rename("Id");
            newIdColumn.Alter();

            //copiedtable.CreatePrimaryKey("Id");
            copiedtable.Alter();

            var copyScript = BuildDataCopyScript(copiedtable, table, columnsToInsertList);
            database.ExecuteNonQuery(copyScript);

            // Копируем ключи, которые ссылаются на текущую таблицу.
            var referencesForeignKeys = CopyReferenceForeignKeys(database, table);
            var checks = CreateChecks(table, copiedtable);

            // Копируем ключи, которые ссылаются с текущей таблицы.
            var foreignKeys = CreateForeignKeys(table, copiedtable);
            var indexes = CreateIndexes(table, copiedtable);

            var name = table.Name;
            table.Drop();
            copiedtable.Rename(name);

            foreach (var index in indexes)
            {
                index.Create();
            }

            foreach (var c in checks)
            {
                c.Create();
            }

            foreach (var key in foreignKeys)
            {
                key.Create();
            }

            foreach (var key in referencesForeignKeys)
            {
                key.Create();
            }

            return copiedtable;
        }

        /// <summary>
        /// Вставка колонок в середину таблицы - производится с помощью создания новой таблицы и затиранием ею старой таблицы.
        /// </summary>
        /// <param name="columnsToInsert">Примеры, проясняющие позиционирование вставленных колонок - в комментах к классу InsertedColumnDefinition.</param>
        public static Table CopyAndInsertColumns(Database database, Table sourcetable, IEnumerable<InsertedColumnDefinition> columnsToInsert)
        {
            var columnsToInsertList = (columnsToInsert ?? Enumerable.Empty<InsertedColumnDefinition>()).ToArray();

            var copiedtable = CopyTableSchema(sourcetable, columnsToInsertList);

            var copyScript = BuildDataCopyScript(copiedtable, sourcetable, columnsToInsertList);
            database.ExecuteNonQuery(copyScript);

            // Копируем ключи, которые ссылаются на текущую таблицу.
            var referencesForeignKeys = CopyReferenceForeignKeys(database, sourcetable);
            var checks = CreateChecks(sourcetable, copiedtable);

            // Копируем ключи, которые ссылаются с текущей таблицы.
            var foreignKeys = CreateForeignKeys(sourcetable, copiedtable);
            var indexes = CreateIndexes(sourcetable, copiedtable);

            var name = sourcetable.Name;
            sourcetable.Drop();
            copiedtable.Rename(name);

            foreach (var index in indexes)
            {
                index.Create();
            }

            foreach (var c in checks)
            {
                c.Create();
            }

            foreach (var key in foreignKeys)
            {
                key.Create();
            }

            foreach (var key in referencesForeignKeys)
            {
                key.Create();
            }

            return copiedtable;
        }

        public static void SetUpdateAction(Database database, SchemaQualifiedObjectName tableName, SchemaQualifiedObjectName referencedTableName, ForeignKeyAction updateAction)
        {
            var table = database.GetTable(tableName);

            var foreignKey = table.ForeignKeys.Cast<ForeignKey>().SingleOrDefault(x => x.ReferencedTable == referencedTableName.Name
                                                                                       && x.ReferencedTableSchema == referencedTableName.Schema);

            if (foreignKey == null)
            {
                return;
            }

            var newForeignKey = CopyForeignKey(foreignKey, table);

            foreignKey.Drop();

            newForeignKey.UpdateAction = updateAction;

            newForeignKey.Create();
        }

        public static Index ReplaceIndexedColumn(Index index, string columnToReplace, string newColumn)
        {
            var newIndex = new Index(index.Parent, index.Name)
            {
                IndexKeyType = index.IndexKeyType,
                IsClustered = index.IsClustered,
                IsUnique = index.IsUnique,
                CompactLargeObjects = index.CompactLargeObjects,
                IgnoreDuplicateKeys = index.IgnoreDuplicateKeys,
                IsFullTextKey = index.IsFullTextKey,
                PadIndex = index.PadIndex,
                FileGroup = index.FileGroup,
                FillFactor = index.FillFactor,
                NoAutomaticRecomputation = index.NoAutomaticRecomputation,
                SortInTempdb = index.SortInTempdb,
                FilterDefinition = index.FilterDefinition.Replace("[" + columnToReplace + "]", "[" + newColumn + "]"),
                BoundingBoxXMax = index.BoundingBoxXMax,
                BoundingBoxXMin = index.BoundingBoxXMin,
                BoundingBoxYMin = index.BoundingBoxYMin,
                BoundingBoxYMax = index.BoundingBoxYMax,
                CellsPerObject = index.CellsPerObject,
                DisallowPageLocks = index.DisallowPageLocks,
                DisallowRowLocks = index.DisallowRowLocks
            };

            foreach (IndexedColumn indexedColumn in index.IndexedColumns)
            {
                var columnName = indexedColumn.Name == columnToReplace ? newColumn : indexedColumn.Name;
                if (newIndex.IndexedColumns.Contains(columnName))
                {
                    continue;
                }

                newIndex.IndexedColumns.Add(new IndexedColumn(newIndex, columnName, indexedColumn.Descending)
                {
                    IsIncluded = indexedColumn.IsIncluded,
                    UserData = indexedColumn.UserData
                });
            }

            return newIndex;
        }

        public static Index CopyIndex(Index index)
        {
            var newIndex = new Index(index.Parent, index.Name)
            {
                IndexKeyType = index.IndexKeyType,
                IsClustered = index.IsClustered,
                IsUnique = index.IsUnique,
                CompactLargeObjects = index.CompactLargeObjects,
                IgnoreDuplicateKeys = index.IgnoreDuplicateKeys,
                IsFullTextKey = index.IsFullTextKey,
                PadIndex = index.PadIndex,
                FileGroup = index.FileGroup,
                FillFactor = index.FillFactor,
                NoAutomaticRecomputation = index.NoAutomaticRecomputation,
                SortInTempdb = index.SortInTempdb,
                FilterDefinition = index.FilterDefinition,
                BoundingBoxXMax = index.BoundingBoxXMax,
                BoundingBoxXMin = index.BoundingBoxXMin,
                BoundingBoxYMin = index.BoundingBoxYMin,
                BoundingBoxYMax = index.BoundingBoxYMax,
                CellsPerObject = index.CellsPerObject,
                DisallowPageLocks = index.DisallowPageLocks,
                DisallowRowLocks = index.DisallowRowLocks
            };

            foreach (IndexedColumn indexedColumn in index.IndexedColumns)
            {
                newIndex.IndexedColumns.Add(new IndexedColumn(newIndex, indexedColumn.Name, indexedColumn.Descending)
                {
                    IsIncluded = indexedColumn.IsIncluded,
                    UserData = indexedColumn.UserData
                });
            }

            return newIndex;
        }

        private static IEnumerable<ForeignKey> CopyReferenceForeignKeys(Database database, Table sourcetable)
        {
            var table = sourcetable.EnumForeignKeys();
            var result = new List<ForeignKey>();
            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    var schema = row[0].ToString();
                    var tableName = row[1].ToString();
                    var foreignKeyName = row[2].ToString();

                    if (tableName == sourcetable.Name && schema == sourcetable.Schema)
                    {
                        // Если ключ ссылается на саму таблицу, пропускаем - 
                        // он скопируется в методе CreateForeignKeys.
                        continue;
                    }

                    var refTable = database.Tables[tableName, schema];
                    var foreignKey = refTable.ForeignKeys[foreignKeyName];

                    var copiedForeignKey = CopyForeignKey(foreignKey, refTable);
                    result.Add(copiedForeignKey);
                    foreignKey.Drop();
                }
            }
            return result;
        }

        private static string BuildDataCopyScript(Table copiedTable, Table sourceTable, InsertedColumnDefinition[] columnsToInsert)
        {
            var capacity = sourceTable.Columns.Count + columnsToInsert.Count();
            var targetColumns = new List<string>(capacity);
            var sourceColumns = new List<string>(capacity);

            for (var i = 0; i < sourceTable.Columns.Count; i++)
            {
                if (sourceTable.Columns[i].DataType.SqlDataType != SqlDataType.Timestamp)
                {
                    sourceColumns.Add(string.Format("[{0}]", sourceTable.Columns[i].Name));
                    targetColumns.Add(string.Format("[{0}]", sourceTable.Columns[i].Name));
                }
            }

            foreach (var notNullableColumn in columnsToInsert.OfType<InsertedNotNullableColumnDefinition>())
            {
                targetColumns.Add(string.Format("[{0}]", notNullableColumn.ColumnName));
                sourceColumns.Add(string.Format("{0} as [{1}]", notNullableColumn.DefaultValue, notNullableColumn.ColumnName));
            }

            var sb = new StringBuilder();

            var copiedTableName = new SchemaQualifiedObjectName(copiedTable.Schema, copiedTable.Name);
            var sourceTableName = new SchemaQualifiedObjectName(sourceTable.Schema, sourceTable.Name);

            if (copiedTable.Columns.OfType<Column>().Any(column => column.Identity))
            {
                sb.AppendFormat("SET IDENTITY_INSERT {0} ON\nGO\n", copiedTableName);
            }

            sb.Append("INSERT INTO ");
            sb.Append(copiedTableName);
            sb.Append(" (");
            sb.Append(string.Join(", ", targetColumns));
            sb.Append(") SELECT ");
            sb.Append(string.Join(", ", sourceColumns));
            sb.Append(" FROM ");
            sb.Append(sourceTableName);
            sb.Append(" WITH (HOLDLOCK TABLOCKX);\n GO\n");

            if (copiedTable.Columns.OfType<Column>().Any(column => column.Identity))
            {
                sb.AppendFormat("SET IDENTITY_INSERT {0} OFF\nGO\n", copiedTableName);
            }

            return sb.ToString();
        }

        private static Table CopyTableSchema(Table sourcetable, IEnumerable<InsertedColumnDefinition> columnsToInsert = null)
        {
            var db = sourcetable.Parent;
            var schema = sourcetable.Schema;
            var copiedtable = new Table(db, sourcetable.Name + "_Copy", schema);

            CreateColumns(sourcetable, copiedtable, columnsToInsert);

            copiedtable.AnsiNullsStatus = sourcetable.AnsiNullsStatus;
            copiedtable.QuotedIdentifierStatus = sourcetable.QuotedIdentifierStatus;
            copiedtable.TextFileGroup = sourcetable.TextFileGroup;
            copiedtable.FileGroup = sourcetable.FileGroup;
            copiedtable.Create();

            return copiedtable;
        }

        private static void CreateColumns(Table sourcetable, Table copiedtable, IEnumerable<InsertedColumnDefinition> columnsToInsert)
        {
            var server = sourcetable.Parent.Parent;
            var originalColumnsIndex = 0;

            var columnsQueue = new Queue<InsertedColumnDefinition>();
            if (columnsToInsert != null)
            {
                foreach (var columnDefinition in columnsToInsert.OrderBy(_ => _.Index))
                {
                    columnsQueue.Enqueue(columnDefinition);
                }
            }

            foreach (Column source in sourcetable.Columns)
            {
                while (columnsQueue.Count > 0 && columnsQueue.Peek().Index == originalColumnsIndex)
                {
                    var insertedColumnTuple = columnsQueue.Dequeue();
                    copiedtable.Columns.Add(insertedColumnTuple.ColumnCreateFunc(copiedtable));
                }

                var column = new Column(copiedtable, source.Name, source.DataType)
                    {
                        Collation = source.Collation,
                        Computed = source.Computed,
                        ComputedText = source.ComputedText,
                        Default = source.Default,
                        DefaultSchema = source.DefaultSchema,
                        Identity = source.Identity,
                        IdentityIncrement = source.IdentityIncrement,
                        IdentitySeed = source.IdentitySeed,
                        IsPersisted = source.IsPersisted
                    };

                if (source.DefaultConstraint != null)
                {
                    var constrname = source.DefaultConstraint.Name;
                    column.AddDefaultConstraint(constrname);
                    column.DefaultConstraint.Text = source.DefaultConstraint.Text;
                    source.DefaultConstraint.Drop();
                }

                column.Nullable = source.Nullable;
                column.NotForReplication = source.NotForReplication;

                column.RowGuidCol = source.RowGuidCol;

                if (server.VersionMajor >= 10)
                {
                    column.IsFileStream = source.IsFileStream;
                    column.IsSparse = source.IsSparse;
                    column.IsColumnSet = source.IsColumnSet;
                }

                copiedtable.Columns.Add(column);
                originalColumnsIndex++;
            }

            while (columnsQueue.Count > 0 && columnsQueue.Peek().Index == originalColumnsIndex)
            {
                var insertedColumnTuple = columnsQueue.Dequeue();
                copiedtable.Columns.Add(insertedColumnTuple.ColumnCreateFunc(copiedtable));
            }
        }

        private static IEnumerable<Check> CreateChecks(Table sourcetable, Table copiedtable)
        {
            var checks = sourcetable.Checks.Cast<Check>().ToList();
            var result = new List<Check>();

            foreach (var chkConstr in checks)
            {
                var name = chkConstr.Name;
                var check = new Check(copiedtable, name)
                    {
                        IsChecked = chkConstr.IsChecked, 
                        IsEnabled = chkConstr.IsEnabled, 
                        Text = chkConstr.Text
                    };
                chkConstr.Drop();
                result.Add(check);
            }
            return result;
        }

        private static IEnumerable<ForeignKey> CreateForeignKeys(Table sourcetable, Table copiedtable)
        {
            var keys = sourcetable.ForeignKeys.Cast<ForeignKey>().ToList();
            var result = new List<ForeignKey>();

            foreach (var sourcefk in keys)
            {
                var newForeignKey = CopyForeignKey(sourcefk, copiedtable);
                sourcefk.Drop();
                result.Add(newForeignKey);
            }

            return result;
        }

        private static ForeignKey CopyForeignKey(ForeignKey sourcefk, Table newParent)
        {
            var name = sourcefk.Name;
            var foreignkey = new ForeignKey(newParent, name)
                {
                    DeleteAction = sourcefk.DeleteAction,
                    IsChecked = sourcefk.IsChecked,
                    IsEnabled = sourcefk.IsEnabled,
                    ReferencedTable = sourcefk.ReferencedTable,
                    ReferencedTableSchema = sourcefk.ReferencedTableSchema,
                    UpdateAction = sourcefk.UpdateAction
                };

            foreach (ForeignKeyColumn scol in sourcefk.Columns)
            {
                var refcol = scol.ReferencedColumn;
                var column = new ForeignKeyColumn(foreignkey, scol.Name, refcol);
                foreignkey.Columns.Add(column);
            }

            return foreignkey;
        }

        private static IEnumerable<Index> CreateIndexes(Table sourcetable, Table copiedtable)
        {
            var indexes = sourcetable.Indexes.Cast<Index>().ToList();
            var result = new List<Index>();

            foreach (var srcind in indexes)
            {
                if (!srcind.IsDisabled && (srcind.IsClustered ||
                    (!srcind.IsClustered && !srcind.IsXmlIndex)))
                {
                    var name = srcind.Name;
                    var index = new Index(copiedtable, name)
                        {
                            IndexKeyType = srcind.IndexKeyType,
                            IsClustered = srcind.IsClustered,
                            IsUnique = srcind.IsUnique,
                            CompactLargeObjects = srcind.CompactLargeObjects,
                            IgnoreDuplicateKeys = srcind.IgnoreDuplicateKeys,
                            IsFullTextKey = srcind.IsFullTextKey,
                            PadIndex = srcind.PadIndex,
                            FileGroup = srcind.FileGroup,
                            FillFactor = srcind.FillFactor,
                            NoAutomaticRecomputation = srcind.NoAutomaticRecomputation,
                            SortInTempdb = srcind.SortInTempdb,
                            FilterDefinition = srcind.FilterDefinition
                        };

                    foreach (IndexedColumn srccol in srcind.IndexedColumns)
                    {
                        var column = new IndexedColumn(index, srccol.Name, srccol.Descending)
                            {
                                IsIncluded = srccol.IsIncluded
                            };
                        index.IndexedColumns.Add(column);
                    }

                    srcind.Drop();
                    result.Add(index);
                }
            }
            return result;
        }
    }
}
