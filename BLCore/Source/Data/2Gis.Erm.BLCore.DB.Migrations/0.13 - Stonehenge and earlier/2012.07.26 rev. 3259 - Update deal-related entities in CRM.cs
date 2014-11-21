using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3259, "Обновление сущностей, связанных со сделкой в CRM.")]
    public class Migration3259 : TransactedMigration, INonDefaultDatabaseMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            String updateScript = String.Empty;

            #region Меняем этапы сделки в CRM

            // У текущих сделок, где на данный момент цель указана «Определение ЛПР» изменить значение в соотв. со следующей логикой:
            // 1.	Определяем последнее (по дате создания) действие (Звонок, встреча) у которого текущий этап «Определение ЛПР»
            // a.	  Если действие «Звонок», то этап сделки меняем на «Сбор информации»;
            // b.	 Если действие «Встреча», то этап сделки меняем на «Проведение презентации».

            updateScript +=
                String.Format(
@"UPDATE {0} SET Dg_dealstage = 
(CASE WHEN (SELECT TOP 1 ActivityTypeCode FROM [dbo].[ActivityPointerBase] apb 
WHERE apb.RegardingObjectId = OpportunityId AND apb.StateCode <> 2 ORDER BY CreatedOn DESC) = 4201 THEN 3 ELSE 1 END) WHERE Dg_dealstage = 2
GO
", CrmTableNames.OpportunityExtensionBase);

            // Меняем удаленные этапы сделки: "Проведение презентации рекламных возможностей" => "Проведение презентации" 
            // Согласование условий и сроков принятия решения => "Согласование КП"
            const String dg_dealStageColumn = "Dg_dealstage";
            updateScript += String.Format("UPDATE {0} SET {1} = {1} - 1 WHERE {1} IN (4, 6)\nGO\n", CrmTableNames.OpportunityExtensionBase, dg_dealStageColumn);

            #endregion

            #region Меняем этапы сделки в ERM

            // Обновляем стадии сделки в ERM. Текущая база - CRM, так что к базе ERM стучимся с помощью context.Options.EnvironmentSuffix.
            updateScript +=
                String.Format(
@"UPDATE {0}.Billing.DealExtensions SET DealStage = 
(CASE WHEN
	(SELECT TOP 1 apb.ActivityTypeCode FROM {0}.Billing.Deals d
		JOIN dbo.OpportunityBase ob ON d.ReplicationCode = ob.OpportunityId
		JOIN dbo.ActivityPointerBase apb ON apb.RegardingObjectId = ob.OpportunityId
		WHERE apb.StateCode <> 2 AND d.Id = Id
		ORDER BY apb.CreatedOn DESC) = 4201 
			THEN 3 ELSE 1 END
)
WHERE DealStage = 2
GO
", context.ErmDatabaseName);

            updateScript += String.Format("UPDATE {0}.Billing.DealExtensions SET DealStage = DealStage - 1 WHERE DealStage IN (4, 6)\nGO\n", context.ErmDatabaseName);

            #endregion Меняем этапы сделки в ERM

            // Обновляем поле "Результат" для звонка и встречи.
            const String dg_resultColumn = "Dg_result";
            const String updateResultCommandFormat = "UPDATE {0} SET {1} = 4 WHERE {1} > 1 AND {1} < 4\nGO\n";

            updateScript += String.Format(updateResultCommandFormat, CrmTableNames.AppointmentExtensionBase, dg_resultColumn);
            updateScript += String.Format(updateResultCommandFormat, CrmTableNames.PhoneCallExtensionBase, dg_resultColumn);

            const String dg_purposeColumn = "Dg_purpose";
            // У текущих действий «Звонок», где цель указана «Определение ЛПР» изменить значение на «Первый звонок (Знакомство)».
            updateScript += String.Format("UPDATE {0} SET {1} = 1 WHERE {1} = 2\nGO\n", CrmTableNames.PhoneCallExtensionBase, dg_purposeColumn);

            // У текущих действий «Встреча», где цель указана «Определение ЛПР» изменить значение на «Проведение презентации продукта».
            updateScript += String.Format("UPDATE {0} SET {1} = 3 WHERE {1} = 2\nGO\n", CrmTableNames.AppointmentExtensionBase, dg_purposeColumn);

            context.Connection.ExecuteNonQuery(updateScript);
        }

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.CrmDatabase; }
        }
    }
}
