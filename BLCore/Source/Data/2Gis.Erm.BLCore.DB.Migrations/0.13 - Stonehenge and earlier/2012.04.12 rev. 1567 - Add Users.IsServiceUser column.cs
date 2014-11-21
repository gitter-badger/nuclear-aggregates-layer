using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(1567, "Добавление в таблицу пользователей признака \"Служебный\"")]
    public class Migration_1567 : TransactedMigration
	{
        private readonly SchemaQualifiedObjectName _tableName = new SchemaQualifiedObjectName("Security", "Users");
        private const String ColumnName = "IsServiceUser";

        private const string CommandText = @"
                                            BEGIN TRAN userPriv

                                            BEGIN TRY 

			                                            DECLARE @serviceUserAssignAccess INT;
			                                            DECLARE @serviceUserAssignAccessPrvDepthMaskIdStart INT;
			                                            DECLARE @PrivilegeName VARCHAR(200);

			                                            SET @serviceUserAssignAccess = 539
			                                            SET @serviceUserAssignAccessPrvDepthMaskIdStart = 130
			                                            SET @PrivilegeName = 'PrvServiceUserAssignAccess'

			                                            IF NOT EXISTS (SELECT 1 FROM [Security].[Privileges] WHERE Id = @serviceUserAssignAccess)
			                                            BEGIN

			                                            SET IDENTITY_INSERT [Security].[Privileges] ON
			                                            INSERT INTO [Security].[Privileges](
				                                            Id
				                                            ,PrivilegeType
				                                            ,FunctionalLocalResourceName
				                                            ,IsDeleted
				                                            ,IsActive
				                                            ,CreatedBy
				                                            ,CreatedOn)
			                                            VALUES(
				                                            @serviceUserAssignAccess
				                                            ,'F'
				                                            ,@PrivilegeName
				                                            ,0
				                                            ,1
				                                            ,-1
				                                            ,GETDATE())
			                                            SET IDENTITY_INSERT [Security].[Privileges] OFF

			                                            INSERT INTO [Security].[FunctionalPrivilegeDepths] (
				                                            PrivilegeId
				                                            ,LocalResourceName
				                                            ,[Priority]
				                                            ,Mask
				                                            ,IsDeleted
				                                            ,IsActive
				                                            ,CreatedBy
				                                            ,CreatedOn)
			                                            VALUES(
				                                            @serviceUserAssignAccess
				                                            ,'FPrvDpthGranted'
				                                            ,1
				                                            ,@serviceUserAssignAccessPrvDepthMaskIdStart
				                                            ,0
				                                            ,1
				                                            ,-1
				                                            ,GETDATE())
			                                            END
                                            COMMIT TRAN userPriv
                                            END TRY 
                                            BEGIN CATCH
	                                            ROLLBACK TRAN userPriv
	                                            DECLARE @msg varchar(1024)
	                                            SELECT @msg=ERROR_MESSAGE()
	                                            RAISERROR(@msg, 16,1)
                                            END CATCH";

        protected override void ApplyOverride(IMigrationContext context)
        {
            Table bargainsTable = context.Database.GetTable(_tableName);
            if (!bargainsTable.Columns.Contains(ColumnName))
            {
                var isServiceUserColumn = new Column(bargainsTable, ColumnName, DataType.Bit)
                                              {
                                                  Nullable = false,
                                              };

                isServiceUserColumn.AddDefaultConstraint("DF_Users_IsServiceUser").Text = "0";
                isServiceUserColumn.Create();
            }
            context.Connection.ExecuteNonQuery(CommandText);
        }
	}
}
