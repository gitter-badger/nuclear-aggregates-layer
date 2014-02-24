using System;

using DoubleGis.Erm.Qds.Etl.Transform.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF
{
    public class DictionaryDocUpdatersRegistrySpecs
    {
        [Subject(typeof(DictionaryDocUpdatersRegistry))]
        public class When_get_updater_by_doc_type
        {
            Establish context = () =>
                {
                    Target = new DictionaryDocUpdatersRegistry();

                    DocType = typeof(TestDoc);
                    var mockModifier = new Mock<IDocsUpdater>();
                    mockModifier.SetupGet(m => m.SupportedDocType).Returns(DocType);

                    ExpectedModifier = mockModifier.Object;
                    Target.AddUpdater(ExpectedModifier);
                };

            Because of = () => Result = Target.GetUpdater(DocType);

            It should_return_registred_modifier = () => Result.Should().Be(ExpectedModifier);

            private static Type DocType;
            private static IDocsUpdater ExpectedModifier;

            public static IDocsUpdater Result { get; set; }
            protected static DictionaryDocUpdatersRegistry Target { get; set; }
        }
    }
}