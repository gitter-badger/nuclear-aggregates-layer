﻿using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Client =
            CardMetadata.For<Client>()
                        .Icon.Path(Icons.Icons.Entity.Client)
                        .Actions
                        .Attach(ToolbarElements.Create<Client>(),
                                ToolbarElements.Update<Client>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<Client>(),
                                ToolbarElements.UpdateAndClose<Client>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<Client>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Additional(ToolbarElements.ChangeOwner<Client>(),
                                                           ToolbarElements.ChangeTerritory<Client>(),
                                                           ToolbarElements.Qualify<Client>(),
                                                           ToolbarElements.Disqualify<Client>(),
                                                           ToolbarElements.Clients.Merge()),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close())
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.Client),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Firm, Icons.Icons.Entity.Firm, () => ErmConfigLocalization.CrdRelFirms),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Contact, Icons.Icons.Entity.ContactSmall, () => ErmConfigLocalization.CrdRelContacts),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Deal, Icons.Icons.Entity.DealSmall, () => ErmConfigLocalization.CrdRelDeals),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.LegalPerson,
                                                                               Icons.Icons.Entity.LegalPerson,
                                                                               () => ErmConfigLocalization.CrdRelLegalPersons),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Order, Icons.Icons.Entity.Order, () => ErmConfigLocalization.CrdRelOrders),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Limit, () => ErmConfigLocalization.CrdRelLimits),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Bargain, Icons.Icons.Entity.BargainSmall, () => ErmConfigLocalization.CrdRelBargains),
                                          RelatedItems.RelatedItem.ActivitiesGrid(),
                                          RelatedItems.RelatedItem.Client.ClientLinksGrid()
                                                      .AppendapleEntity<Client>());
    }
}