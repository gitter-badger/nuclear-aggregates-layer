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
        public static readonly CardMetadata Contact =
            CardMetadata.For<Contact>()
            .Icon.Path(Icons.Icons.Entity.Contact)
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
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(Icons.Icons.Entity.ContactSmall),
                                          UIElementMetadata.Config
                                                           .Name.Static("Actions")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                                           .Icon.Path(Icons.Icons.Entity.Activity)
                                                           .Handler.ShowGridByConvention(EntityName.Activity)
                                                           .FilterToParents()
                                                           .LockOnNew());
    }
}