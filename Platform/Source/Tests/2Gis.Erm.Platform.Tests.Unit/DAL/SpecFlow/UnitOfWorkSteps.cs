using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.EntityTypes;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories;

using Moq;

using Nuclear.Tracing.API;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.SpecFlow
{
    [Binding]
    [Scope(Tag = "DAL")]
    public class UnitOfWorkSteps
    {
        private StubUnitOfWork _stubUnitOfWork;
        private MockUnitOfWork _mockUnitOfWork;

        private IReadDomainContextProvider _readDomainContextProvider;
        private IModifiableDomainContextProvider _modifiableDomainContextProvider;
        
        private TestDelegate _thenAction;

        #region Given

        [Scope(Tag = "StubUnitOfWork")]
        [Given(@"create instance of StubUnitOfWork class")]
        public void GivenCreateInstanceOfStubUnitOfWorkClass()
        {
            var factories = new Dictionary<Type, Func<IReadDomainContextProvider, IModifiableDomainContextProvider, IAggregateRepository>>
                {
                    {
                        typeof(ConcreteAggregateRepository),
                        (readContextProvider, modifiableContextProvider) =>
                        new ConcreteAggregateRepository(new StubFinder(readContextProvider),
                                                        new StubEntityRepository<ErmScopeEntity1>(readContextProvider, modifiableContextProvider),
                                                        new StubEntityRepository<ErmScopeEntity2>(readContextProvider, modifiableContextProvider))
                    }
                };
            _stubUnitOfWork = new StubUnitOfWork(factories,
                                                 new StubDomainContext(),
                                                 new StubDomainContextFactory(),
                                                 new NullPendingChangesHandlingStrategy(),
                                                 new NullLogger());
        }

        [Given(@"create instance of MockUnitOfWork class")]
        public void GivenCreateInstanceOfMockUnitOfWorkClass()
        {
            _mockUnitOfWork = new MockUnitOfWork(
                (x, y) =>
                    {
                        _readDomainContextProvider = x;
                        _modifiableDomainContextProvider = y;
                    },
                Mock.Of<IReadDomainContext>(),
                Mock.Of<IModifiableDomainContextFactory>(),
                Mock.Of<IPendingChangesHandlingStrategy>(),
                Mock.Of<ITracer>());
        }

        #endregion

        #region When

        [Scope(Tag = "StubUnitOfWork")]
        [When(@"call CreateRepository with concrete aggregate repository type as a parameter using test action")]
        public void WhenCallCreateRepositoryWithConcreteAggregateRepositoryTypeAsAParameterUsingTestAction()
        {
            var aggregatesLayerRuntimeFactory = _stubUnitOfWork as IAggregatesLayerRuntimeFactory;
            _thenAction = () => aggregatesLayerRuntimeFactory.CreateRepository(typeof(ConcreteAggregateRepository));
        }

        [Scope(Tag = "StubUnitOfWork")]
        [When(@"call CreateRepository with incorrect type as a parameter using test action")]
        public void WhenCallCreateRepositoryWithIncorrectTypeAsAParameterUsingTestAction()
        {
            var aggregatesLayerRuntimeFactory = _stubUnitOfWork as IAggregatesLayerRuntimeFactory;
            _thenAction = () => aggregatesLayerRuntimeFactory.CreateRepository(typeof(StubFinder));
        }

        [Scope(Tag = "MockUnitOfWork")]
        [When(@"call CreateRepository with aggregate repository interface type as a parameter")]
        public void WhenCallCreateRepositoryWithAggregateRepositoryInterfaceTypeAsAParameter()
        {
            _mockUnitOfWork.CreateRepository<IStubSimpleAggregateRepository>();
        }

        #endregion

        #region Then

        [Scope(Tag = "UnityUnitOfWork")]
        [Then(@"UnityUnitOfWork instance should not be null")]
        public void ThenUnityUnitOfWorkInstanceShouldNotBeNull()
        {
            Assert.That(_stubUnitOfWork, Is.Not.Null);
        }

        [Scope(Tag = "UnityUnitOfWork")]
        [Then(@"ScopeId should not be Guid\.Empty")]
        public void ThenScopeIdShouldNotBeGuid_Empty()
        {
            Assert.That(((IDomainContextHost)_stubUnitOfWork).ScopeId, Is.Not.EqualTo(Guid.Empty));
        }
        
        [Then(@"exception shoud not be thrown")]
        public void ThenExceptionShoudNotBeThrown()
        {
            Assert.That(_thenAction, Throws.Nothing); 
        }

        [Then(@"ArgumentException shoud be thrown")]
        public void ThenArgumentExceptionShoudBeThrown()
        {
            Assert.That(_thenAction, Throws.Exception.TypeOf<ArgumentException>());
        }

        [Then(@"passed parameters are instances of expected classes")]
        public void ThenPassedParametersAreInstancesOfExpectedClasses()
        {
            Assert.That(_readDomainContextProvider, Is.InstanceOf<ReadDomainContextProviderProxy>());
            Assert.That(_modifiableDomainContextProvider, Is.InstanceOf<ModifiableDomainContextProviderProxy>());
        }

        #endregion
    }
}
