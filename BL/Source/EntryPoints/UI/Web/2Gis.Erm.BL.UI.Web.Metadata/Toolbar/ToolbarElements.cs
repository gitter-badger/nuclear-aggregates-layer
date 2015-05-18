using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Operations.Generic;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static UIElementMetadataBuilder Create<TEntity>()
            where TEntity : class, IEntity, IEntityKey
        {
            return UIElementMetadata.Config
                                    .Name.Static("Create")
                                    .Title.Resource(() => ErmConfigLocalization.ControlSave)
                                    .ControlType(ControlType.ImageButton)
                                    .LockOnInactive()
                                    .JSHandler("Save")
                                    .Icon.Path(Icons.Icons.Toolbar.Save)
                                    .HideOn<INewableAspect>(x => !x.IsNew)
                                    .AccessWithPrivelege<TEntity>(EntityAccessTypes.Create)
                                    .Operation.SpecificFor<CreateIdentity, TEntity>();
        }

        public static UIElementMetadataBuilder Update<TEntity>()
            where TEntity : class, IEntity, IEntityKey
        {
            return UIElementMetadata.Config
                                    .Name.Static("Update")
                                    .Title.Resource(() => ErmConfigLocalization.ControlSave)
                                    .ControlType(ControlType.ImageButton)
                                    .LockOnInactive()
                                    .JSHandler("Save")
                                    .Icon.Path(Icons.Icons.Toolbar.Save)
                                    .HideOn<INewableAspect>(x => x.IsNew)
                                    .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                                    .Operation.SpecificFor<UpdateIdentity, TEntity>();
        }

        public static UIElementMetadataBuilder Splitter()
        {
            return UIElementMetadata.Config
                                    .Name.Static("Splitter")
                                    .Title.Resource(() => ErmConfigLocalization.ControlSplitter)
                                    .ControlType(ControlType.Splitter)
                                    .LockOnInactive();
        }

        public static UIElementMetadataBuilder CreateAndClose<TEntity>()
            where TEntity : class, IEntity, IEntityKey
        {
            return UIElementMetadata.Config
                                    .Name.Static("CreateAndClose")
                                    .Title.Resource(() => ErmConfigLocalization.ControlSaveAndClose)
                                    .ControlType(ControlType.TextImageButton)
                                    .LockOnInactive()                                    
                                    .JSHandler("SaveAndClose")
                                    .Icon.Path(Icons.Icons.Toolbar.SaveAndClose)
                                    .HideOn<INewableAspect>(x => !x.IsNew)
                                    .AccessWithPrivelege<TEntity>(EntityAccessTypes.Create)
                                    .Operation.SpecificFor<CreateIdentity, TEntity>()
                                    .Operation.NonCoupled<CloseIdentity>();
        }

        public static UIElementMetadataBuilder UpdateAndClose<TEntity>()
            where TEntity : class, IEntity, IEntityKey
        {
            return UIElementMetadata.Config
                                    .Name.Static("UpdateAndClose")
                                    .Title.Resource(() => ErmConfigLocalization.ControlSaveAndClose)
                                    .ControlType(ControlType.TextImageButton)
                                    .LockOnInactive()
                                    .JSHandler("SaveAndClose")
                                    .Icon.Path(Icons.Icons.Toolbar.SaveAndClose)
                                    .HideOn<INewableAspect>(x => x.IsNew)
                                    .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                                    .Operation.SpecificFor<UpdateIdentity, TEntity>()
                                    .Operation.NonCoupled<CloseIdentity>();
        }

        public static UIElementMetadataBuilder Qualify<TEntity>()
            where TEntity : class, IEntity
        {
            return UIElementMetadata.Config
                                    .Name.Static("Qualify")
                                    .Title.Resource(() => ErmConfigLocalization.ControlQualify)
                                    .ControlType(ControlType.TextButton)
                                    .LockOnNew()
                                    .JSHandler("Qualify")
                                    .AccessWithPrivelege(FunctionalPrivilegeName.ReserveAccess)
                                    .Operation.SpecificFor<QualifyIdentity, TEntity>();
        }

        public static UIElementMetadataBuilder Disqualify<TEntity>()
            where TEntity : class, IEntity
        {
            return UIElementMetadata.Config
                                    .Name.Static("Disqualify")
                                    .Title.Resource(() => ErmConfigLocalization.ControlDisqualify)
                                    .ControlType(ControlType.TextButton)
                                    .LockOnInactive()
                                    .LockOnNew()
                                    .JSHandler("Disqualify")
                                    .AccessWithPrivelege(FunctionalPrivilegeName.ReserveAccess)
                                    .Operation.SpecificFor<QualifyIdentity, TEntity>();
        }

        public static UIElementMetadataBuilder Activate<TEntity>()
            where TEntity : class, IEntity
        {
            return UIElementMetadata.Config
                                    .Name.Static("Activate")
                                    .Title.Resource(() => ErmConfigLocalization.ControlActivate)
                                    .Icon.Path(Icons.Icons.Toolbar.Activate)
                                    .ControlType(ControlType.TextImageButton)
                                    .LockOnNew()
                                    .JSHandler("Activate")
                                    .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                                    .Operation.SpecificFor<ActivateIdentity, TEntity>();
        }

        public static UIElementMetadataBuilder Refresh<TEntity>()
            where TEntity : class, IEntity
        {
            return UIElementMetadata.Config
                                    .Name.Static("Refresh")
                                    .Title.Resource(() => ErmConfigLocalization.ControlRefresh)
                                    .ControlType(ControlType.TextImageButton)
                                    .JSHandler("refresh")
                                    .Icon.Path(Icons.Icons.Toolbar.Refresh)
                                    .Operation.SpecificFor<GetDomainEntityDtoIdentity, TEntity>();
        }

        // COMMENT {all, 28.11.2014}: А почему не assign?
        public static UIElementMetadataBuilder ChangeOwner<TEntity>()
            where TEntity : class, IEntity
        {
            return UIElementMetadata.Config
                                    .Name.Static("ChangeOwner")
                                    .Title.Resource(() => ErmConfigLocalization.ControlChangeOwner)
                                    .ControlType(ControlType.ImageButton)
                                    .LockOnInactive()
                                    .JSHandler("ChangeOwner")
                                    .AccessWithPrivelege<TEntity>(EntityAccessTypes.Assign)
                                    .Operation.SpecificFor<AssignIdentity, TEntity>();
        }

        public static UIElementMetadataBuilder Assign<TEntity>()
            where TEntity : class, IEntity
        {
            return UIElementMetadata.Config
                                    .Name.Static("Assign")
                                    .Title.Resource(() => ErmConfigLocalization.ControlAssign)
                                    .ControlType(ControlType.TextImageButton)
                                    .LockOnInactive()
                                    .JSHandler("Assign")
                                    .Icon.Path(Icons.Icons.Toolbar.Assign)
                                    .Operation.SpecificFor<AssignIdentity, TEntity>();

            // COMMENT {all, 26.11.2014}: а как же безопасность?
        }

        public static UIElementMetadataBuilder ChangeTerritory<TEntity>()
            where TEntity : class, IEntity, IEntityKey
        {
            return

                // COMMENT {all, 27.11.2014}: а как же безопасность?
                UIElementMetadata.Config
                                 .Name.Static("ChangeTerritory")
                                 .Title.Resource(() => ErmConfigLocalization.ControlChangeTerritory)
                                 .ControlType(ControlType.TextButton)
                                 .LockOnNew()
                                 .LockOnInactive()
                                 .JSHandler("ChangeTerritory")
                                 .Operation.SpecificFor<ChangeTerritoryIdentity, TEntity>();
        }

        public static UIElementMetadataBuilder Close()
        {
            return UIElementMetadata.Config
                                    .Name.Static("Close")
                                    .Title.Resource(() => ErmConfigLocalization.ControlClose)
                                    .ControlType(ControlType.TextImageButton)
                                    .JSHandler("Close")
                                    .Icon.Path(Icons.Icons.Toolbar.Close)
                                    .Operation.NonCoupled<CloseIdentity>();
        }

        public static UIElementMetadataBuilder Print(params UIElementMetadata[] printActions)
        {
            return UIElementMetadata.Config
                                    .Name.Static("PrintActions")
                                    .Title.Resource(() => ErmConfigLocalization.ControlPrintActions)
                                    .ControlType(ControlType.Menu)
                                    .Childs(printActions);
        }

        public static UIElementMetadataBuilder Additional(params UIElementMetadata[] actions)
        {
            return UIElementMetadata.Config
                                    .Name.Static("Actions")
                                    .Title.Resource(() => ErmConfigLocalization.ControlActions)
                                    .ControlType(ControlType.Menu)
                                    .Childs(actions);
        }

        public static UIElementMetadataBuilder SaveAndCreateOnBasis(params UIElementMetadata[] activities)
        {
            return
                UIElementMetadata.Config.Name.Static("SaveAndCreateOnBasis")
                                 .Title.Resource(() => ErmConfigLocalization.ControlSaveAndCreateOnBasis)
                                 .ControlType(ControlType.Menu)
                                 .Childs(activities);
        }

        public static UIElementMetadataBuilder CreateAppointment()
        {
            return UIElementMetadata.Config
                                    .Name.Static("CreateAppointment")
                                    .Title.Resource(() => ErmConfigLocalization.ControlCreateAppointment)
                                    .ControlType(ControlType.TextButton)
                                    .JSHandler("CreateAppointment")                                    
                                    .AccessWithEntityTypePrivilege<Appointment>(EntityAccessTypes.Create)                                    
                                    .Operation.SpecificFor<CreateIdentity, Appointment>();
        }

        public static UIElementMetadataBuilder CreateTask()
        {
            return UIElementMetadata.Config
                                    .Name.Static("CreateTask")
                                    .Title.Resource(() => ErmConfigLocalization.ControlCreateTask)
                                    .ControlType(ControlType.TextButton)
                                    .JSHandler("CreateTask")
                                    .AccessWithEntityTypePrivilege<Task>(EntityAccessTypes.Create)                                    
                                    .Operation.SpecificFor<CreateIdentity, Task>();
        }

        public static UIElementMetadataBuilder CreatePhonecall()
        {
            return UIElementMetadata.Config
                                    .Name.Static("CreatePhonecall")
                                    .Title.Resource(() => ErmConfigLocalization.ControlCreatePhonecall)
                                    .ControlType(ControlType.TextButton)
                                    .JSHandler("CreatePhonecall")
                                    .AccessWithEntityTypePrivilege<Phonecall>(EntityAccessTypes.Create)                                    
                                    .Operation.SpecificFor<CreateIdentity, Phonecall>();
        }

        public static UIElementMetadataBuilder CreateLetter()
        {
            return UIElementMetadata.Config
                                    .Name.Static("CreateLetter")
                                    .Title.Resource(() => ErmConfigLocalization.ControlCreateLetter)
                                    .ControlType(ControlType.TextButton)
                                    .JSHandler("CreateLetter")
                                    .AccessWithEntityTypePrivilege<Letter>(EntityAccessTypes.Create)                                    
                                    .Operation.SpecificFor<CreateIdentity, Letter>();
        }

        // COMMENT {all, 23.12.2014}: Есть подозрение, что нужно объединить с SaveAs (LocalMessage)
        public static UIElementMetadataBuilder DownloadResult()
        {
            return

                // COMMENT {all, 28.11.2014}: а как же безопасность?
                UIElementMetadata.Config
                                 .Name.Static("DownloadResults")
                                 .Title.Resource(() => ErmConfigLocalization.ControlDownloadResults)
                                 .ControlType(ControlType.TextButton)
                                 .JSHandler("DownloadResults")
                                 .LockOnNew();
        }
    }
}
