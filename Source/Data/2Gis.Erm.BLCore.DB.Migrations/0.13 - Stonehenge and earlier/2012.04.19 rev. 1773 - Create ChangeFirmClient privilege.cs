using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(1773, "Создание функциональной привилегии \"Смена клиента у фирмы\"")]
    public class Migration_1773 : TransactedMigration
    {
        private const string CommandText = @"
                                            BEGIN TRAN userPriv

                                            BEGIN TRY 

			                                            DECLARE @changeFirmClient INT;
			                                            DECLARE @changeFirmClientPrvDepthMaskIdStart INT;
			                                            DECLARE @PrivilegeName VARCHAR(200);

			                                            SET @changeFirmClient = 541
			                                            SET @changeFirmClientPrvDepthMaskIdStart = 132
			                                            SET @PrivilegeName = 'PrvChangeFirmClient'

			                                            IF NOT EXISTS (SELECT 1 FROM [Security].[Privileges] WHERE Id = @changeFirmClient)
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
				                                            @changeFirmClient
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
				                                            @changeFirmClient
				                                            ,'FPrvDpthGranted'
				                                            ,1
				                                            ,@changeFirmClientPrvDepthMaskIdStart
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
            context.Connection.ExecuteNonQuery(CommandText);
        }
    }
}
