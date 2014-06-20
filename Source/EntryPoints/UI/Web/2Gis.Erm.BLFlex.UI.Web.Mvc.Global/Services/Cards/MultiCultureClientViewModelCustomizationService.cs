using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    public class MultiCultureClientViewModelCustomizationService : IGenericViewModelCustomizationService<Client>, ICyprusAdapted, ICzechAdapted, IChileAdapted, IUkraineAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (MultiCultureClientViewModel)viewModel;

            if (entityViewModel.IsNew)
            {
                // При создании нового клиента должна проставляться дата взятия из резерва, но не дата возвращения в резерв.
                // TODO {all, 17.06.2014}: вероятно, этой логике место в сервисе получения DomainEntityDto
                entityViewModel.LastQualifyTime = DateTime.UtcNow.Date;
            }
        }
    }
}