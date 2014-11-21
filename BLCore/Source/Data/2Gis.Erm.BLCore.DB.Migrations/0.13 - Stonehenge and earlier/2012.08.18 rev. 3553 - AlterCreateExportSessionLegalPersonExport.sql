SET NOCOUNT ON;

DECLARE @ids TABLE(Id int, [Timestamp] BINARY(8) NULL);
	
IF @isRecoverySession = 1
	BEGIN
		INSERT INTO @ids
		SELECT DISTINCT(EntityId), NULL
		FROM [Integration].[ExportSessionDetails]	IESD
		JOIN [Integration].[ExportSessions] IES ON IES.Id = IESD.IntegrationExportSessionId
		WHERE IES.EntityType = @entityTypeId
			AND (SELECT TOP 1 IsSuccessful FROM [Integration].[ExportSessionDetails] IESD2 WHERE IESD2.EntityId = IESD.EntityId ORDER BY Id DESC) = 0
	END ELSE
	BEGIN
		DECLARE @sessionExists BIT = 0;
		DECLARE @lastTimestamp TIMESTAMP;
		IF EXISTS(SELECT * FROM Integration.ExportSessions WHERE EntityType = @entityTypeId AND IsRecoverySession = 0)
			BEGIN
				SET @sessionExists = 1;	
				SELECT @lastTimestamp = MAX([LastTimestamp]) FROM Integration.ExportSessions WHERE EntityType = @entityTypeId AND IsRecoverySession = 0;
			END
			
		IF @entityTypeName = 'Order'
			BEGIN
				INSERT INTO @ids
				SELECT TOP 1000 * FROM
				(			
					SELECT
						Id,
						CASE WHEN T2 > T1 THEN T2 ELSE T1 END as [Timestamp]
					FROM (
						SELECT
							O.Id,
							MAX(O.[Timestamp]) AS T1,
							MAX(OP.[Timestamp]) AS T2
						FROM [Billing].[Orders]	O 
							LEFT OUTER JOIN [Billing].[OrderPositions] OP ON OP.OrderId = O.Id
						GROUP BY O.Id 
					) AS O
				) O
				WHERE @sessionExists = 0 OR [Timestamp] > @lastTimestamp
				ORDER BY [Timestamp]
			END 
		ELSE IF @entityTypeName = 'LegalPerson'
			BEGIN
				INSERT INTO @ids
				SELECT TOP 1000 
						[Id],
						[Timestamp] 
				FROM [Billing].[LegalPersons] lp
				WHERE @sessionExists = 0 OR [Timestamp] > @lastTimestamp
				ORDER BY [Timestamp]
			END
	END
		
DECLARE @sessionId INT;	
		
IF EXISTS(SELECT * FROM @ids)
	BEGIN
		DECLARE @newLastTimestamp BINARY(8);
		IF @isRecoverySession = 0
			SELECT @newLastTimestamp = MAX([Timestamp]) FROM @ids;

		INSERT INTO [Integration].[ExportSessions] ([EntityType], [IsRecoverySession], [LastTimestamp], [BeginDate])
		VALUES (@entityTypeId, @isRecoverySession, @newLastTimestamp, @beginDate)
		           
		SET @sessionId = SCOPE_IDENTITY();
		
		INSERT INTO [Integration].[ExportSessionDetails]
		SELECT @SessionId, Id, 1 --'1' means successful. If the serialized object fails validation later, '1' will be changed to '0'
		FROM @ids			
	END 
			
SELECT @sessionId;