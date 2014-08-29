using System;
using System.Linq;
using System.Linq.Expressions;

using AutoMapper.QueryableExtensions;

using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
	public class CompositeEntityDecorator : ICompositeEntityDecorator
	{
        private readonly IFinder _finder;

		public CompositeEntityDecorator(IFinder finder)
		{
			_finder = finder;
		}

        public IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression)
		{
			// TODO {s.pomadin, 06.08.2014}: consider how to query via dynamic expression building
			if (typeof(TEntity) == typeof(Appointment))
            {
                return Find<AppointmentBase, TEntity>(expression, null);
            }
			if (typeof(TEntity) == typeof(Phonecall))
            {
                return Find<PhonecallBase, TEntity>(expression, null);
            }
			if (typeof(TEntity) == typeof(Task))
            {
                return Find<TaskBase, TEntity>(expression, null);
            }
            if (typeof(TEntity) == typeof(Letter))
            {
                return Find<LetterBase, TEntity>(expression, null);
            }

			if (typeof(TEntity) == typeof(RegardingObject<Appointment>))
            {
                return Find<AppointmentReference, TEntity>(expression, x => x.Reference == (int)ReferenceType.RegardingObject);
            }
			if (typeof(TEntity) == typeof(RegardingObject<Phonecall>))
            {
                return Find<PhonecallReference, TEntity>(expression, x => x.Reference == (int)ReferenceType.RegardingObject);
            }
			if (typeof(TEntity) == typeof(RegardingObject<Task>))
            {
                return Find<TaskReference, TEntity>(expression, x => x.Reference == (int)ReferenceType.RegardingObject);
            }
            if (typeof(TEntity) == typeof(RegardingObject<Letter>))
            {
                return Find<LetterReference, TEntity>(expression, x => x.Reference == (int)ReferenceType.RegardingObject);
            }

			throw new NotSupportedException("The requested mapping is not supported");
		}

        public IQueryable<TEntity> Find<TEntity>(IFindSpecification<TEntity> findSpecification)
        {
            return Find(findSpecification.Predicate);
        }

        private static void CheckRegistration<TSource, TTarget>()
        {
            // NOTE: mapping registry should be referenced to ensure the registration performed
            if (!MappingRegistry.CheckRegistration(typeof(TSource), typeof(TTarget)))
            {
                throw new NotSupportedException("The mapping is not supported.");
            }
        }

        private IQueryable<TEntity> Find<TPersistentEntity, TEntity>(
            Expression<Func<TEntity, bool>> postPredicate,
            Expression<Func<TPersistentEntity, bool>> prePredicate)
			where TPersistentEntity : class, IEntity
		{
			CheckRegistration<TPersistentEntity, TEntity>();
			
			var persistentEntities = _finder.FindAll<TPersistentEntity>();
			if (prePredicate != null)
			{
				persistentEntities = persistentEntities.Where(prePredicate);
			}

			var entities = persistentEntities.Project().To<TEntity>();

			if (postPredicate != null)
			{
				entities = entities.Where(postPredicate);
			}
			
			return entities;
		}
	}
}