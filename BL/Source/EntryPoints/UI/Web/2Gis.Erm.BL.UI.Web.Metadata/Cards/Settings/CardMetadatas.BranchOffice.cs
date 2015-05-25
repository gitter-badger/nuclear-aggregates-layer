using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata BranchOffice =
            CardMetadata.For<BranchOffice>()
                        .WithEntityIcon()
                        .CommonCardToolbar()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.Small(EntityType.Instance.BranchOffice())),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.BranchOfficeOrganizationUnit(), () => ErmConfigLocalization.CrdRelBOOU),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.UserBranchOffice(), () => ErmConfigLocalization.CrdRelUsers)
                                                      .AppendapleEntity<User>());
    }
}