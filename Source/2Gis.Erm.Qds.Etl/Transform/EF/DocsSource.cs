using System;
using System.Collections;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public class DocsSource
    {
        public DocsSource(Type documentType, IEnumerable documents)
        {
            if (documentType == null)
            {
                throw new ArgumentNullException("documentType");
            }

            if (documents == null)
            {
                throw new ArgumentNullException("documents");
            }

            DocumentType = documentType;
            Documents = documents;
        }

        public Type DocumentType { get; private set; }
        public IEnumerable Documents { get; private set; }
    }
}