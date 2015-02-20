using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class DepartmentViewModel : EditableIdEntityViewModelBase<Department>, INameAspect
    {
        [DisplayNameLocalized("DepartmentName")]
        [RequiredLocalized]
        public string Name { get; set; }

        [DisplayNameLocalized("ParentDepartment")]
        public LookupField Parent { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (DepartmentDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            Parent = LookupField.FromReference(modelDto.ParentRef);
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new DepartmentDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    ParentRef = Parent.ToReference(),
                    Timestamp = Timestamp
                };
        }
    }
}