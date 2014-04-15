using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers.Helpers
{
    public class IsChooseProfileNeededHelper
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly ILegalPersonRepository _legalPersonRepository;

        public IsChooseProfileNeededHelper(IOrderReadModel orderReadModel, ILegalPersonRepository legalPersonRepository)
        {
            _orderReadModel = orderReadModel;
            _legalPersonRepository = legalPersonRepository;
        }

        public ChooseProfileDialogState GetChooseProfileDialogState(long billId)
        {
            var order = _orderReadModel.GetOrderByBill(billId);

            var legalPersonProfile = GetLegalPersonProfileId(order.LegalPersonId);

            return new ChooseProfileDialogState
            {
                IsChooseProfileNeeded = legalPersonProfile == null,
                LegalPersonProfileId = legalPersonProfile
            };
        }

        public ChooseProfileDialogState GetChooseProfileDialogState(long orderId, PrintOrderType printOrderType)
        {
            var order = _orderReadModel.GetOrder(orderId);

            return new ChooseProfileDialogState
                {
                    IsChooseProfileNeeded = IsChooseProfileNeeded(order, printOrderType),
                    LegalPersonProfileId = GetLegalPersonProfileId(order)
                };
        }

        private bool IsChooseProfileNeeded(Order order, PrintOrderType printOrderType)
        {
            // TODO {all, 10.04.2014}: есть подозрения, что  можно убрать это условие
            if (!order.LegalPersonId.HasValue)
            {
                return true;
            }

            // TODO {all, 07.04.2014}: перевести на LegalPersonReadModel
            var legalPersonWithProfiles = _legalPersonRepository.GetLegalPersonWithProfiles(order.LegalPersonId.Value);

            var validLegalPersonProfileIdSpecified = order.LegalPersonProfileId.HasValue
                                                     && legalPersonWithProfiles.Profiles.Select(profile => profile.Id).Contains(order.LegalPersonProfileId.Value);

            if (validLegalPersonProfileIdSpecified && printOrderType != PrintOrderType.PrintOrder)
            {
                return false;
            }

            // TODO {v.lapeev, 09.04.2014}: Непонятное условие. Предполагаю, что в перечисленных документах (кстати, это какие?) для физлица не используется профиль.
            //                               Но это не так, например документ "Уведомление о расторжении для физ. лица.docx" имеет поле Profile.ChiefNameInGenitive
            //                               Предлагаю разобраться и по возможности выпилить это условие, запрашивая профиль всегда, когда их больше одного.
            if ((LegalPersonType)legalPersonWithProfiles.LegalPerson.LegalPersonTypeEnum == LegalPersonType.NaturalPerson
                && printOrderType != PrintOrderType.PrintOrder
                && printOrderType != PrintOrderType.PrintBargain
                && printOrderType != PrintOrderType.PrintReferenceInformation)
            {
                return false;
            }

            return legalPersonWithProfiles.Profiles.Count() != 1;
        }

        private long? GetLegalPersonProfileId(Order order)
        {
            if (order.LegalPersonProfileId.HasValue)
            {
                return order.LegalPersonProfileId.Value;
            }

            return GetLegalPersonProfileId(order.LegalPersonId);
        }

        private long? GetLegalPersonProfileId(long? legalPersonId)
        {
            if (legalPersonId.HasValue)
            {
                // TODO {all, 07.04.2014}: перевести на LegalPersonReadModel
                var legalPersonWithProfiles = _legalPersonRepository.GetLegalPersonWithProfiles(legalPersonId.Value);

                if (legalPersonWithProfiles.Profiles.Count() == 1)
                {
                    return legalPersonWithProfiles.Profiles.Single().Id;
                }
            }

            return null;
        }

        public class ChooseProfileDialogState
        {
            public bool IsChooseProfileNeeded { get; set; }

            public long? LegalPersonProfileId { get; set; }
        }
    }
}
