using System;

using FluentAssertions;

using Machine.Specifications;

using NuClear.Storage.Core;

using Storage.Tests.Fakes;

namespace Storage.Tests
{
    public class DomainContextHostSpecs
    {
        [Tags("Storage")]
        [Subject(typeof(DomainContextHost))]
        class When_created
        {
            static IDomainContextHost DomainContextHost;

            private Establish context = () => DomainContextHost = new DomainContextHost(
                                                                      new ScopedDomainContextsStore(new StubDomainContext(), new StubDomainContextFactory()),
                                                                      new NullPendingChangesHandlingStrategy());

            Because of = () => { };
            It should_set_ScopeId = () => DomainContextHost.ScopeId.Should().NotBe(Guid.Empty);
        }
    }
}