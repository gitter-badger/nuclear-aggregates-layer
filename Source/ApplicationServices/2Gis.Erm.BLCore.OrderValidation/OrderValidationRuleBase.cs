﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    public abstract class OrderValidationRuleBase : IOrderValidationRule
    {
        protected bool IsCheckMassive { get; private set; }

        IReadOnlyList<OrderValidationMessage> IOrderValidationRule.Validate(ValidateOrdersRequest request, OrderValidationPredicate filterPredicate)
        {
            IsCheckMassive = request.Type == ValidationType.PreReleaseBeta ||
                             request.Type == ValidationType.PreReleaseFinal ||
                             request.Type == ValidationType.ManualReport ||
                             request.Type == ValidationType.ManualReportWithAccountsCheck;

            var orderValidationMessages = new List<OrderValidationMessage>();
            Validate(request, filterPredicate, orderValidationMessages);
            return orderValidationMessages;
        }

        protected abstract void Validate(ValidateOrdersRequest request, OrderValidationPredicate filterPredicate, IList<OrderValidationMessage> messages);

        protected string GenerateDescription(EntityName entityName, string description, long entityId)
        {
            return IsCheckMassive ? description : string.Format("<{0}:{1}:{2}>", entityName, description, entityId);
        }

        protected Expression<Func<Order, bool>> GetFilterPredicateToGetLinkedOrders(IFinder finder, long orderId, out long organizationUnitId, out long? firmId)
        {
            if (orderId == 0)
            {
                throw new ArgumentNullException("orderId");
            }

            var orderInfo = finder.Find(Specs.Find.ById<Order>(orderId))
                                   .Select(item => new
                                                       {
                                                           item.BeginReleaseNumber,
                                                           item.DestOrganizationUnitId,
                                                           EndReleaseNumber = item.EndReleaseNumberFact,
                                                           item.FirmId
                                      })
                                  .Single();

            organizationUnitId = orderInfo.DestOrganizationUnitId;
            firmId = orderInfo.FirmId;

            return order => order.IsActive && !order.IsDeleted &&
                            order.DestOrganizationUnitId == orderInfo.DestOrganizationUnitId &&
                            (order.Id == orderId ||
                             order.WorkflowStepId == (int)OrderState.OnApproval ||
                             order.WorkflowStepId == (int)OrderState.Approved ||
                             order.WorkflowStepId == (int)OrderState.OnTermination) &&
                            ((order.BeginReleaseNumber >= orderInfo.BeginReleaseNumber && order.BeginReleaseNumber <= orderInfo.EndReleaseNumber) ||
                             (order.EndReleaseNumberFact >= orderInfo.BeginReleaseNumber && order.EndReleaseNumberFact <= orderInfo.EndReleaseNumber) ||
                             (orderInfo.BeginReleaseNumber >= order.BeginReleaseNumber && orderInfo.BeginReleaseNumber <= order.EndReleaseNumberFact) ||
                             (orderInfo.EndReleaseNumber >= order.BeginReleaseNumber && orderInfo.EndReleaseNumber <= order.EndReleaseNumberFact));
        }
    }
}
