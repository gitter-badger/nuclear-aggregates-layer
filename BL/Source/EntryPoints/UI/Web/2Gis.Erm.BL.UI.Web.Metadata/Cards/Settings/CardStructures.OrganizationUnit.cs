using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata OrganizationUnit =
            CardMetadata.For<OrganizationUnit>()
                        .MainAttribute<OrganizationUnit, IOrganizationUnitViewModel>(x => x.Name)
                        .Actions.Attach(UiElementMetadata.Config.SaveAction<OrganizationUnit>(),
                                        UiElementMetadata.Config.SplitterAction(),
                                        UiElementMetadata.Config.SaveAndCloseAction<OrganizationUnit>(),
                                        UiElementMetadata.Config.SplitterAction(),
                                        UiElementMetadata.Config.RefreshAction<OrganizationUnit>(),
                                        UiElementMetadata.Config.SplitterAction(),

                                        // COMMENT {all, 01.12.2014}: а как же безопасность?
                                        UiElementMetadata.Config
                                                         .Name.Static("ManageCategories")
                                                         .Title.Resource(() => ErmConfigLocalization.ControlManageCategories)
                                                         .ControlType(ControlType.TextImageButton)
                                                         .LockOnInactive()
                                                         .LockOnNew()
                                                         .Handler.Name("scope.ManageCategories")
                                                         .Icon.Path("en_ico_16_Category.gif"),
                                        UiElementMetadata.Config.SplitterAction(),
                                        UiElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(UiElementMetadata.Config.ContentTab(),
                                            UiElementMetadata.Config
                                                             .Name.Static("OUBO")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelOUBO)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.BranchOfficeOrganizationUnit)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Prices")
                                                             .Title.Resource(() => ErmConfigLocalization.EnMPrices)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Price)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Projects")
                                                             .Title.Resource(() => ErmConfigLocalization.EnMProjects)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Project)
                                                             .FilterToParent());
    }
}