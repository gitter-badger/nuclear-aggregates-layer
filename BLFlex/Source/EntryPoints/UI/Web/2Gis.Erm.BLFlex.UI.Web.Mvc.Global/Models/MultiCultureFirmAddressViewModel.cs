using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using MessageType = DoubleGis.Erm.Platform.UI.Metadata.UIElements.MessageType;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models
{
    public sealed class MultiCultureFirmAddressViewModel : EntityViewModelBase<FirmAddress>, IAddressAspect, IRussiaAdapted, ICyprusAdapted, ICzechAdapted, IChileAdapted, IUkraineAdapted, IKazakhstanAdapted
    {
        public LookupField Firm { get; set; }

        [StringLengthLocalized(4000)]
        public string Address { get; set; }

        public string PaymentMethods { get; set; }

        public string WorkingTime { get; set; }

        [YesNoRadio]
        [DisplayNameLocalized("IsHidden")]
        public bool ClosedForAscertainment { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (FirmAddressDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Address = modelDto.Address;
            PaymentMethods = modelDto.PaymentMethods;
            WorkingTime = modelDto.WorkingTime;
            ClosedForAscertainment = modelDto.ClosedForAscertainment;
            Firm = LookupField.FromReference(modelDto.FirmRef);
            Timestamp = modelDto.Timestamp;

            if (IsNew)
            {
                return;
            }

            MessageType = (MessageType)(modelDto.IsDeleted || modelDto.IsFirmDeleted
                                                 ? (int)MessageType.CriticalError
                                                 : !modelDto.IsActive || !modelDto.IsFirmActive || modelDto.ClosedForAscertainment
                                                   || modelDto.FirmClosedForAscertainment || !modelDto.IsLocatedOnTheMap
                                                       ? (int)MessageType.Warning
                                                       : (int)MessageType.None);

            Message = modelDto.IsDeleted || modelDto.IsFirmDeleted
                          ? BLResources.FirmAddressOrFirmIsDeletedAlertText
                          : !modelDto.IsActive || !modelDto.IsFirmActive
                                ? BLResources.FirmAddressOrFirmIsInactiveAlertText
                                : modelDto.ClosedForAscertainment || modelDto.FirmClosedForAscertainment
                                      ? BLResources.FirmAddressOrFirmIsClosedForAscertainmentAlertText
                                      : !modelDto.IsLocatedOnTheMap ? BLResources.FirmAddressIsNotLinkedWithMap : string.Empty;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            var dto = new FirmAddressDomainEntityDto
                {
                    Id = Id,
                    Address = Address,
                    PaymentMethods = PaymentMethods,
                    WorkingTime = WorkingTime,
                    ClosedForAscertainment = ClosedForAscertainment,
                    FirmRef = Firm.ToReference(),
                    Timestamp = Timestamp
                };

            return dto;
        }
    }
}