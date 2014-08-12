using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL
{
	/// <summary>
	/// Provides the repository with tightly coupled ones, e.g. a repo holding the references.
	/// </summary>
	public sealed class RelationRepository<TEntity> : IRelationalRepository<TEntity> where TEntity : class, IEntity
	{
		private readonly IRepository<TEntity> _repository;
		private readonly Dictionary<Type, object> _relatedRepositories = new Dictionary<Type, object>();

		// TODO {s.pomadin, 12.08.2014}: remove if the unity ctor resolving issue will be solved
		public RelationRepository(IRepository<TEntity> repository, IRepository relatedRepository)
			: this(repository, new [] {relatedRepository})
		{
		}

		public RelationRepository(IRepository<TEntity> repository, params IRepository[] relatedRepositories)
			: this(repository, relatedRepositories.AsEnumerable())
		{
		}

		public RelationRepository(IRepository<TEntity> repository, IEnumerable<IRepository> relatedRepositories)
		{
			if (repository == null)
			{
				throw new ArgumentNullException("repository");
			}

			_repository = repository;

			foreach (var relatedRepository in (relatedRepositories ?? Enumerable.Empty<IRepository>()))
			{
				if (relatedRepository == null)
				{
					throw new ArgumentException("A related repository is null.", "relatedRepositories");
				}

				// gets the repository contract to register on
				var contract = relatedRepository.GetType().GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRepository<>));
				if (contract == null)
				{
					throw new ArgumentException(string.Format("A related repository does not implement {0} contract.", typeof(IRepository<>).Name), "relatedRepositories");
				}

				_relatedRepositories.Add(contract, relatedRepository);
			}
		}

		public void Add(TEntity entity)
		{
			_repository.Add(entity);
		}

		public void AddRange(IEnumerable<TEntity> entities)
		{
			_repository.AddRange(entities);
		}

		public void Update(TEntity entity)
		{
			_repository.Update(entity);
		}

		public void Delete(TEntity entity)
		{
			_repository.Delete(entity);
		}

		public void DeleteRange(IEnumerable<TEntity> entities)
		{
			_repository.DeleteRange(entities);
		}

		public int Save()
		{
			return _repository.Save();
		}

		public IRepository<TRelatedEntity> GetRelatedRepository<TRelatedEntity>() where TRelatedEntity : class, IEntity
		{
			object repository;
			if (_relatedRepositories.TryGetValue(typeof(IRepository<TRelatedEntity>), out repository))
			{
				return (IRepository<TRelatedEntity>)repository;
			}
			return null;
		}
	}
}