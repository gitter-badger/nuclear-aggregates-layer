﻿using DoubleGis.Erm.BL.UI.Metadata.Models.Contracts;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata BargainType =
            CardMetadata.Config
                        .For<BargainType>()
                        .MainAttribute<BargainType, IBargainTypeViewModel>(x => x.Name)                
                        .Actions
                            .Attach(UiElementMetadataHelper.ConfigCommonCardToolbarButtons<BargainType>());
    }
}