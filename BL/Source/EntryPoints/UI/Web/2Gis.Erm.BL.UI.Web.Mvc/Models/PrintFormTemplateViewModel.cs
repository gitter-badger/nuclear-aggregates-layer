using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class PrintFormTemplateViewModel : FileViewModel<PrintFormTemplate>, IFileNameAspect
    {
        [RequiredLocalized]
        public TemplateCode TemplateCode { get; set; }

        [RequiredLocalized]
        public long? FileId { get; set; }

        [PresentationLayerProperty]
        public long? BranchOfficeOrganizationUnitId { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (PrintFormTemplateDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            BranchOfficeOrganizationUnitId = modelDto.BranchOfficeOrganizationUnitRef.Id;
            FileId = modelDto.FileId;
            FileName = modelDto.FileName;
            FileContentType = modelDto.FileContentType;
            FileContentLength = modelDto.FileContentLength;
            TemplateCode = modelDto.TemplateCode;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            if (FileId == null)
            {
                throw new NotificationException(BLResources.FileFiledIsRequired);
            }

            return new PrintFormTemplateDomainEntityDto
                {
                    Id = Id,
                    BranchOfficeOrganizationUnitRef = new EntityReference(BranchOfficeOrganizationUnitId),
                    FileId = FileId.Value, 
                    FileName = FileName,
                    FileContentType = FileContentType,
                    FileContentLength = FileContentLength,
                    TemplateCode = TemplateCode,
                    Timestamp = Timestamp
                };
        }
    }
}
