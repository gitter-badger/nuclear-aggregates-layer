using System;

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
            Because of = () => Result = Target.GetRelatedDocTypes(Mock.Of<Type>());

            It should_return_empty_array = () => Result.Should().BeEmpty();

            static Type[] Result;
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

            Because of = () => Result = Target.GetRelatedDocTypes(_entityType);

            It should_return_all_mapped_doc_types = () =>
                Result.Should().Contain(new[] { _expectedDocType, _expectedAnotherDocType });

            static Type _expectedDocType;
            static Type _entityType;
            static Type _expectedAnotherDocType;

            static Type[] Result { get; set; }
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
                Target.GetRelatedDocTypes(_entityType).Should().OnlyContain(t => t == _expectedDocType);

            It should_return_mapped_doc_type_by_another_entity_type = () =>
                Target.GetRelatedDocTypes(_anotherEntityType).Should().OnlyContain(t => t == _expectedDocType);

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