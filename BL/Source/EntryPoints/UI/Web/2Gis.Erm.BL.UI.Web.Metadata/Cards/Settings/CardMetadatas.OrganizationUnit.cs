using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata OrganizationUnit =
            CardMetadata.For<OrganizationUnit>()
                        .MainAttribute<OrganizationUnit, IOrganizationUnitViewModel>(x => x.Name)
                        .Actions.Attach(UIElementMetadata.Config.CreateAction<OrganizationUnit>(),
                                        UIElementMetadata.Config.UpdateAction<OrganizationUnit>(),
                                        UIElementMetadata.Config.SplitterAction(),
                                        UIElementMetadata.Config.CreateAndCloseAction<OrganizationUnit>(),
                                        UIElementMetadata.Config.UpdateAndCloseAction<OrganizationUnit>(),
                                        UIElementMetadata.Config.SplitterAction(),
                                        UIElementMetadata.Config.RefreshAction<OrganizationUnit>(),
                                        UIElementMetadata.Config.SplitterAction(),

                                        // COMMENT {all, 01.12.2014}: а как же безопасность?
                                        UIElementMetadata.Config
                                                         .Name.Static("ManageCategories")
                                                         .Title.Resource(() => ErmConfigLocalization.ControlManageCategories)
                                                         .ControlType(ControlType.TextImageButton)
                                                         .LockOnInactive()
                                                         .LockOnNew()
                                                         .Handler.Name("scope.ManageCategories")
                                                         .Icon.Path("en_ico_16_Category.gif"),
                                        UIElementMetadata.Config.SplitterAction(),
                                        UIElementMetadata.Config.CloseAction())
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