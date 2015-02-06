using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class ContributionTypeViewModel : EditableIdEntityViewModelBase<ContributionType>
    {
        [RequiredLocalized]
        public string Name { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (ContributionTypeDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new ContributionTypeDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    Timestamp = Timestamp
                };
        }
    }
}