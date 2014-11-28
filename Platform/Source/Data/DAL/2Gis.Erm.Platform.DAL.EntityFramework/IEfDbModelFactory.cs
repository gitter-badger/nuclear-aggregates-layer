using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public interface IEfDbModelFactory
    {
        DbCompiledModel Create(string entityContainerName, DbConnection connection);
    }
}