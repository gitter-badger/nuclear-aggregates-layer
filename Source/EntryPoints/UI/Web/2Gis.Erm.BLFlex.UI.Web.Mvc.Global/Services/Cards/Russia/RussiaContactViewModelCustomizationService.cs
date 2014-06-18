using System.Collections.Generic;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using BLResources = DoubleGis.Erm.BL.Resources.Server.Properties.Resources;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Russia
{
    public class RussiaContactViewModelCustomizationService : IGenericViewModelCustomizationService<Contact>, IRussiaAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (ContactViewModel)viewModel;
            entityViewModel.BusinessModelArea = BusinessModel.Russia.ToString();
            entityViewModel.AvailableSalutations = new Dictionary<string, string[]>
                {
                    // FIXME {y.baranihin, 04.06.2014}: ќставить только SalutationToMale и SalutationToFemale, без постфикса. языки (бизнес-модели) раздел€ютс€ resx-файлами, а не постфиксами к ключам
                    { "Male", new[] { string.Empty, BLResources.SalutationToMaleRussia } }, 
                    { "Female", new[] { string.Empty, BLResources.SalutationToFemaleRussia } },
                };
        }
    }
}