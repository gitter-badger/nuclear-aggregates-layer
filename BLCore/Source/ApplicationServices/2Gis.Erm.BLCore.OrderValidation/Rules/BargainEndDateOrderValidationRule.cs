﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class BargainEndDateOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IQuery _query;

        public BargainEndDateOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            var badOrders = _query.For<Order>()
                                  .Where(ruleContext.OrdersFilterPredicate)
                                  .Select(x => new
                                      {
                                          Order = x,
                                          ProfileNames = x.LegalPerson.LegalPersonProfiles
                                                          .Where(y => y.IsActive && !y.IsDeleted &&
                                                                      y.OperatesOnTheBasisInGenitive != null &&
                                                                      y.OperatesOnTheBasisInGenitive == OperatesOnTheBasisType.Bargain &&
                                                                      y.BargainEndDate != null &&
                                                                      y.BargainEndDate < x.SignupDate)
                                                          .Select(y => y.Name)
                                      })
                                  .Where(x => x.ProfileNames.Any())
                                  .ToArray();

            return from orderInfo in badOrders
                   from profileName in orderInfo.ProfileNames
                   select new OrderValidationMessage
                                {
                                    Type = MessageType.Info, 
                                    OrderId = orderInfo.Order.Id, 
                                    OrderNumber = orderInfo.Order.Number, 
                                    MessageText = string.Format(BLResources.ProfileBargainEndDateIsLessThanSignDate, profileName)
                                };
        }
    }
}
