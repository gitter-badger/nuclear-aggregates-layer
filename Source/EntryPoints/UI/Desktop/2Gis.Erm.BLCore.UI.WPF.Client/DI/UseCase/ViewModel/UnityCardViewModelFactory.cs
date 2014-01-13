using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Simplified;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel
{
    public sealed class UnityCardViewModelFactory : ICardViewModelFactory
    {
        private readonly ICardStructuresProvider _cardStructuresProvider;
        private readonly IViewModelAspectResolver[] _aspectResolvers;

        public UnityCardViewModelFactory(
            ICardStructuresProvider cardStructuresProvider,
            IViewModelAspectResolver[] aspectResolvers)
        {
            _cardStructuresProvider = cardStructuresProvider;
            _aspectResolvers = aspectResolvers;
        }

        public TViewModel Create<TViewModel>(IUseCase useCase, EntityName entityName, long entityId) 
            where TViewModel : class, ICardViewModel
        {
            return (TViewModel)Create(useCase, entityName, entityId);
        }

        public ICardViewModel Create(IUseCase useCase, EntityName entityName, long entityId)
        {
            var container = useCase.ResolveFactoryContext();
            CardStructure cardStructure;
            if (!_cardStructuresProvider.TryGetDescriptor(entityName, out cardStructure) || cardStructure == null)
            {
                throw new InvalidOperationException("Can't get card dependencies structure for entity: " + entityName);
            }

            if (cardStructure.ViewModelViewMapping == null || cardStructure.ViewModelViewMapping.ViewModelType == null)
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
            if (cardStructure.ElementFeatures != null && cardStructure.ElementFeatures.Any())
            {
                foreach (var resolver in _aspectResolvers)
                {
                    DependencyOverride resolvedDependency;
                    if (resolver.TryResolveDependency(useCase, cardStructure, viewModelIdentity, out resolvedDependency))
                    {
                        resolvedDependencies.Add(resolvedDependency);
                    }
                }
            }

            resolvedDependencies.Add(new DependencyOverride(typeof(ICardViewModelIdentity), viewModelIdentity));

            return (ICardViewModel)container.Resolve(cardStructure.ViewModelViewMapping.ViewModelType, resolvedDependencies.ToArray());
        }
    }
}