namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// ��������� ������� ��� �������� domain context ���������� ������ ��� ������
    /// </summary>
    public interface IReadDomainContextFactory
    {
        IReadDomainContext Create(DomainContextMetadata domainContextMetadata);
    }
}