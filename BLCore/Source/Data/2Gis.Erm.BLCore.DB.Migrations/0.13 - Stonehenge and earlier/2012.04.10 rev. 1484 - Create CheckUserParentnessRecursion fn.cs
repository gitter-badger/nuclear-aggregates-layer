using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(1484, "Создание функции проверки циклических подчинений среди пользователей.")]
    // ReSharper disable InconsistentNaming
    public class Migration_1484 : TransactedMigration
    // ReSharper restore InconsistentNaming
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.Database.StoredProcedures.Contains(ErmStoredProcedures.CheckUserParentnessRecursion.Name, ErmStoredProcedures.CheckUserParentnessRecursion.Schema))
            {
                return;
            }

            var targetSP = ErmStoredProcedures.CheckUserParentnessRecursion;
            var fn = new StoredProcedure(context.Database, targetSP.Name, targetSP.Schema)
                         {
                             TextBody = CheckUserParentnessRecursionText,
                             TextMode = false,
                             AnsiNullsStatus = false, 
                             QuotedIdentifierStatus = false,
                         };
            var p1 = new StoredProcedureParameter(fn, "@userId", DataType.Int);
            var p2 = new StoredProcedureParameter(fn, "@proposedParentId", DataType.Int);
            
            fn.Parameters.Add(p1);
            fn.Parameters.Add(p2);
            fn.Create();
        }

        private const String CheckUserParentnessRecursionText =
        @"DECLARE @closureCount INT;
        WITH userParentCTE(userId, userName, parentId, level)
        AS
        (
        -- Anchor member definition
            SELECT U.Id, U.Account, U.ParentId, 0 AS Level
            FROM [Security].[Users] U
            WHERE U.Id=@userId
            UNION ALL
        -- Statement that executes the CTE
            SELECT U.Id, U.Account, U.ParentId, Level+1 AS Level
            FROM [Security].[Users] U
            INNER JOIN userParentCTE ON U.ParentId = userParentCTE.userId
            WHERE level<99
        )

        SELECT @closureCount = COUNT(*)
        FROM userParentCTE cte
        WHERE cte.userId = @proposedParentId

        SELECT @closureCount";
    }
}
