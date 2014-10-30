using System;
using System.Collections.Generic;
using System.Globalization;
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
    public sealed class LinkingObjectsOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public LinkingObjectsOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            CheckForAddressFails(filterPredicate, messages);
            CheckForCategoryFails(filterPredicate, messages);
            CheckForAddressCategoryFails(filterPredicate, messages);
        }

        private void CheckForAddressFails(Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            var orderInfos = _finder.Find(filterPredicate)
                .Select(order => new
                    {
                        OrderId = order.Id,
                        OrderNumber = order.Number,

                        AddressFails = order.OrderPositions
                            .Where(orderPosition => orderPosition.IsActive && !orderPosition.IsDeleted)
                            .SelectMany(orderPosition => orderPosition.OrderPositionAdvertisements)
                            .Where(positionAdvertisement => positionAdvertisement.FirmAddressId != null)
                            .Select(positionAdvertisement => new
                                {
                                    OrderPositionId = positionAdvertisement.OrderPositionId,
                                    PositionName = positionAdvertisement.Position.Name,

                                    AddressId = positionAdvertisement.FirmAddressId.Value,
                                    AddressName = positionAdvertisement.FirmAddress.Address + ((positionAdvertisement.FirmAddress.ReferencePoint == null) ? string.Empty : " — " + positionAdvertisement.FirmAddress.ReferencePoint),

                                    // true means fail
                                    AddressDeleted = positionAdvertisement.FirmAddress.IsDeleted,
                                    AddressHidden = positionAdvertisement.FirmAddress.ClosedForAscertainment,
                                    AddressNotActive = !positionAdvertisement.FirmAddress.IsActive || positionAdvertisement.FirmAddress.IsDeleted,
                                    AddressNotBelongsToFirm = order.Firm.FirmAddresses.All(p => p.Id != positionAdvertisement.FirmAddressId),
                                })
                            .Where(fail => fail.AddressDeleted || fail.AddressHidden || fail.AddressNotActive || fail.AddressNotBelongsToFirm)
                    })
                .Where(order => order.AddressFails.Any())
                .ToArray();

            foreach (var orderInfo in orderInfos)
            {
                // address fails
                foreach (var addressFail in orderInfo.AddressFails)
                {
                    var orderPositionDescription = GenerateDescription(EntityName.OrderPosition, addressFail.PositionName, addressFail.OrderPositionId);
                    var addressDescription = GenerateDescription(EntityName.FirmAddress, addressFail.AddressName, addressFail.AddressId);

                    if (addressFail.AddressDeleted)
                    {
                        messages.Add(new OrderValidationMessage
                            {
                                Type = MessageType.Error,
                                OrderId = orderInfo.OrderId,
                                OrderNumber = orderInfo.OrderNumber,
                                MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrderPositionAddressDeleted, orderPositionDescription, addressDescription),
                            });
                    }

                    if (addressFail.AddressHidden)
                    {
                        messages.Add(new OrderValidationMessage
                            {
                                Type = MessageType.Error,
                                OrderId = orderInfo.OrderId,
                                OrderNumber = orderInfo.OrderNumber,
                                MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrderPositionAddressHidden, orderPositionDescription, addressDescription),
                            });
                    }

                    if (addressFail.AddressNotActive)
                    {
                        messages.Add(new OrderValidationMessage
                            {
                                Type = MessageType.Error,
                                OrderId = orderInfo.OrderId,
                                OrderNumber = orderInfo.OrderNumber,
                                MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrderPositionAddressNotActive, orderPositionDescription, addressDescription),
                            });
                    }

                    if (addressFail.AddressNotBelongsToFirm)
                    {
                        messages.Add(new OrderValidationMessage
                            {
                                Type = MessageType.Error,
                                OrderId = orderInfo.OrderId,
                                OrderNumber = orderInfo.OrderNumber,
                                MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrderPositionAddressNotBelongsToFirm, orderPositionDescription, addressDescription),
                            });
                    }
                }
            }
        }

        private void CheckForCategoryFails(Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            var categoryFails = _finder.Find(filterPredicate)
                .Select(x => new
                    {
                        OrderId = x.Id,
                        OrderNumber = x.Number,

                        CategoryFails = x.OrderPositions
                            .Where(orderPosition => orderPosition.IsActive && !orderPosition.IsDeleted)
                            .SelectMany(orderPosition => orderPosition.OrderPositionAdvertisements)
                            .Where(positionAdvertisement => positionAdvertisement.CategoryId != null)
                            .Select(positionAdvertisement => new
                                {
                                    OrderPositionId = positionAdvertisement.OrderPositionId,
                                    PositionName = positionAdvertisement.Position.Name,

                                    BindingObjectType = positionAdvertisement.Position.BindingObjectTypeEnum,
                                    CategoryId = positionAdvertisement.CategoryId.Value,
                                    CategoryName = positionAdvertisement.Category.Name,

                                    // true means fail
                                    CategoryNotActive = !(positionAdvertisement.Category.IsActive && !positionAdvertisement.Category.IsDeleted),
                                    CategoryNotBelongsToFirm = x.Firm.FirmAddresses
                                                                .Where(p => p.IsActive && !p.IsDeleted && !p.ClosedForAscertainment)
                                                                .SelectMany(p => p.CategoryFirmAddresses)
                                                                .Where(p => p.IsActive && !p.IsDeleted)
                                                                .All(p => p.CategoryId != positionAdvertisement.CategoryId && p.Category.ParentCategory.ParentCategory.Id != positionAdvertisement.CategoryId),
                                })
                            .Where(fail => fail.CategoryNotActive || fail.CategoryNotBelongsToFirm)
                    })
                .Where(order => order.CategoryFails.Any())
                .ToArray();

            foreach (var orderInfo in categoryFails)
            {
                // category fails
                foreach (var categoryFail in orderInfo.CategoryFails)
                {
                    var orderPositionDescription = GenerateDescription(EntityName.OrderPosition, categoryFail.PositionName, categoryFail.OrderPositionId);
                    var categoryDescription = GenerateDescription(EntityName.Category, categoryFail.CategoryName, categoryFail.CategoryId);

                    if (categoryFail.CategoryNotActive)
                    {
                        messages.Add(new OrderValidationMessage
                            {
                                Type = MessageType.Error,
                                OrderId = orderInfo.OrderId,
                                OrderNumber = orderInfo.OrderNumber,
                                MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrderPositionCategoryNotActive, orderPositionDescription, categoryDescription),
                            });
                    }

                    if (categoryFail.CategoryNotBelongsToFirm)
                    {
                        MessageType messageType;

                        if (categoryFail.BindingObjectType == PositionBindingObjectType.CategoryMultipleAsterix)
                        {
                            messageType = MessageType.Info;
                        }
                        else
                        {
                            messageType = IsCheckMassive ? MessageType.Error : MessageType.Warning;
                        }

                        messages.Add(new OrderValidationMessage
                            {
                                Type = messageType,
                                OrderId = orderInfo.OrderId,
                                OrderNumber = orderInfo.OrderNumber,
                                MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrderPositionCategoryNotBelongsToFirm, orderPositionDescription, categoryDescription),
                            });
                    }
                }
            }
        }

        private void CheckForAddressCategoryFails(Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            var orderInfos = _finder.Find(filterPredicate)
                .Select(order => new
                    {
                        OrderId = order.Id,
                        OrderNumber = order.Number,

                        AddressesAndCategoryFails = order.OrderPositions
                            .Where(orderPosition => orderPosition.IsActive && !orderPosition.IsDeleted)
                            .SelectMany(orderPosition => orderPosition.OrderPositionAdvertisements)
                            .Where(positionAdvertisement => positionAdvertisement.FirmAddressId != null && positionAdvertisement.CategoryId != null)
                            .Select(z => new
                                {
                                    OrderPositionId = z.OrderPositionId,
                                    PositionName = z.Position.Name,

                                    AddressId = z.FirmAddressId.Value,
                                    AddressName = z.FirmAddress.Address + ((z.FirmAddress.ReferencePoint == null) ? string.Empty : " — " + z.FirmAddress.ReferencePoint),
                                    CategoryId = z.CategoryId.Value,
                                    CategoryName = z.Category.Name,

                                    // true means fail
                                    CategoryNotBelongsToAddress = z.FirmAddress.CategoryFirmAddresses
                                                                .Where(p => p.IsActive && !p.IsDeleted)
                                                                .All(p => p.CategoryId != z.CategoryId && p.Category.ParentCategory.ParentCategory.Id != z.CategoryId),
                                })
                                .Where(fail => fail.CategoryNotBelongsToAddress)
                    })
                .Where(order => order.AddressesAndCategoryFails.Any())
                .ToArray();

            foreach (var orderInfo in orderInfos)
            {
                // address and category fails
                foreach (var addressesAndCategoryFail in orderInfo.AddressesAndCategoryFails)
                {
                    var orderPositionDescription = GenerateDescription(EntityName.OrderPosition, addressesAndCategoryFail.PositionName, addressesAndCategoryFail.OrderPositionId);
                    var addressDescription = GenerateDescription(EntityName.FirmAddress, addressesAndCategoryFail.AddressName, addressesAndCategoryFail.AddressId);
                    var categoryDescription = GenerateDescription(EntityName.Category, addressesAndCategoryFail.CategoryName, addressesAndCategoryFail.CategoryId);

                    if (addressesAndCategoryFail.CategoryNotBelongsToAddress)
                    {
                        messages.Add(new OrderValidationMessage
                            {
                                Type = IsCheckMassive ? MessageType.Error : MessageType.Warning,
                                OrderId = orderInfo.OrderId,
                                OrderNumber = orderInfo.OrderNumber,
                                MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrderPositionCategoryNotBelongsToAddress, orderPositionDescription, categoryDescription, addressDescription),
                            });
                    }
                }
            }
        }
    }
}