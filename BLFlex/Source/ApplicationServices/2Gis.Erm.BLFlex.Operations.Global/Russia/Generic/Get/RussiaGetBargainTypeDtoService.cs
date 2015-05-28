using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Get
{
    public class RussiaGetBargainTypeDtoService : GetDomainEntityDtoServiceBase<BargainType>, IRussiaAdapted
    {
        private readonly ISecureFinder _finder;

        public RussiaGetBargainTypeDtoService(IUserContext userContext, ISecureFinder finder)
            : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<BargainType> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new BargainTypeDomainEntityDto();
        }

        protected override IDomainEntityDto<BargainType> GetDto(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<BargainType>(entityId)).One();

            return BargainTypeFlexSpecs.BargainTypes.Russia.Project.DomainEntityDto().Map(entity);
        }
    }
}