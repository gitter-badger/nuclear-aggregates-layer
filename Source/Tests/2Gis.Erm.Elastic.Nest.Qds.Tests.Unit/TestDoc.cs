using DoubleGis.Erm.Qds;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit
{
    public class TestDoc : IDoc
    {
        public TestDoc()
        {
        }

        public TestDoc(long id, string textValue)
        {
            Id = id;
            TextValue = textValue;
        }

        public long Id { get; set; }
        public string TextValue { get; set; }
    }
}