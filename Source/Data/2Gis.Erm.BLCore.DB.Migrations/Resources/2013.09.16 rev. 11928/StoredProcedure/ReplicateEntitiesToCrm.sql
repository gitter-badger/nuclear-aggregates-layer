-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
--	 16.09.2013, v.lapeev: Перевел строки в Unicode
ALTER PROCEDURE [Shared].[ReplicateEntitiesToCrm]
AS
BEGIN
	DECLARE @Ids Shared.Int64IdsTableType
	DECLARE @LastTimeStamp binary(8)
	DECLARE @CurrentTimestamp binary(8)
	DECLARE @ReplicatedCount int

-- 4) Реплицируем территории
	SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = N'Territory')
	while exists(SELECT * FROM BusinessDirectory.Territories WHERE Timestamp > @LastTimeStamp)
	BEGIN
		INSERT INTO @ids
		SELECT TOP 200 Id
		FROM BusinessDirectory.Territories
		WHERE [Timestamp] > @LastTimeStamp
		ORDER BY [Timestamp]

		EXEC BusinessDirectory.ReplicateTerritories @ids

		SET @CurrentTimestamp = (SELECT max(Timestamp) FROM BusinessDirectory.Territories WHERE Id IN (SELECT Id FROM @ids))
		SET @ReplicatedCount = (SELECT Count(*) FROM @Ids)
		INSERT INTO Shared.CrmReplicationDetails (Entity, ReplicatedCount, LastTimestamp, CreatedOn) VALUES (N'Territory', @ReplicatedCount, @CurrentTimestamp, GETUTCDATE())
		UPDATE Shared.CrmReplicationInfo SET ModifiedOn = GETUTCDATE(), LastTimeStamp = @CurrentTimestamp
		WHERE Entity = N'Territory'
		
		DELETE FROM @Ids
		SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = N'Territory')
	END

-- 6) Реплицируем фирмы
	SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = N'Firm')
	while exists(SELECT * FROM BusinessDirectory.Firms WHERE Timestamp > @LastTimeStamp)
	BEGIN
		INSERT INTO @ids
		SELECT TOP 200 Id
		FROM BusinessDirectory.Firms
		WHERE [Timestamp] > @LastTimeStamp
		ORDER BY [Timestamp]

		EXEC BusinessDirectory.ReplicateFirms @ids
		
		SET @CurrentTimestamp = (SELECT max(Timestamp) FROM BusinessDirectory.Firms WHERE Id IN (SELECT Id FROM @ids))
		SET @ReplicatedCount = (SELECT Count(*) FROM @Ids)
		INSERT INTO Shared.CrmReplicationDetails (Entity, ReplicatedCount, LastTimestamp, CreatedOn) VALUES (N'Firm', @ReplicatedCount, @CurrentTimestamp, GETUTCDATE())
		UPDATE Shared.CrmReplicationInfo SET ModifiedOn = GETUTCDATE(), LastTimeStamp = @CurrentTimestamp
		WHERE Entity = 'Firm'

		DELETE FROM @Ids
		SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = N'Firm')
	END

-- 7) Реплицируем адреса фирм
	SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = N'FirmAddress')
	while exists(SELECT * FROM BusinessDirectory.FirmAddresses WHERE Timestamp > @LastTimeStamp)
	BEGIN
		INSERT INTO @ids
		SELECT TOP 200 Id
		FROM BusinessDirectory.FirmAddresses
		WHERE [Timestamp] > @LastTimeStamp
		ORDER BY [Timestamp]

		EXEC BusinessDirectory.ReplicateFirmAddresses @ids
		
		SET @CurrentTimestamp = (SELECT max(Timestamp) FROM BusinessDirectory.FirmAddresses WHERE Id IN (SELECT Id FROM @ids))
		SET @ReplicatedCount = (SELECT Count(*) FROM @Ids)
		INSERT INTO Shared.CrmReplicationDetails (Entity, ReplicatedCount, LastTimestamp, CreatedOn) VALUES (N'FirmAddress', @ReplicatedCount, @CurrentTimestamp, GETUTCDATE())
		UPDATE Shared.CrmReplicationInfo SET ModifiedOn = GETUTCDATE(), LastTimeStamp = @CurrentTimestamp
		WHERE Entity = N'FirmAddress'

		DELETE FROM @Ids
		SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = N'FirmAddress')
	END
END
