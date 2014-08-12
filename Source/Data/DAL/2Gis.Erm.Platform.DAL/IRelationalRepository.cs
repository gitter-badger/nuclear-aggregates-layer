using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL
{
	/// <summary>
	/// Extends the repository contract to provide the related repositories if any.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public interface IRelationalRepository<in TEntity> : IRepository<TEntity> where TEntity : class, IEntity
	{
		/// <summary>
		/// Returns the related repository if it's registered, otherwise <c>null</c>.
		/// </summary>
		IRepository<TRelatedEntity> GetRelatedRepository<TRelatedEntity>() where TRelatedEntity : class, IEntity;
	}
}