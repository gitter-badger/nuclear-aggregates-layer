namespace NuClear.Storage.Core
{
    /// <summary>
    /// ��������� ������� ��� �������� domain context ���������� ������ ��� ������
    /// </summary>
    public interface IReadableDomainContextFactory
    {
        IReadableDomainContext Create(DomainContextMetadata domainContextMetadata);
    }
}