using System.IO;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Metadata;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BL.UI.Web.Metadata
{
    public sealed class EntityViewNameProvider : IEntityViewNameProvider
    {
        private readonly IGlobalizationSettings _businessModelSettings;
        private readonly IMetadataProvider _metadataProvider;

        public EntityViewNameProvider(IGlobalizationSettings businessModelSettings, IMetadataProvider metadataProvider)
        {
            _businessModelSettings = businessModelSettings;
            _metadataProvider = metadataProvider;
        }

        public string GetView<TViewModel, TEntity>() 
            where TViewModel : class, IViewModel
            where TEntity : IEntityKey
        {
            string viewName;
            if (TryGetViewNameFromMetadata<TViewModel, TEntity>(out viewName))
            {
                return viewName;
            }

            if (TryGetViewNameByAdaptation<TViewModel, TEntity>(out viewName))
            {
                return viewName;
            }

            if (TryGetViewNameByEntityName<TViewModel, TEntity>(out viewName))
            {
                return viewName;
            }

            throw new InvalidDataException(
                string.Format("Unable to determine a view for {0} viewmodel of {1} entity",
                              typeof(TViewModel),
                              typeof(TEntity).AsEntityName()));
        }

        internal bool TryGetViewNameFromMetadata<TViewModel, TEntity>(out string viewName)
        {
            var entityTypeName = typeof(TEntity).AsEntityName();

            viewName = string.Empty;

            CardMetadata metadata;
            if (_metadataProvider.TryGetMetadata(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataCardsIdentity>(entityTypeName.Description).Build().AsIdentity().Id, out metadata))
            {
                if (metadata.Uses<IViewModelViewMappingFeature>())
                {
                    viewName = metadata.Feature<IViewModelViewMappingFeature>().Mapping.ViewName;
                }
            }

            return !string.IsNullOrWhiteSpace(viewName);
        }

        internal bool TryGetViewNameByAdaptation<TViewModel, TEntity>(out string viewName)
        {
            var entityTypeName = typeof(TEntity).AsEntityName();

            viewName = string.Empty;

            if (typeof(IAdapted).IsAssignableFrom(typeof(TViewModel)))
            {
                viewName = string.Format("{0}/{1}", _businessModelSettings.BusinessModel, entityTypeName.Description);
            }

            return !string.IsNullOrWhiteSpace(viewName);
        }

        internal bool TryGetViewNameByEntityName<TViewModel, TEntity>(out string viewName)
        {
            var entityTypeName = typeof(TEntity).AsEntityName();

            viewName = entityTypeName.Description;

            return !string.IsNullOrWhiteSpace(viewName);
        }
    }
}
