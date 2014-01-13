using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4884, "Удаляем колонку FunctionalLocalResourceName из таблицы Security.Privileges")]
    public sealed class Migration4884 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropColumn1(context);
            SetNotNullableClumn(context);
        }

        private static void SetNotNullableClumn(IMigrationContext context)
        {
            var table = context.Database.Tables["Privileges", ErmSchemas.Security];

            var column = table.Columns["Operation"];
            if (!column.Nullable)
                return;

            context.Database.ExecuteNonQuery(@"DELETE RP FROM Security.RolePrivileges RP INNER JOIN Security.Privileges P ON P.Id = RP.PrivilegeId WHERE P.Operation IS NULL");
            context.Database.ExecuteNonQuery(@"DELETE FPD FROM Security.FunctionalPrivilegeDepths FPD INNER JOIN Security.Privileges P ON P.Id = FPD.PrivilegeId WHERE P.Operation IS NULL");
            context.Database.ExecuteNonQuery(@"DELETE Security.Privileges WHERE Operation IS NULL");
            column.Nullable = false;
            column.Alter();
        }

        private static void DropColumn1(IMigrationContext context)
        {
            var table = context.Database.Tables["Privileges", ErmSchemas.Security];

            var column = table.Columns["FunctionalLocalResourceName"];
            if (column == null)
                return;

            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 5 WHERE FunctionalLocalResourceName = 'PrvReserveAccess'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 3 WHERE FunctionalLocalResourceName = 'PrvOrderStatesAccess'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 189 WHERE FunctionalLocalResourceName = 'PrvLimitManagement'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 190 WHERE FunctionalLocalResourceName = 'PrvOrderCreationExtended'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 191 WHERE FunctionalLocalResourceName = 'PrvOrderBranchOfficeOrganizationUnitSelection'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 192 WHERE FunctionalLocalResourceName = 'PrvLegalPersonChangeClient'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 193 WHERE FunctionalLocalResourceName = 'PrvLegalPersonChangeRequisites'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 496 WHERE FunctionalLocalResourceName = 'PrvOrderChangeDealExtended'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 497 WHERE FunctionalLocalResourceName = 'PrvOrderChangeDocumentsDebt'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 498 WHERE FunctionalLocalResourceName = 'PrvOrderCreationForFuture'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 531 WHERE FunctionalLocalResourceName = 'PrvMergeClients'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 532 WHERE FunctionalLocalResourceName = 'PrvCreateAccountDetail'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 533 WHERE FunctionalLocalResourceName = 'PrvWithdrawal'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 534 WHERE FunctionalLocalResourceName = 'PrvRelease'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 535 WHERE FunctionalLocalResourceName = 'PrvCorporateQueue'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 536 WHERE FunctionalLocalResourceName = 'PrvMakeRegionalAdsDocs'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 537 WHERE FunctionalLocalResourceName = 'PrvProcessAccountsWithDebts'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 538 WHERE FunctionalLocalResourceName = 'PrvCloseBargainOperationalPeriod'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 539 WHERE FunctionalLocalResourceName = 'PrvServiceUserAssignAccess'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 540 WHERE FunctionalLocalResourceName = 'PrvSendMassEmail'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 541 WHERE FunctionalLocalResourceName = 'PrvChangeFirmClient'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 542 WHERE FunctionalLocalResourceName = 'PrvLeaveClientWithNoFirms'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 543 WHERE FunctionalLocalResourceName = 'PrvPrereleaseOrderValidationExecution'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 544 WHERE FunctionalLocalResourceName = 'PrvEditOrderType'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 545 WHERE FunctionalLocalResourceName = 'PrvChangeFirmTerritory'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 546 WHERE FunctionalLocalResourceName = 'PrvMergeLegalPersons'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 546 WHERE FunctionalLocalResourceName = 'PrvMergeLegalPersons'");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 604 WHERE FunctionalLocalResourceName = 'PrvFranchiseesWithdrawalExport'");

            column.Drop();
        }
    }
}