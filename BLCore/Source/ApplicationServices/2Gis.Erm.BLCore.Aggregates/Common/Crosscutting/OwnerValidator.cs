using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting
{
    public sealed class OwnerValidator : IOwnerValidator
    {
        private readonly IFinder _finder;
        private readonly ISecurityServiceUserIdentifier _securityService;

        public OwnerValidator(IFinder finder, ISecurityServiceUserIdentifier securityService)
        {
            _finder = finder;
            _securityService = securityService;
        }

        public void CheckIsNotReserve<TEntity>(long entityId) where TEntity : class, IEntityKey, ICuratedEntity, IEntity
        {
            var ownerCode = _finder.Find(Specs.Select.Owner<TEntity>(), Specs.Find.ById<TEntity>(entityId)).Single();

            var reserveUser = _securityService.GetReserveUserIdentity();
            if (ownerCode == reserveUser.Code)
            {
                throw new NotificationException(string.Format(BLResources.PleaseUseQualifyOperation, reserveUser.DisplayName));
            }
        }
    }
}