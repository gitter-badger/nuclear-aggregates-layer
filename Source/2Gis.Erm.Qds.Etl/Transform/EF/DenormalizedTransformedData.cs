using System;
using System.Collections;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public class DenormalizedTransformedData : ITransformedData
    {
        public DenormalizedTransformedData(IEnumerable<IDoc> documents, ITrackState state)
        {
            if (documents == null)
            {
                throw new ArgumentNullException("documents");
            }

            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            Documents = documents;
            State = state;
        }

        public IEnumerable<IDoc> Documents { get; private set; }
        public ITrackState State { get; private set; }
    }
}