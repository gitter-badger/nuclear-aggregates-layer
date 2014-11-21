using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(1650, "Создание функциональной привилегии \"Массовая рассылка email\"")]
    public class Migration_1650 : TransactedMigration
	{
        private const string CommandText = @"
                                            BEGIN TRAN userPriv

                                            BEGIN TRY 

			                                            DECLARE @sendMassEmail INT;
			                                            DECLARE @sendMassEmailPrvDepthMaskIdStart INT;
			                                            DECLARE @PrivilegeName VARCHAR(200);

			                                            SET @sendMassEmail = 540
			                                            SET @sendMassEmailPrvDepthMaskIdStart = 131
			                                            SET @PrivilegeName = 'PrvSendMassEmail'

			                                            IF NOT EXISTS (SELECT 1 FROM [Security].[Privileges] WHERE Id = @sendMassEmail)
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
				                                            @sendMassEmail
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
				                                            @sendMassEmail
				                                            ,'FPrvDpthGranted'
				                                            ,1
				                                            ,@sendMassEmailPrvDepthMaskIdStart
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
