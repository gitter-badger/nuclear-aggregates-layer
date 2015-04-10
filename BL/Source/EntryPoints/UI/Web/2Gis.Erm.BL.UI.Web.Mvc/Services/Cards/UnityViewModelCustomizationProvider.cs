using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards
{
    public class UnityViewModelCustomizationProvider : IViewModelCustomizationProvider
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IUnityContainer _container;

        private readonly Type[] _sharedCustomizations =
            {
                typeof(SetReadonlyCustomization),
                typeof(SetTitleCustomization),
                typeof(RelatedItemsCustomization),
                typeof(ToolbarElementsAvailabilityCustomization),
                typeof(SetMessagesCustomization),
                typeof(EntityIsInactiveCustomization)
            };

        public UnityViewModelCustomizationProvider(IMetadataProvider metadataProvider, IUnityContainer container)
        {
            _metadataProvider = metadataProvider;
            _container = container;
        }

        public IEnumerable<IViewModelCustomization<TModel>> GetCustomizations<TModel, TEntity>()
            where TModel : IEntityViewModelBase
            where TEntity : IEntityKey
        {
            ViewModelCustomizationsMetadata customizationsMetadata;
            IEnumerable<Type> customizationTypes;

            if (!_metadataProvider.TryGetMetadata(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ViewModelCustomizationsIdentity>(typeof(TEntity).AsEntityName().ToString()), out customizationsMetadata))
            {
                customizationTypes = Enumerable.Empty<Type>();
            }
            else
            {
                customizationTypes = customizationsMetadata.Features
                                                           .Cast<IViewModelCustomizationFeature>()
                                                           .OrderBy(x => x.Order)
                                                           .Select(x => x.CustomizationType)
                                                           .ToArray();
            }

            return customizationTypes.Union(_sharedCustomizations)
                                     .Select(type => (IViewModelCustomization<TModel>)_container.Resolve(type))
                                     .ToArray();
        }
    }
}