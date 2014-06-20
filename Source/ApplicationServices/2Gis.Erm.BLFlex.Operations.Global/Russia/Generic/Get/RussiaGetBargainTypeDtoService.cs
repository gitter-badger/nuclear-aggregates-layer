using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Get
{
    public class RussiaGetBargainTypeDtoService : GetDomainEntityDtoServiceBase<BargainType>, IRussiaAdapted
    {
        private readonly ISecureFinder _finder;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public RussiaGetBargainTypeDtoService(IUserContext userContext, ISecureFinder finder, IAPIIdentityServiceSettings identityServiceSettings)
            : base(userContext)
        {
            _finder = finder;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<BargainType> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new BargainTypeDomainEntityDto
                {
                    IdentityServiceUrl = _identityServiceSettings.RestUrl
                };
        }

        protected override IDomainEntityDto<BargainType> GetDto(long entityId)
        {
            var entity = _finder.FindOne(Specs.Find.ById<BargainType>(entityId));

            return BargainTypeFlexSpecs.BargainTypes.Russia.Project.DomainEntityDto().Project(entity);
        }
    }
}