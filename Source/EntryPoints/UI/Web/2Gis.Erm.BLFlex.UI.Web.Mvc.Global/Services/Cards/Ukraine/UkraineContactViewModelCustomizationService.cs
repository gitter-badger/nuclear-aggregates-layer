using System.Collections.Generic;
using System.Web.Mvc;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Ukraine
{
    public class UkraineContactViewModelCustomizationService : IGenericViewModelCustomizationService<Contact>, IUkraineAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (UkraineContactViewModel)viewModel;
            entityViewModel.BusinessModelArea = BusinessModel.Ukraine.ToString();
            entityViewModel.AvailableSalutations = new Dictionary<string, string[]>
                {
                    { "Male", new[] { string.Empty, Resources.SalutationToMaleUkraine } }, 
                    { "Female", new[] { string.Empty, Resources.SalutationToFemaleUkraine } },
                };
        }
    }
}