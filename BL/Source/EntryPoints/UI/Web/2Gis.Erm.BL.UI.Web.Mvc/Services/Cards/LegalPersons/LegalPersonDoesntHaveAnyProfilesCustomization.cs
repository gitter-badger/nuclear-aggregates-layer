using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.LegalPersons
{
    public sealed class LegalPersonDoesntHaveAnyProfilesCustomization : IViewModelCustomization<ICustomizableLegalPersonViewModel>
    {
        public void Customize(ICustomizableLegalPersonViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsNew)
            {
                return;
            }

            if (!viewModel.HasProfiles)
            {
                viewModel.SetWarning(BLResources.MustMakeLegalPersonProfile);
            }
        }
    }
}