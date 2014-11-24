using System;
using System.Diagnostics;

using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Extensions;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration
{
    using CrmAnnotationMetadata = Metadata.Crm.Annotation;
    using CrmEntityName = Microsoft.Crm.SdkTypeProxy.EntityName;
    using ErmEntityName = Metadata.Erm.EntityName;

    [Migration(23494, "Migrates the activity annotations from CRM to ERM.", "s.pomadin")]
    public sealed class AnnotationMigration : ActivityMigration<AnnotationMigration.Annotation>
    {
        private const string InsertNoteTemplate = @"
INSERT INTO [Shared].[Notes] 
	([Id],[ParentType],[ParentId],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted],[OwnerCode],[Title],[Text])
SELECT {{0}}, {{1}}, Id, {{3}}, {{4}}, {{5}}, {{6}}, 0, {{7}}, {{8}}, {{9}} FROM {0} WHERE ReplicationCode = {{2}}";

        private const string InsertFileTemplate = @"
INSERT INTO [Shared].[Files] 
    ([Id], [FileName], [ContentType], [ContentLength], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn])
VALUES ({{9}},{{10}}, {{12}}, {{11}}, {{3}}, {{4}}, {{5}}, {{6}})

INSERT INTO [Shared].[FileBinaries] ([Id], [Data]) VALUES ({{9}}, {{13}})

INSERT INTO [Shared].[Notes] 
	([Id],[ParentType],[ParentId],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted],[OwnerCode],[Title],[FileId])
SELECT {{0}}, {{1}}, Id, {{3}}, {{4}}, {{5}}, {{6}}, 0, {{7}}, {{8}}, {{9}} FROM {0} WHERE ReplicationCode = {{2}}";

        private static readonly string InsertAppointmentNoteTemplate = string.Format(InsertNoteTemplate, "[Activity].[AppointmentBase]");
        private static readonly string InsertPhonecallNoteTemplate = string.Format(InsertNoteTemplate, "[Activity].[PhonecallBase]");
        private static readonly string InsertTaskNoteTemplate = string.Format(InsertNoteTemplate, "[Activity].[TaskBase]");
        private static readonly string InsertLetterNoteTemplate = string.Format(InsertNoteTemplate, "[Activity].[LetterBase]");
        
        private static readonly string InsertAppointmentFileTemplate = string.Format(InsertFileTemplate, "[Activity].[AppointmentBase]");
        private static readonly string InsertPhonecallFileTemplate = string.Format(InsertFileTemplate, "[Activity].[PhonecallBase]");
        private static readonly string InsertTaskFileTemplate = string.Format(InsertFileTemplate, "[Activity].[TaskBase]");
        private static readonly string InsertLetterFileTemplate = string.Format(InsertFileTemplate, "[Activity].[LetterBase]");
        
        public AnnotationMigration()
            : base(1000)
        {
        }

        internal override QueryExpression CreateQuery()
        {
            var query = new QueryExpression
                            {
                                EntityName = CrmEntityName.annotation.ToString(),
                                ColumnSet = new ColumnSet(new[]
                                                              {
                                                                  Metadata.Crm.Annotation.AnnotationId,
                                                                  Metadata.Crm.Annotation.CreatedBy,
                                                                  Metadata.Crm.Annotation.CreatedOn,
                                                                  Metadata.Crm.Annotation.ModifiedBy,
                                                                  Metadata.Crm.Annotation.ModifiedOn,
                                                                  Metadata.Crm.Annotation.OwnerId,
                                                                  Metadata.Crm.Annotation.ObjectTypeCode,
                                                                  Metadata.Crm.Annotation.ObjectId,
                                                                  Metadata.Crm.Annotation.IsDocument,
                                                                  Metadata.Crm.Annotation.Subject,
                                                                  Metadata.Crm.Annotation.NoteText,
                                                                  Metadata.Crm.Annotation.FileName,
                                                                  Metadata.Crm.Annotation.FileSize,
                                                                  Metadata.Crm.Annotation.MimeType,
                                                                  Metadata.Crm.Annotation.DocumentBody,
                                                              }),
                                Criteria = new FilterExpression
                                               {
                                                   Conditions =
                                                       {
                                                           new ConditionExpression(Metadata.Crm.Annotation.ObjectTypeCode,
                                                                                   ConditionOperator.In,
                                                                                   new[]
                                                                                       {
                                                                                           CrmEntityName.appointment.ToString(),
                                                                                           CrmEntityName.phonecall.ToString(),
                                                                                           CrmEntityName.task.ToString(),
                                                                                           CrmEntityName.letter.ToString(),
                                                                                           CrmEntityName.fax.ToString(),
                                                                                           CrmEntityName.email.ToString(),
                                                                                       }),
                                                       }
                                               },
                            };
            return query;
        }

        internal override Annotation ParseActivity(IActivityMigrationContextExtended context, DynamicEntity entity)
        {
            return Annotation.Create(context, entity);
        }

        internal override string BuildSql(Annotation annotation)
        {
            return
                annotation.IsDocument
                    ? QueryBuilder.Format(GetFileTemplate(annotation.ParentEntity),
                                          annotation.Id,
                                          annotation.ParentEntity,
                                          annotation.ParentId,
                                          annotation.CreatedBy,
                                          annotation.CreatedOn,
                                          annotation.ModifiedBy,
                                          annotation.ModifiedOn,
                                          annotation.OwnerId,
                                          annotation.Subject,
                                          annotation.FileId,
                                          annotation.FileName,
                                          annotation.FileSize,
                                          annotation.MimeType,
                                          annotation.DocumentBody)
                    : QueryBuilder.Format(GetNoteTemplate(annotation.ParentEntity),
                                          annotation.Id,
                                          annotation.ParentEntity,
                                          annotation.ParentId,
                                          annotation.CreatedBy,
                                          annotation.CreatedOn,
                                          annotation.ModifiedBy,
                                          annotation.ModifiedOn,
                                          annotation.OwnerId,
                                          annotation.Subject,
                                          annotation.NoteText);
        }

        private static string GetNoteTemplate(ErmEntityName entityName)
        {
            switch (entityName)
            {
                case ErmEntityName.Appointment:
                    return InsertAppointmentNoteTemplate;
                case ErmEntityName.Phonecall:
                    return InsertPhonecallNoteTemplate;
                case ErmEntityName.Task:
                    return InsertTaskNoteTemplate;
                case ErmEntityName.Letter:
                    return InsertLetterNoteTemplate;
            }

            throw new NotSupportedException(string.Format("The parent of type {0} is not supported.", entityName));
        }

        private static string GetFileTemplate(ErmEntityName entityName)
        {
            switch (entityName)
            {
                case ErmEntityName.Appointment:
                    return InsertAppointmentFileTemplate;
                case ErmEntityName.Phonecall:
                    return InsertPhonecallFileTemplate;
                case ErmEntityName.Task:
                    return InsertTaskFileTemplate;
                case ErmEntityName.Letter:
                    return InsertLetterFileTemplate;
            }

            throw new NotSupportedException(string.Format("The parent of type {0} is not supported.", entityName));
        }

        #region Annotation

        [DebuggerDisplay("Subject: {Subject}, IsDocument: {IsDocument}")]
        public sealed class Annotation
        {
            private Annotation()
            {
            }

            public long Id { get; private set; }
            public long CreatedBy { get; private set; }
            public DateTime CreatedOn { get; private set; }
            public long ModifiedBy { get; private set; }
            public DateTime ModifiedOn { get; private set; }
            public Guid ReplicationCode { get; private set; }
            public long? OwnerId { get; private set; }

            public ErmEntityName ParentEntity { get; private set; }
            public Guid ParentId { get; private set; }
            public string Subject { get; private set; }
            public bool IsDocument { get; private set; }
            public string NoteText { get; private set; }

            public long FileId { get; private set; }
            public string FileName { get; private set; }
            public long FileSize { get; private set; }
            public string MimeType { get; private set; }
            public byte[] DocumentBody { get; private set; }

            internal static Annotation Create(IActivityMigrationContextExtended context, DynamicEntity entity)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                if (entity.Name != CrmEntityName.annotation.ToString())
                {
                    throw new ArgumentException("The specified entity is not an annotation.", "entity");
                }

                var annotation = new Annotation
                {
                    Id = context.NewIdentity(),
                    ReplicationCode = context.Parse<Guid>(entity.Value(Metadata.Crm.Annotation.AnnotationId)),
                    CreatedBy = context.Parse<long>(entity.Value(Metadata.Crm.Annotation.CreatedBy)),
                    CreatedOn = context.Parse<DateTime>(entity.Value(Metadata.Crm.Annotation.CreatedOn)),
                    ModifiedBy = context.Parse<long>(entity.Value(Metadata.Crm.Annotation.ModifiedBy)),
                    ModifiedOn = context.Parse<DateTime>(entity.Value(Metadata.Crm.Annotation.ModifiedOn)),
                    OwnerId = context.Parse<long?>(entity.Value(Metadata.Crm.Annotation.OwnerId)),
                    ParentEntity = context.Parse<string>(entity.Value(Metadata.Crm.Annotation.ObjectTypeCode)).Map(EntityNameExtensions.ToEntityName),
                    IsDocument = context.Parse<bool>(entity.Value(Metadata.Crm.Annotation.IsDocument)),
                    Subject = context.Parse<string>(entity.Value(Metadata.Crm.Annotation.Subject)),
                    NoteText = context.Parse<string>(entity.Value(Metadata.Crm.Annotation.NoteText)),

                    FileId = context.NewIdentity(),
                    FileName = context.Parse<string>(entity.Value(Metadata.Crm.Annotation.FileName)),
                    FileSize = context.Parse<long>(entity.Value(Metadata.Crm.Annotation.FileSize)),
                    MimeType = context.Parse<string>(entity.Value(Metadata.Crm.Annotation.MimeType)),
                    DocumentBody = context.Parse<string>(entity.Value(Metadata.Crm.Annotation.DocumentBody)).Map(ToBytes),
                };

                var parentReference = entity.Value(Metadata.Crm.Annotation.ObjectId) as CrmReference;
                if (parentReference != null)
                {
                    annotation.ParentId = parentReference.Value;
                }

                return annotation;
            }

            private static byte[] ToBytes(string base64)
            {
                return base64 == null
                    ? null
                    : Convert.FromBase64String(base64);
            }
        }

        #endregion
    }
}