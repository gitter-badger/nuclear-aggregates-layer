namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Определяет стратегию, используемую при сохранении изменений в domain context.
    /// Например, если domain context используется репозиторием внутри UoWScope, 
    /// то сохранение изменений происходит не в момент вызова Save у репозитория, а при вызове Complete UoWScope - т.е. отложенное сохранение
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