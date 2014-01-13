using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class RegionalApiAdvertisementsOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private const long ApiPlatformId = 3;
        private readonly IFinder _finder;

        public RegionalApiAdvertisementsOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request,
                                                 Expression<Func<Order, bool>> filterPredicate,
                                                 IList<OrderValidationMessage> messages)
        {
            var ordersWithErrors = _finder.Find(filterPredicate)
                                          .Where(x => x.Firm.IsActive && !x.Firm.IsDeleted)
                                          .Select(x => new
                                              {
                                                  ApiPositions = x.OrderPositions.Where(y =>
                                                                                        y.IsActive && !y.IsDeleted &&
                                                                                        y.OrderPositionAdvertisements.Any(
                                                                                            z => z.Position.PlatformId == ApiPlatformId && z.Position.AdvertisementTemplateId != null))
                                                                                      .Select(y => new { y.PricePosition.Position.Name, y.Id }),
                                                  x.Number,
                                                  OrderId = x.Id,
                                                  OrderProjectIds = x.DestOrganizationUnit.Projects.Select(y => y.Id),
                                                  AddressProjectIds =
                                                           x.Firm.FirmAddresses.Where(y => y.IsActive && !y.IsDeleted && !y.ClosedForAscertainment && y.BuildingCode != null && !y.Building.IsDeleted)
                                                                                          .SelectMany(y => y.Building.Territory.OrganizationUnit.Projects.Where(z => z.IsActive).Select(z => z.Id))
                                              })
                                          .Where(x => x.ApiPositions.Any())
                                          .ToArray()
                                          .Where(x => !x.OrderProjectIds.Any(y => x.AddressProjectIds.Any(z => z == y)));

            foreach (var order in ordersWithErrors)
            {
                foreach (var apiPosition in order.ApiPositions)
                {
                    var positionLink = GenerateDescription(EntityName.OrderPosition, apiPosition.Name, apiPosition.Id);

                    messages.Add(new OrderValidationMessage
                        {
                            Type = request.Type == ValidationType.SingleOrderOnRegistration ? MessageType.Warning : MessageType.Error,
                            OrderId = order.OrderId,
                            OrderNumber = order.Number,
                            MessageText = string.Format(BLResources.RegionalApiError, positionLink)
                        });
                }
            }
        }
    }
}
