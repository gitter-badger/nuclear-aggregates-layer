using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Machine.Specifications;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Specifications;

namespace Storage.Tests
{
    public static class FindSpecificationParserSpecs
    {
        #region Environment
        public interface IFindSpecificationFactory<T>
        {
            IFindSpecification<T> FindSpecification { get; }
            long ExpectedValue { get; }
        }

        public class BarFindSpecificationFactory : IFindSpecificationFactory<Bar>
        {
            public IFindSpecification<Bar> FindSpecification
            {
                get { return new FindSpecification<Bar>(x => x.Id == 5); }
            }

            public long ExpectedValue { get; private set; }
        }

        public class WithoutIdComparisonFindSpecificationFactory : IFindSpecificationFactory<Foo>
        {
            public IFindSpecification<Foo> FindSpecification
            {
                get { return new FindSpecification<Foo>(x => x == null); }
            }

            public long ExpectedValue { get; private set; }
        }

        public class WithoutEqualityFindSpecificationFactory : IFindSpecificationFactory<Foo>
        {
            public IFindSpecification<Foo> FindSpecification
            {
                get { return new FindSpecification<Foo>(x => x.Id != 5); }
            }

            public long ExpectedValue { get; private set; }
        }

        public class ExcessOfStatementsFindSpecificationFactory : IFindSpecificationFactory<Foo>
        {
            public IFindSpecification<Foo> FindSpecification
            {
                get { return new FindSpecification<Foo>(x => x.Id == 5 && x is Foo); }
            }

            public long ExpectedValue { get; private set; }
        }

        public class ConstFindSpecificationFactory : IFindSpecificationFactory<Foo>
        {
            public IFindSpecification<Foo> FindSpecification
            {
                get { return new FindSpecification<Foo>(x => x.Id == 5); }
            }

            public long ExpectedValue
            {
                get { return 5; }
            }
        }

        public class SimpleVariableFindSpecificationFactory : IFindSpecificationFactory<Foo>
        {
            public IFindSpecification<Foo> FindSpecification
            {
                get
                {
                    var value = new Random().Next();
                    ExpectedValue = value;

                    return new FindSpecification<Foo>(x => x.Id == value);
                }
            }

            public long ExpectedValue { get; private set; }
        }

        public class Foo : IEntityKey
        {
            public long Id { get; set; }
        }

        public class Bar
        {
            public long Id { get; set; }
        } 
        #endregion

        public class ContextForSingleId<TExpressionFactory, T>
            where TExpressionFactory : IFindSpecificationFactory<T>, new()
        {
            protected static long ExpectedValue;
            protected static long ParsedValue;
            protected static Exception Exception;

            static IFindSpecification<T> FindSpecification;

            Establish context = () =>
                {
                    var factory = new TExpressionFactory();
                    FindSpecification = factory.FindSpecification;
                    ExpectedValue = factory.ExpectedValue;
                };

            Because of = () =>
                {
                    try
                    {
                        ParsedValue = FindSpecification.ExtractEntityId();
                    }
                    catch (Exception e)
                    {
                        Exception = e;
                    }
                };
        }

        public class When_use_expression_with_a_constant : ContextForSingleId<ConstFindSpecificationFactory, Foo>
        {
            It should_not_throw_exception = () => Exception.Should().BeNull();
            It should_parse_id = () => ParsedValue.Should().Be(ExpectedValue);
        }

        public class When_use_expression_with_a_simple_variable : ContextForSingleId<SimpleVariableFindSpecificationFactory, Foo>
        {
            It should_not_throw_exception = () => Exception.Should().BeNull();
            It should_parse_id = () => ParsedValue.Should().Be(ExpectedValue);
        }

        public class When_use_expression_with_excess_of_statements : ContextForSingleId<ExcessOfStatementsFindSpecificationFactory, Foo>
        {
            It should_throw_exception = () => Exception.Should().NotBeNull();
        }

        public class When_use_expression_without_equality : ContextForSingleId<WithoutEqualityFindSpecificationFactory, Foo>
        {
            It should_throw_exception = () => Exception.Should().NotBeNull();
        }

        public class When_use_expression_without_id_comparison : ContextForSingleId<WithoutIdComparisonFindSpecificationFactory, Foo>
        {
            It should_throw_exception = () => Exception.Should().NotBeNull();
        }

        public class When_use_wrong_type_in_expression : ContextForSingleId<BarFindSpecificationFactory, Bar>
        {
            It should_throw_exception = () => Exception.Should().NotBeNull();
        }

        public class When_parsing_many_ids
        {
            protected static IEnumerable<long> ExpectedValue;
            protected static IEnumerable<long> ParsedValue;

            static IFindSpecification<Foo> FindSpecification;

            Establish context = () =>
            {
                ExpectedValue = new long[] { 1, 2, 3 };
                FindSpecification = new FindSpecification<Foo>(f => new long[] { 1, 2, 3 }.Contains(f.Id));
            };

            Because of = () => ParsedValue = FindSpecification.ExtractEntityIds();

            It should_give_exprected_result = () => ParsedValue.ShouldAllBeEquivalentTo(ExpectedValue);
        }
    }
}
