using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards
{
    public static class UiElementMetadataBuilderExtensions
    {
        public static UiElementMetadataBuilder SaveAction<TEntity>(this UiElementMetadataBuilder builder)
            where TEntity : class, IEntity
        {
            return builder.Name.Static("Save")
                          .Title.Resource(() => ErmConfigLocalization.ControlSave)
                          .Handler.Name("scope.Save")
                          .Operation.SpecificFor<CreateIdentity, TEntity>()
                          .Operation.SpecificFor<UpdateIdentity, TEntity>()
                          .Icon.Path("Save.gif");
        }

        public static UiElementMetadataBuilder SaveAndCloseAction<TEntity>(this UiElementMetadataBuilder builder)
            where TEntity : class, IEntity
        {
            return builder.Handler.Name("scope.SaveAndClose")
                          .Operation.SpecificFor<CreateIdentity, TEntity>()
                          .Operation.SpecificFor<UpdateIdentity, TEntity>();
        }
    }
}
