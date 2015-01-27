using System.Collections.Generic;
using System.Web.Mvc;

using DoubleGis.Erm.BL.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Contacts
{
    public sealed class ContactSalutationsCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        private readonly IContactSalutationsProvider _contactSalutationsPtovider;

        public ContactSalutationsCustomization(IContactSalutationsProvider contactSalutationsPtovider)
        {
            _contactSalutationsPtovider = contactSalutationsPtovider;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            ((IContactSalutationsAspect)viewModel).AvailableSalutations = new Dictionary<string, string[]>
                                                                              {
                                                                                  { "Male", _contactSalutationsPtovider.GetMaleSalutations() },
                                                                                  { "Female", _contactSalutationsPtovider.GetFemaleSalutations() },
                                                                              };
        }
    }
}