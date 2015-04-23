using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions
{
    public static class CardMetadataBuilderExtensions
    {
        public static CardMetadataBuilder<TEntity> WithEntityIcon<TEntity>(this CardMetadataBuilder<TEntity> builder)
            where TEntity : class, IEntityKey, IEntity
        {
            return builder.Icon.Path(Icons.Icons.Entity.Large(typeof(TEntity).AsEntityName()));
        }

        public static CardMetadataBuilder<TEntity> WithDefaultIcon<TEntity>(this CardMetadataBuilder<TEntity> builder)
            where TEntity : class, IEntityKey, IEntity
        {
            return builder.Icon.Path(Icons.Icons.Entity.Default);
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
                       ToolbarElements.SaveAndCreateOnBasis(ToolbarElements.CreateTask(), ToolbarElements.CreatePhonecall(), ToolbarElements.CreateAppointment(), ToolbarElements.CreateLetter()),
                       ToolbarElements.CreateAndClose<TEntity>(),
                       ToolbarElements.UpdateAndClose<TEntity>(),
                           ToolbarElements.Splitter(),
                           ToolbarElements.Activities.Complete<TEntity>(),
                           ToolbarElements.Activities.Cancel<TEntity>(),
                           ToolbarElements.Activities.Revert<TEntity>(),
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