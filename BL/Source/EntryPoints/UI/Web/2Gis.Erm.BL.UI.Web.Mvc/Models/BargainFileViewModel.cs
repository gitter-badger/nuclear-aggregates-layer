using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class BargainFileViewModel : FileViewModel<BargainFile>, IFileNameAspect
    {
        [PresentationLayerProperty]
        public long BargainId { get; set; }

        public BargainFileKind FileKind { get; set; }

        public string Comment { get; set; }

        [RequiredLocalized]
        public long? FileId { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (BargainFileDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            BargainId = modelDto.BargainRef.Id.Value;
            FileId = modelDto.FileId;
            FileName = modelDto.FileName;
            FileContentType = modelDto.FileContentType;
            FileContentLength = modelDto.FileContentLength;
            FileKind = modelDto.FileKind;
            Comment = modelDto.Comment;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            if (FileId == null)
            {
                throw new ArgumentNullException();
            }

            return new BargainFileDomainEntityDto
                {
                    Id = Id,
                    BargainRef = new EntityReference(BargainId),
                    FileId = FileId.Value,
                    FileName = FileName,
                    FileContentType = FileContentType,
                    FileContentLength = FileContentLength,
                    FileKind = FileKind,
                    Comment = Comment,
                    Timestamp = Timestamp
                };
        }
    }
}