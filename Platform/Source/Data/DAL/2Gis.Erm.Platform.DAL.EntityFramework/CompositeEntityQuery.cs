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
    public class CompositeEntityQuery : ICompositeEntityQuery
    {
        private readonly IQuery _query;

        public CompositeEntityQuery(IQuery query)
        {
            _query = query;
        }

        public IQueryable<TEntity> For<TEntity>(FindSpecification<TEntity> findSpecification)
        {
            // TODO {s.pomadin, 06.08.2014}: consider how to query via dynamic expression building

            if (typeof(TEntity) == typeof(Appointment))
            {
                return Find<AppointmentBase, TEntity>(null, findSpecification);
            }
            if (typeof(TEntity) == typeof(AppointmentRegardingObject))
            {
                return Find<AppointmentReference, TEntity>(x => x.Reference == (int)AppointmentReferenceType.RegardingObject, findSpecification);
            }
            if (typeof(TEntity) == typeof(AppointmentAttendee))
            {
                return Find<AppointmentReference, TEntity>(x => x.Reference == (int)AppointmentReferenceType.RequiredAttendees, findSpecification);
            }
            if (typeof(TEntity) == typeof(AppointmentOrganizer))
            {
                return Find<AppointmentReference, TEntity>(x => x.Reference == (int)AppointmentReferenceType.Organizer, findSpecification);
            }

            if (typeof(TEntity) == typeof(Phonecall))
            {
                return Find<PhonecallBase, TEntity>(null, findSpecification);
            }
            if (typeof(TEntity) == typeof(PhonecallRegardingObject))
            {
                return Find<PhonecallReference, TEntity>(x => x.Reference == (int)PhonecallReferenceType.RegardingObject, findSpecification);
            }
            if (typeof(TEntity) == typeof(PhonecallRecipient))
            {
                return Find<PhonecallReference, TEntity>(x => x.Reference == (int)PhonecallReferenceType.Recipient, findSpecification);
            }


            if (typeof(TEntity) == typeof(Task))
            {
                return Find<TaskBase, TEntity>(null, findSpecification);
            }
            if (typeof(TEntity) == typeof(TaskRegardingObject))
            {
                return Find<TaskReference, TEntity>(x => x.Reference == (int)TaskReferenceType.RegardingObject, findSpecification);
            }

            if (typeof(TEntity) == typeof(Letter))
            {
                return Find<LetterBase, TEntity>(null, findSpecification);
            }
            if (typeof(TEntity) == typeof(LetterRegardingObject))
            {
                return Find<LetterReference, TEntity>(x => x.Reference == (int)LetterReferenceType.RegardingObject, findSpecification);
            }
            if (typeof(TEntity) == typeof(LetterSender))
            {
                return Find<LetterReference, TEntity>(x => x.Reference == (int)LetterReferenceType.Sender, findSpecification);
            }
            if (typeof(TEntity) == typeof(LetterRecipient))
            {
                return Find<LetterReference, TEntity>(x => x.Reference == (int)LetterReferenceType.Recipient, findSpecification);
            }

            throw new NotSupportedException("The requested mapping is not supported");
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
            Expression<Func<TPersistentEntity, bool>> preExpression,
            FindSpecification<TEntity> postSpecification)
            where TPersistentEntity : class, IEntity
        {
            CheckRegistration<TPersistentEntity, TEntity>();

            var persistentEntities = preExpression != null ? _query.For(new FindSpecification<TPersistentEntity>(preExpression)) : _query.For<TPersistentEntity>();

            var entities = persistentEntities.Project().To<TEntity>();

            if (postSpecification != null)
            {
                return entities.Where(postSpecification);
            }

            return entities;
        }
    }
}