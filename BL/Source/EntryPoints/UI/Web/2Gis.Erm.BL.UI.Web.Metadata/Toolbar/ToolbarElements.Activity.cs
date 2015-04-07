using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class Activities
        {
            public static UIElementMetadataBuilder Complete<TEntity>()
                where TEntity : class, IEntityKey, IEntity
            {
                return
                    UIElementMetadata.Config
                                     .Name.Static("Complete")
                                     .Title.Resource(() => ErmConfigLocalization.ControlComplete)
                                     .ControlType(ControlType.TextImageButton)
                                     .LockOnNew()
                                     .JSHandler("CompleteActivity")
                                     .Icon.Path(Icons.Icons.Toolbar.Check)
                                     .DisableOn<IDeactivatableAspect>(x => !x.IsActive)
                                     .DisableOn<IActivityStateAspect>(x => x.Status == ActivityStatus.Canceled,
                                                                      x => x.Status == ActivityStatus.Completed)

                        // COMMENT {all, 26.11.2014}:  А зачем права на создание? 
                                     .AccessWithPrivelege<TEntity>(EntityAccessTypes.Create)
                                     .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                                     .Operation.SpecificFor<UpdateIdentity, TEntity>();
            }

            public static UIElementMetadataBuilder Cancel<TEntity>()
                where TEntity : class, IEntityKey, IEntity
            {
                return
                    UIElementMetadata.Config
                                     .Name.Static("Cancel")
                                     .Title.Resource(() => ErmConfigLocalization.ControlCancel)
                                     .ControlType(ControlType.TextImageButton)
                                     .LockOnNew()
                                     .JSHandler("CancelActivity")
                                     .Icon.Path(Icons.Icons.Toolbar.Delete)
                                     .DisableOn<IDeactivatableAspect>(x => !x.IsActive)
                                     .DisableOn<IActivityStateAspect>(x => x.Status == ActivityStatus.Canceled,
                                                                      x => x.Status == ActivityStatus.Completed)

                        // COMMENT {all, 26.11.2014}:  А зачем права на создание? 
                                     .AccessWithPrivelege<TEntity>(EntityAccessTypes.Create)
                                     .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                                     .Operation.SpecificFor<UpdateIdentity, TEntity>();
            }

            public static UIElementMetadataBuilder Revert<TEntity>()
                where TEntity : class, IEntityKey, IEntity
            {
                return
                    UIElementMetadata.Config
                                     .Name.Static("Revert")
                                     .Title.Resource(() => ErmConfigLocalization.ControlRevert)
                                     .ControlType(ControlType.TextImageButton)
                                     .LockOnNew()
                                     .JSHandler("ReopenActivity")
                                     .Icon.Path(Icons.Icons.Toolbar.Reschedule)
                                     .DisableOn<IDeactivatableAspect>(x => !x.IsActive)
                                     .DisableOn<IActivityStateAspect>(x => x.Status == ActivityStatus.InProgress)
                                                         
                        // COMMENT {all, 26.11.2014}:  А зачем права на создание? 
                                     .AccessWithPrivelege<TEntity>(EntityAccessTypes.Create)
                                     .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                                     .Operation.SpecificFor<UpdateIdentity, TEntity>();
            }
        }
    }
}
