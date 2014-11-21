using System;
using System.Globalization;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4456, "Персчет timezones в профилях пользователя")]
    public sealed class Migration4456 : TransactedMigration
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
if (Select TOP 1 TimeZoneId FROM Billing.OrganizationUnits Where (Select TOP 1 Name from Security.Departments Where Id in (Select DepartmentId From Security.Users where id = @UserId)) like '%'+Name+'%') is null 
begin 
if (Select TOP 1 TimeZoneId FROM Billing.OrganizationUnits Where (Select TOP 1 'Новосибирск' from Security.Departments Where Id in (Select DepartmentId From Security.Users where id = @UserId) AND Name ='2ГИС') like '%'+Name+'%') is null
begin
Select TOP 1 @TimeZoneId = TimeZoneId FROM Billing.OrganizationUnits Where Name = 'Новосибирск' 
end 
Select TOP 1 @TimeZoneId = TimeZoneId FROM Billing.OrganizationUnits Where (Select TOP 1 'Новосибирск' from Security.Departments Where Id in (Select DepartmentId From Security.Users where id = @UserId) AND Name ='2ГИС') like '%'+Name+'%' 
end 
else 
Select TOP 1 @TimeZoneId = TimeZoneId FROM Billing.OrganizationUnits Where (Select TOP 1 Name from Security.Departments Where Id in (Select DepartmentId From Security.Users where id = @UserId)) like '%'+Name+'%'
UPDATE [Security].UserProfiles Set TimeZoneId = @TimeZoneId WHERE UserId = @UserId

FETCH NEXT FROM @CURSOR INTO @UserId
END
CLOSE @CURSOR";

        protected override void ApplyOverride(IMigrationContext context)
        {
            FillWithData(context);
            SetTimeZones(context);
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
