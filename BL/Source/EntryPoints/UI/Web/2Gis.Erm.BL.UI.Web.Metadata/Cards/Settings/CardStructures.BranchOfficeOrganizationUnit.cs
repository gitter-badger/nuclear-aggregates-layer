using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.BranchOfficeOrganizationUnit;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata BranchOfficeOrganizationUnit =
            CardMetadata.For<BranchOfficeOrganizationUnit>()
                        .MainAttribute<BranchOfficeOrganizationUnit, IBranchOfficeOrganizationUnitViewModel>(x => x.ShortLegalName)
                        .Actions
                        .Attach(UiElementMetadata.Config.CreateAction<BranchOfficeOrganizationUnit>(),
                                UiElementMetadata.Config.UpdateAction<BranchOfficeOrganizationUnit>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.SaveAndCloseAction<BranchOfficeOrganizationUnit>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.RefreshAction<BranchOfficeOrganizationUnit>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.AdditionalActions(

                                                                           // COMMENT {all, 27.11.2014}: а как же безопасность?
                                                                           UiElementMetadata.Config
                                                                                            .Name.Static("SetAsPrimary")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlSetAsPrimary)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnNew()
                                                                                            .LockOnInactive()
                                                                                            .Handler.Name("scope.SetAsPrimary")
                                                                                            .Operation.NonCoupled<SetBranchOfficeOrganizationUnitAsPrimaryIdentity>(),

                                                                           // COMMENT {all, 27.11.2014}: а как же безопасность?
                                                                           UiElementMetadata.Config
                                                                                            .Name.Static("SetAsPrimaryForRegSales")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlSetAsPrimaryForRegSales)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .LockOnNew()
                                                                                            .Handler.Name("scope.SetAsPrimaryForRegSales")
                                                                                            .Operation.NonCoupled<SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesIdentity>()),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(UiElementMetadata.Config.ContentTab(),
                                            UiElementMetadata.Config
                                                             .Name.Static("PrintFormTemplates")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelPrintFormTemplates)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.PrintFormTemplate)
                                                             .FilterToParent());
    }
}