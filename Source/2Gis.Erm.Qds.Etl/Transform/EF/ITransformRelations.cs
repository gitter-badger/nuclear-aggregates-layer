using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public interface ITransformRelations
    {
        // FIXME {m.pashuk, 24.04.2014}: Зачем надо было менять сигнатуру этого методв? Он выглядит не удобным.
        bool TryGetRelatedDocTypes(Type entityType, out IEnumerable<Type> docTypes);
        void RegisterRelation(IDocRelation docRelation);
    }
}