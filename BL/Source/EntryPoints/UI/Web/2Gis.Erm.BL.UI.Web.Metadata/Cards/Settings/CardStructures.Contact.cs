using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Metadata.Models.Contracts;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Contact =
            CardMetadata.Config
                        .For<Contact>()
                        .MainAttribute<Contact, IContactViewModel>(x => x.FullName)
                        .Actions
                            .Attach(UiElementMetadata.Config.SaveAction<Contact>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.SaveAndCloseAction<Contact>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.RefreshAction<Contact>(),
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
                                                                                                 .AccessWithPrivelege(EntityAccessTypes.Assign, EntityName.Contact)
                                                                                                 .Operation.SpecificFor<AssignIdentity, Contact>()),
                                  UiElementMetadata.Config.SplitterAction(),
                                  UiElementMetadata.Config.CloseAction())
                            .RelatedItems
                                .Name("Information")
                                .Title(() => ErmConfigLocalization.CrdRelInformationHeader)
                                .Attach(UiElementMetadata.Config.ContentTab(),
                                        UiElementMetadata.Config
                                                         .Name.Static("Actions")
                                                         .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                                         .Icon.Path("en_ico_16_Action.gif")
                                                         .LockOnNew(),
                                        UiElementMetadata.Config
                                                         .Name.Static("ActivityHistory")
                                                         .Title.Resource(() => ErmConfigLocalization.CrdRelActivityHistory)
                                                         .Icon.Path("en_ico_16_history.gif")
                                                         .LockOnNew());
    }
}