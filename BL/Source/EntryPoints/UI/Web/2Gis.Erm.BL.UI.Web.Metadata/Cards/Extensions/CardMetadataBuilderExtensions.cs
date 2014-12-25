using System;
using System.Linq.Expressions;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions
{
    public static class CardMetadataBuilderExtensions
    {
        public static CardMetadataBuilder<TEntity> MainAttribute<TEntity, TViewModel>(this CardMetadataBuilder<TEntity> builder, Expression<Func<TViewModel, object>> propertyNameExpression)
            where TEntity : IEntityKey, IEntity
            where TViewModel : IEntityViewModelAbstract<TEntity>
        {
            builder.WithFeatures(new CardMainAttributeFeature(new PropertyDescriptor<TViewModel>(propertyNameExpression)));
            return builder;
        }

        public static CardMetadataBuilder<TEntity> MainAttribute<TEntity>(this CardMetadataBuilder<TEntity> builder, Expression<Func<IEntityViewModelAbstract<TEntity>, object>> propertyNameExpression)
            where TEntity : IEntityKey, IEntity
        {
            builder.WithFeatures(new CardMainAttributeFeature(new PropertyDescriptor<IEntityViewModelAbstract<TEntity>>(propertyNameExpression)));
            return builder;
        }

        public static CardMetadataBuilder<TEntity> CommonCardToolbar<TEntity>(this CardMetadataBuilder<TEntity> builder)
            where TEntity : class, IEntityKey, IEntity
        {
            builder.Actions
                   .Attach(ToolbarElements.Create<TEntity>(),
                           ToolbarElements.Update<TEntity>(),
                           ToolbarElements.Splitter(),
                           ToolbarElements.CreateAndClose<TEntity>(),
                           ToolbarElements.UpdateAndClose<TEntity>(),
                           ToolbarElements.Splitter(),
                           ToolbarElements.Refresh<TEntity>(),
                           ToolbarElements.Splitter(),
                           ToolbarElements.Close());

            return builder;
        }

        public static CardMetadataBuilder<TEntity> ActivityCardToolbar<TEntity>(this CardMetadataBuilder<TEntity> builder)
            where TEntity : class, IEntityKey, IEntity
        {
            builder.Actions
                   .Attach(ToolbarElements.Create<TEntity>(),
                           ToolbarElements.Update<TEntity>(),
                           ToolbarElements.Splitter(),
                           ToolbarElements.CreateAndClose<TEntity>(),
                           ToolbarElements.UpdateAndClose<TEntity>(),
                           ToolbarElements.Splitter(),

                           UIElementMetadata.Config
                                            .Name.Static("Complete")
                                            .Title.Resource(() => ErmConfigLocalization.ControlComplete)
                                            .ControlType(ControlType.TextImageButton)
                                            .LockOnNew()
                                            .Handler.Name("scope.CompleteActivity")
                                            .Icon.Path("Check.gif")
                                            .DisableOn<IEntityViewModelAbstract<TEntity>>(x => !x.IsActive)
                                            .DisableOn<IActivityViewModel>(x => x.Status == ActivityStatus.Canceled,
                                                                           x => x.Status == ActivityStatus.Completed)
                                                                           
                               // COMMENT {all, 26.11.2014}:  А зачем права на создание? 
                                            .AccessWithPrivelege<TEntity>(EntityAccessTypes.Create)
                                            .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                                            .Operation.SpecificFor<UpdateIdentity, TEntity>(),

                           UIElementMetadata.Config
                                            .Name.Static("Cancel")
                                            .Title.Resource(() => ErmConfigLocalization.ControlCancel)
                                            .ControlType(ControlType.TextImageButton)
                                            .LockOnNew()
                                            .Handler.Name("scope.CancelActivity")
                                            .Icon.Path("Delete.png")
                                            .DisableOn<IEntityViewModelAbstract<TEntity>>(x => !x.IsActive)
                                            .DisableOn<IActivityViewModel>(x => x.Status == ActivityStatus.Canceled,
                                                                           x => x.Status == ActivityStatus.Completed)

                               // COMMENT {all, 26.11.2014}:  А зачем права на создание? 
                                            .AccessWithPrivelege<TEntity>(EntityAccessTypes.Create)
                                            .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                                            .Operation.SpecificFor<UpdateIdentity, TEntity>(),

                           UIElementMetadata.Config
                                            .Name.Static("Revert")
                                            .Title.Resource(() => ErmConfigLocalization.ControlRevert)
                                            .ControlType(ControlType.TextImageButton)
                                            .LockOnNew()
                                            .Handler.Name("scope.RevertActivity")
                                            .Icon.Path("Reschedule.gif")
                                            .DisableOn<IEntityViewModelAbstract<TEntity>>(x => !x.IsActive)
                                            .DisableOn<IActivityViewModel>(x => x.Status == ActivityStatus.InProgress)
                                                         
                               // COMMENT {all, 26.11.2014}:  А зачем права на создание? 
                                            .AccessWithPrivelege<TEntity>(EntityAccessTypes.Create)
                                            .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                                            .Operation.SpecificFor<UpdateIdentity, TEntity>(),

                           ToolbarElements.Assign<TEntity>(),
                           ToolbarElements.Splitter(),
                           ToolbarElements.Refresh<TEntity>(),
                           ToolbarElements.Close());

            return builder;
        }

        public static CardMetadataBuilder<TEntity> WithRelatedItems<TEntity>(this CardMetadataBuilder<TEntity> builder, params UIElementMetadata[] items)
            where TEntity : class, IEntityKey, IEntity
        {
            builder.RelatedItems
                   .Name("Information")
                   .Title(() => ErmConfigLocalization.CrdRelInformationHeader)
                   .Attach(items);

            return builder;
        }
    }
}