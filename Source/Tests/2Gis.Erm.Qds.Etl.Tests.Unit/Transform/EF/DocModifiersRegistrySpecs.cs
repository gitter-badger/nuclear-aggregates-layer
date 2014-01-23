using System;

using DoubleGis.Erm.Qds.Etl.Transform.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF
{
    public class DocModifiersRegistrySpecs
    {
        [Subject(typeof(DictionaryDocModifiersRegistry))]
        public class When_get_modifier_by_doc_type
        {
            Establish context = () =>
                {
                    Target = new DictionaryDocModifiersRegistry();

                    _docType = typeof(TestDoc);
                    var mockModifier = new Mock<IDocsSelector>();
                    mockModifier.SetupGet(m => m.SupportedDocType).Returns(_docType);

                    _expectedModifier = mockModifier.Object;
                    Target.AddModifier(_expectedModifier);
                };

            Because of = () => Result = Target.GetModifier(_docType);

            It should_return_registred_modifier = () => Result.Should().Be(_expectedModifier);

            private static Type _docType;
            private static IDocsSelector _expectedModifier;

            public static IDocsSelector Result { get; set; }
            protected static DictionaryDocModifiersRegistry Target { get; set; }
        }
    }
}