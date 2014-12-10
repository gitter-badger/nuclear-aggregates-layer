using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Contact =
            CardMetadata.For<Contact>()
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
                                                                                UiElementMetadata.Config.ChangeOwnerAction<Contact>()),
                                  UiElementMetadata.Config.SplitterAction(),
                                  UiElementMetadata.Config.CloseAction())
                            .ConfigRelatedItems(
                                        UiElementMetadata.Config.ContentTab("en_ico_16_Contact.gif"),
                                        UiElementMetadata.Config
                                                         .Name.Static("Actions")
                                                         .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                                         .Icon.Path("en_ico_16_Action.gif")
                                                         .Handler.ShowGridByConvention(EntityName.Activity)
                                                         .FilterToParents()
                                                         .LockOnNew());
    }
}