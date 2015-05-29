using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata BranchOfficeOrganizationUnit =
            CardMetadata.For<BranchOfficeOrganizationUnit>()
                        .WithDefaultIcon()
                        .Actions
                        .Attach(ToolbarElements.Create<BranchOfficeOrganizationUnit>(),
                                ToolbarElements.Update<BranchOfficeOrganizationUnit>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<BranchOfficeOrganizationUnit>(),
                                ToolbarElements.UpdateAndClose<BranchOfficeOrganizationUnit>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<BranchOfficeOrganizationUnit>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Additional(ToolbarElements.BranchOfficeOrganizationUnits.SetAsPrimary(),
                                                           ToolbarElements.BranchOfficeOrganizationUnits.SetAsPrimaryForRegSales()),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close())
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.PrintFormTemplate(), () => ErmConfigLocalization.CrdRelPrintFormTemplates));
    }
}