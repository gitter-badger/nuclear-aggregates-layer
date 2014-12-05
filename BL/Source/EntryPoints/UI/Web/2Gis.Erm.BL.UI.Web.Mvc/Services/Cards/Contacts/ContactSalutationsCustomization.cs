using System.Collections.Generic;
using System.Web.Mvc;

using DoubleGis.Erm.BL.API.Aggregates.Clients;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Contacts
{
    public sealed class ContactSalutationsCustomization : IViewModelCustomization<ICustomizableContactViewModel>
    {
        private readonly IContactSalutationsProvider _contactSalutationsPtovider;

        public ContactSalutationsCustomization(IContactSalutationsProvider contactSalutationsPtovider)
        {
            _contactSalutationsPtovider = contactSalutationsPtovider;
        }

        public void Customize(ICustomizableContactViewModel viewModel, ModelStateDictionary modelState)
        {
            viewModel.AvailableSalutations = new Dictionary<string, string[]>
                                                 {
                                                     { "Male", _contactSalutationsPtovider.GetMaleSalutations() },
                                                     { "Female", _contactSalutationsPtovider.GetFemaleSalutations() },
                                                 };
        }
    }
}