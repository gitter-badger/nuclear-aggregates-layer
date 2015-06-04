namespace NuClear.Storage.Core
{
    public interface IDomainContextsScope : IDomainContextHost
    {
         void Complete();
    }
}