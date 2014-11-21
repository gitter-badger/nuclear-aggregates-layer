using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public interface IExportMetadataProvider
    {
        QueryRuleContainer<TEntity> GetMetadata<TEntity>() where TEntity : class, IEntity, IEntityKey;
    }
}
