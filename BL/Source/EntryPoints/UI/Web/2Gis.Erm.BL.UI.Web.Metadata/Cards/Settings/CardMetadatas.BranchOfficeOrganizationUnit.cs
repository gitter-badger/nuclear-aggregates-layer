using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.BranchOfficeOrganizationUnit;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata BranchOfficeOrganizationUnit =
            CardMetadata.For<BranchOfficeOrganizationUnit>()
                        .MainAttribute<BranchOfficeOrganizationUnit, IBranchOfficeOrganizationUnitViewModel>(x => x.ShortLegalName)
                        .Actions
                        .Attach(UIElementMetadata.Config.CreateAction<BranchOfficeOrganizationUnit>(),
                                UIElementMetadata.Config.UpdateAction<BranchOfficeOrganizationUnit>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CreateAndCloseAction<BranchOfficeOrganizationUnit>(),
                                UIElementMetadata.Config.UpdateAndCloseAction<BranchOfficeOrganizationUnit>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.RefreshAction<BranchOfficeOrganizationUnit>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.AdditionalActions(

                                                                           // COMMENT {all, 27.11.2014}: а как же безопасность?
                                                                           UIElementMetadata.Config
                                                                                            .Name.Static("SetAsPrimary")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlSetAsPrimary)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnNew()
                                                                                            .LockOnInactive()
                                                                                            .Handler.Name("scope.SetAsPrimary")
                                                                                            .Operation.NonCoupled<SetBranchOfficeOrganizationUnitAsPrimaryIdentity>(),

                                                                           // COMMENT {all, 27.11.2014}: а как же безопасность?
                                                                           UIElementMetadata.Config
                                                                                            .Name.Static("SetAsPrimaryForRegSales")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlSetAsPrimaryForRegSales)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .LockOnNew()
                                                                                            .Handler.Name("scope.SetAsPrimaryForRegSales")
                                                                                            .Operation.NonCoupled<SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesIdentity>()),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CloseAction())
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(),
                                            UIElementMetadata.Config
                                                             .Name.Static("PrintFormTemplates")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelPrintFormTemplates)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.PrintFormTemplate)
                                                             .FilterToParent());
    }
}