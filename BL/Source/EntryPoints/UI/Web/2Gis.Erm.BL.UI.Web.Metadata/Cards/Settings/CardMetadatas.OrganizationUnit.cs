﻿using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata OrganizationUnit =
            CardMetadata.For<OrganizationUnit>()
                        .WithEntityIcon()
                        .Actions.Attach(ToolbarElements.Create<OrganizationUnit>(),
                                        ToolbarElements.Update<OrganizationUnit>(),
                                        ToolbarElements.Splitter(),
                                        ToolbarElements.CreateAndClose<OrganizationUnit>(),
                                        ToolbarElements.UpdateAndClose<OrganizationUnit>(),
                                        ToolbarElements.Splitter(),
                                        ToolbarElements.Refresh<OrganizationUnit>(),
                                        ToolbarElements.Splitter(),
                                        ToolbarElements.OrganizationUnits.ManageCategories(),
                                        ToolbarElements.Splitter(),
                                        ToolbarElements.Close())
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.Small(EntityType.Instance.OrganizationUnit())),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.BranchOfficeOrganizationUnit(), () => ErmConfigLocalization.CrdRelOUBO),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Price(), () => ErmConfigLocalization.EnMPrices),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Project(), () => ErmConfigLocalization.EnMProjects));
    }
}