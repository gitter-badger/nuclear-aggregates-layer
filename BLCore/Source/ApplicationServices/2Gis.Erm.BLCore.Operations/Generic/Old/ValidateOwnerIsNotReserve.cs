using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL.Specifications;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Old
{
    [Obsolete("Use IOwnerValidator")]
    public sealed class ValidateOwnerIsNotReserve<TEntity> : RequestHandler<ValidateOwnerIsNotReserveRequest<TEntity>, EmptyResponse>
        where TEntity : class,   IEntity, IEntityKey, ICuratedEntity
    {
        private readonly IFinder _finder;
        private readonly ISecurityServiceUserIdentifier _securityService;

        public ValidateOwnerIsNotReserve(IFinder finder, ISecurityServiceUserIdentifier securityService)
        {
            _securityService = securityService;
            _finder = finder;
        }

        protected override EmptyResponse Handle(ValidateOwnerIsNotReserveRequest<TEntity> request)
        {
            var entityOwnerCode = _finder.Find(Specs.Find.ById<TEntity>(request.Id)).Map(q => q.Select(x => (long?)x.OwnerCode)).One();

            if (!entityOwnerCode.HasValue)
            {
                throw new ArgumentException(string.Format(BLResources.EntityWithTypeAndIdIsNotFound, typeof(TEntity).Name, request.Id));
            }

            // check reserve user
            var reserveUser = _securityService.GetReserveUserIdentity();
            if (entityOwnerCode == reserveUser.Code)
            {
                throw new NotificationException(
                    string.Format(CultureInfo.CurrentCulture, BLResources.PleaseUseQualifyOperation, reserveUser.DisplayName));
            }

            return Response.Empty;
        }
    }
}