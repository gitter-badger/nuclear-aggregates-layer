using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

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
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(Icons.Icons.Entity.Client),
                                          UIElementMetadata.Config
                                                           .Name.Static("Firm")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelFirms)
                                                           .Icon.Path(Icons.Icons.Entity.Firm)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Firm)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Contact")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelContacts)
                                                           .Icon.Path(Icons.Icons.Entity.ContactSmall)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Contact)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Deal")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelDeals)
                                                           .Icon.Path(Icons.Icons.Entity.DealSmall)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Deal)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("LegalPerson")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelLegalPersons)
                                                           .Icon.Path(Icons.Icons.Entity.LegalPerson)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.LegalPerson)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Orders")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                           .Icon.Path(Icons.Icons.Entity.Order)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Order)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Limits")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelLimits)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Limit)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Bargains")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelBargains)
                                                           .Icon.Path(Icons.Icons.Entity.BargainSmall)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Bargain)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Actions")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                                           .Icon.Path(Icons.Icons.Entity.Activity)
                                                           .Handler.ShowGridByConvention(EntityName.Activity)
                                                           .FilterToParents()
                                                           .LockOnNew(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Links")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelClientLinks)
                                                           .Icon.Path(Icons.Icons.Entity.ClientLink)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.ClientLink)
                                                           .AppendapleEntity<Client>());
    }
}