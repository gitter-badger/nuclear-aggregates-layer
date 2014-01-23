using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.Etl.Transform.Docs;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public class ErmData : IData
    {
        public ErmData(IEnumerable<TypedEntitySet> data, ITrackState state)
        {
            State = state;
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            Data = data;
        }

        public IEnumerable<TypedEntitySet> Data { get; private set; }
        public ITrackState State { get; private set; }
    }
}