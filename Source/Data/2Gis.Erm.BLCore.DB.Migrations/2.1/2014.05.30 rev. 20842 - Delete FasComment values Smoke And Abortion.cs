using System.Globalization;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(20842, "Из enum FasComment удалены значения Smoke=2 и Abortion=5", "i.maslennikov")]
    public class Migration20842 : TransactedMigration
    {
        #region Список Ids Рекламных материалов которые нужно перевыгрузить на production после наката этой миграции:
        /* 
32132
32693
43201
55885
55895
66094
70559
81937
85621
86764
99199
99239
105421
106803
106818
112779
126277
135755
143693
147990
149389
167212
167249
169414
170350
170600
208227
208408
208521
208522
214201
225241
246935
265752
265753
265754
265765
298733
312445
314595
325929
330289
330391
332054
332204*/
        #endregion
        
        private const string CommandText = @"
DECLARE @ModifiedAdvertisementIds TABLE (Id bigint NOT NULL)

UPDATE [Billing].[AdvertisementElements]
SET FasCommentType = 6,
	Text = null,
	ModifiedOn = GETUTCDATE(),
	ModifiedBy = 30
OUTPUT inserted.AdvertisementId INTO @ModifiedAdvertisementIds
WHERE FasCommentType = 5 OR FasCommentType = 2

DECLARE @CurrentAdvertisementId bigint
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE type = 'P'AND id = OBJECT_ID('Adm.PushAdvertisement'))
BEGIN
    while exists(SELECT * FROM @ModifiedAdvertisementIds)
	BEGIN
		SET @CurrentAdvertisementId = (SELECT TOP 1 Id FROM @ModifiedAdvertisementIds)
		print @CurrentAdvertisementId
		BEGIN TRY
			EXEC Adm.PushAdvertisement @CurrentAdvertisementId
		END TRY
		BEGIN CATCH
			   --print ERROR_MESSAGE() + ' ' + ERROR_SEVERITY() + ' ' + ERROR_STATE()
		END CATCH
		DELETE FROM @ModifiedAdvertisementIds WHERE Id = @CurrentAdvertisementId
	END
END";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(CommandText);
        }
    }
}
