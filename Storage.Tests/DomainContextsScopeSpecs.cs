using System;

using FluentAssertions;

using Machine.Specifications;

using NuClear.Storage.Core;

using Storage.Tests.Fakes;

namespace Storage.Tests
{
    public class DomainContextsScopeSpecs
    {
        class When_call_Complete_on_disposed_object 
        {
            static ScopedDomainContextsStore _scopedDomainContextsStore;
            static IDomainContextsScope _domainContextsScope;
            static Exception _exception;
            
            Establish context = () =>
            {
                _scopedDomainContextsStore = new ScopedDomainContextsStore(new StubDomainContext(), new StubDomainContextFactory());
                _domainContextsScope = new DomainContextsScope(_scopedDomainContextsStore, new NullPendingChangesHandlingStrategy());
                _domainContextsScope.Dispose();
            };

            Because of = () => _exception = Catch.Exception(() => _domainContextsScope.Complete());

            It exception_of_type_ObjectDisposedException_should_be_thrown = () => _exception.Should().BeOfType<ObjectDisposedException>();
        }

        class When_Complete_is_called_in_empty_scope
        {
            static ScopedDomainContextsStore _scopedDomainContextsStore;
            static IDomainContextsScope _domainContextsScope;
            static Exception _exception;

            Establish context = () =>
            {
                _scopedDomainContextsStore = new ScopedDomainContextsStore(new StubDomainContext(), new StubDomainContextFactory());
                _domainContextsScope = new DomainContextsScope(_scopedDomainContextsStore, new NullPendingChangesHandlingStrategy());
            };

            Because of = () => _exception = Catch.Exception(() => _domainContextsScope.Complete());

            It exception_should_not_be_thrown = () => _exception.Should().Be(null);
        }

        class When_Disposing_twice
        {
            static ScopedDomainContextsStore _scopedDomainContextsStore;
            static IDomainContextsScope _domainContextsScope;
            static Exception _exception;

            Establish context = () =>
            {
                _scopedDomainContextsStore = new ScopedDomainContextsStore(new StubDomainContext(), new StubDomainContextFactory());
                _domainContextsScope = new DomainContextsScope(_scopedDomainContextsStore, new NullPendingChangesHandlingStrategy());
            };

            Because of = () => _exception = Catch.Exception(() =>
            {
                _domainContextsScope.Dispose();
                _domainContextsScope.Dispose();
            });

            It exception_should_not_be_thrown = () => _exception.Should().BeNull();
        } 
    }
}