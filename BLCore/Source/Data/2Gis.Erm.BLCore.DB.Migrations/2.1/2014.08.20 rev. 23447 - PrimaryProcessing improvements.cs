using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(23447, "Перевод PrimaryProcessing на модель работы FinalProcessing", "i.maslennikov")]
    public class Migration23447 : TransactedMigration
    {
        private const string UseCaseIdColumnName = "UseCaseId";
        private const string MessageFlowIdColumnName = "MessageFlowId";
        private const string CreatedOnColumnName = "CreatedOn";
        private const string AttemptCountColumnName = "AttemptCount";
        private const string LastProcessedOnColumnName = "LastProcessedOn";
        
        private readonly SchemaQualifiedObjectName _newPrimaryProcessingTableName = 
            new SchemaQualifiedObjectName(ErmTableNames.PerformedOperationPrimaryProcessings.Schema, ErmTableNames.PerformedOperationPrimaryProcessings.Name + "_AsFinal");

        protected override void ApplyOverride(IMigrationContext context)
        {
            var existingPrimaryProcessingTable = context.Database.GetTable(ErmTableNames.PerformedOperationPrimaryProcessings);
            if (existingPrimaryProcessingTable == null)
            {
                throw new InvalidOperationException("Can't find table " + ErmTableNames.PerformedOperationPrimaryProcessings);
            } 
            
            var newPrimaryProcessingTable = context.Database.GetTable(_newPrimaryProcessingTableName);
            if (newPrimaryProcessingTable != null)
            {
                newPrimaryProcessingTable.Drop();
            }

            newPrimaryProcessingTable = PrepareNewPrimaryProcessingInfrastructure(context);
            FillNewPrimaryProcessingFromExistingData(context);
            CreateNewPrimaryProcessingPrimaryKey(newPrimaryProcessingTable);
            ReplaceExistingPrimaryProcessingByNew(existingPrimaryProcessingTable, newPrimaryProcessingTable);
        }

        private Table PrepareNewPrimaryProcessingInfrastructure(IMigrationContext context)
        {
            var table = new Table(context.Database, _newPrimaryProcessingTableName.Name, _newPrimaryProcessingTableName.Schema);

            table.CreateField(UseCaseIdColumnName, DataType.UniqueIdentifier, false);
            table.CreateField(MessageFlowIdColumnName, DataType.UniqueIdentifier, false);
            table.CreateField(CreatedOnColumnName, DataType.DateTime2(2), false);
            table.CreateField(AttemptCountColumnName, DataType.Int, false);
            table.CreateField(LastProcessedOnColumnName, DataType.DateTime2(2), true);

            table.Create();
            return table;
        }

        private void CreateNewPrimaryProcessingPrimaryKey(Table table)
        {
            string primaryKeyIndexName = string.Join("_",
                                                     "PK",
                                                     ErmTableNames.PerformedOperationPrimaryProcessings.Name,
                                                     UseCaseIdColumnName,
                                                     MessageFlowIdColumnName,
                                                     CreatedOnColumnName);

            var primaryKey = new Index(table, primaryKeyIndexName);

            var keyColumn = new IndexedColumn(primaryKey, UseCaseIdColumnName);
            primaryKey.IndexedColumns.Add(keyColumn);
            keyColumn = new IndexedColumn(primaryKey, MessageFlowIdColumnName);
            primaryKey.IndexedColumns.Add(keyColumn);
            /*keyColumn = new IndexedColumn(primaryKey, CreatedOnColumnName, false);
            primaryKey.IndexedColumns.Add(keyColumn);
            keyColumn = new IndexedColumn(primaryKey, AttemptCountColumnName) { IsIncluded = true };
            primaryKey.IndexedColumns.Add(keyColumn);
            keyColumn = new IndexedColumn(primaryKey, LastProcessedOnColumnName) { IsIncluded = true };
            primaryKey.IndexedColumns.Add(keyColumn);*/

            primaryKey.IsClustered = false;
            primaryKey.IndexKeyType = IndexKeyType.DriPrimaryKey;
            primaryKey.Create();
        }

        private void FillNewPrimaryProcessingFromExistingData(IMigrationContext context)
        {
            const string FillDataCommandBatch =
@"
DECLARE @StubUseCaseId uniqueidentifier = '00000000-0000-0000-0000-000000000000'
DECLARE @TimeSafetyOffsetInHours int = 2

DECLARE @FlowProcessingStates TABLE (
	MessageFlowId uniqueidentifier,
	LastProcessedOperationDate datetime2(2))

INSERT INTO @FlowProcessingStates
SELECT 
popp.MessageFlowId as MessageFlowId, 
MAX(popp.Date) as LastProcessedOperationDate
FROM [Shared].[PerformedOperationPrimaryProcessings] popp
GROUP BY popp.MessageFlowId

SELECT * FROM @FlowProcessingStates 

DECLARE @MessageFlowId uniqueidentifier
DECLARE	@LastProcessedOperationDate datetime2(2)
DECLARE	@OldestOperstionBoundaryDate datetime2(2)

DECLARE FlowProcessingStatesCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR 
		SELECT 
			fps.MessageFlowId,
			fps.LastProcessedOperationDate
		FROM @FlowProcessingStates fps
OPEN FlowProcessingStatesCursor;
WHILE 1 = 1
BEGIN
        FETCH NEXT FROM FlowProcessingStatesCursor INTO @MessageFlowId, @LastProcessedOperationDate;
        IF @@FETCH_STATUS <> 0 BREAK;

		SET @OldestOperstionBoundaryDate = DATEADD(HOUR, -@TimeSafetyOffsetInHours, @LastProcessedOperationDate)
		
		INSERT INTO [Shared].[PerformedOperationPrimaryProcessings_AsFinal]
		SELECT pbo.UseCaseId as UseCaseId, @MessageFlowId as MessageFlowId, pbo.Date as CreatedOn, 0 as AttemptCount, null as LastProcessedOn
		FROM [Shared].[PerformedBusinessOperations] pbo
		LEFT OUTER JOIN [Shared].[PerformedOperationPrimaryProcessings] popp
			ON pbo.Id = popp.Id AND popp.MessageFlowId = @MessageFlowId 
		WHERE popp.Id is null 
			AND pbo.Date > @OldestOperstionBoundaryDate 
			AND pbo.Parent is null
			AND pbo.UseCaseId <> @StubUseCaseId
END
CLOSE FlowProcessingStatesCursor;
DEALLOCATE FlowProcessingStatesCursor;";

            context.Connection.StatementTimeout = 1200;
            context.Database.ExecuteNonQuery(FillDataCommandBatch);
        }

        private void ReplaceExistingPrimaryProcessingByNew(Table existingPrimaryProcessingTable, Table newPrimaryProcessingTable)
        {
            existingPrimaryProcessingTable.Drop();
            newPrimaryProcessingTable.Rename(ErmTableNames.PerformedOperationPrimaryProcessings.Name);
        }
    }
}