using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public class EntityLinksDataSource : IDataSource
    {
        public EntityLinksDataSource(IEnumerable<EntityLink> links, ITrackState state)
        {
            if (links == null)
            {
                throw new ArgumentNullException("links");
            }

            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            Links = links;
            State = state;
        }

        public IEnumerable<EntityLink> Links { get; private set; }
        public ITrackState State { get; private set; }
    }
}