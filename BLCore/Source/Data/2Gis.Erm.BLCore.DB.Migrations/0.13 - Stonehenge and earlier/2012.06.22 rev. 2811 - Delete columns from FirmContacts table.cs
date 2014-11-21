using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
	[Migration(2811, "Удаление колонок из таблицы FirmContacts")]
	public class Migration2811 : TransactedMigration
	{
		protected override void ApplyOverride(IMigrationContext context)
		{
		    DeleteColumnsFromFirmContactsTable(context);
		}

        private static void DeleteColumnsFromFirmContactsTable(IMigrationContext context)
        {
            var cardFirmContacts = new SchemaQualifiedObjectName(ErmSchemas.Integration, "CardsFirmContacts");

            var firmContacts = new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "FirmContacts");
            var firmContactsTable = context.Database.GetTable(firmContacts);

            // объединяем по смыслу IsActive и ClosedForAscertainment
            const string closedForAscertainment = "ClosedForAscertainment";
            if (firmContactsTable.Columns.Contains(closedForAscertainment))
            {
                context.Connection.ExecuteNonQuery(string.Format(@"update {0} set IsActive = 0 where {1} = 1", firmContacts, closedForAscertainment));

                var closedForAscertainmentColumn = firmContactsTable.Columns[closedForAscertainment];
                closedForAscertainmentColumn.Drop();
            }

            // контакты должны физически удаляться чтобы не было проблем с unique dgppid
            const string isDeleted = "IsDeleted";
            if (firmContactsTable.Columns.Contains(isDeleted))
            {
                context.Connection.ExecuteNonQuery(string.Format(@"delete {0} from {0} as CFC inner join {1} as FC on CFC.FirmContactId = FC.Id and {2} = 1", cardFirmContacts, firmContacts, isDeleted));
                context.Connection.ExecuteNonQuery(string.Format(@"delete from {0} where {1} = 1", firmContacts, isDeleted));

                var column = firmContactsTable.Columns[isDeleted];
                column.Drop();
            }

            // вообще лишняя колонка, никому она не нужна
            const string ownerCode = "OwnerCode";
            if (firmContactsTable.Columns.Contains(ownerCode))
            {
                var column = firmContactsTable.Columns[ownerCode];
                column.Drop();
            }
        }
	}
}