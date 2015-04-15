using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;

using Microsoft.Practices.Unity;

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

            if (!_metadataProvider.TryGetMetadata(IdBuilder.For<ViewModelCustomizationsIdentity>(typeof(TEntity).AsEntityName().ToString()), out customizationsMetadata))
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