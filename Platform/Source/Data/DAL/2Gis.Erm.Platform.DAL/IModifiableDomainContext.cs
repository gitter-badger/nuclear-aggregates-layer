namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Абстракция для domaincontext поддерживающего модификацию данных, а следовательно, для него необходимы методы сохранения изменений и т.д.
    /// </summary>
    public interface IModifiableDomainContext : IDomainContext, IPendingChangesMonitorable
    {
        int SaveChanges();
    }
}
