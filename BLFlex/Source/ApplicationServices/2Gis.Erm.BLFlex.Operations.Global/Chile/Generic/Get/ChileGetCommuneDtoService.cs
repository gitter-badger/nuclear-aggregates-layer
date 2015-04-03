using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Get
{
    public sealed class ChileGetCommuneDtoService : GetDomainEntityDtoServiceBase<Commune>, IChileAdapted
    {
        public ChileGetCommuneDtoService(IUserContext userContext)
            : base(userContext)
        {
        }

        protected override IDomainEntityDto<Commune> GetDto(long entityId)
        {
            throw new NotificationException(BLResources.ChileCommuneModificationNotSupported);
        }

        protected override IDomainEntityDto<Commune> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            throw new NotificationException(BLResources.ChileCreationCommuneNotSupported);
        }
    }
}