using System;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Aggregates
{
    /// <summary>
    /// ��������� ��������� ��� ������������ (��������������, �������������) ������������ ������ ��������� �������� ��������
    /// �.�. ��������� ���� ��������� ������ ���������� ������������ ����� ��������� �������� �������� ��� �����-�� ���������� �������� - ��������� ����� ��������
    /// </summary>
    [Obsolete("Use non-generic interface marked with IAggregateSpecificOperation")]
    public interface IAggregatePartRepository<TAggregateRoot> : IAggregateRepository
        where TAggregateRoot : class, IEntity, IEntityKey
    {
    }
}