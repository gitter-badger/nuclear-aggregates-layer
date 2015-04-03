using System.Collections.Generic;
using System.Windows.Controls;

using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations.Concrete;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Views.Operations;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Utils;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

using Microsoft.Practices.Unity;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel
{
    public sealed class UnityOperationConfiguratorViewModelFactory : IOperationConfiguratorViewModelFactory
    {
        private readonly IDictionary<IOperationIdentity, IViewModelViewMapping> _concreteOperationManagersMap;

        public UnityOperationConfiguratorViewModelFactory()
        {
            _concreteOperationManagersMap = new Dictionary<IOperationIdentity, IViewModelViewMapping>
                {
                    { AssignIdentity.Instance, ViewModelViewMapping<AssignConfiguratorViewModel, AssignConfiguratorView>.Instance }
                };
        }

        public IOperationConfiguratorViewModel Create<TOperationIdentity>(IUseCase useCase, IEntityType entityName, long[] operationProcessingEntities) 
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new()
        {
            var targetOperationIdentity = new TOperationIdentity();
            return Create(useCase, targetOperationIdentity, entityName, operationProcessingEntities);
        }

        public IOperationConfiguratorViewModel Create(IUseCase useCase, IOperationIdentity operationIdentity, IEntityType entityName, long[] operationProcessingEntities)
        {
            IViewModelViewMapping concreteOperationConfiguratorMapping;
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
                        new DependencyOverride(typeof(IEntityType), entityName),
                        new DependencyOverride(typeof(long[]), operationProcessingEntities),
                        new DependencyOverride(typeof(IMessageSink), messageSink),
                        new DependencyOverride(typeof(DataTemplateSelector), configuratorViewSelector)
                    });
        }
    }
}