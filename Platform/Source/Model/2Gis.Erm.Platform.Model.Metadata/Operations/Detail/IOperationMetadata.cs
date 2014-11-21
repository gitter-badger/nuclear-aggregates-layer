using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail
{
    /// <summary>
    /// ��������� ��������� ��� ���������� �������� (�����)
    /// </summary>
    public interface IOperationMetadata
    {
    }

    /// <summary>
    /// ��������� ��������� ��� ���������� �����-�� ���������� ��������
    /// </summary>
    public interface IOperationMetadata<TOperationIdentity> : IOperationMetadata
        where TOperationIdentity : IOperationIdentity
    {
    }
}