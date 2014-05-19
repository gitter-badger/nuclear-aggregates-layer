using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class ReleaseNotExistsOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IFinder _finder;
        private readonly IReleaseReadModel _releaseRepository;

        public ReleaseNotExistsOrderValidationRule(ISubRequestProcessor subRequestProcessor, IFinder finder, IReleaseReadModel releaseRepository)
        {
            _subRequestProcessor = subRequestProcessor;
            _finder = finder;
            _releaseRepository = releaseRepository;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            if (request.Type == ValidationType.SingleOrderOnRegistration)
            {
                if (request.OrderId == null)
                {
                    throw new ArgumentException("request.OrderId");
                }

                if (request.OrderId != null)
                {
                    var response = (CheckOrderReleasePeriodResponse)_subRequestProcessor.HandleSubRequest(
                        new CheckOrderReleasePeriodRequest
                            {
                                OrderId = request.OrderId.Value,
                                InProgressOnly = false,
                            },
                        null);
                    if (!response.Success)
                    {
                        if (request.CurrentOrderState != OrderState.Approved)
                        {
                            messages.Add(response.Message);                            
                        }
                    }
                }                
            }
            else
            {
                // we just need to check whether there exists a release for the speicified period/OrganizationUnit
                const short SuccessReleaseStatus = (short)ReleaseStatus.Success;

                var organizationUnitId = request.OrganizationUnitId ?? 0;

                var prevReleaseInfo = _releaseRepository.GetLastRelease(organizationUnitId, request.Period);

                if (prevReleaseInfo != null && !prevReleaseInfo.IsBeta && prevReleaseInfo.Status == SuccessReleaseStatus)
                {
                    var organizationUnit = _finder.Find<OrganizationUnit>(ou => ou.Id == request.OrganizationUnitId)
                        .Select(ou => new {ou.Name})
                        .Single();

                    messages.Add(new OrderValidationMessage
                        {
                            Type = MessageType.Error,
                            MessageText = string.Format(BLResources.OrdersCheckOrderHasReleaseInfo, request.Period.Start, request.Period.End, organizationUnit.Name),
                            OrderId = 0,
                            OrderNumber = string.Empty
                        });
                }
            }
        }
    }
}
