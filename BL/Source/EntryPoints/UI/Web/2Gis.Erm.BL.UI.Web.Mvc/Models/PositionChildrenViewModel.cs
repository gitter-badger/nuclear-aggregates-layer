using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class PositionChildrenViewModel : EntityViewModelBase<PositionChildren>
    {
        [RequiredLocalized]
        public LookupField MasterPosition { get; set; }

        [RequiredLocalized]
        public LookupField ChildPosition { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (PositionChildrenDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            MasterPosition = LookupField.FromReference(modelDto.MasterPositionRef);
            ChildPosition = LookupField.FromReference(modelDto.ChildPositionRef);
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new PositionChildrenDomainEntityDto
                {
                    Id = Id,
                    MasterPositionRef = MasterPosition.ToReference(),
                    ChildPositionRef = ChildPosition.ToReference(),
                    Timestamp = Timestamp
                };
        }
    }
}