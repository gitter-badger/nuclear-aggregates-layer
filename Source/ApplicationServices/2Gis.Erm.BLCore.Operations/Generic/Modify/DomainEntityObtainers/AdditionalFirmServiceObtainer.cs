using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class AdditionalFirmServiceObtainer : ISimplifiedModelEntityObtainer<AdditionalFirmService>
    {
        private readonly IFinder _finder;

        public AdditionalFirmServiceObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public AdditionalFirmService ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AdditionalFirmServiceDomainEntityDto)domainEntityDto;

            var entity = dto.Id == 0
                             ? new AdditionalFirmService()
                             : _finder.Find(Specs.Find.ById<AdditionalFirmService>(dto.Id)).Single();

            entity.IsManaged = dto.IsManaged;
            entity.ServiceCode = dto.ServiceCode;
            entity.Description = dto.Description;

            return entity;
        }
    }
}