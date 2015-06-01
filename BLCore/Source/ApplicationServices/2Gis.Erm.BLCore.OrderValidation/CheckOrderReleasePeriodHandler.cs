using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    public sealed class CheckOrderReleasePeriodHandler : RequestHandler<CheckOrderReleasePeriodRequest, CheckOrderReleasePeriodResponse>
    {
        private readonly IQuery _query;

        public CheckOrderReleasePeriodHandler(IQuery query)
        {
            _query = query;
        }

        protected override CheckOrderReleasePeriodResponse Handle(CheckOrderReleasePeriodRequest request)
        {
            var result = new CheckOrderReleasePeriodResponse { Success = true };

            if (request.OrderId == default(int))
            {
                return result;
            }

            var orderInfo = _query.For(Specs.Find.ById<Order>(request.OrderId))
                .Select(x => new { x.Number, x.DestOrganizationUnitId, x.BeginDistributionDate, x.EndDistributionDateFact })
                .Single();

            Expression<Func<ReleaseInfo, bool>> expression =
                ri => ri.IsActive && !ri.IsDeleted && !ri.IsBeta &&
                      ri.OrganizationUnitId == orderInfo.DestOrganizationUnitId &&
                      orderInfo.BeginDistributionDate <= ri.PeriodStartDate && ri.PeriodEndDate <= orderInfo.EndDistributionDateFact;

            var isReleaseExist = request.InProgressOnly
                                     ? _query.For<ReleaseInfo>()
                                             .Where(expression)
                                             .Any(ri => ri.Status == ReleaseStatus.InProgressInternalProcessingStarted
                                                        || ri.Status == ReleaseStatus.InProgressWaitingExternalProcessing)
                                     : _query.For<ReleaseInfo>()
                                             .Where(expression)
                                             .Any(ri => ri.Status == ReleaseStatus.InProgressInternalProcessingStarted
                                                        || ri.Status == ReleaseStatus.InProgressWaitingExternalProcessing
                                                        || ri.Status == ReleaseStatus.Success);

            if (isReleaseExist)
            {
                result.Success = false;
                result.Message = new OrderValidationMessage
                {
                    OrderId = request.OrderId,
                    OrderNumber = orderInfo.Number,
                    MessageText = BLResources.OrderCheckHasReleases,
                    Type = MessageType.Error
                };
            }

            return result;
        }
    }
}