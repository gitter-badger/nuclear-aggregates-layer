namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// ���������� ���������, ������������ ��� ���������� ��������� � domain context.
    /// ��������, ���� domain context ������������ ������������ ������ UoWScope, 
    /// �� ���������� ��������� ���������� �� � ������ ������ Save � �����������, � ��� ������ Complete UoWScope - �.�. ���������� ����������
    /// </summary>
    public sealed class DomainContextSaveStrategy : IDomainContextSaveStrategy
    {
        private readonly bool _isSaveDeferred;

        public DomainContextSaveStrategy(bool isSaveDeferred)
        {
            _isSaveDeferred = isSaveDeferred;
        }

        #region Implementation of IDomainContextSaveStrategy

        public bool IsSaveDeferred
        {
            get { return _isSaveDeferred; }
        }

        #endregion
    }
}