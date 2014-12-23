using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Contact =
            CardMetadata.For<Contact>()
                        .MainAttribute<Contact, IContactViewModel>(x => x.FullName)
                        .Actions
                        .Attach(UIElementMetadata.Config.CreateAction<Contact>(),
                                UIElementMetadata.Config.UpdateAction<Contact>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CreateAndCloseAction<Contact>(),
                                UIElementMetadata.Config.UpdateAndCloseAction<Contact>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.RefreshAction<Contact>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.AdditionalActions(
                                                                           // COMMENT {all, 27.11.2014}: а почему не Assign?
                                                                           UIElementMetadata.Config.ChangeOwnerAction<Contact>()),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(
                                            UIElementMetadata.Config.ContentTab("en_ico_16_Contact.gif"),
                                            UIElementMetadata.Config
                                                             .Name.Static("Actions")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                                             .Icon.Path("en_ico_16_Action.gif")
                                                             .Handler.ShowGridByConvention(EntityName.Activity)
                                                             .FilterToParents()
                                                             .LockOnNew());
    }
}