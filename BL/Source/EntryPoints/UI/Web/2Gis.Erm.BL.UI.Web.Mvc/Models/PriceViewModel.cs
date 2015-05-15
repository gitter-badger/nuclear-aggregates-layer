using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class PriceViewModel : EntityViewModelBase<Platform.Model.Entities.Erm.Price>, IPublishableAspect, INameAspect
    {
        public string Name { get; set; }

        [DisplayNameLocalized("PriceCreateDate")]
        public DateTime CreateDate { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("PricePublishDate")]
        public DateTime PublishDate { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("PublishBeginDate")]
        [GreaterOrEqualThan("PublishDate",
            ErrorMessageResourceType = typeof(BLResources),
            ErrorMessageResourceName = "BeginDateMustBeGreaterOrEqualThenPublishDate")]
        public DateTime BeginDate { get; set; }

        [PresentationLayerProperty]
        public long CurrencyId { get; set; }

        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        [DisplayNameLocalized("PriceIsPublished")]
        [YesNoRadio]
        public bool IsPublished { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (PriceDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            CreateDate = modelDto.CreateDate;
            BeginDate = modelDto.BeginDate;
            PublishDate = modelDto.PublishDate;
            OrganizationUnit = LookupField.FromReference(modelDto.OrganizationUnitRef);
            CurrencyId = modelDto.CurrencyRef.Id ?? 0;
            IsPublished = modelDto.IsPublished;
            Timestamp = modelDto.Timestamp;
            Name = modelDto.Name;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new PriceDomainEntityDto
                {
                    Id = Id,
                    CreateDate = CreateDate,
                    BeginDate = BeginDate,
                    PublishDate = PublishDate,
                    OrganizationUnitRef = new EntityReference(OrganizationUnit.Key.Value, OrganizationUnit.Value),
                    CurrencyRef = new EntityReference(CurrencyId),
                    IsPublished = IsPublished,
                    Timestamp = Timestamp
                };
        }
    }
}