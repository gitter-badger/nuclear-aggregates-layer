using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers.Helpers
{
    public class ProfileChooseHelper
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;

        public ProfileChooseHelper(IOrderReadModel orderReadModel, ILegalPersonReadModel legalPersonReadModel, ISecurityServiceEntityAccess securityServiceEntityAccess)
        {
            _orderReadModel = orderReadModel;
            _legalPersonReadModel = legalPersonReadModel;
            _securityServiceEntityAccess = securityServiceEntityAccess;
        }

        public ChooseProfileDialogState GetChooseProfileDialogStateForBill(long billId)
        {
            var order = _orderReadModel.GetOrderByBill(billId);
            return GetChooseProfileDialogState(order);
        }

        public ChooseProfileDialogState GetChooseProfileDialogStateForOrder(long orderId)
        {
            var order = _orderReadModel.GetOrder(orderId);
            return GetChooseProfileDialogState(order);
        }

        private ChooseProfileDialogState GetChooseProfileDialogState(Order order)
        {
            if (!order.LegalPersonId.HasValue)
            {
                return new ChooseProfileDialogState
                {
                    IsChooseProfileNeeded = true,
                    LegalPersonProfileId = null
                };
            }

            var profileIds = _legalPersonReadModel.GetLegalPersonProfileIds(order.LegalPersonId.Value);
            return new ChooseProfileDialogState
            {
                IsChooseProfileNeeded = IsChooseProfileNeeded(order, profileIds),
                LegalPersonProfileId = GetLegalPersonProfileId(order, profileIds)
            };
        }

        private bool IsChooseProfileNeeded(Order order, IEnumerable<long> profileIds)
        {
            var orderContainsValidProfileId = !order.LegalPersonProfileId.HasValue || profileIds.Contains(order.LegalPersonProfileId.Value);
            if (!orderContainsValidProfileId)
            {
                return true;
            }

            return profileIds.Count() != 1;
        }

        private long? GetLegalPersonProfileId(Order order, IEnumerable<long> profileIds)
        {
            if (order.LegalPersonProfileId.HasValue)
            {
                return order.LegalPersonProfileId.Value;
            }

            if (order.LegalPersonId.HasValue)
            {
                if (profileIds.Count() == 1)
                {
                    return profileIds.Single();
                }
            }

            return null;
        }

        public class ChooseProfileDialogState
        {
            public bool IsChooseProfileNeeded { get; set; }
            public long? LegalPersonProfileId { get; set; }
        }

        public ChooseProfileViewModel GetViewModelByOrder(long orderId, long userId, long? profileId)
        {
            var order = _orderReadModel.GetOrder(orderId);
            return CreateViewModel(order, userId, profileId);
        }

        public ChooseProfileViewModel GetViewModelByBill(long billId, long userId, long? profileId)
        {
            var order = _orderReadModel.GetOrderByBill(billId);
            return CreateViewModel(order, userId, profileId);
        }

        private ChooseProfileViewModel CreateViewModel(Order order, long userId, long? profileId)
        {
            if (!order.LegalPersonId.HasValue)
            {
                throw new ArgumentException(string.Format("Order {0} has no LegalPerson specified", order.Id), "order");
            }

            return new ChooseProfileViewModel
                {
                    LegalPersonId = order.LegalPersonId.Value,
                    DefaultLegalPersonProfileId = profileId,
                    LegalPersonProfile = profileId.HasValue ? CreateLookupField(profileId.Value) : null,
                    IsCardReadOnly = !OrderUpdateAccessAllowed(order, userId)
                };
        }

        private bool OrderUpdateAccessAllowed(Order order, long userId)
        {
            return _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                                EntityName.Order,
                                                                userId,
                                                                order.Id,
                                                                order.OwnerCode,
                                                                order.OwnerCode);
        }

        private LookupField CreateLookupField(long profileId)
        {
            return new LookupField
                {
                    Key = profileId,
                    Value = _legalPersonReadModel.GetLegalPersonProfile(profileId).Name
                };
        }
    }
}
