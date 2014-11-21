DECLARE @LastTimeStamp binary(8)

SET @LastTimeStamp = (SELECT max(Timestamp) from BusinessDirectory.Territories)
INSERT INTO Shared.CrmReplicationInfo (Entity, LastTimestamp, ModifiedOn) VALUES ('Territory', @LastTimeStamp, GETUTCDATE())

IF NOT EXISTS(SELECT * FROM Integration.FirmsForPostIntegrationActivities)
	BEGIN
		SET @LastTimeStamp = (SELECT max(Timestamp) from BusinessDirectory.Firms)
		INSERT INTO Shared.CrmReplicationInfo (Entity, LastTimestamp, ModifiedOn) VALUES ('Firm', @LastTimeStamp, GETUTCDATE())

		SET @LastTimeStamp = (SELECT max(Timestamp) from BusinessDirectory.FirmAddresses)
		INSERT INTO Shared.CrmReplicationInfo (Entity, LastTimestamp, ModifiedOn) VALUES ('FirmAddress', @LastTimeStamp, GETUTCDATE())
	END
ELSE
	BEGIN
		SET @LastTimeStamp = (SELECT min(Timestamp) from BusinessDirectory.Firms WHERE Id In (SELECT FirmId FROM Integration.FirmsForPostIntegrationActivities))
		INSERT INTO Shared.CrmReplicationInfo (Entity, LastTimestamp, ModifiedOn) VALUES ('Firm', @LastTimeStamp, GETUTCDATE())

		SET @LastTimeStamp = (SELECT min(Timestamp) from BusinessDirectory.FirmAddresses WHERE Id IN (SELECT Id FROM BusinessDirectory.Firms WHERE Timestamp = @LastTimeStamp))
		INSERT INTO Shared.CrmReplicationInfo (Entity, LastTimestamp, ModifiedOn) VALUES ('FirmAddress', @LastTimeStamp, GETUTCDATE())
	END