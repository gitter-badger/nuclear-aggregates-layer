using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4598, "Удалёем колонку PrivilegeType из Security.Privileges")]
    public sealed class Migration4598 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            ProcessPrivileges(context);

            DeleteRelation(context);
            RenameColumn(context);

            DropUnneededPrivileges(context);
            CalculateNewEntityIds(context);
            DropTable(context);

            CleanUnnneededPrivileges(context);
            AddRequiredPrivileges(context);

            DeleteUnneededColumns(context);

            AlterView(context);
        }

        private static void DeleteUnneededColumns(IMigrationContext context)
        {
            var table = context.Database.Tables["Privileges", ErmSchemas.Security];

            var column = table.Columns["IsDeleted"];
            if (column != null)
                column.Drop();

            column = table.Columns["IsActive"];
            if (column != null)
                column.Drop();

            column = table.Columns["CreatedBy"];
            if (column != null)
                column.Drop();

            column = table.Columns["ModifiedBy"];
            if (column != null)
                column.Drop();

            column = table.Columns["CreatedOn"];
            if (column != null)
                column.Drop();

            column = table.Columns["ModifiedOn"];
            if (column != null)
                column.Drop();

            column = table.Columns["Timestamp"];
            if (column != null)
                column.Drop();
        }

        private static void AddRequiredPrivileges(IMigrationContext context)
        {
            // OrderFile - 202
            // ContributionType - 179
            // WithdrawalInfo - 209
            // ReleaseInfo - 203
            // LocalMessage - 189
            // UserTerritoriesOrganizationUnits - 213
            // Note - 207

            context.Database.ExecuteNonQuery(@"
            DECLARE @PrivilegeIds TABLE (Id INT NOT NULL)

            DECLARE @EntityTypes TABLE (EntityType INT NOT NULL)
            INSERT INTO @EntityTypes VALUES (202), (179), (209), (203), (189), (213), (207)

            INSERT INTO Security.Privileges (EntityAccessRightId, EntityType, CreatedBy, CreatedOn)
            OUTPUT inserted.id INTO @PrivilegeIds
            SELECT EAR.Id, EntityTypes.EntityType, -1, GETUTCDATE() FROM Security.EntityAccessRights EAR CROSS JOIN @EntityTypes EntityTypes

            INSERT INTO Security.RolePrivileges (RoleId, PrivilegeId, Priority, Mask, CreatedBy, CreatedOn)
            SELECT R.Id, PrivilegeIds.Id, 0, 16, -1, GETUTCDATE() FROM Security.Roles R CROSS JOIN @PrivilegeIds PrivilegeIds");
        }

        private static void CleanUnnneededPrivileges(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"
            DECLARE @PrivilegeIds TABLE (Id INT NOT NULL)
            INSERT INTO @PrivilegeIds VALUES (456), (457), (458), (459), (460), (461), (462), (463) -- entity type 180
            INSERT INTO @PrivilegeIds VALUES (464), (465), (466), (467), (468), (469), (470), (471) -- entity type 181
            INSERT INTO @PrivilegeIds VALUES (448), (449), (450), (451), (452), (453), (454), (455) -- entity type 216
            INSERT INTO @PrivilegeIds VALUES (472), (473), (474), (475), (476), (477), (478), (479) -- entity type 54
            INSERT INTO @PrivilegeIds VALUES (288), (289), (290), (291), (292), (293), (294), (295) -- entity type 165

            DELETE FROM Security.RolePrivileges
            WHERE PrivilegeId IN (Select Id FROM @PrivilegeIds)

            DELETE FROM Security.Privileges
            WHERE Id IN (Select Id FROM @PrivilegeIds)");
        }

        private static void DropTable(IMigrationContext context)
        {
            var table = context.Database.Tables["EntityTypes", ErmSchemas.Security];
            if (table == null)
                return;

            table.Drop();
        }

        private static void DropUnneededPrivileges(IMigrationContext context)
        {
            var table = context.Database.Tables["EntityTypes", ErmSchemas.Security];
            if (table == null)
                return;

            context.Database.ExecuteWithResults(@"
            DECLARE @PrivilegeIds TABLE (Id INT NOT NULL)

            INSERT INTO @PrivilegeIds
            SELECT p.Id FROM Security.Privileges P
            INNER JOIN Security.EntityTypes ET ON ET.Id = P.EntityType
            WHERE ET.Name IN ('AdvertisementType', 'AdvertisementTypeElement', 'FirmFile', 'EntityType')

            DELETE FROM Security.RolePrivileges
            WHERE PrivilegeId IN (Select Id FROM @PrivilegeIds)

            DELETE FROM Security.Privileges
            WHERE Id IN (Select Id FROM @PrivilegeIds)");
        }

        private static void CalculateNewEntityIds(IMigrationContext context)
        {
            var table1 = context.Database.Tables["EntityTypes", ErmSchemas.Security];
            if (table1 == null)
                return;

            var dataSet = context.Database.ExecuteWithResults(@"
            SELECT P.Id, ET.Name FROM Security.Privileges P
            INNER JOIN Security.EntityTypes ET ON ET.Id = P.EntityType
            WHERE P.EntityType IS NOT NULL");

            var table = dataSet.Tables[0];

            for (var i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];

                var id = Convert.ToInt32(row[0]);
                var name = Convert.ToString(row[1]);

                EntityName entityName;
                if (!Enum.TryParse(name, true, out entityName))
                    throw new Exception("Невозможно распознать имя сущности " + name);

                context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET EntityType=" + Convert.ToInt32(entityName) + " WHERE Id=" + id);
            }
        }

        private static void RenameColumn(IMigrationContext context)
        {
            var table = context.Database.Tables["Privileges", ErmSchemas.Security];

            var column = table.Columns["EntityTypeId"];
            if (column == null)
                return;

            column.Rename("EntityType");
        }

        private static void DeleteRelation(IMigrationContext context)
        {
            var table = context.Database.Tables["Privileges", ErmSchemas.Security];
            if (table == null)
                return;

            var foreignKey = table.ForeignKeys["FK_Privileges_EntityTypes"];
            if (foreignKey == null)
                return;

            foreignKey.Drop();
        }

        private static void AlterView(IMigrationContext context)
        {
            var view = context.Database.Views["vwUserFunctionalPrivileges", ErmSchemas.Security];
            view.TextBody = @"
            SELECT        ROW_NUMBER() OVER (ORDER BY UserId, PrivilegeId) AS Id, U.Id AS UserId, P.Id AS PrivilegeId, RP.Priority, RP.Mask
            FROM            Security.Users U INNER JOIN
                                     Security.UserRoles UR ON UR.UserId = U.Id INNER JOIN
                                     Security.RolePrivileges RP ON RP.RoleId = UR.RoleId INNER JOIN
                                     Security.Privileges P ON P.Id = RP.PrivilegeId
            WHERE        P.EntityType IS NULL";
            view.Alter();
        }

        private static void ProcessPrivileges(IMigrationContext context)
        {
            var table = context.Database.Tables["Privileges", ErmSchemas.Security];

            DropPrivilegeTypeConstraint(table);
            DropPrivilegeTypeColumn(table);
        }

        private static void DropPrivilegeTypeConstraint(Table table)
        {
            var check = table.Checks["CK_PrivilegeType"];
            if (check == null)
                return;

            check.Drop();
        }

        private static void DropPrivilegeTypeColumn(TableViewTableTypeBase table)
        {
            var column = table.Columns["PrivilegeType"];
            if (column == null)
                return;

            column.Drop();
        }

        // ReSharper disable UnusedMember.Local
        private enum EntityName
        {
            None = 0,

            // erm
            LegalPerson = 147,
            Order = 151,
            OrderPosition = 150,
            OrderFile = 202,
            Deal = 199,
            BranchOfficeOrganizationUnit = 139,
            OperationType = 149,
            AccountDetail = 141,
            Price = 155,
            Firm = 146,
            FirmAddress = 164,
            FirmContact = 165,
            BranchOffice = 156,
            OrganizationUnit = 157,
            Client = 200,
            Bargain = 198,
            BargainType = 178,
            BargainFile = 204,
            Currency = 144,
            CurrencyRate = 145,
            Platform = 182,
            PositionCategory = 181,
            AdvertisementTemplate = 184,
            PricePosition = 154,
            Account = 142,
            Limit = 192,
            Position = 153,
            PositionChildren = 183,
            AssociatedPositionsGroup = 176,
            AssociatedPosition = 177,
            DeniedPosition = 180,
            ContributionType = 179,
            Category = 160,
            CategoryOrganizationUnit = 161,
            CategoryFirmAddress = 166,
            Country = 143,
            Advertisement = 186,
            AdvertisementElement = 187,
            AdvertisementElementTemplate = 185,
            AdsTemplatesAdsElementTemplate = 201,
            Bill = 188,
            Lock = 159,
            LockDetail = 148,
            RegionalAdvertisingSharing = 210,
            Contact = 206,
            WithdrawalInfo = 209,
            ReleaseInfo = 203,
            LocalMessage = 189,
            PrintFormTemplate = 193,
            UserTerritoriesOrganizationUnits = 213,
            UsersDepartment = 152,
            File = 194,
            OrderReleaseTotal = 214,
            ReleaseWithdrawal = 215,
            OrderPositionAdvertisement = 216,

            // configuration
            Entity = 124,
            ControlType = 120,
            CardToolbar = 118,
            FieldType = 125,
            DataListField = 122,
            DataList = 121,
            DataListToolbar = 123,
            Card = 116,
            CardRelatedItem = 117,
            Module = 126,
            NavigationGroup = 113,
            NavigationArea = 112,
            DataListScript = 205,

            // security
            User = 53,
            UserRole = 56,
            UserTerritory = 172,
            UserOrganizationUnit = 174,
            Department = 51,
            Role = 54,
            RoleMapping = 173,
            Territory = 191,
            UserProfile = 211,
            EntityPrivilege = 168,
            FunctionalPrivilege = 52,
            FunctionalPrivilegeDepth = 171,
            RolePrivilege = 170,

            // shared
            TimeZone = 212,
            Note = 207,
        }
        // ReSharper restore UnusedMember.Local
    }
}
