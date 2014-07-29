using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2217, "Создание инфраструктуры необходимой для рассылки уведомлений через ERM")]
    public class Migration2217 : TransactedMigration
    {

        private readonly SchemaQualifiedObjectName _notificationEmails =
            new SchemaQualifiedObjectName(ErmSchemas.Shared, "NotificationEmails");

        private readonly SchemaQualifiedObjectName _notificationEmailsAttachments =
            new SchemaQualifiedObjectName(ErmSchemas.Shared, "NotificationEmailsAttachments");

        private readonly SchemaQualifiedObjectName _notificationAddresses =
            new SchemaQualifiedObjectName(ErmSchemas.Shared, "NotificationAddresses");

        private readonly SchemaQualifiedObjectName _notificationEmailsTo =
            new SchemaQualifiedObjectName(ErmSchemas.Shared, "NotificationEmailsTo");

        private readonly SchemaQualifiedObjectName _notificationEmailsCc =
           new SchemaQualifiedObjectName(ErmSchemas.Shared, "NotificationEmailsCc");

        private readonly SchemaQualifiedObjectName _notificationProcessings =
           new SchemaQualifiedObjectName(ErmSchemas.Shared, "NotificationProcessings");

        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateAddressesTable(context);
            CreateEmailsTable(context);
            CreateProcessingsTable(context);
            CreateEmailsAttachmentsTable(context);
            CreateEmailsToTable(context);
            CreateEmailsCcTable(context);
        }

        private void CreateAddressesTable(IMigrationContext context)
        {
            const String addressColumnName = "Address";
            const String displayNameColumnName = "DisplayName";
            const String displayNameEncodingColumnName = "DisplayNameEncoding";

            if (context.Database.Tables.Contains(_notificationAddresses.Name, _notificationAddresses.Schema))
            {
                return;
            }

            var table = new Table(context.Database, _notificationAddresses.Name, _notificationAddresses.Schema);
            table.Columns.Add(new Column(table, ErmTableUtils.CommonIdColumnName, DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1});
            table.Columns.Add(new Column(table, addressColumnName, DataType.NVarChar(1024)) { Nullable = false });
            table.Columns.Add(new Column(table, displayNameColumnName, DataType.NVarChar(1024)) { Nullable = true });
            table.Columns.Add(new Column(table, displayNameEncodingColumnName, DataType.NVarChar(1024)) { Nullable = true });
            ErmTableUtilsForOldIntKeys.CreateStandartColumns(table);
            table.Create();
            table.CreatePrimaryKey();
        }

        private void CreateEmailsTable(IMigrationContext context)
        {
            const String senderIdColumnName = "SenderId";
            const String subjectColumnName = "Subject";
            const String subjectEncodingColumnName = "SubjectEncoding";
            const String bodyColumnName = "Body";
            const String bodyEncodingColumnName = "BodyEncoding";
            const String isBodyHtmlColumnName = "IsBodyHtml";
            const String expirationTimeColumnName = "ExpirationTime";
            const String priorityColumnName = "Priority";
            const String maxAttemptsCountColumnName = "MaxAttemptsCount";

            if (context.Database.Tables.Contains(_notificationEmails.Name, _notificationEmails.Schema))
            {
                return;
            }

            var table = new Table(context.Database, _notificationEmails.Name, _notificationEmails.Schema);
            table.Columns.Add(new Column(table, ErmTableUtils.CommonIdColumnName, DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1});
            table.Columns.Add(new Column(table, senderIdColumnName, DataType.Int) { Nullable = true });
            table.Columns.Add(new Column(table, subjectColumnName, DataType.NVarChar(4000)) { Nullable = false });
            table.Columns.Add(new Column(table, subjectEncodingColumnName, DataType.NVarChar(128)) { Nullable = true });
            table.Columns.Add(new Column(table, bodyColumnName, DataType.NVarCharMax) { Nullable = false });
            table.Columns.Add(new Column(table, bodyEncodingColumnName, DataType.NVarChar(128)) { Nullable = true });
            table.Columns.Add(new Column(table, isBodyHtmlColumnName, DataType.Bit) { Nullable = false });
            table.Columns.Add(new Column(table, expirationTimeColumnName, DataType.DateTime2(2)) { Nullable = true });
            table.Columns.Add(new Column(table, priorityColumnName, DataType.NVarChar(128)) { Nullable = true });
            table.Columns.Add(new Column(table, maxAttemptsCountColumnName, DataType.Int) { Nullable = true });
            ErmTableUtilsForOldIntKeys.CreateStandartColumns(table);
            table.Create();
            table.CreatePrimaryKey();
            table.CreateForeignKey(senderIdColumnName, _notificationAddresses, ErmTableUtils.CommonIdColumnName);
        }

        private void CreateProcessingsTable(IMigrationContext context)
        {
            const String emailIdColumnName = "EmailId";
            const String statusColumnName = "Status";
            const String attemptsCountColumnName = "AttemptsCount";
            const String descriptionColumnName = "Description";

            if (context.Database.Tables.Contains(_notificationProcessings.Name, _notificationProcessings.Schema))
            {
                return;
            }

            var table = new Table(context.Database, _notificationProcessings.Name, _notificationProcessings.Schema);
            table.Columns.Add(new Column(table, ErmTableUtils.CommonIdColumnName, DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1});
            table.Columns.Add(new Column(table, emailIdColumnName, DataType.Int) { Nullable = false });
            table.Columns.Add(new Column(table, statusColumnName, DataType.Int) { Nullable = false });
            var attemptsCountColumn = new Column(table, attemptsCountColumnName, DataType.Int) { Nullable = false };
            attemptsCountColumn.AddDefaultConstraint("DF_" + table.Name + attemptsCountColumnName);
            attemptsCountColumn.DefaultConstraint.Text = "0";
            table.Columns.Add(attemptsCountColumn);
            table.Columns.Add(new Column(table, descriptionColumnName, DataType.NVarChar(4000)) { Nullable = true });
            ErmTableUtilsForOldIntKeys.CreateStandartColumns(table);
            table.Create();
            table.CreatePrimaryKey();
            table.CreateForeignKey(emailIdColumnName, _notificationEmails, ErmTableUtils.CommonIdColumnName);
        }

        private void CreateEmailsAttachmentsTable(IMigrationContext context)
        {
            const String emailIdColumnName = "EmailId";
            const String attachmentIdColumnName = "AttachmentId";

            if (context.Database.Tables.Contains(_notificationEmailsAttachments.Name, _notificationEmailsAttachments.Schema))
            {
                return;
            }

            var table = new Table(context.Database, _notificationEmailsAttachments.Name, _notificationEmailsAttachments.Schema);
            table.Columns.Add(new Column(table, ErmTableUtils.CommonIdColumnName, DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1});
            table.Columns.Add(new Column(table, emailIdColumnName, DataType.Int) { Nullable = false });
            table.Columns.Add(new Column(table, attachmentIdColumnName, DataType.Int) { Nullable = false });
            ErmTableUtilsForOldIntKeys.CreateStandartColumns(table);
            table.Create();
            table.CreatePrimaryKey();
            table.CreateForeignKey(emailIdColumnName,_notificationEmails, ErmTableUtils.CommonIdColumnName);
            table.CreateForeignKey(attachmentIdColumnName, ErmTableNames.Files, ErmTableUtils.CommonIdColumnName);
        }

        private void CreateEmailsToTable(IMigrationContext context)
        {
            const String emailIdColumnName = "EmailId";
            const String addressIdColumnName = "AddressId";

            if (context.Database.Tables.Contains(_notificationEmailsTo.Name, _notificationEmailsTo.Schema))
            {
                return;
            }

            var table = new Table(context.Database, _notificationEmailsTo.Name, _notificationEmailsTo.Schema);
            table.Columns.Add(new Column(table, ErmTableUtils.CommonIdColumnName, DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1});
            table.Columns.Add(new Column(table, emailIdColumnName, DataType.Int) { Nullable = false });
            table.Columns.Add(new Column(table, addressIdColumnName, DataType.Int) { Nullable = false });
            ErmTableUtilsForOldIntKeys.CreateStandartColumns(table);
            table.Create();
            table.CreatePrimaryKey();
            table.CreateForeignKey(emailIdColumnName, _notificationEmails, ErmTableUtils.CommonIdColumnName);
            table.CreateForeignKey(addressIdColumnName, _notificationEmailsTo, ErmTableUtils.CommonIdColumnName);
        }

        private void CreateEmailsCcTable(IMigrationContext context)
        {
            const String emailIdColumnName = "EmailId";
            const String addressIdColumnName = "AddressId";

            if (context.Database.Tables.Contains(_notificationEmailsCc.Name, _notificationEmailsCc.Schema))
            {
                return;
            }

            var table = new Table(context.Database, _notificationEmailsCc.Name, _notificationEmailsCc.Schema);
            table.Columns.Add(new Column(table, ErmTableUtils.CommonIdColumnName, DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1});
            table.Columns.Add(new Column(table, emailIdColumnName, DataType.Int) { Nullable = false });
            table.Columns.Add(new Column(table, addressIdColumnName, DataType.Int) { Nullable = false });
            ErmTableUtilsForOldIntKeys.CreateStandartColumns(table);
            table.Create();
            table.CreatePrimaryKey();
            table.CreateForeignKey(emailIdColumnName, _notificationEmails, ErmTableUtils.CommonIdColumnName);
            table.CreateForeignKey(addressIdColumnName, _notificationEmailsCc, ErmTableUtils.CommonIdColumnName);
        }
    }
}
