﻿using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Operation =
            CardMetadata.For<Operation>()
                        .Icon.Path(Icons.Icons.Entity.LocalMessage)
                        .Actions.Attach(ToolbarElements.Close());
    }
}