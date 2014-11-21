using System;
using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
	[Migration(3255, "Удаляем колонку DgpppId из таблицы FirmContacts")]
	public sealed class Migration3255: TransactedMigration
	{
		protected override void ApplyOverride(IMigrationContext context)
		{
		    DropDgppIdUniqueConstraint(context);
		    DropDgppIdColumn(context);
		}

        private static void DropDgppIdUniqueConstraint(IMigrationContext context)
        {
            const string firmContacts = "FirmContacts";
            const string indexName = "UQ_Contacts_DgppId";

            var firmContactsTable = context.Database.Tables[firmContacts, ErmSchemas.BusinessDirectory];
            var index = firmContactsTable.Indexes.Cast<Index>().SingleOrDefault(x => string.Equals(x.Name, indexName, StringComparison.OrdinalIgnoreCase));
            if (index == null)
                return;
            
            index.Drop();
        }

        private static void DropDgppIdColumn(IMigrationContext context)
        {
            const string firmContacts = "FirmContacts";
            const string dgppId = "DgppId";

            var firmContactsTable = context.Database.Tables[firmContacts, ErmSchemas.BusinessDirectory];
            var dgppIdColumn = firmContactsTable.Columns[dgppId];

            if (dgppIdColumn == null)
                return;

            dgppIdColumn.Drop();
        }
	}
}
