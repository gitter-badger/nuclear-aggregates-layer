using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public class AdditionalFirmServiceViewModel : EntityViewModelBase<AdditionalFirmService>
    {
        private bool _isNew;

        public string ServiceCode { get; set; }

        public string Description { get; set; }

        [YesNoRadio]
        public bool IsManaged { get; set; }

        public override byte[] Timestamp { get; set; }

        public override bool IsNew
        {
            get { return _isNew; }
        }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var dto = (AdditionalFirmServiceDomainEntityDto)domainEntityDto;
            Id = dto.Id;
            IsManaged = dto.IsManaged;
            ServiceCode = dto.ServiceCode;
            Description = dto.Description;

            _isNew = Id == 0;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new AdditionalFirmServiceDomainEntityDto
                {
                    Id = Id,
                    IsManaged = IsManaged,
                    ServiceCode = ServiceCode,
                    Description = Description
                };
        }
    }
}