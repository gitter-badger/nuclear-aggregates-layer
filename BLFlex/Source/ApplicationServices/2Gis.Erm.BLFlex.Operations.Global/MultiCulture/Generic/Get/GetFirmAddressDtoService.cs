using System.Linq;

using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get
{
    public class GetFirmAddressDtoService : GetDomainEntityDtoServiceBase<FirmAddress>, IRussiaAdapted, ICyprusAdapted, ICzechAdapted, IChileAdapted, IUkraineAdapted, IKazakhstanAdapted
    {
        private readonly ISecureFinder _finder;
        private readonly IUserContext _userContext;

        public GetFirmAddressDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
            _userContext = userContext;
        }

        protected override IDomainEntityDto<FirmAddress> GetDto(long entityId)
        {
            // TODO {y.baranihin, 30.04.2014}:  адаптировать после вливания рефакторинга из Украины
            var firmAddressDto = _finder.Find(new FindSpecification<FirmAddress>(x => x.Id == entityId))
                          .Select(entity => new FirmAddressDomainEntityDto
                              {
                                  IsFirmActive = entity.Firm.IsActive,
                                  IsFirmDeleted = entity.Firm.IsDeleted,
                                  FirmClosedForAscertainment = entity.Firm.ClosedForAscertainment,
                                  Id = entity.Id,
                                  Address = entity.Address + ((entity.ReferencePoint == null) ? string.Empty : " — " + entity.ReferencePoint),
                                  PaymentMethods = entity.PaymentMethods,
                                  WorkingTime = entity.WorkingTime,
                                  ClosedForAscertainment = entity.ClosedForAscertainment,
                                  IsLocatedOnTheMap = entity.IsLocatedOnTheMap,
                                  FirmRef = new EntityReference { Id = entity.FirmId, Name = entity.Firm.Name },
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                  CreatedOn = entity.CreatedOn,
                                  IsActive = entity.IsActive,
                                  IsDeleted = entity.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                  ModifiedOn = entity.ModifiedOn,
                                  Timestamp = entity.Timestamp
                              })
                          .Single();

            firmAddressDto.WorkingTime = FirmWorkingTimeLocalizer.LocalizeWorkingTime(firmAddressDto.WorkingTime, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
            return firmAddressDto;
        }

        protected override IDomainEntityDto<FirmAddress> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new FirmAddressDomainEntityDto();
        }
    }
}