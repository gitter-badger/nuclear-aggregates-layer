using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Chile
{
    public class ChileLegalPersonProfileDisableDocumentsCustomization : IViewModelCustomization<ChileLegalPersonProfileViewModel>, IChileAdapted
    {
        public void Customize(ChileLegalPersonProfileViewModel viewModel, ModelStateDictionary modelState)
        {
            viewModel.DisabledDocuments = new string[0];
        }
    }
}