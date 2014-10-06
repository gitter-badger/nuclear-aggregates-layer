using System;
using System.Collections.Specialized;
using System.Linq;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Maintenance
{
    [Migration(23612, "Cleaning up the activities in MS CRM.", "s.pomadin")]
    public sealed class CrmActivitiesCleanup : TransactedMigration, INonDefaultDatabaseMigration
    {
        private static readonly string[] CleanupTemplates =
            {
                // чистим примечания
                "DELETE FROM [dbo].[AnnotationBase] WHERE [ObjectTypeCode] = {0};",

                // чистим связи с внешними объектами
                "DELETE FROM [dbo].[ActivityPartyBase] FROM [dbo].[ActivityPartyBase] partyBase JOIN [dbo].[ActivityPointerBase] pointerBase ON partyBase.[ActivityId] = pointerBase.[ActivityId] WHERE ActivityTypeCode = {0};",
                
                // удаляем декомпозированные действия
                "DELETE FROM {2};",
                "DELETE FROM {1};",
                "DELETE FROM [dbo].[ActivityPointerBase] WHERE ActivityTypeCode = {0};"
            };

        private static readonly string[] EmailCleanupTemplates =
            {
                // чистим объекты, связанные с электронной почтой
                "DELETE FROM [dbo].[EmailHashBase]",

                // чистим вложения для emails
                "DELETE FROM [dbo].[ActivityMimeAttachment] FROM [dbo].[ActivityMimeAttachment] attachment JOIN [dbo].[ActivityPointerBase] pointerBase ON attachment.[ActivityId] = pointerBase.[ActivityId] WHERE pointerBase.ActivityTypeCode = {0};",

                // чистим связи с внешними объектами
                "DELETE FROM [dbo].[ActivityPartyBase] FROM [dbo].[ActivityPartyBase] partyBase JOIN [dbo].[ActivityPointerBase] pointerBase ON partyBase.[ActivityId] = pointerBase.[ActivityId] WHERE ActivityTypeCode = {0};",
                
                // удаляем декомпозированные действия
                "DELETE FROM {2};",
                "DELETE FROM {1};",
                "DELETE FROM [dbo].[ActivityPointerBase] WHERE ActivityTypeCode = {0};"
            };
        
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.StatementTimeout = 1 * 60 * 60; // a hour

            foreach (var tuple in new[]
                {
                    Tuple.Create(CleanupTemplates, 4201, "[dbo].[AppointmentBase]", "[dbo].[AppointmentExtensionBase]"),
                    Tuple.Create(CleanupTemplates, 4202, "[dbo].[EmailBase]", "[dbo].[EmailExtensionBase]"),
                    Tuple.Create(EmailCleanupTemplates, 4204, "[dbo].[FaxBase]", "[dbo].[FaxExtensionBase]"),
                    Tuple.Create(CleanupTemplates, 4207, "[dbo].[LetterBase]", "[dbo].[LetterExtensionBase]"),
                    Tuple.Create(CleanupTemplates, 4210, "[dbo].[PhonecallBase]", "[dbo].[PhonecallExtensionBase]"),
                    Tuple.Create(CleanupTemplates, 4212, "[dbo].[TaskBase]", "[dbo].[TaskExtensionBase]"),
                })
            {
                try
                {
                    context.Connection.ExecuteNonQuery(BuildSql(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cleaning up activity with code {0} was failed.", tuple.Item1);
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        private static StringCollection BuildSql(string[] templates, int typeCode, string tableName, string extensionTableName)
        {
            var statements = new StringCollection();
            statements.AddRange(templates.Select(statementTemplate => string.Format(statementTemplate, typeCode, tableName, extensionTableName)).ToArray());
            return statements;
        }

        public ErmConnectionStringKey ConnectionStringKey { get { return ErmConnectionStringKey.CrmDatabase; } }
    }
}
