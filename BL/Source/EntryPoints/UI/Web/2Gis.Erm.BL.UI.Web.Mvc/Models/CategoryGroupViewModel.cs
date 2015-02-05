using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public class CategoryGroupViewModel : EditableIdEntityViewModelBase<CategoryGroup>
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string CategoryGroupName { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("CategoryGroupRate")]
        public decimal GroupRate { get; set; }

        public override byte[] Timestamp { get; set; }

        public override bool IsSecurityRoot
        {
            get
            {
                return true;
            }
        }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (CategoryGroupDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            CategoryGroupName = modelDto.CategoryGroupName;
            GroupRate = modelDto.GroupRate;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new CategoryGroupDomainEntityDto
                {
                    Id = Id,
                    CategoryGroupName = CategoryGroupName,
                    GroupRate = GroupRate,
                    OwnerRef = Owner.ToReference(),
                    Timestamp = Timestamp
                };
        }
    }
}