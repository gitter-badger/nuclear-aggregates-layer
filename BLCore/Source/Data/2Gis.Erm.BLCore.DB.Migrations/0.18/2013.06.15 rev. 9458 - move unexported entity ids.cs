using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9458, "Перенос неэкспортированных объектов")]
    public class Migration9458 : TransactedMigration
    {
        private const string Command = @"
-- Скрипт выбирает сущности, которые не были экспортированы до сих пор
insert into Integration.ExportFailedEntities([EntityName], [EntityId])
select
	ExportSessions.EntityType,
	ExportSessionDetails.EntityId
from 
	Integration.ExportSessions
	inner join Integration.ExportSessionDetails on ExportSessionDetails.IntegrationExportSessionId = ExportSessions.Id
where 
	ExportSessions.EntityType <> 186 -- Рекламные материалы можно не продолжать долбить
	and (case ExportSessions.EntityType
		when 147 then (select ModifiedOn from Billing.LegalPersons where LegalPersons.Id = ExportSessionDetails.EntityId)
		when 151 then (select ModifiedOn from Billing.Orders where Orders.Id = ExportSessionDetails.EntityId)
		else GETDATE()
	end) > '2013-03-01' -- И те сущности, что с марта не менялись тоже выгружать уже не обязательно
group by 
	ExportSessions.EntityType, 
	ExportSessionDetails.EntityId
having
	(select IsSuccessful from Integration.ExportSessionDetails as ESD where ESD.IntegrationExportSessionId = max(ExportSessions.Id) and ESD.EntityId = ExportSessionDetails.EntityId ) = 0
";
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Command);
        }
    }
}
