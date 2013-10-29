using System;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// ��������� ��������� ���������������� ���������� ���������-���� ��� ������������� � ������ �������� ����������� domain context (������� ����� ����, ��������: UoW, UoWScope)
    /// </summary>
    public interface IDomainContextHost
    {
        Guid ScopeId { get; }
    }
}