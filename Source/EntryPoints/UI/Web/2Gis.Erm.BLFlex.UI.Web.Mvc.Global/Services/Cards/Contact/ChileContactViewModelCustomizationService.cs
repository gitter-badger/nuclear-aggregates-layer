using System.Collections.Generic;
using System.Web.Mvc;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    public class ChileContactViewModelCustomizationService : IGenericViewModelCustomizationService<Contact>, IChileAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (MultiCultureContactViewModel)viewModel;
            entityViewModel.BusinessModelArea = BusinessModel.Chile.ToString();
            entityViewModel.AvailableSalutations = new Dictionary<string, string[]>
                {
                    { "Male", new[] { string.Empty, Resources.SalutationToMaleChile } }, 
                    { "Female", new[] { string.Empty, Resources.SalutationToFemaleChile } },
                };
        }
    }
}