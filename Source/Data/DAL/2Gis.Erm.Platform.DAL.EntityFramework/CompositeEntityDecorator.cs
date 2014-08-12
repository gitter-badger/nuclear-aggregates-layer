using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper.QueryableExtensions;

using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
	public class CompositeEntityDecorator : ICompositeEntityDecorator
	{
		private readonly IFinderBase _finder;

		public CompositeEntityDecorator(IFinder finder)
		{
			_finder = finder;
		}

		public IEnumerable<TEntity> Find<TEntity>(params long[] ids)
		{
			// NOTE: mapping registry should be referenced to ensure the registration performed
			var sourceType = MappingRegistry.LookupType(typeof(TEntity));
			if (sourceType == null)
				throw new NotSupportedException("The entity type is not supported.");

			// // TODO {s.pomadin, 06.08.2014}: consider how to query via dynamic expression building
			if (typeof(TEntity) == typeof(Appointment))
				return FindAppointment(ids).Cast<TEntity>();
			if (typeof(TEntity) == typeof(Phonecall))
				return FindPhonecall(ids).Cast<TEntity>();
			if (typeof(TEntity) == typeof(Task))
				return FindTask(ids).Cast<TEntity>();

			throw new NotSupportedException("The requested mapping is not supported");
		}

		private IEnumerable<Appointment> FindAppointment(params long[] ids)
		{
			return _finder.FindAll<AppointmentBase>()
			              .Where(x => ids.Contains(x.Id))
			              .Project().To<Appointment>().AsEnumerable();
		}

		private IEnumerable<Phonecall> FindPhonecall(params long[] ids)
		{
			return _finder.FindAll<PhonecallBase>()
			              .Where(x => ids.Contains(x.Id))
			              .Project().To<Phonecall>().AsEnumerable();
		}

		private IEnumerable<Task> FindTask(params long[] ids)
		{
			return _finder.FindAll<TaskBase>()
			              .Where(x => ids.Contains(x.Id))
			              .Project().To<Task>().AsEnumerable();
		}
	}
}