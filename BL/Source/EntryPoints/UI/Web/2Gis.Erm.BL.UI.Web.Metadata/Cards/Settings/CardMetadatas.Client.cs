using DoubleGis.Erm.BL.Resources.Server.Properties;
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
        public static readonly CardMetadata Client =
            CardMetadata.For<Client>()
                        .Icon.Path(Icons.Icons.Entity.Small(EntityType.Instance.Client()))
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
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.Small(EntityType.Instance.Client())),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Firm(), Icons.Icons.Entity.Small(EntityType.Instance.Firm()), () => ErmConfigLocalization.CrdRelFirms),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Contact(), Icons.Icons.Entity.Small(EntityType.Instance.Contact()), () => ErmConfigLocalization.CrdRelContacts),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Deal(), Icons.Icons.Entity.Small(EntityType.Instance.Deal()), () => ErmConfigLocalization.CrdRelDeals),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.LegalPerson(),
                                                                               Icons.Icons.Entity.Small(EntityType.Instance.LegalPerson()),
                                                                               () => ErmConfigLocalization.CrdRelLegalPersons),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Order(), Icons.Icons.Entity.Small(EntityType.Instance.Order()), () => ErmConfigLocalization.CrdRelOrders),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Limit(), () => ErmConfigLocalization.CrdRelLimits),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Bargain(), Icons.Icons.Entity.Small(EntityType.Instance.Bargain()), () => ErmConfigLocalization.CrdRelBargains),
                                          RelatedItems.RelatedItem.ActivitiesGrid(),
                                          RelatedItems.RelatedItem.Client.ClientLinksGrid()
                                                      .AppendapleEntity<Client>());
    }
}