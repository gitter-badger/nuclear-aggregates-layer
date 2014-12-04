using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions
{
    public static class CardMetadataBuilderExtensions
    {
        public static CardMetadataBuilder<TEntity> MainAttribute<TEntity, TViewModel>(this CardMetadataBuilder<TEntity> builder, Expression<Func<TViewModel, object>> propertyNameExpression)
            where TEntity : IEntityKey, IEntity
            where TViewModel : IEntityViewModelAbstract<TEntity>
        {
            builder.WithFeatures(new CardMainAttributeFeature(PropertyDescriptor.Create(propertyNameExpression)));
            return builder;
        }

        public static CardMetadataBuilder<TEntity> MainAttribute<TEntity>(this CardMetadataBuilder<TEntity> builder, Expression<Func<IEntityViewModelAbstract<TEntity>, object>> propertyNameExpression)
            where TEntity : IEntityKey, IEntity
        {
            builder.WithFeatures(new CardMainAttributeFeature(PropertyDescriptor.Create(propertyNameExpression)));
            return builder;
        }

        public static CardMetadataBuilder<TEntity> ConfigCommonCardToolbar<TEntity>(this CardMetadataBuilder<TEntity> builder)
             where TEntity : class, IEntityKey, IEntity
        {
            builder.Actions
                   .Attach(UiElementMetadata.Config.SaveAction<TEntity>(),
                           UiElementMetadata.Config.SplitterAction(),
                           UiElementMetadata.Config.SaveAndCloseAction<TEntity>(),
                           UiElementMetadata.Config.SplitterAction(),
                           UiElementMetadata.Config.RefreshAction<TEntity>(),
                           UiElementMetadata.Config.CloseAction());
           
            return builder;
        }

        public static CardMetadataBuilder<TEntity> ConfigActivityCardToolbar<TEntity>(this CardMetadataBuilder<TEntity> builder)
             where TEntity : class, IEntityKey, IEntity
        {
            builder.Actions
                   .Attach(UiElementMetadata.Config.SaveAction<TEntity>(),
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
                                            .AccessWithPrivelege<TEntity>(EntityAccessTypes.Create)
                                            .AccessWithPrivelege<TEntity>(EntityAccessTypes.Update)
                                            .Operation.SpecificFor<UpdateIdentity, TEntity>(),

                           UiElementMetadata.Config
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

                           UiElementMetadata.Config
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

                           UiElementMetadata.Config.AssignAction<TEntity>(),
                           UiElementMetadata.Config.SplitterAction(),
                           UiElementMetadata.Config.RefreshAction<TEntity>(),
                           UiElementMetadata.Config.CloseAction());

            return builder;
        }

        public static CardMetadataBuilder<TEntity> ConfigRelatedItems<TEntity>(this CardMetadataBuilder<TEntity> builder, params UiElementMetadata[] items)
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