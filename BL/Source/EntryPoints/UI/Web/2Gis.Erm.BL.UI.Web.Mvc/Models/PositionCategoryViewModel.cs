using System.ComponentModel.DataAnnotations;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class PositionCategoryViewModel : EditableIdEntityViewModelBase<PositionCategory>, INameAspect
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "ExportCodeRangeMessage")]
        [RequiredLocalized]
        public int ExportCode { get; set; }

        [RequiredLocalized]
        public bool IsSupportedByExport { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (PositionCategoryDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            ExportCode = modelDto.ExportCode;
            IsSupportedByExport = modelDto.IsSupportedByExport;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new PositionCategoryDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    ExportCode = ExportCode,
                    IsSupportedByExport = IsSupportedByExport,
                    Timestamp = Timestamp
                };
        }
    }
}