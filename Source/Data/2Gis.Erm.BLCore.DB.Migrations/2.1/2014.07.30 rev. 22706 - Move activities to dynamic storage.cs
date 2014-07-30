using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(22706, "Moves activities from separate tables to dynamic storage ones.", "s.pomadin")]
    public class Migration22706 : TransactedMigration
    {
		private const int ClientIdPropertyId = 36;
	    private const int ContactIdPropertyId = 37;
	    private const int FirmIdPropertyId = 38;
	    
		private const string CopyActivities = @"
INSERT INTO [DynamicStorage].[DictionaryEntityInstances] 
	([Id],[IsActive],[IsDeleted],[OwnerCode],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn])
SELECT 
	[Id],[IsActive],[IsDeleted],[OwnerCode],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn]
FROM [Activity].[ActivityInstances]
";
	    private const string CopyMainProperties = @"
INSERT INTO [DynamicStorage].[DictionaryEntityPropertyInstances]
	([EntityInstanceId],[PropertyId],[NumericValue])
SELECT 
	[Id],{0},{1}
FROM [Activity].[ActivityInstances]
";
	    private const string CopyExtraProperties = @"
INSERT INTO [DynamicStorage].[DictionaryEntityPropertyInstances]
	([EntityInstanceId],[PropertyId],[TextValue],[NumericValue],[DateTimeValue])
SELECT 
	[ActivityId],[PropertyId],[TextValue],[NumericValue],[DateTimeValue]
FROM [Activity].[ActivityPropertyInstances]
";
	    private const string DropTables = @"
DROP TABLE [Activity].[ActivityPropertyInstances]
DROP TABLE [Activity].[ActivityInstances]
";
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(CopyActivities);

	        foreach (var settings in new[]
		        {
			        Tuple.Create(ClientIdPropertyId, "[ClientId]"),
			        Tuple.Create(ContactIdPropertyId, "[ContactId]"),
			        Tuple.Create(FirmIdPropertyId, "[FirmId]"),
		        })
	        {
		        context.Connection.ExecuteNonQuery(string.Format(CopyMainProperties, settings.Item1, settings.Item2));
	        }

	        context.Connection.ExecuteNonQuery(CopyExtraProperties);

	        context.Connection.ExecuteNonQuery(DropTables);
        }
    }
}
