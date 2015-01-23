using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class FirmContactViewModel : EntityViewModelBase<FirmContact>
    {
        public FirmAddressContactType ContactType { get; set; }

        [DisplayNameLocalized("FirmContact")]
        public string Contact { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (FirmContactDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            ContactType = modelDto.ContactType;
            Contact = modelDto.Contact;
            Timestamp = modelDto.Timestamp;

            if (IsNew)
            {
                return;
            }

            MessageType = (DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType)(modelDto.IsFirmAddressDeleted || modelDto.IsFirmDeleted
                                            ? (int)DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType.CriticalError
                                            : !modelDto.IsFirmAddressActive || !modelDto.IsFirmActive ||
                                              modelDto.FirmAddressClosedForAscertainment || modelDto.FirmClosedForAscertainment
                                                  ? (int)DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType.Warning
                                                  : (int)DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType.None);

            Message = modelDto.IsFirmAddressDeleted || modelDto.IsFirmDeleted
                          ? BLResources.FirmContactOrFirmAddressOrFirmIsDeletedAlertText
                          : !modelDto.IsFirmAddressActive || !modelDto.IsFirmActive
                                ? BLResources.FirmContactOrFirmAddressOrFirmIsInactiveAlertText
                                : modelDto.FirmAddressClosedForAscertainment || modelDto.FirmClosedForAscertainment
                                      ? BLResources.FirmContactOrFirmAddressOrFirmIsClosedForAscertainmentAlertText
                                      : string.Empty;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            throw new NotImplementedException();
        }
    }
}