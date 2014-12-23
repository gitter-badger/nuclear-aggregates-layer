using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Client =
            CardMetadata.For<Client>()
                        .MainAttribute<Client, IClientViewModel>(x => x.Name)
                        .Actions
                        .Attach(UIElementMetadata.Config.CreateAction<Client>(),
                                UIElementMetadata.Config.UpdateAction<Client>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CreateAndCloseAction<Client>(),
                                UIElementMetadata.Config.UpdateAndCloseAction<Client>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.RefreshAction<Client>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.AdditionalActions(
                                                                           // COMMENT {all, 27.11.2014}: а почему не Assign?
                                                                           UIElementMetadata.Config.ChangeOwnerAction<Client>(),

                                                                           // COMMENT {all, 27.11.2014}: а как же безопасность?
                                                                           UIElementMetadata.Config
                                                                                            .Name.Static("ChangeTerritory")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlChangeTerritory)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnNew()
                                                                                            .LockOnInactive()
                                                                                            .Handler.Name("scope.ChangeTerritory")
                                                                                            .Operation.SpecificFor<ChangeTerritoryIdentity, Client>(),
                                                                           UIElementMetadata.Config.QualifyAction<Client>(),
                                                                           UIElementMetadata.Config.DisqualifyAction<Client>(),
                                                                           UIElementMetadata.Config
                                                                                            .Name.Static("Merge")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlMerge)
                                                                                            .Icon.Path("Merge.gif")
                                                                                            .ControlType(ControlType.ImageButton)
                                                                                            .LockOnNew()
                                                                                            .LockOnInactive()
                                                                                            .Handler.Name("scope.Merge")
                                                                                            .AccessWithPrivelege(FunctionalPrivilegeName.MergeClients)
                                                                                            .Operation.SpecificFor<MergeIdentity, Client>()),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CloseAction())
                        .WithRelatedItems(
                                            UIElementMetadata.Config.ContentTab("en_ico_16_Client.gif"),
                                            UIElementMetadata.Config
                                                             .Name.Static("Firm")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelFirms)
                                                             .Icon.Path("en_ico_16_Firm.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Firm)
                                                             .FilterToParent(),
                                            UIElementMetadata.Config
                                                             .Name.Static("Contact")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelContacts)
                                                             .Icon.Path("en_ico_16_Contact.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Contact)
                                                             .FilterToParent(),
                                            UIElementMetadata.Config
                                                             .Name.Static("Deal")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelDeals)
                                                             .Icon.Path("en_ico_16_Deal.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Deal)
                                                             .FilterToParent(),
                                            UIElementMetadata.Config
                                                             .Name.Static("LegalPerson")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelLegalPersons)
                                                             .Icon.Path("en_ico_16_LegalPerson.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.LegalPerson)
                                                             .FilterToParent(),
                                            UIElementMetadata.Config
                                                             .Name.Static("Orders")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                             .Icon.Path("en_ico_16_Order.gif")
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
                                                             .Icon.Path("en_ico_16_Bargain.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Bargain)
                                                             .FilterToParent(),
                                            UIElementMetadata.Config
                                                             .Name.Static("Actions")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                                             .Icon.Path("en_ico_16_Action.gif")
                                                             .Handler.ShowGridByConvention(EntityName.Activity)
                                                             .FilterToParents()
                                                             .LockOnNew(),
                                            UIElementMetadata.Config
                                                             .Name.Static("Links")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelClientLinks)
                                                             .Icon.Path("en_ico_16_Links.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.ClientLink)
                                                             .AppendapleEntity<Client>());
    }
}