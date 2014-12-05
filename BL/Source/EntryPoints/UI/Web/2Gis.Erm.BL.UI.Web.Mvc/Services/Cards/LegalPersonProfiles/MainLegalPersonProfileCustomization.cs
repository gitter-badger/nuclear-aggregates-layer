using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.LegalPersonProfiles
{
    public sealed class MainLegalPersonProfileCustomization : IViewModelCustomization<ICustomizableLegalPersonProfileViewModel>
    {
        public void Customize(ICustomizableLegalPersonProfileViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsMainProfile)
            {
                viewModel.SetInfo(BLResources.LegalPersonProfileIsMain);
            }
        }
    }
}