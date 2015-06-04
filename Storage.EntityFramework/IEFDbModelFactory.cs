using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace NuClear.Storage.EntityFramework
{
    public interface IEFDbModelFactory
    {
        DbCompiledModel Create(string entityContainerName, DbConnection connection);
    }
}