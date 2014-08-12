using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
	internal sealed class MappingRegistry
	{
		private static readonly IDictionary<Type,Type> Registry = new Dictionary<Type, Type>
			{
				// BL -> DAL
				{typeof(Appointment), typeof(AppointmentBase)},
				{typeof(Phonecall), typeof(PhonecallBase)},
				{typeof(Task), typeof(TaskBase)},
				// DAL -> BL
				{typeof(AppointmentBase), typeof(Appointment)},
				{typeof(PhonecallBase), typeof(Phonecall)},
				{typeof(TaskBase), typeof(Task)},
			};

		static MappingRegistry()
		{
			RegisterMappingFromDal();
			RegisterMappingToDal();
		}

		private static void RegisterMappingFromDal()
		{
			Mapper.CreateMap<AppointmentBase, Appointment>()
				.ForMember(dto => dto.Header, x => x.MapFrom(t => t.Subject))
				.ForMember(dto => dto.Status, x => x.MapFrom(t => (ActivityStatus)t.Status))
				.ForMember(dto => dto.Priority, x => x.MapFrom(t => (ActivityPriority)t.Priority))
				.ForMember(dto => dto.Purpose, x => x.MapFrom(t => (ActivityPurpose)t.Purpose))
				.ForMember(t => t.RegardingObjects, x => x.MapFrom(t => t.AppointmentReferences
					.Select(link => new EntityToEntityReference
					{
						ReferenceType = (ReferenceType)link.Reference,
						SourceEntityName = EntityName.Appointment,
						SourceEntityId = link.AppointmentId,
						TargetEntityName = (EntityName)link.ReferencedType,
						TargetEntityId = link.ReferencedObjectId,
					})))
				;

			Mapper.CreateMap<PhonecallBase, Phonecall>()
				.ForMember(dto => dto.Header, x => x.MapFrom(t => t.Subject))
				.ForMember(dto => dto.Status, x => x.MapFrom(t => (ActivityStatus)t.Status))
				.ForMember(dto => dto.Priority, x => x.MapFrom(t => (ActivityPriority)t.Priority))
				.ForMember(dto => dto.Purpose, x => x.MapFrom(t => (ActivityPurpose)t.Purpose))
				.ForMember(t => t.RegardingObjects, x => x.MapFrom(t => t.PhonecallReferences
					.Select(link => new EntityToEntityReference
					{
						ReferenceType = (ReferenceType)link.Reference,
						SourceEntityName = EntityName.Phonecall,
						SourceEntityId = link.PhonecallId,
						TargetEntityName = (EntityName)link.ReferencedType,
						TargetEntityId = link.ReferencedObjectId,
					})))
				;

			Mapper.CreateMap<TaskBase, Task>()
				.ForMember(dto => dto.Header, x => x.MapFrom(t => t.Subject))
				.ForMember(dto => dto.Status, x => x.MapFrom(t => (ActivityStatus)t.Status))
				.ForMember(dto => dto.Priority, x => x.MapFrom(t => (ActivityPriority)t.Priority))
				.ForMember(dto => dto.TaskType, x => x.MapFrom(t => (TaskType)t.TaskType))
				.ForMember(t => t.RegardingObjects, x => x.MapFrom(t => t.TaskReferences
					.Select(link => new EntityToEntityReference
					{
						ReferenceType = (ReferenceType) link.Reference,
						SourceEntityName = EntityName.Task,
						SourceEntityId = link.TaskId,
						TargetEntityName = (EntityName) link.ReferencedType,
						TargetEntityId = link.ReferencedObjectId,
					})))
				;
		}

		private static void RegisterMappingToDal()
		{
			Mapper.CreateMap<Appointment, AppointmentBase>()
				.ForMember(dto => dto.Subject, x => x.MapFrom(t => t.Header))
				.ForMember(dto => dto.Status, x => x.MapFrom(t => (int)t.Status))
				.ForMember(dto => dto.Priority, x => x.MapFrom(t => (int)t.Priority))
				;

			Mapper.CreateMap<Phonecall, PhonecallBase>()
				.ForMember(dto => dto.Subject, x => x.MapFrom(t => t.Header))
				.ForMember(dto => dto.Status, x => x.MapFrom(t => (int)t.Status))
				.ForMember(dto => dto.Priority, x => x.MapFrom(t => (int)t.Priority))
				;

			Mapper.CreateMap<Task, TaskBase>()
				.ForMember(dto => dto.Subject, x => x.MapFrom(t => t.Header))
				.ForMember(dto => dto.Status, x => x.MapFrom(t => (int)t.Status))
				.ForMember(dto => dto.Priority, x => x.MapFrom(t => (int)t.Priority))
				.ForMember(dto => dto.TaskType, x => x.MapFrom(t => (int)t.TaskType))
				;

			Mapper.CreateMap<EntityToEntityReference, AppointmentReference>()
				.ForMember(dest => dest.Reference, cfg => cfg.MapFrom(src => (int) src.ReferenceType))
				.ForMember(dest => dest.AppointmentId, cfg => cfg.MapFrom(src => src.SourceEntityId))
				.ForMember(dest => dest.ReferencedType, cfg => cfg.MapFrom(src => (int) src.TargetEntityName))
				.ForMember(dest => dest.ReferencedObjectId, cfg => cfg.MapFrom(src => src.TargetEntityId))
				;
			
			Mapper.CreateMap<EntityToEntityReference, PhonecallReference>()
				.ForMember(dest => dest.Reference, cfg => cfg.MapFrom(src => (int) src.ReferenceType))
				.ForMember(dest => dest.PhonecallId, cfg => cfg.MapFrom(src => src.SourceEntityId))
				.ForMember(dest => dest.ReferencedType, cfg => cfg.MapFrom(src => (int) src.TargetEntityName))
				.ForMember(dest => dest.ReferencedObjectId, cfg => cfg.MapFrom(src => src.TargetEntityId))
				;
			
			Mapper.CreateMap<EntityToEntityReference, TaskReference>()
				.ForMember(dest => dest.Reference, cfg => cfg.MapFrom(src => (int) src.ReferenceType))
				.ForMember(dest => dest.TaskId, cfg => cfg.MapFrom(src => src.SourceEntityId))
				.ForMember(dest => dest.ReferencedType, cfg => cfg.MapFrom(src => (int) src.TargetEntityName))
				.ForMember(dest => dest.ReferencedObjectId, cfg => cfg.MapFrom(src => src.TargetEntityId))
				;
		}

		public static TOutput Map<TInput, TOutput>(TInput input)
		{
			if (/*!Registry.ContainsKey(typeof(TInput)) || */Mapper.FindTypeMapFor<TInput,TOutput>() == null)
				throw new InvalidOperationException("The requested mapping is not supported.");

			return Mapper.Map<TOutput>(input);
		}

		public static Type LookupType(Type targetType)
		{
			Type sourceType;
			return Registry.TryGetValue(targetType, out sourceType) ? sourceType : null;
		}
	}
}