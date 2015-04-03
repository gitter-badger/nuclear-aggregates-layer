using System.Collections.Generic;
using System.Windows.Controls;

using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations.Concrete;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Views.Operations;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Utils;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel
{
    public sealed class UnityOperationConfiguratorViewModelFactory : IOperationConfiguratorViewModelFactory
    {
        private readonly IDictionary<IOperationIdentity, IViewModelViewTypeMapping> _concreteOperationManagersMap;

        public UnityOperationConfiguratorViewModelFactory()
        {
            _concreteOperationManagersMap = new Dictionary<IOperationIdentity, IViewModelViewTypeMapping>
                {
                    { AssignIdentity.Instance, ViewModelTypedViewMapping<AssignConfiguratorViewModel, AssignConfiguratorView>.Instance }
                };
        }

        public IOperationConfiguratorViewModel Create<TOperationIdentity>(IUseCase useCase, EntityName entityName, long[] operationProcessingEntities) 
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new()
        {
            var targetOperationIdentity = new TOperationIdentity();
            return Create(useCase, targetOperationIdentity, entityName, operationProcessingEntities);
        }

        public IOperationConfiguratorViewModel Create(IUseCase useCase, IOperationIdentity operationIdentity, EntityName entityName, long[] operationProcessingEntities)
        {
            IViewModelViewTypeMapping concreteOperationConfiguratorMapping;
            if (!_concreteOperationManagersMap.TryGetValue(operationIdentity, out concreteOperationConfiguratorMapping))
            {
                return null;
            }

            var factory = useCase.ResolveFactoryContext();
            var messageSink = factory.Resolve<IMessageSink>();

            var configuratorViewSelector = new ViewModel2ViewMappingsSelector(new[] { concreteOperationConfiguratorMapping });

            return (IOperationConfiguratorViewModel)factory.Resolve(
                concreteOperationConfiguratorMapping.ViewModelType,
                new ResolverOverride[]
                    {
                        new DependencyOverride(typeof(EntityName), entityName),
                        new DependencyOverride(typeof(long[]), operationProcessingEntities),
                        new DependencyOverride(typeof(IMessageSink), messageSink),
                        new DependencyOverride(typeof(DataTemplateSelector), configuratorViewSelector)
                    });
        }
    }
}