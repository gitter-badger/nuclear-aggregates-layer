using System;
using System.Diagnostics.CodeAnalysis;
using System.Transactions;

using FluentAssertions;

using Machine.Specifications;

using NuClear.Storage.Core;
using NuClear.Storage.LinqToDB;

namespace Storage.LinqToDB.Tests
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedMember.Local
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
    class LinqToDBDomainContextSpecs : SqliteSpecsContext
    {
        class When_call_nongeneric_GetQueryableSource
        {
            static IReadableDomainContext _readableDomainContext;
            static Exception _exception;
            Establish context = () =>
                                    {
                                        _readableDomainContext = new LinqToDBDomainContext(DataConnection,
                                                                                           new TransactionOptions
                                                                                               {
                                                                                                   IsolationLevel = IsolationLevel.ReadCommitted,
                                                                                                   Timeout = TimeSpan.Zero
                                                                                               },
                                                                                           new NullPendingChangesHandlingStrategy());
                                    };

            Because of = () => _exception = Catch.Exception(() => _readableDomainContext.GetQueryableSource(typeof(Entity)));
            It should_not_throw_exceptions = () => _exception.Should().Be(null);
        }
    }
}
