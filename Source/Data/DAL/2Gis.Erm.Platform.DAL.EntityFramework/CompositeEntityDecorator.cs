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
			// TODO {s.pomadin, 06.08.2014}: consider how to query via dynamic expression building
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
			CheckRegistration<AppointmentBase, Appointment>();
			return _finder.FindAll<AppointmentBase>()
			              .Where(x => ids.Contains(x.Id))
			              .Project().To<Appointment>().AsEnumerable();
		}

		private IEnumerable<Phonecall> FindPhonecall(params long[] ids)
		{
			CheckRegistration<PhonecallBase, Phonecall>();
			return _finder.FindAll<PhonecallBase>()
			              .Where(x => ids.Contains(x.Id))
			              .Project().To<Phonecall>().AsEnumerable();
		}

		private IEnumerable<Task> FindTask(params long[] ids)
		{
			CheckRegistration<TaskBase, Task>();
			return _finder.FindAll<TaskBase>()
			              .Where(x => ids.Contains(x.Id))
			              .Project().To<Task>().AsEnumerable();
		}

		private static void CheckRegistration<TSource,TTarget>()
		{
			// NOTE: mapping registry should be referenced to ensure the registration performed
			if (!MappingRegistry.CheckRegistration(typeof(TSource), typeof(TTarget)))
				throw new NotSupportedException("The mapping is not supported.");
		}
	}
}