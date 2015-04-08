using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class RoleViewModel : EditableIdEntityViewModelBase<Role>, INameAspect
    {
        [RequiredLocalized]
        public string Name { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (RoleDomainEntityDto)domainEntityDto;
            Id = modelDto.Id;
            Name = modelDto.Name;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new RoleDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    Timestamp = Timestamp
                };
        }
    }
}