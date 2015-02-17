using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public class ThemeViewModel : FileViewModel<Theme>
    {
        [NonZeroInteger]
        [RequiredLocalized]
        [OnlyDigitsLocalized]
        public override long Id { get; set; }

        [RequiredLocalized]
        public string Name { get; set; }

        [RequiredLocalized]
        public string Description { get; set; }

        [RequiredLocalized]
        public LookupField ThemeTemplate { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("BeginDistributionDate")]
        public DateTime BeginDistribution { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("EndDistributionDate")]
        [GreaterOrEqualThan("BeginDistribution", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "EndDistributionDateMustBeGreaterOrEqualThanBeginDistributionDate")]
        public DateTime EndDistribution { get; set; }

        [YesNoRadio]
        [RequiredLocalized]
        [DisplayNameLocalized("ThemeIsDefault")]
        public bool IsDefault { get; set; }

        [RequiredLocalized]
        public long? FileId { get; set; }

        public override byte[] Timestamp { get; set; }

        public int OrganizationUnitCount { get; set; }

        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (ThemeDomainEntityDto)domainEntityDto;
            if (modelDto.Id != 0)
            {
                modelDto.ThemeTemplateRef.Name = modelDto.ThemeTemplateCode.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture);
            }

            Id = modelDto.Id;
            Name = modelDto.Name;
            ThemeTemplate = LookupField.FromReference(modelDto.ThemeTemplateRef);
            Description = modelDto.Description;
            BeginDistribution = modelDto.BeginDistribution;
            EndDistribution = modelDto.EndDistribution;
            IsDefault = modelDto.IsDefault;
            FileId = modelDto.FileId;
            OrganizationUnitCount = modelDto.OrganizationUnitCount;

            // Поля требуются для рендеринга контрола загрузки файлов на форме;
                // не изменяются клиентом и не требуют обратного преобразования в сущность
            FileName = modelDto.FileName;
            FileContentLength = modelDto.FileContentLength;
            FileContentType = modelDto.FileContentType;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            if (ThemeTemplate == null || !ThemeTemplate.Key.HasValue)
            {
                throw new NotificationException(BLResources.FiledThemeTemplateIsRequired);
            }

            if (!FileId.HasValue)
            {
                throw new NotificationException(BLResources.FileFiledIsRequired);
        }

            var dto = new ThemeDomainEntityDto
                                {
                Id = Id,
                Name = Name,
                Description = Description,
                ThemeTemplateRef = ThemeTemplate.ToReference(),
                BeginDistribution = BeginDistribution,
                EndDistribution = EndDistribution,
                IsDefault = IsDefault,
                FileId = FileId.Value,
                FileName = FileName,
                OrganizationUnitCount = OrganizationUnitCount,
                Timestamp = Timestamp
                                };

            return dto;
        }
        }
    }
