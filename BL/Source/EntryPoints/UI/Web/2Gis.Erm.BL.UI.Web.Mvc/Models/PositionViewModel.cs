﻿using System.ComponentModel.DataAnnotations;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class PositionViewModel : EditableIdEntityViewModelBase<Position>, ICompositePositionAspect, INameAspect
    {
        public bool IsPublished { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [DisplayNameLocalized("CompositePosition")]
        [Dependency(DependencyType.Hidden, "RestrictChildPositionPlatforms", "!this.checked")]
        [Dependency(DependencyType.DisableAndHide, "AdvertisementTemplate", "this.checked")]
        public bool IsComposite { get; set; }

        [RequiredLocalized]
        public PositionCalculationMethod CalculationMethod { get; set; }

        [RequiredLocalized]
        public PositionBindingObjectType BindingObjectType { get; set; }

        [RequiredLocalized]
        public PositionAccountingMethod AccountingMethod { get; set; }

        [RequiredLocalized]
        public LookupField Platform { get; set; }

        [RequiredLocalized]
        public LookupField PositionCategory { get; set; }

        public LookupField AdvertisementTemplate { get; set; }

        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "ExportCodeRangeMessage")]
        [RequiredLocalized]
        public int ExportCode { get; set; }

        public long? DgppId { get; set; }

        public bool IsControledByAmount { get; set; }

        public bool RestrictChildPositionPlatforms { get; set; }

        [Dependency(DependencyType.Disable, "RestrictChildPositionPlatforms", "this.value.toLowerCase() === 'false'")]
        public bool RestrictChildPositionPlatformsCanBeChanged { get; set; }

        [Dependency(DependencyType.ReadOnly, "AdvertisementTemplate", "this.value.toLowerCase() === 'true'")]
        public bool IsReadonlyTemplate { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (PositionDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            DgppId = modelDto.DgppId;
            ExportCode = modelDto.ExportCode;

            Name = modelDto.Name;
            IsComposite = modelDto.IsComposite;
            IsControledByAmount = modelDto.IsControlledByAmount;

            BindingObjectType = modelDto.BindingObjectTypeEnum;
            AccountingMethod = modelDto.AccountingMethodEnum;
            CalculationMethod = modelDto.CalculationMethodEnum;

            RestrictChildPositionPlatforms = modelDto.RestrictChildPositionPlatforms;
            RestrictChildPositionPlatformsCanBeChanged = modelDto.RestrictChildPositionPlatformsCanBeChanged;

            Platform = LookupField.FromReference(modelDto.PlatformRef);
            PositionCategory = LookupField.FromReference(modelDto.CategoryRef);
            AdvertisementTemplate = LookupField.FromReference(modelDto.AdvertisementTemplateRef);
            IsReadonlyTemplate = modelDto.IsReadOnlyTemplate;
            Timestamp = modelDto.Timestamp;
            IdentityServiceUrl = modelDto.IdentityServiceUrl;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            var dto = new PositionDomainEntityDto
            {
                Id = Id,
                DgppId = DgppId,
                ExportCode = ExportCode,
                Name = Name,
                IsComposite = IsComposite,
                IsControlledByAmount = IsControledByAmount,
                BindingObjectTypeEnum = BindingObjectType,
                AccountingMethodEnum = AccountingMethod,
                CalculationMethodEnum = CalculationMethod,
                AdvertisementTemplateRef = AdvertisementTemplate != null ? AdvertisementTemplate.ToReference() : null,
                PlatformRef = Platform.ToReference(),
                Timestamp = Timestamp,
                RestrictChildPositionPlatforms = RestrictChildPositionPlatforms
            };

            if (PositionCategory.Key != null)
            {
                dto.CategoryRef = PositionCategory.ToReference();
            }

            return dto;
        }
    }
}
