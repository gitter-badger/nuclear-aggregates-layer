using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Operations.Generic;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions
{
    public static class UIElementMetadataBuilderExtensions
    {
        public static UIElementMetadataBuilder CreateAction<TEntity>(this UIElementMetadataBuilder builder)
            where TEntity : class, IEntity, IEntityKey
        {
            return builder.Name.Static("Create")
                          .Title.Resource(() => ErmConfigLocalization.ControlSave)
                          .ControlType(ControlType.ImageButton)
                          .LockOnInactive()
                          .Handler.Name("scope.Save")
                          .Icon.Path("Save.gif")
                          .HideOn<IEntityViewModelAbstract<TEntity>>(x => !x.IsNew)
                          .AccessWithPrivelege<TEntity>(EntityAccessTypes.Create)
                          .Operation.SpecificFor<CreateIdentity, TEntity>();
        }

        public static UIElementMetadataBuilder UpdateAction<TEntity>(this UIElementMetadataBuilder builder)
            where TEntity : class, IEntity, IEntityKey
        {
            return builder.Name.Static("Update")
                          .Title.Resource(() => ErmConfigLocalization.ControlSave)
                          .ControlType(ControlType.ImageButton)
                          .LockOnInactive()
                          .Handler.Name("scope.Save")
                          .Icon.Path("Save.gif")
                          .HideOn<IEntityViewModelAbstract<TEntity>>(x => x.IsNew)
                          .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                          .Operation.SpecificFor<UpdateIdentity, TEntity>();
        }

        public static UIElementMetadataBuilder SplitterAction(this UIElementMetadataBuilder builder)
        {
            return builder.Name.Static("Splitter")
                          .Title.Resource(() => ErmConfigLocalization.ControlSplitter)
                          .ControlType(ControlType.Splitter)
                          .LockOnInactive();
        }

        public static UIElementMetadataBuilder PrintActions(this UIElementMetadataBuilder builder, params UIElementMetadata[] printActions)
        {
            return builder.Name.Static("PrintActions")
                          .Title.Resource(() => ErmConfigLocalization.ControlPrintActions)
                          .ControlType(ControlType.Menu)
                          .Childs(printActions);
        }

        public static UIElementMetadataBuilder AdditionalActions(this UIElementMetadataBuilder builder, params UIElementMetadata[] actions)
        {
            return builder.Name.Static("Actions")
                          .Title.Resource(() => ErmConfigLocalization.ControlActions)
                          .ControlType(ControlType.Menu)
                          .Childs(actions);
        }

        public static UIElementMetadataBuilder CreateAndCloseAction<TEntity>(this UIElementMetadataBuilder builder)
            where TEntity : class, IEntity, IEntityKey
        {
            return builder.Name.Static("CreateAndClose")
                          .Title.Resource(() => ErmConfigLocalization.ControlSaveAndClose)
                          .ControlType(ControlType.TextImageButton)
                          .LockOnInactive()
                          .Handler.Name("scope.SaveAndClose")
                          .Icon.Path("SaveAndClose.gif")
                          .HideOn<IEntityViewModelAbstract<TEntity>>(x => !x.IsNew)
                          .AccessWithPrivelege<TEntity>(EntityAccessTypes.Create)
                          .Operation.SpecificFor<CreateIdentity, TEntity>()
                          .Operation.NonCoupled<CloseIdentity>();
        }

        public static UIElementMetadataBuilder UpdateAndCloseAction<TEntity>(this UIElementMetadataBuilder builder)
            where TEntity : class, IEntity, IEntityKey
        {
            return builder.Name.Static("UpdateAndClose")
                          .Title.Resource(() => ErmConfigLocalization.ControlSaveAndClose)
                          .ControlType(ControlType.TextImageButton)
                          .LockOnInactive()
                          .Handler.Name("scope.SaveAndClose")
                          .Icon.Path("SaveAndClose.gif")
                          .HideOn<IEntityViewModelAbstract<TEntity>>(x => x.IsNew)
                          .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                          .Operation.SpecificFor<UpdateIdentity, TEntity>()
                          .Operation.NonCoupled<CloseIdentity>();
        }

        public static UIElementMetadataBuilder QualifyAction<TEntity>(this UIElementMetadataBuilder builder)
            where TEntity : class, IEntity
        {
            return builder.Name.Static("Qualify")
                          .Title.Resource(() => ErmConfigLocalization.ControlQualify)
                          .ControlType(ControlType.TextButton)
                          .LockOnNew()
                          .Handler.Name("scope.Qualify")
                          .AccessWithPrivelege(FunctionalPrivilegeName.ReserveAccess)
                          .Operation.SpecificFor<QualifyIdentity, TEntity>();
        }

        public static UIElementMetadataBuilder DisqualifyAction<TEntity>(this UIElementMetadataBuilder builder)
           where TEntity : class, IEntity
        {
            return builder.Name.Static("Disqualify")
                          .Title.Resource(() => ErmConfigLocalization.ControlDisqualify)
                          .ControlType(ControlType.TextButton)
                          .LockOnInactive()
                          .LockOnNew()
                          .Handler.Name("scope.Disqualify")
                          .AccessWithPrivelege(FunctionalPrivilegeName.ReserveAccess)
                          .Operation.SpecificFor<QualifyIdentity, TEntity>();
        }

        public static UIElementMetadataBuilder ActivateAction<TEntity>(this UIElementMetadataBuilder builder)
           where TEntity : class, IEntity
        {
            return builder.Name.Static("Activate")
                          .Title.Resource(() => ErmConfigLocalization.ControlActivate)
                          .Icon.Path("Activate.png")
                          .ControlType(ControlType.TextImageButton)
                          .LockOnNew()
                          .Handler.Name("scope.Activate")
                          .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                          .Operation.SpecificFor<ActivateIdentity, TEntity>();
        }

        public static UIElementMetadataBuilder RefreshAction<TEntity>(this UIElementMetadataBuilder builder)
            where TEntity : class, IEntity
        {
            return builder.Name.Static("Refresh")
                          .Title.Resource(() => ErmConfigLocalization.ControlRefresh)
                          .ControlType(ControlType.TextImageButton)
                          .Handler.Name("scope.refresh")
                          .Icon.Path("Refresh.gif")
                          .Operation.SpecificFor<GetDomainEntityDtoIdentity, TEntity>();
        }

        // COMMENT {all, 28.11.2014}: А почему не assign?
        public static UIElementMetadataBuilder ChangeOwnerAction<TEntity>(this UIElementMetadataBuilder builder)
            where TEntity : class, IEntity
        {
            return builder.Name.Static("ChangeOwner")
                          .Title.Resource(() => ErmConfigLocalization.ControlChangeOwner)
                          .ControlType(ControlType.ImageButton)
                          .LockOnInactive()
                          .Handler.Name("scope.ChangeOwner")
                          .AccessWithPrivelege<TEntity>(EntityAccessTypes.Assign)
                          .Operation.SpecificFor<AssignIdentity, TEntity>();
        }

        public static UIElementMetadataBuilder AssignAction<TEntity>(this UIElementMetadataBuilder builder)
            where TEntity : class, IEntity
        {
            return builder.Name.Static("Assign")
                          .Title.Resource(() => ErmConfigLocalization.ControlAssign)
                          .ControlType(ControlType.TextImageButton)
                          .LockOnInactive()
                          .Handler.Name("scope.Assign")
                          .Icon.Path("en_ico_16_Assign.gif")
                          .Operation.SpecificFor<AssignIdentity, TEntity>();

            // COMMENT {all, 26.11.2014}: Доступ никак не ограничиваем?
        }

        public static UIElementMetadataBuilder CloseAction(this UIElementMetadataBuilder builder)
        {
            return builder.Name.Static("Close")
                          .Title.Resource(() => ErmConfigLocalization.ControlClose)
                          .ControlType(ControlType.TextImageButton)
                          .Handler.Name("scope.Close")
                          .Icon.Path("Close.gif")
                          .Operation.NonCoupled<CloseIdentity>();
        }

        public static UIElementMetadataBuilder ContentTab(this UIElementMetadataBuilder builder, string icon)
        {
            return builder.Name.Static("ContentTab")
                          .Title.Resource(() => ErmConfigLocalization.CrdRelInformation)
                          .Icon.Path(icon);
        }

        public static UIElementMetadataBuilder ContentTab(this UIElementMetadataBuilder builder)
        {
            return builder.ContentTab("en_ico_16_Default.gif");
        }

        public static UIElementMetadataBuilder AppendapleEntity<TEntity>(this UIElementMetadataBuilder builder)
            where TEntity : IEntity
        {
            return builder.WithFeatures(new AppendableEntityFeature(typeof(TEntity).AsEntityName()));
        }

        public static UIElementMetadataBuilder FilterToParent(this UIElementMetadataBuilder builder)
        {
            return builder.WithFeatures(new FilterToParentFeature());
        }

        public static UIElementMetadataBuilder FilterToParents(this UIElementMetadataBuilder builder)
        {
            return builder.WithFeatures(new FilterByParentsFeature());
        }

        public static UIElementMetadataBuilder ExtendedInfo(this UIElementMetadataBuilder builder, IResourceDescriptor extendedInfo)
        {
            return builder.WithFeatures(new ExtendedInfoFeature(extendedInfo));
        }
    }
}
