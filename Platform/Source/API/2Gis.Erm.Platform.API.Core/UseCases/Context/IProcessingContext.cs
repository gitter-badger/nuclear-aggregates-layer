namespace DoubleGis.Erm.Platform.API.Core.UseCases.Context
{
    /// <summary>
    /// ��������� ��������� ������, ��� � ��������������� ������� ������������ ��������
    /// </summary>
    public interface IProcessingContext
    {
        /// <summary>
        /// �������� �������� �� ����� � ���������
        /// </summary>
        /// <typeparam name="T">��� ��������</typeparam>
        /// <param name="key">Instance �����</param>
        /// <param name="throwIfNotExists">���� <c>true</c>, �� ������� exception ������ ��� �������� � ����� ������ � ��������� �� ����������������</param>
        T GetValue<T>(IContextKey<T> key, bool throwIfNotExists);

        /// <summary>
        /// �������� �������� �� ����� � ���������. Exception ������� �� �������, ���� ���� � ��������� ��� ����������� ������
        /// </summary>
        /// <typeparam name="T">��� ��������</typeparam>
        /// <param name="key">Instance �����</param>
        T GetValue<T>(IContextKey<T> key);

        /// <summary>
        /// ��������/�������� �������� � ���������
        /// </summary>
        /// <typeparam name="T">��� ��������</typeparam>
        /// <param name="key">Instance �����</param>
        void SetValue<T>(IContextKey<T> key, T value);

        /// <summary>
        /// ��������� ��������������� �� ������ ���� � ���������
        /// </summary>
        /// <typeparam name="T">��� ��������</typeparam>
        /// <param name="key">Instance �����</param>
        bool ContainsKey<T>(IContextKey<T> key);
    }
}