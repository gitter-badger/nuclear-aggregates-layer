using System;
using System.Web.Mvc;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.UI.Web.Mvc.Models;
using DoubleGis.Erm.UI.Web.Mvc.Models.Base;

namespace DoubleGis.Erm.UI.Web.Mvc.Services.Cards
{
    // 2+: BLFlex\Source\EntryPoints\UI\Web\2Gis.Erm.BLFlex.Web.Mvc.Global\Services\Cards\CyprusClientViewModelCustomizationService.cs
    public class CyprusClientViewModelCustomizationService : IGenericViewModelCustomizationService<Client>, ICyprusAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (CyprusClientViewModel)viewModel;

            if (entityViewModel.IsNew)
            {
                // При создании нового клиента должна проставляться дата взятия из резерва, но не дата возвращения в резерв.
                entityViewModel.LastQualifyTime = DateTime.UtcNow.Date;
            }
        }
    }
}