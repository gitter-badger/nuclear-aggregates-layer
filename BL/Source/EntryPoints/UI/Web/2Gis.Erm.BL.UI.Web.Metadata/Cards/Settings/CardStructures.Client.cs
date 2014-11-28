using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Metadata.Models.Contracts;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
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
            CardMetadata.Config
                        .For<Client>()
                        .MainAttribute<Client, IClientViewModel>(x => x.Id)
                        .Actions
                            .Attach(UiElementMetadata.Config.SaveAction<Client>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.SaveAndCloseAction<Client>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.RefreshAction<Client>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.AdditionalActions(
                                                                                // COMMENT {all, 27.11.2014}: а почему не Assign?
                                                                                UiElementMetadata.Config
                                                                                                 .Name.Static("ChangeOwner")
                                                                                                 .Title.Resource(() => ErmConfigLocalization.ControlChangeOwner)
                                                                                                 .ControlType(ControlType.TextButton)
                                                                                                 .LockOnNew()
                                                                                                 .LockOnInactive()
                                                                                                 .Handler.Name("scope.ChangeOwner")
                                                                                                 .AccessWithPrivelege(EntityAccessTypes.Assign, EntityName.Client)
                                                                                                 .Operation.SpecificFor<AssignIdentity, Client>(),

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
                                                                                                 .ControlType(ControlType.ImageButton)
                                                                                                 .LockOnNew()
                                                                                                 .LockOnInactive()
                                                                                                 .Handler.Name("scope.Merge")
                                                                                                 .AccessWithPrivelege(FunctionalPrivilegeName.MergeClients)
                                                                                                 .Operation.SpecificFor<MergeIdentity, Client>()),
                                  UiElementMetadata.Config.SplitterAction(),
                                  UiElementMetadata.Config.CloseAction())
                            .RelatedItems
                            .Name("Information")
                            .Title(() => ErmConfigLocalization.CrdRelInformationHeader)
                            .Attach(UiElementMetadata.Config.ContentTab(),
                                    UiElementMetadata.Config
                                                     .Name.Static("Firm")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelFirms)
                                                     .Icon.Path("en_ico_16_Firm.gif")
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.Firm),
                                    UiElementMetadata.Config
                                                     .Name.Static("Contact")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelContacts)
                                                     .Icon.Path("en_ico_16_Contact.gif")
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.Contact),
                                    UiElementMetadata.Config
                                                     .Name.Static("Deal")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelDeals)
                                                     .Icon.Path("en_ico_16_Deal.gif")
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.Deal),
                                    UiElementMetadata.Config
                                                     .Name.Static("LegalPerson")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelLegalPersons)
                                                     .Icon.Path("en_ico_16_LegalPerson.gif")
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.LegalPerson),
                                    UiElementMetadata.Config
                                                     .Name.Static("Orders")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                     .Icon.Path("en_ico_16_Order.gif")
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.Order),
                                    UiElementMetadata.Config
                                                     .Name.Static("Limits")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelLimits)
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.Limit),
                                    UiElementMetadata.Config
                                                     .Name.Static("Bargains")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelBargains)
                                                     .Icon.Path("en_ico_16_Bargain.gif")
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.Bargain),
                                    UiElementMetadata.Config
                                                     .Name.Static("ActivityHistory")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelActivityHistory)
                                                     .Icon.Path("en_ico_16_history.gif")
                                                     .LockOnNew(),
                                    UiElementMetadata.Config
                                                     .Name.Static("Actions")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                                     .Icon.Path("en_ico_16_Action.gif")
                                                     .LockOnNew(),
                                    UiElementMetadata.Config
                                                     .Name.Static("Links")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelClientLinks)
                                                     .Icon.Path("en_ico_16_Links.gif")
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.ClientLink));
    }
}