﻿using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Aspects;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Territory =
            CardMetadata.For<Territory>()
                        .WithEntityIcon()
                        .Actions
                        .Attach(ToolbarElements.Create<Territory>(),
                                ToolbarElements.Update<Territory>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<Territory>(),
                                ToolbarElements.UpdateAndClose<Territory>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<Territory>(),
                                ToolbarElements.Activate<Territory>()
                                               .HideOn<IDeactivatableAspect>(x => x.IsActive),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close())
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Firm, Icons.Icons.Entity.Small(EntityName.Firm), () => ErmConfigLocalization.CrdRelFirms));
    }
}