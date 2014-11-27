using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Operations.Generic;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions
{
    public static class UiElementMetadataBuilderExtensions
    {
        public static UiElementMetadataBuilder SaveAction<TEntity>(this UiElementMetadataBuilder builder)
            where TEntity : class, IEntity
        {
            return builder.Name.Static("Save")
                          .Title.Resource(() => ErmConfigLocalization.ControlSave)
                          .ControlType(ControlType.ImageButton)
                          .LockOnInactive()
                          .Handler.Name("scope.Save")
                          .Icon.Path("Save.gif")
                          .AccessWithPrivelege(EntityAccessTypes.Create, typeof(TEntity).AsEntityName())
                          .AccessWithPrivelege(EntityAccessTypes.Update, typeof(TEntity).AsEntityName())
                          .Operation.SpecificFor<CreateIdentity, TEntity>()
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

        public static UiElementMetadataBuilder SaveAndCloseAction<TEntity>(this UiElementMetadataBuilder builder)
            where TEntity : class, IEntity
        {
            return builder.Name.Static("SaveAndClose")
                          .Title.Resource(() => ErmConfigLocalization.ControlSaveAndClose)
                          .ControlType(ControlType.TextImageButton)
                          .LockOnInactive()
                          .Handler.Name("scope.SaveAndClose")
                          .Icon.Path("SaveAndClose.gif")
                          .AccessWithPrivelege(EntityAccessTypes.Create, typeof(TEntity).AsEntityName())
                          .AccessWithPrivelege(EntityAccessTypes.Update, typeof(TEntity).AsEntityName())
                          .Operation.SpecificFor<CreateIdentity, TEntity>()
                          .Operation.SpecificFor<UpdateIdentity, TEntity>()
                          .Operation.NonCoupled<CloseIdentity>();
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

        public static UiElementMetadataBuilder ContentTab(this UiElementMetadataBuilder builder)
        {
            return builder.Name.Static("ContentTab")
                          .Title.Resource(() => ErmConfigLocalization.CrdRelInformation)
                          .Icon.Path("en_ico_16_Default.gif");
        }
    }
}
