using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.LegalPersons
{
    public sealed class LegalPersonDoesntHaveAnyProfilesCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsNew)
            {
                return;
            }

            if (!((IDoesLegalPersonHaveAnyProfilesAspect)viewModel).HasProfiles)
            {
                viewModel.SetWarning(BLResources.MustMakeLegalPersonProfile);
            }
        }
    }
}