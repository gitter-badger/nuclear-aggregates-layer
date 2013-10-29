namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Определяет стратегию, используемую при сохранении изменений в domain context.
    /// Например, если domain context используется репозиторием внутри UoWScope, 
    /// то сохранение изменений происходит не в момент вызова Save у репозитория, а при вызове Complete UoWScope - т.е. отложенное сохранение
    /// </summary>
    public interface IDomainContextSaveStrategy
    {
        bool IsSaveDeferred { get; }
    }
}
