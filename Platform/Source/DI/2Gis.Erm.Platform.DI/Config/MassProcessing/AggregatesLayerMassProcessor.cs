using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Model.Aggregates;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Config.MassProcessing
{
    public sealed class AggregatesLayerMassProcessor : IMassProcessor
    {
        private readonly IUnityContainer _container;

        private readonly List<Type> _aggregateRepositoryTypes = new List<Type>();
        private readonly List<Type> _readModelTypes = new List<Type>();

        public AggregatesLayerMassProcessor(IUnityContainer container)
        {
            _container = container;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { ModelIndicators.Aggregates.ReadModel, ModelIndicators.Aggregates.Repository };
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
            if (type.IsInterface || type.IsAbstract)
            {
                return false;
            }

            return true;
        }

        private static object AggregateRepositoryInjectionFactory(IUnityContainer container, Type repositoryType, string registrationName)
        {
            var aggregateRepositoryFactory = container.Resolve<IAggregatesLayerRuntimeFactory>();
            return aggregateRepositoryFactory.CreateRepository(repositoryType);
        }

        private static object AggregateReadModelInjectionFactory(IUnityContainer container, Type readModelType, string registrationName)
        {
            var aggregateRepositoryFactory = container.Resolve<IAggregatesLayerRuntimeFactory>();
            return aggregateRepositoryFactory.CreateAggregateReadModel(readModelType);
        }

        private void ProcessAggregateRepositories()
        {
            var repositoriesMarkers = ModelIndicators.Aggregates.Group.Repositories;
            
            foreach (var aggregateRepositoryType in _aggregateRepositoryTypes)
            {
                var implementedInterfacesWithoutMarkers =
                    aggregateRepositoryType.GetInterfaces()
                                  .Where(t => ModelIndicators.IsAggregateRepository(t)
                                            && !repositoriesMarkers.Contains(t.IsGenericType ? t.GetGenericTypeDefinition() : t))
                                  .Distinct()
                    .ToArray();

                // �.�. ����� �������� ������������ ����������� ����� ����� �� ������������ ����������� ��� ���� ��������� (������ IAggregateRepository) - 
                // ������ ����� ��������� ����� ����������� �����, ����� ����������� ���������� ������������ �����������
                if (!implementedInterfacesWithoutMarkers.Any())
                {
                    throw new InvalidOperationException("Aggregate repository " + aggregateRepositoryType + " have to implement contract interface and must be used through the interface");
                }

                foreach (var aggregateRepositoryInterface in implementedInterfacesWithoutMarkers)
                {
                    var genericArguments = aggregateRepositoryInterface.GetGenericArguments();
                    if (aggregateRepositoryInterface.IsGenericType &&
                        !aggregateRepositoryInterface.IsGenericTypeDefinition &&
                        genericArguments.Any(x => x.IsGenericParameter))
                    {
                        // ����������� ��������� generic-���������� � ������� ��� �� �������� generic-���
                        // � ���� ������ � ���� T �������� IsGenericParameter = true
                        // ��������, IDeleteGenericEntityService<LegalPerson> <-> DeleteGenericEntityService<T>
                        // ������, � ����������� �� ����������� �� T ������������ ��� � secure, ���� unsecure scope; 

                        var genericTypeDefinition = aggregateRepositoryInterface.GetGenericTypeDefinition();
                        var mapping = aggregateRepositoryType.GetGenericArguments()
                                                             .Any(x => x.IsEntity() && x.IsSecurableAccessRequired())
                                          ? Mapping.SecureOperationRepositoriesScope
                                          : Mapping.UnsecureOperationRepositoriesScope;
                        
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
                        var mapping = aggregateRepositoryInterface.GetGenericArguments()
                                                                  .Any(x => x.IsEntity() && x.IsSecurableAccessRequired())
                                          ? Mapping.SecureOperationRepositoriesScope
                                          : Mapping.UnsecureOperationRepositoriesScope;
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
                    readModel.GetInterfaces()
                             .Where(t => t.IsAggregateReadModel()
                                         && !ModelIndicators.Aggregates.Group.ReadOnly.Contains(t.IsGenericType ? t.GetGenericTypeDefinition() : t))
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
                    var genericArguments = readModelInterface.GetGenericArguments();
                    if (readModelInterface.IsGenericType &&
                        !readModelInterface.IsGenericTypeDefinition &&
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