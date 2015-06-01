using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Версия IRepository с проверкой безопасности.
    /// </summary>
    public interface ISecureRepository<in TEntity> : IRepository<TEntity> where TEntity : class, IEntity, ICuratedEntity
    {
    }
}