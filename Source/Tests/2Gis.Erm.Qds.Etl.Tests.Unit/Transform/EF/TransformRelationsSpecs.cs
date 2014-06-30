using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.Etl.Transform.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF
{
    public class TransformRelationsSpecs
    {
        // TODO {f.zaharov}: Test Расширение с дублированием мапинга

        [Subject(typeof(TransformRelations))]
        class When_get_related_doc_types_for_not_supported_entity_type : TransformRelationsContext
        {
            Because of = () =>
            {
                IEnumerable<Type> docTypes;
                Result = Target.TryGetRelatedDocTypes(Mock.Of<Type>(), out docTypes);
            };

            It should_return_false = () => Result.ShouldBeEquivalentTo(false);

            static bool Result;
        }

        [Subject(typeof(TransformRelations))]
        class When_one_to_many_mapping_registred : TransformRelationsContext
        {
            Establish context = () =>
            {
                _expectedDocType = typeof(TestDoc);
                _expectedAnotherDocType = typeof(AnotherTestDoc);

                _entityType = typeof(TestEntity);

                Target.RegisterRelation(new DocRelation(_expectedDocType, new Link(_entityType, Mock.Of<IDocsQueryBuilder>())));
                Target.RegisterRelation(new DocRelation(_expectedAnotherDocType, new Link(_entityType, Mock.Of<IDocsQueryBuilder>())));
            };

            Because of = () => Target.TryGetRelatedDocTypes(_entityType, out Result);

            It should_return_all_mapped_doc_types = () =>
                Result.Should().Contain(new[] { _expectedDocType, _expectedAnotherDocType });

            static Type _expectedDocType;
            static Type _entityType;
            static Type _expectedAnotherDocType;

            static IEnumerable<Type> Result;
        }

        [Subject(typeof(TransformRelations))]
        class When_mapping_registred : TransformRelationsContext
        {
            Establish context = () =>
                {
                    _expectedDocType = typeof(TestDoc);
                    _entityType = typeof(TestEntity);
                    _anotherEntityType = typeof(AnotherTestEntity);

                    DocRelation = new DocRelation(_expectedDocType, new Link(_entityType, Mock.Of<IDocsQueryBuilder>()), new Link(_anotherEntityType, Mock.Of<IDocsQueryBuilder>()));
                };

            Because of = () => Target.RegisterRelation(DocRelation);

            It should_return_mapped_doc_type_by_entity_type = () =>
                {
                    IEnumerable<Type> docTypes;
                    Target.TryGetRelatedDocTypes(_entityType, out docTypes);
                    docTypes.Should().OnlyContain(t => t == _expectedDocType);
                };

            It should_return_mapped_doc_type_by_another_entity_type = () =>
                {
                    IEnumerable<Type> docTypes;
                    Target.TryGetRelatedDocTypes(_anotherEntityType, out docTypes);
                    docTypes.Should().OnlyContain(t => t == _expectedDocType);
                };

            private static Type _expectedDocType;
            private static Type _entityType;
            private static Type _anotherEntityType;
            private static DocRelation DocRelation;
        }

        class TransformRelationsContext
        {
            Establish context = () => Target = new TransformRelations();

            protected static TransformRelations Target { get; private set; }
        }
    }
}