using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class ReleaseNotExistsOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IReleaseReadModel _releaseReadModel;

        public ReleaseNotExistsOrderValidationRule(ISubRequestProcessor subRequestProcessor, IReleaseReadModel releaseReadModel)
        {
            _subRequestProcessor = subRequestProcessor;
            _releaseReadModel = releaseReadModel;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            if (!ruleContext.ValidationParams.IsMassValidation)
            {
                var response = 
                    (CheckOrderReleasePeriodResponse)_subRequestProcessor.HandleSubRequest(
                        new CheckOrderReleasePeriodRequest
                                {
                                    OrderId = ruleContext.ValidationParams.Single.OrderId,
                                    InProgressOnly = false,
                                },
                            null);
                    if (!response.Success)
                    {
                        if (ruleContext.ValidationParams.Single.CurrentOrderState != OrderState.Approved)
                        {
                            return new[] { response.Message };                         
                        }
                    }
            }
            else
            {
                // we just need to check whether there exists a release for the speicified period/OrganizationUnit
                const short SuccessReleaseStatus = (short)ReleaseStatus.Success;

                var previousReleaseInfo = _releaseReadModel.GetLastRelease(ruleContext.ValidationParams.Mass.OrganizationUnitId, ruleContext.ValidationParams.Mass.Period);
                if (previousReleaseInfo != null && !previousReleaseInfo.IsBeta && previousReleaseInfo.Status == SuccessReleaseStatus)
                {
                    var organizationUnitName = _releaseReadModel.GetOrganizationUnitName(ruleContext.ValidationParams.Mass.OrganizationUnitId);
                    return new[]
                               {
                                   new OrderValidationMessage
                                       {
                                           Type = MessageType.Error,
                                           MessageText =
                                               string.Format(BLResources.OrdersCheckOrderHasReleaseInfo,
                                                             ruleContext.ValidationParams.Mass.Period.Start,
                                                             ruleContext.ValidationParams.Mass.Period.End,
                                                             organizationUnitName),
                                           OrderId = 0,
                                           OrderNumber = string.Empty
                                       }
                               };
                }
            }

            return Enumerable.Empty<OrderValidationMessage>();
        }
    }
}
