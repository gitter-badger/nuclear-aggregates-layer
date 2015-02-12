using System.ComponentModel.DataAnnotations;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class OperationTypeViewModel : EntityViewModelBase<OperationType>
    {
        [Required]
        public string Name { get; set; }

        [DisplayNameLocalized("OperationTypeDescription")]
        public string Description { get; set; }

        [DisplayNameLocalized("OperationTypeKind")]
        public bool IsPlus { get; set; }

        [DisplayNameLocalized("InSyncWith1C")]
        public bool IsInSyncWith1C { get; set; }

        public string SyncCode1C { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (OperationTypeDomainEntityDto)domainEntityDto;
            Id = modelDto.Id;
            Name = modelDto.Name;
            Description = modelDto.Description;
            IsPlus = modelDto.IsPlus;
            IsInSyncWith1C = modelDto.IsInSyncWith1C;
            SyncCode1C = modelDto.SyncCode1C;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new OperationTypeDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    Description = Description,
                    IsPlus = IsPlus,
                    IsInSyncWith1C = IsInSyncWith1C,
                    SyncCode1C = SyncCode1C,
                    Timestamp = Timestamp
                };
        }
    }
}