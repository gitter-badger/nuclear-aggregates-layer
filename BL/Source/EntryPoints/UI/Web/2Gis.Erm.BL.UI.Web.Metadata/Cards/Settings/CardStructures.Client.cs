using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Client =
            CardMetadata.For<Client>()
                        .MainAttribute<Client, IClientViewModel>(x => x.Name)
                        .Actions
                        .Attach(UiElementMetadata.Config.CreateAction<Client>(),
                                UiElementMetadata.Config.UpdateAction<Client>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.SaveAndCloseAction<Client>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.RefreshAction<Client>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.AdditionalActions(
                                                                           // COMMENT {all, 27.11.2014}: а почему не Assign?
                                                                           UiElementMetadata.Config.ChangeOwnerAction<Client>(),

                                                                           // COMMENT {all, 27.11.2014}: а как же безопасность?
                                                                           UiElementMetadata.Config
                                                                                            .Name.Static("ChangeTerritory")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlChangeTerritory)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnNew()
                                                                                            .LockOnInactive()
                                                                                            .Handler.Name("scope.ChangeTerritory")
                                                                                            .Operation.SpecificFor<ChangeTerritoryIdentity, Client>(),
                                                                           UiElementMetadata.Config.QualifyAction<Client>(),
                                                                           UiElementMetadata.Config.DisqualifyAction<Client>(),
                                                                           UiElementMetadata.Config
                                                                                            .Name.Static("Merge")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlMerge)
                                                                                            .Icon.Path("Merge.gif")
                                                                                            .ControlType(ControlType.ImageButton)
                                                                                            .LockOnNew()
                                                                                            .LockOnInactive()
                                                                                            .Handler.Name("scope.Merge")
                                                                                            .AccessWithPrivelege(FunctionalPrivilegeName.MergeClients)
                                                                                            .Operation.SpecificFor<MergeIdentity, Client>()),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(
                                            UiElementMetadata.Config.ContentTab("en_ico_16_Client.gif"),
                                            UiElementMetadata.Config
                                                             .Name.Static("Firm")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelFirms)
                                                             .Icon.Path("en_ico_16_Firm.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Firm)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Contact")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelContacts)
                                                             .Icon.Path("en_ico_16_Contact.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Contact)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Deal")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelDeals)
                                                             .Icon.Path("en_ico_16_Deal.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Deal)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("LegalPerson")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelLegalPersons)
                                                             .Icon.Path("en_ico_16_LegalPerson.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.LegalPerson)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Orders")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                             .Icon.Path("en_ico_16_Order.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Order)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Limits")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelLimits)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Limit)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Bargains")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelBargains)
                                                             .Icon.Path("en_ico_16_Bargain.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Bargain)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Actions")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                                             .Icon.Path("en_ico_16_Action.gif")
                                                             .Handler.ShowGridByConvention(EntityName.Activity)
                                                             .FilterToParents()
                                                             .LockOnNew(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Links")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelClientLinks)
                                                             .Icon.Path("en_ico_16_Links.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.ClientLink)
                                                             .AppendapleEntity<Client>());
    }
}