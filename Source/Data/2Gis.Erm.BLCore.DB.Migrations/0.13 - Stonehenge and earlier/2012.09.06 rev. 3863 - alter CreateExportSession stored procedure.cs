using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3863, "Изменение хранимой процедуры CreateExportSession")]
    public sealed class Migration3863 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string createExportSession = "CreateExportSession";
            const string createExportSessionText = @"
SET NOCOUNT ON;

DECLARE @ids TABLE(Id int NOT NULL, [Timestamp] BINARY(8) NULL);
	
IF (@isRecoverySession = 1)
BEGIN
	INSERT INTO @ids
	SELECT DISTINCT(EntityId), NULL
	FROM Integration.ExportSessionDetails	IESD
	JOIN Integration.ExportSessions IES ON IES.Id = IESD.IntegrationExportSessionId
	WHERE IES.EntityType = @entityTypeId
		AND (SELECT TOP 1 IsSuccessful FROM Integration.ExportSessionDetails IESD2 WHERE IESD2.EntityId = IESD.EntityId ORDER BY Id DESC) = 0
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
		SELECT TOP 1000 * FROM
		(			
		SELECT O.Id, MaxTimeStamp =
		CASE
			WHEN OP.MaxPositionsTimestamp > O.Timestamp THEN OP.MaxPositionsTimestamp
			ELSE O.Timestamp
		END
		FROM Billing.Orders O
		LEFT JOIN (SELECT OrderId, MAX([TimeStamp]) MaxPositionsTimestamp FROM Billing.OrderPositions OP GROUP BY OrderId) OP ON OP.OrderId = O.Id
		) O
		WHERE @sessionExists = 0 OR MaxTimeStamp > @lastTimestamp
		ORDER BY MaxTimeStamp
	END 
	ELSE IF (@entityTypeName = N'LegalPerson')
	BEGIN
		INSERT INTO @ids
		SELECT TOP 1000 
				Id,
				[Timestamp] 
		FROM Billing.LegalPersons lp
		WHERE @sessionExists = 0 OR [Timestamp] > @lastTimestamp
		ORDER BY [Timestamp]
	END
	ELSE IF (@entityTypeName = N'Advertisement')
	BEGIN
		INSERT INTO @ids
		SELECT TOP 1000 * FROM
		(
		SELECT A.Id, MaxTimeStamp =
		CASE
			WHEN AE.MaxElementsTimestamp > A.Timestamp THEN
				CASE
					WHEN AE.MaxElementsTimestamp > OPA.MaxPositionsTimestamp THEN AE.MaxElementsTimestamp
					ELSE OPA.MaxPositionsTimestamp
				END
			ELSE
				CASE
					WHEN A.Timestamp > OPA.MaxPositionsTimestamp THEN A.Timestamp
					ELSE OPA.MaxPositionsTimestamp
				END
		END
		FROM Billing.Advertisements A
		LEFT JOIN (SELECT AdvertisementId, MAX([TimeStamp]) MaxElementsTimestamp FROM Billing.AdvertisementElements GROUP BY AdvertisementId) AE ON AE.AdvertisementId = A.Id
		LEFT JOIN (SELECT AdvertisementId, MAX([TimeStamp]) MaxPositionsTimestamp FROM Billing.OrderPositionAdvertisement GROUP BY AdvertisementId) OPA ON OPA.AdvertisementId = A.Id
		) A
		WHERE @sessionExists = 0 OR A.MaxTimeStamp > @lastTimestamp
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
			
SELECT @sessionId;";

            var createExportSessionSp = context.Database.StoredProcedures[createExportSession, ErmSchemas.Integration];
            createExportSessionSp.TextBody = createExportSessionText;
            createExportSessionSp.Alter();
        }
    }
}