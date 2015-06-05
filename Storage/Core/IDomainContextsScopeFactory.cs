namespace NuClear.Storage.Core
{
    public interface IDomainContextsScopeFactory
    {
        IDomainContextsScope CreateScope();
    }
}