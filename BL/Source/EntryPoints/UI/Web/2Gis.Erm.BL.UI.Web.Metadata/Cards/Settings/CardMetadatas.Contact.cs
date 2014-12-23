using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Contact =
            CardMetadata.For<Contact>()
                        .MainAttribute<Contact, IContactViewModel>(x => x.FullName)
                        .Actions
                        .Attach(ToolbarElements.Create<Contact>(),
                                ToolbarElements.Update<Contact>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<Contact>(),
                                ToolbarElements.UpdateAndClose<Contact>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<Contact>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Additional(ToolbarElements.ChangeOwner<Contact>()),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close())
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab("en_ico_16_Contact.gif"),
                                          UIElementMetadata.Config
                                                           .Name.Static("Actions")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                                           .Icon.Path("en_ico_16_Action.gif")
                                                           .Handler.ShowGridByConvention(EntityName.Activity)
                                                           .FilterToParents()
                                                           .LockOnNew());
    }
}