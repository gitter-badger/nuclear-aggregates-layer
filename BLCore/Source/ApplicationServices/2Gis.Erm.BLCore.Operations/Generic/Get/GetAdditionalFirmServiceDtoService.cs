using System.Linq;

using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetAdditionalFirmServiceDtoService : GetDomainEntityDtoServiceBase<AdditionalFirmService>
    {
        private readonly IFinder _finder;

        public GetAdditionalFirmServiceDtoService(IUserContext userContext, IFinder finder)
            : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<AdditionalFirmService> GetDto(long entityId)
        {
            return _finder.Find<AdditionalFirmService>(x => x.Id == entityId)
                          .Select(x => new AdditionalFirmServiceDomainEntityDto
                              {
                                  Id = x.Id,
                                  ServiceCode = x.ServiceCode,
                                  Description = x.Description,
                                  IsManaged = x.IsManaged,
                              })
                          .Single();
        }

        protected override IDomainEntityDto<AdditionalFirmService> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new AdditionalFirmServiceDomainEntityDto
                {
                    IsManaged = false
                };
        }
    }
}