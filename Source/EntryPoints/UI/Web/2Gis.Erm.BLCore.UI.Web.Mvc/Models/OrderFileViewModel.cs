using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class OrderFileViewModel : FileViewModel<OrderFile>
    {
        [PresentationLayerProperty]
        public long OrderId { get; set; }

        public OrderFileKind FileKind { get; set; }

        [RequiredLocalized]
        public long? FileId { get; set; }

        public string Comment { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (OrderFileDomainEntityDto)domainEntityDto;
            Id = modelDto.Id;
            OrderId = modelDto.OrderId;
            FileId = modelDto.FileId;
            FileName = modelDto.FileName;
            FileContentLength = modelDto.FileContentLength;
            FileContentType = modelDto.FileContentType;
            FileKind = modelDto.FileKind;
            Comment = modelDto.Comment;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            if (FileId == null)
            {
                throw new NotificationException(BLResources.FileFiledIsRequired);
            }

            return new OrderFileDomainEntityDto
                {
                    Id = Id,
                    OrderId = OrderId,
                    FileId  = FileId.Value, 
                    FileName = FileName,
                    FileContentLength = FileContentLength,
                    FileContentType = FileContentType,
                    FileKind = FileKind,
                    Comment = Comment,
                    Timestamp = Timestamp
                };
        }
    }
}