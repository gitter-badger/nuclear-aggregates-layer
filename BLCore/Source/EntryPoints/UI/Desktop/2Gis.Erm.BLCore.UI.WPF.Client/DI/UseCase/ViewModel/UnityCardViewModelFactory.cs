using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.Platform.Model.Simplified;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel
{
    public sealed class UnityCardViewModelFactory : ICardViewModelFactory
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IViewModelAspectResolver[] _aspectResolvers;

        public UnityCardViewModelFactory(IMetadataProvider metadataProvider, IViewModelAspectResolver[] aspectResolvers)
        {
            _metadataProvider = metadataProvider;
            _aspectResolvers = aspectResolvers;
        }

        public TViewModel Create<TViewModel>(IUseCase useCase, IEntityType entityName, long entityId) 
            where TViewModel : class, ICardViewModel
        {
            return (TViewModel)Create(useCase, entityName, entityId);
        }

        public ICardViewModel Create(IUseCase useCase, IEntityType entityName, long entityId)
        {
            var container = useCase.ResolveFactoryContext();

            var metadataId = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataCardsIdentity>(entityName.Description);
            CardMetadata cardMetadata;
            if (!_metadataProvider.TryGetMetadata(metadataId, out cardMetadata))
            {
                throw new InvalidOperationException("Can't get card metadata for entity: " + entityName);
            }

            if (cardMetadata.ViewModelViewMapping == null || cardMetadata.ViewModelViewMapping.ViewModelType == null)
            {
                throw new InvalidOperationException("Invalid structure for card dependencies. MVVM parts is not configured properly. EntityName: " + entityName);
            }

            var viewModelIdentity = new CardViewModelIdentity
                {
                    EntityId = entityId,
                    EntityName = entityName,
                    OperationIdentity = SimplifiedEntities.Entities.Contains(entityName)
                                                ? (IOperationIdentity)new ModifySimplifiedModelEntityIdentity()
                                                : (IOperationIdentity)new ModifyBusinessModelEntityIdentity()
                };

            var resolvedDependencies = new List<ResolverOverride>();
            if (cardMetadata.Features != null && cardMetadata.Features.Any())
            {
                foreach (var resolver in _aspectResolvers)
                {
                    DependencyOverride resolvedDependency;
                    if (resolver.TryResolveDependency(useCase, cardMetadata, viewModelIdentity, out resolvedDependency))
                    {
                        resolvedDependencies.Add(resolvedDependency);
                    }
                }
            }

            resolvedDependencies.Add(new DependencyOverride(typeof(ICardViewModelIdentity), viewModelIdentity));

            return (ICardViewModel)container.Resolve(cardMetadata.ViewModelViewMapping.ViewModelType, resolvedDependencies.ToArray());
        }
    }
}