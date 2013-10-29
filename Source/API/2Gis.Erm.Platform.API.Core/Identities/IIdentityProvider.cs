using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.API.Core.Identities
{
    /// <summary>
    /// Обеспечивает присвоение корректных id для указанных экземпляров сущностей, использую сервис генерации идентификаторов
    /// Т.о. фактически это accessor для сервиса генерации ID
    /// </summary>
    public interface IIdentityProvider
    {
        void SetFor<TEntity>(params TEntity[] entities) where TEntity : class, IEntityKey;
    }
}