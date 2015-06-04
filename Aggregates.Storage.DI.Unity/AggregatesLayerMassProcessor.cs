using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.DI.Unity.Config;

namespace NuClear.Aggregates.Storage.DI.Unity
{
    public sealed class AggregatesLayerMassProcessor : IMassProcessor
    {
        private readonly IUnityContainer _container;
        private readonly Func<Type, string> _registrationTypeMappingResolver;

        private readonly List<Type> _aggregateRepositoryTypes = new List<Type>();
        private readonly List<Type> _readModelTypes = new List<Type>();

        public AggregatesLayerMassProcessor(IUnityContainer container, Func<Type, string> registrationTypeMappingResolver)
        {
            _container = container;
            _registrationTypeMappingResolver = registrationTypeMappingResolver;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { Indicators.Aggregates.ReadModel, Indicators.Aggregates.Repository };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            foreach (var type in types.Where(ShouldBeProcessed))
            {
                if (type.IsAggregateReadModel())
                {
                    _readModelTypes.Add(type);
                }

                if (type.IsAggregateRepository())
                {
                    _aggregateRepositoryTypes.Add(type);
                }
            }
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                // ���������� ��� ������ �������
                return;
            }

            ProcessReadModels();
            ProcessAggregateRepositories();
        }

        private static bool ShouldBeProcessed(Type type)
        {
            if (type.GetTypeInfo().IsInterface || type.GetTypeInfo().IsAbstract)
            {
                return false;
            }

            return true;
        }

        private static object AggregateReadModelInjectionFactory(IUnityContainer container, Type readModelType, string registrationName)
        {
            var aggregateRepositoryFactory = container.Resolve<UnityAggregateReadModelFactory>();
            return aggregateRepositoryFactory.CreateAggregateReadModel(readModelType);
        }

        private static object AggregateRepositoryInjectionFactory(IUnityContainer container, Type repositoryType, string registrationName)
        {
            var aggregateRepositoryFactory = container.Resolve<UnityScopedAggregateRepositoryFactory>();
            return aggregateRepositoryFactory.CreateRepository(repositoryType);
        }
        
        private void ProcessAggregateRepositories()
        {
            var repositoriesMarkers = Indicators.Aggregates.Group.Repositories;
            
            foreach (var aggregateRepositoryType in _aggregateRepositoryTypes)
            {
                var implementedInterfacesWithoutMarkers =
                    aggregateRepositoryType.GetTypeInfo().ImplementedInterfaces
                                  .Where(t => t.IsAggregateRepository()
                                            && !repositoriesMarkers.Contains(t.GetTypeInfo().IsGenericType ? t.GetGenericTypeDefinition() : t))
                                  .Distinct()
                    .ToArray();

                // �.�. ����� �������� ������������ ����������� ����� ����� �� ������������ ����������� ��� ���� ��������� (������ IAggregateRepository) - 
                // ������ ����� ��������� ����� ����������� �����, ����� ����������� ���������� ������������ �����������
                if (!implementedInterfacesWithoutMarkers.Any())
                {
                    throw new InvalidOperationException(string.Format("Aggregate repository {0} have to implement " +
                                                                      "contract interface and must be used through the interface",
                                                                      aggregateRepositoryType));
                }

                foreach (var aggregateRepositoryInterface in implementedInterfacesWithoutMarkers)
                {
                    var genericArguments = aggregateRepositoryInterface.GenericTypeArguments;
                    if (aggregateRepositoryInterface.GetTypeInfo().IsGenericType &&
                        !aggregateRepositoryInterface.GetTypeInfo().IsGenericTypeDefinition &&
                        genericArguments.Any(x => x.IsGenericParameter))
                    {
                        // ����������� ��������� generic-���������� � ������� ��� �� �������� generic-���
                        // � ���� ������ � ���� T �������� IsGenericParameter = true
                        // ��������, IDeleteGenericEntityService<LegalPerson> <-> DeleteGenericEntityService<T>
                        // ������, � ����������� �� ����������� �� T ������������ ��� � secure, ���� unsecure scope; 
                        var genericTypeDefinition = aggregateRepositoryInterface.GetGenericTypeDefinition();
                        var mapping = _registrationTypeMappingResolver(aggregateRepositoryType);
                        
                        _container.RegisterType(genericTypeDefinition,
                                                aggregateRepositoryType,
                                                mapping,
                                                Lifetime.PerResolve,
                                                new InjectionFactory(AggregateRepositoryInjectionFactory));

                        // ��������� ����������� �������� �������� generic aggregate service � ����������� ������� aggregate service
                        _container.RegisterType(genericTypeDefinition,
                                                aggregateRepositoryType,
                                                Mapping.ConstructorInjectionNestedAggregateRepositoriesScope,
                                                Lifetime.PerResolve,
                                                new InjectionFactory(AggregateRepositoryInjectionFactory));
                    }
                    else
                    {
                        // ����������� ��������� generic-���������� � ������� ��� �� ��������� ���
                        // � ���� ������ � ���� T = LegalPerson �������� IsGenericParameter = false, �.�. generic ��������
                        // ��������, IDeleteGenericEntityService<LegalPerson> <-> DeleteLegalPersonService
                        // � ���� ������ ���������� ����������� �������� � ����������� �� ������ ������������ �����������, � ���� �� ��� ������������� ������, ��������, ��������
                        // ��������� ��� ��� ���, ��� ��������� ������. ���� ��� �� �����, ������� ����� ������������ �� ����� �����
                        _container.RegisterType(aggregateRepositoryInterface,
                                                aggregateRepositoryType,
                                                Mapping.ConstructorInjectionAggregateRepositoriesScope,
                                                Lifetime.PerResolve,
                                                new InjectionFactory(AggregateRepositoryInjectionFactory));

                        // COMMENT {all, 19.09.2013}: ����������� � �������������� scope ��� ����������� ���������� � UnityOperationServicesManager.GetEntitySpecificOperation
                        var mapping = _registrationTypeMappingResolver(aggregateRepositoryInterface);
                        _container.RegisterType(aggregateRepositoryInterface,
                                                aggregateRepositoryType,
                                                mapping,
                                                Lifetime.PerResolve,
                                                new InjectionFactory(AggregateRepositoryInjectionFactory));

                        // �����������, ����� ����� ���� ���� ����������� �������� ������������� �����������, 
                        // �������� � UoWScope, �������� ��������� �����������, � �� ���������� ����� - ����������
                        _container.RegisterType(aggregateRepositoryInterface,
                                                aggregateRepositoryType,
                                                Mapping.ExplicitlyCreatedAggregateRepositoriesScope,
                                                Lifetime.PerResolve);
                    }
                }

                _container.RegisterTypeWithDependencies(aggregateRepositoryType,
                                                        Mapping.ExplicitlyCreatedAggregateRepositoriesScope,
                                                        Lifetime.PerResolve,
                                                        null);
            }
        }

        private void ProcessReadModels()
        {
            foreach (var readModel in _readModelTypes)
            {
                var implementedInterfacesWithoutMarkers =
                    readModel.GetTypeInfo().ImplementedInterfaces
                             .Where(t => t.IsAggregateReadModel()
                                         && !Indicators.Aggregates.Group.ReadOnly.Contains(t.GetTypeInfo().IsGenericType ? t.GetGenericTypeDefinition() : t))
                             .Distinct()
                             .ToArray();

                // �.�. ����� �������� ������������ ����������� ����� ����� �� ������������ ����������� ��� ���� ��������� (������ IReadModel) - 
                // ������ ����� ��������� ����� ����������� �����, ����� ����������� ���������� ������������ �����������
                if (!implementedInterfacesWithoutMarkers.Any())
                {
                    throw new InvalidOperationException("Read model " + readModel + " have to implement contract interface and must be used through the interface");
                }

                foreach (var readModelInterface in implementedInterfacesWithoutMarkers)
                {
                    var genericArguments = readModelInterface.GenericTypeArguments;
                    if (readModelInterface.GetTypeInfo().IsGenericType &&
                        !readModelInterface.GetTypeInfo().IsGenericTypeDefinition &&
                        genericArguments.Any(x => x.IsGenericParameter))
                    {
                        throw new InvalidOperationException("Open generic implementations for read model are not allowed");
                    }

                    _container.RegisterType(readModelInterface,
                                            readModel,
                                            Mapping.ConstructorInjectionReadModelsScope,
                                            Lifetime.PerResolve,
                                            new InjectionFactory(AggregateReadModelInjectionFactory));
                }
            }
        }
    }
}