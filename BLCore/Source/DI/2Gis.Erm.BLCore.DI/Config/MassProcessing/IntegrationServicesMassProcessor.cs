using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.DI.Factories.Integration;
using DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.DI.Common.Config;
using NuClear.Assembling.TypeProcessing;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.DI.Config.MassProcessing
{
    public sealed class IntegrationServicesMassProcessor : IMassProcessor
    {
        private static readonly Type DeserializeBusObjectServiceMarker = typeof(IDeserializeServiceBusObjectService);
        private static readonly Type GenericDeserializeBusObjectServiceMarker = typeof(IDeserializeServiceBusObjectService<>);

        private static readonly Type ImportServiceBusObjectServiceMarker = typeof(IImportServiceBusDtoService);
        private static readonly Type GenericImportServiceBusObjectServiceMarker = typeof(IImportServiceBusDtoService<>);

        private static readonly Type ServiceBusFlowMarker = typeof(IServiceBusFlow);

        private static readonly Type ServiceBusObjectMarker = typeof(IServiceBusDto);
        private static readonly Type GenericServiceBusObjectMarker = typeof(IServiceBusDto<>);

        private readonly IUnityContainer _container;
        private readonly List<Type> _deserializeBusObjectServices = new List<Type>();
        private readonly IDictionary<Type, string> _flowTypeToNameMap = new Dictionary<Type, string>();
        private readonly List<Type> _importServiceBusObjectServices = new List<Type>();
        private readonly Func<LifetimeManager> _lifetimeFactory;
        private readonly Type[] _messageTypesToIgnore;
        private readonly List<ServiceBusObjectDescriptor> _objectDescriptors = new List<ServiceBusObjectDescriptor>();

        public IntegrationServicesMassProcessor(IUnityContainer container, Func<LifetimeManager> lifetimeFactory, params Type[] messageTypesToIgnore)
        {
            _container = container;
            _lifetimeFactory = lifetimeFactory;
            _messageTypesToIgnore = messageTypesToIgnore;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { DeserializeBusObjectServiceMarker, ImportServiceBusObjectServiceMarker, ServiceBusFlowMarker, ServiceBusObjectMarker };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            foreach (var type in types.Where(ShouldBeProcessed))
            {
                if (DeserializeBusObjectServiceMarker.IsAssignableFrom(type))
                {
                    _deserializeBusObjectServices.Add(type);
                }

                if (ImportServiceBusObjectServiceMarker.IsAssignableFrom(type))
                {
                    _importServiceBusObjectServices.Add(type);
                }

                if (ServiceBusFlowMarker.IsAssignableFrom(type))
                {
                    _flowTypeToNameMap.Add(type, GetFlowName(type));
                }

                if (ServiceBusObjectMarker.IsAssignableFrom(type) && !_messageTypesToIgnore.Contains(type))
                {
                    _objectDescriptors.Add(GetObjectDescriptor(type));
                }
            }
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                // процессинг при втором проходе
                return;
            }

            ValidateAndRegisterDeserializeBusObjectServices();
            ValidateAndRegisterImportServiceBusObjectServicesFactory();
            RegisterImportMetadataProvider();
            RegisterImportService();
        }

        public bool ShouldBeProcessed(Type type)
        {
            return !type.IsInterface;
        }

        private static string GetFlowName(Type type)
        {
            var attribute = type.GetCustomAttribute<ServiceBusFlowDescriptionAttribute>();
            if (attribute == null)
            {
                throw new ApplicationException("Type implementing IServiceBusFlow should be marked with ServiceBusFlowDescriptionAttribute");
            }

            return attribute.FlowName;
        }

        private static ServiceBusObjectDescriptor GetObjectDescriptor(Type type)
        {
            var attribute = type.GetCustomAttribute<ServiceBusObjectDescriptionAttribute>();
            if (attribute == null)
            {
                throw new ApplicationException("Type implementing IServiceBusObject should be marked with ServiceBusObjectDescriptionAttribute");
            }

            return new ServiceBusObjectDescriptor(attribute.ObjectName, type, attribute.ProcessingOrder);
        }

        private static Type GetObjectFlow(Type objectType)
        {
            return objectType.GetInterfaces()
                             .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == GenericServiceBusObjectMarker)
                             .Select(x => x.GetGenericArguments()[0])
                             .Single();
        }

        private static bool HasImportOperationInterface(Type type)
        {
            var interfaces = type.GetInterfaces();
            return interfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == OperationIndicators.IdentifiedOperation) &&
                   interfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == GenericImportServiceBusObjectServiceMarker);
        }

        private static Type GetServiceBusDtoType(Type operationServiceType)
        {
            return operationServiceType.GetInterfaces()
                                       .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == GenericImportServiceBusObjectServiceMarker)
                                       .Select(x => x.GetGenericArguments()[0])
                                       .SingleOrDefault();
        }

        private void ValidateAndRegisterDeserializeBusObjectServices()
        {
            var processedTypes = new List<Type>();
            foreach (var serviceType in _deserializeBusObjectServices)
            {
                var serviceInterface = serviceType.GetInterfaces()
                                                  .Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == GenericDeserializeBusObjectServiceMarker);
                processedTypes.Add(serviceInterface.GetGenericArguments()[0]);
                _container.RegisterTypeWithDependencies(serviceInterface, serviceType, serviceType.ToString(), _lifetimeFactory(), Mapping.Erm);
            }

            var missedTypes = _objectDescriptors.Select(x => x.DtoType).Except(processedTypes).Select(x => x.Name).ToArray();
            if (missedTypes.Any())
            {
                throw new ApplicationException(string.Format("The following types are declared but not processed by any IDeserializeBusObjectService: {0}",
                                                             string.Join(", ", missedTypes)));
            }

            _container.RegisterType<IDeserializeServiceBusDtoServiceFactory, UnityDeserializeServiceBusDtoServiceFactory>();
        }

        private void ValidateAndRegisterImportServiceBusObjectServicesFactory()
        {
            var dtoTypeToImportOperationServiceMap = new Dictionary<Type, Type>();

            foreach (var serviceType in _importServiceBusObjectServices)
            {
                var serviceInterface = serviceType.GetInterfaces()
                                                  .Where(x => !x.IsGenericType && HasImportOperationInterface(x))
                                                  .Select(x => new { DtoType = GetServiceBusDtoType(x), OperationServiceType = x })
                                                  .SingleOrDefault();

                if (serviceInterface == null || serviceInterface.DtoType == null)
                {
                    throw new ApplicationException(string.Format("Service type {0} should implement IOperation<TOperationIdentity>", serviceType));
                }

                dtoTypeToImportOperationServiceMap.Add(serviceInterface.DtoType, serviceInterface.OperationServiceType);
            }

            var missedTypes = _objectDescriptors.Select(x => x.DtoType).Except(dtoTypeToImportOperationServiceMap.Keys).Select(x => x.Name).ToArray();
            if (missedTypes.Any())
            {
                throw new ApplicationException(string.Format("The following types are declared but not processed by any IImportServiceBusObjectService: {0}",
                                                             string.Join(", ", missedTypes)));
            }

            var injectionConstructor = new InjectionConstructor(new ResolvedParameter<IUnityContainer>(),
                                                                new ResolvedParameter<IImportMetadataProvider>(),
                                                                dtoTypeToImportOperationServiceMap);
            _container.RegisterType<IImportServiceBusDtoServiceFactory, UnityImportServiceBusDtoServiceFactory>(injectionConstructor);
        }

        private void RegisterImportMetadataProvider()
        {
            var objectsByFlow = _objectDescriptors.GroupBy(x => GetObjectFlow(x.DtoType))
                                                  .Select(x => new ServiceBusFlowDescriptor(GetFlowName(x.Key), x.Key, x.AsEnumerable()))
                                                  .ToArray();

            var flowsWithoutObjects = _flowTypeToNameMap.Keys.Except(objectsByFlow.Select(x => x.FlowType)).Select(x => x.Name).ToArray();
            if (flowsWithoutObjects.Any())
            {
                throw new ApplicationException(string.Format("The following flows are declared but don't have any object: {0}",
                                                             string.Join(", ", flowsWithoutObjects)));
            }

            _container.RegisterInstance<IImportMetadataProvider>(new ImportMetadataProvider(objectsByFlow));
        }

        private void RegisterImportService()
        {
            _container.RegisterTypeWithDependencies(typeof(IImportFromServiceBusService), typeof(ImportFromServiceBusService), _lifetimeFactory(), Mapping.Erm);
        }
    }
}