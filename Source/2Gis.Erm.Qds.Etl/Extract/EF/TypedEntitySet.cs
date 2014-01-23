using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public class TypedEntitySet
    {
        public TypedEntitySet(Type entityType, IEnumerable<IEntityKey> entities)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            EntityType = entityType;
            Entities = entities;
        }

        public Type EntityType { get; private set; }
        public IEnumerable<IEntityKey> Entities { get; private set; }
    }
}