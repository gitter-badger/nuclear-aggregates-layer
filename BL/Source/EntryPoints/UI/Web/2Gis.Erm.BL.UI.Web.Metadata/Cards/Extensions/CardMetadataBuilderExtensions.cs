using System;
using System.Linq.Expressions;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
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
                   .Attach(UIElementMetadata.Config.CreateAction<TEntity>(),
                           UIElementMetadata.Config.UpdateAction<TEntity>(),
                           UIElementMetadata.Config.SplitterAction(),
                           UIElementMetadata.Config.CreateAndCloseAction<TEntity>(),
                           UIElementMetadata.Config.UpdateAndCloseAction<TEntity>(),
                           UIElementMetadata.Config.SplitterAction(),
                           UIElementMetadata.Config.RefreshAction<TEntity>(),
                           UIElementMetadata.Config.SplitterAction(),
                           UIElementMetadata.Config.CloseAction());

            return builder;
        }

        public static CardMetadataBuilder<TEntity> ActivityCardToolbar<TEntity>(this CardMetadataBuilder<TEntity> builder)
             where TEntity : class, IEntityKey, IEntity
        {
            builder.Actions
                   .Attach(UIElementMetadata.Config.CreateAction<TEntity>(),
                           UIElementMetadata.Config.UpdateAction<TEntity>(),
                           UIElementMetadata.Config.SplitterAction(),
                           UIElementMetadata.Config.CreateAndCloseAction<TEntity>(),
                           UIElementMetadata.Config.UpdateAndCloseAction<TEntity>(),
                           UIElementMetadata.Config.SplitterAction(),

                           UIElementMetadata.Config
                                            .Name.Static("Complete")
                                            .Title.Resource(() => ErmConfigLocalization.ControlComplete)
                                            .ControlType(ControlType.TextImageButton)
                                            .LockOnNew()
                                            .Handler.Name("scope.CompleteActivity")
                                            .Icon.Path("Check.gif")
                                                         
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
                                                         
                                            // COMMENT {all, 26.11.2014}:  А зачем права на создание? 
                                            .AccessWithPrivelege<TEntity>(EntityAccessTypes.Create)
                                            .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                                            .Operation.SpecificFor<UpdateIdentity, TEntity>(),

                           UIElementMetadata.Config.AssignAction<TEntity>(),
                           UIElementMetadata.Config.SplitterAction(),
                           UIElementMetadata.Config.RefreshAction<TEntity>(),
                           UIElementMetadata.Config.CloseAction());

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