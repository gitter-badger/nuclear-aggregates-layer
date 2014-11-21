using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(8784, "Изменения в хранимке ReplicateEntitiesToCrm")]
    public sealed class Migration8784 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string command = @"ALTER PROCEDURE [Shared].[ReplicateEntitiesToCrm]
AS

BEGIN
	DECLARE @Ids Shared.Int32IdsTableType
	DECLARE @LastTimeStamp binary(8)
	DECLARE @CurrentTimestamp binary(8)
	DECLARE @ReplicatedCount int

-- 4) Реплицируем территории
	SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = 'Territory')
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
		INSERT INTO Shared.CrmReplicationDetails (Entity, ReplicatedCount, LastTimestamp, CreatedOn) VALUES ('Territory', @ReplicatedCount, @CurrentTimestamp, GETUTCDATE())
		UPDATE Shared.CrmReplicationInfo SET ModifiedOn = GETUTCDATE(), LastTimeStamp = @CurrentTimestamp
		WHERE Entity = 'Territory'
		
		DELETE FROM @Ids
		SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = 'Territory')
	END

-- 6) Реплицируем фирмы
	SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = 'Firm')
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
		INSERT INTO Shared.CrmReplicationDetails (Entity, ReplicatedCount, LastTimestamp, CreatedOn) VALUES ('Firm', @ReplicatedCount, @CurrentTimestamp, GETUTCDATE())
		UPDATE Shared.CrmReplicationInfo SET ModifiedOn = GETUTCDATE(), LastTimeStamp = @CurrentTimestamp
		WHERE Entity = 'Firm'

		DELETE FROM @Ids
		SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = 'Firm')
	END

-- 7) Реплицируем адреса фирм
	SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = 'FirmAddress')
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
		INSERT INTO Shared.CrmReplicationDetails (Entity, ReplicatedCount, LastTimestamp, CreatedOn) VALUES ('FirmAddress', @ReplicatedCount, @CurrentTimestamp, GETUTCDATE())
		UPDATE Shared.CrmReplicationInfo SET ModifiedOn = GETUTCDATE(), LastTimeStamp = @CurrentTimestamp
		WHERE Entity = 'FirmAddress'

		DELETE FROM @Ids
		SET @LastTimeStamp = (SELECT LastTimeStamp FROM Shared.CrmReplicationInfo WHERE Entity = 'FirmAddress')
	END
END";
            context.Connection.ExecuteNonQuery(command);
        }
    }
}
