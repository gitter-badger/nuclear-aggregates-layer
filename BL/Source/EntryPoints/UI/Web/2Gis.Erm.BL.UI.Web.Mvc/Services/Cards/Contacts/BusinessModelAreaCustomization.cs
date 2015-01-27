using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model.Aspects;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Contacts
{
    public sealed class BusinessModelAreaCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        private readonly IBusinessModelSettings _businessModelSettings;

        public BusinessModelAreaCustomization(IBusinessModelSettings businessModelSettings)
        {
            _businessModelSettings = businessModelSettings;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            ((IBusinessModelAreaAspect)viewModel).BusinessModelArea = _businessModelSettings.BusinessModel.ToString();
        }
    }
}