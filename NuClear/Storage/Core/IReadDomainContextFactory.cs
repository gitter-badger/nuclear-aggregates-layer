namespace NuClear.Storage.Core
{
    /// <summary>
    /// ��������� ������� ��� �������� domain context ���������� ������ ��� ������
    /// </summary>
    public interface IReadDomainContextFactory
    {
        IReadDomainContext Create(DomainContextMetadata domainContextMetadata);
    }
}