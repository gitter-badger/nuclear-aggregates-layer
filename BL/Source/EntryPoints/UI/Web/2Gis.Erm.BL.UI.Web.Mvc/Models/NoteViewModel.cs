using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public class NoteViewModel : FileViewModel<Note>, ITitleAspect
    {
        [StringLengthLocalized(64)]
        [RequiredLocalized]
        public string Title { get; set; }
        
        public string Text { get; set; }

        public long? FileId { get; set; }

        public long ParentId { get; set; }
        public IEntityType ParentTypeName { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (NoteDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            ParentId = modelDto.ParentRef.Id.Value;
            ParentTypeName = modelDto.ParentTypeName;
            Title = modelDto.Title;
            Text = modelDto.Text;
            FileId = modelDto.FileId;
            FileName = modelDto.FileName;
            FileContentLength = modelDto.FileContentLength;
            FileContentType = modelDto.FileContentType;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new NoteDomainEntityDto
                {
                    Id = Id,
                    ParentRef = new EntityReference(ParentId),
                    ParentTypeName = ParentTypeName,
                    Title = Title,
                    Text = Text,
                    FileId = FileId,
                    FileName = FileName,
                    FileContentLength = FileContentLength,
                    FileContentType = FileContentType,
                    Timestamp = Timestamp
                };
        }
    }
}
