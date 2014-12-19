﻿using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata UserProfile =
            CardMetadata.For<UserProfile>()
                        .MainAttribute(x => x.Id)
                        .Actions
                        .Attach(UIElementMetadata.Config.CreateAction<UserProfile>(),
                                UIElementMetadata.Config.UpdateAction<UserProfile>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CreateAndCloseAction<UserProfile>(),
                                UIElementMetadata.Config.UpdateAndCloseAction<UserProfile>(),
                                UIElementMetadata.Config.CloseAction());
    }
}