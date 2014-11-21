using System;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations.Shared
{
    /// <summary>
    /// Index - индекс, который будет иметь вставленная колонка (если вставляется одна колонка).
    /// ColumnCreateFunc - "фабрика" для создания и настройки колонки. Аргумент делегата (SqlSmoObject) - 
    /// таблица, нужна для передачи в конструктор Column.
    /// 
    /// Чтобы вставить несколько колонок подряд, индекс у них должен быть одинаковый.
    /// Пример:
    /// <code>
    ///     columnsToInsert.Add(new InsertedColumnDefinition(31, x => new Column(x, "PayablePrice", DataType.Decimal(4, 19))));
    ///     columnsToInsert.Add(new InsertedColumnDefinition(31, x => new Column(x, "PayablePlan", DataType.Decimal(4, 19))));
    ///     columnsToInsert.Add(new InsertedColumnDefinition(31, x => new Column(x, "PayableFact", DataType.Decimal(4, 19))));
    /// </code>
    /// </summary>
    public class InsertedColumnDefinition
    {
        public Int32 Index { get; private set; }

        public Func<SqlSmoObject, Column> ColumnCreateFunc { get; private set; }

        public InsertedColumnDefinition(int index, Func<SqlSmoObject, Column> columnCreator)
        {
            Index = index;
            ColumnCreateFunc = columnCreator;
        }
    }
}
