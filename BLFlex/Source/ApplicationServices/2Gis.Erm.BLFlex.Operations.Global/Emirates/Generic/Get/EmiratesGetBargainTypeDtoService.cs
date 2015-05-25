using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Get
{
    public class EmiratesGetBargainTypeDtoService : GetDomainEntityDtoServiceBase<BargainType>, IEmiratesAdapted
    {
        private readonly ISecureFinder _finder;

        public EmiratesGetBargainTypeDtoService(IUserContext userContext, ISecureFinder finder)
            : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<BargainType> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new EmiratesBargainTypeDomainEntityDto();
        }

        protected override IDomainEntityDto<BargainType> GetDto(long entityId)
        {
            var entity = _finder.FindOne(Specs.Find.ById<BargainType>(entityId));

            return BargainTypeFlexSpecs.BargainTypes.Emirates.Project.DomainEntityDto().Project(entity);
        }
    }
}