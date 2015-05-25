using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class TerritoryViewModel : EditableIdEntityViewModelBase<Territory>, INameAspect
    {
        [RequiredLocalized]
        [StringLengthLocalized(50, MinimumLength = 1)]
        public string Name { get; set; }

        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (TerritoryDomainEntityDto)domainEntityDto;
            Id = modelDto.Id;
            Name = modelDto.Name;
            OrganizationUnit = LookupField.FromReference(modelDto.OrganizationUnitRef);
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            var dto = new TerritoryDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    Timestamp = Timestamp,
                    OrganizationUnitRef = OrganizationUnit.ToReference()
                };

            return dto;
        }
    }
}