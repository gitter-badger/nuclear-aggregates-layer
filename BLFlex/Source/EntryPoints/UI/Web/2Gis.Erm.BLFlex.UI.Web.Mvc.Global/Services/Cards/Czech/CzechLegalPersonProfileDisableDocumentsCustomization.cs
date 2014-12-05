using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Czech;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Czech
{
    public sealed class CzechLegalPersonProfileDisableDocumentsCustomization : IViewModelCustomization<CzechLegalPersonProfileViewModel>, ICzechAdapted
    {
        public void Customize(CzechLegalPersonProfileViewModel viewModel, ModelStateDictionary modelState)
        {
            viewModel.DisabledDocuments = new string[0];
        }
    }
}