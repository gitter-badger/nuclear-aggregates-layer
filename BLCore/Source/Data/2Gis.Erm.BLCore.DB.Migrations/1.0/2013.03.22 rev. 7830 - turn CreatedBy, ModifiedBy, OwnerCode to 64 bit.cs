using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7830, "Преобразование полей хранящих идентификаторы пользователей а 64 разрядные")]
    public sealed class Migration7830 : IContextedMigration<IMigrationContext>
    {
        public void Apply(IMigrationContext context)
        {
            var clusters = Initialize(context).ToArray();

            var progress = new Progress(clusters.Length);
            foreach (var cluster in clusters)
            {
                context.Connection.BeginTransaction();
                try
                {
                    cluster.Upgrade();
                    context.Connection.CommitTransaction();
                }
                catch (Exception e)
                {
                    context.Connection.RollBackTransaction();
                    throw new Exception(string.Format("While processing {0}", cluster), e);
                }
                finally
                {
                    progress.Increment();
                }
            }
        }

        public void Revert(IMigrationContext context)
        {
            throw new NotImplementedException();
        }

        private static Cluster[] Initialize(IMigrationContext context)
        {
            var explorer = new DatabaseExplorer(context.Database);

            var idsToProcess = context.Database.Tables.OfType<Table>()
                                      .SelectMany(table => new[] { table.Columns["CreatedBy"], table.Columns["ModifiedBy"], table.Columns["OwnerCode"] })
                                      .Where(id => id != null && id.DataType.SqlDataType == DataType.Int.SqlDataType)
                                      .ToList();

            var clusters = new List<Cluster>();
            while (idsToProcess.Count > 0)
            {
                var id = idsToProcess.First();
                var cluster = explorer.InitializeColumns(id);
                clusters.Add(cluster);

                foreach (var column in cluster.ColumnNames)
                {
                    idsToProcess.RemoveAll(c => ((Table)c.Parent).Name == column.TableName
                                                && ((Table)c.Parent).Schema == column.TableSchema
                                                && c.Name == column.Name);
                }
            }

            return clusters.ToArray();
        }

        private class Progress
        {
            private const int Step = 10;
            private readonly int _count;
            private int _position;
            private int _displayedPosition;

            public Progress(int count)
            {
                _count = count;
                _displayedPosition = 0;
            }

            public void Increment()
            {
                _position++;
                var percentComplete = 100 * _position / _count;
                if (_displayedPosition + Step < percentComplete)
                {
                    _displayedPosition += Step;
                    Debug.WriteLine(percentComplete + "%");
                }
            }
        }

        private class Cluster
        {
            private readonly List<StringCollection> _restoreScript;
            private readonly Database _database;

            public Cluster(Database database)
            {
                _database = database;
                _restoreScript = new List<StringCollection>();
            }

            public ObjectName[] IndexNames { get; set; }
            public ObjectName[] ForeignKeyNames { get; set; }
            public ObjectName[] ColumnNames { get; set; }

            private Index[] Indexes { get; set; }
            private ForeignKey[] ForeignKeys { get; set; }
            private Column[] Columns { get; set; }

            public void Upgrade()
            {
                InitObjects();
                if (IsUpgraded())
                {
                    return;
                }

                InitRestoreScript();
                DropStuff();
                ChangeType();
                RestoreStuff();
            }

            public override string ToString()
            {
                var indexes = string.Format("indexes: {0}", string.Join(", ", Indexes.Select(index => index != null ? index.Name : "null")));
                var keys = string.Format("foreign keys: {0}", string.Join(", ", ForeignKeys.Select(key => key != null ? key.Name : "null")));
                var cloumns = string.Format("cloumns: {0}", string.Join(", ", Columns.Select(cloumn => cloumn != null ? cloumn.Name : "null")));
                return string.Format("({0}), ({1}), ({2})", indexes, keys, cloumns);
            }

            private bool IsUpgraded()
            {
                return Columns.All(column => column.DataType.SqlDataType == SqlDataType.BigInt);
            }

            private void InitObjects()
            {
                // Обновляю сведения о состоянии всех связанных с кластером таблиц, поскольку если объект был удален через вызов метода Drop(), 
                // а восстановлен скриптом, то ссылки на него в соответсвующей коллекции не будет.
                var tables = IndexNames.Concat(ForeignKeyNames).Concat(ColumnNames)
                                       .Select(name => _database.Tables[name.TableName, name.TableSchema])
                                       .Distinct();

                foreach (var table in tables)
                {
                    table.Indexes.Refresh();
                    table.ForeignKeys.Refresh();
                    table.Columns.Refresh();
                }

                Indexes = IndexNames.Select(name => _database.Tables[name.TableName, name.TableSchema].Indexes[name.Name]).ToArray();
                ForeignKeys = ForeignKeyNames.Select(name => _database.Tables[name.TableName, name.TableSchema].ForeignKeys[name.Name]).ToArray();
                Columns = ColumnNames.Select(name => _database.Tables[name.TableName, name.TableSchema].Columns[name.Name]).ToArray();
            }

            private void InitRestoreScript()
            {
                InitRestoreScript(Indexes);
                InitRestoreScript(ForeignKeys);
            }

            private void DropStuff()
            {
                Drop(ForeignKeys);
                Drop(Indexes);
                Drop(Columns.Select(column => column.DefaultConstraint).Where(constraint => constraint != null));
            }

            private void ChangeType()
            {
                foreach (var column in Columns)
                {
                    column.DataType = DataType.BigInt;
                    column.Alter();
                }
            }

            private void RestoreStuff()
            {
                foreach (var script in _restoreScript)
                {
                    _database.ExecuteNonQuery(script);
                }
            }

            private void Drop<T>(IEnumerable<T> collection)
            {
                foreach (var obj in collection)
                {
                    var constraint = obj as DefaultConstraint;
                    if (constraint != null)
                    {
                        Debug.WriteLine("Dropping default constraint {0} from table {1}", constraint.Name, ((Table)constraint.Parent.Parent).Name);
                    }

                    ((IDroppable)obj).Drop();
                }
            }

            private void InitRestoreScript<T>(IEnumerable<T> collection)
            {
                foreach (var obj in collection)
                {
                    StringCollection script;

                    var scriptNameObjectBase = obj as ScriptNameObjectBase;
                    if (scriptNameObjectBase != null)
                    {
                        scriptNameObjectBase.Refresh();
                    }

                    if (obj is ForeignKey)
                    {
                        // Решаем проблему отсутствия имени схемы в сгенерённом скрипте
                        var so = new ScriptingOptions { DriForeignKeys = true, SchemaQualifyForeignKeysReferences = true };
                        script = ((IScriptable)obj).Script(so);
                    }
                    else
                    {
                        script = ((IScriptable)obj).Script();
                    }

                    _restoreScript.Add(script);
                }
            }
        }

        private class DatabaseExplorer
        {
            private readonly Database _database;
            private readonly Table[] _tables;
            private readonly Dictionary<string, ForeignKey> _foreignKeys;
            private readonly Dictionary<string, Index> _indexes;

            public DatabaseExplorer(Database database)
            {
                _database = database;
                _tables = _database.Tables.OfType<Table>().ToArray();
                _foreignKeys = _tables.SelectMany(table => table.ForeignKeys.OfType<ForeignKey>()).ToDictionary(key => key.Name, key => key);
                _indexes = _tables.SelectMany(table => table.Indexes.OfType<Index>()).ToDictionary(index => index.Name, index => index);
            }

            public Cluster InitializeColumns(Column column)
            {
                var foreignKeys = column.EnumForeignKeys().Rows.OfType<DataRow>().Select(row => _foreignKeys[row[2].ToString()]).ToArray();
                var columns = new[] { column }.Concat(GetRelatedColumns(foreignKeys)).Distinct().ToArray();
                var indexes = GetRelatedIndexes(columns)
                    .Distinct();

                return new Cluster(_database)
                {
                    ColumnNames = columns.Select(ObjectName.FromColumn).ToArray(),
                    ForeignKeyNames = foreignKeys.Select(ObjectName.FromForeignKey).ToArray(),
                    IndexNames = indexes.Select(ObjectName.FromIndex).ToArray(),
                };
            }

            private IEnumerable<Index> GetRelatedIndexes(Column[] columns)
            {
                var result = new List<Index>();
                foreach (var column in columns)
                {
                    var columnRelatedIndexes =
                        column.EnumIndexes().Rows.OfType<DataRow>().Select(row => _indexes[row[0].ToString()]);
                    result.AddRange(columnRelatedIndexes);
                }

                return result.Distinct().ToArray();
            }

            private IEnumerable<Column> GetRelatedColumns(IEnumerable<ForeignKey> keys)
            {
                var result = new List<Column>();
                foreach (var key in keys)
                {
                    var keyRelatedColumns = key.Columns.OfType<ForeignKeyColumn>()
                       .Select(keyColumn => keyColumn.Parent.Parent.Columns[keyColumn.Name])
                       .ToArray();
                    result.AddRange(keyRelatedColumns);
                }

                return result.Distinct().ToArray();
            }
        }

        // имена вместо самих объектов использую, чтобы находить объекты в тот момент, когда они непосредственно требуются,
        // чтобы их состояние было актуальным.
        private class ObjectName
        {
            public string TableSchema { get; set; }
            public string TableName { get; set; }
            public string Name { get; set; }

            public static ObjectName FromColumn(Column arg)
            {
                return new ObjectName { Name = arg.Name, TableName = ((Table)arg.Parent).Name, TableSchema = ((Table)arg.Parent).Schema };
            }

            public static ObjectName FromForeignKey(ForeignKey arg)
            {
                return new ObjectName { Name = arg.Name, TableName = arg.Parent.Name, TableSchema = arg.Parent.Schema };
            }

            public static ObjectName FromIndex(Index arg)
            {
                return new ObjectName { Name = arg.Name, TableName = ((Table)arg.Parent).Name, TableSchema = ((Table)arg.Parent).Schema };
            }
        }
    }
}
