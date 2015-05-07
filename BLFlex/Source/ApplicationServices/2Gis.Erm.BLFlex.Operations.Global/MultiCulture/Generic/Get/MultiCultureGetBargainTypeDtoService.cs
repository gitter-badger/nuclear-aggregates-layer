using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.MultiCulture;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

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

        protected override IDomainEntityDto<BargainType> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new MultiCultureBargainTypeDomainEntityDto();
        }

        protected override IDomainEntityDto<BargainType> GetDto(long entityId)
        {
            var entity = _finder.FindOne(Specs.Find.ById<BargainType>(entityId));

            return BargainTypeFlexSpecs.BargainTypes.MultiCulture.Project.DomainEntityDto().Project(entity);
        }
    }
}