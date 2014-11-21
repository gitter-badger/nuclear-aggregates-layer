-- Порядок реплицирования сущностей:
-- 1) [Billing].[OrganizationUnits]
-- 2) [Billing].[Currencies]
-- 3) [BusinessDirectory].[Categories]
-- 4) [BusinessDirectory].[Territories]
-- 5) [Billing].[Clients]
-- 6) [BusinessDirectory].[Firms]
-- 7) [BusinessDirectory].[FirmAddresses]
-- 8) [Billing].[Contacts]
-- 9) [Billing].[Positions]
-- 10) [Billing].[BranchOffices]
-- 11) [Billing].[BranchOfficeOrganizationUnits]
-- 12) [Billing].[LegalPersons]
-- 13) [Billing].[Accounts]
-- 14) [Billing].[OperationTypes]
-- 15) [Billing].[AccountDetails]
-- 16) [Billing].[Deals]
-- 17) [Billing].[Limits]
-- 18) [Billing].[Orders]
-- 19) [Billing].[OrderPositions]
-- 20) [Billing].[Bargains]

ALTER PROCEDURE [Shared].[ReplicateEntitiesToCrm]
AS

BEGIN
	DECLARE @Ids Shared.Int32IdsTableType
	DECLARE @LastTimeStamp binary(8)
	DECLARE @CurrentTimestamp binary(8)
	DECLARE @ReplicatedCount int

-- 4) Реплицируем территории
	begin tran
	SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = 'Territory')
	while exists(SELECT * FROM BusinessDirectory.Territories WHERE Timestamp > @LastTimeStamp)
	BEGIN
		INSERT INTO @ids
		SELECT TOP 1000 Id
		FROM BusinessDirectory.Territories
		WHERE [Timestamp] > @LastTimeStamp
		ORDER BY [Timestamp]

		EXEC BusinessDirectory.ReplicateTerritories @ids

		SET @CurrentTimestamp = (SELECT max(Timestamp) FROM BusinessDirectory.Territories WHERE Id IN (SELECT Id FROM @ids))
		SET @ReplicatedCount = (SELECT Count(*) FROM @Ids)
		INSERT INTO Shared.CrmReplicationDetails (Entity, ReplicatedCount, LastTimestamp, CreatedOn) VALUES ('Territory', @ReplicatedCount, @CurrentTimestamp, GETUTCDATE())
		UPDATE Shared.CrmReplicationInfo SET ModifiedOn = GETUTCDATE(), LastTimeStamp = @CurrentTimestamp
		WHERE Entity = 'Territory'
		
		DELETE FROM @Ids
		SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = 'Territory')
	END
	commit tran

-- 6) Реплицируем фирмы
	begin tran
	SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = 'Firm')
	while exists(SELECT * FROM BusinessDirectory.Firms WHERE Timestamp > @LastTimeStamp)
	BEGIN
		INSERT INTO @ids
		SELECT TOP 1000 Id
		FROM BusinessDirectory.Firms
		WHERE [Timestamp] > @LastTimeStamp
		ORDER BY [Timestamp]

		EXEC BusinessDirectory.ReplicateFirms @ids
		
		SET @CurrentTimestamp = (SELECT max(Timestamp) FROM BusinessDirectory.Firms WHERE Id IN (SELECT Id FROM @ids))
		SET @ReplicatedCount = (SELECT Count(*) FROM @Ids)
		INSERT INTO Shared.CrmReplicationDetails (Entity, ReplicatedCount, LastTimestamp, CreatedOn) VALUES ('Firm', @ReplicatedCount, @CurrentTimestamp, GETUTCDATE())
		UPDATE Shared.CrmReplicationInfo SET ModifiedOn = GETUTCDATE(), LastTimeStamp = @CurrentTimestamp
		WHERE Entity = 'Firm'

		DELETE FROM @Ids
		SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = 'Firm')
	END
	commit tran

-- 7) Реплицируем адреса фирм
	begin tran
	SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = 'FirmAddress')
	while exists(SELECT * FROM BusinessDirectory.FirmAddresses WHERE Timestamp > @LastTimeStamp)
	BEGIN
		INSERT INTO @ids
		SELECT TOP 1000 Id
		FROM BusinessDirectory.FirmAddresses
		WHERE [Timestamp] > @LastTimeStamp
		ORDER BY [Timestamp]

		EXEC BusinessDirectory.ReplicateFirmAddresses @ids
		
		SET @CurrentTimestamp = (SELECT max(Timestamp) FROM BusinessDirectory.FirmAddresses WHERE Id IN (SELECT Id FROM @ids))
		SET @ReplicatedCount = (SELECT Count(*) FROM @Ids)
		INSERT INTO Shared.CrmReplicationDetails (Entity, ReplicatedCount, LastTimestamp, CreatedOn) VALUES ('FirmAddress', @ReplicatedCount, @CurrentTimestamp, GETUTCDATE())
		UPDATE Shared.CrmReplicationInfo SET ModifiedOn = GETUTCDATE(), LastTimeStamp = @CurrentTimestamp
		WHERE Entity = 'FirmAddress'

		DELETE FROM @Ids
		SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = 'FirmAddress')
	END
	commit tran
END