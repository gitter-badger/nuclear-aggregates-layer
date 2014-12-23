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
        public static readonly CardMetadata OrganizationUnit =
            CardMetadata.For<OrganizationUnit>()
                        .MainAttribute<OrganizationUnit, IOrganizationUnitViewModel>(x => x.Name)
                        .Actions.Attach(ToolbarElements.Create<OrganizationUnit>(),
                                        ToolbarElements.Update<OrganizationUnit>(),
                                        ToolbarElements.Splitter(),
                                        ToolbarElements.CreateAndClose<OrganizationUnit>(),
                                        ToolbarElements.UpdateAndClose<OrganizationUnit>(),
                                        ToolbarElements.Splitter(),
                                        ToolbarElements.Refresh<OrganizationUnit>(),
                                        ToolbarElements.Splitter(),
                                        ToolbarElements.OrganizationUnits.ManageCategories(),
                                        ToolbarElements.Splitter(),
                                        ToolbarElements.Close())
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab("en_ico_16_OrganizationUnit.gif"),
                                            UIElementMetadata.Config
                                                             .Name.Static("OUBO")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelOUBO)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.BranchOfficeOrganizationUnit)
                                                             .FilterToParent(),
                                            UIElementMetadata.Config
                                                             .Name.Static("Prices")
                                                             .Title.Resource(() => ErmConfigLocalization.EnMPrices)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Price)
                                                             .FilterToParent(),
                                            UIElementMetadata.Config
                                                             .Name.Static("Projects")
                                                             .Title.Resource(() => ErmConfigLocalization.EnMProjects)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Project)
                                                             .FilterToParent());
    }
}