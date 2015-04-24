using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.DI.Proxies.Messaging;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Config;
using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.Platform.DI.Factories.Messaging
{
    public sealed class UnityMessageAggregatedProcessingResultsHandlerFactory : IMessageAggregatedProcessingResultsHandlerFactory
    {
        private readonly IReadOnlyDictionary<IMessageFlow, Func<Type>> _resolversMap;
        private readonly IUnityContainer _unityContainer;

        public UnityMessageAggregatedProcessingResultsHandlerFactory(IUnityContainer unityContainer, IReadOnlyDictionary<IMessageFlow, Func<Type>> resolversMap)
        {
            _unityContainer = unityContainer;

            _resolversMap = resolversMap;
        }

        public IMessageAggregatedProcessingResultsHandler[] Create(IEnumerable<IMessageFlow> sourceMessageFlows)
        {
            // COMMENT {all, 26.06.2014}: общий смысл - handler выполняет batch дообработку результатов после processingstrategy, основная цель снизить кол-во мелких вызовов persistence и т.п.  объединяя их в batch
            // => handler должен соответствовать не одному flow processing strategy, а целой группе, например, все кто использует одну и ту же таблицу для хранения и т.п.
            return sourceMessageFlows
                        .Select(ResolveHandler)
                        .ToArray();
        }

        private IMessageAggregatedProcessingResultsHandler ResolveHandler(IMessageFlow messageFlow)
        {
            var resolvedType = ResolveType(messageFlow);

            var scopedContainer = _unityContainer.CreateChildContainerWithParentDependencies(new[] { typeof(IUserContext) });

            var messageAggregatedProcessingResultsHandler = (IMessageAggregatedProcessingResultsHandler)scopedContainer.Resolve(resolvedType);
            return new UnityMessageAggregatedProcessingResultsHandlerProxy(scopedContainer, messageAggregatedProcessingResultsHandler);
        }

        private Type ResolveType(IMessageFlow messageFlow)
        {
            Func<Type> resolver;
            if (!_resolversMap.TryGetValue(messageFlow, out resolver))
            {
                return DefaultType();
            }

            return resolver();
        }

        private Type DefaultType()
        {
            return typeof(NullMessageAggregatedProcessingResultsHandler);
        }
    }
}