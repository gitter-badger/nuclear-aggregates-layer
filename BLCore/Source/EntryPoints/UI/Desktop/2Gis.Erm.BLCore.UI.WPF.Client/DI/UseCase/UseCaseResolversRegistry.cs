using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Resolvers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Resolvers;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase
{
    public sealed class UseCaseResolversRegistry : IUseCaseResolversRegistry
    {
        private readonly IUnityContainer _container;
        private readonly Dictionary<Type, IUseCaseResolver[]> _resolversMap;

        public UseCaseResolversRegistry(IUnityContainer container)
        {
            _container = container;
            var resolvers = new []
                                {
                                    typeof(NavigationUsecaseResolver), 
                                    typeof(NotificationFeedbackUseCaseResolver),
                                    typeof(OperationProgressUseCaseResolver)
                                };

            _resolversMap = resolvers
                .GroupBy(GetProcessingMessage, (type, types) => new KeyValuePair<Type, IUseCaseResolver[]>(type, GetResolvers(types)))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public bool TryGetResolvers(IMessage message, out IUseCaseResolver[] resolvers)
        {
            return _resolversMap.TryGetValue(message.GetType(), out resolvers);
        }

        private IUseCaseResolver[] GetResolvers(IEnumerable<Type> resolverTypes)
        {
            return resolverTypes
                    .Select(t => (IUseCaseResolver)_container.Resolve(t))
                    .ToArray();
        }

        private static Type GetProcessingMessage(Type resolverType)
        {
            var resolverIndicator = typeof(IUseCaseResolver);
            if (!resolverIndicator.IsAssignableFrom(resolverType))
            {
                throw new InvalidOperationException("Specified resolver " + resolverType.Name + " doesn't implement interface " + resolverIndicator);
            }

            var resolverGenericIndicator = typeof(IUseCaseResolver<>);
            var processingMessageType =
                resolverType.GetInterfaces()
                       .Where(t => t.IsGenericType && resolverGenericIndicator.IsAssignableFrom(t.GetGenericTypeDefinition()))
                       .Select(t => t.GetGenericArguments().First())
                       .SingleOrDefault();

            if (processingMessageType == null)
            {
                throw new InvalidOperationException("Resolver " + resolverType.Name + " doesn't implement generic interface " + resolverGenericIndicator);
            }

            return processingMessageType;
        }
    }
}