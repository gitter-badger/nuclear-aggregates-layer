﻿using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata AccountDetail =
            CardMetadata.For<AccountDetail>()
                        .Actions.Attach(UiElementMetadata.Config.SaveAction<AccountDetail>(),
                                        UiElementMetadata.Config.SaveAndCloseAction<AccountDetail>(),
                                        UiElementMetadata.Config.RefreshAction<AccountDetail>(),
                                        UiElementMetadata.Config.CloseAction());
    }
}