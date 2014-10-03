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
            return GetChooseProfileDialogState(order.LegalPersonProfileId, order.LegalPersonId);
        }

        public ChooseProfileDialogState GetChooseProfileDialogStateForBargain(long bargainId)
        {
            var legalPersonId = _orderReadModel.GetBargainLegalPersonId(bargainId);
            return GetChooseProfileDialogState(null, legalPersonId);
        }

        public ChooseProfileDialogState GetChooseProfileDialogStateForOrder(long orderId)
        {
            var order = _orderReadModel.GetOrder(orderId);
            return GetChooseProfileDialogState(order.LegalPersonProfileId, order.LegalPersonId);
        }

        private ChooseProfileDialogState GetChooseProfileDialogState(long? entityLegalPersonProfileId, long? entityLegalPersonId)
        {
            if (!entityLegalPersonId.HasValue)
            {
                return new ChooseProfileDialogState
                    {
                        IsChooseProfileNeeded = true,
                        LegalPersonProfileId = null
                    };
            }

            var profileIds = _legalPersonReadModel.GetLegalPersonProfileIds(entityLegalPersonId.Value);
            return new ChooseProfileDialogState
                {
                    IsChooseProfileNeeded = IsChooseProfileNeeded(entityLegalPersonProfileId, profileIds),
                    LegalPersonProfileId = GetLegalPersonProfileId(entityLegalPersonProfileId, entityLegalPersonId, profileIds)
                };
        }

        private bool IsChooseProfileNeeded(long? entityLegalPersonProfileId, IEnumerable<long> profileIds)
        {
            var orderContainsValidProfileId = !entityLegalPersonProfileId.HasValue || profileIds.Contains(entityLegalPersonProfileId.Value);
            if (!orderContainsValidProfileId)
            {
                return true;
            }

            return profileIds.Count() != 1;
        }

        private long? GetLegalPersonProfileId(long? entityLegalPersonProfileId, long? entityLegalPersonId, IEnumerable<long> profileIds)
        {
            if (entityLegalPersonProfileId.HasValue)
            {
                return entityLegalPersonProfileId.Value;
            }

            if (entityLegalPersonId.HasValue)
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
            return CreateViewModel(order.LegalPersonId, profileId, !OrderUpdateAccessAllowed(order, userId));
        }

        public ChooseProfileViewModel GetViewModelByBill(long billId, long userId, long? profileId)
        {
            var order = _orderReadModel.GetOrderByBill(billId);
            return CreateViewModel(order.LegalPersonId, profileId, !OrderUpdateAccessAllowed(order, userId));
        }

        public ChooseProfileViewModel GetViewModelByBargain(long bargainId, long? profileId)
        {
            var legalPersonId = _orderReadModel.GetBargainLegalPersonId(bargainId);
            return CreateViewModel(legalPersonId, profileId, false);
        }

        private ChooseProfileViewModel CreateViewModel(long? legalPersonId, long? profileId, bool isCardReadOnly)
        {
            if (!legalPersonId.HasValue)
            {
                throw new ArgumentException("LegalPersonId is not specified", "legalPersonId");
            }

            return new ChooseProfileViewModel
                {
                    LegalPersonId = legalPersonId.Value,
                    DefaultLegalPersonProfileId = profileId,
                    LegalPersonProfile = profileId.HasValue ? CreateLookupField(profileId.Value) : null,
                    IsCardReadOnly = isCardReadOnly 
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
