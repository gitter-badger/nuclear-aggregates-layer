using System.Data.Entity;

namespace NuClear.Storage.EntityFramework
{
    public interface IEFDbModelConvention
    {
        string ContainerName { get; }
        void Apply(DbModelBuilder builder);
    }
}