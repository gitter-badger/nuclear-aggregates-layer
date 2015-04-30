using System;

namespace NuClear.Storage.Core
{
    /// <summary>
    /// ��������� ��������� ���������������� ���������� ���������-���� ��� ������������� � ������ �������� ����������� domain context (������� ����� ����, ��������: UoW, UoWScope)
    /// </summary>
    public interface IDomainContextHost : IDisposable
    {
        Guid ScopeId { get; }
    }
}