using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public interface ITransformRelations
    {
        // FIXME {m.pashuk, 24.04.2014}: ����� ���� ���� ������ ��������� ����� ������? �� �������� �� �������.
        bool TryGetRelatedDocTypes(Type entityType, out IEnumerable<Type> docTypes);
        void RegisterRelation(IDocRelation docRelation);
    }
}