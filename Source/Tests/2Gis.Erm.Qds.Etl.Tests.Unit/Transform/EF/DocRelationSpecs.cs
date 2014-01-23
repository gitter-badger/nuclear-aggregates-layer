using System;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF
{
    public class DocRelationSpecs
    {
        [Subject(typeof(DocRelation))]
        class When_get_by_part_query : DocRelationContext
        {
            Establish context = () =>
                {
                    TestEntity = new TestEntity();
                    ExpectedQuery = Mock.Of<IDocsQuery>();

                    var queryBuilder = new Mock<IDocsQueryBuilder>();
                    queryBuilder.Setup(q => q.CreateQuery(TestEntity)).Returns(ExpectedQuery);

                    CreateTarget(typeof(TestDoc), new Link(typeof(TestEntity), queryBuilder.Object));
                };

            Because of = () => Result = Target.GetByPartQuery(TestEntity);

            It should_return_query_for_part_type = () => Result.Should().Be(ExpectedQuery);

            static IDocsQuery Result;
            static IDocsQuery ExpectedQuery;
            static IEntityKey TestEntity;
        }

        [Subject(typeof(DocRelation))]
        class When_get_by_part_query_of_unsupported_part_type : DocRelationContext
        {
            Establish context = () => CreateTarget(typeof(TestDoc), typeof(TestEntity));

            Because of = () => Result = Catch.Exception(() => Target.GetByPartQuery(new AnotherTestEntity()));

            It should_throw_unsupported_exception = () => Result.Should().BeOfType<NotSupportedException>();

            static Exception Result;
        }

        [Subject(typeof(DocRelation))]
        class When_add_entity_type_as_generic : DocRelationContext
        {
            Establish context = () =>
                {
                    _initialDocType = typeof(TestDoc);
                    _initialEntityType = typeof(TestEntity);
                    _addedEntityType = typeof(AnotherTestEntity);

                    CreateTarget(_initialDocType, _initialEntityType);
                };

            Because of = () => Result = Target.LinkPart<AnotherTestEntity>(Mock.Of<IDocsQueryBuilder>());

            It should_return_same_instance = () => Result.Should().Be(Target);

            It should_contains_initial_doc_type = () => Result.GetDocType().Should().Be(_initialDocType);

            It should_contains_initial_and_added_entity_types = () =>
                Result.GetPartTypes().Should().Contain(new[] { _initialEntityType, _addedEntityType });

            static Type _initialDocType;
            static Type _addedEntityType;
            static Type _initialEntityType;
            static DocRelation Result;
        }

        [Subject(typeof(DocRelation))]
        class When_create_for_doc_type_and_part_types : DocRelationContext
        {
            Establish context = () =>
            {
                _expectedEntityType = typeof(TestEntity);
                _expectedAnotherEntityType = typeof(AnotherTestEntity);
            };

            Because of = () => Target = DocRelation.ForDoc<TestDoc>(new Link(_expectedEntityType, Mock.Of<IDocsQueryBuilder>()),
                    new Link(_expectedAnotherEntityType, Mock.Of<IDocsQueryBuilder>()));

            It should_return_doc_type = () =>
                Target.GetDocType().Should().Be(typeof(TestDoc));

            It should_return_part_types = () =>
                Target.GetPartTypes().Should().Contain(new[] { _expectedEntityType, _expectedAnotherEntityType });

            static Type _expectedEntityType;
            static Type _expectedAnotherEntityType;
        }

        [Subject(typeof(DocRelation))]
        class When_get_doc_type : DocRelationContext
        {
            Establish context = () =>
                {
                    _expectedDocType = typeof(TestDoc);
                    CreateTarget(_expectedDocType, new Link[0]);
                };

            Because of = () => Result = Target.GetDocType();

            It should_return_type_passed_to_constructor = () =>
                Result.Should().Be(_expectedDocType);

            static Type Result;
            static Type _expectedDocType;
        }

        [Subject(typeof(DocRelation))]
        class When_get_part_types : DocRelationContext
        {
            Establish context = () =>
            {
                _expectedEntityType = typeof(TestEntity);
                _expectedAnotherEntityType = typeof(AnotherTestEntity);

                CreateTarget(typeof(TestDoc), _expectedEntityType, _expectedAnotherEntityType);
            };

            Because of = () => Result = Target.GetPartTypes();

            It should_return_types_form_all_links = () =>
                Result.Should().Contain(new[] { _expectedEntityType, _expectedAnotherEntityType });

            static Type[] Result;
            static Type _expectedEntityType;
            static Type _expectedAnotherEntityType;
        }

        class DocRelationContext
        {
            protected static DocRelation Target { get; set; }

            protected static void CreateTarget(Type docType, params Type[] partTypes)
            {
                Target = new DocRelation(docType, partTypes.Select(t => new Link(t, Mock.Of<IDocsQueryBuilder>())).ToArray());
            }

            protected static void CreateTarget(Type docType, params Link[] links)
            {
                Target = new DocRelation(docType, links);
            }
        }
    }
}