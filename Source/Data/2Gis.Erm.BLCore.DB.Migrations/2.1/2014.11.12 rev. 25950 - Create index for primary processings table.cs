using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(25950, "Создаем индекс для таблицы primary processings", "i.maslennikov")]
    public sealed class Migration25950 : TransactedMigration
    {
        private const string UseCaseIdColumnName = "UseCaseId";
        private const string MessageFlowIdColumnName = "MessageFlowId";
        private const string CreatedOnColumnName = "CreatedOn";
        private const string AttemptCountColumnName = "AttemptCount";
        private const string LastProcessedOnColumnName = "LastProcessedOn";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var targetIndexName = string.Join("_",
                                              "NCI",
                                              ErmTableNames.PerformedOperationPrimaryProcessings.Name,
                                              MessageFlowIdColumnName,
                                              CreatedOnColumnName,
                                              UseCaseIdColumnName);

            var primaryProcessingTable = context.Database.GetTable(ErmTableNames.PerformedOperationPrimaryProcessings);
            if (primaryProcessingTable.Indexes.Contains(targetIndexName))
            {
                return;
            }

            var index = new Index(primaryProcessingTable, targetIndexName);
            var indexColumn = new IndexedColumn(index, MessageFlowIdColumnName);
            index.IndexedColumns.Add(indexColumn);
            indexColumn = new IndexedColumn(index, CreatedOnColumnName);
            index.IndexedColumns.Add(indexColumn);
            indexColumn = new IndexedColumn(index, UseCaseIdColumnName);
            index.IndexedColumns.Add(indexColumn);
            indexColumn = new IndexedColumn(index, AttemptCountColumnName) { IsIncluded = true };
            index.IndexedColumns.Add(indexColumn);
            indexColumn = new IndexedColumn(index, LastProcessedOnColumnName) { IsIncluded = true };
            index.IndexedColumns.Add(indexColumn);

            index.IsClustered = false;
            index.SortInTempdb = false;
            index.OnlineIndexOperation = true;
            index.IndexKeyType = IndexKeyType.None;
            index.Create();
        }
    }
}