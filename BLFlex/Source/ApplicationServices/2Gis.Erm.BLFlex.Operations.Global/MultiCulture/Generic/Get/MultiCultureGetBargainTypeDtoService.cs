using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.MultiCulture;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get
{
    public class MultiCultureGetBargainTypeDtoService : GetDomainEntityDtoServiceBase<BargainType>,
                                                        IChileAdapted,
                                                        ICyprusAdapted,
                                                        ICzechAdapted,
                                                        IUkraineAdapted,
                                                        IKazakhstanAdapted
    {
        private readonly ISecureFinder _finder;

        public MultiCultureGetBargainTypeDtoService(IUserContext userContext, ISecureFinder finder)
            : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<BargainType> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new MultiCultureBargainTypeDomainEntityDto();
        }

        protected override IDomainEntityDto<BargainType> GetDto(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<BargainType>(entityId)).One();

            return BargainTypeFlexSpecs.BargainTypes.MultiCulture.Project.DomainEntityDto().Map(entity);
        }
    }
}