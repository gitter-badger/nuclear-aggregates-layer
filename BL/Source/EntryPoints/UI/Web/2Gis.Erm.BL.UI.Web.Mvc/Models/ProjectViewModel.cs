using System.Globalization;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class ProjectViewModel : EntityViewModelBase<Project>, IDisplayNameAspect
    {
        [RequiredLocalized]
        public string DisplayName { get; set; }

        public string NameLat { get; set; }

        public LookupField OrganizationUnit { get; set; }

        public string DefaultLanguage { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (ProjectDomainEntityDto)domainEntityDto;
            Id = modelDto.Id;
            DisplayName = modelDto.DisplayName;
            NameLat = modelDto.NameLat;
            OrganizationUnit = LookupField.FromReference(modelDto.OrganizationUnitRef);
            var culture = new CultureInfo(modelDto.DefaultLang);
            DefaultLanguage = culture.DisplayName;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new ProjectDomainEntityDto
                {
                    Id = Id,
                    DisplayName = DisplayName,
                    NameLat = NameLat,
                    OrganizationUnitRef = OrganizationUnit.ToReference(),
                    Timestamp = Timestamp,
                };
        }
    }
}