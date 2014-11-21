using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6374, "Исправление хранимки CreateExportSession. Добавил тип сущности Theme.")]
    public sealed class Migration6374 : TransactedMigration
    {
        #region Текст запроса

        private const string Query = @"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [Integration].[CreateExportSession]
	@entityTypeName AS NVARCHAR(50), 
	@entityTypeId INT,
	@isRecoverySession INT,
	@beginDate AS DATETIME
AS

SET NOCOUNT ON;

DECLARE @ids TABLE(Id int NOT NULL, [Timestamp] BINARY(8) NULL);
	
IF (@isRecoverySession = 1)
BEGIN
	INSERT INTO @ids
	SELECT DISTINCT(IESD.EntityId), NULL as timestampfield
	FROM Integration.ExportSessionDetails IESD
	JOIN 
	(
		SELECT TOP 300 IESD2.Id
		FROM Integration.ExportSessionDetails IESD
		JOIN Integration.ExportSessions IES ON IES.Id = IESD.IntegrationExportSessionId AND IES.EntityType = @entityTypeId
		JOIN Integration.ExportSessionDetails IESD2 ON IESD2.EntityId = IESD.EntityId
		GROUP BY IESD2.Id, IESD2.IsSuccessful
		HAVING IESD2.IsSuccessful = 0 AND IESD2.Id = MAX(IESD.Id)
	) LAST_IESD ON LAST_IESD.Id = IESD.Id
END
ELSE
BEGIN
	DECLARE @sessionExists BIT = 0;
	DECLARE @lastTimestamp TIMESTAMP;
	IF (EXISTS(SELECT * FROM Integration.ExportSessions WHERE EntityType = @entityTypeId AND IsRecoverySession = 0))
	BEGIN
		SET @sessionExists = 1;	
		SELECT @lastTimestamp = MAX(LastTimestamp) FROM Integration.ExportSessions WHERE EntityType = @entityTypeId AND IsRecoverySession = 0;
	END

	IF (@entityTypeName = N'Order')
	BEGIN
		INSERT INTO @ids
		SELECT TOP 100 * FROM
		(			
		SELECT O.Id, MaxTimeStamp =
		CASE
			WHEN OP.MaxPositionsTimestamp > O.Timestamp THEN OP.MaxPositionsTimestamp
			ELSE O.Timestamp
		END
		FROM Billing.Orders O
		LEFT JOIN
		(
			SELECT OrderId, MAX([MaxOPATimeStamp]) MaxPositionsTimestamp FROM
			(
			SELECT OP.OrderId, MaxOPATimeStamp =
			(
			CASE
				WHEN OPA.MaxTimestamp IS NULL THEN OP.TimeStamp
				WHEN OPA.MaxTimestamp > OP.TimeStamp THEN OPA.MaxTimestamp
				ELSE OP.TimeStamp
			END
			)
			FROM Billing.OrderPositions OP
			LEFT JOIN
			(
				SELECT OrderPositionId, MAX([TimeStamp]) MaxTimestamp
				FROM Billing.OrderPositionAdvertisement OPA
				GROUP BY OrderPositionId
			) OPA
			ON OPA.OrderPositionId = OP.Id
			) OP
			GROUP BY OrderId
		) OP ON OP.OrderId = O.Id
		) O
		WHERE @sessionExists = 0 OR MaxTimeStamp > @lastTimestamp
		ORDER BY MaxTimeStamp
	END 
	ELSE IF (@entityTypeName = N'LegalPerson')
	BEGIN
		INSERT INTO @ids
		SELECT TOP 1000 * FROM (
		SELECT lp.Id, [Timestamp] = 
		(
			CASE
				WHEN lpp.Timestamp IS NULL THEN lp.TimeStamp
				WHEN lpp.Timestamp > lp.TimeStamp THEN lpp.Timestamp
				ELSE lp.TimeStamp
			END
		)
		FROM Billing.LegalPersons lp LEFT JOIN Billing.LegalPersonProfiles lpp ON lp.Id = lpp.LegalPersonId AND lpp.IsMainProfile = 1) tl
		WHERE @sessionExists = 0 OR [Timestamp] > @lastTimestamp
		ORDER BY [Timestamp]
	END
	ELSE IF (@entityTypeName = N'Advertisement')
	BEGIN
		INSERT INTO @ids
		SELECT TOP 10 * FROM
		(
		SELECT A.Id, MaxTimeStamp =
		CASE
			WHEN AE.MaxElementsTimestamp > A.Timestamp THEN AE.MaxElementsTimestamp
			ELSE A.Timestamp
		END
		FROM Billing.Advertisements A
		LEFT JOIN
		(
			SELECT AdvertisementId, MaxElementsTimestamp = MAX(MaxFilesTimeStamp) FROM
			(
			SELECT AE.AdvertisementId, MaxFilesTimeStamp =
			(
			CASE
				WHEN F.TimeStamp IS NULL THEN AE.TimeStamp
				WHEN AE.TimeStamp > F.TimeStamp THEN AE.TimeStamp
				ELSE F.TimeStamp
			END
			) FROM Billing.AdvertisementElements AE
			LEFT JOIN Shared.Files F ON F.Id = AE.FileId
			) AE
			GROUP BY AdvertisementId
		) AE ON AE.AdvertisementId = A.Id
		) A
		WHERE @sessionExists = 0 OR A.MaxTimeStamp > @lastTimestamp
		ORDER BY MaxTimeStamp
	END
	ELSE IF (@entityTypeName = N'Theme')
	BEGIN
		-- Выбираем измененные тематики, либо тематики, привязанные к изменённому шаблону
		INSERT INTO @ids
		SELECT Themes.Id, case when Themes.[Timestamp] > ThemeTemplates.[Timestamp] then Themes.[Timestamp] else ThemeTemplates.[Timestamp] end as MaxTimeStamp 
		FROM Billing.Themes inner join Billing.ThemeTemplates on ThemeTemplates.Id = Themes.ThemeTemplateId
		WHERE @sessionExists = 0 OR Themes.[Timestamp] > @lastTimestamp OR ThemeTemplates.[Timestamp] > @lastTimestamp
		ORDER BY MaxTimeStamp
	END
END
		
DECLARE @sessionId INT;	
		
IF EXISTS(SELECT * FROM @ids)
BEGIN
	DECLARE @newLastTimestamp BINARY(8);
	IF @isRecoverySession = 0
		SELECT @newLastTimestamp = MAX([Timestamp]) FROM @ids;

	INSERT INTO Integration.ExportSessions (EntityType, IsRecoverySession, LastTimestamp, BeginDate)
	VALUES (@entityTypeId, @isRecoverySession, @newLastTimestamp, @beginDate)
		           
	SET @sessionId = SCOPE_IDENTITY();
		
	INSERT INTO Integration.ExportSessionDetails
	SELECT @SessionId, Id, 1 --'1' means successful. If the serialized object fails validation later, '1' will be changed to '0'
	FROM @ids			
END 
			
SELECT @sessionId;
";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
