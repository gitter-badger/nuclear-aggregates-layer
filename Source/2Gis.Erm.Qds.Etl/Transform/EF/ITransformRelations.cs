using System;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public interface ITransformRelations
    {
        Type[] GetRelatedDocTypes(Type entityType);
    }
}