using System;
using System.Linq;
using System.Linq.Expressions;

using AutoMapper.QueryableExtensions;

using DoubleGis.Erm.Platform.DAL.EntityFramework.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
	public class CompositeEntityDecorator : ICompositeEntityDecorator
	{
        private readonly IQuery _query;

		public CompositeEntityDecorator(IQuery query)
		{
            _query = query;
		}

        public IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression)
		{
			// TODO {s.pomadin, 06.08.2014}: consider how to query via dynamic expression building

			if (typeof(TEntity) == typeof(Appointment))
            {
                return Find<AppointmentBase, TEntity>(expression, null);
            }
            if (typeof(TEntity) == typeof(AppointmentRegardingObject))
            {
                return Find<AppointmentReference, TEntity>(expression, x => x.Reference == (int)AppointmentReferenceType.RegardingObject);
            }
            if (typeof(TEntity) == typeof(AppointmentAttendee))
            {
                return Find<AppointmentReference, TEntity>(expression, x => x.Reference == (int)AppointmentReferenceType.RequiredAttendees);
            }
            if (typeof(TEntity) == typeof(AppointmentOrganizer))
            {
                return Find<AppointmentReference, TEntity>(expression, x => x.Reference == (int)AppointmentReferenceType.Organizer);
            }
			
            if (typeof(TEntity) == typeof(Phonecall))
            {
                return Find<PhonecallBase, TEntity>(expression, null);
            }
            if (typeof(TEntity) == typeof(PhonecallRegardingObject))
            {
                return Find<PhonecallReference, TEntity>(expression, x => x.Reference == (int)PhonecallReferenceType.RegardingObject);
            }
            if (typeof(TEntity) == typeof(PhonecallRecipient))
            {
                return Find<PhonecallReference, TEntity>(expression, x => x.Reference == (int)PhonecallReferenceType.Recipient);
            }
			
            
            if (typeof(TEntity) == typeof(Task))
            {
                return Find<TaskBase, TEntity>(expression, null);
            }
            if (typeof(TEntity) == typeof(TaskRegardingObject))
            {
                return Find<TaskReference, TEntity>(expression, x => x.Reference == (int)TaskReferenceType.RegardingObject);
            }

            
            if (typeof(TEntity) == typeof(Letter))
            {
                return Find<LetterBase, TEntity>(expression, null);
            }
            if (typeof(TEntity) == typeof(LetterRegardingObject))
            {
                return Find<LetterReference, TEntity>(expression, x => x.Reference == (int)LetterReferenceType.RegardingObject);
            }
            if (typeof(TEntity) == typeof(LetterSender))
            {
                return Find<LetterReference, TEntity>(expression, x => x.Reference == (int)LetterReferenceType.Sender);
            }
            if (typeof(TEntity) == typeof(LetterRecipient))
            {
                return Find<LetterReference, TEntity>(expression, x => x.Reference == (int)LetterReferenceType.Recipient);
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
			
			var persistentEntities = _query.For<TPersistentEntity>();
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