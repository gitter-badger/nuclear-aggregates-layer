using System.Collections.Generic;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using BLResources = DoubleGis.Erm.BL.Resources.Server.Properties.Resources;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Kazakhstan
{
    public class KazakhstanContactViewModelCustomizationService : IGenericViewModelCustomizationService<Contact>, IKazakhstanAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (UkraineKazakhstanContactViewModel)viewModel;
            entityViewModel.BusinessModelArea = BusinessModel.Ukraine.ToString();
            entityViewModel.AvailableSalutations = new Dictionary<string, string[]>
                {
                    { "Male", new[] { string.Empty, BLResources.SalutationToMaleRussia } }, 
                    { "Female", new[] { string.Empty, BLResources.SalutationToFemaleRussia } },
                };
        }
    }
}