using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class OrganizationUnitViewModel : EditableIdEntityViewModelBase<OrganizationUnit>, INameAspect
    {
        [RequiredLocalized]
        [StringLengthLocalized(100)]
        public string Name { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(5)]
        public string Code { get; set; }

        public int? DgppId { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("CountryName")]
        public LookupField Country { get; set; }

        [RequiredLocalized]
        public DateTime FirstEmitDate { get; set; }

        public DateTime? ErmLaunchDate { get; set; }

        public DateTime? InfoRussiaLaunchDate { get; set; }

        public string SyncCode1C { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("TimeZoneId")]
        public LookupField TimeZone { get; set; }

        [RequiredLocalized]
        public string ElectronicMedia { get; set; }

        [Dependency(DependencyType.ReadOnly, "FirstEmitDate", "this.value=='False'")]
        public bool CanEditFirstEmitDate { get; set; }
        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (OrganizationUnitDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Code = modelDto.Code;
            Name = modelDto.Name;
            DgppId = modelDto.DgppId;
            Country = LookupField.FromReference(modelDto.CountryRef);
            FirstEmitDate = modelDto.FirstEmitDate;
            ErmLaunchDate = modelDto.ErmLaunchDate;
            InfoRussiaLaunchDate = modelDto.InfoRussiaLaunchDate;
            SyncCode1C = modelDto.SyncCode1C;
            TimeZone = LookupField.FromReference(modelDto.TimeZoneRef);
            ElectronicMedia = modelDto.ElectronicMedia;
            CanEditFirstEmitDate = !InfoRussiaLaunchDate.HasValue && !ErmLaunchDate.HasValue;
            Timestamp = modelDto.Timestamp;
            ViewConfig.ReadOnly = modelDto.Id != 0 && !modelDto.IsActive;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            var dto = new OrganizationUnitDomainEntityDto
                {
                    Id = Id,
                    Code = Code,
                    Name = Name,
                    DgppId = DgppId,
                    CountryRef = Country.ToReference(),
                    TimeZoneRef = TimeZone.ToReference(),
                    FirstEmitDate = FirstEmitDate,
                    ErmLaunchDate = ErmLaunchDate,
                    InfoRussiaLaunchDate = InfoRussiaLaunchDate,
                    SyncCode1C = SyncCode1C,
                    ElectronicMedia = ElectronicMedia,
                    Timestamp = Timestamp
                };

            if (IsNew)
            {
                dto.FirstEmitDate = FirstEmitDate;
            }

            return dto;
        }
    }
}