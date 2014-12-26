using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

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
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.ContactSmall),
                                          RelatedItems.RelatedItem.ActivitiesGrid());
    }
}