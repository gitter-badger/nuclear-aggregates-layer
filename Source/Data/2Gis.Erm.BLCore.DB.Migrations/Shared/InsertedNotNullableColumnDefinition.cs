using System;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations.Shared
{
    /// <summary>
    /// InsertedColumnDefinition для оптимальной вставки NotNullable колонок
    /// </summary>
    public class InsertedNotNullableColumnDefinition : InsertedColumnDefinition
    {
        public InsertedNotNullableColumnDefinition(int index, Func<SqlSmoObject, Column> columnCreator, string columnName, string defaultValue) 
            : base(index, columnCreator)
        {
            this.ColumnName = columnName;
            this.DefaultValue = defaultValue;
        }

        public string ColumnName { get; private set; }

        public string DefaultValue { get; private set; }
    }
}