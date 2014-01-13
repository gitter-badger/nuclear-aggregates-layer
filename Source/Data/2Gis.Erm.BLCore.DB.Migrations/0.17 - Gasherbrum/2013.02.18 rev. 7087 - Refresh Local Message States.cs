using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7087, "Перезагрузим фирмы из дгпп")]
    public sealed class Migration7087 : TransactedMigration
    {
        #region Текст запроса
        private const string Query = @"DECLARE @ReleaseTime datetime2(2)

SET @ReleaseTime = (SELECT TOP 1 [DateApplied]
  FROM [Shared].[Migrations] where Version = 5823)

  UPDATE Shared.LocalMessages SET Status = 2 WHERE MessageTypeId = 1 AND (DATEDIFF(second,@ReleaseTime,CreatedOn)>=1 OR DATEDIFF(second ,@ReleaseTime, ModifiedOn)>=1)";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
