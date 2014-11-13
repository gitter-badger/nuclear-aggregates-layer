using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.LegalPersonProfiles
{
    public class MainLegalPersonProfileCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (ILegalPersonProfileViewModel)viewModel;

            if (entityViewModel.IsMainProfile)
            {
                entityViewModel.SetInfo(BLResources.LegalPersonProfileIsMain);
            }
        }
    }
}