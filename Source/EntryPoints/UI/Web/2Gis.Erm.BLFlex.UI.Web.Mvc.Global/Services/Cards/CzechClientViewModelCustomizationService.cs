using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
        // 2+: BLFlex\Source\EntryPoints\UI\Web\2Gis.Erm.BLFlex.Web.Mvc.Global\Services\Cards\CzechClientViewModelCustomizationService.cs
        public class CzechClientViewModelCustomizationService : IGenericViewModelCustomizationService<Client>, ICzechAdapted
        {
            public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
            {
                var entityViewModel = (CzechClientViewModel)viewModel;

                if (entityViewModel.IsNew)
                {
                    // При создании нового клиента должна проставляться дата взятия из резерва, но не дата возвращения в резерв.
                    entityViewModel.LastQualifyTime = DateTime.UtcNow.Date;
                }
            }
        }
}