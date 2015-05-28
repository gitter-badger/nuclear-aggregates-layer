using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Get
{
    public class EmiratesGetFirmAddressDtoService : GetDomainEntityDtoServiceBase<FirmAddress>, IEmiratesAdapted
    {
        private readonly IFirmReadModel _firmReadModel;
        private readonly IUserContext _userContext;

        public EmiratesGetFirmAddressDtoService(IUserContext userContext, IFirmReadModel firmReadModel)
            : base(userContext)
        {
            _userContext = userContext;
            _firmReadModel = firmReadModel;
        }

        protected override IDomainEntityDto<FirmAddress> GetDto(long entityId)
        {
            var address = _firmReadModel.GetFirmAddress(entityId);

            var firmAddressDto = FirmFlexSpecs.FirmAddresses.Emirates.Project.DomainEntityDto().Map(address);

            var firm = _firmReadModel.GetFirm(firmAddressDto.FirmRef.Id.Value);

            firmAddressDto.WorkingTime = FirmWorkingTimeLocalizer.LocalizeWorkingTime(address.WorkingTime, _userContext.Profile.UserLocaleInfo.UserCultureInfo);

            firmAddressDto.IsFirmActive = firm.IsActive;
            firmAddressDto.IsFirmDeleted = firm.IsDeleted;
            firmAddressDto.FirmClosedForAscertainment = firm.ClosedForAscertainment;
            firmAddressDto.FirmRef.Name = firm.Name;

            return firmAddressDto;
        }

        protected override IDomainEntityDto<FirmAddress> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new EmiratesFirmAddressDomainEntityDto();
        }
    }
}