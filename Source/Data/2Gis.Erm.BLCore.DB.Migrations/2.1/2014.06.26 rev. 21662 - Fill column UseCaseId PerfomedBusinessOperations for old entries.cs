using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    // FIXME {all, 26.06.2014}: internal миграция, чтобы исключить её запуск мигратором, нужна пока как хранилище скрипта, позднее будет переведена в public или удалена с переносом всей логики во внешний SQL скрипт
    [Migration(21662, "Заполнение колонки UseCaseId в таблице [Shared].[PerformedBusinessOperations], для ранее имевшихся записей", "i.maslennikov")]
    internal sealed class Migration21662 : TransactedMigration
    {
        private const string TargetColumnName = "UseCaseId";
        private readonly SchemaQualifiedObjectName _targetTableName = ErmTableNames.PerformedBusinessOperations;

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.StatementTimeout = 1200;

            UpdateData(context.Database);
            
            context.Database
                   .GetTable(_targetTableName)
                   .CreateIndex(false, TargetColumnName);

            Complete(context);
        }

        private void UpdateData(Database database)
        {
            const string UpdateDataCommandText =

@"
DECLARE @DefaultUseCaseId UNIQUEIDENTIFIER = CAST('00000000-0000-0000-0000-000000000000' as UNIQUEIDENTIFIER)
DECLARE @IntervalStart datetime = DATEADD(DAY, -3, GETUTCDATE())
;WITH TrackedUseCases AS
(
SELECT root.Id, root.Parent, root.Operation, root.Descriptor, root.Context, root.Date, NEWID() as UseCaseId
FROM [Shared].[PerformedBusinessOperations] root
WHERE root.Parent is null AND root.Date > @IntervalStart AND root.UseCaseId = @DefaultUseCaseId
UNION ALL
SELECT child.Id, child.Parent, child.Operation, child.Descriptor, child.Context, child.Date, TrackedUseCases.UseCaseId
FROM [Shared].[PerformedBusinessOperations] child
INNER JOIN TrackedUseCases ON child.Parent = TrackedUseCases.Id AND child.UseCaseId = @DefaultUseCaseId
WHERE child.Parent is not null AND child.Date > @IntervalStart
)
UPDATE pbo    
  SET pbo.UseCaseId = tuc.UseCaseId   
from [Shared].[PerformedBusinessOperations] pbo
INNER JOIN TrackedUseCases tuc
    ON pbo.Id = tuc.Id";

            database.ExecuteNonQuery(UpdateDataCommandText);
        }

        private void Complete(IMigrationContext context)
        {
            var table = context.Database.GetTable(_targetTableName);
            var column = table.Columns[TargetColumnName];
            if (column.DefaultConstraint != null)
            {
                column.DefaultConstraint.Drop();
            }
        }
    }
}
