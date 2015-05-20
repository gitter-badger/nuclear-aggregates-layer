using System;
using System.IO;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class FileWithContent : IEntity, IEntityKey, IAuditableEntity
    {
        public FileWithContent()
        {
        }

        public FileWithContent(File file)
        {
            Id = file.Id;
            FileName = file.FileName;
            ContentLength = file.ContentLength;
            ContentType = file.ContentType;
            CreatedBy = file.CreatedBy;
            CreatedOn = file.CreatedOn;
            ModifiedBy = file.ModifiedBy;
            ModifiedOn = file.ModifiedOn;
            DgppId = file.DgppId;
        }

        public long Id { get; set; }

        public string FileName { get; set; }

        public long ContentLength { get; set; }

        public string ContentType { get; set; }

        public long CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public long? DgppId { get; set; }

        public Stream Content { get; set; }
    }
}