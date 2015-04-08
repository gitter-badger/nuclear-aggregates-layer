using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Emirates
{
    public sealed class EmiratesBargainTypeViewModel : EditableIdEntityViewModelBase<BargainType>, INameAspect, IEmiratesAdapted
    {
        [RequiredLocalized]
        public string Name { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (EmiratesBargainTypeDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new EmiratesBargainTypeDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    Timestamp = Timestamp,
                };
        }
    }
}