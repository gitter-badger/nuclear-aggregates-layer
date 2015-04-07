using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class AdvertisementViewModel : EntityViewModelBase<Advertisement>, ISelectableToWhiteListAspect, INameAspect, IDummyAdvertisementAspect
    {
        [PresentationLayerProperty]
        [Dependency(DependencyType.Hidden, "AdsElemsContainer", "this.value=='0'")]
        public override long Id { get; set; }

        public bool HasAssignedOrder { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(128)]
        public string Name { get; set; }

        [StringLengthLocalized(512)]
        public string Comment { get; set; }

        [RequiredLocalized]
        public LookupField AdvertisementTemplate { get; set; }

        [RequiredLocalized]
        public LookupField Firm { get; set; }

        [Dependency(DependencyType.NotRequiredDisableHide, "Firm", "this.value=='True'")]
        [Dependency(DependencyType.ReadOnly, "Name", "this.value=='True'")]
        [Dependency(DependencyType.ReadOnly, "Comment", "this.value=='True'")]
        [Dependency(DependencyType.ReadOnly, "AdvertisementTemplate", "this.value=='True'")]
        [Dependency(DependencyType.ReadOnly, "IsSelectedToWhiteList", "this.value=='True'")]
        public bool IsDummy { get; set; }

        public bool IsSelectedToWhiteList { get; set; }

        [Dependency(DependencyType.ReadOnly, "AdvertisementTemplate", "this.value=='True'")]
        public bool IsReadOnlyTemplate { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var advDto = domainEntityDto as AdvertisementDomainEntityDto;
            if (advDto == null)
            {
                return;
            }

            Id = advDto.Id;
            Name = advDto.Name;
            Comment = advDto.Comment;
            IsSelectedToWhiteList = advDto.IsSelectedToWhiteList;
            Firm = LookupField.FromReference(advDto.FirmRef);
            IsDummy = !advDto.FirmRef.Id.HasValue;
            AdvertisementTemplate = LookupField.FromReference(advDto.AdvertisementTemplateRef);
            HasAssignedOrder = advDto.HasAssignedOrder;
            IsReadOnlyTemplate = advDto.IsReadOnlyTemplate;         

            Timestamp = advDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new AdvertisementDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    Comment = Comment,
                    IsSelectedToWhiteList = IsSelectedToWhiteList,
                    FirmRef = Firm.ToReference(),
                    AdvertisementTemplateRef = AdvertisementTemplate.ToReference(),
                    HasAssignedOrder = HasAssignedOrder,
                    
                    // TODO: Выпилить OwnerCode
                    OwnerRef = Owner != null ? new EntityReference(Owner.Key.HasValue ? Owner.Key.Value : 0) : new EntityReference(),
                    Timestamp = Timestamp
                };
        }
    }
}