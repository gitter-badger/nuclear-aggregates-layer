using DoubleGis.Erm.Qds;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit
{
    public class TestDoc : IDoc
    {
        public TestDoc()
        {
        }

        public TestDoc(string id, string textValue, params NestedDoc[] nestedDocs)
        {
            Id = id;
            TextValue = textValue;
            NestedDocs = nestedDocs;
        }

        public string Id { get; set; }
        public string TextValue { get; set; }

        public NestedDoc[] NestedDocs { get; set; }
    }

    public class NestedDoc : IDoc
    {
        public string Id { get; set; }

        public MoreNestedDoc[] MoreNestedDocs { get; set; }
    }

    public class MoreNestedDoc : IDoc
    {
        public string Id { get; set; }
    }
}