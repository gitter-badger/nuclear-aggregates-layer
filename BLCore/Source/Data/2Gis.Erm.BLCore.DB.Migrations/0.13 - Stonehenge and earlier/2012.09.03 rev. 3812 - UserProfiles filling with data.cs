using System;
using System.Globalization;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3812, "Расширяем и заполняем профиль пользователя данными")]
    public sealed class Migration3812 : TransactedMigration
    {
        private const Int32 DefaultTimeZoneId = 26;

        private const String InserStatementTemplate =
            "INSERT Into Security.UserProfiles (UserId, TimeZoneId, CultureInfoLCID, CreatedBy, CreatedOn, Gender) Select Id, {0}, {1}, 1, getDate(), 0 From Security.Users WHERE Id NOT IN (SELECT UserId FROM Security.UserProfiles);";

        private const String SetTimeZonesStatement = @"
DECLARE @TimeZoneId int
DECLARE @UserId int
DECLARE @CURSOR CURSOR

SET @CURSOR  = CURSOR SCROLL
FOR
SELECT  UserId
  FROM  [Security].UserProfiles

OPEN @CURSOR

FETCH NEXT FROM @CURSOR INTO @UserId

WHILE @@FETCH_STATUS = 0
BEGIN
Set @TimeZoneId = (Select TOP 1 * FROM
(
Select TimeZoneId FROM Billing.OrganizationUnits Where (Select TOP 1 Name from Security.Departments Where Id in (Select DepartmentId From Security.Users where id = @UserId)) like '%'+Name+'%' 
UNION
Select TimeZoneId FROM Billing.OrganizationUnits Where (Select TOP 1 'Новосибирск' from Security.Departments Where Id in (Select DepartmentId From Security.Users where id = @UserId) AND Name ='2ГИС') like '%'+Name+'%' 
UNION
Select TimeZoneId FROM Billing.OrganizationUnits Where Name = 'Новосибирск'
) t)
UPDATE [Security].UserProfiles Set TimeZoneId = @TimeZoneId WHERE UserId = @UserId

FETCH NEXT FROM @CURSOR INTO @UserId
END
CLOSE @CURSOR";

        protected override void ApplyOverride(IMigrationContext context)
        {
            AddColumns(context);
            FillWithData(context);
            SetTimeZones(context);
        }

        private static void AddColumns(IMigrationContext context)
        {
            var table = context.Database.Tables["UserProfiles", ErmSchemas.Security];
            Column emailColumn = new Column(table, "Email", DataType.NVarChar(50)) {Nullable = true};
            table.Columns.Add(emailColumn);
            Column phoneColumn = new Column(table, "Phone", DataType.NVarChar(50)) {Nullable = true};
            table.Columns.Add(phoneColumn);
            Column mobileColumn = new Column(table, "Mobile", DataType.NVarChar(50)) {Nullable = true};
            table.Columns.Add(mobileColumn);
            Column addressColumn = new Column(table, "Address", DataType.NVarChar(100)) {Nullable = true};
            table.Columns.Add(addressColumn);
            Column companyColumn = new Column(table, "Company", DataType.NVarChar(100)) {Nullable = true};
            table.Columns.Add(companyColumn);
            Column positionColumn = new Column(table, "Position", DataType.NVarChar(100)) {Nullable = true};
            table.Columns.Add(positionColumn);
            Column birthdayColumn = new Column(table, "Birthday", DataType.DateTime2(2)) {Nullable = true};
            table.Columns.Add(birthdayColumn);
            Column genderColumn = new Column(table, "Gender", DataType.Int) {Nullable = false};
            table.Columns.Add(genderColumn);
            Column planetURLColumn = new Column(table, "PlanetURL", DataType.NVarChar(100)) {Nullable = true};
            table.Columns.Add(planetURLColumn);
            table.Alter();
        }

        private static void FillWithData(IMigrationContext context)
        {
            CultureInfo culture = new CultureInfo("ru-RU");
            String insertStatement = String.Format(InserStatementTemplate, DefaultTimeZoneId, culture.LCID);
            context.Connection.ExecuteNonQuery(insertStatement);
        }

        private static void SetTimeZones(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(SetTimeZonesStatement);
        }
    }
}
