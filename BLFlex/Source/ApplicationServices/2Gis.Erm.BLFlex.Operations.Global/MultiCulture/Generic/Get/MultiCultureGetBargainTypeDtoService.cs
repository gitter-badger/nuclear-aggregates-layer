using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.MultiCulture;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get
{
    public class MultiCultureGetBargainTypeDtoService : GetDomainEntityDtoServiceBase<BargainType>, IChileAdapted, ICyprusAdapted, ICzechAdapted,
                                                        IUkraineAdapted, IKazakhstanAdapted
    {
        private readonly ISecureFinder _finder;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public MultiCultureGetBargainTypeDtoService(IUserContext userContext, ISecureFinder finder, IAPIIdentityServiceSettings identityServiceSettings)
            : base(userContext)
        {
            _finder = finder;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<BargainType> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new MultiCultureBargainTypeDomainEntityDto
                {
                    IdentityServiceUrl = _identityServiceSettings.RestUrl
                };
        }

        protected override IDomainEntityDto<BargainType> GetDto(long entityId)
        {
            var entity = _finder.FindOne(Specs.Find.ById<BargainType>(entityId));

            return BargainTypeFlexSpecs.BargainTypes.MultiCulture.Project.DomainEntityDto().Project(entity);
        }
    }
}