using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Metadata.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared
{
    [Obsolete("Убрать после удаление EntitySettings.xml")]
    public sealed class SetReadonlyCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        private readonly ICardsMetadataSettings _cardsMetadataSettings;

        public SetReadonlyCustomization(ICardsMetadataSettings cardsMetadataSettings)
        {
            _cardsMetadataSettings = cardsMetadataSettings;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            if (_cardsMetadataSettings.CardsMetadataSource == CardsMetadataSource.CodedMetadata)
            {
                return;
            }

            viewModel.ViewConfig.ReadOnly = true;
        }
    }
}