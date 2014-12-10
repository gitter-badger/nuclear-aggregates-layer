using System;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Operations.Generic;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions
{
    public static class UiElementMetadataBuilderExtensions
    {
        [Obsolete("Использовать CreateAction и UpdateAction, когда благоприятные времена наступят")]
        public static UiElementMetadataBuilder SaveAction<TEntity>(this UiElementMetadataBuilder builder)
            where TEntity : class, IEntity
        {
            return builder.Name.Static("Save")
                          .Title.Resource(() => ErmConfigLocalization.ControlSave)
                          .ControlType(ControlType.ImageButton)
                          .LockOnInactive()
                          .Handler.Name("scope.Save")
                          .Icon.Path("Save.gif")
                          .AccessWithPrivelege<TEntity>(EntityAccessTypes.Create)
                          .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                          .Operation.SpecificFor<CreateIdentity, TEntity>()
                          .Operation.SpecificFor<UpdateIdentity, TEntity>();
        }

        public static UiElementMetadataBuilder CreateAction<TEntity>(this UiElementMetadataBuilder builder)
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

        public static UiElementMetadataBuilder UpdateAction<TEntity>(this UiElementMetadataBuilder builder)
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

        public static UiElementMetadataBuilder SplitterAction(this UiElementMetadataBuilder builder)
        {
            return builder.Name.Static("Splitter")
                          .Title.Resource(() => ErmConfigLocalization.ControlSplitter)
                          .ControlType(ControlType.Splitter)
                          .LockOnInactive();
        }

        public static UiElementMetadataBuilder PrintActions(this UiElementMetadataBuilder builder, params UiElementMetadata[] printActions)
        {
            return builder.Name.Static("PrintActions")
                          .Title.Resource(() => ErmConfigLocalization.ControlPrintActions)
                          .ControlType(ControlType.Menu)
                          .Childs(printActions);
        }

        public static UiElementMetadataBuilder AdditionalActions(this UiElementMetadataBuilder builder, params UiElementMetadata[] actions)
        {
            return builder.Name.Static("Actions")
                          .Title.Resource(() => ErmConfigLocalization.ControlActions)
                          .ControlType(ControlType.Menu)
                          .Childs(actions);
        }

        [Obsolete("Использовать CreateAndCloseAction и UpdateAndCloseAction, когда благоприятные времена наступят")]
        public static UiElementMetadataBuilder SaveAndCloseAction<TEntity>(this UiElementMetadataBuilder builder)
            where TEntity : class, IEntity
        {
            return builder.Name.Static("SaveAndClose")
                          .Title.Resource(() => ErmConfigLocalization.ControlSaveAndClose)
                          .ControlType(ControlType.TextImageButton)
                          .LockOnInactive()
                          .Handler.Name("scope.SaveAndClose")
                          .Icon.Path("SaveAndClose.gif")
                          .AccessWithPrivelege<TEntity>(EntityAccessTypes.Create)
                          .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                          .Operation.SpecificFor<CreateIdentity, TEntity>()
                          .Operation.SpecificFor<UpdateIdentity, TEntity>()
                          .Operation.NonCoupled<CloseIdentity>();
        }

        public static UiElementMetadataBuilder CreateAndCloseAction<TEntity>(this UiElementMetadataBuilder builder)
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

        public static UiElementMetadataBuilder UpdateAndCloseAction<TEntity>(this UiElementMetadataBuilder builder)
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

        public static UiElementMetadataBuilder QualifyAction<TEntity>(this UiElementMetadataBuilder builder)
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

        public static UiElementMetadataBuilder DisqualifyAction<TEntity>(this UiElementMetadataBuilder builder)
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

        public static UiElementMetadataBuilder ActivateAction<TEntity>(this UiElementMetadataBuilder builder)
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

        public static UiElementMetadataBuilder RefreshAction<TEntity>(this UiElementMetadataBuilder builder)
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
        public static UiElementMetadataBuilder ChangeOwnerAction<TEntity>(this UiElementMetadataBuilder builder)
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

        public static UiElementMetadataBuilder AssignAction<TEntity>(this UiElementMetadataBuilder builder)
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

        public static UiElementMetadataBuilder CloseAction(this UiElementMetadataBuilder builder)
        {
            return builder.Name.Static("Close")
                          .Title.Resource(() => ErmConfigLocalization.ControlClose)
                          .ControlType(ControlType.TextImageButton)
                          .Handler.Name("scope.Close")
                          .Icon.Path("Close.gif")
                          .Operation.NonCoupled<CloseIdentity>();
        }

        public static UiElementMetadataBuilder ContentTab(this UiElementMetadataBuilder builder, string icon)
        {
            return builder.Name.Static("ContentTab")
                          .Title.Resource(() => ErmConfigLocalization.CrdRelInformation)
                          .Icon.Path(icon);
        }

        public static UiElementMetadataBuilder ContentTab(this UiElementMetadataBuilder builder)
        {
            return builder.ContentTab("en_ico_16_Default.gif");
        }

        public static UiElementMetadataBuilder AppendapleEntity<TEntity>(this UiElementMetadataBuilder builder)
            where TEntity : IEntity
        {
            return builder.WithFeatures(new AppendableEntityFeature(typeof(TEntity).AsEntityName()));
        }

        public static UiElementMetadataBuilder FilterToParent(this UiElementMetadataBuilder builder)
        {
            return builder.WithFeatures(new FilterToParentFeature());
        }

        public static UiElementMetadataBuilder FilterToParents(this UiElementMetadataBuilder builder)
        {
            return builder.WithFeatures(new FilterByParentsFeature());
        }

        public static UiElementMetadataBuilder ExtendedInfo(this UiElementMetadataBuilder builder, IResourceDescriptor extendedInfo)
        {
            return builder.WithFeatures(new ExtendedInfoFeature(extendedInfo));
        }
    }
}
