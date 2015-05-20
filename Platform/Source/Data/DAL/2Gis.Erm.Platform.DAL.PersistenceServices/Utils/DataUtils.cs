using System;
using System.Collections.Generic;
using System.Data;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices.Utils
{
    public static class DataUtils
    {
        public static DataTable ToIdsContainer(this IEnumerable<long> pregeneratedIds)
        {
            var idsContainer = new DataTable();
            idsContainer.Columns.Add("Id", typeof(long));

            foreach (var id in pregeneratedIds)
            {
                idsContainer.Rows.Add(id);
            }

            return idsContainer;
        }

        public static EntityChangesContext ToEntityChanges(this DataTable dataTable)
        {
            var changedEntitiesReport = new EntityChangesContext();
            if (dataTable.Rows.Count == 0)
            {
                return changedEntitiesReport;
            }

            var idColumn = dataTable.ToColumnDescriptorFor("Id");
            var entityNameColumn = dataTable.ToColumnDescriptorFor("EntityName");
            var changeTypeColumn = dataTable.ToColumnDescriptorFor("ChangeType");

            foreach (DataRow rowData in dataTable.Rows)
            {
                var entityId = rowData.Column<long>(idColumn);
                var entityType = rowData.Column<IEntityType>(entityNameColumn).AsEntityType();
                var changesType = rowData.Column<ChangesType>(changeTypeColumn);

                switch (changesType)
                {
                    case ChangesType.Added:
                    {
                        changedEntitiesReport.Added(entityType, new[] { entityId });
                        break;
                    }
                    case ChangesType.Updated:
                    {
                        changedEntitiesReport.Updated(entityType, new[] { entityId });
                        break;
                    }
                    case ChangesType.Deleted:
                    {
                        changedEntitiesReport.Deleted(entityType, new[] { entityId });
                        break;
                    }
                }
            }

            return changedEntitiesReport;
        }

        private static TValue Column<TValue>(this DataRow dataRow, ColumnDescriptor columnDescriptor)
        {
            var targetType = typeof(TValue);
            string rawValue = null;

            try
            {
                rawValue = dataRow[columnDescriptor.Index].ToString();

                if (targetType == typeof(IEntityType))
                {
                    return (TValue)EntityType.Instance.Parse(int.Parse(rawValue));
                }

                if (targetType.IsEnum)
                {
                    return (TValue)Enum.Parse(targetType, rawValue);
                }

                return (TValue)Convert.ChangeType(rawValue, targetType);
            }
            catch (Exception ex)
            {
                var msg = string.Format(
                    "Can't extract value for column name {0} with index {1} from raw value {2}, column raw data type is {3}, specified result type is {4}", 
                    columnDescriptor.Name, 
                    columnDescriptor.Index,
                    rawValue ?? "is null",
                    columnDescriptor.RawDataType,
                    targetType);
                throw new InvalidOperationException(msg, ex);
            }
        }

        private static ColumnDescriptor ToColumnDescriptorFor(this DataTable table, string columnName)
        {
            if (!table.Columns.Contains(columnName))
            {
                throw new InvalidOperationException("Specified data table has invalid scheme and can't be converted to changes container. Column not found: " + columnName);
            }

            var column = table.Columns[columnName];
            var columnDescriptor = new ColumnDescriptor
                {
                    Name = columnName, 
                    Index = column.Ordinal,
                    RawDataType = column.DataType
                };

            return columnDescriptor;
        }

        private class ColumnDescriptor
        {
            public string Name { get; set; }
            public int Index { get; set; }
            public Type RawDataType { get; set; }
        }
    }
}
