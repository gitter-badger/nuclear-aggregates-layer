using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

using MessageType = DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public class ThemeTemplateViewModel : FileViewModel<ThemeTemplate>
    {
        public ThemeTemplateCode TemplateCode { get; set; }

        public bool IsSkyScraper { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("Archive")]
        public long? FileId { get; set; }

        public bool IsTemplateUsedInThemes { get; set; }

        public override byte[] Timestamp { get; set; }

        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (ThemeTemplateDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            TemplateCode = (ThemeTemplateCode)modelDto.TemplateCode;
            FileId = modelDto.FileId;
            IsSkyScraper = modelDto.IsSkyScraper;

            // Поля требуются для рендеринга контрола загрузки файлов на форме;
            // не изменяются клиентом и не требуют обратного преобразования в сущность
            FileName = modelDto.FileName;
            FileContentLength = modelDto.FileContentLength;
            FileContentType = modelDto.FileContentType;

            if (modelDto.IsTemplateUsedInThemes)
            {
                IsTemplateUsedInThemes = true;
                Message = BLResources.TemplateIsUsedInThemes;
                MessageType = MessageType.Info;
            }

            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            if (!FileId.HasValue)
            {
                throw new NotificationException(BLResources.FileFiledIsRequired);
            }

            return new ThemeTemplateDomainEntityDto
            {
                Id = Id,
                TemplateCode = (int)TemplateCode,
                FileId = FileId.Value,
                IsSkyScraper = IsSkyScraper,
                Timestamp = Timestamp
            };
        }
    }
}