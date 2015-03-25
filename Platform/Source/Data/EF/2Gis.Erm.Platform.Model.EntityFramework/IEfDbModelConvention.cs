using System.Data.Entity;

namespace DoubleGis.Erm.Platform.Model.EntityFramework
{
    public interface IEfDbModelConvention
    {
        string ContainerName { get; }
        void Apply(DbModelBuilder builder);
    }
}