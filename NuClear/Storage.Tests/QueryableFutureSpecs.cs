using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Machine.Specifications;

using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

namespace Storage.Tests
{
    public class QueryableFutureSpecs
    {
        class When_call_Project_using_queryable_projector_signature
        {
            static QueryableFutureSequence<string> _futureSequence;
            static Func<IQueryable<string>, IQueryable<int>> Projector;
            static Exception Exception;
            static IReadOnlyCollection<int> Result;

            private Establish context = () =>
                                            {
                                                _futureSequence = new QueryableFutureSequence<string>(new[] { "a", "aa", "aaa" }.AsQueryable());
                                                Projector = q => q.Select(x => x.Length);
                                            };
            Because of = () => Exception = Catch.Exception(() => Result = _futureSequence.Project(Projector).Many());
            It should_be_executed_without_exceptions = () => Exception.Should().Be(null);
            It should_return_correct_result = () =>
                                                  {
                                                      Result.Count.Should().Be(3);
                                                      var array = Result.ToArray();
                                                      array[0].Should().Be(1);
                                                      array[1].Should().Be(2);
                                                      array[2].Should().Be(3);
                                                  };
        }

        class When_call_Project_using_queryable_specification_signature
        {
            static QueryableFutureSequence<string> _futureSequence;
            static ProjectSpecification<IQueryable<string>, IQueryable<int>> Spec;
            static Exception Exception;
            static IReadOnlyCollection<int> Result;

            Establish context = () =>
                                    {
                                        _futureSequence = new QueryableFutureSequence<string>(new[] { "a", "aa", "aaa" }.AsQueryable());
                                        Spec = new ProjectSpecification<IQueryable<string>, IQueryable<int>>(q => q.Select(x => x.Length));
                                    };
            Because of = () => Exception = Catch.Exception(() => Result = _futureSequence.Project(Spec).Many());
            It should_be_executed_without_exceptions = () => Exception.Should().Be(null);
            It should_return_correct_result = () =>
                                                          {
                                                              Result.Count.Should().Be(3);
                                                              var array = Result.ToArray();
                                                              array[0].Should().Be(1);
                                                              array[1].Should().Be(2);
                                                              array[2].Should().Be(3);
                                                          };
        }
    }
}