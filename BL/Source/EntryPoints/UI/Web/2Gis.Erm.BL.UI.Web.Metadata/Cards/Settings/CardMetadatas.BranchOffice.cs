using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata BranchOffice =
            CardMetadata.For<BranchOffice>()
                        .WithEntityIcon()
                        .CommonCardToolbar()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.Small(EntityName.BranchOffice)),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.BranchOfficeOrganizationUnit, () => ErmConfigLocalization.CrdRelBOOU));
    }
}