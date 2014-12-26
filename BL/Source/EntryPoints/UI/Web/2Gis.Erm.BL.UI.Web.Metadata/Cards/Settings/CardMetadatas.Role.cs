using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Role =
            CardMetadata.For<Role>()
                        .WithEntityIcon()
                        .CommonCardToolbar()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.Small(EntityName.Role)),
                                          RelatedItems.RelatedItem.Role.EntityPrivilege(),
                                          RelatedItems.RelatedItem.Role.FunctionalPrivilege());
    }
}