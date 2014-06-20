using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Emirates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Emirates
{
    public class EmiratesClientViewModelCustomizationService : IGenericViewModelCustomizationService<Client>, IEmiratesAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (EmiratesClientViewModel)viewModel;

            if (entityViewModel.IsNew)
            {
                // При создании нового клиента должна проставляться дата взятия из резерва, но не дата возвращения в резерв.
                // TODO {all, 17.06.2014}: вероятно, этой логике место в сервисе получения DomainEntityDto
                entityViewModel.LastQualifyTime = DateTime.UtcNow.Date;
            }
        }
    }
}