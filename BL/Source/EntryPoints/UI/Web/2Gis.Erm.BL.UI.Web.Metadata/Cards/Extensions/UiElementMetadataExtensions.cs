using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions
{
    public static class UiElementMetadataHelper
    {
        public static UiElementMetadata[] ConfigCommonCardToolbarButtons<TEntity>()
            where TEntity : class, IEntity
        {
            return new UiElementMetadata[]
                       {
                           UiElementMetadata.Config.SaveAction<TEntity>(),
                           UiElementMetadata.Config.SplitterAction(),
                           UiElementMetadata.Config.SaveAndCloseAction<TEntity>(),
                           UiElementMetadata.Config.SplitterAction(),
                           UiElementMetadata.Config.RefreshAction<TEntity>(),
                           UiElementMetadata.Config.CloseAction()
                       };
        }

        public static UiElementMetadata[] ConfigActivityCardToolbarButtons<TEntity>()
           where TEntity : class, IEntity
        {
            return new UiElementMetadata[]
                       {
                           UiElementMetadata.Config.SaveAction<TEntity>(),
                           UiElementMetadata.Config.SplitterAction(),
                           UiElementMetadata.Config.SaveAndCloseAction<TEntity>(),
                           UiElementMetadata.Config.SplitterAction(),

                           UiElementMetadata.Config
                                                .Name.Static("Complete")
                                                .Title.Resource(() => ErmConfigLocalization.ControlComplete)
                                                .ControlType(ControlType.TextImageButton)
                                                .LockOnNew()
                                                .Handler.Name("scope.CompleteActivity")
                                                .Icon.Path("Check.gif")
                                                         
                                                // COMMENT {all, 26.11.2014}:  А зачем права на создание? 
                                                .AccessWithPrivelege(EntityAccessTypes.Create, typeof(TEntity).AsEntityName())
                                                .AccessWithPrivelege(EntityAccessTypes.Update, typeof(TEntity).AsEntityName())
                                                .Operation.SpecificFor<UpdateIdentity, TEntity>(),

                           UiElementMetadata.Config
                                                .Name.Static("Cancel")
                                                .Title.Resource(() => ErmConfigLocalization.ControlCancel)
                                                .ControlType(ControlType.TextImageButton)
                                                .LockOnNew()
                                                .Handler.Name("scope.CancelActivity")
                                                .Icon.Path("Delete.png")
                                                         
                                                // COMMENT {all, 26.11.2014}:  А зачем права на создание? 
                                                .AccessWithPrivelege(EntityAccessTypes.Create, typeof(TEntity).AsEntityName())
                                                .AccessWithPrivelege(EntityAccessTypes.Update, typeof(TEntity).AsEntityName())
                                                .Operation.SpecificFor<UpdateIdentity, TEntity>(),

                           UiElementMetadata.Config
                                                .Name.Static("Revert")
                                                .Title.Resource(() => ErmConfigLocalization.ControlRevert)
                                                .ControlType(ControlType.TextImageButton)
                                                .LockOnNew()
                                                .Handler.Name("scope.RevertActivity")
                                                .Icon.Path("Reschedule.gif")
                                                         
                                                // COMMENT {all, 26.11.2014}:  А зачем права на создание? 
                                                .AccessWithPrivelege(EntityAccessTypes.Create, typeof(TEntity).AsEntityName())
                                                .AccessWithPrivelege(EntityAccessTypes.Update, typeof(TEntity).AsEntityName())
                                                .Operation.SpecificFor<UpdateIdentity, TEntity>(),

                           UiElementMetadata.Config.AssignAction<TEntity>(),
                           UiElementMetadata.Config.SplitterAction(),
                           UiElementMetadata.Config.RefreshAction<TEntity>(),
                           UiElementMetadata.Config.CloseAction()
                       };
        }
    }
}
