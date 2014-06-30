using System;

using DoubleGis.Erm.Qds;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public class OrObjectQuery : IDocsQuery
    {
        public OrObjectQuery(IDocsQuery left, IDocsQuery right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            Right = right;
            Left = left;
        }

        public IDocsQuery Left { get; private set; }
        public IDocsQuery Right { get; private set; }
    }
}