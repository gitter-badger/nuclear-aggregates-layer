using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata BranchOffice =
            CardMetadata.For<BranchOffice>()
                        .MainAttribute<BranchOffice, IBranchOfficeViewModel>(x => x.Name)
                        .CommonCardToolbar()
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab("en_ico_16_BranchOffice.gif"),
                                          UIElementMetadata.Config
                                                           .Name.Static("BOOU")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelBOOU)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.BranchOfficeOrganizationUnit)
                                                           .FilterToParent());
    }
}