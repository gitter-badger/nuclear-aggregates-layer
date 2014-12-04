using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata LegalPerson =
            CardMetadata.For<LegalPerson>()
                        .MainAttribute<LegalPerson, ILegalPersonViewModel>(x => x.LegalName)
                        .Actions
                            .Attach(UiElementMetadata.Config.SaveAction<LegalPerson>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.SaveAndCloseAction<LegalPerson>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.RefreshAction<LegalPerson>(),
                                    UiElementMetadata.Config.AdditionalActions(

                                                                               // COMMENT {all, 29.11.2014}: а как же безопасность?
                                                                               UiElementMetadata.Config
                                                                                                .Name.Static("ChangeLegalPersonClient")
                                                                                                .Title.Resource(() => ErmConfigLocalization.ControlChangeLegalPersonClient)
                                                                                                .ControlType(ControlType.TextButton)
                                                                                                .LockOnInactive()
                                                                                                .LockOnNew()
                                                                                                .Handler.Name("scope.ChangeLegalPersonClient")
                                                                                                .Operation.SpecificFor<ChangeClientIdentity, LegalPerson>(),

                                                                               // COMMENT {all, 29.11.2014}: а как же безопасность?
                                                                               UiElementMetadata.Config
                                                                                                .Name.Static("ChangeLPRequisites")
                                                                                                .Title.Resource(() => ErmConfigLocalization.ControlChangeLPRequisites)
                                                                                                .ControlType(ControlType.TextButton)
                                                                                                .LockOnInactive()
                                                                                                .LockOnNew()
                                                                                                .Handler.Name("scope.ChangeLegalPersonRequisites")
                                                                                                .Operation.NonCoupled<ChangeRequisitesIdentity>(),

                                                                               UiElementMetadata.Config
                                                                                                .Name.Static("Merge")
                                                                                                .Icon.Path("Merge.gif")
                                                                                                .Title.Resource(() => ErmConfigLocalization.ControlMerge)
                                                                                                .ControlType(ControlType.ImageButton)
                                                                                                .LockOnInactive()
                                                                                                .LockOnNew()
                                                                                                .Handler.Name("scope.Merge")
                                                                                                .AccessWithPrivelege(FunctionalPrivilegeName.MergeLegalPersons)
                                                                                                .Operation.SpecificFor<MergeIdentity, LegalPerson>()),

                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(UiElementMetadata.Config.ContentTab(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Account")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelAccounts)
                                                             .Icon.Path("en_ico_16_Account.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Account)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Limits")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelLimits)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Limit)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Bargains")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelBargains)
                                                             .Icon.Path("en_ico_16_Bargain.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Bargain)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Orders")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                             .Icon.Path("en_ico_16_Order.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Order)
                                                             .FilterToParent());
    }
}