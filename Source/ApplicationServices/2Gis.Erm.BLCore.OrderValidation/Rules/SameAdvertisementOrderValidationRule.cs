using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    // Проверка неактуальна - AdditionalAdvertisementsOrderValidationRule ее включает. 
    public sealed class SameAdvertisementOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        private class Item
        {
            public int BeginRelease { get; set; }
            public int EndRelease { get; set; }
            public bool IsBanner { get; set; }
            public bool AllowesDifferentImages { get; set; }
            public long PositionId { get; set; }
            public long OrderPositionId { get; set; }
            public long OrderId { get; set; }
            public string OrderNumber { get; set; }
            public long? AdvertisementId { get; set; }
            public string PositionName { get; set; }
        }

        private const int DifferentImagesForSingleBannerId = 7;
        private const int DifferentImagesForDoubleBannerId = 8;
        private const int DifferentImagesForTripleBannerId = 9;
        private const int SingleBannerId = 1;
        private const int DoubleBannerId = 2;
        private const int TripleBannerId = 3;
        private const int AdditionalSingleBannerId = 4;
        private const int AdditionalDoubleBannerId = 5;
        private const int AdditionalTripleBannerId = 6;

        public SameAdvertisementOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            // 1. Define begin/end releases
            int beginRelease = 0;
            int endRelease = 0;
            bool disregardReleaseNumbers = true;

            if (request.Type == ValidationType.SingleOrderOnRegistration)
            {
                var principalOrder = _finder.Find<Order>(order => order.Id == request.OrderId)
                    .Select(order => new
                        {
                            order.BeginReleaseNumber,
                            order.EndReleaseNumberFact,
                            order.FirmId,
                            order.Number,
                            order.Id
                        })
                    .Single();

                beginRelease = principalOrder.BeginReleaseNumber;
                endRelease = principalOrder.EndReleaseNumberFact;
                disregardReleaseNumbers = false;

                filterPredicate = order => order.IsActive
                                           && !order.IsDeleted
                                           && order.FirmId == principalOrder.FirmId
                                           && (order.Id == principalOrder.Id
                                               || order.WorkflowStepId == (int)OrderState.OnApproval
                                               || order.WorkflowStepId == (int)OrderState.Approved
                                               || order.WorkflowStepId == (int)OrderState.OnTermination)
                                           && (order.BeginReleaseNumber <= beginRelease && beginRelease <= order.EndReleaseNumberFact
                                               || order.BeginReleaseNumber <= endRelease && endRelease <= order.EndReleaseNumberFact
                                               || beginRelease <= order.BeginReleaseNumber && order.BeginReleaseNumber <= endRelease
                                               || beginRelease <= order.EndReleaseNumberFact && order.EndReleaseNumberFact <= endRelease);
            }

            var firms = _finder.Find(filterPredicate)
                .GroupBy(order => order.FirmId)
                .Select(group => new
                                     {
                                         FirmId = group.FirstOrDefault().Firm.Id,
                                         FirmName = group.FirstOrDefault().Firm.Name,
                                         Opas = group.SelectMany(order => order.OrderPositions.Where(orderPosition => orderPosition.IsActive && !orderPosition.IsDeleted)
                                                                                            .SelectMany(orderPosition => orderPosition.OrderPositionAdvertisements)
                                                                                            .Select(opa => new
                                                                                                               {
                                                                                                                   opa.PositionId,
                                                                                                                   PositionName = opa.Position.Name,
                                                                                                                   opa.OrderPositionId,
                                                                                                                   AllowesDifferentImages = opa.PositionId == DifferentImagesForSingleBannerId
                                                                                                                                || opa.PositionId == DifferentImagesForDoubleBannerId
                                                                                                                                || opa.PositionId == DifferentImagesForTripleBannerId, 
                                                                                                                   IsBanner = opa.Advertisement != null 
                                                                                                                                && (opa.PositionId == SingleBannerId 
                                                                                                                                        || opa.PositionId == DoubleBannerId 
                                                                                                                                        || opa.PositionId == TripleBannerId 
                                                                                                                                        || opa.PositionId == AdditionalSingleBannerId 
                                                                                                                                        || opa.PositionId == AdditionalDoubleBannerId 
                                                                                                                                        || opa.PositionId == AdditionalTripleBannerId), opa.AdvertisementId,
                                                                                                                    OrderNumber = order.Number,
                                                                                                                    OrderId = order.Id
                                                                                                               })
                                                                                            .Where(opa => opa.IsBanner || opa.AllowesDifferentImages)
                                                                                            .Select(opa => new Item
                                                                                                               {
                                                                                                                   BeginRelease = order.BeginReleaseNumber,
                                                                                                                   EndRelease = order.EndReleaseNumberFact,
                                                                                                                   IsBanner = opa.IsBanner,
                                                                                                                   AllowesDifferentImages = opa.AllowesDifferentImages,
                                                                                                                   PositionId = opa.PositionId,
                                                                                                                   OrderPositionId = opa.OrderPositionId,
                                                                                                                   AdvertisementId = opa.AdvertisementId,
                                                                                                                   OrderId = opa.OrderId,
                                                                                                                   OrderNumber = opa.OrderNumber,
                                                                                                                   PositionName = opa.PositionName
                                                                                                               }))
                                     }).ToArray();

            var allowingPositions = GetAllowingPositions();

            foreach (var firm in firms)
            {
                // Collect all releases that can have different advertisements: [release, type of banner(0=singular,1=double,2=triple)]
                var canSkip = new bool[endRelease - beginRelease + 1, 3]; 
                
                foreach (var opa in firm.Opas.Where(opa => opa.AllowesDifferentImages))
                {
                    int index = -1;
                    switch (opa.PositionId)
                    {
                        case DifferentImagesForSingleBannerId:
                            index = 0;
                            break;
                        case DifferentImagesForDoubleBannerId:
                            index = 1;
                            break;
                        case DifferentImagesForTripleBannerId:
                            index = 2;
                            break;
                    }

                    int lowerBound = 0;
                    int upperBound = 0;

                    if (!disregardReleaseNumbers)
                    {
                        lowerBound = Math.Max(opa.BeginRelease, beginRelease);
                        upperBound = Math.Min(opa.EndRelease, endRelease);
                    }

                    for (int release = lowerBound; release <= upperBound; release++)
                    {
                        canSkip[release - beginRelease, index] = true;
                    }
                }

                var advertisements = new Dictionary<Tuple<int, int>, Item>(); // ReleaseNumber, Advertisement type=>Item 
                var foundConflicts = new HashSet<Tuple<long, long, int>>(); // OrderPositionId conflicts with OrderPositionId at specific advertisement type

                foreach (Item opa in firm.Opas.Where(opa => opa.IsBanner))
                {
                    int type = -1;
                    switch (opa.PositionId)
                    {
                        case SingleBannerId:
                        case AdditionalSingleBannerId:
                            type = 0;
                            break;
                        case DoubleBannerId:
                        case AdditionalDoubleBannerId:
                            type = 1;
                            break;
                        case TripleBannerId:
                        case AdditionalTripleBannerId:
                            type = 2;
                            break;
                    }

                    int lowerBound = 0;
                    int upperBound = 0;

                    if (!disregardReleaseNumbers)
                    {
                        lowerBound = Math.Max(opa.BeginRelease, beginRelease);
                        upperBound = Math.Min(opa.EndRelease, endRelease);
                    }

                    for (int release = lowerBound; release <= upperBound; release++)
                    {
                        if (canSkip[release - lowerBound, type])
                        {
                            continue; // Different advertisements of the current type are allowed for this release
                        }

                        var key = Tuple.Create(release, type);

                        if (advertisements.ContainsKey(key))
                        {
                            Item anotherOrderPosition = advertisements[key];
                            if (anotherOrderPosition.AdvertisementId != opa.AdvertisementId)
                            {   // error found
                                // If the error lasts for more than 1 release, we report it only once
                                var errorKey = Tuple.Create(Math.Min(opa.OrderPositionId, anotherOrderPosition.OrderPositionId), Math.Max(opa.OrderPositionId, anotherOrderPosition.OrderPositionId), type);
                                if (foundConflicts.Contains(errorKey))
                                {
                                    continue;
                                }

                                foundConflicts.Add(errorKey);
                                if (IsCheckMassive || request.OrderId == opa.OrderId || request.OrderId == anotherOrderPosition.OrderId)
                                {
                                    var message = string.Format(BLResources.OrderCheckThereAreBannersWithDifferentAdvertisements,
                                                                  GenerateDescription(EntityName.Firm, firm.FirmName, firm.FirmId),
                                                                  GenerateDescription(EntityName.OrderPosition, opa.PositionName, opa.OrderPositionId),
                                                                  GenerateDescription(EntityName.Order, opa.OrderNumber, opa.OrderId),
                                                                  GenerateDescription(EntityName.OrderPosition, anotherOrderPosition.PositionName, anotherOrderPosition.OrderPositionId),
                                                                  GenerateDescription(EntityName.Order, anotherOrderPosition.OrderNumber, anotherOrderPosition.OrderId),
                                                                  GenerateDescription(EntityName.Position, allowingPositions[type].Value, allowingPositions[type].Key));
                                    messages.Add(new OrderValidationMessage
                                    {
                                        Type = MessageType.Error,
                                        MessageText = message,
                                        OrderId = opa.OrderId,
                                        OrderNumber = opa.OrderNumber,
                                    });   
                                }
                            }
                        }
                        else
                        {
                            advertisements[key] = opa;
                        }
                    }
                }
            }
        }

        private KeyValuePair<long, string>[] GetAllowingPositions()
        {
            var allowingPositions = _finder.Find<Position>(item => item.Id == DifferentImagesForSingleBannerId || item.Id == DifferentImagesForDoubleBannerId || item.Id == DifferentImagesForTripleBannerId).Select(item => new { item.Id, item.Name }).ToArray();
            Func<int, KeyValuePair<long, string>> getName = id => allowingPositions.Where(item => item.Id == id).Select(item => new KeyValuePair<long, string>(item.Id, item.Name)).Single();
            var allowinfPositionNames = new[] { getName(DifferentImagesForSingleBannerId), getName(DifferentImagesForDoubleBannerId), getName(DifferentImagesForTripleBannerId) };
            return allowinfPositionNames;
        }
    }
}
